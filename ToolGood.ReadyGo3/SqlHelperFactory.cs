using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
        /// <param name="server">服务器</param>
        /// <param name="database">活动数据库</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static SqlHelper OpenMysql(string server, string database, string user, string pwd)
        {
            var connstr = $"Server={server};Database={database};Uid={user};Pwd={pwd};charset=utf8;";
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
            var connstr = string.Format("Server={0};Port={4}:Database={1};Uid={2};Pwd={3};charset=utf8;", server, database, user, pwd, port);
            return OpenDatabase(connstr, "MySql.Data.MySqlClient", SqlType.MySql);
        }

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

#if !NETSTANDARD2_0
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

    /// <summary>
    /// Sqlite 
    /// </summary>
    public enum JournalMode
    {
        /// <summary>
        /// 在此模式下，每次事务终止的时候，journal文件会被删除，它会导致事务提交。
        /// </summary>
        Delete,

        /// <summary>
        /// 通过将回滚journal截短成0，而不是删除它。
        /// </summary>
        Truncate,

        /// <summary>
        /// 每次事务结束时，并不删除rollback journal，而只是在journal的头部填充0，
        /// 这样会阻止别的数据库连接来rollback. 该模式在某些平台下，是一种优化，
        /// 特别是删除或者truncate一个文件比覆盖文件的第一块代价高的时候。
        /// </summary>
        Persist,

        /// <summary>
        /// 只将rollback日志存储到RAM中，节省了磁盘I/O，但带来的代价是稳定性和完整性上的损失。
        /// 如果中间crash掉了，数据库有可能损坏。
        /// </summary>
        Memory,

        /// <summary>
        /// 也就是write-ahead　log取代rollback journal。
        /// 该模式是持久化的，跨多个数据为连接，在重新打开数据库以后，仍然有效。该模式只在3.7.0以后才有效。
        /// </summary>
        Wal,

        /// <summary>
        /// 这样就没有事务支持了
        /// </summary>
        Off
    }
}
