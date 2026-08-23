using System;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示一个异步事务，支持提交与异步释放。
    /// </summary>
    public interface IAsyncTransaction : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// 提交事务。
        /// </summary>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        Task CompleteAsync(CancellationToken cancellationToken = default);
    }
}