using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class ColumnTag : BaseTag
    {
        public ColumnTag() { }
        public ColumnTag(string name)
        {
            this.ColumnName = name;
        }
        public ColumnTag(TableTag table, string name)
        {
            Table = table;
            this.ColumnName = name;
        }


        public TableTag Table { get; set; }
        public string ColumnName { get; set; }

        internal override void ToSql(StringBuilder sb)
        {
            if (Table != null) {
                if (Table.AsName != null) {
                    sb.AppendFormat("{0}.{1}", Table.AsName.AsName, ColumnName);
                }
            } else {
                sb.Append(ColumnName);
            }
        }
    }
}
