using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Enums;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
 
namespace ToolGood.ReadyGo3.DataCentxt.Providers
{
    /// <summary>
    /// Ms Access 
    /// </summary>
    public class MsAccessDbDatabaseProvider : DatabaseProvider
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
                case SqlFunction.Len: break;
                case SqlFunction.Max: break;
                case SqlFunction.Min: break;
                case SqlFunction.Avg: break;
                case SqlFunction.Sum: break;
                case SqlFunction.Count: break;
                case SqlFunction.CountDistinct: break;
                case SqlFunction.DatePart: break;
                case SqlFunction.DateDiff: break;
                case SqlFunction.Year: break;
                case SqlFunction.Month: break;
                case SqlFunction.Day: break;
                case SqlFunction.Hour: break;
                case SqlFunction.Minute: break;
                case SqlFunction.Second: break;
                case SqlFunction.DayOfYear: break;
                case SqlFunction.Week: break;
                case SqlFunction.WeekDay: break;
                case SqlFunction.SubString3: return CreateFunction("MID({0}, {1}, {2})", args);
                case SqlFunction.SubString2: return CreateFunction("MID({0}, {1})", args);
                case SqlFunction.Left: break;
                case SqlFunction.Right: break;
                case SqlFunction.Lower: return CreateFunction("LCASE({0})", args);
                case SqlFunction.Upper: return CreateFunction("UCASE({0})", args);
                case SqlFunction.Ascii: return CreateFunction("ASC({0})", args);
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
            //http://bbs.csdn.net/topics/340167958
            return $"DELETE distinctrow t1.* FROM {fromtable} {jointables} WHERE {where};";
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
            // http://blog.csdn.net/hsg77/article/details/6128253
            return $"UPDATE {fromtable} {jointables} SET {setValues} WHERE {where}; " ;
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
            throw new DatabaseUnsupportException();
        }

 
    }
}
