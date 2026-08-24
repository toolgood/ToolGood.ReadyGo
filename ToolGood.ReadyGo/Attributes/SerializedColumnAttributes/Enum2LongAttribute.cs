using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 枚举转长整数标签：enum 以底层长整数值保存（需 bigint 列）。
    /// 例：UserState.Vip → 2。基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Enum2LongAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public static Enum2LongColumnSerializer Serializer { get; } = new Enum2LongColumnSerializer();

        /// <summary>
        /// 枚举转长整数标签
        /// </summary>
        public Enum2LongAttribute()
        {
        }

        /// <summary>
        /// 枚举转长整数标签
        /// </summary>
        /// <param name="name">列名</param>
        public Enum2LongAttribute(string name) : base(name)
        {
        }
    }
}
