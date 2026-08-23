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
            if (value == null) {
                return null;
            }
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // 部分数据库驱动（如 MySQL）会直接返回日期类型
            switch (value) {
                case DateTime dateTime:
                    if (t == typeof(DateTime)) { return dateTime; }
                    if (t == typeof(DateTimeOffset)) { return new DateTimeOffset(dateTime); }
                    if (t == typeof(DateOnly)) { return DateOnly.FromDateTime(dateTime); }
                    break;
                case DateTimeOffset dateTimeOffset:
                    if (t == typeof(DateTimeOffset)) { return dateTimeOffset; }
                    if (t == typeof(DateTime)) { return dateTimeOffset.DateTime; }
                    if (t == typeof(DateOnly)) { return DateOnly.FromDateTime(dateTimeOffset.DateTime); }
                    break;
                case DateOnly dateOnly:
                    if (t == typeof(DateOnly)) { return dateOnly; }
                    if (t == typeof(DateTime)) { return dateOnly.ToDateTime(TimeOnly.MinValue); }
                    if (t == typeof(DateTimeOffset)) { return new DateTimeOffset(dateOnly.ToDateTime(TimeOnly.MinValue)); }
                    break;
            }

            var s = value as string ?? value?.ToString();
            if (string.IsNullOrEmpty(s)) {
                return null;
            }
            if (t == typeof(DateOnly)) {
                if (DateOnly.TryParseExact(s, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly)) {
                    return dateOnly;
                }
                // 兼容数据库中带时间部分的值，如 "1991-04-03 00:00:00"
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)) {
                    return DateOnly.FromDateTime(parsed);
                }
                throw new FormatException($"String '{s}' was not recognized as a valid DateOnly.");
            }

            if (!TryParse(s, out var result)) {
                throw new FormatException($"String '{s}' was not recognized as a valid DateTime.");
            }
            if (t == typeof(DateTimeOffset)) {
                return new DateTimeOffset(result);
            }
            return result;
        }

        /// <summary>
        /// 优先按 yyyy-MM-dd 精确解析，失败时宽松解析（兼容 "1991-04-03 00:00:00" 等带时间部分的值）。
        /// </summary>
        private static bool TryParse(string s, out DateTime result)
        {
            if (DateTime.TryParseExact(s, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) {
                return true;
            }
            return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}
