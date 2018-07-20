using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ToolGood.ReadyGo3.DataCentxt.Internals;
using ToolGood.ReadyGo3.Gadget.Internals;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.DataCentxt
{
    public abstract partial class QTable : IDisposable
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string __TableName__ { get { return _tableName; } set { _tableName = value.Trim(); } }
        /// <summary>
        /// Schema名
        /// </summary>
        public string __SchemaName__ { get { return _schemaName; } set { _schemaName = value?.Trim(); } }
        /// <summary>
        /// SQL记录
        /// </summary>
        public SqlRecord __SQL__ { get { return GetSqlHelper()._Sql; } }

        private bool _singleSqlHelper;
        private SqlHelper _sqlHelper;

        internal SqlBuilder _sqlBuilder;
        internal string _schemaName;
        internal string _tableName;
        internal string _asName;
        internal JoinType _joinType;
        internal QJoinCondition _joinCondition;
        internal Dictionary<string, QTableColumn> _columns = new Dictionary<string, QTableColumn>();
        internal QTableColumn _primaryKey;
        /// <summary>
        /// 
        /// </summary>
        protected QTable() { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlHelper"></param>
        protected QTable(SqlHelper sqlHelper)
        {
            _singleSqlHelper = false;
            _sqlHelper = sqlHelper;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (_singleSqlHelper) {
                if (_sqlHelper != null) {
                    _sqlHelper.Dispose();
                }
            }
        }



        #region AddColumn
        protected QTableColumn<T> AddColumn<T>(string columnName, string fieldName, bool isPk, bool isAutoIncrement, bool resultColumn, string resultSql)
        {
            QTableColumn<T> column = new QTableColumn<T> {
                _columnType = Enums.ColumnType.Column,
                _columnName = columnName,
                _isPrimaryKey = isPk,
                _isResultColumn = resultColumn,
                _resultSql = resultSql,
                _table = this,
                _isAutoIncrement = isAutoIncrement
            };
            _columns.Add(fieldName.ToLower(), column);
            if (isPk) {
                _primaryKey = column;
            }
            return column;
        }
        /// <summary>
        /// 添加列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="fieldName"></param>
        /// <param name="isPk"></param>
        /// <param name="isAutoIncrement"></param>
        /// <returns></returns>
        protected QTableColumn<T> AddColumn<T>(string columnName, string fieldName, bool isPk, bool? isAutoIncrement = null)
        {
            if (isPk) {
                if (isAutoIncrement == null) {
                    var type = typeof(T);
                    if (type == typeof(Int16) || type == typeof(int) || type == typeof(Int64) ||
                        type == typeof(UInt16) || type == typeof(UInt32) || type == typeof(UInt64)) {
                        return AddColumn<T>(columnName, fieldName, isPk, true, false, null);
                    }
                    return AddColumn<T>(columnName, fieldName, isPk, false, false, null);
                }
                return AddColumn<T>(columnName, fieldName, isPk, isAutoIncrement.Value, false, null);
            }
            return AddColumn<T>(columnName, fieldName, isPk, false, false, null);
        }
        /// <summary>
        /// 添加列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="fieldName"></param>
        /// <param name="resultSql"></param>
        /// <returns></returns>
        protected QTableColumn<T> AddColumn<T>(string columnName, string fieldName, string resultSql)
        {
            if (string.IsNullOrEmpty(resultSql) == false) {
                resultSql = resultSql.Replace("{0}.", "{0}");
                if (resultSql[0] != '(') {
                    resultSql = "(" + resultSql + ")";
                }
            }
            return AddColumn<T>(columnName, fieldName, false, false, true, resultSql);
        }


        #endregion

        /// <summary>
        /// 清空，进行下一次
        /// </summary>
        public void Clear()
        {
            if (_sqlBuilder != null) {
                _sqlBuilder.Dispose();
                _sqlBuilder = null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract void Init();

        internal SqlHelper GetSqlHelper()
        {
            //            if (_sqlHelper == null || _sqlHelper._isDisposable) {
            //#if !NETSTANDARD2_0

            //                if (string.IsNullOrEmpty(_connStringName) == false) {
            //                    _singleSqlHelper = true;
            //                    _sqlHelper = new SqlHelper(_connStringName);
            //                } else if (SqlHelper._lastSqlHelper != null && SqlHelper._lastSqlHelper._isDisposable == false) {
            //                    _singleSqlHelper = false;
            //                    _sqlHelper = SqlHelper._lastSqlHelper;
            //                } else {
            //                    _singleSqlHelper = true;
            //                    _sqlHelper = new SqlHelper();
            //                }
            //#else
            //                            if (SqlHelper._lastSqlHelper != null && SqlHelper._lastSqlHelper._isDisposable == false) {
            //                                _singleSqlHelper = false;
            //                                _sqlHelper = SqlHelper._lastSqlHelper;
            //                            }
            //                            throw new InvalidProgramException();
            //#endif
            //            }
            return _sqlHelper;
        }

        internal SqlType GetSqlType()
        {
            return GetSqlHelper()._sqlType;
        }
        internal SqlBuilder GetSqlBuilder()
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
        /// <summary>
        /// 
        /// </summary>
        protected QTable() : base()
        {
            var pd = PetaPoco.Core.PocoData.ForType(typeof(T));
            __TableName__ = pd.TableInfo.TableName;
            __SchemaName__ = pd.TableInfo.SchemaName;
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlHelper"></param>
        protected QTable(SqlHelper sqlHelper) : base(sqlHelper)
        {
            var pd = PetaPoco.Core.PocoData.ForType(typeof(T));
            __TableName__ = pd.TableInfo.TableName;
            __SchemaName__ = pd.TableInfo.SchemaName;
            Init();
        }


        /// <summary>
        /// 添加列
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        protected QTableColumn<T1> AddColumn<T1>(string fieldName)
        {
            var pd = PetaPoco.Core.PocoData.ForType(typeof(T));
            var column = pd.Columns.Where(q => q.Value.PropertyName == fieldName).Select(q => q.Value).FirstOrDefault();
            if (column != null) {
                var isPk = pd.TableInfo.PrimaryKey == fieldName;
                return AddColumn<T1>(column.ColumnName, fieldName, isPk, isPk ? pd.TableInfo.AutoIncrement : false, column.ResultColumn, column.ResultSql);
            }
            return new QTableColumn<T1>();
        }

    }


}
