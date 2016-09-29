using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class FromTag : BaseTag
    {
        public FromTag()
        {
            Tables = new List<TableTag>();
        }

        public TableTag Table { get; internal set; }

        public List<TableTag> Tables { get;internal set; }

        public void AddTable(TableTag table)
        {
            if (Table==null) {
                Table = table;
            }
            Tables.Add(table);
        }

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append(" FROM ");
            for (int i = 0; i < Tables.Count; i++) {
                if (i>0) {
                    sb.Append(",");
                }
                Tables[i].ToSql(sb);
            }
        
        }
    }
}
