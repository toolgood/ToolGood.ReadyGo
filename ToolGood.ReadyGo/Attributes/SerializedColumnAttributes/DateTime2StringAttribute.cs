using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 时间转字符串标签：将 DateTime / DateTimeOffset 以 "yyyy-MM-dd HH:mm:ss" 文本保存（需文本列）。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DateTime2StringAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public override DateTime2StringColumnSerializer Serializer => DefaultSerializer;

        private static readonly DateTime2StringColumnSerializer DefaultSerializer = new DateTime2StringColumnSerializer();

        /// <summary>
        /// 时间转字符串标签
        /// </summary>
        public DateTime2StringAttribute()
        {
        }

        /// <summary>
        /// 时间转字符串标签
        /// </summary>
        /// <param name="name">列名</param>
        public DateTime2StringAttribute(string name) : base(name)
        {
        }
    }
}
