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
        public override string GetParameterPrefix(string connectionString)
        {
            // DuckDB 命名参数使用 $ 前缀（如 $p0），与内核默认的 @ 不兼容
            return "$p";
        }

        public override object MapParameterValue(object value)
        {
            // DuckDB 原生支持 bool 类型，无需转为 1/0
            if (value is bool) return value;

            return base.MapParameterValue(value);
        }

        public override string EscapeTableName(string tableName)
        {
            return string.Format("\"{0}\"", tableName);
        }

        public override string EscapeSqlIdentifier(string str)
        {
            return string.Format("\"{0}\"", str);
        }

        public override string GetExistsSql()
        {
            return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
        }

        private void AdjustSqlInsertCommandText(DbCommand cmd, string primaryKeyName)
        {
            // DuckDB 0.7+ 支持 INSERT ... RETURNING
            cmd.CommandText += string.Format(" returning {0} as NewID", EscapeSqlIdentifier(primaryKeyName));
        }

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

        public override string GetProviderName()
        {
            return "DuckDB.NET.Data.Full";
        }
    }
}
