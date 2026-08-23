using System;
using System.Globalization;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 日期列序列化器：序列化为 "yyyy-MM-dd"，反序列化回 DateTime / DateTimeOffset / DateOnly
    /// </summary>
    public class DateColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// 日期序列化所使用的格式字符串。
        /// </summary>
        public const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// 序列化为日期字符串
        /// </summary>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case DateTime dateTime:
                    return dateTime.ToString(DateFormat, CultureInfo.InvariantCulture);
                case DateTimeOffset dateTimeOffset:
                    return dateTimeOffset.ToString(DateFormat, CultureInfo.InvariantCulture);
                case DateOnly dateOnly:
                    return dateOnly.ToString(DateFormat, CultureInfo.InvariantCulture);
                default:
                    return value.ToString();
            }
        }

        /// <summary>
        /// 从日期字符串反序列化
        /// </summary>
        /// <param name="value">格式为 yyyy-MM-dd 的日期字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>反序列化后的日期值</returns>
        public object Deserialize(object value, Type targetType)
        {
            var s = value as string ?? value?.ToString();
            if (string.IsNullOrEmpty(s)) {
                return null;
            }
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (t == typeof(DateOnly)) {
                return DateOnly.ParseExact(s, DateFormat, CultureInfo.InvariantCulture);
            }
            if (t == typeof(DateTimeOffset)) {
                return new DateTimeOffset(DateTime.ParseExact(s, DateFormat, CultureInfo.InvariantCulture));
            }
            return DateTime.ParseExact(s, DateFormat, CultureInfo.InvariantCulture);
        }
    }
}
