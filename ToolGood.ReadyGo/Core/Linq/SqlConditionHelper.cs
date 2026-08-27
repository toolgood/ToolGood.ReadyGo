using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 动态条件 SQL 构建辅助类，供查询/删除/更新查询器复用。
    /// </summary>
    internal static class SqlConditionHelper
    {
        /// <summary>
        /// 构建 EXISTS 子查询 SQL（自动添加 "EXISTS(" 与 "SELECT * " 前缀）。
        /// </summary>
        internal static string BuildExistsSql(string sql)
        {
            sql = sql.TrimStart();
            if (sql.StartsWith("EXISTS", StringComparison.OrdinalIgnoreCase))
                return sql;
            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                return $"EXISTS({sql})";
            return $"EXISTS(SELECT * {sql})";
        }

        /// <summary>
        /// 添加 Where In 条件。空集合生成 1=2，单值生成等值判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="whereSql">原生 SQL 条件回调。</param>
        /// <param name="column">列名。</param>
        /// <param name="values">值集合。</param>
        /// <param name="databaseType">数据库类型，用于转义列名。</param>
        internal static void ApplyWhereIn<TValue>(Action<string, object[]> whereSql, string column, IEnumerable<TValue> values, IDatabaseType databaseType)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var list = values as IReadOnlyList<TValue> ?? values.ToList();
            if (list.Count == 0) {
                whereSql("1 = 2", Array.Empty<object>());
                return;
            }
            var escapedColumn = EscapeColumnName(column, databaseType);
            if (list.Count == 1) {
                whereSql($"{escapedColumn} = @0", new object[] { list[0] });
                return;
            }

            var sb = new StringBuilder();
            sb.Append(escapedColumn).Append(" IN (");
            for (int i = 0; i < list.Count; i++) {
                if (i > 0) sb.Append(", ");
                sb.Append("@").Append(i);
            }
            sb.Append(")");
            whereSql(sb.ToString(), list.Cast<object>().ToArray());
        }

        /// <summary>
        /// 添加 Where Not In 条件。空集合生成 1=1，单值生成不等于判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="whereSql">原生 SQL 条件回调。</param>
        /// <param name="column">列名。</param>
        /// <param name="values">值集合。</param>
        /// <param name="databaseType">数据库类型，用于转义列名。</param>
        internal static void ApplyWhereNotIn<TValue>(Action<string, object[]> whereSql, string column, IEnumerable<TValue> values, IDatabaseType databaseType)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var list = values as IReadOnlyList<TValue> ?? values.ToList();
            if (list.Count == 0) {
                whereSql("1 = 1", Array.Empty<object>());
                return;
            }
            var escapedColumn = EscapeColumnName(column, databaseType);
            if (list.Count == 1) {
                whereSql($"{escapedColumn} <> @0", new object[] { list[0] });
                return;
            }

            var sb = new StringBuilder();
            sb.Append(escapedColumn).Append(" NOT IN (");
            for (int i = 0; i < list.Count; i++) {
                if (i > 0) sb.Append(", ");
                sb.Append("@").Append(i);
            }
            sb.Append(")");
            whereSql(sb.ToString(), list.Cast<object>().ToArray());
        }

        /// <summary>
        /// 从字段表达式获取列名，支持嵌套成员访问（如 x => x.User.Name 返回 "User.Name"）。
        /// </summary>
        internal static string GetFieldName(LambdaExpression expression)
        {
            var body = expression.Body;
            if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
                body = unary.Operand;

            var names = new Stack<string>();
            while (body is MemberExpression member) {
                names.Push(member.Member.Name);
                body = member.Expression;
            }
            if (names.Count == 0)
                throw new ArgumentException($"无法从表达式获取列名：{expression}");
            return string.Join(".", names);
        }

        /// <summary>
        /// 转义列名，按 "." 拆分并转义每个标识符，防止 SQL 注入。
        /// </summary>
        internal static string EscapeColumnName(string column, IDatabaseType databaseType)
        {
            if (string.IsNullOrEmpty(column)) return column;

            var sb = new StringBuilder();
            int start = 0;
            for (int i = 0; i <= column.Length; i++) {
                if (i == column.Length || column[i] == '.') {
                    sb.Append(EscapeIdentifier(column.Substring(start, i - start), databaseType));
                    if (i < column.Length) sb.Append('.');
                    start = i + 1;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 转义 LIKE 模式中的通配符（%、_、\），配合 ESCAPE '\' 使用。
        /// </summary>
        internal static string EscapeLikePattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return pattern;
            return pattern.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
        }

        private static string EscapeIdentifier(string identifier, IDatabaseType databaseType)
        {
            if (string.IsNullOrEmpty(identifier)) return identifier;
            if (identifier.Length >= 2) {
                char first = identifier[0];
                char last = identifier[identifier.Length - 1];
                if ((first == '[' && last == ']') ||
                    (first == '`' && last == '`') ||
                    (first == '"' && last == '"')) {
                    return identifier;
                }
            }
            return databaseType.EscapeSqlIdentifier(identifier);
        }
    }
}
