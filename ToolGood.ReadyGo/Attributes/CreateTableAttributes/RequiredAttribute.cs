using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 非空标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RequiredAttribute : Attribute
    {
        /// <summary>
        /// 是否非空
        /// </summary>
        public bool Required;

        /// <summary>
        /// 非空标签
        /// </summary>
        /// <param name="required">是否非空，默认为 true</param>
        public RequiredAttribute(bool required = true)
        {
            Required = required;
        }
    }
}
