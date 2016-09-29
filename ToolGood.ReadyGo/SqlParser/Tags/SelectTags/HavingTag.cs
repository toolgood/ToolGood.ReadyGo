using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class HavingTag : BaseTag
    {
        public HavingTag()
        {
            HavingItems = new List<OperationTag>();
        }
        public void AddHaving(OperationTag having)
        {
            HavingItems.Add(having);
        }


        public List<OperationTag> HavingItems { get; internal set; }

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append(" HAVING ");
            for (int i = 0; i < HavingItems.Count; i++) {
                if (i > 0) {
                    sb.Append(",");
                }
                HavingItems[i].ToSql(sb);
            }


        }
    }
}
