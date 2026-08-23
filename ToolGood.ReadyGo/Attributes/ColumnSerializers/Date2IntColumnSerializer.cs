using System;
using System.Globalization;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 日期转整数列序列化器：序列化为 yyyyMMdd 整数（date.Year × 10000 + date.Month × 100 + date.Day），反序列化回 DateTime / DateTimeOffset / DateOnly
    /// </summary>
    public class Date2IntColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// 序列化为 yyyyMMdd 整数
        /// </summary>
        /// <param name="value">日期值（DateTime / DateTimeOffset / DateOnly）</param>
        /// <returns>yyyyMMdd 整数</returns>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case DateTime dateTime:
                    return ToInt(dateTime);
                case DateTimeOffset dateTimeOffset:
                    return ToInt(dateTimeOffset.DateTime);
                case DateOnly dateOnly:
                    return ToInt(dateOnly.ToDateTime(TimeOnly.MinValue));
                default:
                    throw new NotSupportedException($"Date2Int 不支持的类型：{value.GetType().Name}");
            }
        }

        /// <summary>
        /// 从 yyyyMMdd 整数反序列化
        /// </summary>
        /// <param name="value">yyyyMMdd 整数</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>反序列化后的日期值</returns>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null) {
                return null;
            }
            var v = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            var year = v / 10000;
            var month = v / 100 % 100;
            var day = v % 100;

            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (t == typeof(DateOnly)) {
                return new DateOnly(year, month, day);
            }
            if (t == typeof(DateTimeOffset)) {
                return new DateTimeOffset(new DateTime(year, month, day));
            }
            return new DateTime(year, month, day);
        }

        private static int ToInt(DateTime date)
        {
            return date.Year * 10000 + date.Month * 100 + date.Day;
        }
    }
}
