using System.Data.Common;
using System.Text;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.PetaPoco.Providers
{
    public class PostgreSQLDatabaseProvider : DatabaseProvider
    {
        public override bool HasNativeGuidSupport
        {
            get { return true; }
        }

        public override DbProviderFactory GetFactory()
        {
            return GetFactory(
                //"Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7",
                "Npgsql.NpgsqlFactory, Npgsql"
                );
        }

        public override string GetExistsSql()
        {
            return "SELECT CASE WHEN EXISTS(SELECT 1 FROM {0} WHERE {1}) THEN 1 ELSE 0 END";
        }

        public override object MapParameterValue(object value)
        {
            // Don't map bools to ints in PostgreSQL
            if (value is bool)
                return value;

            return base.MapParameterValue(value);
        }

        public override string EscapeSqlIdentifier(string sqlIdentifier)
        {
            return $"\"{sqlIdentifier}\"";
        }
        public override string GetTableName(string databaseName, string schemaName, string tableName)
        {
            if (string.IsNullOrEmpty(databaseName) == false) {
                if (string.IsNullOrEmpty(schemaName) == false) {
                    return $"\"{databaseName}\".\"{schemaName}\".\"{tableName}\"";
                }
                return $"\"{databaseName}\".\"{tableName}\"";
            }
            if (string.IsNullOrEmpty(schemaName) == false) {
                return $"\"{schemaName}\".\"{tableName}\"";
            }
            return $"\"{tableName}\"";
        }

        public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string primaryKeyName)
        {
            if (primaryKeyName != null)
            {
                cmd.CommandText += $"returning {EscapeSqlIdentifier(primaryKeyName)} as NewID";
                return db.ExecuteScalarHelper(cmd);
            }
            else
            {
                db.ExecuteNonQueryHelper(cmd);
                return -1;
            }
        }
        public override string CreateSql(int limit, int offset, string columnSql, string fromtable, string order, string where)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            sb.Append(columnSql);
            sb.Append(" FROM ");
            sb.Append(fromtable);
            if (string.IsNullOrEmpty(where) == false) {
                sb.Append(" WHERE ");
                sb.Append(where);
            }
            if (string.IsNullOrEmpty(order) == false) {
                sb.Append(" ORDER BY ");
                sb.Append(order);
            }
            if (limit > 0) {
                sb.Append(" LIMIT ");
                if (offset > 0) {
                    sb.Append(offset);
                    sb.Append(",");
                }
                sb.Append(limit);
            }
            return sb.ToString();
        }
    }
}