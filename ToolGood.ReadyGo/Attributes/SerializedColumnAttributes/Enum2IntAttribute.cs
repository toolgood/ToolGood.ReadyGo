using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 枚举转整数标签：enum 以底层整数值保存（需整数列）。
    /// 例：UserState.Vip → 2。基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Enum2IntAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public static Enum2IntColumnSerializer Serializer { get; } = new Enum2IntColumnSerializer();

        /// <summary>
        /// 枚举转整数标签
        /// </summary>
        public Enum2IntAttribute()
        {
        }

        /// <summary>
        /// 枚举转整数标签
        /// </summary>
        /// <param name="name">列名</param>
        public Enum2IntAttribute(string name) : base(name)
        {
        }
    }
}
