using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 布尔转字符串标签：bool 以 "true"/"false" 文本保存（需文本列）。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class Bool2StringAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public override Bool2StringColumnSerializer Serializer => DefaultSerializer;

        private static readonly Bool2StringColumnSerializer DefaultSerializer = new Bool2StringColumnSerializer();

        /// <summary>
        /// 布尔转字符串标签
        /// </summary>
        public Bool2StringAttribute()
        {
        }

        /// <summary>
        /// 布尔转字符串标签
        /// </summary>
        /// <param name="name">列名</param>
        public Bool2StringAttribute(string name) : base(name)
        {
        }
    }
}
