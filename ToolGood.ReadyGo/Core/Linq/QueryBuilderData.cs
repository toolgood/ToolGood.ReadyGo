using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 查询构建器使用的数据载体。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public class QueryBuilderData<T>
    {
        /// <summary>
        /// 初始化实例并创建排序表达式列表。
        /// </summary>
        public QueryBuilderData()
        {
            ThenByExpression = new List<Expression<Func<T, object>>>();
            ThenByDescendingExpression = new List<Expression<Func<T, object>>>();
        }

        /// <summary>
        /// 跳过的行数。
        /// </summary>
        public int? Skip { get; set; }
        /// <summary>
        /// 返回行数。
        /// </summary>
        public int? Rows { get; set; }
        /// <summary>
        /// WHERE 条件表达式。
        /// </summary>
        public Expression<Func<T, bool>> WhereExpression { get; set; }
        /// <summary>
        /// 升序排序表达式。
        /// </summary>
        public Expression<Func<T, object>> OrderByExpression { get; set; }
        /// <summary>
        /// 降序排序表达式。
        /// </summary>
        public Expression<Func<T, object>> OrderByDescendingExpression { get; set; }
        /// <summary>
        /// 追加的升序排序表达式列表。
        /// </summary>
        public List<Expression<Func<T, object>>> ThenByExpression  { get; private set; }
        /// <summary>
        /// 追加的降序排序表达式列表。
        /// </summary>
        public List<Expression<Func<T, object>>> ThenByDescendingExpression  { get; private set; }
    }
}