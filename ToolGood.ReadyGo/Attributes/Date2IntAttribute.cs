using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 日期转整数标签：只保存 yyyyMMdd 整数，不保存时间。
    /// 算法：date.Year × 10000 + date.Month × 100 + date.Day，如 2026-08-23 → 20260823。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Date2IntAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public static Date2IntColumnSerializer Serializer { get; } = new Date2IntColumnSerializer();

        /// <summary>
        /// 日期转整数标签
        /// </summary>
        public Date2IntAttribute()
        {
        }

        /// <summary>
        /// 日期转整数标签
        /// </summary>
        /// <param name="name">列名</param>
        public Date2IntAttribute(string name) : base(name)
        {
        }
    }
}
