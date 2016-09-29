using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class FunTag:BaseTag
    {
        public string FunctionName { get; set; }
        public List<BaseTag> Parameters { get;internal set; }

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append(" ");
            sb.Append(FunctionName);
            sb.Append("(");
            for (int i = 0; i < Parameters.Count; i++) {
                if (i>0) {
                    sb.Append(",");
                }
                Parameters[i].ToSql(sb);
            }
            sb.Append(")");

        }
    }
}
