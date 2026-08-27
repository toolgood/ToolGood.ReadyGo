using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 别名标签：用于为属性或字段指定查询时的别名。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 别名标签
        /// </summary>
        /// <param name="alias">别名</param>
        public AliasAttribute(string alias)
        {
            Alias = alias;
        }
    }
}