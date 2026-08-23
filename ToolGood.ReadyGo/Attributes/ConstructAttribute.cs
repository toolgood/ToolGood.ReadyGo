using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 构造标签：用于标记反序列化时应使用的构造函数。
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class ConstructAttribute : Attribute
    {
        /// <summary>
        /// 构造标签
        /// </summary>
        public ConstructAttribute() { }
    }
}