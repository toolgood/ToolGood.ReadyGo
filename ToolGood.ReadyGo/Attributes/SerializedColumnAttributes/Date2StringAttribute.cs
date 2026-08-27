using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 日期标签：只保存日期，不保存时间。
    /// 基于 SerializedColumn + IColumnSerializer 实现，数据库中仅存 "yyyy-MM-dd"。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class Date2StringAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public override Date2StringColumnSerializer Serializer => DefaultSerializer;

        private static readonly Date2StringColumnSerializer DefaultSerializer = new Date2StringColumnSerializer();

        /// <summary>
        /// 日期标签
        /// </summary>
        public Date2StringAttribute()
        {
        }

        /// <summary>
        /// 日期标签
        /// </summary>
        /// <param name="name">列名</param>
        public Date2StringAttribute(string name) : base(name)
        {
        }
    }
}
