using System;
using System.Data.Common;
using ToolGood.ReadyGo.Exceptions;

namespace ToolGood.ReadyGo
{
    /// <summary>
    /// 数据库提供程序解析（SqlType → DbProviderFactory / DatabaseType）
    /// </summary>
    internal static class DatabaseProvider
    {
        /// <summary>
        /// 根据程序集限定名反射获取 DbProviderFactory
        /// </summary>
        /// <param name="assemblyQualifiedNames"></param>
        /// <returns></returns>
        public static DbProviderFactory GetFactory(params string[] assemblyQualifiedNames)
        {
            DbProviderFactory providerFactory = null;
            if (providerFactory == null) {
                Type ft = null;
                foreach (var assemblyName in assemblyQualifiedNames) {
                    ft = Type.GetType(assemblyName);
                    if (ft != null) break;
                }

                if (ft == null) throw new ArgumentException("Could not load the DbProviderFactory.");

                providerFactory = (DbProviderFactory)ft.GetField("Instance").GetValue(null);
            }
            return providerFactory;
        }

        /// <summary>
        /// 获取 NPoco 内核数据库类型
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public static ToolGood.ReadyGo.NPoco.DatabaseType GetDatabaseType(SqlType sqlType)
        {
            switch (sqlType) {
                case SqlType.SQLite: return ToolGood.ReadyGo.NPoco.DatabaseType.SQLite;
                case SqlType.SqlServer:
                case SqlType.SqlServer2012: return ToolGood.ReadyGo.NPoco.DatabaseType.SqlServer2012;
                case SqlType.MySql:
                case SqlType.MariaDb: return ToolGood.ReadyGo.NPoco.DatabaseType.MySQL;
                case SqlType.Oracle: return ToolGood.ReadyGo.NPoco.DatabaseType.Oracle;
                case SqlType.PostgreSQL: return ToolGood.ReadyGo.NPoco.DatabaseType.PostgreSQL;
                case SqlType.FirebirdDb: return ToolGood.ReadyGo.NPoco.DatabaseType.Firebird;
                case SqlType.DuckDb:
                    // DuckDB 参数语法为 $name，与 NPoco 内核默认的 @ 前缀不兼容，需自定义 IDatabaseType 后接入
                    throw new DatabaseUnsupportException("DuckDB 暂不支持：NPoco 内核未提供 DuckDB 的 DatabaseType，且 DuckDB 参数语法($name)与内核默认参数前缀(@)不兼容。请自定义 DuckDB 的 IDatabaseType 实现后接入，或改用 SQLite。");
                case SqlType.MsAccessDb:
                    // Jet/ACE SQL 方言（TOP 分页、? 参数）与现有 DatabaseType 不兼容
                    throw new DatabaseUnsupportException("Access(MsAccessDb) 暂不支持：Jet/ACE SQL 方言与 NPoco 内核现有 DatabaseType 不兼容，需自定义 Access 的 IDatabaseType 实现后接入。");
                case SqlType.SqlServerCE:
                    throw new DatabaseUnsupportException("SqlServerCE 已停止维护，不再支持。");
                default:
                    throw new DatabaseUnsupportException($"未知的数据库类型: {sqlType}。");
            }
        }

