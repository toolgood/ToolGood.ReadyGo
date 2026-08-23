using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// IQueryProvider 动态条件扩展方法（独立文件，不改动 NPOCO 源码）。
    /// 提供 IfTrue* 条件开关、WhereExists/WhereNotExists、WhereIn、WhereLike 等便捷方法。
    /// </summary>
    public static class QueryProviderDynamicExtensions
    {
        #region IfTrueWhere / IfTrueOrderBy / IfTrueOrderByDescending / IfTrueLimit

        /// <summary>
        /// 条件成立时添加 Where 条件
        /// </summary>
        public static IQueryProvider<T> IfTrueWhere<T>(this IQueryProvider<T> provider, bool condition, Expression<Func<T, bool>> predicate)
        {
            if (condition && predicate != null)
                provider.Where(predicate);
            return provider;
        }

        /// <summary>
        /// 条件成立时添加 Order By
        /// </summary>
        public static IQueryProvider<T> IfTrueOrderBy<T>(this IQueryProvider<T> provider, bool condition, Expression<Func<T, object>> column)
        {
            if (condition && column != null)
                provider.OrderBy(column);
            return provider;
        }

        /// <summary>
        /// 条件成立时添加 Order By Descending
        /// </summary>
        public static IQueryProvider<T> IfTrueOrderByDescending<T>(this IQueryProvider<T> provider, bool condition, Expression<Func<T, object>> column)
        {
            if (condition && column != null)
                provider.OrderByDescending(column);
            return provider;
        }

        /// <summary>
        /// 条件成立时添加 Limit（行数需大于 0）
        /// </summary>
        public static IQueryProvider<T> IfTrueLimit<T>(this IQueryProvider<T> provider, bool condition, int rows)
        {
            if (condition && rows > 0)
                provider.Limit(rows);
            return provider;
        }

        /// <summary>
        /// 条件成立时添加 Limit（跳过 skip 行，取 rows 行）
        /// </summary>
        public static IQueryProvider<T> IfTrueLimit<T>(this IQueryProvider<T> provider, bool condition, int skip, int rows)
        {
            if (condition && rows > 0)
                provider.Limit(skip, rows);
            return provider;
        }

        #endregion IfTrueWhere / IfTrueOrderBy / IfTrueOrderByDescending / IfTrueLimit

        #region WhereExists / WhereNotExists

        /// <summary>
        /// Where Exists（自动添加 "EXISTS(" 与 "SELECT * " 前缀）
        /// </summary>
        /// <param name="provider">查询器</param>
        /// <param name="sql">子查询 SQL 或表名/过滤条件</param>
        /// <param name="args">SQL 参数</param>
        public static IQueryProvider<T> WhereExists<T>(this IQueryProvider<T> provider, string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException(nameof(sql));
            provider.WhereSql(BuildExistsSql(sql), args);
            return provider;
        }

        /// <summary>
        /// Where Not Exists（自动添加 "NOT EXISTS(" 前缀）
        /// </summary>
        /// <param name="provider">查询器</param>
        /// <param name="sql">子查询 SQL 或表名/过滤条件</param>
        /// <param name="args">SQL 参数</param>
        public static IQueryProvider<T> WhereNotExists<T>(this IQueryProvider<T> provider, string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException(nameof(sql));
            provider.WhereSql("NOT " + BuildExistsSql(sql), args);
            return provider;
        }

        /// <summary>
        /// 条件成立时添加 Where Exists
        /// </summary>
        public static IQueryProvider<T> IfTrueWhereExists<T>(this IQueryProvider<T> provider, bool condition, string sql, params object[] args)
        {
            return condition ? provider.WhereExists(sql, args) : provider;
        }

        /// <summary>
        /// 条件成立时添加 Where Not Exists
        /// </summary>
        public static IQueryProvider<T> IfTrueWhereNotExists<T>(this IQueryProvider<T> provider, bool condition, string sql, params object[] args)
        {
            return condition ? provider.WhereNotExists(sql, args) : provider;
        }

        #endregion WhereExists / WhereNotExists

        #region WhereIn / IfTrueWhereIn

        /// <summary>
        /// Where {column} In (values)。空集合生成 1=2，单值生成等值判断。
        /// </summary>
        /// <param name="provider">查询器</param>
        /// <param name="column">列名（可带别名，如 "t0.Age"）</param>
        /// <param name="values">值集合</param>
        public static IQueryProvider<T> WhereIn<T, TValue>(this IQueryProvider<T> provider, string column, IEnumerable<TValue> values)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            ApplyWhereIn(provider, column, values);
            return provider;
        }

        /// <summary>
        /// Where {field} In (values)。空集合生成 1=2，单值生成等值判断。
        /// </summary>
        /// <param name="provider">查询器</param>
        /// <param name="field">列表达式，如 x =&gt; x.Age</param>
        /// <param name="values">值集合</param>
        public static IQueryProvider<T> WhereIn<T, TValue>(this IQueryProvider<T> provider, Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            ApplyWhereIn(provider, GetFieldName(field), values);
            return provider;
        }

        /// <summary>
        /// 条件成立时添加 Where In（字符串列名版本）
        /// </summary>
        public static IQueryProvider<T> IfTrueWhereIn<T, TValue>(this IQueryProvider<T> provider, bool condition, string column, IEnumerable<TValue> values)
        {
            return condition ? provider.WhereIn(column, values) : provider;
        }

        /// <summary>
        /// 条件成立时添加 Where In（表达式版本）
        /// </summary>
        public static IQueryProvider<T> IfTrueWhereIn<T, TValue>(this IQueryProvider<T> provider, bool condition, Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return condition ? provider.WhereIn(field, values) : provider;
        }

        #endregion WhereIn / IfTrueWhereIn

        #region WhereLike / IfTrueWhereLike

        /// <summary>
        /// Where {column} Like '%pattern%'
        /// </summary>
        /// <param name="provider">查询器</param>
        /// <param name="column">列名（可带别名，如 "t0.Name"）</param>
        /// <param name="pattern">匹配内容（自动加前后 %）</param>
        public static IQueryProvider<T> WhereLike<T>(this IQueryProvider<T> provider, string column, string pattern)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            if (string.IsNullOrEmpty(pattern)) return provider;
            provider.WhereSql($"{column} LIKE @0", $"%{pattern}%");
            return provider;
        }

        /// <summary>
        /// Where {field} Like '%pattern%'
        /// </summary>
        /// <param name="provider">查询器</param>
        /// <param name="field">列表达式，如 x =&gt; x.Name</param>
        /// <param name="pattern">匹配内容（自动加前后 %）</param>
        public static IQueryProvider<T> WhereLike<T, TValue>(this IQueryProvider<T> provider, Expression<Func<T, TValue>> field, string pattern)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return provider.WhereLike(GetFieldName(field), pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like（字符串列名版本）
        /// </summary>
        public static IQueryProvider<T> IfTrueWhereLike<T>(this IQueryProvider<T> provider, bool condition, string column, string pattern)
        {
            return condition ? provider.WhereLike(column, pattern) : provider;
        }

        /// <summary>
        /// 条件成立时添加 Where Like（表达式版本）
        /// </summary>
        public static IQueryProvider<T> IfTrueWhereLike<T, TValue>(this IQueryProvider<T> provider, bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return condition ? provider.WhereLike(field, pattern) : provider;
        }

        #endregion WhereLike / IfTrueWhereLike

        #region 私有辅助

        private static string BuildExistsSql(string sql)
        {
            sql = sql.TrimStart();
            if (sql.StartsWith("EXISTS", StringComparison.OrdinalIgnoreCase))
                return sql;
            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                return $"EXISTS({sql})";
            return $"EXISTS(SELECT * {sql})";
        }

        private static void ApplyWhereIn<T, TValue>(IQueryProvider<T> provider, string column, IEnumerable<TValue> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var list = values as IReadOnlyList<TValue> ?? values.ToList();
            if (list.Count == 0) {
                provider.WhereSql("1 = 2");
                return;
            }
            if (list.Count == 1) {
                provider.WhereSql($"{column} = @0", list[0]);
                return;
            }

            var sb = new StringBuilder();
            sb.Append(column).Append(" IN (");
            for (int i = 0; i < list.Count; i++) {
                if (i > 0) sb.Append(", ");
                sb.Append("@").Append(i);
            }
            sb.Append(")");
            provider.WhereSql(sb.ToString(), list.Cast<object>().ToArray());
        }

        private static string GetFieldName(LambdaExpression expression)
        {
            var body = expression.Body;
            if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
                body = unary.Operand;
            if (body is MemberExpression member)
                return member.Member.Name;
            throw new ArgumentException($"无法从表达式获取列名：{expression}");
        }

        #endregion 私有辅助
    }
}
