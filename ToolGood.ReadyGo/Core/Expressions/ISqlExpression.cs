using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ToolGood.ReadyGo.NPoco.Linq;

namespace ToolGood.ReadyGo.NPoco.Expressions
{
    /// <summary>
    /// SQL 表达式的基础接口，定义查询 SQL 构建所需的核心状态与分页能力。
    /// </summary>
    public interface ISqlExpression
    {
        /// <summary>
        /// 排序成员集合。
        /// </summary>
        List<OrderByMember> OrderByMembers { get; }
        /// <summary>
        /// 返回行数。
        /// </summary>
        int? Rows { get; }
        /// <summary>
        /// 跳过的行数。
        /// </summary>
        int? Skip { get; }
        /// <summary>
        /// WHERE 条件 SQL。
        /// </summary>
        string WhereSql { get; }
        /// <summary>
        /// 查询参数数组。
        /// </summary>
        object[] Params { get; }
        /// <summary>
        /// 实体类型。
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// 选择成员集合。
        /// </summary>
        List<SelectMember> SelectMembers { get; }
        /// <summary>
        /// 通用成员集合。
        /// </summary>
        List<GeneralMember> GeneralMembers { get; }
        /// <summary>
        /// 对 SQL 应用分页处理。
        /// </summary>
        /// <param name="sql">待分页的 SQL。</param>
        /// <param name="columns">分页涉及的列集合。</param>
        /// <param name="joinSqlExpressions">关联查询表达式集合。</param>
        /// <returns>分页后的 SQL。</returns>
        string ApplyPaging(string sql, IEnumerable<PocoColumn[]> columns, Dictionary<string, JoinData> joinSqlExpressions);
        /// <summary>
        /// 表提示（Table Hint）。
        /// </summary>
        string TableHint { get; }
    }

