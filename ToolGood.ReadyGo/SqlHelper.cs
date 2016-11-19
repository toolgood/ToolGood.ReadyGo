using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo.Caches;
using ToolGood.ReadyGo.Events;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.Monitor;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.Providers;
using ToolGood.ReadyGo.SqlBuilding;

namespace ToolGood.ReadyGo
{
    public sealed partial class SqlHelper : IDisposable, IUseCache
    {
        #region 私有变量
        //是否设置默认值
        internal bool _setDateTimeDefaultNow;
        internal bool _setStringDefaultNotNull;
        internal bool _setGuidDefaultNew;


        // 读写数据库
        internal bool _useTwoDatabase;
        internal string _writeConnectionString;
        internal string _readConnectionString;
        internal DbProviderFactory _factory;
        internal Database _writeDatabase;
        internal Database _readDatabase;

        // 缓存设置
        internal ICacheService _cacheService;
        internal bool _usedCacheService;
        internal bool _usedCacheServiceOnce;
        internal int _cacheTime;
        internal int _cacheTimeOnce;
        internal string _cacheTag;

        // 连接时间 事务级别
        internal int _commandTimeout;
        internal int _oneTimeCommandTimeout;
        internal IsolationLevel? _isolationLevel;

        internal SqlEvents _events;
        internal TableNameManger _tableNameManger;
        internal SqlConfig _sqlConfig;
        internal SqlType _sqlType;
        internal ConnectionType _connectionType;
        internal SqlRecord _sql;
        internal ISqlMonitor _sqlMonitor;
        private DatabaseProvider _provider;


        #endregion 私有变量

        #region 共公属性
        /// <summary>
        /// SQL操作事件
        /// </summary>
        public SqlEvents Events { get { return _events; } }
        /// <summary>
        /// 数据表名管理
        /// </summary>
        public TableNameManger TableNameManger { get { return _tableNameManger; } }
        /// <summary>
        /// 数据库配置
        /// </summary>
        public SqlConfig Config { get { return _sqlConfig; } }
        /// <summary>
        /// SQL设置
        /// </summary>
        public SqlRecord Sql { get { return _sql; } }

        #endregion 共公属性

        #region 构造方法 释放方法
        /// <summary>
        /// SqlHelper 构造方法
        /// </summary>
        /// <param name="connectionStringName">xml配置名</param>
        public SqlHelper(string connectionStringName, SqlType type = SqlType.None)
        {
            _sqlType = type;

            _useTwoDatabase = false;
            var entry = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (entry == null)
                throw new InvalidOperationException(string.Format("Can't find a connection string with the name '{0}'", connectionStringName));
            _writeConnectionString = entry.ConnectionString;
            initSqlHelper(entry.ProviderName);
        }

        /// <summary>
        /// SqlHelper 构造方法
        /// </summary>
        /// <param name="connectionString">数据库链接字符串</param>
        /// <param name="providerName">provider名</param>
        public SqlHelper(string connectionString, string providerName, SqlType type = SqlType.None)
        {
            _sqlType = type;

            _useTwoDatabase = false;
            _writeConnectionString = connectionString;
            initSqlHelper(providerName);
        }
        /// <summary>
        /// SqlHelper 构造方法
        /// </summary>
        /// <param name="connectionString">数据库链接字符串</param>
        /// <param name="factory">provider工厂</param>
        public SqlHelper(string connectionString, DbProviderFactory factory, SqlType type = SqlType.None)
        {
            _sqlType = type;

            _useTwoDatabase = false;
            _writeConnectionString = connectionString;
            initSqlHelper(null);
        }

        /// <summary>
        /// SqlHelper 构造方法
        /// </summary>
        /// <param name="writeConnectionString">数据库链接字符串（写）</param>
        /// <param name="readConnectionString">数据库链接字符串（读）</param>
        /// <param name="providerName">provider名</param>
        public SqlHelper(string writeConnectionString, string readConnectionString, string providerName, SqlType type = SqlType.None)
        {
            _sqlType = type;

            _useTwoDatabase = true;
            _writeConnectionString = writeConnectionString;
            _readConnectionString = readConnectionString;
            initSqlHelper(providerName);
        }

