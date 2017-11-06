using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
using ToolGood.ReadyGo3.DataCentxt.Internals;

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    public partial class SqlBuilder : IDisposable
    {
        internal DatabaseProvider _provider;
        internal DatabaseProvider Provider
        {
            get
            {
                if (_provider==null) {
                    _provider = DatabaseProvider.Resolve(_tables[0].GetSqlType());
                }
                return _provider;
            }
        }
        internal List<QTable> _tables = new List<QTable>();
        private QWhereCondition whereCondition = new QWhereCondition();
        private string _joinOnText;
        private List<OrderItem> _orderBys = new List<OrderItem>();
        private List<QColumn> _groupBy = new List<QColumn>();
        private List<QCondition> _having = new List<QCondition>();
        private bool _useDistinct = false;
        private bool _usedSchemaName = false;
        private bool _jump = false;

        public SqlBuilder(QTable table)
        {
            AddTable(table);
        }

        internal void AddTable(QTable table)
        {
            _tables.Add(table);
            table._asName = "t" + _tables.Count;
            if (string.IsNullOrEmpty(table._schemaName)==false ) {
                SetUsedSchemaName();
            }
        }

        private SqlHelper GetSqlHelper()
        {
           return _tables[0].GetSqlHelper();
        }

        private void SetUsedSchemaName()
        {
            if (_usedSchemaName==false) {
                bool hasSchemaName = false;
                string schemaName = "";

                if (string.IsNullOrEmpty(GetSqlHelper()._schemaName)==false) {
                    hasSchemaName = true;
                    schemaName = GetSqlHelper()._schemaName;
                }
                foreach (var item in _tables) {
                    if (string.IsNullOrEmpty(item._schemaName) == false) {
                        if (hasSchemaName) {
                            if (schemaName!= item._schemaName) {
                                _usedSchemaName = true;
                                return;
                            }
                        } else {
                            hasSchemaName = true;
                            schemaName = item._schemaName;
                        }
                    }
                }
            }
        }



        public void Dispose()
        {
            whereCondition = null;
            _joinOnText = null;
            _orderBys.Clear();
            _groupBy.Clear();
            _groupBy = null;
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
