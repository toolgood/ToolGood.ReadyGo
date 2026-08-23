using ToolGood.ReadyGo.NPoco.Expressions;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco.DatabaseTypes
{
    /// <summary>
    /// Oracle 数据库类型实现。
    /// </summary>
    public class OracleDatabaseType : DatabaseType
    {
        /// <summary>
        /// 创建 Oracle SQL 表达式访问器。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <param name="prefixTableName">是否添加表名前缀。</param>
        /// <returns>SQL 表达式访问器。</returns>
        public override ISqlExpression<T> ExpressionVisitor<T>(IDatabase db, PocoData pocoData, bool prefixTableName)
        {
            return new OracleExpression<T>(db, pocoData, prefixTableName);
        }

        /// <summary>
        /// 获取 Oracle 的参数前缀。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>参数前缀“:”。</returns>
        public override string GetParameterPrefix(string connectionString)
        {
            return ":";
        }

        /// <summary>
        /// 在执行命令前预处理：按名称绑定参数，并将 poco_dual 占位符替换为 dual 表。
        /// </summary>
        /// <param name="cmd">数据库命令。</param>
        public override void PreExecute(DbCommand cmd)
        {
            cmd.GetType().GetProperty("BindByName").SetValue(cmd, true, null);
            cmd.CommandText = cmd.CommandText.Replace("/*poco_dual*/", "from dual");
        }

        /// <summary>
        /// 构建 Oracle 分页查询。
        /// </summary>
        /// <param name="skip">跳过的记录数。</param>
        /// <param name="take">取回的记录数。</param>
        /// <param name="parts">SQL 各部分。</param>
        /// <param name="args">SQL 参数数组（引用传递）。</param>
        /// <returns>分页查询 SQL。</returns>
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (parts.sqlSelectRemoved.StartsWith("*"))
                throw new Exception("Query must alias '*' when performing a paged query.\neg. select t.* from table t order by t.id");

            // Same deal as SQL Server
            return PagingHelper.BuildPaging(skip, take, parts, ref args);
        }

        /// <summary>
        /// 使用双引号包裹 SQL 标识符并转为大写。
        /// </summary>
        /// <param name="str">标识符。</param>
        /// <returns>转义后的标识符。</returns>
        public override string EscapeSqlIdentifier(string str)
        {
            return string.Format("\"{0}\"", str.ToUpperInvariant());
        }

        /// <summary>
        /// 获取自增列表达式；若配置了序列名则返回“序列名.nextval”。
        /// </summary>
        /// <param name="ti">表信息。</param>
        /// <returns>自增表达式；未配置序列时返回 null。</returns>
        public override string GetAutoIncrementExpression(TableInfo ti)
        {
            if (!string.IsNullOrEmpty(ti.SequenceName))
                return string.Format("{0}.nextval", ti.SequenceName);

            return null;
        }

        private DbParameter AdjustSqlInsertCommandText(DbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText += string.Format(" returning {0} into :newid", EscapeSqlIdentifier(primaryKeyName));
            var param = cmd.CreateParameter();
            param.ParameterName = ":newid";
            param.Value = DBNull.Value;
            param.Direction = ParameterDirection.ReturnValue;
            param.DbType = DbType.Int64;
            cmd.Parameters.Add(param);
            return param;
        }

        /// <summary>
        /// 执行插入操作，并在指定主键时通过 RETURNING INTO 返回自增值。
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
                ((IDatabaseHelpers)db).ExecuteNonQueryHelper(cmd);
                return param.Value;
            }

            ((IDatabaseHelpers)db).ExecuteNonQueryHelper(cmd);
            return -1;
        }

        /// <summary>
        /// 异步执行插入操作，并在指定主键时通过 RETURNING INTO 返回自增值。
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
                await ((IDatabaseHelpers)db).ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
                return param.Value;
            }

            await ((IDatabaseHelpers)db).ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
            return -1;
        }

        /// <summary>
        /// 获取 Oracle 驱动提供程序名称。
        /// </summary>
        /// <returns>提供程序名称。</returns>
        public override string GetProviderName()
        {
            return "Oracle.DataAccess.Client";
        }
    }
}
