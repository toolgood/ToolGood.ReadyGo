//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using ToolGood.ReadyGo3.PetaPoco.Internal;
//using System.IO;

//namespace ToolGood.ReadyGo3
//{
//#if NETSTANDARD2_0

//    public static class DbProviderFactories
//    {
//        private static Dictionary<SqlType, DbProviderFactory> _dictionary = new Dictionary<SqlType, DbProviderFactory>();
//        private static object _lock = new object();


//        public static void RegisterProviderFactory(SqlType type, DbProviderFactory factory)
//        {
//            _dictionary[type] = factory;
//        }

//        /// <summary>
//        /// 通过在appsettings.json文件中配置 "providerName",来创建对应的数据库链接
//        /// </summary>
//        /// <param name="providerInvariantName">例如：MySql.Data.MySqlClient</param>
//        /// <returns>DbProviderFactory</returns>
//        public static DbProviderFactory GetFactory(string providerInvariantName)
//        {
//            if (string.IsNullOrEmpty(providerInvariantName)) throw new Exception("数据库链接字符串配置不正确！");

//            var sqlType = GetSqlType(providerInvariantName);
//            if (_dictionary.TryGetValue(sqlType, out DbProviderFactory factory)) { return factory; }

//            lock (_lock) {
//                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                


//                var folder = Path.GetDirectoryName(typeof(DbProviderFactories).Assembly.Location);




//            }










//            if (sqlType == SqlType.SqlServer) {
//                return SqlClientFactory.Instance;
//            }
//            throw new Exception("暂不支持您使用的数据库类型！");

//        }

//        private static SqlType GetSqlType(string providerNameOrTypeName)
//        {
//            if (providerNameOrTypeName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.MySql;
//            if (providerNameOrTypeName.IndexOf("MariaDb", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.MariaDb;
//            if (providerNameOrTypeName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
//                providerNameOrTypeName.IndexOf("SqlCeConnection", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
//                providerNameOrTypeName.IndexOf("SqlCe", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.SqlServerCE;
//            if (providerNameOrTypeName.IndexOf("Npgsql", StringComparison.InvariantCultureIgnoreCase) >= 0
//                || providerNameOrTypeName.IndexOf("pgsql", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.PostgreSQL;
//            if (providerNameOrTypeName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.Oracle;
//            if (providerNameOrTypeName.IndexOf("SQLite", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.SQLite;
//            if (providerNameOrTypeName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.Oracle;
//            if (providerNameOrTypeName.IndexOf("Firebird", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
//                providerNameOrTypeName.IndexOf("FbConnection", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.FirebirdDb;
//            if (providerNameOrTypeName.StartsWith("FbConnection") || providerNameOrTypeName.EndsWith("FirebirdClientFactory")) return SqlType.FirebirdDb;


//            if (providerNameOrTypeName.IndexOf("SqlServer", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
//                providerNameOrTypeName.IndexOf("System.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase) >= 0)
//                return SqlType.SqlServer;
//            if (providerNameOrTypeName.Equals("SqlConnection") || providerNameOrTypeName.Equals("SqlClientFactory")) return SqlType.SqlServer;

//            // Assume SQL Server
//            return SqlType.SqlServer;
//        }

//    }

//#endif
//}
