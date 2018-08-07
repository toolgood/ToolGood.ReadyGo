using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ToolGood.ReadyGo3.DataCentxt.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlDatabaseProvider : DatabaseProvider
    {
 /// <summary>
 /// 
 /// </summary>
        public MySqlDatabaseProvider()
        {
            usedEscapeSql = true;
            escapeSql = '`';
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
            return $"DELETE t1 FROM {fromtable} {jointables} WHERE {where};" ;
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
            return $"UPDATE FROM {fromtable} {jointables} SET {setValues} WHERE {where}";
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
