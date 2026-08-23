using System;
using System.Globalization;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 日期列序列化器：序列化为 "yyyy-MM-dd"，反序列化回 DateTime / DateTimeOffset / DateOnly
    /// </summary>
    public class DateColumnSerializer : NPoco.IColumnSerializer
    {
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
