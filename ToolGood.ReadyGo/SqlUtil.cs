using System;
using System.Collections.Generic;
using System.Text;

namespace ToolGood.ReadyGo
{
    /// <summary>
    /// sql 工具类
    /// </summary>
    public static class SqlUtil
    {
        /// <summary>
        /// 转义 SQL 字符串中的特殊字符
        /// </summary>
        /// <param name="stringValue">原始字符串</param>
        /// <returns>转义后的字符串</returns>
        public static string ToEscapeParam(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue)) {
                return "";
            }

            return stringValue.Replace(@"\", @"\\").Replace("'", "\\'")
                                  .Replace("\0", "\\0").Replace("\a", "\\a").Replace("\b", "\\b")
                                  .Replace("\f", "\\f").Replace("\n", "\\n").Replace("\r", "\\r")
                                  .Replace("\t", "\\t").Replace("\v", "\\v");
        }

        /// <summary>
        /// 转义 LIKE 匹配字符串中的通配符与特殊字符
        /// </summary>
        /// <param name="param">原始匹配字符串</param>
        /// <returns>转义后的匹配字符串</returns>
        public static string ToEscapeLikeParam(string param)
        {
            return ToEscapeParam(param)
                .Replace("_", @"\_")
                .Replace("%", @"\%")
                .Replace("[", @"\[")
                .Replace("]", @"\]");
        }

        /// <summary>
        /// 拼接 LIKE 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="param">匹配值</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>LIKE 条件字符串</returns>
        public static string WhereLike(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} LIKE '{ToEscapeLikeParam(param)}'";
        }

        /// <summary>
        /// 拼接前缀模糊匹配（值%）的 LIKE 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="param">匹配值</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>LIKE 条件字符串</returns>
        public static string WhereLikeStart(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} LIKE '{ToEscapeLikeParam(param)}%'";
        }

        /// <summary>
        /// 拼接后缀模糊匹配（%值）的 LIKE 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="param">匹配值</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>LIKE 条件字符串</returns>
        public static string WhereLikeEnd(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} LIKE '%{ToEscapeLikeParam(param)}'";
        }

        /// <summary>
        /// 拼接 NOT LIKE 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="param">匹配值</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>NOT LIKE 条件字符串</returns>
        public static string WhereNotLike(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} NOT LIKE '{ToEscapeLikeParam(param)}'";
        }

        /// <summary>
        /// 拼接前缀模糊匹配（值%）的 NOT LIKE 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="param">匹配值</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>NOT LIKE 条件字符串</returns>
        public static string WhereNotLikeStart(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} NOT LIKE '{ToEscapeLikeParam(param)}%'";
        }

        /// <summary>
        /// 拼接后缀模糊匹配（%值）的 NOT LIKE 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="param">匹配值</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>NOT LIKE 条件字符串</returns>
        public static string WhereNotLikeEnd(string columnName, string param, bool and = false)
        {
            if (string.IsNullOrEmpty(columnName)) { throw new Exception("columnName is null or empty"); }
            return $"{(and ? " AND" : "")} {columnName} NOT LIKE '%{ToEscapeLikeParam(param)}'";
        }

        /// <summary>
        /// 拼接 IN 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="param">字符串值集合</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>IN 条件字符串，集合为空时返回恒假条件</returns>
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
            return $"{(and ? " AND" : "")} 1=2";
        }

        /// <summary>
        /// 拼接 IN 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="numbers">整型值集合</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>IN 条件字符串，集合为空时返回恒假条件</returns>
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
        /// 拼接 IN 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="numbers">长整型值集合</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>IN 条件字符串，集合为空时返回恒假条件</returns>
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
        /// 拼接 NOT IN 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="param">字符串值集合</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>NOT IN 条件字符串，集合为空时返回空字符串</returns>
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
        /// 拼接 NOT IN 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="numbers">整型值集合</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>NOT IN 条件字符串，集合为空时返回空字符串</returns>
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
        /// 拼接 NOT IN 条件字符串
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="numbers">长整型值集合</param>
        /// <param name="and">是否以 AND 开头</param>
        /// <returns>NOT IN 条件字符串，集合为空时返回空字符串</returns>
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
