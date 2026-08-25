#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 异步查询结果接口，提供异步执行查询的各类结果获取方法。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public interface IAsyncQueryResultProvider<T>
    {
        /// <summary>
        /// 异步返回结果列表。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>结果列表。</returns>
        Task<List<T>> ToList(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回结果列表（ToList 的别名）。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>结果列表。</returns>
        Task<List<T>> Select(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回结果数组。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>结果数组。</returns>
        Task<T[]> ToArray(CancellationToken cancellationToken = default);
        /// <summary>
        /// 返回异步枚举序列。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>异步枚举序列。</returns>
        IAsyncEnumerable<T> ToEnumerable(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回第一个元素或默认值。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素或默认值。</returns>
        Task<T?> FirstOrDefault(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回满足条件的第一个元素或默认值。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素或默认值。</returns>
        Task<T?> FirstOrDefault(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回第一个元素。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素。</returns>
        Task<T> First(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回满足条件的第一个元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素。</returns>
        Task<T> First(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回唯一元素或默认值。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素或默认值。</returns>
        Task<T?> SingleOrDefault(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回满足条件的唯一元素或默认值。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素或默认值。</returns>
        Task<T?> SingleOrDefault(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回唯一元素。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素。</returns>
        Task<T> Single(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回满足条件的唯一元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素。</returns>
        Task<T> Single(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回元素数量。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        Task<int> Count(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步统计元素数量（Count 的别名）。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        Task<int> SelectCount(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回满足条件的元素数量。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        Task<int> Count(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步统计满足条件的元素数量（Count 的别名）。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        Task<int> SelectCount(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步判断是否存在元素。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        Task<bool> Any(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步判断是否存在满足条件的元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        Task<bool> Any(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步分页返回结果。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>分页结果。</returns>
        Task<Page<T>> ToPage(int page, int pageSize, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步分页返回结果（ToPage 的别名）。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>分页结果。</returns>
        Task<Page<T>> Page_Async(int page, int pageSize, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步投影返回结果列表。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>投影结果列表。</returns>
        Task<List<T2>> ProjectTo<T2>(Expression<Func<T, T2>> projectionExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步投影分页返回结果。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>投影分页结果。</returns>
        Task<Page<T2>> ToProjectedPage<T2>(Expression<Func<T, T2>> projectionExpression, int page, int pageSize, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步按投影去重返回结果列表。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>去重后的投影结果列表。</returns>
        Task<List<T2>> Distinct<T2>(Expression<Func<T, T2>> projectionExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步去重返回结果列表。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>去重后的结果列表。</returns>
        Task<List<T>> Distinct(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 同步查询结果接口，提供同步与异步执行查询的各类结果获取方法。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public interface IQueryResultProvider<T>
    {
        /// <summary>
        /// 返回第一个元素或默认值。
        /// </summary>
        /// <returns>第一个元素或默认值。</returns>
        T? FirstOrDefault();
        /// <summary>
        /// 返回满足条件的第一个元素或默认值。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>第一个元素或默认值。</returns>
        T? FirstOrDefault(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 返回第一个元素。
        /// </summary>
        /// <returns>第一个元素。</returns>
        T First();
        /// <summary>
        /// 返回满足条件的第一个元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>第一个元素。</returns>
        T First(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 返回唯一元素或默认值。
        /// </summary>
        /// <returns>唯一元素或默认值。</returns>
        T? SingleOrDefault();
        /// <summary>
        /// 返回满足条件的唯一元素或默认值。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>唯一元素或默认值。</returns>
        T? SingleOrDefault(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 返回唯一元素。
        /// </summary>
        /// <returns>唯一元素。</returns>
        T Single();
        /// <summary>
        /// 返回满足条件的唯一元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>唯一元素。</returns>
        T Single(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 返回元素数量。
        /// </summary>
        /// <returns>元素数量。</returns>
        int Count();
        /// <summary>
        /// 统计元素数量（Count 的别名）。
        /// </summary>
        /// <returns>元素数量。</returns>
        int SelectCount();
        /// <summary>
        /// 返回满足条件的元素数量。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>元素数量。</returns>
        int Count(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 统计满足条件的元素数量（Count 的别名）。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>元素数量。</returns>
        int SelectCount(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 判断是否存在元素。
        /// </summary>
        /// <returns>存在返回 true，否则返回 false。</returns>
        bool Any();
        /// <summary>
        /// 判断是否存在满足条件的元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        bool Any(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 返回结果列表。
        /// </summary>
        /// <returns>结果列表。</returns>
        List<T> ToList();
        /// <summary>
        /// 返回结果列表（ToList 的别名）。
        /// </summary>
        /// <returns>结果列表。</returns>
        List<T> Select();
        /// <summary>
        /// 返回结果数组。
        /// </summary>
        /// <returns>结果数组。</returns>
        T[] ToArray();
        /// <summary>
        /// 返回枚举序列。
        /// </summary>
        /// <returns>枚举序列。</returns>
        IEnumerable<T> ToEnumerable();
        /// <summary>
        /// 返回动态对象列表。
        /// </summary>
        /// <returns>动态对象列表。</returns>
        List<dynamic> ToDynamicList();
        /// <summary>
        /// 返回动态对象枚举序列。
        /// </summary>
        /// <returns>动态对象枚举序列。</returns>
        IEnumerable<dynamic> ToDynamicEnumerable();
        /// <summary>
        /// 分页返回结果。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <returns>分页结果。</returns>
        Page<T> ToPage(int page, int pageSize);
        /// <summary>
        /// 分页返回结果（ToPage 的别名）。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <returns>分页结果。</returns>
        Page<T> Page(int page, int pageSize);
        /// <summary>
        /// 投影返回结果列表。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <returns>投影结果列表。</returns>
        List<T2> ProjectTo<T2>(Expression<Func<T, T2>> projectionExpression);
        /// <summary>
        /// 投影分页返回结果。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <returns>投影分页结果。</returns>
        Page<T2> ToProjectedPage<T2>(Expression<Func<T, T2>> projectionExpression, int page, int pageSize);
        /// <summary>
        /// 按投影去重返回结果列表。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <returns>去重后的投影结果列表。</returns>
        List<T2> Distinct<T2>(Expression<Func<T, T2>> projectionExpression);
        /// <summary>
        /// 去重返回结果列表。
        /// </summary>
        /// <returns>去重后的结果列表。</returns>
        List<T> Distinct();
        /// <summary>
        /// 异步返回结果列表。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>结果列表。</returns>
        Task<List<T>> ToList_Async(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回结果列表（ToList_Async 的别名）。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>结果列表。</returns>
        Task<List<T>> Select_Async(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回结果数组。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>结果数组。</returns>
        Task<T[]> ToArray_Async(CancellationToken cancellationToken = default);
        /// <summary>
        /// 返回异步枚举序列。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>异步枚举序列。</returns>
        IAsyncEnumerable<T> ToEnumerable_Async(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回第一个元素或默认值。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素或默认值。</returns>
        Task<T?> FirstOrDefault_Async(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回满足条件的第一个元素或默认值。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素或默认值。</returns>
        Task<T?> FirstOrDefault_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回第一个元素。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素。</returns>
        Task<T> First_Async(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回满足条件的第一个元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素。</returns>
        Task<T> First_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回唯一元素或默认值。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素或默认值。</returns>
        Task<T?> SingleOrDefault_Async(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回满足条件的唯一元素或默认值。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素或默认值。</returns>
        Task<T?> SingleOrDefault_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回唯一元素。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素。</returns>
        Task<T> Single_Async(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回满足条件的唯一元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素。</returns>
        Task<T> Single_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回元素数量。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        Task<int> Count_Async(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步统计元素数量（Count_Async 的别名）。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        Task<int> SelectCount_Async(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步返回满足条件的元素数量。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        Task<int> Count_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步统计满足条件的元素数量（Count_Async 的别名）。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        Task<int> SelectCount_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步判断是否存在元素。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        Task<bool> Any_Async(CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步判断是否存在满足条件的元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        Task<bool> Any_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步分页返回结果。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>分页结果。</returns>
        Task<Page<T>> ToPage_Async(int page, int pageSize, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步分页返回结果（ToPage_Async 的别名）。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>分页结果。</returns>
        Task<Page<T>> Page_Async(int page, int pageSize, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步投影返回结果列表。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>投影结果列表。</returns>
        Task<List<T2>> ProjectTo_Async<T2>(Expression<Func<T, T2>> projectionExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步投影分页返回结果。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>投影分页结果。</returns>
        Task<Page<T2>> ToProjectedPage_Async<T2>(Expression<Func<T, T2>> projectionExpression, int page, int pageSize, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步按投影去重返回结果列表。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>去重后的投影结果列表。</returns>
        Task<List<T2>> Distinct_Async<T2>(Expression<Func<T, T2>> projectionExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步去重返回结果列表。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>去重后的结果列表。</returns>
        Task<List<T>> Distinct_Async(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 同步查询器接口，提供链式查询构建能力。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public interface IQueryProvider<T> : IQueryResultProvider<T>
    {
        /// <summary>
        /// 添加 WHERE 条件。
        /// </summary>
        /// <param name="whereExpression">条件表达式。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 添加 WHERE 条件 SQL。
        /// </summary>
        /// <param name="sql">条件 SQL。</param>
        /// <param name="args">条件参数。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> WhereSql(string sql, params object[] args);
        /// <summary>
        /// 添加 WHERE 条件 SQL。
        /// </summary>
        /// <param name="sql">条件 SQL。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> WhereSql(Sql sql);
        /// <summary>
        /// 添加 WHERE 条件 SQL（通过查询上下文构建）。
        /// </summary>
        /// <param name="queryBuilder">查询上下文构建函数。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> WhereSql(Func<QueryContext<T>, Sql> queryBuilder);
        /// <summary>
        /// 添加升序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> OrderBy(Expression<Func<T, object>> column);
        /// <summary>
        /// 添加降序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> OrderByDescending(Expression<Func<T, object>> column);
        /// <summary>
        /// 追加升序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> ThenBy(Expression<Func<T, object>> column);
        /// <summary>
        /// 追加降序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> ThenByDescending(Expression<Func<T, object>> column);
        /// <summary>
        /// 限制返回行数。
        /// </summary>
        /// <param name="rows">返回行数。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> Limit(int rows);
        /// <summary>
        /// 限制返回行数并跳过指定行数。
        /// </summary>
        /// <param name="skip">跳过的行数。</param>
        /// <param name="rows">返回行数。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> Limit(int skip, int rows);
        /// <summary>
        /// 应用查询构建器中的条件、排序与分页。
        /// </summary>
        /// <param name="builder">查询构建器。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> From(QueryBuilder<T> builder);
    }

    /// <summary>
    /// 异步查询器接口，提供链式查询构建能力。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public interface IAsyncQueryProvider<T> : IAsyncQueryResultProvider<T>
    {
        /// <summary>
        /// 添加 WHERE 条件。
        /// </summary>
        /// <param name="whereExpression">条件表达式。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 添加 WHERE 条件 SQL。
        /// </summary>
        /// <param name="sql">条件 SQL。</param>
        /// <param name="args">条件参数。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> WhereSql(string sql, params object[] args);
        /// <summary>
        /// 添加 WHERE 条件 SQL。
        /// </summary>
        /// <param name="sql">条件 SQL。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> WhereSql(Sql sql);
        /// <summary>
        /// 添加 WHERE 条件 SQL（通过查询上下文构建）。
        /// </summary>
        /// <param name="queryBuilder">查询上下文构建函数。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> WhereSql(Func<QueryContext<T>, Sql> queryBuilder);
        /// <summary>
        /// 添加升序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> OrderBy(Expression<Func<T, object>> column);
        /// <summary>
        /// 添加降序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> OrderByDescending(Expression<Func<T, object>> column);
        /// <summary>
        /// 追加升序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> ThenBy(Expression<Func<T, object>> column);
        /// <summary>
        /// 追加降序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> ThenByDescending(Expression<Func<T, object>> column);
        /// <summary>
        /// 限制返回行数。
        /// </summary>
        /// <param name="rows">返回行数。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> Limit(int rows);
        /// <summary>
        /// 限制返回行数并跳过指定行数。
        /// </summary>
        /// <param name="skip">跳过的行数。</param>
        /// <param name="rows">返回行数。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> Limit(int skip, int rows);
        /// <summary>
        /// 应用查询构建器中的条件、排序与分页。
        /// </summary>
        /// <param name="builder">查询构建器。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> From(QueryBuilder<T> builder);
    }

    /// <summary>
    /// 带关联加载的异步查询器接口。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public interface IAsyncQueryProviderWithIncludes<T> : IAsyncQueryProvider<T>
    {
        /// <summary>
        /// 添加一对多关联加载。
        /// </summary>
        /// <param name="expression">集合属性表达式。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProvider<T> IncludeMany(Expression<Func<T, IList>> expression, JoinType joinType = JoinType.Left, string joinTableHint = "");
        /// <summary>
        /// 按类型自动加载一对一或外键关联。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProviderWithIncludes<T> Include<T2>(JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class;
        /// <summary>
        /// 按表达式加载关联。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="expression">关联属性表达式。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProviderWithIncludes<T> Include<T2>(Expression<Func<T, T2>> expression, JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class;
        /// <summary>
        /// 按表达式加载关联并指定表别名。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="expression">关联属性表达式。</param>
        /// <param name="tableAlias">表别名。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProviderWithIncludes<T> Include<T2>(Expression<Func<T, T2>> expression, string tableAlias, JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class;
        /// <summary>
        /// 指定主表别名。
        /// </summary>
        /// <param name="tableAlias">表别名。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProviderWithIncludes<T> UsingAlias(string tableAlias);
        /// <summary>
        /// 添加表提示。
        /// </summary>
        /// <param name="tableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        IAsyncQueryProviderWithIncludes<T> Hint(string tableHint);
    }

    /// <summary>
    /// 带关联加载的同步查询器接口。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public interface IQueryProviderWithIncludes<T> : IQueryProvider<T>
    {
        /// <summary>
        /// 添加一对多关联加载。
        /// </summary>
        /// <param name="expression">集合属性表达式。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        IQueryProvider<T> IncludeMany(Expression<Func<T, IList>> expression, JoinType joinType = JoinType.Left, string joinTableHint = "");
        /// <summary>
        /// 按类型自动加载一对一或外键关联。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        IQueryProviderWithIncludes<T> Include<T2>(JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class;
        /// <summary>
        /// 按表达式加载关联。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="expression">关联属性表达式。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        IQueryProviderWithIncludes<T> Include<T2>(Expression<Func<T, T2>> expression, JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class;
        /// <summary>
        /// 按表达式加载关联并指定表别名。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="expression">关联属性表达式。</param>
        /// <param name="tableAlias">表别名。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        IQueryProviderWithIncludes<T> Include<T2>(Expression<Func<T, T2>> expression, string tableAlias, JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class;
        /// <summary>
        /// 指定主表别名。
        /// </summary>
        /// <param name="tableAlias">表别名。</param>
        /// <returns>当前查询器。</returns>
        IQueryProviderWithIncludes<T> UsingAlias(string tableAlias);
        /// <summary>
        /// 添加表提示。
        /// </summary>
        /// <param name="tableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        IQueryProviderWithIncludes<T> Hint(string tableHint);
    }
}
