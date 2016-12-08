using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class TableTag : BaseTag
    {
        public string TableName { get; set; }
        public AsTag AsName { get; set; }

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append(" ");
            sb.Append(TableName);
            if (AsName != null) {
                AsName.ToSql(sb);
            }
        }
    }
}
