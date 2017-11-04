using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Providers
{
    public class OracleDatabaseProvider : DatabaseProvider
    {
        protected static readonly Dictionary<string, string> functionDict = new Dictionary<string, string>();

        static OracleDatabaseProvider()
        {
            // http://blog.csdn.net/gccr/article/details/1802740
            functionDict.Add("LEN", "LENGTH({0})");
            functionDict.Add("DATEDIFF", "ROUND(TO_NUMBER(TimeStamp {1} - TimeStamp {0}))");
            functionDict.Add("YEAR", "EXTRACT(YEAR FROM TIMESTAMP {0})");
            functionDict.Add("MONTH", "EXTRACT(MONTH FROM TIMESTAMP {0})");
            functionDict.Add("DAY", "EXTRACT(DAY FROM TIMESTAMP {0})");
            functionDict.Add("HOUR", "EXTRACT(HOUR FROM TIMESTAMP {0})");
            functionDict.Add("MINUTE", "EXTRACT(MINUTE FROM TIMESTAMP {0})");
            functionDict.Add("SECOND", "EXTRACT(SECOND FROM TIMESTAMP {0})");
            functionDict.Add("DAYOFYEAR", "EXTRACT(DAYOFYEAR FROM TIMESTAMP {0})");
            functionDict.Add("WEEK", "EXTRACT(WEEK FROM TIMESTAMP {0})");
            functionDict.Add("WEEKDAY", "EXTRACT(WEEKDAY FROM TIMESTAMP {0})");
        }

        public OracleDatabaseProvider()
        {
            usedEscapeSql = true;
            escapeSql = '"';
        }

        public override string Delete(List<QTable> tables, QColumnBase pk, string tableName, string fromtable, string jointables, string where)
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
        //        stringBuilder.Append(((IColumnConvert)col).ToSql(this, tables.Count));
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
        //        stringBuilder.Append(((IColumnConvert)cols2[i]).ToOracleUpdateSet(provider));
        //    }
        //    stringBuilder.Append(";");
        //    return stringBuilder.ToString();
        //    //return null;
        //    return base.Update(tables, setValues, fromtable, jointables, where);
        //}

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



        public override string EscapeSqlIdentifier(string sqlIdentifier)
        {
            return $"\"{sqlIdentifier}\"";
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
