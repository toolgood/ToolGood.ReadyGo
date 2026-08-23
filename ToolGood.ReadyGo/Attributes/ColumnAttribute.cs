using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 列标签：用于指定属性或字段映射到数据库的列。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 列标签
        /// </summary>
        /// <param name="name">列名</param>
        public ColumnAttribute(string name) { Name = name; }

        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否将时间强制转换为 UTC 时间
        /// </summary>
        public bool ForceToUtc { get; set; } = true;

        /// <summary>
        /// 是否精确匹配列名
        /// </summary>
        public bool ExactNameMatch { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Comment;

        /// <summary>
        /// 列标签
        /// </summary>
        public ColumnAttribute()
        {
            ForceToUtc = false;
        }

        /// <summary>
        /// 列标签
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="comment">备注</param>
        public ColumnAttribute(string name, string comment = null)
        {
            Name = name.Trim();
            ForceToUtc = false;
            if(comment != null) {
                this.Comment = comment.Trim();
            }
        }
    }
}