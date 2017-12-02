using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Interfaces
{
    public interface ISqlBuilderConvert
    {
        string GetWhere(DatabaseProvider provider);
        string GetUpdateWhere(DatabaseProvider provider);
        string GetFromTable(DatabaseProvider provider);
        string GetJoinSql(DatabaseProvider provider);
        string GetOrderSql(DatabaseProvider provider);
        string GetGroupBySql(DatabaseProvider provider);
        string GetHavingSql(DatabaseProvider provider);
        string GetUpdateSetSql(DatabaseProvider provider);
        string GetInsertHeaderSql(DatabaseProvider provider);
        string GetInsertValueSql(DatabaseProvider provider);

        string GetFullInsertSql(DatabaseProvider provider);
        string GetFullUpdateSql(DatabaseProvider provider);
        string GetFullDeleteSql(DatabaseProvider provider);


        string GetFullSelectSql(DatabaseProvider provider, int limit, int offset, List<string> selectColumns);
    }
}

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class SqlBuilder : ISqlBuilderConvert
    {
        string ISqlBuilderConvert.GetWhere(DatabaseProvider provider)
        {
            if (whereCondition.WhereType == Enums.WhereType.None) {
                return null;
            }
            return ((IConditionConvert)whereCondition).ToSql(provider, _tables.Count);
        }

        string ISqlBuilderConvert.GetUpdateWhere(DatabaseProvider provider)
        {
            var where = ((ISqlBuilderConvert)this).GetWhere(provider);
            if (string.IsNullOrEmpty(where) == false) {
                return where;
            }
            var table = _tables[0];
            if (((ITableConvert)table).IsSetPrimaryKey()) {
                var pk = ((ITableConvert)table).GetPrimaryKey();
                QColumnValueCondition condition;
                if (pk._fieldType == "string") {
                    condition = (QColumnValueCondition)(pk == (string)pk.GetValue());
                } else if (pk._fieldType == "int32") {
                    condition = (QColumnValueCondition)(pk == (int)pk.GetValue());
                } else if (pk._fieldType == "int64") {
                    condition = (QColumnValueCondition)(pk == (long)pk.GetValue());
                } else if (pk._fieldType == "uint32") {
                    condition = (QColumnValueCondition)(pk == (uint)pk.GetValue());
                } else if (pk._fieldType == "uint64") {
                    condition = (QColumnValueCondition)(pk == (ulong)pk.GetValue());
                } else if (pk._fieldType == "guid") {
                    condition = (QColumnValueCondition)(pk == ((Guid)pk.GetValue()).ToString());
                } else {
                    condition = (QColumnValueCondition)(pk == (int)pk.GetValue());
                }

                return ((IConditionConvert)condition).ToSql(provider, _tables.Count);
            }
            throw new ArgumentNullException("where is not set!");
        }

        string ISqlBuilderConvert.GetFromTable(DatabaseProvider provider)
        {
            var table = _tables[0];
            return ((ITableConvert)table).ToTableSql(provider, _tables.Count, _usedSchemaName);
        }

        string ISqlBuilderConvert.GetJoinSql(DatabaseProvider provider)
        {
            if (_tables.Count == 0 && string.IsNullOrWhiteSpace(_joinOnText)) return null;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i < _tables.Count; i++) {
                var table = _tables[i];
                stringBuilder.Append(" ");
                var joinsql = ((ITableConvert)table).ToJoinSql(provider, _tables.Count, _usedSchemaName);
                stringBuilder.Append(joinsql);
            }
            if (string.IsNullOrWhiteSpace(_joinOnText) == false) {
                stringBuilder.Append(" ");
                stringBuilder.Append(_joinOnText);
            }
            return stringBuilder.ToString();
        }

        string ISqlBuilderConvert.GetOrderSql(DatabaseProvider provider)
        {
            if (_orderBys.Count == 0) return null;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in _orderBys) {
                stringBuilder.Append(",");
                var order = item.ToSql(provider, _tables.Count);
                stringBuilder.Append(order);
            }
            stringBuilder.Remove(0, 1);
            return stringBuilder.ToString();
        }

        string ISqlBuilderConvert.GetGroupBySql(DatabaseProvider provider)
        {
            if (_groupBy.Count == 0) return null;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in _groupBy) {
                stringBuilder.Append(",");
                var col = ((IColumnConvert)item).ToSql(provider, _tables.Count);
                stringBuilder.Append(col);
            }
            stringBuilder.Remove(0, 1);
            return stringBuilder.ToString();
        }

        string ISqlBuilderConvert.GetHavingSql(DatabaseProvider provider)
        {
            if (_having.Count == 0) return null;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in _having) {
                stringBuilder.Append(",");
                var col = ((IConditionConvert)item).ToSql(provider, _tables.Count);
                stringBuilder.Append(col);
            }
            stringBuilder.Remove(0, 1);
            return stringBuilder.ToString();
        }

        string ISqlBuilderConvert.GetUpdateSetSql(DatabaseProvider provider)
        {
            var table = _tables[0];
            var cols = ((ITableConvert)table).GetUpdateColumns();
            if (cols.Count == 0) return null;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in cols) {
                stringBuilder.Append(",");
                var col = ((IColumnConvert)item).ToUpdateSet(provider, _tables.Count);
                stringBuilder.Append(col);
            }
            stringBuilder.Remove(0, 1);
            return stringBuilder.ToString();
        }

        string ISqlBuilderConvert.GetInsertHeaderSql(DatabaseProvider provider)
        {
            var table = (ITableConvert)_tables[0];
            var cols = table.GetInsertColumns();
            if (cols.Count == 0) return null;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("INSERT INTO ");
            stringBuilder.Append(table.ToTableSql(provider, 0, _usedSchemaName));
            stringBuilder.Append(" (");
            for (int i = 0; i < cols.Count; i++) {
                var col = cols[i];
                if (col._changeType == Enums.ColumnChangeType.None) continue;
    
                stringBuilder.Append(((IColumnConvert)col).ToSql(provider, 0));
                stringBuilder.Append(",");
            }
            stringBuilder[stringBuilder.Length - 1] = ')';
            //stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        string ISqlBuilderConvert.GetInsertValueSql(DatabaseProvider provider)
        {
            var table = (ITableConvert)_tables[0];
            var cols = table.GetInsertColumns();
            if (cols.Count == 0) return null;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            for (int i = 0; i < cols.Count; i++) {
                var col = cols[i];
                if (col._changeType == Enums.ColumnChangeType.None) continue;
 
                var obj = provider.ConvertTo(col.GetValue());
                stringBuilder.Append(obj);
                stringBuilder.Append(",");

            }
            stringBuilder[stringBuilder.Length - 1] = ')';
            return stringBuilder.ToString();
        }

        string ISqlBuilderConvert.GetFullInsertSql(DatabaseProvider provider)
        {
            var header = ((ISqlBuilderConvert)this).GetInsertHeaderSql(provider);
            if (header == null) throw new NoColumnException();
            var value = ((ISqlBuilderConvert)this).GetInsertValueSql(provider);
            return header + " VALUES " + value + ";";
        }

        string ISqlBuilderConvert.GetFullUpdateSql(DatabaseProvider provider)
        {
            var sb = (ISqlBuilderConvert)this;
            var where = sb.GetUpdateWhere(provider);
            if (where == null) throw new NoWhereException();

            if (_tables.Count == 1) {
                var setValues = ((ISqlBuilderConvert)this).GetUpdateSetSql(provider);

                var table = (ITableConvert)_tables[0];
                return "UPDATE " + table.ToTableSql(provider, _tables.Count, _usedSchemaName)
                       + " SET " + setValues
                       + " WHERE " + where;
            } else {
                var setValues = ((ISqlBuilderConvert)this).GetUpdateSetSql(provider);
                var table = (ITableConvert)_tables[0];
                var fromtable = table.ToTableSql(provider, _tables.Count, _usedSchemaName);
                var jointables = sb.GetJoinSql(provider);

                return provider.Update(_tables, setValues, fromtable, jointables, where);
            }
        }

        string ISqlBuilderConvert.GetFullDeleteSql(DatabaseProvider provider)
        {
            var sb = (ISqlBuilderConvert)this;
            var where = sb.GetUpdateWhere(provider);
            if (where == null) throw new NoWhereException();
            var table = (ITableConvert)_tables[0];

            if (_tables.Count == 1) {
                return "DELETE FROM " + table.ToTableSql(provider, _tables.Count, _usedSchemaName) + " WHERE " + where;
            }

            var tableName = table.ToSql(provider, _usedSchemaName);
            var fromtable = table.ToTableSql(provider, _tables.Count, _usedSchemaName);
            var jointables = sb.GetJoinSql(provider);
            var pk = table.GetPrimaryKey();
            return provider.Delete(_tables, pk, tableName, fromtable, jointables, where);
        }

        string ISqlBuilderConvert.GetFullSelectSql(DatabaseProvider provider, int limit, int offset, List<string> selectColumns)
        {
            var where = ((ISqlBuilderConvert)this).GetWhere(provider);
            var fromTable = ((ISqlBuilderConvert)this).GetFromTable(provider);
            var joinTable = ((ISqlBuilderConvert)this).GetJoinSql(provider);
            var orderSql = ((ISqlBuilderConvert)this).GetOrderSql(provider);
            var groupSql = ((ISqlBuilderConvert)this).GetGroupBySql(provider);
            var havingSql = ((ISqlBuilderConvert)this).GetHavingSql(provider);

            if (limit <= 0) {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT ");
                if (_useDistinct) sb.Append("DISTINCT ");
                sb.Append(string.Join(",", selectColumns));
                sb.Append(" FROM ");
                sb.Append(fromTable);
                if (string.IsNullOrEmpty(joinTable) == false) {
                    sb.Append(" ");
                    sb.Append(joinTable);
                }
                if (string.IsNullOrEmpty(where) == false) {
                    sb.Append(" WHERE ");
                    sb.Append(where);
                }
                if (string.IsNullOrEmpty(groupSql) == false) {
                    sb.Append(" GROUP BY ");
                    sb.Append(groupSql);
                }
                if (string.IsNullOrEmpty(havingSql) == false) {
                    sb.Append(" HAVING ");
                    sb.Append(havingSql);
                }
                if (string.IsNullOrEmpty(orderSql) == false) {
                    sb.Append(" ORDER BY ");
                    sb.Append(orderSql);
                }
                return sb.ToString();
            }
            return provider.Select(_tables, _useDistinct, limit, offset, selectColumns, fromTable, joinTable, where,
                orderSql, groupSql, havingSql);
        }





    }


}
