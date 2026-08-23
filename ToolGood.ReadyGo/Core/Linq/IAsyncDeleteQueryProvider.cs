using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 异步删除查询器接口。
    /// </summary>
    /// <typeparam name="T">删除的实体类型。</typeparam>
    public interface IAsyncDeleteQueryProvider<T>
    {
        /// <summary>
        /// 添加删除条件。
        /// </summary>
        /// <param name="whereExpression">删除条件表达式。</param>
        /// <returns>当前删除查询器。</returns>
        IAsyncDeleteQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression);
        /// <summary>
        /// 异步执行删除。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>受影响的行数。</returns>
        Task<int> Execute(CancellationToken cancellationToken = default);
    }
}