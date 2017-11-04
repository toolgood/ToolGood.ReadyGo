using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;

namespace ToolGood.ReadyGo3.DataCentxt.Providers
{
    public class MsAccessDbDatabaseProvider : DatabaseProvider
    {
        protected static readonly Dictionary<string, string> functionDict = new Dictionary<string, string>();

        static MsAccessDbDatabaseProvider()
        {
            // 
            functionDict.Add("LOWER", "LCASE({0})");
            functionDict.Add("UPPER", "UCASE({0})");


            functionDict.Add("SubString3", "MID({0}, {1}, {2})");
            functionDict.Add("SubString2", "MID({0}, {1})");
            functionDict.Add("Ascii", "ASC({0})");
        }

        public override string Delete(List<QTable> tables, QColumnBase pk, string tableName, string fromtable, string jointables, string where)
        {
            //http://bbs.csdn.net/topics/340167958
            return $"DELETE distinctrow t1.* FROM {fromtable} {jointables} WHERE {where};";
        }

        public override string Update(List<QTable> tables, string setValues, string fromtable, string jointables, string where)
        {
            // http://blog.csdn.net/hsg77/article/details/6128253
            return $"UPDATE {fromtable} {jointables} SET {setValues} WHERE {where}; " ;
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
            throw new DatabaseUnsupportException();
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
