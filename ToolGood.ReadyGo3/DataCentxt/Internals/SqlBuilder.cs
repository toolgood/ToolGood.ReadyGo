using System;
using System.Collections.Generic;

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class SqlBuilder : IDisposable
    {
        internal DatabaseProvider _provider;
        internal DatabaseProvider Provider {
            get {
                if (_provider == null) {
                    _provider = DatabaseProvider.Resolve(_tables[0].GetSqlType());
                }
                return _provider;
            }
        }
        internal List<QTable> _tables = new List<QTable>();
        private QWhereCondition whereCondition = new QWhereCondition();
        private string _joinOnText;
        private readonly List<OrderItem> _orderBys = new List<OrderItem>();
        private readonly List<QColumn> _groupBy = new List<QColumn>();
        private readonly List<QCondition> _having = new List<QCondition>();
        private bool _useDistinct = false;
        private bool _jump = false;

        public SqlBuilder(QTable table)
        {
            AddTable(table);
        }

        internal void AddTable(QTable table)
        {
            _tables.Add(table);
            table._asName = "t" + _tables.Count;
        }

        private SqlHelper GetSqlHelper()
        {
            return _tables[0].GetSqlHelper();
        }


        public void Dispose()
        {
            whereCondition = null;
            _joinOnText = null;
            _orderBys.Clear();
            _groupBy.Clear();
            _having.Clear();

            foreach (var table in _tables) {
                foreach (var col in table._columns) {
                    col.Value._asName = null;
                    col.Value.ClearValue();
                }
                table._asName = "t1";
                table._joinCondition = null;
                table._joinType = JoinType.Inner;
                table._sqlBuilder = null;
            }
            _tables.Clear();
            _tables = null;
        }
    }
}