        /// <summary>
        /// SqlHelper 构造方法
        /// </summary>
        /// <param name="writeConnectionString">数据库链接字符串（写）</param>
        /// <param name="readConnectionString">数据库链接字符串（读）</param>
        /// <param name="factory">provider工厂</param>
        public SqlHelper(string writeConnectionString, string readConnectionString, DbProviderFactory factory, SqlType type = SqlType.None)
        {
            _sqlType = type;

            _useTwoDatabase = true;
            _writeConnectionString = writeConnectionString;
            _readConnectionString = readConnectionString;
            _factory = factory;
            initSqlHelper(null);
        }

        private void initSqlHelper(string _providerName)
        {
            _cacheService = new MemoryCacheService();
            _events = new SqlEvents();
            _tableNameManger = new TableNameManger();
            _sqlConfig = new SqlConfig(this);
            _sql = new SqlRecord();
            _sqlMonitor = new NullSqlMonitor();
            _sql.SqlMonitor = _sqlMonitor;


            _connectionType = ConnectionType.Default;

            if (_providerName != null) {
                if (_sqlType == SqlType.None) {
                    _sqlType = SqlConfig.GetSqlType(_providerName, _writeConnectionString);
                }
                var _provider = DatabaseProvider.Resolve(_sqlType);
                _factory = _provider.GetFactory();
            } else if (_sqlType == SqlType.None) {
                _sqlType = SqlConfig.GetSqlType(_factory.GetType().FullName, _writeConnectionString);
            }
            _provider = DatabaseProvider.Resolve(_sqlType);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (_readDatabase != null) {
                _readDatabase.Dispose();
            }
            if (_writeDatabase != null) {
                _writeDatabase.Dispose();
            }
            if (_tableNameManger != null) {
                _tableNameManger.Dispose();
            }
        }


        #endregion 构造方法 释放方法

        #region 私有方法

        internal Database getDatabase(ConnectionType type)
        {
            Database db;
            if (_useTwoDatabase == false) {
                db = getWriteDatabase();
            } else if (_connectionType == ConnectionType.Default) {
                if (type == ConnectionType.Read) {
                    db = getWriteDatabase();
                } else {
                    db = getReadDatabase();
                }
            } else if (_connectionType == ConnectionType.Read && type == ConnectionType.Read) {
                db = getReadDatabase();
            } else {
                db = getWriteDatabase();
            }

            db.CommandTimeout = _commandTimeout;
            db.OneTimeCommandTimeout = _oneTimeCommandTimeout;
            //上次
            if (_oneTimeCommandTimeout != 0) {
                _sql.LastCommandTimeout = _oneTimeCommandTimeout;
            } else {
                _sql.LastCommandTimeout = _commandTimeout;
            }


            //记录缓存
            _sql.LastCacheService = _cacheService;
            if (_usedCacheServiceOnce) {
                _sql.LastCacheTime = _cacheTimeOnce;
                _sql.LastUsedCacheService = _usedCacheServiceOnce;
                _sql.LastCacheTag = _cacheTag;
            } else {
                _sql.LastCacheTime = _cacheTime;
                _sql.LastUsedCacheService = _usedCacheService;
                _sql.LastCacheTag = null;
            }

            _oneTimeCommandTimeout = 0;
            return db;
        }

        private Database getWriteDatabase()
        {
            if (_writeDatabase == null) {
                _writeDatabase = new Database(_writeConnectionString, _factory, _sqlType);
                initDatabase(_writeDatabase);
            }
            return _writeDatabase;
        }

        private Database getReadDatabase()
        {
            if (_readDatabase == null) {
                _readDatabase = new Database(_readConnectionString, _factory, _sqlType);
                initDatabase(_readDatabase);
            }
            return _readDatabase;
        }

