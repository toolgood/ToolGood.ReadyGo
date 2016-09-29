using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 列标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property , AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// 是否转成Utc
        /// </summary>
        public bool ForceToUtc { get; private set; }

        /// <summary>
        /// 列标签
        /// </summary>
        public ColumnAttribute() { }

        /// <summary>
        /// 列标签
        /// </summary>
        /// <param name="Name">列名</param>
        /// <param name="Comment">备注</param>
        public ColumnAttribute(string Name, string Comment = null)
        {
            this.Name = Name;
            ForceToUtc = false;
            this.Comment = Comment;
        }
    }
}