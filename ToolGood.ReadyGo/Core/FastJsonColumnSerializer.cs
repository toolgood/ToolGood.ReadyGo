using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 基于 System.Text.Json 的列序列化器，用于序列化/反序列化 POCO 中被标记为序列化的列。
    /// </summary>
    public class FastJsonColumnSerializer : IColumnSerializer
    {
        private static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions
        {
            WriteIndented = false
        };

        private static readonly JsonSerializerOptions DeserializeOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>
        /// 将对象序列化为 JSON 字符串。
        /// </summary>
        /// <param name="value">要序列化的对象。</param>
        /// <returns>序列化后的 JSON 字符串；若值为 null 则返回 null。</returns>
        public object Serialize(object value)
        {
            if (value == null) return null;
            return JsonSerializer.Serialize(value, value.GetType(), SerializeOptions);
        }

        /// <summary>
        /// 将 JSON 字符串反序列化为目标类型的对象。
        /// </summary>
        /// <param name="value">JSON 字符串。</param>
        /// <param name="targetType">目标类型。</param>
        /// <returns>反序列化后的对象；若值为 null 则返回 null。</returns>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null) return null;
            var json = value as string ?? value.ToString();
            var result = JsonSerializer.Deserialize(json, targetType, DeserializeOptions);
            return NormalizeJsonElements(result);
        }

        // 将 System.Text.Json 的 JsonElement 展平为 fastJSON 风格的 CLR 对象，
        // 保持 Dictionary<string,object>/List<object>/object 属性可直接强转的行为兼容。
        private static object NormalizeJsonElements(object value)
        {
            return NormalizeJsonElements(value, new HashSet<object>());
        }

        private static object NormalizeJsonElements(object value, HashSet<object> visited)
        {
            if (value == null) return null;
            if (value is JsonElement e) return JsonElementToClr(e);

            if (value is IDictionary<string, object> dict)
            {
                foreach (var key in dict.Keys.ToList())
                    dict[key] = NormalizeJsonElements(dict[key], visited);
                return dict;
            }

            if (value is IList list && !(value is byte[]))
            {
                for (var i = 0; i < list.Count; i++)
                    list[i] = NormalizeJsonElements(list[i], visited);
                return list;
            }

            // 具体 POCO：遍历其属性，展平可能出现的 JsonElement
            var type = value.GetType();
            if (type.IsClass && visited.Add(value))
            {
                foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (!prop.CanRead || prop.GetIndexParameters().Length > 0) continue;
                    var v = prop.GetValue(value);
                    if (v == null) continue;
                    if (v is JsonElement || v is IDictionary<string, object> || (v is IList && !(v is byte[])))
                    {
                        if (prop.CanWrite)
                            prop.SetValue(value, NormalizeJsonElements(v, visited));
                    }
                }
            }
            return value;
        }

        private static object JsonElementToClr(JsonElement e)
        {
            switch (e.ValueKind)
            {
                case JsonValueKind.Object:
                    return e.EnumerateObject().ToDictionary(p => p.Name, p => NormalizeJsonElements(p.Value));
                case JsonValueKind.Array:
                    return e.EnumerateArray().Select(x => NormalizeJsonElements(x)).ToList();
                case JsonValueKind.String:
                    return e.GetString();
                case JsonValueKind.Number:
                    if (e.TryGetInt64(out var l)) return l;
                    if (e.TryGetDecimal(out var d)) return d;
                    if (e.TryGetDouble(out var db)) return db;
                    return e.GetRawText();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null;
                default:
                    return e.GetRawText();
            }
        }
    }
}
