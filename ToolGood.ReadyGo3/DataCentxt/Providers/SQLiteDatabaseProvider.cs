using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt.Providers
{
    public class SQLiteDatabaseProvider : DatabaseProvider
    {
        protected static readonly Dictionary<string, string> functionDict = new Dictionary<string, string>();

        static SQLiteDatabaseProvider()
        {
            // http://www.sqlite.org/lang_corefunc.html
            functionDict.Add("LEN", "LENGTH({0})");
            functionDict.Add("DATEDIFF", "FLOOR(STRFTIME('%J',{0}) - STRFTIME('%J',{1}))");
            functionDict.Add("YEAR", "STRFTIME('%Y',{0})");
            functionDict.Add("MONTH", "STRFTIME('%m',{0})");
            functionDict.Add("DAY", "STRFTIME('%d',{0})");
            functionDict.Add("HOUR", "STRFTIME('%H',{0})");
            functionDict.Add("MINUTE", "STRFTIME('%M',{0})");
            functionDict.Add("SECOND", "STRFTIME('%S',{0})");
            functionDict.Add("DAYOFYEAR", "STRFTIME('%j',{0})");
            functionDict.Add("WEEK", "STRFTIME('%W',{0})");
            functionDict.Add("WEEKDAY", "STRFTIME('%w',{0})");
            functionDict.Add("SubString3", "SUBSTR({0},{1},{2})");
            functionDict.Add("SubString2", "SUBSTR({0},{1})");
            functionDict.Add("LEFT", "SUBSTR({0},1,{1})");
            functionDict.Add("Right", "SUBSTR({0},-{1})");
        }


        public override string Delete(List<QTable> tables, QColumnBase pk, string tableName, string fromtable, string jointables, string where)
        {
            return $"DELETE t1 FROM {fromtable} {jointables} WHERE {where};";
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
