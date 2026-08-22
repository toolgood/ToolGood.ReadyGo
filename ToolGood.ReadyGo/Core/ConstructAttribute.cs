using System;

namespace ToolGood.ReadyGo.NPoco
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class ConstructAttribute : Attribute
    {
        public ConstructAttribute() { }
    }
}