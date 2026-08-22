using System;

namespace ToolGood.ReadyGo.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class ConstructAttribute : Attribute
    {
        public ConstructAttribute() { }
    }
}