using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using ToolGood.ReadyGo.Gadget.Internals;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo
{
    /// <summary>
    /// SqlHelper 辅助类
    /// </summary>
    public partial class SqlHelper : IDisposable
    {
        #region 私有变量
        //是否设置默认值
        internal bool _setDateTimeDefaultNow;
        internal bool _setStringDefaultNotNull;
        internal bool _setGuidDefaultNew;
        internal bool _sql_firstWithLimit1;

        // 读写数据库
        internal readonly string _connectionString;
        internal readonly DbProviderFactory _factory;
        internal Database _database;

        // 连接时间 事务级别
        internal int _commandTimeout;
        internal int _oneTimeCommandTimeout;
        internal IsolationLevel? _isolationLevel;

        internal SqlRecord _sql = new SqlRecord();
        internal bool _isDisposable;
        internal readonly SqlType _sqlType;
        private SqlConfig _sqlConfig;

        #endregion 私有变量

        #region 共公属性
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

            _connectionString = connectionString;
            _sqlConfig = new SqlConfig(this);
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

                _sqlConfig = null;
                _sql = null;
            }
        }

        #endregion 构造方法 释放方法

        #region 私有方法

        internal Database GetDatabase()
        {
            if (_database == null) {
                _database = new Database(_connectionString, DatabaseProvider.GetDatabaseType(_sqlType), _factory, _isolationLevel);
                _database._sqlHelper = this;
            }
            Database db = _database;

            db.CommandTimeout = _commandTimeout;
            db.OneTimeCommandTimeout = _oneTimeCommandTimeout;

            _oneTimeCommandTimeout = 0;
            return db;
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

        private Page<T> ToPage<T>(NPoco.Page<T> page) where T : class
        {
            return new Page<T> {
                CurrentPage = (int)page.CurrentPage,
                PageSize = (int)page.ItemsPerPage,
                TotalItems = (int)page.TotalItems,
                Items = page.Items,
            };
        }

        #endregion 私有方法

        #region UseTransaction
        /// <summary>
        /// 使用事务
        /// </summary>
        /// <returns></returns>
        public Transaction UseTransaction()
        {
            return new Transaction(GetDatabase(), _isolationLevel ?? IsolationLevel.Unspecified);
        }
        #endregion UseTransaction

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

            return GetDatabase().ExecuteDataTable(sql, args);
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

            return GetDatabase().ExecuteDataSet(sql, args);
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
                var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
                var table = GetDatabase().DatabaseType.EscapeTableName(pd.TableInfo.TableName);

                sql = $"SELECT COUNT(*) FROM {table} {sql}";
            }
            return GetDatabase().ExecuteScalar<int>(sql, args);
        }

        /// <summary>
        ///  执行SQL 查询,返回数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public int Select_Count<T>(string sql = "", params object[] args)
        {
            return Count<T>(sql, args);
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
        public List<T> Select<T>(int limit, string sql = "", params object[] args) where T : class
        {
            return GetDatabase().SkipTake<T>(0, limit, sql, args);
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
        public List<T> Select<T>(int limit, int offset, string sql = "", params object[] args) where T : class
        {
            return GetDatabase().SkipTake<T>(offset, limit, sql, args);
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
        public List<T> SelectPage<T>(int page, int itemsPerPage, string sql = "", params object[] args) where T : class
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            return GetDatabase().SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);
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
        public Page<T> Page<T>(int page, int itemsPerPage, string sql = "", params object[] args) where T : class
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            return ToPage(GetDatabase().Page<T>(page, itemsPerPage, sql, args));
        }

        #region Obsolete
        /// <summary>
        /// 执行SQL 查询, 返回单个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnSql"></param>
        /// <param name="tableSql"></param>
        /// <param name="whereSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T SQL_FirstOrDefault<T>(string columnSql, string tableSql, string whereSql, params object[] args)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(columnSql)) { throw new ArgumentNullException("columnSql is null."); }
            if (string.IsNullOrWhiteSpace(tableSql)) { throw new ArgumentNullException("tableSql is null."); }

            columnSql = RemoveStart(columnSql, "SELECT ");
            tableSql = RemoveStart(tableSql, "FROM ");
            whereSql = RemoveStart(whereSql, "WHERE ");

            var sql = $"SELECT {columnSql} FROM {tableSql} WHERE {whereSql}";

            return GetDatabase().Query<T>(sql, args).FirstOrDefault()!;
        }

        /// <summary>
        /// 执行SQL 查询, 返回单个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnSql"></param>
        /// <param name="tableSql"></param>
        /// <param name="whereSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T SQL_FirstOrDefault<T>(string columnSql, string tableSql, string whereSql, SqlParameterCollection args)
            where T : class
        {
            return SQL_FirstOrDefault<T>(columnSql, tableSql, whereSql, args.ToArray());
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
        public List<T> SQL_Select<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(columnSql)) { throw new ArgumentNullException("columnSql is null."); }
            if (string.IsNullOrWhiteSpace(tableSql)) { throw new ArgumentNullException("tableSql is null."); }
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            columnSql = RemoveStart(columnSql, "SELECT ");
            tableSql = RemoveStart(tableSql, "FROM ");
            orderSql = RemoveStart(orderSql, "ORDER BY ");
            whereSql = RemoveStart(whereSql, "WHERE ");

            var sql = BuildSelectSql(columnSql, tableSql, orderSql, whereSql);

            return GetDatabase().SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);
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
        public List<T> SQL_Select<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, SqlParameterCollection args)
            where T : class
        {
            return SQL_Select<T>(page, itemsPerPage, columnSql, tableSql, orderSql, whereSql, args.ToArray());
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">每页数量</param>
        /// <param name="columnSql">查询列 SQL语句</param>
        /// <param name="tableSql">TABLE SQL语句</param>
        /// <param name="orderSql">ORDER BY SQL语句</param>
        /// <param name="whereSql">WHERE SQL语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public List<T> SQL_Select<T>(int limit, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(columnSql)) { throw new ArgumentNullException("columnSql is null."); }
            if (string.IsNullOrWhiteSpace(tableSql)) { throw new ArgumentNullException("tableSql is null."); }
            if (limit <= 0) { limit = 20; }

            columnSql = RemoveStart(columnSql, "SELECT ");
            tableSql = RemoveStart(tableSql, "FROM ");
            orderSql = RemoveStart(orderSql, "ORDER BY ");
            whereSql = RemoveStart(whereSql, "WHERE ");

            var sql = BuildSelectSql(columnSql, tableSql, orderSql, whereSql);

            return GetDatabase().SkipTake<T>(0, limit, sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">每页数量</param>
        /// <param name="columnSql">查询列 SQL语句</param>
        /// <param name="tableSql">TABLE SQL语句</param>
        /// <param name="orderSql">ORDER BY SQL语句</param>
        /// <param name="whereSql">WHERE SQL语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public List<T> SQL_Select<T>(int limit, string columnSql, string tableSql, string orderSql, string whereSql, SqlParameterCollection args)
            where T : class
        {
            return SQL_Select<T>(limit, columnSql, tableSql, orderSql, whereSql, args.ToArray());
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnSql">查询列 SQL语句</param>
        /// <param name="tableSql">TABLE SQL语句</param>
        /// <param name="orderSql">ORDER BY SQL语句</param>
        /// <param name="whereSql">WHERE SQL语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public List<T> SQL_Select<T>(string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(columnSql)) { throw new ArgumentNullException("columnSql is null."); }
            if (string.IsNullOrWhiteSpace(tableSql)) { throw new ArgumentNullException("tableSql is null."); }

            columnSql = RemoveStart(columnSql, "SELECT ");
            tableSql = RemoveStart(tableSql, "FROM ");
            orderSql = RemoveStart(orderSql, "ORDER BY ");
            whereSql = RemoveStart(whereSql, "WHERE ");

            var sql = BuildSelectSql(columnSql, tableSql, orderSql, whereSql);

            return GetDatabase().Query<T>(sql, args).ToList();
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnSql">查询列 SQL语句</param>
        /// <param name="tableSql">TABLE SQL语句</param>
        /// <param name="orderSql">ORDER BY SQL语句</param>
        /// <param name="whereSql">WHERE SQL语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public List<T> SQL_Select<T>(string columnSql, string tableSql, string orderSql, string whereSql, SqlParameterCollection args)
            where T : class
        {
            return SQL_Select<T>(columnSql, tableSql, orderSql, whereSql, args.ToArray());
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
        public Page<T> SQL_Page<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
            where T : class
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

            var sql = BuildSelectSql(columnSql, tableSql, orderSql, whereSql);

            var db = GetDatabase();
            int total = db.ExecuteScalar<int>(countSql, args);
            var items = db.SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);

            return new Page<T> {
                CurrentPage = page,
                PageSize = itemsPerPage,
                TotalItems = total,
                Items = items,
            };
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
        public Page<T> SQL_Page<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, SqlParameterCollection args)
            where T : class
        {
            return SQL_Page<T>(page, itemsPerPage, columnSql, tableSql, orderSql, whereSql, args.ToArray());
        }
        #endregion

        private string BuildSelectSql(string columnSql, string tableSql, string orderSql, string whereSql)
        {
            var sql = $"SELECT {columnSql} FROM {tableSql}";
            if (string.IsNullOrWhiteSpace(whereSql) == false) {
                sql += $" WHERE {whereSql}";
            }
            if (string.IsNullOrWhiteSpace(orderSql) == false) {
                sql += $" ORDER BY {orderSql}";
            }
            return sql;
        }

        #endregion Select Page Select

        #region FirstOrDefault

        /// <summary>
        /// 获取第一个类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public T FirstOrDefault<T>(string sql = "", params object[] args)
        {
            if (_sql_firstWithLimit1 == false) {
                return GetDatabase().FirstOrDefault<T>(sql, args)!;
            }
            return GetDatabase().SkipTake<T>(0, 1, sql, args).FirstOrDefault()!;
        }

        #endregion FirstOrDefault

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
                var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
                foreach (var item in list) {
                    DefaultValue.SetDefaultValue<T>(item, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew, pd);
                }
            }

            GetDatabase().InsertBatch(list);
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
                var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
                DefaultValue.SetDefaultValue<T>(poco, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew, pd);
            }

            var obj = GetDatabase().Insert(poco);
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
            int r = GetDatabase().Update(poco);
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

            var t = GetDatabase().Delete(poco);

            return t;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public int Delete<T>(string sql, params object[] args) where T : class
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");

            var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
            var table = GetDatabase().DatabaseType.EscapeTableName(pd.TableInfo.TableName);

            return GetDatabase().Execute($"DELETE FROM {table} {sql}", args);
        }

        /// <summary>
        /// 根据ID 删除表数据, 注： 单独从delete方法，防止出错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public int DeleteById<T>(object primaryKey) where T : class
        {
            return GetDatabase().Delete<T>(primaryKey);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="poco"></param>
        public void Save<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
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

            if (sql.StartsWith("UPDATE ", StringComparison.CurrentCultureIgnoreCase)) {
                return GetDatabase().Execute(sql, args);
            }
            var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
            var table = GetDatabase().DatabaseType.EscapeTableName(pd.TableInfo.TableName);
            return GetDatabase().Execute($"UPDATE {table} {sql}", args);
        }

        #endregion Object  Insert Update Delete DeleteById Save

        internal string _databaseName;
        /// <summary>
        /// 切换数据库
        /// </summary>
        /// <param name="databaseName"></param>
        public void ChangeDatabase(string databaseName)
        {
            _databaseName = databaseName;
        }

        /// <summary>
        /// 获取动态表名，适合绑定数据表列名
        /// <para>var so = helper.GetTableName(typeof(DbSaleOrder), "so");</para>
        /// <para>var select = $"select {so.Code} from {so} where {so.Id}='123'";</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="asName"></param>
        /// <returns></returns>
        public dynamic GetTableName(Type type, string asName = null)
        {
            var pd = GetDatabase().PocoDataFactory.ForType(type);
            return new TableName(pd, GetDatabase().DatabaseType, asName);
        }

        /// <summary>
        /// 获取动态表名，适合绑定数据表列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asName"></param>
        /// <returns></returns>
        public TableName<T> GetTableName<T>(string asName = null) where T : class, new()
        {
            var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
            return new TableName<T>(pd, GetDatabase().DatabaseType, asName);
        }
    }
}
