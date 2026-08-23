using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 列类型标签：用于显式指定属性或字段映射到数据库时使用的 CLR 类型。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ColumnTypeAttribute : Attribute
    {
        /// <summary>
        /// 指定的类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 列类型标签
        /// </summary>
        /// <param name="type">指定的类型</param>
        public ColumnTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}
