using System;
using System.Data;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示一个数据库事务，在 Dispose 时自动中止未完成的事务。
    /// </summary>
    public class Transaction : ITransaction
    {
        IDatabase _db;

        /// <summary>
        /// 初始化 Transaction 类的新实例并开启事务。
        /// </summary>
        /// <param name="db">数据库实例。</param>
        /// <param name="isolationLevel">事务隔离级别。</param>
        public Transaction(IDatabase db, IsolationLevel isolationLevel)
        {
            _db = db;
            _db.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// 提交并完成事务。
        /// </summary>
        public virtual void Complete()
        {
            _db.CompleteTransaction();
            _db = null;
        }

        /// <summary>
        /// 释放资源；若事务尚未完成则中止事务。
        /// </summary>
        public void Dispose()
        {
            if (_db != null)
            {
                _db.AbortTransaction();
            }
        }
    }
}
