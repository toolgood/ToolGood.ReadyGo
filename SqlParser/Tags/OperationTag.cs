using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class OperationTag: BaseTag
    {
        public BaseTag Left { get; set; }
        public string Operation { get; set; }
        public BaseTag Right { get; set; }

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append(" ");
            if (Operation=="OR") {
                sb.Append("(");
            }
            Left.ToSql(sb);
            sb.AppendFormat(" {0} ", Operation);
            Right.ToSql(sb);
            if (Operation == "OR") {
                sb.Append(")");
            }
        }
    }
}
