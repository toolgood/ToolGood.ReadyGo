using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Enums;
//using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Providers
{
    public class PostgreSQLDatabaseProvider : DatabaseProvider
    {
 
        public override string CreateFunction(SqlFunction function, params object[] args)
        {
            //  https://www.postgresql.org/docs/9.1/static/functions-math.html
            //  https://www.postgresql.org/docs/9.1/static/functions-string.html
            //  https://www.postgresql.org/docs/9.1/static/functions-datetime.html
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
                case SqlFunction.DateDiff: return CreateFunction("AGE(TIMESTAMP {0}, TIMESTAMP {1})", args);
                case SqlFunction.Year: return CreateFunction("EXTRACT(YEAR FROM TIMESTAMP {0})", args);
                case SqlFunction.Month: return CreateFunction("EXTRACT(MONTH FROM TIMESTAMP {0})", args);
                case SqlFunction.Day: return CreateFunction("EXTRACT(DAY FROM TIMESTAMP {0})", args);
                case SqlFunction.Hour: return CreateFunction("EXTRACT(HOUR FROM TIMESTAMP {0})", args);
                case SqlFunction.Minute: return CreateFunction("EXTRACT(MINUTE FROM TIMESTAMP {0})", args);
                case SqlFunction.Second: return CreateFunction("EXTRACT(SECOND FROM TIMESTAMP {0})", args);
                case SqlFunction.DayOfYear: return CreateFunction("EXTRACT(DAYOFYEAR FROM TIMESTAMP {0})", args);
                case SqlFunction.Week: return CreateFunction("EXTRACT(WEEK FROM TIMESTAMP {0})", args);
                case SqlFunction.WeekDay: return CreateFunction("EXTRACT(WEEKDAY FROM TIMESTAMP {0})", args);
                case SqlFunction.SubString3: return CreateFunction("SUBSTR({0}, {1}, {2})", args);
                case SqlFunction.SubString2: return CreateFunction("SUBSTR({0}, {1})", args);
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

        public PostgreSQLDatabaseProvider()
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
        



        public override string EscapeSqlIdentifier(  string sqlIdentifier)
        {
            return $"\"{sqlIdentifier}\"";
        }

 
    }
}
