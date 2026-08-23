using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using ToolGood.ReadyGo.NPoco.Expressions;

namespace ToolGood.ReadyGo.NPoco.DatabaseTypes
{
    /// <summary>
    /// DuckDB 数据库类型（基于 DuckDB.NET 驱动）
    /// </summary>
    public class DuckDbDatabaseType : DatabaseType
    {
        /// <summary>
        /// 获取 DuckDB 的参数前缀（$p，如 $p0）。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>参数前缀字符串。</returns>
        public override string GetParameterPrefix(string connectionString)
        {
            // DuckDB 命名参数使用 $ 前缀（如 $p0），与内核默认的 @ 不兼容
            return "$p";
        }

        /// <summary>
        /// 映射参数值；DuckDB 原生支持 bool 类型，无需转为 1/0。
        /// </summary>
        /// <param name="value">待映射的值。</param>
        /// <returns>映射后的值。</returns>
        public override object MapParameterValue(object value)
        {
            // DuckDB 原生支持 bool 类型，无需转为 1/0
            if (value is bool) return value;

            return base.MapParameterValue(value);
        }

        /// <summary>
        /// 使用双引号包裹表名。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>转义后的表名。</returns>
        public override string EscapeTableName(string tableName)
        {
            return string.Format("\"{0}\"", tableName);
        }

        /// <summary>
        /// 使用双引号包裹 SQL 标识符。
        /// </summary>
        /// <param name="str">标识符。</param>
        /// <returns>转义后的标识符。</returns>
        public override string EscapeSqlIdentifier(string str)
        {
            return string.Format("\"{0}\"", str);
        }

        /// <summary>
        /// 获取 EXISTS 查询的 SQL 模板。
        /// </summary>
        /// <returns>EXISTS 查询 SQL 模板。</returns>
        public override string GetExistsSql()
        {
            return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
        }

        private void AdjustSqlInsertCommandText(DbCommand cmd, string primaryKeyName)
        {
            // DuckDB 0.7+ 支持 INSERT ... RETURNING
            cmd.CommandText += string.Format(" returning {0} as NewID", EscapeSqlIdentifier(primaryKeyName));
        }

        /// <summary>
        /// 执行插入操作，并在指定主键时通过 RETURNING 返回自增值。
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
                AdjustSqlInsertCommandText(cmd, primaryKeyName);
                return ((IDatabaseHelpers)db).ExecuteScalarHelper(cmd);
            }

            ((IDatabaseHelpers)db).ExecuteNonQueryHelper(cmd);
            return -1;
        }

        /// <summary>
        /// 异步执行插入操作，并在指定主键时通过 RETURNING 返回自增值。
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
                AdjustSqlInsertCommandText(cmd, primaryKeyName);
                return await ((IDatabaseHelpers)db).ExecuteScalarHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
            }

            await ((IDatabaseHelpers)db).ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
            return -1;
        }

        /// <summary>
        /// 获取 DuckDB 驱动提供程序名称。
        /// </summary>
        /// <returns>提供程序名称。</returns>
        public override string GetProviderName()
        {
            return "DuckDB.NET.Data.Full";
        }
    }
}
