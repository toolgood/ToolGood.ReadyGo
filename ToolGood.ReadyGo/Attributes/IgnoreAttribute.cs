using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 忽略标签：用于标记不参与数据库映射的属性或字段。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreAttribute : Attribute
    {
    }
}