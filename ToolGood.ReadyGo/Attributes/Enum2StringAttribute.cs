using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 枚举以字符串形式存储标签：标记枚举类型本身，而非属性。
    /// 被 <see cref="ToolGood.ReadyGo.Gadget.EnumHelper"/> 消费，用于指示该枚举值以名称字符串读写。
    /// 注意：本标签与属性级序列化标签（如 <see cref="Enum2IntAttribute"/>）不同，
    /// 它作用于枚举类型，不提供列级 IColumnSerializer。
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
    public class Enum2StringAttribute : Attribute
    {
    }
}
