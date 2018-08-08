namespace ToolGood.ReadyGo3.DataCentxt.Internals
{

    partial class OrderItem
    {
        private QColumn column;
        private OrderType orderType;
        private string orderString;

        public OrderItem(QColumn column, OrderType orderType)
        {
            this.column = column;
            this.orderType = orderType;
        }

        public OrderItem(string order)
        {
            this.orderString = order;
        }

        public string ToSql(DatabaseProvider provider, int tableCount)
        {
            if (orderString != null) {
                return orderString;
            }
            if (orderType == OrderType.Asc) {
                return (column).ToSql(provider, tableCount) + " ASC";
            }
            return (column).ToSql(provider, tableCount) + " DESC";
        }
    }
}
