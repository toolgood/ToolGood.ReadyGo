using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 分页帮助类，用于拆分 SQL 语句并构建分页查询。
    /// </summary>
    public class PagingHelper
    {
        /// <summary>
        /// 用于匹配 SELECT 列列表的正则表达式。
        /// </summary>
        public static Regex rxColumns = new Regex(@"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        /// <summary>
        /// 用于匹配 ORDER BY 子句的正则表达式。
        /// </summary>
        public static Regex rxOrderBy = new Regex(@"(?!.*(?:\s+FROM[\s\(]+))ORDER\s+BY\s+([\w\.\[\]\(\)\s""`,]+)(?!.*\))", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
                
        /// <summary>
        /// 拆分 SQL 语句，提取列列表、排序子句并生成计数 SQL。
        /// </summary>
        /// <param name="sql">原始 SQL 语句。</param>
        /// <param name="parts">用于接收拆分结果的 SQL 片段。</param>
        /// <returns>拆分成功返回 true，否则返回 false。</returns>
        public static bool SplitSQL(string sql, out SQLParts parts)
        {
            parts.sql = sql;
            parts.sqlSelectRemoved = null;
            parts.sqlCount = null;
            parts.sqlOrderBy = null;
            parts.sqlUnordered = sql.Trim().Trim(';');
            parts.sqlColumns = "*";

            // Extract the columns from "SELECT <whatever> FROM"
            var m = rxColumns.Match(sql);
            if (!m.Success) return false;

            // Save column list  [and replace with COUNT(*)]
            Group g = m.Groups[1];
            parts.sqlSelectRemoved = sql.Substring(g.Index);

            // Look for the last "ORDER BY <whatever>" clause not part of a ROW_NUMBER expression
            var matches = rxOrderBy.Matches(parts.sqlUnordered);
            if (matches.Count > 0)
            {
                m = matches[matches.Count - 1];
                g = m.Groups[0];
                parts.sqlOrderBy = g.ToString();
                parts.sqlUnordered = rxOrderBy.Replace(parts.sqlUnordered, "", 1, m.Index);
            }

            parts.sqlCount = string.Format(@"SELECT COUNT(*) FROM ({0}) poco_tbl", parts.sqlUnordered);

            return true;
        }

        private static readonly Regex OrderByAlias = new Regex(@"[\""\[\]\w]+\.([\[\]\""\w]+)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        /// <summary>
        /// 基于拆分后的 SQL 片段构建分页查询语句。
        /// </summary>
        /// <param name="skip">要跳过的记录数。</param>
        /// <param name="take">要获取的记录数。</param>
        /// <param name="parts">拆分后的 SQL 片段。</param>
        /// <param name="args">查询参数数组，按引用传递。</param>
        /// <returns>分页查询语句。</returns>
        public static string BuildPaging(long skip, long take, SQLParts parts, ref object[] args)
        {
            parts.sqlOrderBy = string.IsNullOrEmpty(parts.sqlOrderBy) ? null : OrderByAlias.Replace(parts.sqlOrderBy, "$1");
            var sqlPage = string.Format("SELECT {4} FROM (SELECT poco_base.*, ROW_NUMBER() OVER ({0}) poco_rn \nFROM ( \n{1}) poco_base ) poco_paged \nWHERE poco_rn > @{2} AND poco_rn <= @{3} \nORDER BY poco_rn",
                parts.sqlOrderBy ?? "ORDER BY (SELECT NULL /*poco_dual*/)", parts.sqlUnordered, args.Length, args.Length + 1, parts.sqlColumns);
            args = args.Concat(new object[] { skip, skip + take }).ToArray();

            return sqlPage;
        }
    }
}
