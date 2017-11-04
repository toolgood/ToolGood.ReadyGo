using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
using ToolGood.ReadyGo3.DataCentxt.Internals;
using ToolGood.ReadyGo3.Gadget.Internals;

namespace ToolGood.ReadyGo3.DataCentxt
{
    [Serializable]
    public abstract partial class QTable:IDisposable
    {
        public string __TableName__ { get { return _tableName; } set { _tableName = value.Trim(); } }
        public string __SchemaName__ { get { return _schemaName; } set { _schemaName = value.Trim(); } }
        public SqlRecord __SQL__ { get { return _sqlHelper.Sql; } }

        internal bool _singleSqlHelper;
        internal SqlHelper _sqlHelper;
        internal SqlType _sqlType;
        internal string _connStringName;
        internal SqlBuilder _sqlBuilder;
        internal string _schemaName;
        internal string _tableName;
        internal string _asName;
        internal JoinType _joinType;
        internal QJoinCondition _joinCondition;
        internal Dictionary<string, QTableColumn> _columns = new Dictionary<string, QTableColumn>();

        #region AddColumn
        private QTableColumn<T> AddColumn<T>(string columnName, string fieldName, bool isPk, bool resultColumn, string resultSql)
        {
            QTableColumn<T> column = new QTableColumn<T>();
            column._columnType = Enums.ColumnType.Column;
            column._columnName = columnName;
            column._isPrimaryKey = isPk;
            column._isResultColumn = resultColumn;
            column._resultSql = resultSql;
            _columns.Add(fieldName.ToLower(), column);
            return column;
        }
        protected QTableColumn<T> AddColumn<T>(string columnName, string fieldName, bool isPk = false)
        {
            return AddColumn<T>(columnName, fieldName, isPk, false, null);
        }
        protected QTableColumn<T> AddColumn<T>(string columnName, string fieldName, string resultSql)
        {
            return AddColumn<T>(columnName, fieldName, false, true, null);
        }
        #endregion

        protected internal SqlBuilder getSqlBuilder()
        {
            if (_sqlBuilder == null) {
                _sqlBuilder = new SqlBuilder(this);
            }
            return _sqlBuilder;
        }

        public void Clear()
        {
            if (_sqlBuilder != null) {
                _sqlBuilder.Dispose();
            }
        }

        public void Dispose()
        {
            if (_singleSqlHelper) {
                if (_sqlHelper!=null) {
                    _sqlHelper.Dispose();
                }
            }
        }
    }

    public abstract partial class QTable<T> : QTable
        where T : class
    {
        protected QTable()
        {
            _singleSqlHelper = true;
            _sqlHelper = new SqlHelper();
            _sqlType = _sqlHelper._sqlType;
            Init();
        }

        protected QTable(string connStringName)
        {
            _singleSqlHelper = true;
            _connStringName = connStringName;
            _sqlHelper = new SqlHelper(connStringName);
            _sqlType = _sqlHelper._sqlType;
            Init();

        }
        protected QTable(SqlHelper sqlHelper)
        {
            _singleSqlHelper = false;
            _sqlHelper = sqlHelper;
            _sqlType = _sqlHelper._sqlType;
            Init();
        }

        protected abstract void Init();


    }


}
