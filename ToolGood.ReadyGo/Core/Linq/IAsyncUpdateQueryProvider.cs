using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 异步更新查询器接口。
    /// </summary>
    /// <typeparam name="T">更新的实体类型。</typeparam>
    public interface IAsyncUpdateQueryProvider<T>
    {
        /// <summary>
        /// 添加更新条件。
        /// </summary>
        /// <param name="whereExpression">更新条件表达式。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 排除默认值字段（不更新为默认值的字段）。
        /// </summary>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> ExcludeDefaults();
        /// <summary>
        /// 仅更新指定字段。
        /// </summary>
        /// <param name="onlyFields">字段选择器。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> OnlyFields(Expression<Func<T, object>> onlyFields);
        /// <summary>
        /// 条件成立时添加更新条件。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="predicate">筛选条件表达式。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhere(bool condition, Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Where Exists（自动添加 "EXISTS(" 与 "SELECT * " 前缀）。
        /// </summary>
        /// <param name="sql">子查询 SQL 或表名/过滤条件。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereExists(string sql, params object[] args);
        /// <summary>
        /// Where Not Exists（自动添加 "NOT EXISTS(" 前缀）。
        /// </summary>
        /// <param name="sql">子查询 SQL 或表名/过滤条件。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereNotExists(string sql, params object[] args);
        /// <summary>
        /// 条件成立时添加 Where Exists。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="sql">子查询 SQL 或表名/过滤条件。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereExists(bool condition, string sql, params object[] args);
        /// <summary>
        /// 条件成立时添加 Where Not Exists。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="sql">子查询 SQL 或表名/过滤条件。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereNotExists(bool condition, string sql, params object[] args);
        /// <summary>
        /// Where {column} In (values)。空集合生成 1=2，单值生成等值判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="column">列名（可带别名，如 "t0.Age"）。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereIn<TValue>(string column, IEnumerable<TValue> values);
        /// <summary>
        /// Where {field} In (values)。空集合生成 1=2，单值生成等值判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Age。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereIn<TValue>(Expression<Func<T, TValue>> field, IEnumerable<TValue> values);
        /// <summary>
        /// 条件成立时添加 Where In（字符串列名版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereIn<TValue>(bool condition, string column, IEnumerable<TValue> values);
        /// <summary>
        /// 条件成立时添加 Where In（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereIn<TValue>(bool condition, Expression<Func<T, TValue>> field, IEnumerable<TValue> values);
        /// <summary>
        /// Where {column} Like '%pattern%'。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加前后 %）。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereLike(string column, string pattern);
        /// <summary>
        /// Where {field} Like '%pattern%'。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Name。</param>
        /// <param name="pattern">匹配内容（自动加前后 %）。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereLike<TValue>(Expression<Func<T, TValue>> field, string pattern);
        /// <summary>
        /// 条件成立时添加 Where Like（字符串列名版本）。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereLike(bool condition, string column, string pattern);
        /// <summary>
        /// 条件成立时添加 Where Like（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereLike<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern);
        /// <summary>
        /// Where {column} Not In (values)。空集合生成 1=1，单值生成不等于判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="column">列名（可带别名，如 "t0.Age"）。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereNotIn<TValue>(string column, IEnumerable<TValue> values);
        /// <summary>
        /// Where {field} Not In (values)。空集合生成 1=1，单值生成不等于判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Age。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereNotIn<TValue>(Expression<Func<T, TValue>> field, IEnumerable<TValue> values);
        /// <summary>
        /// 条件成立时添加 Where Not In（字符串列名版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereNotIn<TValue>(bool condition, string column, IEnumerable<TValue> values);
        /// <summary>
        /// 条件成立时添加 Where Not In（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereNotIn<TValue>(bool condition, Expression<Func<T, TValue>> field, IEnumerable<TValue> values);
        /// <summary>
        /// Where {column} Like '%pattern'（右匹配）。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加前缀 %）。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereLikeStart(string column, string pattern);
        /// <summary>
        /// Where {field} Like '%pattern'（右匹配）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereLikeStart<TValue>(Expression<Func<T, TValue>> field, string pattern);
        /// <summary>
        /// 条件成立时添加 Where Like Start（字符串列名版本）。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereLikeStart(bool condition, string column, string pattern);
        /// <summary>
        /// 条件成立时添加 Where Like Start（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereLikeStart<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern);
        /// <summary>
        /// Where {column} Like 'pattern%'（左匹配）。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加后缀 %）。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereLikeEnd(string column, string pattern);
        /// <summary>
        /// Where {field} Like 'pattern%'（左匹配）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> WhereLikeEnd<TValue>(Expression<Func<T, TValue>> field, string pattern);
        /// <summary>
        /// 条件成立时添加 Where Like End（字符串列名版本）。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereLikeEnd(bool condition, string column, string pattern);
        /// <summary>
        /// 条件成立时添加 Where Like End（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前更新查询器。</returns>
        IAsyncUpdateQueryProvider<T> IfTrueWhereLikeEnd<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern);
        /// <summary>
        /// 异步执行更新。
        /// </summary>
        /// <param name="obj">待更新的实体。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>受影响的行数。</returns>
        Task<int> Execute(T obj, CancellationToken cancellationToken = default);
    }
}
