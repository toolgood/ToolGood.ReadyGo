using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 忽略特征
    /// </summary>
    [AttributeUsage(AttributeTargets.Property , AllowMultiple = false, Inherited = true)]
    public class IgnoreAttribute : Attribute
    {
    }
}