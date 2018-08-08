using System.Configuration;
using System.Data.Common;
using System.Text;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// SqlHelper工厂
    /// </summary>
    public static class SqlHelperFactory
    {
#if !NETSTANDARD2_0
        /// <summary>
        /// 根据config配置名打开数据据库
        /// </summary>
        /// <param name="name">配置名</param>
        /// <param name="type">SqlType</param>
        /// <returns></returns>
        public static SqlHelper OpenFormConnStr(string name, SqlType type = SqlType.None)
        {

            var c = ConfigurationManager.ConnectionStrings[name];
            return OpenDatabase(c.ConnectionString, c.ProviderName, type);
        }
#endif
        /// <summary>
        /// 打开数据据库
        /// </summary>
        /// <param name="connectionString">链接字符串</param>
        /// <param name="providerName">适配器名称</param>
        /// <param name="type">SqlType类型</param>
        /// <returns></returns>
        public static SqlHelper OpenDatabase(string connectionString, string providerName, SqlType type = SqlType.None)
        {
            if (type == SqlType.None) {
                type = DatabaseProvider.GetSqlType(providerName, connectionString);
            }
            var factory = DatabaseProvider.Resolve(type).GetFactory();
            return new SqlHelper(connectionString, factory, type);
        }

        /// <summary>
        /// 打开数据据库
        /// </summary>
        /// <param name="connectionString">链接字符串</param>
        /// <param name="factory">适配器工厂</param>
        /// <param name="type">SqlType类型</param>
        /// <returns></returns>
        public static SqlHelper OpenDatabase(string connectionString, DbProviderFactory factory, SqlType type = SqlType.None)
        {
            return new SqlHelper(connectionString, factory, type);
        }

        /// <summary>
        /// 打开Sql Server本地数据库
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="database"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public static SqlHelper OpenSqlServerFile(string filePath, string database, string server = "(LocalDb)\v11.0")
        {
            var connstr = string.Format(@"Data Source={0};Initial Catalog={2};Integrated Security=SSPI;AttachDBFilename={1}", server, filePath, database);
            return OpenDatabase(connstr, "System.Data.SqlClient", SqlType.SqlServer);
        }

        /// <summary>
        /// 打开Sql Server数据库
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static SqlHelper OpenSqlServer(SqlServerConnectionString connectionString)
        {
            var connstr = connectionString.ToString();
            return OpenDatabase(connstr, "System.Data.SqlClient", SqlType.SqlServer);
        }

        /// <summary>
        /// 打开Sql Server数据库
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="database">活动数据库</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static SqlHelper OpenSqlServer(string server, string database, string user, string pwd)
        {
            var connstr = $"Server={server};Database={database};Uid={user};Pwd={pwd}";
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
        /// <returns></returns>
        public static SqlHelper OpenSqlServer(string server, int port, string database, string user, string pwd)
        {
            var connstr = $"Server={server};Port={port}:Database={database};Uid={user};Pwd={pwd}";
            return OpenDatabase(connstr, "System.Data.SqlClient", SqlType.SqlServer);
        }

        /// <summary>
        /// 开Sql Server2012数据库
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static SqlHelper OpenSqlServer2012(SqlServerConnectionString connectionString)
        {
            var connstr = connectionString.ToString();
            return OpenDatabase(connstr, "System.Data.SqlClient", SqlType.SqlServer2012);
        }

        /// <summary>
        /// 打开Sql Server2012数据库
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="database">活动数据库</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static SqlHelper OpenSqlServer2012(string server, string database, string user, string pwd)
        {
            var connstr = $"Server={server};Database={database};Uid={user};Pwd={pwd}";
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
        /// <returns></returns>
        public static SqlHelper OpenSqlServer2012(string server, int port, string database, string user, string pwd)
        {
            var connstr = $"Server={server};Port={port}:Database={database};Uid={user};Pwd={pwd}";
            return OpenDatabase(connstr, "System.Data.SqlClient", SqlType.SqlServer2012);
        }

        /// <summary>
        /// 打开Mysql数据库
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static SqlHelper OpenMysql(MysqlConnectionString connectionString)
        {
            var connstr = connectionString.ToString();
            return OpenDatabase(connstr, "MySql.Data.MySqlClient", SqlType.MySql);
        }
        /// <summary>
        /// 打开Mysql数据库
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="database">活动数据库</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static SqlHelper OpenMysql(string server, string database, string user, string pwd)
        {
            var connstr = $"Server={server};Database={database};Uid={user};Pwd={pwd};charset=utf8;Allow User Variables=True;";
            return OpenDatabase(connstr, "MySql.Data.MySqlClient", SqlType.MySql);
        }

        /// <summary>
        /// 打开Mysql数据库
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="port">端口号</param>
        /// <param name="database">活动数据库</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static SqlHelper OpenMysql(string server, int port, string database, string user, string pwd)
        {
            var connstr = $"Server={server};Port={port};Database={database};Uid={user};Pwd={pwd};charset=utf8;Allow User Variables=True;";
            return OpenDatabase(connstr, "MySql.Data.MySqlClient", SqlType.MySql);
        }

#if NETSTANDARD2_0
        /// <summary>
        /// 打开Sqlite数据库
        /// </summary>
        /// <param name="filePath">文件目录</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static SqlHelper OpenSqliteFile(string filePath, string pwd = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Data Source={0};", filePath);
            if (string.IsNullOrEmpty(pwd) == false) {
                sb.AppendFormat("Password={0};", pwd);
            }
            return OpenDatabase(sb.ToString(), "System.Data.SQLite", SqlType.SQLite);
        }
#else

        /// <summary>
        /// 打开Sqlite数据库
        /// </summary>
        /// <param name="filePath">文件目录</param>
        /// <param name="pwd">密码</param>
        /// <param name="useSynchronous">使用同步，为False则更快</param>
        /// <param name="journalMode">Journal模式</param>
        /// <returns></returns>
        public static SqlHelper OpenSqliteFile(string filePath, string pwd = "", bool useSynchronous = true, JournalMode journalMode = JournalMode.Delete)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Data Source={0};Version=3;", filePath);
            //sb.AppendFormat("Data Source={0};", filePath);
            if (string.IsNullOrEmpty(pwd) == false) {
                sb.AppendFormat("Password={0};", pwd);
            }
            if (useSynchronous == false) {
                sb.Append("synchronous=OFF;");
            }
            sb.Append("Page Size=4096;");
            sb.AppendFormat("Journal Mode={0};", journalMode.ToString());
            return OpenDatabase(sb.ToString(), "System.Data.SQLite", SqlType.SQLite);
        }


        /// <summary>
        /// 打开Access数据库 32位
        /// </summary>
        /// <param name="filePath">文件目录</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static SqlHelper OpenAccessFile(string filePath, string pwd = null)
        {
            var connstr = $"Provider=Microsoft.Jet.Oledb.4.0;data source={filePath};";
            if (string.IsNullOrEmpty(pwd) == false) {
                connstr = connstr + "Database Password=" + pwd + ";";
            }
            return OpenDatabase(connstr, "System.Data.OleDb", SqlType.MsAccessDb);
        }

        /// <summary>
        /// 打开Access数据库 64位
        /// </summary>
        /// <param name="filePath">文件目录</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static SqlHelper OpenAccessFile64x(string filePath, string pwd = null)
        {
            var connstr = $"Provider=Microsoft.ACE.OLEDB.12.0;data source={filePath};";
            if (string.IsNullOrEmpty(pwd) == false) {
                connstr = connstr + "Password=" + pwd + ";";
            }
            return OpenDatabase(connstr, "System.Data.OleDb", SqlType.MsAccessDb);
        }
#endif
    }

}
