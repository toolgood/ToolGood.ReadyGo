using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo.WhereHelpers;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class JoinTag : BaseTag
    {
        public JoinType JoinType { get; set; }
        public TableTag Table { get; set; }
        public BaseTag On { get; set; }

        internal override void ToSql(StringBuilder sb)
        {
            switch (JoinType) {
                case JoinType.Inner: sb.Append(" INNER JOIN "); break;
                case JoinType.Left: sb.Append(" LEFT JOIN "); break;
                case JoinType.Right: sb.Append(" RIGHT JOIN "); break;
                case JoinType.Full: sb.Append(" FULL JOIN "); break;
                default: break;
            }
            Table.ToSql(sb);
            sb.Append(" ");
            On.ToSql(sb);
        }
    }
}
