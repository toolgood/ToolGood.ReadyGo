using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Enums;


namespace ToolGood.ReadyGo3.DataCentxt.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlServer2012DatabaseProvider : SqlServerDatabaseProvider { }
    /// <summary>
    /// 
    /// </summary>
    public class SqlServerDatabaseProvider : DatabaseProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override string CreateFunction(SqlFunction function, params object[] args)
        {
            switch (function) {
                case SqlFunction.Fuction: break;
                case SqlFunction.Len: return CreateFunction("LENGTH({0})", args);
                case SqlFunction.Max: break;
                case SqlFunction.Min: break;
                case SqlFunction.Avg: break;
                case SqlFunction.Sum: break;
                case SqlFunction.Count: break;
                case SqlFunction.CountDistinct: break;
                case SqlFunction.DatePart: break;
                case SqlFunction.DateDiff: return CreateFunction("DATEDIFF(DAY,{0},{1})", args);
                case SqlFunction.Year: return CreateFunction("DATEPART(YEAR,{0})", args);
                case SqlFunction.Month: return CreateFunction("DATEPART(MONTH,{0})", args);
                case SqlFunction.Day: return CreateFunction("DATEPART(DAY,{0})", args);
                case SqlFunction.Hour: return CreateFunction("DATEPART(HOUR,{0})", args);
                case SqlFunction.Minute: return CreateFunction("DATEPART(MINUTE,{0})", args);
                case SqlFunction.Second: return CreateFunction("DATEPART(SECOND,{0})", args);
                case SqlFunction.DayOfYear: return CreateFunction("DATEPART(DAYOFYEAR,{0})", args);
                case SqlFunction.Week: return CreateFunction("DATEPART(WEEK,{0})", args);
                case SqlFunction.WeekDay: return CreateFunction("DATEPART(WEEKDAY,{0})", args);
                case SqlFunction.SubString3: break;
                case SqlFunction.SubString2: break;
                case SqlFunction.Left: break;
                case SqlFunction.Right: break;
                case SqlFunction.Lower: break;
                case SqlFunction.Upper: break;
                case SqlFunction.Ascii: break;
                case SqlFunction.Concat: break;
                default: break;
            }


            return base.CreateFunction(function, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="pk"></param>
        /// <param name="tableName"></param>
        /// <param name="fromtable"></param>
        /// <param name="jointables"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public override string Delete(List<QTable> tables, QColumn pk, string tableName, string fromtable, string jointables, string where)
        {
            return "DELETE t1 FROM " + fromtable
                + " " + jointables
                + " WHERE " + where;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="setValues"></param>
        /// <param name="fromtable"></param>
        /// <param name="jointables"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public override string Update(List<QTable> tables, string setValues, string fromtable, string jointables, string where)
        {
            return "UPDATE t1 SET " + setValues + " FROM " + fromtable + " " + jointables
                   + " WHERE " + where;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="useDistinct"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="selectColumns"></param>
        /// <param name="fromtable"></param>
        /// <param name="jointables"></param>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <param name="group"></param>
        /// <param name="having"></param>
        /// <returns></returns>
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
                sb.AppendFormat("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {0}) peta_rn,* FROM (", string.IsNullOrWhiteSpace(order) ? "(SELECT NULL)" : order);
                sb.Append("SELECT ");
                sb.Append("DISTINCT ");
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
                sb.AppendFormat(")) peta_paged WHERE peta_rn>{0} AND peta_rn<={1}", offset, limit + offset);
                return sb.ToString();
            } else {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {0})", string.IsNullOrWhiteSpace(order) ? "(SELECT NULL)" : order);
                sb.AppendFormat(" peta_rn,{0} ", selectColumns);
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
                sb.AppendFormat(")  peta_paged WHERE peta_rn>{0} AND peta_rn<={1}", offset, limit + offset);
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


    }
}
