using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.ReadyGo3.Exceptions;

namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// sql 工具类
    /// </summary>
    public static class SqlUtil
    {
        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static string ToEscapeParam(string stringValue)
        {
            return stringValue.Replace(@"\", @"\\").Replace("'", "\\'")
                                  .Replace("\0", "\\0").Replace("\a", "\\a").Replace("\b", "\\b")
                                  .Replace("\f", "\\f").Replace("\n", "\\n").Replace("\r", "\\r")
                                  .Replace("\t", "\\t").Replace("\v", "\\v");
        }
        /// <summary>
        /// 格式化like字符串
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ToEscapeLikeParam(string param)
        {
            return ToEscapeParam(param)
                .Replace("_", @"\_")
                .Replace("%", @"\%")
                .Replace("[", @"\[")
                .Replace("]", @"\]");
        }
        /// <summary>
        /// 拼接like字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="param"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereLike(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} LIKE '{ToEscapeLikeParam(param)}'";
        }

        /// <summary>
        /// 拼接like字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="param"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereLikeStart(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} LIKE '%{ToEscapeLikeParam(param)}'";
        }

        /// <summary>
        /// 拼接like字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="param"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereLikeEnd(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} LIKE '{ToEscapeLikeParam(param)}%'";
        }

        /// <summary>
        /// 拼接not like字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="param"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereNotLike(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} NOT LIKE '{ToEscapeLikeParam(param)}'";
        }

        /// <summary>
        /// 拼接not like字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="param"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereNotLikeStart(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} NOT LIKE '%{ToEscapeLikeParam(param)}'";
        }

        /// <summary>
        /// 拼接not like字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="param"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereNotLikeEnd(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} NOT LIKE '{ToEscapeLikeParam(param)}%'";
        }


        /// <summary>
        /// 拼接in字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="param"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereIn(string columnName, List<string> param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            if (param != null && param.Count > 0) {
                StringBuilder sql = new StringBuilder();
                sql.Append(' ');
                if (and) {
                    sql.Append("AND ");
                }
                sql.Append(columnName);
                sql.Append(" IN (");
                for (global::System.Int32 i = 0; i < param.Count; i++) {
                    if (i > 0) { sql.Append(','); }
                    sql.Append('\'');
                    sql.Append(SqlUtil.ToEscapeParam(param[i]));
                    sql.Append('\'');
                }
                sql.Append(")");
                return sql.ToString();
            }
            return $"{(and?" AND":"")} 1=2";
        }
        /// <summary>
        /// 拼接in字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="numbers"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereIn(string columnName, List<int> numbers, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            if (numbers != null && numbers.Count > 0) {
                StringBuilder sql = new StringBuilder();
                sql.Append(' ');
                if (and) {
                    sql.Append("AND ");
                }
                sql.Append(columnName);
                sql.Append(" IN (");
                for (int i = 0; i < numbers.Count; i++) {
                    if (i > 0) { sql.Append(','); }
                    sql.Append(numbers[i]);
                }
                sql.Append(')');
                return sql.ToString();
            }
            return $"{(and ? " AND" : "")} 1=2";
        }
        /// <summary>
        /// 拼接in字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="numbers"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereIn(string columnName, List<long> numbers, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            if (numbers != null && numbers.Count > 0) {
                StringBuilder sql = new StringBuilder();
                sql.Append(' ');
                if (and) {
                    sql.Append("AND ");
                }
                sql.Append(columnName);
                sql.Append(" IN (");
                for (int i = 0; i < numbers.Count; i++) {
                    if (i > 0) { sql.Append(','); }
                    sql.Append(numbers[i]);
                }
                sql.Append(')');
                return sql.ToString();
            }
            return $"{(and ? " AND" : "")} 1=2";
        }

        /// <summary>
        /// 拼接not in字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="param"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereNotIn(string columnName, List<string> param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            if (param != null && param.Count > 0) {
                StringBuilder sql = new StringBuilder();
                sql.Append(' ');
                if (and) {
                    sql.Append("AND ");
                }
                sql.Append(columnName);
                sql.Append(" NOT IN (");
                for (global::System.Int32 i = 0; i < param.Count; i++) {
                    if (i > 0) { sql.Append(','); }
                    sql.Append('\'');
                    sql.Append(SqlUtil.ToEscapeParam(param[i]));
                    sql.Append('\'');
                }
                sql.Append(")");
                return sql.ToString();
            }
            return "";
        }

        /// <summary>
        /// 拼接not in字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="numbers"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereNotIn(string columnName, List<int> numbers, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            if (numbers != null && numbers.Count > 0) {
                StringBuilder sql = new StringBuilder();
                sql.Append(' ');
                if (and) {
                    sql.Append("AND ");
                }
                sql.Append(columnName);
                sql.Append(" NOT IN (");
                for (int i = 0; i < numbers.Count; i++) {
                    if (i > 0) { sql.Append(','); }
                    sql.Append(numbers[i]);
                }
                sql.Append(')');
                return sql.ToString();
            }
            return "";
        }

        /// <summary>
        /// 拼接not in字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="numbers"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public static string WhereNotIn(string columnName, List<long> numbers, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            if (numbers != null && numbers.Count > 0) {
                StringBuilder sql = new StringBuilder();
                sql.Append(' ');
                if (and) {
                    sql.Append("AND ");
                }
                sql.Append(columnName);
                sql.Append(" NOT IN (");
                for (int i = 0; i < numbers.Count; i++) {
                    if (i > 0) { sql.Append(','); }
                    sql.Append(numbers[i]);
                }
                sql.Append(')');
                return sql.ToString();
            }
            return "";
        }
    }
}
