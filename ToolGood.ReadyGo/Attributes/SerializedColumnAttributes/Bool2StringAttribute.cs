using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 布尔转字符串标签：bool 以 "true"/"false" 文本保存（需文本列）。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Bool2StringAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public static Bool2StringColumnSerializer Serializer { get; } = new Bool2StringColumnSerializer();

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
