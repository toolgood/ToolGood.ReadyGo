using System.Data.Common;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.Poco;

namespace ToolGood.ReadyGo.Providers
{
    partial class SQLiteDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
        {
            return GetFactory("System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Culture=neutral, PublicKeyToken=db937bc2d44ff139");
        }

        public override object MapParameterValue(object value)
        {
            if (value.GetType() == typeof(uint))
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

        //public override string GetExistsSql()
        //{
        //    return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
        //}

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