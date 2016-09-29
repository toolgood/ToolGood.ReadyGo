using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class LimitOffsetTag : BaseTag
    {
        public LimitOffsetTag() { }

        public string Rows { get; set; }
        public string Offset { get; set; }

        internal override void ToSql(StringBuilder sb)
        {
            if (string.IsNullOrEmpty(Rows) == false) {
                sb.Append(" OFFSET ");
                sb.Append(Offset);
                return;
            }


            sb.Append(" LIMIT ");
            if (string.IsNullOrEmpty(Offset) == false) {
                sb.Append(Offset);
                sb.Append(",");
            }
            sb.Append(Rows);
   

        }
    }

}
