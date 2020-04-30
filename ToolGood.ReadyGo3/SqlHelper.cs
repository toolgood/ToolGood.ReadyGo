using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.Gadget.Events;
using ToolGood.ReadyGo3.Gadget.Internals;
using ToolGood.ReadyGo3.Internals;
using ToolGood.ReadyGo3.PetaPoco;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3
{
    public partial class SqlHelper : IDisposable
    {
        #region 私有变量
        //是否设置默认值
        internal bool _setDateTimeDefaultNow;
        internal bool _setStringDefaultNotNull;
        internal bool _setGuidDefaultNew;
        internal bool _sql_firstWithLimit1;
        internal bool _sql_singleWithLimit2;
        //internal bool _use_proxyType;


        // 读写数据库
        internal readonly string _connectionString;
        internal readonly DbProviderFactory _factory;
        internal Database _database;

        // 连接时间 事务级别
        internal int _commandTimeout;
        internal int _oneTimeCommandTimeout;
        internal IsolationLevel? _isolationLevel;

        internal readonly SqlEvents _events;
        private readonly SqlConfig _sqlConfig;
        internal readonly SqlType _sqlType;
        internal readonly SqlRecord _sql = new SqlRecord();
        private readonly DatabaseProvider _provider;
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

        /// <summary>
        /// SQL语言类型
        /// </summary>
        public SqlType _SqlType { get { return _sqlType; } }

        /// <summary>
        /// 是否释放
        /// </summary>
        public bool _IsDisposed { get { return _isDisposable; } }

        #endregion 共公属性

        #region 构造方法 释放方法
        /// <summary>
        /// SqlHelper 构造方法
        /// </summary>
        /// <param name="connectionString">数据库链接字符串</param>
        /// <param name="factory">provider工厂</param>
        /// <param name="type"></param>
        public SqlHelper(string connectionString, DbProviderFactory factory, SqlType type)
        {
            _sqlType = type;
            _factory = factory;

            _events = new SqlEvents(this);
            _connectionString = connectionString;
            _sqlConfig = new SqlConfig(this);
            _provider = DatabaseProvider.Resolve(_sqlType);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            _isDisposable = true;
            if (_database != null) {

                _database.Dispose();
                _database = null;
            }
        }


        #endregion 构造方法 释放方法

        #region 私有方法

        internal Database GetDatabase()
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

        /// <summary>
        /// 格式SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal string FormatSql(string sql)
        {
            if (sql == null) { return ""; }
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

        #region UseTransaction 
        /// <summary>
        /// 使用事务
        /// </summary>
        /// <returns></returns>
        public Transaction UseTransaction()
        {
            return new Transaction(GetDatabase());
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
            sql = FormatSql(sql);
            return GetDatabase().Execute(sql, args);
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
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            sql = FormatSql(sql);
            return GetDatabase().ExecuteScalar<T>(sql, args);
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
            sql = FormatSql(sql);
            return GetDatabase().ExecuteDataTable(sql, args);

        }

        //#if !NETSTANDARD2_0
        /// <summary>
        /// 执行SQL 查询,返回 DataSet
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回 DataSet</returns>
        public DataSet ExecuteDataSet(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            sql = FormatSql(sql);
            return GetDatabase().ExecuteDataSet(sql, args);
        }
        //#endif

        /// <summary>
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public bool Exists<T>(string sql, params object[] args)
        {
            sql = FormatSql(sql);
            return Count<T>(sql, args) > 0;
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
            sql = sql.Trim();
            if (sql.StartsWith("SELECT ", StringComparison.CurrentCultureIgnoreCase) == false) {
                var pd = PocoData.ForType(typeof(T));
                var table = _provider.GetTableName(pd);
                sql = FormatSql(sql);
                sql = $"SELECT COUNT(*) FROM {table} {sql}";
            }
            return GetDatabase().ExecuteScalar<int>(sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回数量
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Count(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            return GetDatabase().ExecuteScalar<int>(sql, args);
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
            sql = FormatSql(sql);
            return GetDatabase().Query<T>(sql, args).ToList();
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public List<T> Select<T>(int limit, string sql = "", params object[] args)
        {
            sql = FormatSql(sql);
            return GetDatabase().Query<T>(0, limit, sql, args).ToList();
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
        public List<T> Select<T>(int limit, int offset, string sql = "", params object[] args)
        {
            sql = FormatSql(sql);
            return GetDatabase().Query<T>(offset, limit, sql, args).ToList();
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<T> SelectPage<T>(int page, int itemsPerPage, string sql = "", params object[] args)
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            sql = FormatSql(sql);
            return GetDatabase().Query<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args).ToList();
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
        public Page<T> Page<T>(int page, int itemsPerPage, string sql = "", params object[] args)
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            sql = FormatSql(sql);
            return GetDatabase().Page<T>(page, itemsPerPage, sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="columnSql">查询列 SQL语句</param>
        /// <param name="tableSql">TABLE SQL语句</param>
        /// <param name="orderSql">ORDER BY SQL语句</param>
        /// <param name="whereSql">WHERE SQL语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public List<T> SelectSql<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(columnSql)) { throw new ArgumentNullException("columnSql is null."); }
            if (string.IsNullOrWhiteSpace(tableSql)) { throw new ArgumentNullException("tableSql is null."); }
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            columnSql = RemoveStart(columnSql, "SELECT ");
            tableSql = RemoveStart(tableSql, "FROM ");
            orderSql = RemoveStart(orderSql, "ORDER BY ");
            whereSql = RemoveStart(whereSql, "WHERE ");

            var sql = _provider.CreateSql((int)itemsPerPage, (int)((Math.Max(0, page - 1)) * itemsPerPage), columnSql, tableSql, orderSql, whereSql);
            sql = FormatSql(sql);
            return GetDatabase().Query<T>(sql, args).ToList();
        }
        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="columnSql">查询列 SQL语句</param>
        /// <param name="tableSql">TABLE SQL语句</param>
        /// <param name="orderSql">ORDER BY SQL语句</param>
        /// <param name="whereSql">WHERE SQL语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public Page<T> PageSql<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(columnSql)) { throw new ArgumentNullException("columnSql is null."); }
            if (string.IsNullOrWhiteSpace(tableSql)) { throw new ArgumentNullException("tableSql is null."); }
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            columnSql = RemoveStart(columnSql, "SELECT ");
            tableSql = RemoveStart(tableSql, "FROM ");
            orderSql = RemoveStart(orderSql, "ORDER BY ");
            whereSql = RemoveStart(whereSql, "WHERE ");

            string countSql = string.IsNullOrEmpty(whereSql) ? $"SELECT COUNT(1) FROM {tableSql}" : $"SELECT COUNT(1) FROM {tableSql} WHERE {whereSql}";
            countSql = FormatSql(countSql);

            var sql = _provider.CreateSql((int)itemsPerPage, (int)((Math.Max(0, page - 1)) * itemsPerPage), columnSql, tableSql, orderSql, whereSql);
            sql = FormatSql(sql);
            return GetDatabase().PageSql<T>(page, itemsPerPage, sql, countSql, args);
        }
        private string RemoveStart(string txt, string startsText)
        {
            if (string.IsNullOrEmpty(txt) == false) {
                txt = txt.Trim();
                if (txt.StartsWith(startsText, StringComparison.InvariantCultureIgnoreCase)) {
                    txt = txt.Substring(startsText.Length);
                }
            }
            return txt;
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
            var pd = PocoData.ForType(typeof(T));
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
            var pd = PocoData.ForType(typeof(T));
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
            sql = FormatSql(sql);
            if (_sql_singleWithLimit2 == false) {
                return GetDatabase().Query<T>(sql, args).Single();
            }
            return GetDatabase().Query<T>(0, 2, sql, args).Single();
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
            sql = FormatSql(sql);
            if (_sql_singleWithLimit2 == false) {
                return GetDatabase().Query<T>(sql, args).SingleOrDefault();
            }
            return GetDatabase().Query<T>(0, 2, sql, args).SingleOrDefault();
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
            sql = FormatSql(sql);
            if (_sql_firstWithLimit1 == false) {
                return GetDatabase().Query<T>(sql, args).First();
            }
            return GetDatabase().Query<T>(0, 1, sql, args).First();
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
            sql = FormatSql(sql);
            if (_sql_firstWithLimit1 == false) {
                return GetDatabase().Query<T>(sql, args).FirstOrDefault();
            }
            return GetDatabase().Query<T>(0, 1, sql, args).FirstOrDefault();
        }

        #endregion Single SingleOrDefault First FirstOrDefault

        #region Object  Insert Update Delete DeleteById Save

        /// <summary>
        /// 插入集合，不返回主键
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

            GetDatabase().Insert(list);
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

            var obj = GetDatabase().Insert(poco);
            _Events.OnAfterInsert(poco);
            return obj;
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="table"></param>
        /// <param name="poco"></param>
        /// <param name="autoIncrement"></param>
        /// <returns></returns>
        public object Insert(string table, object poco, bool autoIncrement = false)
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (poco is IList) throw new ArgumentException("poco is a list type, use InsertList methon .");

            if (_setDateTimeDefaultNow || _setStringDefaultNotNull || _setGuidDefaultNew) {
                DefaultValue.SetDefaultValue(poco, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew);
            }
            if (_Events.OnBeforeInsert(poco)) return null;

            var obj = GetDatabase().Insert(table, poco, autoIncrement, null);
            _Events.OnAfterInsert(poco);
            return obj;
        }
        public object Insert(string table, object poco, IEnumerable<string> ignoreFields)
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (poco is IList) throw new ArgumentException("poco is a list type, use InsertList methon .");

            if (_setDateTimeDefaultNow || _setStringDefaultNotNull || _setGuidDefaultNew) {
                DefaultValue.SetDefaultValue(poco, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew);
            }
            if (_Events.OnBeforeInsert(poco)) return null;

            var obj = GetDatabase().Insert(table, poco, false, ignoreFields);
            _Events.OnAfterInsert(poco);
            return obj;
        }
        public object Insert(string table, object poco, bool autoIncrement, IEnumerable<string> ignoreFields)
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (poco is IList) throw new ArgumentException("poco is a list type, use InsertList methon .");

            if (_setDateTimeDefaultNow || _setStringDefaultNotNull || _setGuidDefaultNew) {
                DefaultValue.SetDefaultValue(poco, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew);
            }
            if (_Events.OnBeforeInsert(poco)) return null;

            var obj = GetDatabase().Insert(table, poco, autoIncrement, ignoreFields);
            _Events.OnAfterInsert(poco);
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

            //if (poco is IUpdateChange) {
            //    var pd = PocoData.ForType(typeof(T));
            //    StringBuilder stringBuilder = new StringBuilder();
            //    ObjectToSql(stringBuilder, poco, ",", new string[] { pd.TableInfo.PrimaryKey });
            //    if (stringBuilder.Length == 0) { return 0; }
            //    stringBuilder.Insert(0, "SET ");
            //    object primaryKeyValue = null;
            //    foreach (var i in pd.Columns) {
            //        if (i.Value.ResultColumn) continue;
            //        if (string.Compare(i.Value.ColumnName, pd.TableInfo.PrimaryKey, true) == 0) {
            //            if (primaryKeyValue == null) primaryKeyValue = i.Value.GetValue(poco);
            //        }
            //    }
            //    stringBuilder.Append($" WHERE [{pd.TableInfo.PrimaryKey}]=@0");
            //    return Update<T>(stringBuilder.ToString(), primaryKeyValue);
            //}

            if (_Events.OnBeforeUpdate(poco)) return -1;
            int r = GetDatabase().Update(poco);
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

            var t = GetDatabase().Delete(poco);

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
            sql = FormatSql(sql);
            return GetDatabase().Delete<T>(sql, args);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public int DeleteById<T>(object primaryKey)
        {
            return GetDatabase().Delete<T>(primaryKey);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="poco"></param>
        public void Save<T>(T poco)
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            //if (poco is IUpdateChange) {
            //    var pd = PocoData.ForType(typeof(T));
            //    StringBuilder stringBuilder = new StringBuilder();
            //    ObjectToSql(stringBuilder, poco, ",", new string[] { pd.TableInfo.PrimaryKey });
            //    if (stringBuilder.Length == 0) { return; }
            //    stringBuilder.Insert(0, "SET ");
            //    object primaryKeyValue = null;
            //    foreach (var i in pd.Columns) {
            //        if (i.Value.ResultColumn) continue;
            //        if (string.Compare(i.Value.ColumnName, pd.TableInfo.PrimaryKey, true) == 0) {
            //            if (primaryKeyValue == null) primaryKeyValue = i.Value.GetValue(poco);
            //        }
            //    }
            //    stringBuilder.Append($" WHERE [{pd.TableInfo.PrimaryKey}]=@0");
            //    Update<T>(stringBuilder.ToString(), primaryKeyValue);
            //    return;
            //}
            GetDatabase().Save(poco);
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
            sql = FormatSql(sql);
            return GetDatabase().Update<T>(sql, args);
        }

        #endregion Object  Insert Update Delete DeleteById Save


        /// <summary>
        /// 获取表名，
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public dynamic GetTableName(Type type)
        {
            return new TableName(type, _provider);
        }
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public dynamic GetTableName<T>() where T : class
        {
            return GetTableName(typeof(T));
        }



    }
}
