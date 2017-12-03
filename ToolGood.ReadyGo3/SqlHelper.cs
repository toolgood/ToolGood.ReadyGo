using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.Gadget.Caches;
using ToolGood.ReadyGo3.Gadget.Events;
using ToolGood.ReadyGo3.Gadget.Internals;
using ToolGood.ReadyGo3.Gadget.Monitor;
using ToolGood.ReadyGo3.PetaPoco;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3
{
    public partial class SqlHelper:IDisposable
    {
        #region 私有变量
        private static string _lastConnectionString;
        private static DbProviderFactory _lastFactory;
        private static string _lastProviderName;
        [ThreadStatic]
        internal static SqlHelper _lastSqlHelper;


        //是否设置默认值
        internal bool _setDateTimeDefaultNow;
        internal bool _setStringDefaultNotNull;
        internal bool _setGuidDefaultNew;
      

        // 读写数据库
        internal string _connectionString;
        internal DbProviderFactory _factory;
        internal Database _database;
        internal string _schemaName;

        // 缓存设置
        internal ICacheService _cacheService;
        internal bool _usedCacheServiceOnce;
        internal int _cacheTimeOnce;
        internal string _cacheTag;

        // 连接时间 事务级别
        internal int _commandTimeout;
        internal int _oneTimeCommandTimeout;
        internal IsolationLevel? _isolationLevel;

        internal SqlEvents _events;
        internal SqlConfig _sqlConfig;
        internal SqlType _sqlType;
        internal SqlRecord _sql;
        internal ISqlMonitor _sqlMonitor;
        internal DatabaseProvider _provider;
        internal bool _isDisposable;


        #endregion 私有变量

        #region 共公属性
        /// <summary>
        /// SQL操作事件
        /// </summary>
        public SqlEvents _Events { get { return _events; } }

        /// <summary>
        /// 数据库配置
        /// </summary>
        public SqlConfig _Config { get { return _sqlConfig; } }
        /// <summary>
        /// SQL设置
        /// </summary>
        public SqlRecord _Sql { get { return _sql; } }


        public bool _IsDisposed { get { return _isDisposable; } }

        #endregion 共公属性

        #region 构造方法 释放方法

        public SqlHelper()
        {
            if (_lastProviderName != null) {
                initSqlHelper(_lastConnectionString, _lastProviderName);
            } else if (_lastFactory != null) {
                _factory = _lastFactory;
                initSqlHelper(_lastConnectionString, null);
            }
            var conns = ConfigurationManager.ConnectionStrings;
            if (conns.Count == 0)
                throw new InvalidOperationException("Can't find a connection string.");
            initSqlHelper(conns[0].ConnectionString, conns[0].ProviderName);
        }


        /// <summary>
        /// SqlHelper 构造方法
        /// </summary>
        /// <param name="connectionStringName">xml配置名</param>
        public SqlHelper(string connectionStringName, SqlType type = SqlType.None)
        {
            _sqlType = type;

            var entry = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (entry == null)
                throw new InvalidOperationException(string.Format("Can't find a connection string with the name '{0}'", connectionStringName));
            initSqlHelper(entry.ConnectionString, entry.ProviderName);
        }

        /// <summary>
        /// SqlHelper 构造方法
        /// </summary>
        /// <param name="connectionString">数据库链接字符串</param>
        /// <param name="providerName">provider名</param>
        public SqlHelper(string connectionString, string providerName, SqlType type = SqlType.None)
        {
            _sqlType = type;

            initSqlHelper(connectionString, providerName);
        }
        /// <summary>
        /// SqlHelper 构造方法
        /// </summary>
        /// <param name="connectionString">数据库链接字符串</param>
        /// <param name="factory">provider工厂</param>
        public SqlHelper(string connectionString, DbProviderFactory factory, SqlType type = SqlType.None)
        {
            _sqlType = type;
            _factory = factory;
            initSqlHelper(connectionString, null);
        }



        private void initSqlHelper(string connectionString, string providerName)
        {
            _lastConnectionString = connectionString;
            _lastProviderName = providerName;

            _connectionString = connectionString;
            var txts = connectionString.Split(';');
            foreach (var txt in txts) {
                var sp = txt.Split('=');
                if (sp.Length != 2) continue;
                if (sp[0].ToLower() == "database") {
                    _schemaName = sp[1];
                    break;
                }
                if (sp[0].ToLower() == "data source") {
                    _schemaName = sp[1];
                    break;
                }
            }


            _cacheService = new MemoryCacheService();
            _events = new SqlEvents();
            _sqlConfig = new SqlConfig(this);
            _sql = new SqlRecord();
            _sqlMonitor = new NullSqlMonitor();


            if (_sqlType == SqlType.None) {
                _sqlType = DatabaseProvider.GetSqlType(providerName ?? _factory.GetType().FullName, _connectionString);
            }
            _provider = DatabaseProvider.Resolve(_sqlType);
            if (_factory == null) {
                _factory = _provider.GetFactory();
                _lastFactory = _factory;

            }
            if (_lastSqlHelper==null|| _lastSqlHelper._isDisposable) {
                _lastSqlHelper = this;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _isDisposable = true;
            if (_database != null) {
                _database.Dispose();
            }
        }


        #endregion 构造方法 释放方法

        #region 私有方法

        internal Database getDatabase()
        {
            if (_database == null) {
                _database = new Database(this);
            }
            Database db = _database;

            db.CommandTimeout = _commandTimeout;
            db.OneTimeCommandTimeout = _oneTimeCommandTimeout;

            _oneTimeCommandTimeout = 0;
            return db;
        }

        internal T Run<T>(string sql, object[] args, Func<T> func, params string[] methodtags)
        {
            if (_usedCacheServiceOnce == false) return func();
            _usedCacheServiceOnce = false;//记录一次

            StringBuilder sb = new StringBuilder();
            sb.Append(_cacheTag);
            sb.Append("|");

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
            string tag = sb.ToString();// getMd5String(sb.ToString());

            return _cacheService.Get(tag, func, _cacheTimeOnce, _cacheTag);
        }

        /// <summary>
        /// 格式SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal string formatSql(string sql)
        {
            bool usedEscapeSql = false;
            char escapeSql = '`';
            if (_sqlType == SqlType.MySql || _sqlType == SqlType.MariaDb) {
                usedEscapeSql = true;
                escapeSql = '`';
            } else if (_sqlType == SqlType.Oracle || _sqlType == SqlType.FirebirdDb || _sqlType == SqlType.PostgreSQL) {
                usedEscapeSql = true;
                escapeSql = '"';
            }
            if (usedEscapeSql == false) return sql;

            StringBuilder _where = new StringBuilder();

            bool isInText = false, isInTableColumn = false, jump = false;
            var c = 'a';

            for (int i = 0; i < sql.Length; i++) {
                var t = sql[i];
                if (jump) {
                    jump = false;
                } else if (isInText) {
                    if (t == c) isInText = false;
                    if (t == '\\') jump = true;
                } else if (isInTableColumn) {
                    if (t == ']') {
                        isInTableColumn = false;
                        _where.Append(escapeSql);
                        continue;
                    }
                } else if (t == '[') {
                    isInTableColumn = true;
                    _where.Append(escapeSql);
                    continue;
                } else if ("\"'`".Contains(t)) {
                    isInText = true;
                    c = t;
                }
                _where.Append(t);
            }

            return _where.ToString();
        }
        #endregion 私有方法

        #region UseTransaction UseCache UseRecord
        /// <summary>
        /// 使用事务
        /// </summary>
        /// <returns></returns>
        public Transaction UseTransaction()
        {
            var db = getDatabase();
            return new Transaction(db);
        }
        public SqlHelper UseChache(int second, string cacheTag = null, ICacheService cacheService = null)
        {
            _usedCacheServiceOnce = true;
            _cacheTimeOnce = second;
            _cacheTag = cacheTag;
            if (cacheService != null) {
                _cacheService = cacheService;
            }
            return this;
        }

        #endregion UseTransaction UseCache UseRecord

        #region Execute ExecuteScalar ExecuteDataTable ExecuteDataSet Exists

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
                sql = formatSql(sql);
                Database db = getDatabase();
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
                sql = formatSql(sql);
                Database db = getDatabase();
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
                Database db = getDatabase();
                sql = formatSql(sql);
                return db.ExecuteDataTable(sql, args);
            }, "ExecuteDataTable");
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
                Database db = getDatabase();
                sql = formatSql(sql);
                return db.ExecuteDataSet(sql, args);
            }, "ExecuteDataTable");
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
            sql = formatSql(sql);
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
            var pd = PocoData.ForType(typeof(T), null);
            var table = _provider.EscapeSqlIdentifier(pd.TableInfo.TableName);
            var pk = _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
            var sql = $"SELECT COUNT(*) FROM {table} WHERE {pk}=@0";
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
            var pd = PocoData.ForType(typeof(T), null);
            var table = _provider.EscapeSqlIdentifier(pd.TableInfo.TableName);
            sql = formatSql(sql);
            sql = $"SELECT COUNT(*) FROM {table} {sql}";

            return Run(sql, args, () => {
                Database db = getDatabase();
                return db.ExecuteScalar<int>(sql, args);
            }, "Count");
        }

        #endregion Execute ExecuteScalar ExecuteDataTable ExecuteDataSet Exists

        #region Select Page Select
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public List<T> Select<T>(string sql = "", params object[] args)
        {
            return Run<List<T>>(sql, args, () => {
                sql = formatSql(sql);
                Database db = getDatabase();
                return db.Query<T>(sql, args).ToList();
            }, "Select");
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset">跳过</param>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public List<T> Select<T>(long limit, string sql = "", params object[] args)
        {
            return Run<List<T>>(sql, args, () => {
                Database db = getDatabase();
                sql = formatSql(sql);
                return db.Query<T>(0, limit, sql, args).ToList();
            }, "Select",limit.ToString());
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset">跳过</param>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public List<T> Select<T>(long limit, long offset, string sql = "", params object[] args)
        {
            return Run<List<T>>(sql, args, () => {
                Database db = getDatabase();
                sql = formatSql(sql);
                return db.Query<T>(offset, limit, sql, args).ToList();
            }, "Select", offset.ToString(), limit.ToString());
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
            return Run<Page<T>>(sql, args, () => {
                Database db = getDatabase();
                sql = formatSql(sql);
                return db.Page<T>(page, itemsPerPage, sql, args);
            }, "Page", page.ToString(), itemsPerPage.ToString());
        }


        #endregion Select Page Select

        #region Single SingleOrDefault First FirstOrDefault

        /// <summary>
        /// 获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键名</param>
        /// <returns></returns>
        public T SingleById<T>(object primaryKey)
        {
            var pd = PocoData.ForType(typeof(T), null);
            var pk = _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
            var sql = $"WHERE {pk}=@0";

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
            var pd = PocoData.ForType(typeof(T), null);
            var pk = _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
            var sql = $"WHERE {pk}=@0";
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
            return Run<T>(sql, args, () => {
                sql = formatSql(sql);
                Database db = getDatabase();
                return db.Query<T>(0, 2, sql, args).Single();
            }, "Single");
        }
        internal T _Single<T>(string sql = "", params object[] args)
        {
            return Run<T>(sql, args, () => {
                //sql = formatSql(sql);
                Database db = getDatabase();
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
            return Run<T>(sql, args, () => {
                sql = formatSql(sql);
                Database db = getDatabase();
                return db.Query<T>(0, 2, sql, args).SingleOrDefault();
            }, "SingleOrDefault");
        }
        internal T _SingleOrDefault<T>(string sql = "", params object[] args)
        {
            return Run<T>(sql, args, () => {
                //sql = formatSql(sql);
                Database db = getDatabase();
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
            return Run<T>(sql, args, () => {
                sql = formatSql(sql);
                Database db = getDatabase();
                return db.Query<T>(0, 1, sql, args).First();
            }, "First");
        }
        internal T _First<T>(string sql = "", params object[] args)
        {
            return Run<T>(sql, args, () => {
                //sql = formatSql(sql);
                Database db = getDatabase();
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
            return Run<T>(sql, args, () => {
                sql = formatSql(sql);
                Database db = getDatabase();
                return db.Query<T>(0, 1, sql, args).FirstOrDefault();
            }, "FirstOrDefault");
        }


        internal T _FirstOrDefault<T>(string sql = "", params object[] args)
        {
            return Run<T>(sql, args, () => {
                //sql = formatSql(sql);
                Database db = getDatabase();
                return db.Query<T>(sql, args).FirstOrDefault();
            }, "FirstOrDefault");
        }
        #endregion Single SingleOrDefault First FirstOrDefault

        #region Object  Insert Update Delete DeleteById Save

        /// <summary>
        /// 插入集合，支持主键(整数型)自动获取。不返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void InsertList<T>(List<T> list) where T : class
        {
            if (list == null) throw new ArgumentNullException("list is null.");
            if (list.Count == 0) return;
            if (_setDateTimeDefaultNow || _setStringDefaultNotNull || _setGuidDefaultNew) {
                foreach (var item in list) {
                    DefaultValue.SetDefaultValue<T>(item, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew);
                }
            }
            if (_Events.OnBeforeInsert(list)) return;
            Database db = getDatabase();
            db.Insert(list);
            _Events.OnAfterInsert(list);
        }

        /// <summary>
        /// 插入，支持主键自动获取。
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public object Insert<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (poco is IList) throw new ArgumentException("poco is a list type, use InsertList methon .");

            if (_setDateTimeDefaultNow || _setStringDefaultNotNull || _setGuidDefaultNew) {
                DefaultValue.SetDefaultValue<T>(poco, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew);
            }
            if (_Events.OnBeforeInsert(poco)) return null;
            Database db = getDatabase();
            var obj = db.Insert(poco);
            _Events.OnAfterInsert(poco);
            return obj;
        }


        internal object Insert(string sql, string primaryKeyName)
        {
            Database db = getDatabase();
            return db.ExecuteInsert(sql, primaryKeyName);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public int Update<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (_Events.OnBeforeUpdate(poco)) return -1;

            int r;
            Database db = getDatabase();
            r = db.Update(poco);
            _Events.OnAfterUpdate(poco);
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
            if (_Events.OnBeforeDelete(poco)) return -1;

            Database db = getDatabase();
            var t = db.Delete(poco);

            _Events.OnAfterDelete(poco);
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
            sql = formatSql(sql);
            var db = getDatabase();
            return db.Delete<T>(sql, args);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public int DeleteById<T>(object primaryKey)
        {
            Database db = getDatabase();
            return db.Delete<T>(primaryKey);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="poco"></param>
        public void Save(object poco)
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            Database db = getDatabase();
            db.Save(poco);
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
            sql = formatSql(sql);
            Database db = getDatabase();
            return db.Update<T>(sql, args);
        }





        #endregion Object  Insert Update Delete DeleteById Save



    }
}
