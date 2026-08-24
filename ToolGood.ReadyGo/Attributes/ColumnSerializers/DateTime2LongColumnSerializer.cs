using System;
using System.Globalization;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 时间转长整数列序列化器：序列化为 yyyyMMddHHmmss 整数（秒级精度，需 long），反序列化回 DateTime / DateTimeOffset / DateOnly
    /// </summary>
    public class DateTime2LongColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// 序列化为 yyyyMMddHHmmss 整数
        /// </summary>
        /// <param name="value">时间值（DateTime / DateTimeOffset / DateOnly）</param>
        /// <returns>yyyyMMddHHmmss 整数</returns>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case DateTime dateTime:
                    return ToLong(dateTime);
                case DateTimeOffset dateTimeOffset:
                    return ToLong(dateTimeOffset.DateTime);
                case DateOnly dateOnly:
                    return ToLong(dateOnly.ToDateTime(TimeOnly.MinValue));
                default:
                    throw new NotSupportedException($"DateTime2Long 不支持的类型：{value.GetType().Name}");
            }
        }

        /// <summary>
        /// 从 yyyyMMddHHmmss 整数反序列化
        /// </summary>
        /// <param name="value">yyyyMMddHHmmss 整数</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>反序列化后的时间值</returns>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null) {
                return null;
            }
            var v = Convert.ToInt64(value, CultureInfo.InvariantCulture);
            var year = (int)(v / 10000000000L);
            var month = (int)(v / 100000000L % 100);
            var day = (int)(v / 1000000L % 100);
            var hour = (int)(v / 10000L % 100);
            var minute = (int)(v / 100L % 100);
            var second = (int)(v % 100);

            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (t == typeof(DateOnly)) {
                return new DateOnly(year, month, day);
            }
            if (t == typeof(DateTimeOffset)) {
                return new DateTimeOffset(new DateTime(year, month, day, hour, minute, second));
            }
            return new DateTime(year, month, day, hour, minute, second);
        }

        private static long ToLong(DateTime dt)
        {
            return dt.Year * 10000000000L
                 + dt.Month * 100000000L
                 + dt.Day * 1000000L
                 + dt.Hour * 10000L
                 + dt.Minute * 100L
                 + dt.Second;
        }
    }
}
