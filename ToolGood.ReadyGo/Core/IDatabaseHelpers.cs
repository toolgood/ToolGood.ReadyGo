using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 定义执行数据库命令的帮助方法接口，用于封装同步与异步的执行、读取操作。
    /// </summary>
    public interface IDatabaseHelpers
    {
        /// <summary>
        /// 执行命令并返回受影响的行数。
        /// </summary>
        /// <param name="cmd">要执行的数据库命令。</param>
        /// <returns>受影响的行数。</returns>
        int ExecuteNonQueryHelper(DbCommand cmd);
        /// <summary>
        /// 执行命令并返回结果集中第一行第一列的值。
        /// </summary>
        /// <param name="cmd">要执行的数据库命令。</param>
        /// <returns>结果集中第一行第一列的值。</returns>
        object ExecuteScalarHelper(DbCommand cmd);
        /// <summary>
        /// 执行命令并返回数据读取器。
        /// </summary>
        /// <param name="cmd">要执行的数据库命令。</param>
        /// <returns>用于读取结果集的数据读取器。</returns>
        DbDataReader ExecuteReaderHelper(DbCommand cmd);
        /// <summary>
        /// 异步执行命令并返回受影响的行数。
        /// </summary>
        /// <param name="cmd">要执行的数据库命令。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>表示异步操作的任务，结果为受影响的行数。</returns>
        Task<int> ExecuteNonQueryHelperAsync(DbCommand cmd, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步执行命令并返回结果集中第一行第一列的值。
        /// </summary>
        /// <param name="cmd">要执行的数据库命令。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>表示异步操作的任务，结果为结果集中第一行第一列的值。</returns>
        Task<object> ExecuteScalarHelperAsync(DbCommand cmd, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步执行命令并返回数据读取器。
        /// </summary>
        /// <param name="cmd">要执行的数据库命令。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>表示异步操作的任务，结果为用于读取结果集的数据读取器。</returns>
        Task<DbDataReader> ExecuteReaderHelperAsync(DbCommand cmd, CancellationToken cancellationToken = default);
    }
}
