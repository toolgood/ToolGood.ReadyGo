using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 布尔转整数标签：bool 以 0/1 整数保存（需整数列）。
    /// 例：true → 1，false → 0。基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Bool2IntAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public static Bool2IntColumnSerializer Serializer { get; } = new Bool2IntColumnSerializer();

        /// <summary>
        /// 布尔转整数标签
        /// </summary>
        public Bool2IntAttribute()
        {
        }

        /// <summary>
        /// 布尔转整数标签
        /// </summary>
        /// <param name="name">列名</param>
        public Bool2IntAttribute(string name) : base(name)
        {
        }
    }
}
