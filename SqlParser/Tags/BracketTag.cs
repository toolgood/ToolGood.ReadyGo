using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class BracketTag : BaseTag
    {
        public BracketTag()
        {
            Expressions = new List<BaseTag>();
        }


        public List<BaseTag> Expressions { get; set; }

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append("(");

            for (int i = 0; i < Expressions.Count; i++) {
                if (i > 0) {
                    sb.Append(" ");
                }
                Expressions[i].ToSql(sb);
            }
            sb.Append(")");

        }
    }
}
