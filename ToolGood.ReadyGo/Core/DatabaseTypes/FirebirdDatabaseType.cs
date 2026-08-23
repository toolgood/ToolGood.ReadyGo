
using System;
using System.Data;
using System.Data.Common;
using System.Text;
using ToolGood.ReadyGo.NPoco.Expressions;
using System.Threading.Tasks;
using System.Threading;

namespace ToolGood.ReadyGo.NPoco.DatabaseTypes
{
    /// <summary>
    /// Firebird 数据库类型实现。
    /// </summary>
    public class FirebirdDatabaseType : DatabaseType
    {
        /// <summary>
        /// 获取 Firebird 的参数前缀。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>参数前缀“@”。</returns>
        public override string GetParameterPrefix(string connectionString)
        {
            return "@";
        }

        /// <summary>
        /// 在执行命令前预处理命令文本，将 poco_dual 占位符替换为 Firebird 的系统表。
        /// </summary>
        /// <param name="cmd">数据库命令。</param>
        public override void PreExecute(DbCommand cmd)
        {
            cmd.CommandText = cmd.CommandText.Replace("/*poco_dual*/", "from RDB$DATABASE");
        }

        /// <summary>
        /// 使用双引号包裹表名。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>转义后的表名。</returns>
        public override string EscapeTableName(string tableName)
        {
            return  $"\"{tableName}\"";
        }

        /// <summary>
        /// 使用双引号包裹 SQL 标识符。
        /// </summary>
        /// <param name="str">标识符。</param>
        /// <returns>转义后的标识符。</returns>
        public override string EscapeSqlIdentifier(string str)
        {
            return $"\"{str}\"";
        }

        /// <summary>
        /// 构建 Firebird 分页查询，使用 FIRST/SKIP 语法。
        /// </summary>
        /// <param name="skip">跳过的记录数。</param>
        /// <param name="take">取回的记录数。</param>
        /// <param name="parts">SQL 各部分。</param>
        /// <param name="args">SQL 参数数组（引用传递）。</param>
        /// <returns>分页查询 SQL。</returns>
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            StringBuilder sql = new StringBuilder("SELECT ");

            if (take > 0)
                sql.AppendFormat("FIRST {0} ", take);

            if (skip > 0)
                sql.AppendFormat("SKIP {0} ", skip);

            sql.Append(parts.sqlSelectRemoved);
            return sql.ToString();
        }


        /// <summary>
        /// 获取默认的 INSERT 语句。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键名称。</param>
        /// <param name="useOutputClause">是否使用 OUTPUT 子句。</param>
        /// <param name="names">列名数组。</param>
        /// <param name="parameters">参数名数组。</param>
        /// <returns>INSERT 语句。</returns>
        public override string GetDefaultInsertSql(string tableName, string primaryKeyName, bool useOutputClause, string[] names, string[] parameters)
        {
            return string.Format("INSERT INTO {0} ({1}) VALUES ({2})", EscapeTableName(tableName), string.Join(",", names), string.Join(",", parameters));
        }


        private DbParameter AdjustSqlInsertCommandText(DbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText += string.Format(" returning {0}", EscapeSqlIdentifier(primaryKeyName));
            var param = cmd.CreateParameter();
            param.ParameterName = primaryKeyName;
            param.Value = DBNull.Value;
            param.Direction = ParameterDirection.ReturnValue;
            param.DbType = DbType.Int64;
            cmd.Parameters.Add(param);
            return param;
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
                var param = AdjustSqlInsertCommandText(cmd, primaryKeyName);
                (db as IDatabaseHelpers).ExecuteNonQueryHelper(cmd);
                return param.Value;
            }

            (db as IDatabaseHelpers).ExecuteNonQueryHelper(cmd);
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
                var param = AdjustSqlInsertCommandText(cmd, primaryKeyName);
                await (db as IDatabaseHelpers).ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
                return param.Value;
            }

            await (db as IDatabaseHelpers).ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
            return -1;
        }

        /// <summary>
        /// 创建 Firebird SQL 表达式访问器。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <param name="prefixTableName">是否添加表名前缀。</param>
        /// <returns>SQL 表达式访问器。</returns>
        public override ISqlExpression<T> ExpressionVisitor<T>(IDatabase db, PocoData pocoData, bool prefixTableName)
        {
            return new FirebirdSqlExpression<T>(db, pocoData, prefixTableName);
        }

        /// <summary>
        /// 获取 Firebird 驱动提供程序名称。
        /// </summary>
        /// <returns>提供程序名称。</returns>
        public override string GetProviderName()
        {
            return "FirebirdSql.Data.FirebirdClient";
        }
    }
}