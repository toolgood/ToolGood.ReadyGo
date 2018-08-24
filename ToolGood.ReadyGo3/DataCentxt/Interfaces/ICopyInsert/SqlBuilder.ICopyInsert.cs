using System;
using System.Collections.Generic;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class SqlBuilder
    {
        /// <summary>
        /// Insert Into T(*)  Select * from T
        /// </summary>
        /// <param name="insertTableName"></param>
        /// <param name="replaceColumns"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int SelectInsert(string insertTableName, string replaceColumns, object[] args)
        {
            var sql = CreateSelectInsertSql(_tables[0].GetTableType(), insertTableName, replaceColumns, args);
            return GetSqlHelper().Execute(sql);
        }




        /// <summary>
        /// Insert Into T1(*)  Select * from T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="insertTableName"></param>
        /// <param name="replaceColumns"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int SelectInsert<T>(string insertTableName, string replaceColumns, object[] args)
        {
            var sql = CreateSelectInsertSql(typeof(T), insertTableName, replaceColumns, args);
            return GetSqlHelper().Execute(sql);
        }


        private string CreateSelectInsertSql(Type type, string insertTableName, string replaceColumns, object[] args)
        {
            Dictionary<string, string> replaceCols = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(replaceColumns)==false) {
                var columnSqls = Provider.FormatSql(replaceColumns, args).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in columnSqls) {
                    var sp = item.Split(new char[] { '.', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var header = sp[sp.Length - 1].Replace("'", "").Replace("\"", ""); ;
                    replaceCols[header] = item;
                }
            }

            var pd = PetaPoco.Core.PocoData.ForType(type);
            var where = GetWhere(Provider);
            var fromTable = GetFromTable(Provider);
            var joinTable = GetJoinSql(Provider);
            var orderSql = GetOrderSql(Provider);
            var groupSql = GetGroupBySql(Provider);
            var havingSql = GetHavingSql(Provider);

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            if (string.IsNullOrEmpty(insertTableName)) {
                sb.Append(Provider.GetTableName(pd));
            } else {
                sb.Append(insertTableName);
            }
            sb.Append("(");
            Dictionary<string, string> selectColumns = new Dictionary<string, string>();
            var count = string.IsNullOrEmpty(joinTable) ? 0 : 1;
            foreach (var item in pd.Columns) {
                var colName = item.Key;
                if (colName == pd.TableInfo.PrimaryKey) continue;
                if (item.Value.ResultColumn) continue;

                if (replaceCols.TryGetValue(colName, out string sql)) {
                    selectColumns[Provider.EscapeSqlIdentifier(colName)] = sql;
                } else if (_tables[0]._columns.TryGetValue(colName.ToLower(), out QTableColumn column)) {
                    selectColumns[Provider.EscapeSqlIdentifier(colName)] = column.ToSelectColumn(Provider, _tables.Count + count);
                }
            }
            sb.Append(string.Join(",", selectColumns.Keys));
            sb.Append(") SELECT ");
            if (_useDistinct) sb.Append("DISTINCT ");
            sb.Append(string.Join(",", selectColumns.Values));

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


    }
}
