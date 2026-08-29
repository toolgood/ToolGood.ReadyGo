using System;
using System.Globalization;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 时间戳列序列化器：将 DateTime / DateTimeOffset 序列化为 Unix 时间戳（UTC 基准），
    /// 精度支持秒和毫秒，需 long 存储。
    /// </summary>
    public class DateTime2TimestampColumnSerializer : NPoco.IColumnSerializer
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 是否为毫秒精度
        /// </summary>
        public bool IsMilliseconds { get; }

        /// <summary>
        /// 时间戳列序列化器
        /// </summary>
        /// <param name="milliseconds">true 表示毫秒精度，false 表示秒精度</param>
        public DateTime2TimestampColumnSerializer(bool milliseconds = false)
        {
            IsMilliseconds = milliseconds;
        }

        /// <summary>
        /// 序列化为 Unix 时间戳
        /// </summary>
        /// <param name="value">时间值（DateTime / DateTimeOffset）</param>
        /// <returns>Unix 时间戳（long）</returns>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case DateTime dateTime:
                    return ToTimestamp(dateTime.ToUniversalTime());
                case DateTimeOffset dateTimeOffset:
                    return ToTimestamp(dateTimeOffset.UtcDateTime);
                default:
                    throw new NotSupportedException($"Timestamp 不支持的类型：{value.GetType().Name}");
            }
        }

        /// <summary>
        /// 从 Unix 时间戳反序列化（返回 UTC 时间）
        /// </summary>
        /// <param name="value">Unix 时间戳（long）</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>反序列化后的时间值</returns>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null || value is DBNull) {
                return null;
            }
            var v = Convert.ToInt64(value, CultureInfo.InvariantCulture);
            var utc = IsMilliseconds
                ? UnixEpoch.AddMilliseconds(v)
                : UnixEpoch.AddSeconds(v);

            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (t == typeof(DateTimeOffset)) {
                return new DateTimeOffset(utc);
            }
            return utc;
        }

        private long ToTimestamp(DateTime utc)
        {
            var ts = utc - UnixEpoch;
            return IsMilliseconds ? (long)ts.TotalMilliseconds : (long)ts.TotalSeconds;
        }
    }
}
