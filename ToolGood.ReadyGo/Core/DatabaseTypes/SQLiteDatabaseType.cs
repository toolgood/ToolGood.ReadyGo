using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco.DatabaseTypes
{
    /// <summary>
    /// SQLite 数据库类型实现。
    /// </summary>
    public class SQLiteDatabaseType : DatabaseType
    {
        /// <summary>
        /// 映射参数值；将 uint 转换为 long。
        /// </summary>
        /// <param name="value">待映射的值。</param>
        /// <returns>映射后的值。</returns>
        public override object MapParameterValue(object value)
        {
            if (value is uint)
                return (long)((uint)value);

            return base.MapParameterValue(value);
        }

        private void AdjustSqlInsertCommandText(DbCommand cmd)
        {
            cmd.CommandText += ";\nSELECT last_insert_rowid();";
        }

        /// <summary>
        /// 执行插入操作，并在指定主键时返回最后插入的行号。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="cmd">数据库命令。</param>
        /// <param name="primaryKeyName">主键名称（可为 null）。</param>
        /// <param name="useOutputClause">是否使用 OUTPUT 子句。</param>
        /// <param name="poco">待插入的 POCO 对象。</param>
        /// <param name="args">附加参数。</param>
        /// <returns>主键值；无主键时返回 -1。</returns>
        public override object ExecuteInsert<T>(IDatabase db, DbCommand cmd, string primaryKeyName, bool useOutputClause, T poco, object[] args)
        {
            if (primaryKeyName != null)
            {
                AdjustSqlInsertCommandText(cmd);
                return ((IDatabaseHelpers)db).ExecuteScalarHelper(cmd);
            }

            ((IDatabaseHelpers)db).ExecuteNonQueryHelper(cmd);
            return -1;
        }

        /// <summary>
        /// 异步执行插入操作，并在指定主键时返回最后插入的行号。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="cmd">数据库命令。</param>
        /// <param name="primaryKeyName">主键名称（可为 null）。</param>
        /// <param name="useOutputClause">是否使用 OUTPUT 子句。</param>
        /// <param name="poco">待插入的 POCO 对象。</param>
        /// <param name="args">附加参数。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>主键值；无主键时返回 -1。</returns>
        public override async Task<object> ExecuteInsertAsync<T>(IDatabase db, DbCommand cmd, string primaryKeyName, bool useOutputClause, T poco, object[] args, CancellationToken cancellationToken = default)
        {
            if (primaryKeyName != null)
            {
                AdjustSqlInsertCommandText(cmd);
                return await ((IDatabaseHelpers)db).ExecuteScalarHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
            }

            await ((IDatabaseHelpers)db).ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
            return -1;
        }

        /// <summary>
        /// 获取 EXISTS 查询的 SQL 模板。
        /// </summary>
        /// <returns>EXISTS 查询 SQL 模板。</returns>
        public override string GetExistsSql()
        {
            return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
        }

        /// <summary>
        /// 获取默认的事务隔离级别。
        /// </summary>
        /// <returns>默认为 ReadCommitted。</returns>
        public override IsolationLevel GetDefaultTransactionIsolationLevel()
        {
            return IsolationLevel.ReadCommitted;
        }

        /// <summary>
        /// 获取 SQLite 驱动提供程序名称。
        /// </summary>
        /// <returns>提供程序名称。</returns>
        public override string GetProviderName()
        {
            return "System.Data.SQLite";
        }
    }
}