using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace ToolGood.ReadyGo3.DataDiffer.JsonDiffer
{
    public class JsonDifferentiator
    {
        private static TargetNode PointTargetNode(JToken diff, string property, ChangeMode mode)
        {
            string symbol = string.Empty;

            switch (mode) {
                case ChangeMode.Changed: symbol = $"*{property}"; break;
                case ChangeMode.Added: symbol = $"+{property}"; break;
                case ChangeMode.Removed: symbol = $"-{property}"; break;
            }
            return new TargetNode(symbol, null);
        }

        public static JToken Differentiate(JToken oldValue, JToken newValue)
        {
            if (JToken.DeepEquals(oldValue, newValue)) return null;

            if (oldValue != null && newValue != null && oldValue?.GetType() != newValue?.GetType())
                throw new InvalidOperationException($"Operands' types must match; '{oldValue.GetType().Name}' <> '{newValue.GetType().Name}'");

            var propertyNames = (oldValue?.Children() ?? default).Union(newValue?.Children() ?? default)?.Select(_ => (_ as JProperty)?.Name)?.Distinct().ToList();
            if (propertyNames.Contains("id")) { //id放在第一位比较好找
                propertyNames.Remove("id");
                propertyNames.Insert(0, "id");
            }

            if (!propertyNames.Any() && (oldValue is JValue || newValue is JValue)) {
                return (oldValue == null) ? newValue : oldValue;
            }

            var difference = JToken.Parse("{}");

            foreach (var property in propertyNames) {
                if (property == null) {
                    if (oldValue == null) {
                        difference = newValue;
                    }
                    // array of object?
                    else if (oldValue is JArray && oldValue.Children().All(c => !(c is JValue))) {
                        var difrences = new JArray();
                        var maximum = Math.Max(oldValue?.Count() ?? 0, newValue?.Count() ?? 0);

                        for (int i = 0; i < maximum; i++) {
                            var firstsItem = oldValue?.ElementAtOrDefault(i);
                            var secondsItem = newValue?.ElementAtOrDefault(i);

                            var diff = Differentiate(firstsItem, secondsItem);

                            if (diff != null) {
                                if (diff["*id"] != null || diff["+id"] != null || diff["-id"] != null || diff["id"] != null) {
                                } else if (firstsItem != null && firstsItem["id"] != null && firstsItem["id"].Type != JTokenType.Null) {
                                    var diff2 = new JObject();
                                    diff2["$id"] = firstsItem["id"];
                                    foreach (var (k, v) in (JObject)diff) {
                                        diff2[k] = v;
                                    }
                                    diff = diff2;
                                } else {
                                    var diff2 = new JObject();
                                    diff2["_"] = i + 1;
                                    foreach (var (k, v) in (JObject)diff) {
                                        diff2[k] = v;
                                    }
                                    diff = diff2;
                                }
                                difrences.Add(diff);
                            }
                        }

                        if (difrences.HasValues) {
                            difference = difrences;
                        }
                    } else {
                        difference = oldValue;
                    }

                    continue;
                }

                if (oldValue?[property] == null) {
                    var secondVal = newValue?[property]?.Parent as JProperty;

                    var targetNode = PointTargetNode(difference, property, ChangeMode.Added);

                    if (targetNode.Property != null) {
                        difference[targetNode.Symbol][targetNode.Property] = secondVal.Value;
                    } else
                        difference[targetNode.Symbol] = secondVal.Value;

                    continue;
                }

                if (newValue?[property] == null) {
                    var firstVal = oldValue?[property]?.Parent as JProperty;

                    var targetNode = PointTargetNode(difference, property, ChangeMode.Removed);

                    if (targetNode.Property != null) {
                        difference[targetNode.Symbol][targetNode.Property] = firstVal.Value;
                    } else
                        difference[targetNode.Symbol] = firstVal.Value;

                    continue;
                }

                if (oldValue?[property] is JValue value) {
                    if (!JToken.DeepEquals(oldValue?[property], newValue?[property])) {
                        var targetNode = PointTargetNode(difference, property, ChangeMode.Changed);

                        if (targetNode.Property != null) {
                            difference[targetNode.Symbol][targetNode.Property] = value + "->" + newValue?[property];
                        } else
                            difference[targetNode.Symbol] = value + "->" + newValue?[property];
                    }

                    continue;
                }

                if (oldValue?[property] is JObject) {

                    var targetNode = newValue?[property] == null
                        ? PointTargetNode(difference, property, ChangeMode.Removed)
                        : PointTargetNode(difference, property, ChangeMode.Changed);

                    var firstsItem = oldValue[property];
                    var secondsItem = newValue[property];

                    var diffrence = Differentiate(firstsItem, secondsItem);

                    if (diffrence != null) {

                        if (targetNode.Property != null) {
                            difference[targetNode.Symbol][targetNode.Property] = diffrence;
                        } else
                            difference[targetNode.Symbol] = diffrence;

                    }

                    continue;
                }

                if (oldValue?[property] is JArray) {
                    var difrences = new JArray();

                    var targetNode = newValue?[property] == null
                       ? PointTargetNode(difference, property, ChangeMode.Removed)
                       : PointTargetNode(difference, property, ChangeMode.Changed);

                    var maximum = Math.Max(oldValue?[property]?.Count() ?? 0, newValue?[property]?.Count() ?? 0);

                    for (int i = 0; i < maximum; i++) {
                        var firstsItem = oldValue[property]?.ElementAtOrDefault(i);
                        var secondsItem = newValue[property]?.ElementAtOrDefault(i);

                        var diff = Differentiate(firstsItem, secondsItem);

                        if (diff != null) {
                            difrences.Add(diff);
                        }
                    }

                    if (difrences.HasValues) {
                        if (targetNode.Property != null) {
                            difference[targetNode.Symbol][targetNode.Property] = difrences;
                        } else
                            difference[targetNode.Symbol] = difrences;
                    }

                    continue;
                }
            }

            return difference;
        }
    }

}
