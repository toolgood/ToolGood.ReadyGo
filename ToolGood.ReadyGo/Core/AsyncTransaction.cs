using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示一个异步数据库事务，用于管理事务的提交、回滚与释放。
    /// </summary>
    public class AsyncTransaction : IAsyncTransaction
    {
        IAsyncDatabase _db;

        private AsyncTransaction(IAsyncDatabase db)
        {
            _db = db;
        }

#pragma warning disable CS1998
        /// <summary>
        /// 在指定数据库上开启一个具有指定隔离级别的事务，并返回对应的事务实例。
        /// </summary>
        /// <param name="db">要开启事务的数据库实例。</param>
        /// <param name="isolationLevel">事务的隔离级别。</param>
        /// <returns>已开启的异步事务实例。</returns>
        public static async Task<IAsyncTransaction> Init(IAsyncDatabase db, IsolationLevel isolationLevel)
        {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
            await db.BeginTransactionAsync(isolationLevel);
#else
            ((IBaseDatabase)db).BeginTransaction();
#endif
      
            return new AsyncTransaction(db);
        }

        /// <summary>
        /// 提交当前事务。
        /// </summary>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        public async Task CompleteAsync(CancellationToken cancellationToken = default)
        {

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
            await _db.CompleteTransactionAsync(cancellationToken);
#else
            ((IBaseDatabase)_db).CompleteTransaction();
#endif
            _db = null;
        }

        /// <summary>
        /// 释放事务资源；若事务尚未完成，则回滚事务。
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_db != null)
            {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                await _db.AbortTransactionAsync();
#else
                ((IBaseDatabase)_db).AbortTransaction();
#endif
            }
        }
#pragma warning restore CS1998

        /// <summary>
        /// 释放事务资源；若事务尚未完成，则回滚事务。
        /// </summary>
        public void Dispose()
        {
            ((IBaseDatabase)_db)?.AbortTransaction();
        }
    }
}