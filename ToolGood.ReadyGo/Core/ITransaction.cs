using System;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示一个数据库事务，提供提交事务的能力，并实现 <see cref="IDisposable"/> 以支持资源释放。
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// 提交当前事务。
        /// </summary>
        void Complete();
    }
}