    /// <summary>
    /// 泛型 SQL 表达式接口，提供链式查询构建能力。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public interface ISqlExpression<T> : ISqlExpression
    {
        /// <summary>
        /// 表达式上下文。
        /// </summary>
        ISqlExpressionContext Context { get; }

        /// <summary>
        /// 添加分组字段。
        /// </summary>
        /// <typeparam name="TKey">分组字段类型。</typeparam>
        /// <param name="keySelector">分组字段选择器。</param>
        /// <returns>当前表达式。</returns>
        ISqlExpression<T> GroupBy<TKey>(Expression<Func<T, TKey>> keySelector);
        /// <summary>
        /// 限制返回行数。
        /// </summary>
        /// <param name="rows">返回行数。</param>
        /// <returns>当前表达式。</returns>
        ISqlExpression<T> Limit(int rows);
        /// <summary>
        /// 限制返回行数并跳过指定行数。
        /// </summary>
        /// <param name="skip">跳过的行数。</param>
        /// <param name="rows">返回行数。</param>
        /// <returns>当前表达式。</returns>
        ISqlExpression<T> Limit(int skip, int rows);
        /// <summary>
        /// 生成关联（JOIN ON）条件 SQL。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="predicate">关联条件。</param>
        /// <returns>ON 条件 SQL。</returns>
        string On<T2>(Expression<Func<T, T2, bool>> predicate);
        /// <summary>
        /// 添加升序排序字段。
        /// </summary>
        /// <typeparam name="TKey">排序字段类型。</typeparam>
        /// <param name="keySelector">排序字段选择器。</param>
        /// <returns>当前表达式。</returns>
        ISqlExpression<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);
        /// <summary>
        /// 添加降序排序字段。
        /// </summary>
        /// <typeparam name="TKey">排序字段类型。</typeparam>
        /// <param name="keySelector">排序字段选择器。</param>
        /// <returns>当前表达式。</returns>
        ISqlExpression<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
        /// <summary>
        /// 设置查询字段。
        /// </summary>
        /// <typeparam name="TKey">字段类型。</typeparam>
        /// <param name="fields">字段选择器。</param>
        /// <returns>当前表达式。</returns>
        ISqlExpression<T> Select<TKey>(Expression<Func<T, TKey>> fields);
        /// <summary>
        /// 设置去重查询字段。
        /// </summary>
        /// <typeparam name="TKey">字段类型。</typeparam>
        /// <param name="fields">字段选择器。</param>
        /// <returns>选择成员集合。</returns>
        List<SelectMember> SelectDistinct<TKey>(Expression<Func<T, TKey>> fields);
        /// <summary>
        /// 设置投影查询字段。
        /// </summary>
        /// <typeparam name="TKey">字段类型。</typeparam>
        /// <param name="fields">字段选择器。</param>
        /// <returns>选择成员集合。</returns>
        List<SelectMember> SelectProjection<TKey>(Expression<Func<T, TKey>> fields);
        /// <summary>
        /// 添加表提示。
        /// </summary>
        /// <param name="hint">表提示内容。</param>
        void Hint(string hint);
        /// <summary>
        /// 追加升序排序字段。
        /// </summary>
        /// <typeparam name="TKey">排序字段类型。</typeparam>
        /// <param name="keySelector">排序字段选择器。</param>
        /// <returns>当前表达式。</returns>
        ISqlExpression<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector);
        /// <summary>
        /// 追加降序排序字段。
        /// </summary>
        /// <typeparam name="TKey">排序字段类型。</typeparam>
        /// <param name="keySelector">排序字段选择器。</param>
        /// <returns>当前表达式。</returns>
        ISqlExpression<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
        /// <summary>
        /// 设置更新字段。
        /// </summary>
        /// <typeparam name="TKey">字段类型。</typeparam>
        /// <param name="fields">字段选择器。</param>
        /// <returns>当前表达式。</returns>
        ISqlExpression<T> Update<TKey>(Expression<Func<T, TKey>> fields);
        /// <summary>
        /// 添加 WHERE 条件。
        /// </summary>
        /// <param name="predicate">条件表达式。</param>
        /// <returns>当前表达式。</returns>
        ISqlExpression<T> Where(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 添加 WHERE 条件 SQL。
        /// </summary>
        /// <param name="sqlFilter">条件 SQL。</param>
        /// <param name="filterParams">条件参数。</param>
        /// <returns>当前表达式。</returns>
        ISqlExpression<T> Where(string sqlFilter, params object[] filterParams);

        /// <summary>
        /// SQL 表达式上下文接口，提供语句生成与参数、更新字段访问。
        /// </summary>
        public interface ISqlExpressionContext
        {
            /// <summary>
            /// 查询参数数组。
            /// </summary>
            object[] Params { get; }
            /// <summary>
            /// 更新字段名称集合。
            /// </summary>
            List<string> UpdateFields { get; set; }

            /// <summary>
            /// 生成删除语句。
            /// </summary>
            /// <returns>删除 SQL。</returns>
            string ToDeleteStatement();
            /// <summary>
            /// 生成查询语句。
            /// </summary>
            /// <returns>查询 SQL。</returns>
            string ToSelectStatement();
            /// <summary>
            /// 生成查询语句。
            /// </summary>
            /// <param name="applyPaging">是否应用分页。</param>
            /// <param name="distinct">是否去重。</param>
            /// <returns>查询 SQL。</returns>
            string ToSelectStatement(bool applyPaging, bool distinct);
            /// <summary>
            /// 生成更新语句。
            /// </summary>
            /// <param name="item">待更新的实体。</param>
            /// <param name="excludeDefaults">是否排除默认值字段。</param>
            /// <param name="allFields">是否更新所有字段。</param>
            /// <returns>更新 SQL。</returns>
            string ToUpdateStatement(T item, bool excludeDefaults, bool allFields);
            /// <summary>
            /// 生成 WHERE 条件语句。
            /// </summary>
            /// <returns>WHERE 条件 SQL。</returns>
            string ToWhereStatement();
        }
    }
}