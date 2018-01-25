using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Enums;
//using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Providers
{
    public class FirebirdDbDatabaseProvider : DatabaseProvider
    {
 
        public override string CreateFunction(SqlFunction function, params object[] args)
        {
            //  http://www.firebirdsql.org/refdocs/langrefupd21.html
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
                case SqlFunction.DateDiff: return CreateFunction("DATEDIFF(day,{0},{1})", args);
                case SqlFunction.Year: return CreateFunction("EXTRACT(YEAR FROM {0})", args);
                case SqlFunction.Month: return CreateFunction("EXTRACT(MONTH FROM {0})", args);
                case SqlFunction.Day: return CreateFunction("EXTRACT(DAY FROM {0})", args);
                case SqlFunction.Hour: return CreateFunction("EXTRACT(HOUR FROM {0})", args);
                case SqlFunction.Minute: return CreateFunction("EXTRACT(MINUTE FROM {0})", args);
                case SqlFunction.Second: return CreateFunction("EXTRACT(SECOND FROM {0})", args);
                case SqlFunction.DayOfYear: return CreateFunction("EXTRACT(DAYOFYEAR FROM {0})", args);
                case SqlFunction.Week: return CreateFunction("EXTRACT(WEEK FROM {0})", args);
                case SqlFunction.WeekDay: return CreateFunction("EXTRACT(WEEKDAY FROM {0})", args);
                case SqlFunction.SubString3: return CreateFunction("SUBSTRING({0} FROM {1} FOR {2})", args);
                case SqlFunction.SubString2: return CreateFunction("SUBSTRING({0} FROM {1})", args);
                case SqlFunction.Left: return CreateFunction("SLEFT({0},{1})", args);
                case SqlFunction.Right: return CreateFunction("SRIGHT({0},{1})", args);
                case SqlFunction.Lower: break;
                case SqlFunction.Upper: break;
                case SqlFunction.Ascii: break;
                case SqlFunction.Concat: break;
                default: break;
            }
            return base.CreateFunction(function, args);
        }

        public FirebirdDbDatabaseProvider()
        {
            usedEscapeSql = true;
            escapeSql = '"';
        }

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
            sb.AppendFormat($" ROWS {offset + 1} TO {offset + limit}");
            return sb.ToString();
        }



        public override string EscapeSqlIdentifier(string sqlIdentifier)
        {
            return $"\"{sqlIdentifier}\"";
        }

 

    }
}
