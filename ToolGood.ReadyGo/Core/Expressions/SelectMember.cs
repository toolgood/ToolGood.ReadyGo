using System;

namespace ToolGood.ReadyGo.NPoco.Expressions
{
    /// <summary>
    /// 表示查询中选择的成员（列）信息。
    /// </summary>
    public class SelectMember : IEquatable<SelectMember>
    {
        /// <summary>
        /// 成员所属的实体类型。
        /// </summary>
        public Type EntityType { get; set; }
        /// <summary>
        /// 自定义的查询列 SQL（为 null 时使用 PocoColumn 生成）。
        /// </summary>
        public string SelectSql { get; set; }
        /// <summary>
        /// 成员对应的列信息。
        /// </summary>
        public PocoColumn PocoColumn { get; set; }
        /// <summary>
        /// 成员关联的列信息数组。
        /// </summary>
        public PocoColumn[] PocoColumns { get; set; }

        /// <summary>
        /// 判断当前成员与另一成员是否按实体类型与列相同。
        /// </summary>
        /// <param name="other">另一选择成员。</param>
        /// <returns>相同返回 true，否则返回 false。</returns>
        public bool Equals(SelectMember other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(EntityType, other.EntityType) && Equals(PocoColumn, other.PocoColumn);
        }

        /// <summary>
        /// 判断当前成员是否与指定对象相等。
        /// </summary>
        /// <param name="obj">待比较的对象。</param>
        /// <returns>类型相同且实体类型、列均相同返回 true，否则返回 false。</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SelectMember)obj);
        }

        /// <summary>
        /// 获取当前成员的哈希码。
        /// </summary>
        /// <returns>基于实体类型与列的哈希码。</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((EntityType != null ? EntityType.GetHashCode() : 0) * 397) ^ (PocoColumn != null ? PocoColumn.GetHashCode() : 0);
            }
        }
    }
}
