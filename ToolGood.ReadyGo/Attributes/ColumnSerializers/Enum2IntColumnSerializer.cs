using System;
using System.Globalization;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 枚举转整数列序列化器：enum 以底层整数值（int）保存，读取时还原。
    /// 支持可空枚举（null → NULL，NULL → null）。
    /// </summary>
    public class Enum2IntColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// 序列化为枚举底层整数值
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns>底层整数（int），null 输入返回 null</returns>
        public object Serialize(object value)
        {
            if (value == null) {
                return null;
            }
            if (value is Enum) {
                return Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }
            throw new NotSupportedException($"Enum2Int 不支持的类型：{value.GetType().Name}");
        }

        /// <summary>
        /// 从整数反序列化
        /// </summary>
        /// <param name="value">数据库中的整数值</param>
        /// <param name="targetType">目标类型（枚举或可空枚举）</param>
        /// <returns>还原的枚举值</returns>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null || value is DBNull) {
                return null;
            }
            var s = value as string ?? value?.ToString();
            if (string.IsNullOrEmpty(s)) {
                return null;
            }
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (!typeof(Enum).IsAssignableFrom(t)) {
                throw new NotSupportedException($"Enum2Int 目标类型必须是枚举：{targetType.Name}");
            }
            return Enum.ToObject(t, int.Parse(s, CultureInfo.InvariantCulture));
        }
    }
}
