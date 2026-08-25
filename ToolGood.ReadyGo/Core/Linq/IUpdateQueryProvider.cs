using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 同步更新查询器接口。
    /// </summary>
    /// <typeparam name="T">更新的实体类型。</typeparam>
    public interface IUpdateQueryProvider<T>
    {
        /// <summary>
        /// 添加更新条件。
        /// </summary>
        /// <param name="whereExpression">更新条件表达式。</param>
        /// <returns>当前更新查询器。</returns>
        IUpdateQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 排除默认值字段（不更新为默认值的字段）。
        /// </summary>
        /// <returns>当前更新查询器。</returns>
        IUpdateQueryProvider<T> ExcludeDefaults();
        /// <summary>
        /// 仅更新指定字段。
        /// </summary>
        /// <param name="onlyFields">字段选择器。</param>
        /// <returns>当前更新查询器。</returns>
        IUpdateQueryProvider<T> OnlyFields(Expression<Func<T, object>> onlyFields);
        /// <summary>
        /// 执行更新。
        /// </summary>
        /// <param name="obj">待更新的实体。</param>
        /// <returns>受影响的行数。</returns>
        int Execute(T obj);
        /// <summary>
        /// 异步执行更新。
        /// </summary>
        /// <param name="obj">待更新的实体。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>受影响的行数。</returns>
        Task<int> Execute_Async(T obj, CancellationToken cancellationToken = default);
    }
}
