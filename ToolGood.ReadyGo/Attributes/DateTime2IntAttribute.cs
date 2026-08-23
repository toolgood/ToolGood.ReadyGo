using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 时间转整数标签：以 yyyyMMddHHmmss 整数保存（秒级精度，需 long 存储）。
    /// 算法：年月日时分秒按位拼接，如 2026-08-23 15:30:45 → 20260823153045。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DateTime2IntAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public static DateTime2IntColumnSerializer Serializer { get; } = new DateTime2IntColumnSerializer();

        /// <summary>
        /// 时间转整数标签
        /// </summary>
        public DateTime2IntAttribute()
        {
        }

        /// <summary>
        /// 时间转整数标签
        /// </summary>
        /// <param name="name">列名</param>
        public DateTime2IntAttribute(string name) : base(name)
        {
        }
    }
}
