using System;
using System.Data;
using System.Data.Common;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.SqlBuilding;

namespace ToolGood.ReadyGo.Providers
{
    internal class MsAccessDbDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
        {
            return DbProviderFactories.GetFactory("System.Data.OleDb");
        }

        public override object ExecuteInsert(Database database, IDbCommand cmd, string primaryKeyName)
        {
            database.ExecuteNonQueryHelper(cmd);
            cmd.CommandText = "SELECT @@IDENTITY AS NewID;";
            return database.ExecuteScalarHelper(cmd);
        }

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            throw new NotSupportedException("The Access provider does not support paging.");
        }

        public override string GetTableName(PocoData pd, TableNameManger tn)
        {
            var ti = pd.TableInfo;
            if (ti.TableName.Contains(".")) {
                return ti.TableName;
            }
            var tag = ti.FixTag;
            var tableName = ti.TableName;

            var name = tn.Get(tag);
            if (name != null) {
                tableName = name.TablePrefix + tableName + name.TableSuffix;
            }
            var schemaName = ti.SchemaName;

            if (string.IsNullOrEmpty(schemaName)) {
                return this.EscapeSqlIdentifier(tableName);
            }
            return string.Format("[{0}_{1}]", schemaName, tableName);
        }
    }
}