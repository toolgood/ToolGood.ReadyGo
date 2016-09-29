using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class GroupByTag: BaseTag
    {
        public GroupByTag()
        {
            Columns = new List<BaseTag>();
        }
        public void AddColumn(BaseTag col)
        {
            Columns.Add(col);
        }


        public List<BaseTag> Columns { get;internal set; }

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append(" GROUP BY ");
            for (int i = 0; i < Columns.Count; i++) {
                if (i>0) {
                    sb.Append(",");
                }
                Columns[i].ToSql(sb);
            }

        }
    }
}
