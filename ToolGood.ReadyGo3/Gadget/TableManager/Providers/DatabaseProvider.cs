using System;
using ToolGood.ReadyGo3.Gadget.Internals;
using ToolGood.ReadyGo3.Gadget.TableManager.Providers;
using ToolGood.ReadyGo3.PetaPoco.Internal;

namespace ToolGood.ReadyGo3.Gadget.TableManager
{
    public abstract class DatabaseProvider
    {
        public virtual string GetTryCreateTable(Type type)
        {
            return null;
        }

        public virtual string GetCreateTable(Type type)
        {
            return null;

        }

        public virtual string GetDropTable(Type type)
        {
            return null;

        }

        public virtual string GetTruncateTable(Type type)
        {
            return null;

        }



        protected virtual string EscapeSqlIdentifier(string name)
        {
            return "[" + name + "]";
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string GetTableName(TableInfo data, TableNameManager manager = null)
        {
            var ti = data;
            var databaseName = ti.DatabaseName;
            var schemaName = ti.SchemaName;
            var tableName = ti.TableName;
            if (manager != null && manager.IsUsed) {
                var setting = manager.TryGetSetting(ti.SettingName);
                if (string.IsNullOrEmpty(databaseName)) {
                    if (string.IsNullOrEmpty(setting.DatabaseNameNullText) == false) {
                        databaseName = setting.DatabaseNameNullText;
                    }
                } else {
                    if (string.IsNullOrEmpty(setting.DatabaseNamePrefixText) == false) {
                        databaseName = setting.DatabaseNamePrefixText + databaseName;
                    }
                    if (string.IsNullOrEmpty(setting.DatabaseNameSuffixText) == false) {
                        databaseName = databaseName + setting.DatabaseNameNullText;
                    }
                }

                if (string.IsNullOrEmpty(schemaName)) {
                    if (string.IsNullOrEmpty(setting.SchemaNameNullText) == false) {
                        schemaName = setting.SchemaNameNullText;
                    }
                } else {
                    if (string.IsNullOrEmpty(setting.SchemaNamePrefixText) == false) {
                        schemaName = setting.SchemaNamePrefixText + schemaName;
                    }
                    if (string.IsNullOrEmpty(setting.SchemaNameSuffixText) == false) {
                        schemaName = schemaName + setting.SchemaNameSuffixText;
                    }
                }

                if (string.IsNullOrEmpty(setting.TableNamePrefixText) == false) {
                    tableName = setting.TableNamePrefixText + tableName;
                }
                if (string.IsNullOrEmpty(setting.TableNameSuffixText) == false) {
                    tableName = tableName + setting.TableNameSuffixText;
                }
            }
            return GetTableName(databaseName, schemaName, tableName);
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual string GetTableName(string databaseName, string schemaName, string tableName)
        {
            if (string.IsNullOrEmpty(databaseName) == false) {
                if (string.IsNullOrEmpty(schemaName) == false) {
                    return $"[{databaseName}].[{schemaName}].[{tableName}]";
                }
                return $"[{databaseName}].[dbo].[{tableName}]";
            }
            if (string.IsNullOrEmpty(schemaName) == false) {
                return $"[{schemaName}].[{tableName}]";
            }
            return $"[{tableName}]";
        }


        internal static DatabaseProvider Resolve(SqlType type)
        {
            switch (type) {
                case SqlType.SqlServer: return Singleton<SqlServerDatabaseProvider>.Instance;
                case SqlType.MySql: return Singleton<MySqlDatabaseProvider>.Instance;
                case SqlType.SQLite: return Singleton<SQLiteDatabaseProvider>.Instance;
                //case SqlType.MsAccessDb: return Singleton<MsAccessDbDatabaseProvider>.Instance;
                //case SqlType.Oracle: return Singleton<OracleDatabaseProvider>.Instance;
                //case SqlType.PostgreSQL: return Singleton<PostgreSQLDatabaseProvider>.Instance;
                //case SqlType.FirebirdDb: return Singleton<FirebirdDbDatabaseProvider>.Instance;
                case SqlType.MariaDb: return Singleton<MariaDbDatabaseProvider>.Instance;
                //case SqlType.SqlServerCE: return Singleton<SqlServerCEDatabaseProviders>.Instance;
                case SqlType.SqlServer2012: return Singleton<SqlServer2012DatabaseProvider>.Instance;
                default: break;
            }
            throw new NotSupportedException();
            //return Singleton<SqlServerDatabaseProvider>.Instance;
        }

    }
}
