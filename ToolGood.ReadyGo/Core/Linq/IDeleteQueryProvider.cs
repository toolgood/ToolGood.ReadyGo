using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    public interface IDeleteQueryProvider<T>
    {
        IDeleteQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression);
        int Execute();
        Task<int> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}