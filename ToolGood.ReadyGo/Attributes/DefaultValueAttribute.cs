using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 默认值特征
    /// </summary>
    [AttributeUsage(AttributeTargets.Property , AllowMultiple = false, Inherited = true)]
    public class DefaultValueAttribute : Attribute
    {
        /// <summary>
        /// 默认值特征
        /// </summary>
        /// <param name="defaultstring">默认SQL</param>
        public DefaultValueAttribute(string defaultstring)
        {
            DefaultValue = defaultstring;
        }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; private set; }
    }
}