        /// <summary>
        /// 获取 DbProviderFactory
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public static DbProviderFactory GetProviderFactory(SqlType sqlType)
        {
            switch (sqlType) {
                case SqlType.SqlServer:
                case SqlType.SqlServer2012:
                    return GetFactory(
                        "System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient",
                        "System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                        "System.Data.SqlClient.SqlClientFactory, System.Data",
                        "Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient"
                        );
                case SqlType.MySql:
                case SqlType.MariaDb:
                    return GetFactory(
                        "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Culture=neutral, PublicKeyToken=c5687fc88969c44d",
                        "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data",
                        "MySqlConnector.MySqlConnectorFactory, MySqlConnector"
                        );
                case SqlType.SQLite:
                    return GetFactory(
                        "System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Culture=neutral, PublicKeyToken=db937bc2d44ff139",
                        "System.Data.SQLite.SQLiteFactory, System.Data.SQLite",
                        "Microsoft.Data.Sqlite.SqliteFactory, Microsoft.Data.Sqlite, Culture=neutral, PublicKeyToken=adb9793829ddae60",
                        "Microsoft.Data.Sqlite.SqliteFactory, Microsoft.Data.Sqlite"
                        );
                case SqlType.PostgreSQL:
                    return GetFactory(
                        "Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7",
                        "Npgsql.NpgsqlFactory, Npgsql"
                        );
                case SqlType.Oracle:
                    return GetFactory(
                        "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Culture=neutral, PublicKeyToken=89b483f429c47342",
                        "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess",
                        "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess, Culture=neutral, PublicKeyToken=89b483f429c47342",
                        "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess",
                        "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                        "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                        "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient"
                        );
                case SqlType.FirebirdDb:
                    return GetFactory(
                        "FirebirdSql.Data.FirebirdClient.FirebirdClientFactory, FirebirdSql.Data.FirebirdClient, Culture=neutral, PublicKeyToken=3750abcc3150b00c",
                        "FirebirdSql.Data.FirebirdClient.FirebirdClientFactory, FirebirdSql.Data.FirebirdClient"
                        );
                case SqlType.MsAccessDb:
                    return GetFactory(
                        "System.Data.OleDb.OleDbFactory, System.Data.OleDb, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51",
                        "System.Data.OleDb.OleDbFactory, System.Data.OleDb"
                        );
                case SqlType.DuckDb:
                    return GetFactory(
                        "DuckDB.NET.Data.DuckDBFactory, DuckDB.NET.Data.Full",
                        "DuckDB.NET.Data.DuckDBFactory, DuckDB.NET.Data"
                        );
                case SqlType.SqlServerCE:
                    throw new DatabaseUnsupportException("SqlServerCE 已停止维护，不再支持。");
                default: throw new DatabaseUnsupportException($"未知的数据库类型: {sqlType}。");
            }
        }

        /// <summary>
        /// 根据提供程序名/类型名解析 SqlType
        /// </summary>
        /// <param name="providerNameOrTypeName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static SqlType GetSqlType(string providerNameOrTypeName, string connectionString)
        {
            if (providerNameOrTypeName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.MySql;
            if (providerNameOrTypeName.IndexOf("MariaDb", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.MariaDb;
            if (providerNameOrTypeName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerNameOrTypeName.IndexOf("SqlCeConnection", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerNameOrTypeName.IndexOf("SqlCe", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.SqlServerCE;
            if (providerNameOrTypeName.IndexOf("Npgsql", StringComparison.InvariantCultureIgnoreCase) >= 0
                || providerNameOrTypeName.IndexOf("pgsql", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.PostgreSQL;
            if (providerNameOrTypeName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.Oracle;
            if (providerNameOrTypeName.IndexOf("SQLite", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.SQLite;
            if (providerNameOrTypeName.IndexOf("DuckDb", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.DuckDb;
            if (providerNameOrTypeName.IndexOf("Firebird", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerNameOrTypeName.IndexOf("FbConnection", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.FirebirdDb;
            if (providerNameOrTypeName.StartsWith("FbConnection") || providerNameOrTypeName.EndsWith("FirebirdClientFactory")) return SqlType.FirebirdDb;

            if (providerNameOrTypeName.IndexOf("OleDb", StringComparison.InvariantCultureIgnoreCase) >= 0
                && (connectionString.IndexOf("Jet.OLEDB", StringComparison.InvariantCultureIgnoreCase) > 0
                || connectionString.IndexOf("ACE.OLEDB", StringComparison.InvariantCultureIgnoreCase) > 0)) {
                return SqlType.MsAccessDb;
            }
            if (providerNameOrTypeName.IndexOf("SqlServer", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerNameOrTypeName.IndexOf("System.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return SqlType.SqlServer;
            if (providerNameOrTypeName.Equals("SqlConnection") || providerNameOrTypeName.Equals("SqlClientFactory")) return SqlType.SqlServer;

            // 无法识别的提供程序，明确报错而不是静默假设 SqlServer，避免错误方言导致难以定位的问题
            throw new DatabaseUnsupportException($"无法识别的数据库提供程序: {providerNameOrTypeName}。请显式指定 SqlType 或使用受支持的 providerName。");
        }
    }
}
