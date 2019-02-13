using System.Data.Common;
using System.Text;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.PetaPoco.Providers
{
    public class SQLiteDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
        {
            return GetFactory(
                "System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Culture=neutral, PublicKeyToken=db937bc2d44ff139",
                "System.Data.SQLite.SQLiteFactory, System.Data.SQLite",
                "Microsoft.Data.Sqlite.SqliteFactory, Microsoft.Data.Sqlite, Culture=neutral, PublicKeyToken=adb9793829ddae60",
                "Microsoft.Data.Sqlite.SqliteFactory, Microsoft.Data.Sqlite"
                );
        }

        public override object MapParameterValue(object value)
        {
            if (value is uint)
                return (long)((uint)value);

            return base.MapParameterValue(value);
        }

        public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string primaryKeyName)
        {
            if (primaryKeyName != null) {
                cmd.CommandText += ";\nSELECT last_insert_rowid();";
                return db.ExecuteScalarHelper(cmd);
            } else {
                db.ExecuteNonQueryHelper(cmd);
                return -1;
            }
        }

        public override string GetExistsSql()
        {
            return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
        }

        public override string GetTableName(string databaseName, string schemaName, string tableName)
        {
            return $"[{tableName}]";
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