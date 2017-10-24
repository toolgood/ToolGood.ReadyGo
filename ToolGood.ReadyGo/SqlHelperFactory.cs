using System.Configuration;
using System.Text;
using ToolGood.ReadyGo.Internals;

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
        /// <param name="providerName">适配器名称</param>
        /// <param name="type">SqlType类型</param>
        /// <returns></returns>
        public static SqlHelper OpenDatabase(string connectionString, string providerName, SqlType type = SqlType.None)
        {
            return new SqlHelper(connectionString, providerName, type);
        }

        /// <summary>
        /// 根据config配置名打开数据据库
        /// </summary>
        /// <param name="name">配置名</param>
        /// <param name="type">SqlType</param>
        /// <returns></returns>
        public static SqlHelper OpenFormConnStr(string name, SqlType type= SqlType.None)
        {
            var c = ConfigurationManager.ConnectionStrings[name];
            return OpenDatabase(c.ConnectionString, c.ProviderName, type);
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
            return OpenDatabase(connstr, "System.Data.SqlClient");
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
            var connstr = string.Format("Server={0};Database={1};Uid={2};Pwd={3}", server, database, user, pwd);
            return OpenDatabase(connstr, "System.Data.SqlClient");
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
            var connstr = string.Format("Server={0};Port={4}:Database={1};Uid={2};Pwd={3}", server, database, user, pwd, port);
            return OpenDatabase(connstr, "System.Data.SqlClient");
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
            var connstr = string.Format("Server={0};Database={1};Uid={2};Pwd={3}", server, database, user, pwd);
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
            var connstr = string.Format("Server={0};Port={4}:Database={1};Uid={2};Pwd={3}", server, database, user, pwd, port);
            return OpenDatabase(connstr, "System.Data.SqlClient", SqlType.SqlServer2012);
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
            var connstr = string.Format("Server={0};Database={1};Uid={2};Pwd={3};charset=utf8;", server, database, user, pwd);
            return OpenDatabase(connstr, "MySql.Data.MySqlClient");
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
            var connstr = string.Format("Server={0};Port={4}:Database={1};Uid={2};Pwd={3};charset=utf8;", server, database, user, pwd, port);
            return OpenDatabase(connstr, "MySql.Data.MySqlClient");
        }

        /// <summary>
        /// 打开Sqlite数据库
        /// </summary>
        /// <param name="filePath">文件目录</param>
        /// <param name="pwd">密码</param>
        /// <param name="UseSynchronous">使用同步，为False则更快</param>
        /// <param name="JournalMode">Journal模式</param>
        /// <returns></returns>
        public static SqlHelper OpenSqliteFile(string filePath, string pwd = "", bool UseSynchronous = true, JournalMode JournalMode = JournalMode.DELETE)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Data Source={0};Version=3;", filePath);
            if (string.IsNullOrEmpty(pwd) == false) {
                sb.AppendFormat("Password={0};", pwd);
            }
            if (UseSynchronous == false) {
                sb.Append("synchronous=OFF;");
            }
            sb.Append("Page Size=4096;");
            sb.AppendFormat("Journal Mode={0};", JournalMode.ToString());
            return OpenDatabase(sb.ToString(), "System.Data.SQLite");
        }

        /// <summary>
        /// 打开Access数据库 32位
        /// </summary>
        /// <param name="filePath">文件目录</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static SqlHelper OpenAccessFile(string filePath, string pwd = null)
        {
            var connstr = string.Format("Provider=Microsoft.Jet.Oledb.4.0;data source={0};", filePath);
            if (string.IsNullOrEmpty(pwd) == false) {
                connstr = connstr + "Database Password=" + pwd + ";";
            }
            return OpenDatabase(connstr, "System.Data.OleDb");
        }

        /// <summary>
        /// 打开Access数据库 64位
        /// </summary>
        /// <param name="filePath">文件目录</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static SqlHelper OpenAccessFile64x(string filePath, string pwd = null)
        {
            var connstr = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;data source={0};", filePath);
            if (string.IsNullOrEmpty(pwd) == false) {
                connstr = connstr + "Password=" + pwd + ";";
            }
            return OpenDatabase(connstr, "System.Data.OleDb");
        }
    }

    /// <summary>
    /// Sqlite 
    /// </summary>
    public enum JournalMode
    {
        /// <summary>
        /// 在此模式下，每次事务终止的时候，journal文件会被删除，它会导致事务提交。
        /// </summary>
        DELETE,

        /// <summary>
        /// 通过将回滚journal截短成0，而不是删除它。
        /// </summary>
        TRUNCATE,

        /// <summary>
        /// 每次事务结束时，并不删除rollback journal，而只是在journal的头部填充0，
        /// 这样会阻止别的数据库连接来rollback. 该模式在某些平台下，是一种优化，
        /// 特别是删除或者truncate一个文件比覆盖文件的第一块代价高的时候。
        /// </summary>
        PERSIST,

        /// <summary>
        /// 只将rollback日志存储到RAM中，节省了磁盘I/O，但带来的代价是稳定性和完整性上的损失。
        /// 如果中间crash掉了，数据库有可能损坏。
        /// </summary>
        MEMORY,

        /// <summary>
        /// 也就是write-ahead　log取代rollback journal。
        /// 该模式是持久化的，跨多个数据为连接，在重新打开数据库以后，仍然有效。该模式只在3.7.0以后才有效。
        /// </summary>
        WAL,

        /// <summary>
        /// 这样就没有事务支持了
        /// </summary>
        OFF
    }
}