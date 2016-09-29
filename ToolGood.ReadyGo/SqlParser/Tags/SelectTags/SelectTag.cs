using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class SelectTag : BaseTag
    {
        public SelectTag()
        {
            Items = new List<SelectItemTag>();
        }
        public SelectItemTag CreateItem()
        {
            SelectItemTag tag = new SelectItemTag();
            return tag;
        }


        public List<SelectItemTag> Items { get; internal set; }

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append(" SELECT ");
            for (int i = 0; i < Items.Count; i++) {
                if (i > 0) {
                    sb.Append(",");
                }
                Items[i].ToSql(sb);
            }
        }
    }


    public class SelectItemTag : BaseTag
    {
        public SelectItemTag()
        {
            Expressions = new List<BaseTag>();
        }


        public List<BaseTag> Expressions { get; internal set; }
        public AsTag AsName { get; set; }

        internal override void ToSql(StringBuilder sb)
        {
            for (int i = 0; i < Expressions.Count; i++) {
                if (i > 0) {
                    sb.Append(" ");
                }
                Expressions[i].ToSql(sb);
            }
            if (AsName != null) {
                AsName.ToSql(sb);
            }
        }
    }
}
