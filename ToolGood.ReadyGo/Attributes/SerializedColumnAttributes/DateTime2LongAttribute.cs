using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 时间转长整数标签：以 yyyyMMddHHmmss 整数保存（秒级精度，需 long 存储）。
    /// 算法：年月日时分秒按位拼接，如 2026-08-23 15:30:45 → 20260823153045。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DateTime2LongAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public override DateTime2LongColumnSerializer Serializer => DefaultSerializer;

        private static readonly DateTime2LongColumnSerializer DefaultSerializer = new DateTime2LongColumnSerializer();

        /// <summary>
        /// 时间转长整数标签
        /// </summary>
        public DateTime2LongAttribute()
        {
        }

        /// <summary>
        /// 时间转长整数标签
        /// </summary>
        /// <param name="name">列名</param>
        public DateTime2LongAttribute(string name) : base(name)
        {
        }
    }
}
