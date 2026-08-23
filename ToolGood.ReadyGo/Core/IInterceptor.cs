using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 拦截器基接口，用于在 CRUD 生命周期中的不同阶段执行自定义逻辑。
    /// </summary>
    public interface IInterceptor
    {
    }

    /// <summary>
    /// 命令执行拦截器，在命令执行前后调用。
    /// </summary>
    public interface IExecutingInterceptor : IInterceptor
    {
        /// <summary>
        /// 在命令执行前调用。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        /// <param name="cmd">即将执行的命令。</param>
        void OnExecutingCommand(IDatabase database, DbCommand cmd);
        /// <summary>
        /// 在命令执行后调用。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        /// <param name="cmd">已执行的命令。</param>
        void OnExecutedCommand(IDatabase database, DbCommand cmd);
    }

    /// <summary>
    /// 连接拦截器，在连接打开和关闭时调用。
    /// </summary>
    public interface IConnectionInterceptor : IInterceptor
    {
        /// <summary>
        /// 在连接打开后调用。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        /// <param name="conn">已打开的连接。</param>
        /// <returns>实际使用的连接。</returns>
        DbConnection OnConnectionOpened(IDatabase database, DbConnection conn);
        /// <summary>
        /// 在连接关闭前调用。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        /// <param name="conn">即将关闭的连接。</param>
        void OnConnectionClosing(IDatabase database, DbConnection conn);
    }

    /// <summary>
    /// 异常拦截器，在发生异常时调用。
    /// </summary>
    public interface IExceptionInterceptor : IInterceptor
    {
        /// <summary>
        /// 在发生异常时调用。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        /// <param name="exception">发生的异常。</param>
        void OnException(IDatabase database, Exception exception);
    }

    /// <summary>
    /// 数据变更拦截器，在插入、更新、删除前后调用。
    /// </summary>
    public interface IDataInterceptor : IInterceptor
    {
        /// <summary>
        /// 在插入前调用。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        /// <param name="insertContext">插入上下文。</param>
        /// <returns>返回 false 可取消本次插入操作。</returns>
        bool OnInserting(IDatabase database, InsertContext insertContext);
        /// <summary>
        /// 在更新前调用。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        /// <param name="updateContext">更新上下文。</param>
        /// <returns>返回 false 可取消本次更新操作。</returns>
        bool OnUpdating(IDatabase database, UpdateContext updateContext);
        /// <summary>
        /// 在删除前调用。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        /// <param name="deleteContext">删除上下文。</param>
        /// <returns>返回 false 可取消本次删除操作。</returns>
        bool OnDeleting(IDatabase database, DeleteContext deleteContext);
    }

    /// <summary>
    /// 事务拦截器，在事务开始、中止和完成时调用。
    /// </summary>
    public interface ITransactionInterceptor : IInterceptor
    {
        /// <summary>
        /// 在事务开始时调用。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        void OnBeginTransaction(IDatabase database);
        /// <summary>
        /// 在事务中止时调用。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        void OnAbortTransaction(IDatabase database);
        /// <summary>
        /// 在事务完成时调用。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        void OnCompleteTransaction(IDatabase database);
    }
}
