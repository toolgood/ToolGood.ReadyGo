using System;
using System.Globalization;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 布尔转整数列序列化器：bool 以 0/1 整数保存，读取时还原。
    /// 支持 bool?（null → NULL，NULL → null）。
    /// </summary>
    public class Bool2IntColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// 布尔转整数列序列化器
        /// </summary>
        /// <param name="value">bool 值</param>
        /// <returns>0 或 1，null 输入返回 null</returns>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case bool b:
                    return b ? 1 : 0;
                default:
                    throw new NotSupportedException($"Bool2Int 不支持的类型：{value.GetType().Name}");
            }
        }

        /// <summary>
        /// 从整数反序列化
        /// </summary>
        /// <param name="value">数据库中的 0/1 值</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>还原的 bool 值</returns>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null || value is DBNull) {
                return null;
            }
            var s = value as string ?? value?.ToString();
            if (string.IsNullOrEmpty(s)) {
                return null;
            }
            if (s == "0") {
                return false;
            }
            if (s == "1") {
                return true;
            }
            throw new FormatException($"String '{s}' was not recognized as a valid bool (0/1).");
        }
    }
}
