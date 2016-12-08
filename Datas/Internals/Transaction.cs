using System;
using System.Data;
using ToolGood.ReadyGo.Poco;

namespace ToolGood.ReadyGo.Internals
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class Transaction : IDisposable
    {
        private Database _db;
        private bool IsAbort = false;

        internal Transaction(Database db, IsolationLevel? _isolationLevel)
        {
            _db = db;
            _db.BeginTransaction(_isolationLevel);
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void Complete()
        {
            if (_db != null) {
                IsAbort = false;
                _db.CompleteTransaction();
                //_db = null;
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        public void Abort()
        {
            if (_db != null) {
                IsAbort = true;
                _db.AbortTransaction();
                //_db = null;
            }
        }

        /// <summary>
        /// 释放内存
        /// </summary>
        public void Dispose()
        {
            if (_db != null) {
                if (IsAbort) {
                    _db.AbortTransaction();
                } else {
                    _db.CompleteTransaction();
                }
                _db = null;
            }
        }
    }
}