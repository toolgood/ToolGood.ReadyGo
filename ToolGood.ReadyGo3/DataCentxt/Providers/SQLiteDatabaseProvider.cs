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
    public class SQLiteDatabaseProvider : DatabaseProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override string CreateFunction(SqlFunction function, params object[] args)
        {
            // http://www.sqlite.org/lang_corefunc.html
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
                case SqlFunction.DateDiff: return CreateFunction("FLOOR(CAST(STRFTIME('%J',{0}) AS INT) - CAST(STRFTIME('%J',{1}) AS INT))", args);
                case SqlFunction.Year: return CreateFunction("CAST(STRFTIME('%Y',{0}) AS INT)", args);
                case SqlFunction.Month: return CreateFunction("CAST(STRFTIME('%m',{0}) AS INT)", args);
                case SqlFunction.Day: return CreateFunction("CAST(STRFTIME('%d',{0}) AS INT)", args);
                case SqlFunction.Hour: return CreateFunction("CAST(STRFTIME('%H',{0}) AS INT)", args);
                case SqlFunction.Minute: return CreateFunction("CAST(STRFTIME('%M',{0}) AS INT)", args);
                case SqlFunction.Second: return CreateFunction("CAST(STRFTIME('%S',{0}) AS INT)", args);
                case SqlFunction.DayOfYear: return CreateFunction("CAST(STRFTIME('%j',{0}) AS INT)", args);
                case SqlFunction.Week: return CreateFunction("CAST(STRFTIME('%W',{0}) AS INT)", args);
                case SqlFunction.WeekDay: return CreateFunction("CAST(STRFTIME('%w',{0}) AS INT)", args);
                case SqlFunction.SubString3: return CreateFunction("SUBSTR({0},{1},{2})", args);
                case SqlFunction.SubString2: return CreateFunction("SUBSTR({0},{1})", args);
                case SqlFunction.Left: return CreateFunction("SUBSTR({0},1,{1})", args);
                case SqlFunction.Right: return CreateFunction("SUBSTR({0},-{1})", args);
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
            return $"DELETE t1 FROM {fromtable} {jointables} WHERE {where};";
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
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            if (useDistinct) sb.Append("DISTINCT ");
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
            if (limit > 0) {
                sb.Append(" LIMIT ");
                if (offset > 0) {
                    sb.Append(offset);
                    sb.Append(",");
                }
                sb.Append(limit);
            }
            return sb.ToString();
        }



    }
}
