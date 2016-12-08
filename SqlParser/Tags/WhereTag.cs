using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class WhereTag:BaseTag
    {
        public OperationTag Operations { get; set; }

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append(" WHERE ");
            Operations.ToSql(sb);
        }
    }
}
