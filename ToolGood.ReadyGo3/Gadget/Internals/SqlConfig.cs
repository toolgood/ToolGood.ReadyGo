using System;
using System.Data;
using System.Data.Common;
using ToolGood.ReadyGo3.Gadget.Caches;
using ToolGood.ReadyGo3.Gadget.Events;
using ToolGood.ReadyGo3.Gadget.Monitor;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.Gadget.Internals
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
        /// SQL执行监控
        /// </summary>
        public ISqlMonitor SqlMonitor
        {
            get { return _sqlHelper._sqlMonitor; }
            set
            {
                if (_sqlHelper._sqlMonitor != value) {
                    if (_sqlHelper._database != null) {
                        _sqlHelper._database.Dispose();
                        _sqlHelper._database = null;
                    }
  
                    _sqlHelper._sqlMonitor = value;
                }
            }
        }

        /// <summary>
        /// SQL语言类型
        /// </summary>
        public SqlType SqlType { get { return _sqlHelper._sqlType; } }
 
        /// <summary>
        /// 数据库链接字符串【写】
        /// </summary>
        public string ConnectionString { get { return _sqlHelper._connectionString; } }

        /// <summary>
        /// 数据库执行超出时间
        /// </summary>
        public int CommandTimeout { get { return _sqlHelper._commandTimeout; } set { _sqlHelper._commandTimeout = value; } }
        /// <summary>
        /// 事务级别
        /// </summary>
        public IsolationLevel? IsolationLevel { get { return _sqlHelper._isolationLevel; } set { _sqlHelper._isolationLevel = value; } }

        /// <summary>
        /// 缓存服务
        /// </summary>
        public ICacheService CacheService { get { return _sqlHelper._cacheService; } set { _sqlHelper._cacheService = value; } }

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

    }
}