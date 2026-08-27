using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 时间戳精度
    /// </summary>
    public enum TimestampPrecision
    {
        /// <summary>
        /// 秒级时间戳（10 位，需 long 存储）
        /// </summary>
        Seconds = 0,

        /// <summary>
        /// 毫秒级时间戳（13 位，需 long 存储）
        /// </summary>
        Milliseconds = 1,
    }

    /// <summary>
    /// 时间戳标签：以 Unix 时间戳（UTC 基准）保存，精度支持秒和毫秒，需 long 存储。
    /// 例：2026-08-23 15:30:45 UTC → 1787xxx 秒 / 1787xxxxxx 毫秒。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DateTime2TimestampAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 时间戳精度
        /// </summary>
        public TimestampPrecision Precision { get; }

        /// <summary>
        /// 列级序列化器
        /// </summary>
        public override DateTime2TimestampColumnSerializer Serializer { get; }

        /// <summary>
        /// 时间戳标签
        /// </summary>
        /// <param name="precision">时间戳精度，默认秒</param>
        public DateTime2TimestampAttribute(TimestampPrecision precision = TimestampPrecision.Seconds)
        {
            Precision = precision;
            Serializer = new DateTime2TimestampColumnSerializer(precision == TimestampPrecision.Milliseconds);
        }

        /// <summary>
        /// 时间戳标签
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="precision">时间戳精度，默认秒</param>
        public DateTime2TimestampAttribute(string name, TimestampPrecision precision = TimestampPrecision.Seconds) : base(name)
        {
            Precision = precision;
            Serializer = new DateTime2TimestampColumnSerializer(precision == TimestampPrecision.Milliseconds);
        }
    }
}
