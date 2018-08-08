using System;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;

using ToolGood.ReadyGo3.DataCentxt.Internals;


namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QColumn
    {
        internal string ToSelectColumn(DatabaseProvider provider, int tableCount)
        {
            if (this._isResultColumn && string.IsNullOrEmpty(this._resultSql) == false) {
                string sql;
                if (tableCount > 1 ) {
                    sql = _resultSql.Replace("{0}", _table._asName + ".");
                } else {
                    sql = _resultSql.Replace("{0}", (_table).ToSql(provider, false) + ".");
                }
                sql = _table.GetSqlHelper().formatSql(sql);
                return sql + " AS '" + _columnName + "'";
            }
            if (string.IsNullOrEmpty(_asName)) {
                return ToSql(provider, tableCount);
            }
            return ToSql(provider, tableCount) + " AS '" + _asName + "'";
        }

        internal string ToSql(DatabaseProvider provider, int tableCount)
        {
            if (_columnType == Enums.ColumnType.None) throw new ColumnTypeException();
            if (_columnType == Enums.ColumnType.Value)
                return provider.EscapeParam(((QTableColumn)this).GetValue());

            if (_columnType == Enums.ColumnType.Column) {
                if (tableCount > 1) {
                    return _table._asName + "." + provider.EscapeSqlIdentifier(_columnName);
                }
                return provider.EscapeSqlIdentifier(_columnName);
            }
            if (_columnType == Enums.ColumnType.Code) {
                return _code;
            }
            return provider.CreateFunction(_function, _functionArgs);
        }

        internal string ToUpdateSet(DatabaseProvider provider, int tableCount)
        {
            var col = this as QTableColumn;
            if (object.Equals(col, null)) throw new NullReferenceException();
            if (col._changeType == Enums.ColumnChangeType.None) throw new ArgumentNullException();
            var left = (col).ToSql(provider, tableCount);

            var column = ((QTableColumn)this);
            string right;
            if (column._changeType == Enums.ColumnChangeType.NewValue) {
                right = provider.EscapeParam(column.GetValue());
            } else {
                var newValue = col.GetNewValue();
                if (object.Equals(null,newValue)) {
                    right = "NULL";
                } else {
                    right = newValue.ToSql(provider, tableCount);
                }
            }
            return left + " = " + right;
        }

        internal SqlBuilder GetSqlBuilder()
        {
            if (_table != null) {
                return _table.GetSqlBuilder();
            }
            foreach (var item in _functionArgs) {
                if (item is QColumn) {
                    var sb = ((QColumn)item).GetSqlBuilder();
                    if (sb != null) {
                        return sb;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }



}
