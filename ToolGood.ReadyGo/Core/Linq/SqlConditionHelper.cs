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
        internal static void ApplyWhereIn<TValue>(Action<string, object[]> whereSql, string column, IEnumerable<TValue> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var list = values as IReadOnlyList<TValue> ?? values.ToList();
            if (list.Count == 0) {
                whereSql("1 = 2", Array.Empty<object>());
                return;
            }
            if (list.Count == 1) {
                whereSql($"{column} = @0", new object[] { list[0] });
                return;
            }

            var sb = new StringBuilder();
            sb.Append(column).Append(" IN (");
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
        internal static void ApplyWhereNotIn<TValue>(Action<string, object[]> whereSql, string column, IEnumerable<TValue> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var list = values as IReadOnlyList<TValue> ?? values.ToList();
            if (list.Count == 0) {
                whereSql("1 = 1", Array.Empty<object>());
                return;
            }
            if (list.Count == 1) {
                whereSql($"{column} <> @0", new object[] { list[0] });
                return;
            }

            var sb = new StringBuilder();
            sb.Append(column).Append(" NOT IN (");
            for (int i = 0; i < list.Count; i++) {
                if (i > 0) sb.Append(", ");
                sb.Append("@").Append(i);
            }
            sb.Append(")");
            whereSql(sb.ToString(), list.Cast<object>().ToArray());
        }

        /// <summary>
        /// 从字段表达式获取列名。
        /// </summary>
        internal static string GetFieldName(LambdaExpression expression)
        {
            var body = expression.Body;
            if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
                body = unary.Operand;
            if (body is MemberExpression member)
                return member.Member.Name;
            throw new ArgumentException($"无法从表达式获取列名：{expression}");
        }
    }
}
