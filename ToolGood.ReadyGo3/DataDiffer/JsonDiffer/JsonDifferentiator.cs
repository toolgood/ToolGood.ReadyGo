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

        public static JToken Differentiate(JToken newValue, JToken oldValue)
        {
            if (JToken.DeepEquals(newValue, oldValue)) return null;

            if (newValue != null && oldValue != null && newValue?.GetType() != oldValue?.GetType())
                throw new InvalidOperationException($"Operands' types must match; '{newValue.GetType().Name}' <> '{oldValue.GetType().Name}'");

            var propertyNames = (newValue?.Children() ?? default).Union(oldValue?.Children() ?? default)?.Select(_ => (_ as JProperty)?.Name)?.Distinct().ToList();
            if (propertyNames.Contains("id")) { //id放在第一位比较好找
                propertyNames.Remove("id");
                propertyNames.Insert(0, "id");
            }

            if (!propertyNames.Any() && (newValue is JValue || oldValue is JValue)) {
                return (newValue == null) ? oldValue : newValue;
            }

            var difference = JToken.Parse("{}");

            foreach (var property in propertyNames) {
                if (property == null) {
                    if (newValue == null) {
                        difference = oldValue;
                    }
                    // array of object?
                    else if (newValue is JArray && newValue.Children().All(c => !(c is JValue))) {
                        var difrences = new JArray();
                        var maximum = Math.Max(newValue?.Count() ?? 0, oldValue?.Count() ?? 0);

                        for (int i = 0; i < maximum; i++) {
                            var firstsItem = newValue?.ElementAtOrDefault(i);
                            var secondsItem = oldValue?.ElementAtOrDefault(i);

                            var diff = Differentiate(firstsItem, secondsItem);

                            if (diff != null) {
                                if (firstsItem["id"] != null) {
                                    if (diff["*id"] != null || diff["+id"] != null || diff["-id"] != null || diff["id"] != null) {
                                    } else {
                                        var diff2 = new JObject();
                                        diff2["$id"] = firstsItem["id"];
                                        foreach (var (k, v) in (JObject)diff) {
                                            diff2[k] = v;
                                        }
                                        diff = diff2;
                                    }
                                }
                                difrences.Add(diff);
                            }
                        }

                        if (difrences.HasValues) {
                            difference = difrences;
                        }
                    } else {
                        difference = newValue;
                    }

                    continue;
                }

                if (newValue?[property] == null) {
                    var secondVal = oldValue?[property]?.Parent as JProperty;

                    var targetNode = PointTargetNode(difference, property, ChangeMode.Added);

                    if (targetNode.Property != null) {
                        difference[targetNode.Symbol][targetNode.Property] = secondVal.Value;
                    } else
                        difference[targetNode.Symbol] = secondVal.Value;

                    continue;
                }

                if (oldValue?[property] == null) {
                    var firstVal = newValue?[property]?.Parent as JProperty;

                    var targetNode = PointTargetNode(difference, property, ChangeMode.Removed);

                    if (targetNode.Property != null) {
                        difference[targetNode.Symbol][targetNode.Property] = firstVal.Value;
                    } else
                        difference[targetNode.Symbol] = firstVal.Value;

                    continue;
                }

                if (newValue?[property] is JValue value) {
                    if (!JToken.DeepEquals(newValue?[property], oldValue?[property])) {
                        var targetNode = PointTargetNode(difference, property, ChangeMode.Changed);

                        if (targetNode.Property != null) {
                            difference[targetNode.Symbol][targetNode.Property] = value;
                        } else
                            difference[targetNode.Symbol] = value;
                        //difference["changed"][property] = showOriginalValues ? second?[property] : value;
                    }

                    continue;
                }

                if (newValue?[property] is JObject) {

                    var targetNode = oldValue?[property] == null
                        ? PointTargetNode(difference, property, ChangeMode.Removed)
                        : PointTargetNode(difference, property, ChangeMode.Changed);

                    var firstsItem = newValue[property];
                    var secondsItem = oldValue[property];

                    var diffrence = Differentiate(firstsItem, secondsItem);

                    if (diffrence != null) {

                        if (targetNode.Property != null) {
                            difference[targetNode.Symbol][targetNode.Property] = diffrence;
                        } else
                            difference[targetNode.Symbol] = diffrence;

                    }

                    continue;
                }

                if (newValue?[property] is JArray) {
                    var difrences = new JArray();

                    var targetNode = oldValue?[property] == null
                       ? PointTargetNode(difference, property, ChangeMode.Removed)
                       : PointTargetNode(difference, property, ChangeMode.Changed);

                    var maximum = Math.Max(newValue?[property]?.Count() ?? 0, oldValue?[property]?.Count() ?? 0);

                    for (int i = 0; i < maximum; i++) {
                        var firstsItem = newValue[property]?.ElementAtOrDefault(i);
                        var secondsItem = oldValue[property]?.ElementAtOrDefault(i);

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
