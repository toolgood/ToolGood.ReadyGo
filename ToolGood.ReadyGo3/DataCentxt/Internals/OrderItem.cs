﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    public partial class OrderItem 
    {
        private QColumnBase column;
        private OrderType orderType;
        private string orderString;

        public OrderItem(QColumnBase column, OrderType orderType)
        {
            this.column = column;
            this.orderType = orderType;
        }

        public OrderItem(string order)
        {
            this.orderString = order;
        }

      public  string ToSql(DatabaseProvider provider, int tableCount)
        {
            if (orderString != null) {
                return orderString;
            }
            if (orderType == OrderType.Asc) {
                return ((IColumnConvert)column).ToSql(provider, tableCount) + " ASC";
            }
            return ((IColumnConvert)column).ToSql(provider, tableCount) + " DESC";
        }
    }
}