        private void initDatabase(Database db)
        {
            db.ConnectionOpened += (cmd) => {
                _sqlMonitor.ConnectionOpened(_connectionType);
            };
            db.ConnectionClosing += (cmd) => {
                _sqlMonitor.ConnectionClosing(_connectionType);
            };
            db.Transactioning += () => {
                _sqlMonitor.Transactioning(_connectionType);
            };
            db.Transactioned += () => {
                _sqlMonitor.Transactioned(_connectionType);
            };

            db.ExecutingCommand += (cmd) => {
                var objs = (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray();
                _sqlMonitor.ExecutingCommand(_connectionType, cmd.CommandText, objs);

                _sql.LastSQL = cmd.CommandText;
                _sql.LastArgs = objs;

                _events.OnExecutingCommand(cmd.CommandText, objs);
            };
            db.ExecutedCommand += (cmd) => {
                var objs = (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray();
                _sqlMonitor.ExecutedCommand(_connectionType, cmd.CommandText, objs);

                _events.OnExecutedCommand(cmd.CommandText, objs);
            };
            db.Exceptioned += (e) => {
                _sqlMonitor.Exception(_connectionType, e.Message);
                _sql.LastErrorMessage = e.Message;
            };
        }

        internal T Run<T>(string sql, object[] args, Func<T> func, params string[] methodtags)
        {
            if (_usedCacheService == false && _usedCacheServiceOnce == false) return func();
            _usedCacheServiceOnce = false;//记录一次

            StringBuilder sb = new StringBuilder();
            if (_usedCacheServiceOnce) {
                sb.Append(_cacheTag);
                sb.Append("|");
            }
            sb.Append(sql);
            sb.Append("|");
            sb.Append(typeof(T).FullName);

            for (int i = 0; i < args.Length; i++) {
                sb.Append("|");
                sb.Append(args[i].ToString());
            }
            for (int i = 0; i < methodtags.Length; i++) {
                sb.Append("|");
                sb.Append(methodtags[i]);
            }
            var t = _tableNameManger.Get<T>();
            if (t != null) {
                sb.Append("|");
                sb.Append(t.ToString());
            }
            string tag = sb.ToString();// getMd5String(sb.ToString());

            if (_usedCacheServiceOnce) {
                return _cacheService.Get(tag, func, _cacheTimeOnce, _cacheTag);
            }
            return _cacheService.Get(tag, func, _cacheTime, null);
        }

        #endregion 私有方法

        #region UseTransaction UseCache UseRecord
        /// <summary>
        /// 使用事务
        /// </summary>
        /// <returns></returns>
        public Transaction UseTransaction()
        {
            var db = getDatabase(_connectionType);

            return new Transaction(db, _isolationLevel);
        }

        void IUseCache.useCache(int second, string cacheTag, ICacheService cacheService)
        {
            //this.useCache(second, cacheTag, cacheService);
            _usedCacheServiceOnce = true;
            _cacheTimeOnce = second;
            _cacheTag = cacheTag;
            if (cacheService != null) {
                _cacheService = cacheService;
            }
        }

        internal void useCache(int second, string cacheTag, ICacheService cacheService)
        {
            _usedCacheServiceOnce = true;
            _cacheTimeOnce = second;
            _cacheTag = cacheTag;
            if (cacheService != null) {
                _cacheService = cacheService;
            }
        }

        #endregion UseTransaction UseCache UseRecord

        #region Execute ExecuteScalar GetDataTable GetDataSet Exists

        /// <summary>
        /// 执行 SQL 语句，并返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回受影响的行数</returns>
        public int Execute(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            return Run<int>(sql, args, () => {
                Database db;
                if (_useTwoDatabase && sql.TrimStart().StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase)) {
                    db = getDatabase(ConnectionType.Read);
                } else {
                    db = getDatabase(ConnectionType.Write);
                }
                return db.Execute(sql, args);
            }, "Execute");
        }

        /// <summary>
        /// 执行SQL 查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回查询所返回的结果集中第一行的第一列。忽略额外的列或行</returns>
        public T ExecuteScalar<T>(string sql = "", params object[] args)
        {
            return Run<T>(sql, args, () => {
                Database db;
                if (_useTwoDatabase && sql.TrimStart().StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase)) {
                    db = getDatabase(ConnectionType.Read);
                } else {
                    db = getDatabase(ConnectionType.Write);
                }
                return db.ExecuteScalar<T>(sql, args);
            }, "ExecuteScalar");
        }

        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回 DataTable</returns>
        public DataTable ExecuteDataTable(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            return Run<DataTable>(sql, args, () => {
                Database db;
                if (_useTwoDatabase && sql.TrimStart().StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase)) {
                    db = getDatabase(ConnectionType.Read);
                } else {
                    db = getDatabase(ConnectionType.Write);
                }
                return db.ExecuteDataTable(sql, args);
            }, "GetDataTable");
        }

