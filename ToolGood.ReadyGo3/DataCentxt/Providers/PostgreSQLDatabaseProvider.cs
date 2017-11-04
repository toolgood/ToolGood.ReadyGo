using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Providers
{
    public class PostgreSQLDatabaseProvider : DatabaseProvider
    {
        protected static readonly Dictionary<string, string> functionDict = new Dictionary<string, string>();

        static PostgreSQLDatabaseProvider()
        {
            //  https://www.postgresql.org/docs/9.1/static/functions-math.html
            //  https://www.postgresql.org/docs/9.1/static/functions-string.html
            //  https://www.postgresql.org/docs/9.1/static/functions-datetime.html
            functionDict.Add( "LEN", "LENGTH({0})");
            functionDict.Add( "DATEDIFF", "AGE(TIMESTAMP {0}, TIMESTAMP {1})");
            functionDict.Add( "YEAR", "EXTRACT(YEAR FROM TIMESTAMP {0})");
            functionDict.Add( "MONTH", "EXTRACT(MONTH FROM TIMESTAMP {0})");
            functionDict.Add( "DAY", "EXTRACT(DAY FROM TIMESTAMP {0})");
            functionDict.Add( "HOUR", "EXTRACT(HOUR FROM TIMESTAMP {0})");
            functionDict.Add( "MINUTE", "EXTRACT(MINUTE FROM TIMESTAMP {0})");
            functionDict.Add( "SECOND", "EXTRACT(SECOND FROM TIMESTAMP {0})");
            functionDict.Add( "DAYOFYEAR", "EXTRACT(DAYOFYEAR FROM TIMESTAMP {0})");
            functionDict.Add( "WEEK", "EXTRACT(WEEK FROM TIMESTAMP {0})");
            functionDict.Add( "WEEKDAY", "EXTRACT(WEEKDAY FROM TIMESTAMP {0})");
            functionDict.Add( "SubString3", "SUBSTR({0}, {1}, {2})");
            functionDict.Add( "SubString2", "SUBSTR({0}, {1})");
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

        /// <summary>
        /// 方法是否使用默认格式化
        /// </summary>
        /// <param name="funName"></param>
        /// <returns></returns>
        public static bool IsFunctionUseDefaultFormat(string funName)
        {
            return functionDict.ContainsKey(funName) == false;
        }
        /// <summary>
        /// 获取非默认格式化
        /// </summary>
        /// <param name="sqlType"></param>
        /// <param name="funName"></param>
        /// <returns></returns>
        public static string GetFunctionFormat(  string funName)
        {
            return functionDict[funName];
        }
    }
}
