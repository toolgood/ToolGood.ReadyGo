using System;
using System.Collections.Generic;
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
        /// <param name="assemblyQualifiedNames">候选 DbProviderFactory 程序集限定名集合</param>
        /// <returns>匹配到的 DbProviderFactory 实例</returns>
        public static DbProviderFactory GetFactory(params string[] assemblyQualifiedNames)
        {
            Type ft = null;
            foreach (var assemblyName in assemblyQualifiedNames) {
                ft = Type.GetType(assemblyName);
                if (ft != null) break;
            }

            if (ft == null) throw new ArgumentException("Could not load the DbProviderFactory.");

            return (DbProviderFactory)ft.GetField("Instance").GetValue(null);
        }

        /// <summary>
        /// 获取 NPoco 内核数据库类型
        /// </summary>
        /// <param name="sqlType">SQL 类型</param>
        /// <returns>对应的 NPoco 数据库类型</returns>
        public static ToolGood.ReadyGo.NPoco.DatabaseType GetDatabaseType(SqlType sqlType)
        {
            switch (sqlType) {
                case SqlType.SQLite: return ToolGood.ReadyGo.NPoco.DatabaseType.SQLite;
                case SqlType.SqlServer: return ToolGood.ReadyGo.NPoco.DatabaseType.SqlServer2012;
                case SqlType.MySql:
                case SqlType.MariaDb: return ToolGood.ReadyGo.NPoco.DatabaseType.MySQL;
                case SqlType.Oracle: return ToolGood.ReadyGo.NPoco.DatabaseType.Oracle;
                case SqlType.PostgreSQL: return ToolGood.ReadyGo.NPoco.DatabaseType.PostgreSQL;
                case SqlType.FirebirdDb: return ToolGood.ReadyGo.NPoco.DatabaseType.Firebird;
                case SqlType.DuckDb:
                    return ToolGood.ReadyGo.NPoco.DatabaseType.DuckDb;
                case SqlType.MsAccessDb:
                    return ToolGood.ReadyGo.NPoco.DatabaseType.MsAccessDb;
                default:
                    throw new DatabaseUnsupportException($"未知的数据库类型: {sqlType}。");
            }
        }

        /// <summary>
        /// 获取 DbProviderFactory
        /// </summary>
        /// <param name="sqlType">SQL 类型</param>
        /// <returns>对应的 DbProviderFactory 实例</returns>
        public static DbProviderFactory GetProviderFactory(SqlType sqlType)
        {
            return GetFactory(GetFactoryCandidates(sqlType));
        }

        /// <summary>
        /// 获取 DbProviderFactory，并优先使用与 providerName 匹配的驱动。
        /// 例如 SqlType.SQLite 同时支持 System.Data.SQLite 与 Microsoft.Data.Sqlite，
        /// 传 providerName="Microsoft.Data.Sqlite" 时优先解析该驱动，失败后再回退其他候选。
        /// </summary>
        /// <param name="providerName">提供程序名（可为 DbProviderFactory 类型名或程序集名，如 MySql.Data.MySqlClient）</param>
        /// <param name="sqlType">SQL 类型</param>
        /// <returns>对应的 DbProviderFactory 实例</returns>
        public static DbProviderFactory GetProviderFactory(string providerName, SqlType sqlType)
        {
            var candidates = GetFactoryCandidates(sqlType);
            if (string.IsNullOrEmpty(providerName) == false && candidates.Length > 1) {
                var list = new List<string>(candidates);
                for (int i = 0; i < candidates.Length; i++) {
                    var typeShortName = candidates[i].Split(',')[0].Trim();
                    if (candidates[i].IndexOf(providerName, StringComparison.OrdinalIgnoreCase) >= 0
                        || providerName.IndexOf(typeShortName, StringComparison.OrdinalIgnoreCase) >= 0) {
                        if (i > 0) {
                            list.RemoveAt(i);
                            list.Insert(0, candidates[i]);
                        }
                        break;
                    }
                }
                return GetFactory(list.ToArray());
            }
            return GetFactory(candidates);
        }

        /// <summary>
        /// 获取各 SqlType 对应的 DbProviderFactory 候选程序集限定名列表（按优先级排列）
        /// </summary>
        private static string[] GetFactoryCandidates(SqlType sqlType)
        {
            switch (sqlType) {
                case SqlType.SqlServer:
                    return new[] {
                        "System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient",
                        "System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                        "System.Data.SqlClient.SqlClientFactory, System.Data",
                        "Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient"
                        };
                case SqlType.MySql:
                case SqlType.MariaDb:
                    return new[] {
                        "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Culture=neutral, PublicKeyToken=c5687fc88969c44d",
                        "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data",
                        "MySqlConnector.MySqlConnectorFactory, MySqlConnector"
                        };
                case SqlType.SQLite:
                    return new[] {
                        "System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Culture=neutral, PublicKeyToken=db937bc2d44ff139",
                        "System.Data.SQLite.SQLiteFactory, System.Data.SQLite",
                        "Microsoft.Data.Sqlite.SqliteFactory, Microsoft.Data.Sqlite, Culture=neutral, PublicKeyToken=adb9793829ddae60",
                        "Microsoft.Data.Sqlite.SqliteFactory, Microsoft.Data.Sqlite"
                        };
                case SqlType.PostgreSQL:
                    return new[] {
                        "Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7",
                        "Npgsql.NpgsqlFactory, Npgsql"
                        };
                case SqlType.Oracle:
                    return new[] {
                        "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Culture=neutral, PublicKeyToken=89b483f429c47342",
                        "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess",
                        "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess, Culture=neutral, PublicKeyToken=89b483f429c47342",
                        "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess",
                        "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                        "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                        "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient"
                        };
                case SqlType.FirebirdDb:
                    return new[] {
                        "FirebirdSql.Data.FirebirdClient.FirebirdClientFactory, FirebirdSql.Data.FirebirdClient, Culture=neutral, PublicKeyToken=3750abcc3150b00c",
                        "FirebirdSql.Data.FirebirdClient.FirebirdClientFactory, FirebirdSql.Data.FirebirdClient"
                        };
                case SqlType.MsAccessDb:
                    return new[] {
                        "System.Data.OleDb.OleDbFactory, System.Data.OleDb, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51",
                        "System.Data.OleDb.OleDbFactory, System.Data.OleDb"
                        };
                case SqlType.DuckDb:
                    return new[] {
                        "DuckDB.NET.Data.DuckDBFactory, DuckDB.NET.Data.Full",
                        "DuckDB.NET.Data.DuckDBFactory, DuckDB.NET.Data"
                        };
                default:
                    throw new DatabaseUnsupportException($"未知的数据库类型: {sqlType}。");
            }
        }

        /// <summary>
        /// 根据提供程序名/类型名解析 SqlType
        /// </summary>
        /// <param name="providerNameOrTypeName">提供程序名或类型名</param>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>解析出的 SqlType</returns>
        public static SqlType GetSqlType(string providerNameOrTypeName, string connectionString)
        {
            if (providerNameOrTypeName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.MySql;
            if (providerNameOrTypeName.IndexOf("MariaDb", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.MariaDb;
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
