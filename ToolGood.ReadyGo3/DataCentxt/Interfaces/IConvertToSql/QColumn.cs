using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
using ToolGood.ReadyGo3.DataCentxt.Internals;

namespace ToolGood.ReadyGo3.DataCentxt.Interfaces
{
    public interface IColumnConvert
    {
        string ToSelectColumn(DatabaseProvider provider, int tableCount);

        string ToSql(DatabaseProvider provider, int tableCount);

        string ToUpdateSet(DatabaseProvider provider, int tableCount);

        SqlBuilder GetSqlBuilder();
    }
}


namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QColumnBase : IColumnConvert
    {
        string IColumnConvert.ToSelectColumn(DatabaseProvider provider, int tableCount)
        {
            if (this._isResultColumn && string.IsNullOrEmpty(this._resultSql) == false) {
                var sql = _resultSql.Replace("{0}", ((ITableConvert)_table).ToSql(provider, false)+".");
                sql = _table.GetSqlHelper().formatSql(sql);
                return sql + " AS '" + _columnName + "'";
            }
            if (string.IsNullOrEmpty(_asName)) {
                return ((IColumnConvert)this).ToSql(provider, tableCount);
            }
            return ((IColumnConvert)this).ToSql(provider, tableCount) + " AS '" + _asName + "'";
        }

        string IColumnConvert.ToSql(DatabaseProvider provider, int tableCount)
        {
            if (_columnType == Enums.ColumnType.None) throw new ColumnTypeException();
            if (_columnType == Enums.ColumnType.Value)
                return provider.ConvertTo(((QTableColumn)this).GetValue());

            if (_columnType == Enums.ColumnType.Column) {
                if (tableCount > 1) {
                    return _table._asName + provider.EscapeSqlIdentifier(_columnName);
                }
                return provider.EscapeSqlIdentifier(_columnName);
            }
            if (_columnType == Enums.ColumnType.Code) {
                return _code;
            }

            string function = _functionFormat;
            if (_columnType == Enums.ColumnType.Function) {
                if (provider.IsFunctionUseDefaultFormat(_functionName) == false) {
                    function = provider.GetFunctionFormat(_functionName);
                }
            }
            var args = new string[_functionArgs.Length];
            for (int i = 0; i < _functionArgs.Length; i++) {
                var item = _functionArgs[i];
                if (item is QColumnBase) {
                    args[i] = ((IColumnConvert)item).ToSql(provider, tableCount);
                } else {
                    args[i] = provider.ConvertTo(item);
                }
            }
            return string.Format(function, args);
        }

        string IColumnConvert.ToUpdateSet(DatabaseProvider provider, int tableCount)
        {
            var col = this as QTableColumn;
            if (object.Equals(col, null)) throw new NullReferenceException();
            if (col._changeType == Enums.ColumnChangeType.None) throw new ArgumentNullException();
            var left = ((IColumnConvert)col).ToSql(provider, tableCount);

            var column = ((QTableColumn)this);
            string right;
            if (column._changeType == Enums.ColumnChangeType.NewValue) {
                right = provider.ConvertTo(column.GetValue());
            } else {
                right = ((IColumnConvert)col.GetNewValue()).ToSql(provider, tableCount);
            }

            return left + " = " + right;
        }

        SqlBuilder IColumnConvert.GetSqlBuilder()
        {
            if (_table != null) {
                return _table.GetSqlBuilder();
            }
            foreach (var item in _functionArgs) {
                if (item is QColumnBase) {
                    var sb = ((IColumnConvert)item).GetSqlBuilder();
                    if (sb != null) {
                        return sb;
                    }
                }
            }
            return null;
        }
    }



}
