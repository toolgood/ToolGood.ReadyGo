using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpYaml.Serialization;
using System.Text.RegularExpressions;

namespace ToolGood.ReadyGo3.DataDiffer.YamlToJson
{
    /// <summary>
    /// This represents the JSON converter entity that takes care of the <c>x-yarm</c> property at the root level of the YAML document.
    /// </summary>
    public class IgnoreXYarmJsonConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);

            if (t.Type != JTokenType.Object) {
                t.WriteTo(writer);

                return;
            }

            JObject o = (JObject)t;

            var propsToRemove = o.Properties()
                                 .Where(x => IsPropertyNameXYarm(x.Name))
                                 .Select(x => x.Name)
                                 .ToList();

            foreach (var prop in propsToRemove) {
                o.Remove(prop);
            }

            o.WriteTo(writer);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        /// <inheritdoc />
        public override bool CanRead { get; } = false;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        private static bool IsPropertyNameXYarm(string name)
        {
            if (name.Equals("x-yarm", StringComparison.CurrentCultureIgnoreCase)) {
                return true;
            }

            if (name.StartsWith("x-yarm-", StringComparison.CurrentCultureIgnoreCase)) {
                return true;
            }

            return false;
        }
    }
}
