using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class StringLiteralTag : BaseTag
    {
        public string StringLiteral;

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append(StringLiteral);
        }
    }
}
