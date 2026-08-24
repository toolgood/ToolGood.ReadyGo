using System;
using System.Globalization;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 布尔转字符串列序列化器：bool 以 "true"/"false" 文本保存，读取时还原。
    /// 支持 bool?（null → NULL，NULL → null）。
    /// </summary>
    public class Bool2StringColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// 序列化为 "true"/"false" 字符串
        /// </summary>
        /// <param name="value">bool 值</param>
        /// <returns>"true" 或 "false"，null 输入返回 null</returns>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case bool b:
                    return b ? "true" : "false";
                default:
                    throw new NotSupportedException($"Bool2String 不支持的类型：{value.GetType().Name}");
            }
        }

        /// <summary>
        /// 从字符串反序列化
        /// </summary>
        /// <param name="value">数据库中的 "true"/"false" 值</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>还原的 bool 值</returns>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null) {
                return null;
            }
            var s = value as string ?? value?.ToString();
            if (string.IsNullOrEmpty(s)) {
                return null;
            }
            if (bool.TryParse(s, out var b)) {
                return b;
            }
            if (s == "1") {
                return true;
            }
            if (s == "0") {
                return false;
            }
            throw new FormatException($"String '{s}' was not recognized as a valid bool.");
        }
    }
}
