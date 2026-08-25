using ToolGood.ReadyGo.NPoco.Expressions;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco.DatabaseTypes
{
    /// <summary>
    /// PostgreSQL 数据库类型实现。
    /// </summary>
    public class PostgreSQLDatabaseType : DatabaseType
    {
        /// <summary>
        /// 创建 PostgreSQL SQL 表达式访问器。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <param name="prefixTableName">是否添加表名前缀。</param>
        /// <returns>SQL 表达式访问器。</returns>
        public override ISqlExpression<T> ExpressionVisitor<T>(IDatabase db, PocoData pocoData, bool prefixTableName)
        {
            return new PostgreSQLExpression<T>(db, pocoData, prefixTableName);
        }

        /// <summary>
        /// 映射参数值；PostgreSQL 原生支持 bool 类型，无需转为整数。
        /// </summary>
        /// <param name="value">待映射的值。</param>
        /// <returns>映射后的值。</returns>
        public override object MapParameterValue(object value)
        {
            // Don't map bools to ints in PostgreSQL
            if (value is bool) return value;

            return base.MapParameterValue(value);
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
        
        private void AdjustSqlInsertCommandText(DbCommand cmd, string primaryKeyName)
        {
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
        /// 获取 PostgreSQL 的参数前缀。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>参数前缀“@p”。</returns>
        public override string GetParameterPrefix(string connectionString)
        {
            return "@p";
        }

        /// <summary>
        /// 获取 PostgreSQL 驱动提供程序名称。
        /// </summary>
        /// <returns>提供程序名称。</returns>
        public override string GetProviderName()
        {
            return "Npgsql";
        }
    }
}