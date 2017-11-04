using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Providers
{
    public class SqlServer2012DatabaseProvider : SqlServerDatabaseProvider { }

    public class SqlServerDatabaseProvider : DatabaseProvider
    {
        protected static readonly Dictionary<string, string> functionDict = new Dictionary<string, string>();

        static SqlServerDatabaseProvider()
        {
            functionDict.Add("LEN", "LENGTH({0})");
            functionDict.Add("DATEDIFF", "DATEDIFF(day,{0},{1})");
            functionDict.Add("YEAR", "DATEPART(YEAR,{0})");
            functionDict.Add("MONTH", "DATEPART(MONTH,{0})");
            functionDict.Add("DAY", "DATEPART(DAY,{0})");
            functionDict.Add("HOUR", "DATEPART(HOUR,{0})");
            functionDict.Add("MINUTE", "DATEPART(MINUTE,{0})");
            functionDict.Add("SECOND", "DATEPART(SECOND,{0})");
            functionDict.Add("DAYOFYEAR", "DATEPART(DAYOFYEAR,{0})");
            functionDict.Add("WEEK", "DATEPART(WEEK,{0})");
            functionDict.Add("WEEKDAY", "DATEPART(WEEKDAY,{0})");
        }

        public override string Delete(List<QTable> tables, QColumnBase pk, string tableName, string fromtable, string jointables, string where)
        {
            return "DELETE t1 FROM " + fromtable
                + " " + jointables
                + " WHERE " + where;
        }

        public override string Update(List<QTable> tables, string setValues, string fromtable, string jointables, string where)
        {
            return "UPDATE t1 SET " + setValues + " FROM " + fromtable + " " + jointables
                   + " WHERE " + where;
        }

        public override string Select(List<QTable> tables, bool useDistinct, int limit, int offset, List<string> selectColumns, string fromtable, string jointables, string where, string order, string group, string having)
        {
            if (offset <= 0) {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT ");
                if (useDistinct) sb.Append("DISTINCT ");
                sb.Append("TOP ");
                sb.Append(limit);
                sb.Append(" ");
                sb.Append(string.Join(",", selectColumns));
                sb.Append(" FROM ");
                sb.Append(fromtable);
                if (string.IsNullOrEmpty(jointables) == false) {
                    sb.Append(" ");
                    sb.Append(jointables);
                }
                if (string.IsNullOrEmpty(where) == false) {
                    sb.Append(" WHERE ");
                    sb.Append(where);
                }
                if (string.IsNullOrEmpty(group) == false) {
                    sb.Append(" GROUP BY ");
                    sb.Append(group);
                }
                if (string.IsNullOrEmpty(having) == false) {
                    sb.Append(" HAVING ");
                    sb.Append(having);
                }
                if (string.IsNullOrEmpty(order) == false) {
                    sb.Append(" ORDER BY ");
                    sb.Append(order);
                }
                return sb.ToString();
            }
            if (useDistinct) {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ({0})) peta_rn,", order ?? "SELECT NULL");
                sb.Append(string.Join(",", selectColumns));
                sb.Append(" FROM ");
                sb.Append(fromtable);
                if (string.IsNullOrEmpty(jointables) == false) {
                    sb.Append(" ");
                    sb.Append(jointables);
                }
                if (string.IsNullOrEmpty(where) == false) {
                    sb.Append(" WHERE ");
                    sb.Append(where);
                }
                if (string.IsNullOrEmpty(group) == false) {
                    sb.Append(" GROUP BY ");
                    sb.Append(group);
                }
                if (string.IsNullOrEmpty(having) == false) {
                    sb.Append(" HAVING ");
                    sb.Append(having);
                }
                sb.AppendFormat(") peta_paged WHERE peta_rn>{0} AND peta_rn<={1}", offset, limit + offset);
                return sb.ToString();
            } else {
                var fields = GetOutSelectColumns(selectColumns);
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ({0})) peta_rn,{1} FROM (", order ?? "SELECT NULL", fields);
                sb.Append(fromtable);
                if (string.IsNullOrEmpty(jointables) == false) {
                    sb.Append(" ");
                    sb.Append(jointables);
                }
                if (string.IsNullOrEmpty(where) == false) {
                    sb.Append(" WHERE ");
                    sb.Append(where);
                }
                if (string.IsNullOrEmpty(group) == false) {
                    sb.Append(" GROUP BY ");
                    sb.Append(group);
                }
                if (string.IsNullOrEmpty(having) == false) {
                    sb.Append(" HAVING ");
                    sb.Append(having);
                }
                sb.AppendFormat("))  peta_paged WHERE peta_rn>{0} AND peta_rn<={1}", offset, limit + offset);
                return sb.ToString();
            }
        }

        private string GetOutSelectColumns(List<string> columns)
        {
            List<string> list = new List<string>();
            foreach (var item in columns) {
                var sp = item.Split("[]'\"` ".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                list.Add(sp.Last());
            }
            return string.Join(",", list);
        }

        /// <summary>
        /// 方法是否使用默认格式化
        /// </summary>
        /// <param name="funName"></param>
        /// <returns></returns>
        public override bool IsFunctionUseDefaultFormat(string funName)
        {
            return functionDict.ContainsKey(funName) == false;
        }
        /// <summary>
        /// 获取非默认格式化
        /// </summary>
        /// <param name="sqlType"></param>
        /// <param name="funName"></param>
        /// <returns></returns>
        public override string GetFunctionFormat(string funName)
        {
            return functionDict[funName];
        }
    }
}
