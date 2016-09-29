using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class OnTag:BaseTag
    {
        public OperationTag Operation { get; set; }

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append(" ON ");
            Operation.ToSql(sb);
        }
    }
}
