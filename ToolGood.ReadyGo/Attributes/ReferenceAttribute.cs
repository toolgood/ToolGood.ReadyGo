using System;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 引用标签：用于标记属性或字段为引用关系。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ReferenceAttribute : Attribute
    {
        /// <summary>
        /// 引用类型
        /// </summary>
        public readonly ReferenceType ReferenceType;

        /// <summary>
        /// 引用标签（默认外键引用）
        /// </summary>
        public ReferenceAttribute() : this(ReferenceType.Foreign)
        {
        }

        /// <summary>
        /// 引用标签
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        public ReferenceAttribute(ReferenceType referenceType)
        {
            ReferenceType = referenceType;
        }
        
        /// <summary>
        /// 用于关联关系的属性名（区分大小写）。
        /// </summary>
        public string ReferenceMemberName { get; set; }

        /// <summary>
        /// 映射到该属性的数据库列名。
        /// </summary>
        public string ColumnName { get; set; }
    }
}