        /// <summary>
        /// 执行SQL 查询,返回 DataSet
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回 DataSet</returns>
        public DataSet ExecuteDataSet(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            return Run<DataSet>(sql, args, () => {
                Database db;
                if (_useTwoDatabase && sql.TrimStart().StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase)) {
                    db = getDatabase(ConnectionType.Read);
                } else {
                    db = getDatabase(ConnectionType.Write);
                }
                return db.ExecuteDataSet(sql, args);
            }, "GetDataTable");
        }

        /// <summary>
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public bool Exists<T>(string sql, params object[] args)
        {
            return Count<T>(sql, args) > 0;
        }

        /// <summary>
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键值</param>
        /// <returns></returns>
        public bool Exists<T>(object primaryKey)
        {
            var pd = PocoData.ForType(typeof(T));

            var sql = string.Format("SELECT * FROM {0} WHERE {1}=@0",
                _provider.GetTableName(pd, _tableNameManger),
                _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey));
            return Count<T>(sql, primaryKey) > 0;
        }

        /// <summary>
        ///  执行SQL 查询,返回数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public int Count<T>(string sql = "", params object[] args)
        {
            var pd = PocoData.ForType(typeof(T));
            sql = SelectHelper.GetSelectCount<T>(_provider, sql, _tableNameManger);

            return Run(sql, args, () => {
                Database db;
                if (_useTwoDatabase && sql.TrimStart().StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase)) {
                    db = getDatabase(ConnectionType.Read);
                } else {
                    db = getDatabase(ConnectionType.Write);
                }
                return db.ExecuteScalar<int>(sql, args);
            }, "Count");
        }

        #endregion Execute ExecuteScalar GetDataTable GetDataSet Exists

        #region Select Page SkipTake
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public List<T> Select<T>(string sql = "", params object[] args)
        {
            sql = SelectHelper.AddSelectClause<T>(_provider, sql, _tableNameManger);
            return Run<List<T>>(sql, args, () => {
                var db = getDatabase(ConnectionType.Read);
                return db.Query<T>(sql, args).ToList();
            }, "Select");
        }
        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public Page<T> Page<T>(long page, long itemsPerPage, string sql = "", params object[] args)
        {
            sql = SelectHelper.AddSelectClause<T>(_provider, sql, _tableNameManger);
            return Run<Page<T>>(sql, args, () => {
                var db = getDatabase(ConnectionType.Read);
                return db.Page<T>(page, itemsPerPage, sql, args);
            }, "Page", page.ToString(), itemsPerPage.ToString());
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="skip">跳过</param>
        /// <param name="take">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public List<T> SkipTake<T>(long skip, long take, string sql = "", params object[] args)
        {

            sql = SelectHelper.AddSelectClause<T>(_provider, sql, _tableNameManger);

            return Run<List<T>>(sql, args, () => {
                var db = getDatabase(ConnectionType.Read);
                return db.SkipTake<T>(skip, take, sql, args);
            }, "SkipTake", skip.ToString(), take.ToString());
        }

        #endregion Select Page SkipTake

        #region Single SingleOrDefault First FirstOrDefault

        /// <summary>
        /// 获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键名</param>
        /// <returns></returns>
        public T SingleById<T>(object primaryKey)
        {
            var pd = PocoData.ForType(typeof(T));

            var sql = string.Format("WHERE {0}=@0", _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey));
            sql = SelectHelper.AddSelectClause<T>(_provider, sql, _tableNameManger);

            return Single<T>(sql, primaryKey);
        }

        /// <summary>
        /// 获取唯一一个类型，若数量大于1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键名</param>
        /// <returns></returns>
        public T SingleOrDefaultById<T>(object primaryKey)
        {
            var pd = PocoData.ForType(typeof(T));

            var sql = string.Format("WHERE {0}=@0", _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey));
            sql = SelectHelper.AddSelectClause<T>(_provider, sql, _tableNameManger);
            return SingleOrDefault<T>(sql, primaryKey);
        }

        /// <summary>
        ///获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public T Single<T>(string sql = "", params object[] args)
        {

            sql = SelectHelper.AddSelectClause<T>(_provider, sql, _tableNameManger);

            return Run<T>(sql, args, () => {
                var db = getDatabase(ConnectionType.Read);
                return db.Query<T>(sql, args).Single();
            }, "Single");
        }

        /// <summary>
        /// 获取唯一一个类型，若数量大于1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public T SingleOrDefault<T>(string sql = "", params object[] args)
        {

            sql = SelectHelper.AddSelectClause<T>(_provider, sql, _tableNameManger);
            return Run<T>(sql, args, () => {
                var db = getDatabase(ConnectionType.Read);
                return db.Query<T>(sql, args).SingleOrDefault();
            }, "SingleOrDefault");
        }

        /// <summary>
        /// 获取第一个类型，若数量为0，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public T First<T>(string sql = "", params object[] args)
        {

            sql = SelectHelper.AddSelectClause<T>(_provider, sql, _tableNameManger);
            return Run<T>(sql, args, () => {
                var db = getDatabase(ConnectionType.Read);
                return db.Query<T>(sql, args).First();
            }, "First");
        }

        /// <summary>
        /// 获取第一个类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public T FirstOrDefault<T>(string sql = "", params object[] args)
        {

            sql = SelectHelper.AddSelectClause<T>(_provider, sql, _tableNameManger);
            return Run<T>(sql, args, () => {
                var db = getDatabase(ConnectionType.Read);
                return db.Query<T>(sql, args).FirstOrDefault();
            }, "FirstOrDefault");
        }

        #endregion Single SingleOrDefault First FirstOrDefault

        #region Object  Insert Update Delete DeleteById Save

        /// <summary>
        /// 插入集合，支持主键(整数型)自动获取。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="quick">为True时，不获取ID</param>
        public void InsertList<T>(List<T> list, bool quick = true) where T : class
        {
            if (list == null) throw new ArgumentNullException("list is null.");
            if (list.Count == 0) return;
            if (_setDateTimeDefaultNow || _setStringDefaultNotNull || _setGuidDefaultNew) {
                foreach (var item in list) {
                    DefaultValue.SetDefaultValue<T>(item, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew);
                }
            }
            if (Events.OnBeforeInsert(list)) return;
            var db = getDatabase(ConnectionType.Write);
            db.Insert(list, _tableNameManger, quick);
            Events.OnAfterInsert(list);
        }
        //private void insertList


        /// <summary>
        /// 插入，支持主键自动获取。
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public object Insert<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (poco is IList) throw new ArgumentException("poco is a list type.");

            if (_setDateTimeDefaultNow || _setStringDefaultNotNull || _setGuidDefaultNew) {
                DefaultValue.SetDefaultValue<T>(poco, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew);
            }
            if (Events.OnBeforeInsert(poco)) return null;
            var db = getDatabase(ConnectionType.Write);
            var obj = db.Insert(poco, _tableNameManger);
            Events.OnAfterInsert(poco);
            return obj;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public int Update<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (Events.OnBeforeUpdate(poco)) return -1;

            int r;
            var db = getDatabase(ConnectionType.Write);
            r = db.Update(poco, _tableNameManger);
            Events.OnAfterUpdate(poco);
            return r;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public int Delete<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (Events.OnBeforeDelete(poco)) return -1;

            var db = getDatabase(ConnectionType.Write);
            var t = db.Delete(poco, _tableNameManger);

            Events.OnAfterDelete(poco);
            return t;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public int Delete<T>(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            if (sql.StartsWith("DELETE ", StringComparison.CurrentCultureIgnoreCase)) {
                var wdb = getDatabase(ConnectionType.Write);
                return wdb.Execute(sql, args);
            }

            var pd = PocoData.ForType(typeof(T));

            var tableName = _provider.GetTableName(pd, _tableNameManger);
            var db = getDatabase(ConnectionType.Write);

            return db.Execute(string.Format("DELETE FROM {0} {1}", tableName, sql), args);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public int DeleteById<T>(object primaryKey)
        {
            var db = getDatabase(ConnectionType.Write);
            return db.Delete<T>(primaryKey, _tableNameManger);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="poco"></param>
        public void Save(object poco)
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            var db = getDatabase(ConnectionType.Write);
            if (db.IsNew(poco)) {
                Insert(poco);
            } else {
                Update(poco);
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public int Update<T>(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");

            if (sql.StartsWith("UPDATE ", StringComparison.CurrentCultureIgnoreCase)) {
                var wdb = getDatabase(ConnectionType.Write);
                return wdb.Execute(sql, args);
            }

            var pd = PocoData.ForType(typeof(T));

            var tableName = _provider.GetTableName(pd, _tableNameManger);
            var db = getDatabase(ConnectionType.Write);

            return db.Execute(string.Format("UPDATE {0} {1}", tableName, sql), args);
        }





        #endregion Object  Insert Update Delete DeleteById Save

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetTableName(Type type)
        {
            if (type == null) throw new ArgumentNullException("type is null.");
            var pd = PocoData.ForType(type);
            return _provider.GetTableName(pd, _tableNameManger);
        }
    }
}