using System;
using System.Linq.Expressions;
using ToolGood.ReadyGo.NPoco.Expressions;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 查询构建器，用于以编程方式组合查询条件、排序与分页。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public class QueryBuilder<T>
    {
        /// <summary>
        /// 构建器数据。
        /// </summary>
        public QueryBuilderData<T> Data { get; private set; }

        /// <summary>
        /// 初始化构建器。
        /// </summary>
        public QueryBuilder()
        {
            Data = new QueryBuilderData<T>();
        }

        /// <summary>
        /// 设置返回行数。
        /// </summary>
        /// <param name="rows">返回行数。</param>
        /// <returns>当前构建器。</returns>
        public virtual QueryBuilder<T> Limit(int rows)
        {
            Data.Rows = rows;
            return this;
        }

        /// <summary>
        /// 设置返回行数并跳过指定行数。
        /// </summary>
        /// <param name="rows">返回行数。</param>
        /// <param name="skip">跳过的行数。</param>
        /// <returns>当前构建器。</returns>
        public virtual QueryBuilder<T> Limit(int rows, int skip)
        {
            Data.Rows = rows;
            Data.Skip = skip;
            return this;
        }

        /// <summary>
        /// 追加 WHERE 条件。
        /// </summary>
        /// <param name="whereExpression">条件表达式。</param>
        /// <returns>当前构建器。</returns>
        public virtual QueryBuilder<T> Where(Expression<Func<T, bool>> whereExpression)
        {
            Data.WhereExpression = Data.WhereExpression == null ? PredicateBuilder.Create(whereExpression) : Data.WhereExpression.And(whereExpression);
            return this;
        }

        /// <summary>
        /// 设置升序排序字段。
        /// </summary>
        /// <param name="orderByExpression">排序字段表达式。</param>
        /// <returns>当前构建器。</returns>
        public virtual QueryBuilder<T> OrderBy(Expression<Func<T, object>> orderByExpression)
        {
            Data.OrderByExpression = orderByExpression;
            return this;
        }

        /// <summary>
        /// 设置降序排序字段。
        /// </summary>
        /// <param name="orderByDescendingExpression">排序字段表达式。</param>
        /// <returns>当前构建器。</returns>
        public virtual QueryBuilder<T> OrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            Data.OrderByDescendingExpression = orderByDescendingExpression;
            return this;
        }

        /// <summary>
        /// 追加升序排序字段。
        /// </summary>
        /// <param name="thenByExpression">排序字段表达式。</param>
        /// <returns>当前构建器。</returns>
        public virtual QueryBuilder<T> ThenBy(Expression<Func<T, object>> thenByExpression)
        {
            Data.ThenByExpression.Add(thenByExpression);
            return this;
        }

        /// <summary>
        /// 追加降序排序字段。
        /// </summary>
        /// <param name="thenByDescendingExpression">排序字段表达式。</param>
        /// <returns>当前构建器。</returns>
        public virtual QueryBuilder<T> ThenByDescending(Expression<Func<T, object>> thenByDescendingExpression)
        {
            Data.ThenByDescendingExpression.Add(thenByDescendingExpression);
            return this;
        }
    }
}