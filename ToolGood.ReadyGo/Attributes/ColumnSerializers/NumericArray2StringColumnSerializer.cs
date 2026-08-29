using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 数值列表列序列化器：将 int[] / long[] / double[] / decimal[] 等数值数组及其 List&lt;T&gt;
    /// 序列化为以分隔符连接的文本列（需文本列），读取时按分隔符还原为数值。
    /// 数值按 InvariantCulture 格式化，保证往返可逆。
    /// </summary>
    public class NumericArray2StringColumnSerializer : NPoco.IColumnSerializer
    {
        private readonly string _separator;

        /// <summary>
        /// 数值列表列序列化器
        /// </summary>
        /// <param name="separator">分隔符，默认逗号</param>
        public NumericArray2StringColumnSerializer(string separator = ",")
        {
            if (string.IsNullOrEmpty(separator)) {
                throw new ArgumentException("separator 不能为空", nameof(separator));
            }
            _separator = separator;
        }

        /// <summary>
        /// 序列化为分隔符文本
        /// </summary>
        /// <param name="value">数值数组或 List&lt;T&gt;</param>
        /// <returns>分隔符文本，null 输入返回 null</returns>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case IList list:
                    if (!IsNumericList(list.GetType())) {
                        throw new NotSupportedException($"NumericStringList 仅支持数值数组或 List<T>，不支持：{list.GetType().Name}");
                    }
                    if (list.Count == 0) {
                        return "";
                    }
                    var sb = new StringBuilder();
                    for (int i = 0; i < list.Count; i++) {
                        if (i > 0) {
                            sb.Append(_separator);
                        }
                        sb.Append(Convert.ToString(list[i], CultureInfo.InvariantCulture));
                    }
                    return sb.ToString();
                default:
                    throw new NotSupportedException($"NumericStringList 不支持的类型：{value.GetType().Name}");
            }
        }

        /// <summary>
        /// 从分隔符文本反序列化
        /// </summary>
        /// <param name="value">分隔符文本</param>
        /// <param name="targetType">目标类型（数值数组或 List&lt;T&gt;）</param>
        /// <returns>还原的数值列表</returns>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null || value is DBNull) {
                return null;
            }
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;

            Type elementType;
            var isArray = t.IsArray;
            if (isArray) {
                elementType = t.GetElementType();
            } else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)) {
                elementType = t.GetGenericArguments()[0];
            } else {
                throw new NotSupportedException($"NumericStringList 目标类型仅支持数值数组或 List<T>：{targetType.Name}");
            }
            if (!IsNumeric(elementType)) {
                throw new NotSupportedException($"NumericStringList 仅支持数值元素类型，不支持：{elementType.Name}");
            }

            var s = value as string ?? value?.ToString();
            if (string.IsNullOrEmpty(s)) {
                return isArray ? Array.CreateInstance(elementType, 0) : Activator.CreateInstance(t);
            }

            var parts = s.Split(new[] { _separator }, StringSplitOptions.None);
            var result = Array.CreateInstance(elementType, parts.Length);
            for (int i = 0; i < parts.Length; i++) {
                result.SetValue(Convert.ChangeType(parts[i].Trim(), elementType, CultureInfo.InvariantCulture), i);
            }
            if (isArray) {
                return result;
            }
            return ToList(t, result);
        }

        private static bool IsNumericList(Type type)
        {
            Type elementType;
            if (type.IsArray) {
                elementType = type.GetElementType();
            } else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
                elementType = type.GetGenericArguments()[0];
            } else {
                return false;
            }
            return IsNumeric(elementType);
        }

        private static bool IsNumeric(Type type)
        {
            switch (Type.GetTypeCode(type)) {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        private static object ToList(Type listType, Array source)
        {
            var list = (IList)Activator.CreateInstance(listType);
            foreach (var item in source) {
                list.Add(item);
            }
            return list;
        }
    }
}
