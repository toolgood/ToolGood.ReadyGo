using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.Exceptions;
using ToolGood.ReadyGo3.PetaPoco.Core;
using ToolGood.ReadyGo3.PetaPoco.Internal;

namespace ToolGood.ReadyGo3.Gadget
{
    internal static class CrudCache
    {
        private static Cache<string, string> _insert = new Cache<string, string>();
        private static Cache<string, string> _selectColumns = new Cache<string, string>();
        private static Cache<string, string> _update = new Cache<string, string>();

        public static string GetInsertSql(DatabaseProvider _provider, string _paramPrefix, PocoData pd, int size, string tableName, string primaryKeyName, bool autoIncrement)
        {
            return _insert.Get($"{_provider.ToString()}|{_paramPrefix}|{size}|{pd.ToString()}", () => {
                var names = new List<string>();
                var values = new List<string>();
                var index = 0;
                foreach (var i in pd.Columns) {
                    if (i.Value.ResultColumn) continue;
                    // Don't insert the primary key (except under oracle where we need bring in the next sequence value)
                    if (autoIncrement && primaryKeyName != null && string.Compare(i.Value.ColumnName, primaryKeyName, true) == 0) {
                        // Setup auto increment expression
                        string autoIncExpression = _provider.GetAutoIncrementExpression(pd.TableInfo);
                        if (autoIncExpression != null) {
                            names.Add(i.Value.ColumnName);
                            values.Add(autoIncExpression);
                        }
                        continue;
                    }
                    names.Add(_provider.EscapeSqlIdentifier(i.Value.ColumnName));
                    values.Add(_paramPrefix + index.ToString());
                    index++;
                }

                string outputClause = String.Empty;
                if (autoIncrement) {
                    outputClause = _provider.GetInsertOutputClause(primaryKeyName);
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("INSERT INTO {0} ({1}){2} VALUES ({3})",
                    _provider.EscapeTableName(tableName),
                    string.Join(",", names.ToArray()),
                    outputClause,
                    string.Join(",", values.ToArray())
                    );
                if (size > 1) {
                    var k = index;
                    for (int i = 1; i < size; i++) {
                        sb.Append(",(");
                        for (int j = 0; j < k; j++) {
                            if (j > 0) { sb.Append(","); }
                            sb.Append(_paramPrefix);
                            sb.Append(index.ToString());
                            index++;
                        }
                        sb.Append(")");
                    }
                }
                return sb.ToString();
            });
        }

        public static string GetUpdateSql(DatabaseProvider _provider, string _paramPrefix, PocoData pd, string tableName, string primaryKeyName)
        {
            return _update.Get($"{_provider.ToString()}|{_paramPrefix}|{pd.ToString()}", () => {
                var sb = new StringBuilder();
                var index = 0;
                foreach (var i in pd.Columns) {
                    if (String.Compare(i.Value.ColumnName, primaryKeyName, StringComparison.OrdinalIgnoreCase) == 0) continue;
                    if (i.Value.ResultColumn) continue;

                    // Build the sql
                    if (index > 0) sb.Append(", ");
                    sb.AppendFormat("{0} = {1}{2}", _provider.EscapeSqlIdentifier(i.Value.ColumnName), _paramPrefix, index++);
                }
                return $"UPDATE {_provider.EscapeTableName(tableName)} SET {sb.ToString()} WHERE { _provider.EscapeSqlIdentifier(primaryKeyName)} = { _paramPrefix}{ index++}";
            });
        }

        public static string GetSelectColumnsSql(DatabaseProvider _provider,  PocoData pd)
        {
            return _selectColumns.Get($"{_provider.ToString()}|{pd.ToString()}", () => {
                var tableName = _provider.EscapeTableName(pd.TableInfo.TableName);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in pd.Columns) {
                    var col = item.Value;

                    if (col.ResultColumn) {
                        if (string.IsNullOrEmpty(col.ResultSql) == false) {
                            stringBuilder.Append(",");
                            stringBuilder.AppendFormat(col.ResultSql, tableName + ".");
                            stringBuilder.Append(" AS '");
                            stringBuilder.Append(col.ColumnName);
                            stringBuilder.Append("'");
                        }
                    } else {
                        stringBuilder.Append(",");
                        stringBuilder.AppendFormat("{0}.{1}", tableName, _provider.EscapeSqlIdentifier(col.ColumnName));
                    }
                }
                if (stringBuilder.Length == 0) throw new NoColumnException();
                stringBuilder.Remove(0, 1);
                return stringBuilder.ToString();
            });
        }


    }
}
