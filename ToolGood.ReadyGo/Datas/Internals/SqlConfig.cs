using System;
using System.Data;
using System.Data.Common;
using ToolGood.ReadyGo.Caches;
using ToolGood.ReadyGo.Events;
using ToolGood.ReadyGo.Monitor;
using ToolGood.ReadyGo.Poco;

namespace ToolGood.ReadyGo.Internals
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class SqlConfig
    {
        internal SqlHelper _sqlHelper;

        internal SqlConfig(SqlHelper helper)
        {
            _sqlHelper = helper;
        }
        #region 属性
        /// <summary>
        /// 插入操作时，默认时间为现在时间
        /// </summary>
        public bool Insert_DateTime_Default_Now { get { return _sqlHelper._setDateTimeDefaultNow; } set { _sqlHelper._setDateTimeDefaultNow = value; } }
        /// <summary>
        /// 插入操作时，默认字符串非Null
        /// </summary>
        public bool Insert_String_Default_NotNull { get { return _sqlHelper._setStringDefaultNotNull; } set { _sqlHelper._setStringDefaultNotNull = value; } }

        /// <summary>
        /// 插入操作时，默认Grid自动生成
        /// </summary>
        public bool Insert_Guid_Default_New { get { return _sqlHelper._setGuidDefaultNew; } set { _sqlHelper._setGuidDefaultNew = value; } }


        /// <summary>
        /// 表名修饰助手
        /// </summary>
        public TableNameManger TableNameManger { get { return _sqlHelper._tableNameManger; } }

        /// <summary>
        /// 事件
        /// </summary>
        public SqlEvents Events { get { return _sqlHelper._events; } }

        /// <summary>
        /// SQL执行监控
        /// </summary>
        public ISqlMonitor SqlMonitor
        {
            get { return _sqlHelper._sqlMonitor; }
            set
            {
                if (_sqlHelper._sqlMonitor != value) {
                    if (_sqlHelper._writeDatabase != null) {
                        _sqlHelper._writeDatabase.Dispose();
                        _sqlHelper._writeDatabase = null;
                    }
                    if (_sqlHelper._readDatabase != null) {
                        _sqlHelper._readDatabase.Dispose();
                        _sqlHelper._readDatabase = null;
                    }
                    _sqlHelper._sqlMonitor = value;
                }
            }
        }
        /// <summary>
        /// 连接类型 读写类型
        /// </summary>
        public ConnectionType ConnectionType { get { return _sqlHelper._connectionType; } }
        /// <summary>
        /// SQL语言类型
        /// </summary>
        public SqlType SqlType { get { return _sqlHelper._sqlType; } }
        /// <summary>
        /// DbProviderFactory
        /// </summary>
        public DbProviderFactory ProviderFactory { get { return _sqlHelper._factory; } }
        /// <summary>
        /// 数据库链接字符串【写】
        /// </summary>
        public string WriteConnectionString { get { return _sqlHelper._writeConnectionString; } }
        /// <summary>
        /// 数据库链接字符串【读】
        /// </summary>
        public string ReadConnectionString { get { return _sqlHelper._readConnectionString; } }

        /// <summary>
        /// 数据库执行超出时间
        /// </summary>
        public int CommandTimeout { get { return _sqlHelper._commandTimeout; } set { _sqlHelper._commandTimeout = value; } }
        /// <summary>
        /// 当前这次 数据库执行超出时间
        /// </summary>
        public int CommandTimeoutOnce { get { return _sqlHelper._oneTimeCommandTimeout; } set { _sqlHelper._oneTimeCommandTimeout = value; } }
        /// <summary>
        /// 事务级别
        /// </summary>
        public IsolationLevel? IsolationLevel { get { return _sqlHelper._isolationLevel; } set { _sqlHelper._isolationLevel = value; } }

        /// <summary>
        /// 缓存服务
        /// </summary>
        public ICacheService CacheService { get { return _sqlHelper._cacheService; } set { _sqlHelper._cacheService = value; } }
        /// <summary>
        /// 是否使用缓存
        /// </summary>
        public bool UsedCacheService { get { return _sqlHelper._usedCacheService; } set { _sqlHelper._usedCacheService = value; } }
        /// <summary>
        /// 是否使用缓存  当前这次
        /// </summary>
        public bool UsedCacheServiceOnce { get { return _sqlHelper._usedCacheServiceOnce; } set { _sqlHelper._usedCacheServiceOnce = value; } }

        /// <summary>
        /// 缓存时间
        /// </summary>
        public int CacheTime { get { return _sqlHelper._cacheTime; } set { _sqlHelper._cacheTime = value; } }

        /// <summary>
        /// 缓存时间 当前这次
        /// </summary>
        public int CacheTimeOnce { get { return _sqlHelper._cacheTimeOnce; } set { _sqlHelper._cacheTimeOnce = value; } }

        /// <summary>
        /// 缓存标签 当前这次
        /// </summary>
        public string CacheTag { get { return _sqlHelper._cacheTag; } set { _sqlHelper._cacheTag = value; } }
        #endregion
        #region 方法
        /// <summary>
        /// 设置SQL监控
        /// </summary>
        /// <returns></returns>
        public SqlConfig SetSqlMonitor()
        {
            this.SqlMonitor = new SqlMonitor();
            return this;
        }

        /// <summary>
        /// 设置空SQL监控
        /// </summary>
        /// <returns></returns>
        public SqlConfig SetNullSqlMonitor()
        {
            this.SqlMonitor = new NullSqlMonitor();
            return this;
        }
        /// <summary>
        /// 设置空SQL缓存
        /// </summary>
        /// <returns></returns>
        public SqlConfig SetNullCacheService()
        {
            this.CacheService = new NullCacheService();
            return this;
        }
        /// <summary>
        /// 设置空SQL缓存--名称Md5化
        /// </summary>
        /// <returns></returns>
        public SqlConfig SetMd5MemoryCacheService()
        {
            this.CacheService = new Md5MemoryCacheService ();
            return this;
        }
        /// <summary>
        /// 清空缓存
        /// </summary>
        public void FlushFunctionCaches()
        {
            PocoData.FlushCaches();
        }

        #endregion

        internal static SqlType GetSqlType(string providerNameOrTypeName, string connectionString)
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
            if (providerNameOrTypeName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0) return SqlType.Oracle;
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

            // Assume SQL Server
            return SqlType.SqlServer;
        }
    }
}