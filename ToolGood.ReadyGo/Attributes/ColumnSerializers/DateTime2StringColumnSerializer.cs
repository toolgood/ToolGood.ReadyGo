using System;
using System.Globalization;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 时间转字符串列序列化器：序列化为 "yyyy-MM-dd HH:mm:ss"，反序列化回 DateTime / DateTimeOffset。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    public class DateTime2StringColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// 时间序列化所使用的格式字符串。
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 序列化为时间字符串
        /// </summary>
        /// <param name="value">时间值（DateTime / DateTimeOffset）</param>
        /// <returns>"yyyy-MM-dd HH:mm:ss" 字符串，null 输入返回 null</returns>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case DateTime dateTime:
                    return dateTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
                case DateTimeOffset dateTimeOffset:
                    return dateTimeOffset.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
                default:
                    throw new NotSupportedException($"DateTime2String 不支持的类型：{value.GetType().Name}");
            }
        }

        /// <summary>
        /// 从时间字符串反序列化
        /// </summary>
        /// <param name="value">格式为 yyyy-MM-dd HH:mm:ss 的时间字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>反序列化后的时间值</returns>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null || value is DBNull) {
                return null;
            }
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // 部分数据库驱动（如 MySQL）会直接返回日期类型
            switch (value) {
                case DateTime dateTime:
                    if (t == typeof(DateTime)) { return dateTime; }
                    if (t == typeof(DateTimeOffset)) { return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)); }
                    break;
                case DateTimeOffset dateTimeOffset:
                    if (t == typeof(DateTimeOffset)) { return dateTimeOffset; }
                    if (t == typeof(DateTime)) { return dateTimeOffset.DateTime; }
                    break;
            }

            var s = value as string ?? value?.ToString();
            if (string.IsNullOrEmpty(s)) {
                return null;
            }
            if (!TryParse(s, out var result)) {
                throw new FormatException($"String '{s}' was not recognized as a valid DateTime.");
            }
            if (t == typeof(DateTimeOffset)) {
                // 存储值为无时区文本，按 UTC 解释，避免依赖服务器本地时区
                return new DateTimeOffset(DateTime.SpecifyKind(result, DateTimeKind.Utc));
            }
            return result;
        }

        /// <summary>
        /// 优先按 yyyy-MM-dd HH:mm:ss 精确解析，再兼容仅日期（yyyy-MM-dd）等带时间部分的值。
        /// </summary>
        private static bool TryParse(string s, out DateTime result)
        {
            if (DateTime.TryParseExact(s, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) {
                return true;
            }
            if (DateTime.TryParseExact(s, Date2StringColumnSerializer.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) {
                return true;
            }
            return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}
