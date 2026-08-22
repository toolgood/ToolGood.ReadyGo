using System;

namespace ToolGood.ReadyGo.NPoco
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreAttribute : Attribute
    {
    }
}