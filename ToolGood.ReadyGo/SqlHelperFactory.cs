using System;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace ToolGood.ReadyGo
{
    /// <summary>
    /// SqlHelper工厂
    /// </summary>
    public static class SqlHelperFactory
    {
        /// <summary>
        /// 打开数据据库
        /// </summary>
        /// <param name="connectionString">链接字符串</param>
        /// <param name="type">SqlType类型</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenDatabase(string connectionString, SqlType type = SqlType.SqlServer)
        {
            if (type == SqlType.None) {
                type = SqlType.SqlServer;
            }
            var factory = DatabaseProvider.GetProviderFactory(type);
            return new SqlHelper(connectionString, factory, type);
        }

        /// <summary>
        /// 打开数据据库
        /// </summary>
        /// <param name="connectionString">链接字符串</param>
        /// <param name="providerName">适配器名称</param>
        /// <param name="type">SqlType类型</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenDatabase(string connectionString, string providerName, SqlType type = SqlType.None)
        {
            if (type == SqlType.None) {
                if (string.IsNullOrEmpty(providerName)) {
                    throw new ArgumentException("未指定 SqlType 时，providerName 不能为空。请显式指定 SqlType 或传入有效的 providerName。");
                }
                type = DatabaseProvider.GetSqlType(providerName, connectionString);
            }
            // 优先解析 providerName 对应的驱动（如 Microsoft.Data.Sqlite），避免同 SqlType 多驱动时选错工厂
            var factory = DatabaseProvider.GetProviderFactory(providerName, type);
            return new SqlHelper(connectionString, factory, type);
        }

        /// <summary>
        /// 打开数据据库
        /// </summary>
        /// <param name="connectionString">链接字符串</param>
        /// <param name="factory">适配器工厂</param>
        /// <param name="type">SqlType类型</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenDatabase(string connectionString, DbProviderFactory factory, SqlType type = SqlType.None)
        {
            return new SqlHelper(connectionString, factory, type);
        }

        /// <summary>
        /// 打开Sql Server本地数据库
        /// </summary>
        /// <param name="filePath">数据库文件路径</param>
        /// <param name="database">数据库名</param>
        /// <param name="server">服务器实例</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenSqlServerFile(string filePath, string database, string server = "(LocalDB)\\MSSQLLocalDB")
        {
            // 注意：C# 字符串中 \\ 转义为 \，原默认值 "(LocalDb)\v11.0" 中的 \v 会被解释为垂直制表符
            var connstr = string.Format(@"Data Source={0};Initial Catalog={2};Integrated Security=SSPI;AttachDBFilename={1}", server, filePath, database);
            return OpenDatabase(connstr, "System.Data.SqlClient", SqlType.SqlServer);
        }

        /// <summary>
        /// 打开Sql Server数据库
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="database">活动数据库</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <param name="trustServerCertificate">是否信任服务器证书</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenSqlServer(string server, string database, string user, string pwd, bool trustServerCertificate = false)
        {
            var connstr = $"Server={server};Database={database};Uid={user};Pwd={pwd}";
            if (trustServerCertificate) {
                connstr += ";TrustServerCertificate=True";
            }
            return OpenDatabase(connstr, "System.Data.SqlClient", SqlType.SqlServer);
        }

        /// <summary>
        /// 打开Sql Server数据库
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="port">端口号</param>
        /// <param name="database">活动数据库</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <param name="trustServerCertificate">是否信任服务器证书</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenSqlServer(string server, int port, string database, string user, string pwd, bool trustServerCertificate = false)
        {
            var connstr = $"Server={server},{port};Database={database};Uid={user};Pwd={pwd}";
            if (trustServerCertificate) {
                connstr += ";TrustServerCertificate=True";
            }
            return OpenDatabase(connstr, "System.Data.SqlClient", SqlType.SqlServer);
        }

        /// <summary>
        /// 开Sql Server2012数据库
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="database">活动数据库</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <param name="trustServerCertificate">是否信任服务器证书</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenSqlServer2012(string server, string database, string user, string pwd, bool trustServerCertificate = false)
        {
            var connstr = $"Server={server};Database={database};Uid={user};Pwd={pwd}";
            if (trustServerCertificate) {
                connstr += ";TrustServerCertificate=True";
            }
            return OpenDatabase(connstr, "System.Data.SqlClient", SqlType.SqlServer2012);
        }

        /// <summary>
        /// 打开Sql Server2012数据库
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="port">端口号</param>
        /// <param name="database">活动数据库</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <param name="trustServerCertificate">是否信任服务器证书</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenSqlServer2012(string server, int port, string database, string user, string pwd, bool trustServerCertificate = false)
        {
            var connstr = $"Server={server},{port};Database={database};Uid={user};Pwd={pwd}";
            if (trustServerCertificate) {
                connstr += ";TrustServerCertificate=True";
            }
            return OpenDatabase(connstr, "System.Data.SqlClient", SqlType.SqlServer2012);
        }

        /// <summary>
        /// 打开Mysql数据库,SslMode默认Disabled
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="database">活动数据库</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenMysql(string server, string database, string user, string pwd)
        {
            var factory = DatabaseProvider.GetProviderFactory(SqlType.MySql);
            var isMySqlData = IsMySqlDataDriver(factory);
            // MySql.Data 使用 charset/AllowUserVariables 关键字，MySqlConnector 使用 CharSet（默认允许用户变量）
            var connstr = isMySqlData
                ? $"Server={server};Database={database};Uid={user};Pwd={pwd};charset=utf8mb4;AllowUserVariables=true;"
                : $"Server={server};Database={database};Uid={user};Pwd={pwd};CharSet=utf8mb4;";
            var options = GetMySqlConnectionOptions(factory.GetType().Assembly.GetName());
            if (options != null) {
                connstr += options;
            }
            return OpenDatabase(connstr, isMySqlData ? "MySql.Data.MySqlClient" : "MySqlConnector", SqlType.MySql);
        }

        /// <summary>
        /// 打开Mysql数据库,SslMode默认Disabled
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="port">端口号</param>
        /// <param name="database">活动数据库</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenMysql(string server, int port, string database, string user, string pwd)
        {
            var factory = DatabaseProvider.GetProviderFactory(SqlType.MySql);
            var isMySqlData = IsMySqlDataDriver(factory);
            // MySql.Data 使用 charset/AllowUserVariables 关键字，MySqlConnector 使用 CharSet（默认允许用户变量）
            var connstr = isMySqlData
                ? $"Server={server};Port={port};Database={database};Uid={user};Pwd={pwd};charset=utf8mb4;AllowUserVariables=true;"
                : $"Server={server};Port={port};Database={database};Uid={user};Pwd={pwd};CharSet=utf8mb4;";
            var options = GetMySqlConnectionOptions(factory.GetType().Assembly.GetName());
            if (options != null) {
                connstr += options;
            }
            return OpenDatabase(connstr, isMySqlData ? "MySql.Data.MySqlClient" : "MySqlConnector", SqlType.MySql);
        }

        /// <summary>
        /// 判断是否为 MySql.Data 驱动（否则视为 MySqlConnector 等兼容驱动）
        /// </summary>
        private static bool IsMySqlDataDriver(DbProviderFactory factory)
        {
            var name = factory.GetType().Assembly.GetName().Name;
            return name != null && name.IndexOf("MySql.Data", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// 根据 MySQL 驱动程序集返回连接字符串的 SSL/公钥选项。
        /// MySql.Data：SslMode=none 自 8.0.29 弃用、9.5.0 移除；SslMode=Disabled 自 8.0.29 引入。
        /// MySqlConnector：SslMode=None 始终支持；SslMode=Disabled 自 2.1.9 引入。
        /// </summary>
        private static string GetMySqlConnectionOptions(AssemblyName assemblyName)
        {
            if (assemblyName == null || assemblyName.Version == null) return null;

            var name = assemblyName.Name;
            var version = assemblyName.Version;

            if (name != null && name.IndexOf("MySql.Data", StringComparison.OrdinalIgnoreCase) >= 0) {
                // 程序集版本为四段结构（如 8.0.29.0），用整体比较而不是 Minor
                if (version.CompareTo(new Version(8, 0, 29)) >= 0) {
                    return "SslMode=Disabled;AllowPublicKeyRetrieval=true;";
                }
                if (version.Major >= 8) {
                    return "SslMode=None;AllowPublicKeyRetrieval=true;";
                }
                return null;
            }

            // MySqlConnector 等其他 MySQL 兼容驱动（版本号为 2.x/3.x）
            // 注意：MySqlConnector 解析连接串是严格的，不认识的 AllowPublicKeyRetrieval 关键字会抛异常，
            // 且其内部默认处理公钥检索，故仅返回 SslMode。
            return version.CompareTo(new Version(2, 1, 9)) >= 0
                ? "SslMode=Disabled;"
                : "SslMode=None;";
        }

        /// <summary>
        /// 打开Oracle数据库
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="port">端口号</param>
        /// <param name="serviceName">服务名</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenOracle(string server, int port, string serviceName, string user, string pwd)
        {
            var conn = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={server})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={serviceName})));User Id={user};Password={pwd}";
            return SqlHelperFactory.OpenDatabase(conn, SqlType.Oracle);
        }

        /// <summary>
        /// 打开Sqlite数据库 使用System.Data.SQLite类库
        /// </summary>
        /// <param name="filePath">文件目录</param>
        /// <param name="pwd">密码, 新版本dll不支持密码</param>
        /// <param name="useSynchronous">使用同步，为False则更快</param>
        /// <param name="journalMode">Journal模式</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenSqliteFile(string filePath, string pwd = null, bool useSynchronous = true, JournalMode journalMode = JournalMode.None)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Data Source={0};", filePath);
            sb.AppendFormat("Pooling=False;"); // 关闭连接池，避免 System.Data.SQLite 复用连接导致状态残留/内存问题

            if (useSynchronous == false) {
                sb.Append("synchronous=OFF;");
            }
            if (journalMode != JournalMode.None) {
                sb.AppendFormat("Journal Mode={0};", journalMode.ToString());
            }
            var helper = OpenDatabase(sb.ToString(), "System.Data.SQLite", SqlType.SQLite);

            // 新版sqlite不支持password参数，解决方案： https://stackoverflow.com/questions/37860933/cannot-provide-password-in-connection-string-for-sqlite
            // PRAGMA key 不支持参数化，必须对密码做转义，防止密码含单引号/反斜杠导致 SQL 语法错误或注入
            if (string.IsNullOrEmpty(pwd) == false) {
                helper.Execute($"PRAGMA key = '{SqlUtil.ToEscapeParam(pwd)}';");
            }
            return helper;
        }

        /// <summary>
        /// 打开微软的Sqlite，支持密码
        /// </summary>
        /// <param name="filePath">数据库文件路径</param>
        /// <param name="pwd">密码</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenMsSqliteFile(string filePath, string pwd = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Data Source={0};Pooling=False;", filePath);//Microsoft.Data.Sqlite的连接池有问题，防止内存爆涨，默认关闭连接池
            if (string.IsNullOrEmpty(pwd) == false) {
                // 值用双引号包裹并转义，防止密码含 ; = " 等字符破坏连接字符串
                sb.Append("Mode=ReadWrite;Password=\"").Append(EscapeConnectionValue(pwd)).Append("\";");
            }
            return OpenDatabase(sb.ToString(), "Microsoft.Data.Sqlite", SqlType.SQLite);
        }

		/// <summary>
		/// 打开微软的Sqlite
		/// </summary>
		/// <returns>打开的 SqlHelper 实例</returns>
		public static SqlHelper OpenSqliteMemory()
		{
			// :memory: 是每连接独立的内存库；关闭连接池避免复用已释放的库，行为更可预期
			return OpenDatabase("Data Source=:memory:;Pooling=False;", SqlType.SQLite);
		}

		/// <summary>
		/// 打开DuckDB数据库，支持密码
		/// </summary>
		/// <param name="filePath">数据库文件路径</param>
		/// <param name="pwd">密码</param>
		/// <returns>打开的 SqlHelper 实例</returns>
		public static SqlHelper OpenDuckDbFile(string filePath, string pwd = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Data Source={0};", filePath);
            if (string.IsNullOrEmpty(pwd) == false) {
                // 值用双引号包裹并转义，防止密码含 ; = " 等字符破坏连接字符串
                sb.Append("Mode=ReadWrite;Password=\"").Append(EscapeConnectionValue(pwd)).Append("\";");
            }
            return OpenDatabase(sb.ToString(), "DuckDB.NET.Data.Full", SqlType.DuckDb);
        }

        /// <summary>
        /// 连接字符串值转义：值内双引号翻倍后再整体用双引号包裹，
        /// 防止值中含 ; = " 等字符破坏连接字符串（.NET DbConnectionStringBuilder 系列均支持该语法）
        /// </summary>
        private static string EscapeConnectionValue(string value)
        {
            return value.Replace("\"", "\"\"");
        }

        /// <summary>
        /// 打开Access数据库 32位
        /// </summary>
        /// <param name="filePath">文件目录</param>
        /// <param name="pwd">密码</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenAccessFile(string filePath, string pwd = null)
        {
            var connstr = $"Provider=Microsoft.Jet.Oledb.4.0;data source={filePath};";
            if (string.IsNullOrEmpty(pwd) == false) {
                // 密码转义后引号包裹，防止含 ; = " 等字符破坏连接字符串
                connstr = connstr + "Database Password=\"" + EscapeConnectionValue(pwd) + "\";";
            }
            return OpenDatabase(connstr, "System.Data.OleDb", SqlType.MsAccessDb);
        }

        /// <summary>
        /// 打开Access数据库 64位
        /// </summary>
        /// <param name="filePath">文件目录</param>
        /// <param name="pwd">密码</param>
        /// <returns>打开的 SqlHelper 实例</returns>
        public static SqlHelper OpenAccessFile64x(string filePath, string pwd = null)
        {
            var connstr = $"Provider=Microsoft.ACE.OLEDB.12.0;data source={filePath};";
            if (string.IsNullOrEmpty(pwd) == false) {
                // 密码转义后引号包裹，防止含 ; = " 等字符破坏连接字符串
                connstr = connstr + "Password=\"" + EscapeConnectionValue(pwd) + "\";";
            }
            return OpenDatabase(connstr, "System.Data.OleDb", SqlType.MsAccessDb);
        }
    }
}
