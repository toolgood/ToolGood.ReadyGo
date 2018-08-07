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
    public class OracleDatabaseProvider : DatabaseProvider
    {
   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override string CreateFunction(SqlFunction function, params object[] args)
        {
            // http://blog.csdn.net/gccr/article/details/1802740
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
                case SqlFunction.DateDiff: return CreateFunction("ROUND(TO_NUMBER(TimeStamp {1} - TimeStamp {0}))", args);
                case SqlFunction.Year: return CreateFunction("EXTRACT(YEAR FROM TIMESTAMP {0})", args);
                case SqlFunction.Month: return CreateFunction("EXTRACT(MONTH FROM TIMESTAMP {0})", args);
                case SqlFunction.Day: return CreateFunction("EXTRACT(DAY FROM TIMESTAMP {0})", args);
                case SqlFunction.Hour: return CreateFunction("EXTRACT(HOUR FROM TIMESTAMP {0})", args);
                case SqlFunction.Minute: return CreateFunction("EXTRACT(MINUTE FROM TIMESTAMP {0})", args);
                case SqlFunction.Second: return CreateFunction("EXTRACT(SECOND FROM TIMESTAMP {0})", args);
                case SqlFunction.DayOfYear: return CreateFunction("EXTRACT(DAYOFYEAR FROM TIMESTAMP {0})", args);
                case SqlFunction.Week: return CreateFunction("EXTRACT(WEEK FROM TIMESTAMP {0})", args);
                case SqlFunction.WeekDay: return CreateFunction("EXTRACT(WEEKDAY FROM TIMESTAMP {0})", args);
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
        public OracleDatabaseProvider()
        {
            usedEscapeSql = true;
            escapeSql = '"';
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
            return $"DELETE (SELECT t1.* FROM {fromtable} {jointables} WHERE {where});";
        }

        //public override string Update(List<QTable> tables, string setValues, string fromtable, string jointables, string where)
        //{
        //    var cols = tables[0].GetUpdateSetRelationColumn();
        //    StringBuilder stringBuilder = new StringBuilder();
        //    stringBuilder.Append("UPDATE ( SELECT ");
        //    for (int i = 0; i < cols.Count; i++) {
        //        if (i > 0) stringBuilder.Append(",");
        //        var col = cols[i];
        //        stringBuilder.Append((col).ToSql(this, tables.Count));
        //        stringBuilder.Append(" ");
        //        stringBuilder.Append(col._table._asName);
        //        stringBuilder.Append("_");
        //        stringBuilder.Append(col._columnName);
        //    }
        //    stringBuilder.Append(" FROM ");
        //    stringBuilder.Append(fromtable);
        //    stringBuilder.Append(" ");
        //    stringBuilder.Append(jointables);
        //    stringBuilder.Append(" WHERE ");
        //    stringBuilder.Append(where);
        //    stringBuilder.Append(") SET ");
        //    var cols2 = table.GetUpdateColumns();
        //    for (int i = 0; i < cols2.Count; i++) {
        //        if (i > 0) stringBuilder.Append(",");
        //        stringBuilder.Append((cols2[i]).ToOracleUpdateSet(provider));
        //    }
        //    stringBuilder.Append(";");
        //    return stringBuilder.ToString();
        //    //return null;
        //    return base.Update(tables, setValues, fromtable, jointables, where);
        //}
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
            var sql = sb.ToString();
            sb.Clear();
            if (offset <= 0) {
                sb.Append("SELECT * FROM (");
                sb.Append(sql);
                sb.Append(" ) WHERE rownum <= ");
                sb.Append(limit.ToString());
                sb.Append(";");
            } else {
                sb.Append("SELECT * FROM (");
                sb.Append("SELECT outtable.*, rownum rn FROM (");
                sb.Append(sql);
                sb.Append(" ) outtable WHERE rownum > ");
                sb.Append(offset.ToString());
                sb.Append(" ) WHERE rn <= ");
                sb.Append((limit + offset).ToString());
                sb.Append(";");
            }
            return sb.ToString();
        }

 

 
    }
}
