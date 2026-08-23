using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 显式列标签：用于标记仅映射显式声明了列标签的成员。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExplicitColumnsAttribute : Attribute
    {
    }
}
