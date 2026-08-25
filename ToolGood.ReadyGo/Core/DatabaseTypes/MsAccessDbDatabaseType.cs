using System;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco.DatabaseTypes
{
    /// <summary>
    /// Microsoft Access 数据库类型（基于 System.Data.OleDb / Jet.OLEDB / ACE.OLEDB 驱动）
    /// </summary>
    public class MsAccessDbDatabaseType : DatabaseType
    {
        // 把内核生成的 ?N（N 为参数序号）位置参数占位符收敛为 OleDb 支持的裸 ?
        private static readonly Regex RxOleDbParam = new Regex(@"\?\d+", RegexOptions.Compiled);

        /// <summary>
        /// 获取 Access 的参数前缀。OleDb 使用位置参数，内核 @N 会被替换为 ?N，
        /// 最终在 <see cref="PreExecute"/> 中收敛为裸 ?。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>参数前缀字符串。</returns>
        public override string GetParameterPrefix(string connectionString)
        {
            return "?";
        }

        /// <summary>
        /// 在命令执行前把 ?N 位置参数占位符转为 OleDb 支持的裸 ?（按参数集合顺序绑定）。
        /// </summary>
        /// <param name="cmd">即将执行的数据库命令。</param>
        public override void PreExecute(DbCommand cmd)
        {
            cmd.CommandText = RxOleDbParam.Replace(cmd.CommandText, "?");
        }

        /// <summary>
        /// 构建分页查询语句。Access 仅支持 SELECT TOP n，不支持 OFFSET，
        /// 因此仅第一页（skip=0）可用，翻页抛异常。
        /// </summary>
        /// <param name="skip">要跳过的记录数。</param>
        /// <param name="take">要获取的记录数。</param>
        /// <param name="parts">拆分后的 SQL 片段。</param>
        /// <param name="args">查询参数数组，按引用传递。</param>
        /// <returns>分页查询语句。</returns>
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (skip <= 0) {
                return Regex.Replace(parts.sql, "^SELECT", $"SELECT TOP {take}", RegexOptions.IgnoreCase);
            }
            throw new NotSupportedException("The Access provider does not support paging with skip offset.");
        }

        /// <summary>
        /// 获取默认的插入语句。Access 不支持 DEFAULT VALUES，无列插入时直接报错。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="useOutputClause">是否使用 OUTPUT 子句。</param>
        /// <param name="names">列名数组。</param>
        /// <param name="parameters">参数名数组。</param>
        /// <returns>默认的插入语句。</returns>
        public override string GetDefaultInsertSql(string tableName, string primaryKeyName, bool useOutputClause, string[] names, string[] parameters)
        {
            throw new NotSupportedException("The Access provider does not support DEFAULT VALUES insert.");
        }

        /// <summary>
        /// 执行插入操作，并在指定主键时通过 @@IDENTITY 返回自增值。
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
            if (primaryKeyName != null) {
                // 先执行 INSERT，再单独查询 @@IDENTITY（Jet/ACE 支持该全局标识变量）
                ((IDatabaseHelpers)db).ExecuteNonQueryHelper(cmd);
                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT @@IDENTITY AS NewID;";
                return ((IDatabaseHelpers)db).ExecuteScalarHelper(cmd);
            }

            ((IDatabaseHelpers)db).ExecuteNonQueryHelper(cmd);
            return -1;
        }

        /// <summary>
        /// 异步执行插入操作，并在指定主键时通过 @@IDENTITY 返回自增值。
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
            if (primaryKeyName != null) {
                await ((IDatabaseHelpers)db).ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT @@IDENTITY AS NewID;";
                return await ((IDatabaseHelpers)db).ExecuteScalarHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
            }

            await ((IDatabaseHelpers)db).ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
            return -1;
        }

        /// <summary>
        /// 获取 Access 驱动提供程序名称。
        /// </summary>
        /// <returns>提供程序名称。</returns>
        public override string GetProviderName()
        {
            return "System.Data.OleDb";
        }
    }
}
