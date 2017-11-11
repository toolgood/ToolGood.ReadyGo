using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
using ToolGood.ReadyGo3.DataCentxt.Internals;
using ToolGood.ReadyGo3.Gadget.Internals;

namespace ToolGood.ReadyGo3.DataCentxt
{
    public abstract partial class QTable : IDisposable
    {
        public string __TableName__ { get { return _tableName; } set { _tableName = value.Trim(); } }
        public string __SchemaName__ { get { return _schemaName; } set { _schemaName = value.Trim(); } }
        public SqlRecord __SQL__ { get { return GetSqlHelper()._Sql; } }

        private bool _singleSqlHelper;
        private SqlHelper _sqlHelper;
        private readonly string _connStringName;
        internal SqlBuilder _sqlBuilder;
        internal string _schemaName;
        internal string _tableName;
        internal string _asName;
        internal JoinType _joinType;
        internal QJoinCondition _joinCondition;
        internal Dictionary<string, QTableColumn> _columns = new Dictionary<string, QTableColumn>();

        protected QTable()
        {
            Init();
        }
        protected QTable(string connStringName)
        {
            _connStringName = connStringName;
            Init();
        }
        protected QTable(SqlHelper sqlHelper)
        {
            _singleSqlHelper = false;
            _sqlHelper = sqlHelper;
            Init();
        }
        public void Dispose()
        {
            if (_singleSqlHelper) {
                if (_sqlHelper != null) {
                    _sqlHelper.Dispose();
                }
            }
        }



        #region AddColumn
        private QTableColumn<T> AddColumn<T>(string columnName, string fieldName, bool isPk, bool resultColumn, string resultSql)
        {
            QTableColumn<T> column = new QTableColumn<T>();
            column._columnType = Enums.ColumnType.Column;
            column._columnName = columnName;
            column._isPrimaryKey = isPk;
            column._isResultColumn = resultColumn;
            column._resultSql = resultSql;
            column._table = this;
            _columns.Add(fieldName.ToLower(), column);
            return column;
        }
        protected QTableColumn<T> AddColumn<T>(string columnName, string fieldName, bool isPk)
        {
            return AddColumn<T>(columnName, fieldName, isPk, false, null);
        }
        protected QTableColumn<T> AddColumn<T>(string columnName, string fieldName, string resultSql)
        {
            if (string.IsNullOrEmpty(resultSql) == false) {
                resultSql = resultSql.Replace("{0}.", "{0}");
                if (resultSql[0] != '(') {
                    resultSql = "(" + resultSql + ")";
                }
            }
            return AddColumn<T>(columnName, fieldName, false, true, resultSql);
        }
        #endregion
        public void Clear()
        {
            if (_sqlBuilder != null) {
                _sqlBuilder.Dispose();
            }
        }


        protected abstract void Init();
        protected internal SqlHelper GetSqlHelper()
        {
            if (_sqlHelper == null || _sqlHelper._isDisposable) {
                if (string.IsNullOrEmpty(_connStringName) == false) {
                    _singleSqlHelper = true;
                    _sqlHelper = new SqlHelper(_connStringName);
                } else if (SqlHelper._lastSqlHelper != null && SqlHelper._lastSqlHelper._isDisposable == false) {
                    _singleSqlHelper = false;
                    _sqlHelper = SqlHelper._lastSqlHelper;
                } else {
                    _singleSqlHelper = true;
                    _sqlHelper = new SqlHelper();
                }
            }
            return _sqlHelper;
        }
        protected internal SqlType GetSqlType()
        {
            return GetSqlHelper()._sqlType;
        }
        protected internal SqlBuilder GetSqlBuilder()
        {
            if (_sqlBuilder == null) {
                _sqlBuilder = new SqlBuilder(this);
            }
            return _sqlBuilder;
        }
    }

    public abstract partial class QTable<T> : QTable
        where T : class
    {
        protected QTable() : base() { }

        protected QTable(string connStringName) : base(connStringName) { }

        protected QTable(SqlHelper sqlHelper) : base(sqlHelper) { }

    }


}
