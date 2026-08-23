using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 复合映射标签：用于将嵌套对象的属性展平映射到同一张表的列。
    /// </summary>
    public class ComplexMappingAttribute : Attribute
    {
        /// <summary>
        /// 是否启用复合映射，默认为 true。
        /// </summary>
        public bool ComplexMapping { get; set; } = true;

        /// <summary>
        /// 自定义列名前缀
        /// </summary>
        public string CustomPrefix { get; set; }

        /// <summary>
        /// 复合映射标签（使用默认前缀）
        /// </summary>
        public ComplexMappingAttribute()
        {
            
        }

        /// <summary>
        /// 复合映射标签
        /// </summary>
        /// <param name="customPrefix">自定义列名前缀</param>
        public ComplexMappingAttribute(string customPrefix)
        {
            CustomPrefix = customPrefix;
        }
    }
}