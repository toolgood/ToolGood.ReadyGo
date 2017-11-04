using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Providers
{
    public class FirebirdDbDatabaseProvider : DatabaseProvider
    {
        protected static readonly Dictionary<string, string> functionDict = new Dictionary<string, string>();

        static FirebirdDbDatabaseProvider()
        {
            //  http://www.firebirdsql.org/refdocs/langrefupd21.html
            functionDict.Add("DATEDIFF", "DATEDIFF(day,{0},{1})");
            functionDict.Add("YEAR", "EXTRACT(YEAR FROM {0})");
            functionDict.Add("MONTH", "EXTRACT(MONTH FROM {0})");
            functionDict.Add("DAY", "EXTRACT(DAY FROM {0})");
            functionDict.Add("HOUR", "EXTRACT(HOUR FROM {0})");
            functionDict.Add("MINUTE", "EXTRACT(MINUTE FROM {0})");
            functionDict.Add("SECOND", "EXTRACT(SECOND FROM {0})");
            functionDict.Add("DAYOFYEAR", "EXTRACT(DAYOFYEAR FROM {0})");
            functionDict.Add("WEEK", "EXTRACT(WEEK FROM {0})");
            functionDict.Add("WEEKDAY", "EXTRACT(WEEKDAY FROM {0})");

            functionDict.Add("SubString3", "SUBSTRING({0} FROM {1} FOR {2})");
            functionDict.Add("SubString2", "SUBSTRING({0} FROM {1})");
            functionDict.Add("LEFT", "SLEFT({0},{1})");
            functionDict.Add("Right", "SRIGHT({0},{1})");
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
            sb.Append("FIRST ");
            sb.Append(limit);
            if (offset >= 0) {
                sb.Append(" SKIP ");
                sb.Append(offset);
            }
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



        public override string EscapeSqlIdentifier(string sqlIdentifier)
        {
            return string.Format("\"{0}\"", sqlIdentifier);
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
