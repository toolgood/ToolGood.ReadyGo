using System;
using System.Collections.Generic;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 表示关联查询（JOIN）所需的数据信息。
    /// </summary>
    public class JoinData
    {
        /// <summary>
        /// ON 条件 SQL。
        /// </summary>
        public string OnSql { get; set; }
        /// <summary>
        /// 关联对应的成员。
        /// </summary>
        public PocoMember PocoMember { get; set; }
        /// <summary>
        /// 被关联的成员。
        /// </summary>
        public PocoMember PocoMemberJoin { get; set; }
        /// <summary>
        /// 关联成员的子成员集合。
        /// </summary>
        public List<PocoMember> PocoMembers { get; set; }
        /// <summary>
        /// 关联类型（LEFT 或 INNER）。
        /// </summary>
        public JoinType JoinType { get; set; }
        /// <summary>
        /// 表提示。
        /// </summary>
        public string Hint { get; set; }
    }
}