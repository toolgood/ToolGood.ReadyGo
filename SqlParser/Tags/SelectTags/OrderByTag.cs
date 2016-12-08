using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class OrderByTag: BaseTag
    {
        public OrderByTag()
        {
            Items = new List<OrderByItemTag>();
        }
        public void AddOrderBy(BaseTag order, OrderType type)
        {
            OrderByItemTag tag = new OrderByItemTag() {
                Field = order,
                OrderType = type
            };
            Items.Add(tag);
        }

        List<OrderByItemTag> Items { get; set; }

        internal override void ToSql(StringBuilder sb)
        {
            sb.Append(" ORDER BY ");
            for (int i = 0; i < Items.Count; i++) {
                if (i>0) {
                    sb.Append(",");
                }
                Items[i].ToSql(sb);
            }
        }
    }
    public class OrderByItemTag : BaseTag
    {
        public BaseTag Field { get; set; }
        public OrderType OrderType { get; set; }

        internal override void ToSql(StringBuilder sb)
        {
            Field.ToSql(sb);
            if (OrderType== OrderType.Asc) {
                sb.Append(" ASC");
            } else {
                sb.Append(" DESC");
            }
        }
    }
}
