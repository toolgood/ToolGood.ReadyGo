using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;




namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class SqlBuilder
    {
        string GetWhere(DatabaseProvider provider)
        {
            if (whereCondition.WhereType == Enums.WhereType.None) {
                return null;
            }
            return (whereCondition).ToSql(provider, _tables.Count);
        }

        string GetUpdateWhere(DatabaseProvider provider)
        {
            var where = GetWhere(provider);
            if (string.IsNullOrEmpty(where) == false) {
                return where;
            }
            var table = _tables[0];
            if (table.IsSetPrimaryKey()) {
                var pk = table.GetPrimaryKey();
                if (pk._changeType == Enums.ColumnChangeType.NewValue) {
                    return pk.ToSql(provider, _tables.Count) + " = " + provider.EscapeParam(pk.GetValue());
                }
                return pk.ToSql(provider, _tables.Count) + " = " + pk.GetNewValue().ToSql(provider, _tables.Count);
            }
            throw new ArgumentNullException("where is not set!");
        }

        string GetFromTable(DatabaseProvider provider)
        {
            var table = _tables[0];
            return table.ToTableSql(provider, _tables.Count);
        }

        string GetJoinSql(DatabaseProvider provider)
        {
            if (_tables.Count == 0 && string.IsNullOrWhiteSpace(_joinOnText)) return null;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i < _tables.Count; i++) {
                var table = _tables[i];
                stringBuilder.Append(" ");
                var joinsql = table.ToJoinSql(provider, _tables.Count);
                stringBuilder.Append(joinsql);
            }
            if (string.IsNullOrWhiteSpace(_joinOnText) == false) {
                stringBuilder.Append(" ");
                stringBuilder.Append(_joinOnText);
            }
            return stringBuilder.ToString();
        }

        string GetOrderSql(DatabaseProvider provider)
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

        string GetGroupBySql(DatabaseProvider provider)
        {
            if (_groupBy.Count == 0) return null;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in _groupBy) {
                stringBuilder.Append(",");
                var col = (item).ToSql(provider, _tables.Count);
                stringBuilder.Append(col);
            }
            stringBuilder.Remove(0, 1);
            return stringBuilder.ToString();
        }

        string GetHavingSql(DatabaseProvider provider)
        {
            if (_having.Count == 0) return null;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in _having) {
                stringBuilder.Append(",");
                var col = (item).ToSql(provider, _tables.Count);
                stringBuilder.Append(col);
            }
            stringBuilder.Remove(0, 1);
            return stringBuilder.ToString();
        }

        string GetUpdateSetSql(DatabaseProvider provider)
        {
            var table = _tables[0];
            var cols = table.GetUpdateColumns();
            if (cols.Count == 0) return null;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in cols) {
                stringBuilder.Append(",");
                var col = (item).ToUpdateSet(provider, _tables.Count);
                stringBuilder.Append(col);
            }
            stringBuilder.Remove(0, 1);
            return stringBuilder.ToString();
        }
        string GetInsertHeaderSql(DatabaseProvider provider)
        {
            var table = _tables[0];
            var cols = table.GetInsertColumns();
            if (cols.Count == 0) return null;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("INSERT INTO ");
            stringBuilder.Append(table.ToTableSql(provider, 0));
            stringBuilder.Append(" (");
            for (int i = 0; i < cols.Count; i++) {
                var col = cols[i];
                if (col._isPrimaryKey && col._isAutoIncrement) continue;
                if ("boolean|int16|int32|int64|uint16|uint32|uint64|single|double|decimal".Contains(col._fieldType) == false) {
                    if (col._changeType == Enums.ColumnChangeType.None) continue;
                }
                stringBuilder.Append((col).ToSql(provider, 0));
                stringBuilder.Append(",");
            }
            stringBuilder[stringBuilder.Length - 1] = ')';
            //stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        string GetInsertValueSql(DatabaseProvider provider)
        {
            var table = _tables[0];
            var cols = table.GetInsertColumns();
            if (cols.Count == 0) return null;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            for (int i = 0; i < cols.Count; i++) {
                var col = cols[i];
                if (col._isPrimaryKey && col._isAutoIncrement) continue;
                if ("boolean|int16|int32|int64|uint16|uint32|uint64|single|double|decimal".Contains(col._fieldType) == false) {
                    if (col._changeType == Enums.ColumnChangeType.None) continue;
                }
                var obj = provider.EscapeParam(col.GetValue());
                stringBuilder.Append(obj);
                stringBuilder.Append(",");
            }
            stringBuilder[stringBuilder.Length - 1] = ')';
            return stringBuilder.ToString();
        }

        string GetFullInsertSql(DatabaseProvider provider)
        {
            var header = GetInsertHeaderSql(provider);
            if (header == null) throw new NoColumnException();
            var value = GetInsertValueSql(provider);
            return header + " VALUES " + value ;
        }

        string GetFullUpdateSql(DatabaseProvider provider)
        {
            var sb = this;
            var where = sb.GetUpdateWhere(provider);
            if (where == null) throw new NoWhereException();

            if (_tables.Count == 1) {
                var setValues = GetUpdateSetSql(provider);

                var table = _tables[0];
                return "UPDATE " + table.ToTableSql(provider, _tables.Count)
                       + " SET " + setValues
                       + " WHERE " + where;
            } else {
                var setValues = GetUpdateSetSql(provider);
                var table = _tables[0];
                var fromtable = table.ToTableSql(provider, _tables.Count);
                var jointables = sb.GetJoinSql(provider);

                return provider.Update(_tables, setValues, fromtable, jointables, where);
            }
        }

        string GetFullDeleteSql(DatabaseProvider provider)
        {
            var sb = this;
            var where = sb.GetUpdateWhere(provider);
            if (where == null) throw new NoWhereException();
            var table = _tables[0];

            if (_tables.Count == 1) {
                return "DELETE FROM " + table.ToTableSql(provider, _tables.Count) + " WHERE " + where;
            }

            var tableName = table.ToSql(provider);
            var fromtable = table.ToTableSql(provider, _tables.Count);
            var jointables = sb.GetJoinSql(provider);
            var pk = table.GetPrimaryKey();
            return provider.Delete(_tables, pk, tableName, fromtable, jointables, where);
        }

        string GetFullSelectSql(DatabaseProvider provider, int limit, int offset, List<string> selectColumns)
        {
            var where = GetWhere(provider);
            var fromTable = GetFromTable(provider);
            var joinTable = GetJoinSql(provider);
            var orderSql = GetOrderSql(provider);
            var groupSql = GetGroupBySql(provider);
            var havingSql = GetHavingSql(provider);

            if (limit <= 0) {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT ");
                if (_useDistinct) sb.Append("DISTINCT ");
                sb.Append(string.Join(",", selectColumns));
                sb.Append(" FROM ");
                sb.Append(fromTable);
                if (string.IsNullOrEmpty(joinTable) == false) {
                    if (_tables.Count == 1) { sb.Append(" AS  " + _tables[0]._asName); }
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
