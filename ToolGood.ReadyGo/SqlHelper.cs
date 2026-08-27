using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using ToolGood.ReadyGo.Gadget;
using ToolGood.ReadyGo.Gadget.Internals;
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
        /// <param name="type">SQL 类型</param>
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

        #endregion 私有方法

        #region UseTransaction
        /// <summary>
        /// 使用事务
        /// </summary>
        /// <returns>事务对象</returns>
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
        /// <typeparam name="T">结果类型</typeparam>
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
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>存在返回 true，否则返回 false</returns>
        public bool Exists<T>(string sql, params object[] args)
        {
            return Count<T>(sql, args) > 0;
        }

        /// <summary>
        ///  执行SQL 查询,返回数量
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>记录数量</returns>
        public int Count<T>(string sql = "", params object[] args)
        {
            sql = (sql ?? "").Trim();
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
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>记录数量</returns>
        public int Select_Count<T>(string sql = "", params object[] args)
        {
            return Count<T>(sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回数量
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>记录数量</returns>
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
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>查询结果集合</returns>
        public List<T> Select<T>(string sql = "", params object[] args)
        {
            return GetDatabase().Query<T>(sql, args).ToList();
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>查询结果集合</returns>
        public List<T> Select<T>(int limit, string sql = "", params object[] args) where T : class
        {
            return GetDatabase().SkipTake<T>(0, limit, sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="offset">跳过</param>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>查询结果集合</returns>
        public List<T> Select<T>(int limit, int offset, string sql = "", params object[] args) where T : class
        {
            return GetDatabase().SkipTake<T>(offset, limit, sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>查询结果集合</returns>
        public List<T> SelectPage<T>(int page, int itemsPerPage, string sql = "", params object[] args) where T : class
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            return GetDatabase().SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>分页结果</returns>
        public Page<T> Page<T>(int page, int itemsPerPage, string sql = "", params object[] args) where T : class
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            return GetDatabase().Page<T>(page, itemsPerPage, sql, args);
        }

        #region SelectOneToMany
        /// <summary>
        /// 一对多查询，将子表数据合并到主表的集合属性中（many 指定主表的子集合属性）
        /// 例如：SelectOneToMany&lt;UserDto&gt;(x =&gt; x.Cars, "select u.*, c.* from Users u inner join Cars c on u.UserId = c.UserId order by u.UserId");
        /// </summary>
        /// <typeparam name="T">主表类型</typeparam>
        /// <param name="many">主表存放子表集合的属性</param>
        /// <param name="sql">SQL 语句，须同时返回主表与子表列</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>合并子表数据后的主表对象集合</returns>
        public List<T> SelectOneToMany<T>(Expression<Func<T, IList>> many, string sql, params object[] args)
        {
            return GetDatabase().FetchOneToMany(many, sql, args);
        }

        /// <summary>
        /// 一对多查询，idFunc 指定主表唯一标识，用于合并行（多表查询须含子表 id 列时使用）
        /// </summary>
        /// <typeparam name="T">主表类型</typeparam>
        /// <param name="many">主表存放子表集合的属性</param>
        /// <param name="idFunc">主表唯一标识选择器</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>合并子表数据后的主表对象集合</returns>
        public List<T> SelectOneToMany<T>(Expression<Func<T, IList>> many, Func<T, object> idFunc, string sql, params object[] args)
        {
            return GetDatabase().FetchOneToMany(many, idFunc, sql, args);
        }

        #endregion SelectOneToMany

        #region SelectMultiple
        /// <summary>
        /// 执行多条 SQL，返回多个结果集。
        /// <para>var (users, addresses) = helper.SelectMultiple&lt;User, Address&gt;("select * from users;select * from addresses;");</para>
        /// <para>var data = helper.SelectMultiple&lt;User, Address&gt;(sql);</para>
        /// <para>var users = data.Item1; var addresses = data.Item2;</para>
        /// </summary>
        /// <typeparam name="T1">第一个结果集类型</typeparam>
        /// <typeparam name="T2">第二个结果集类型</typeparam>
        /// <param name="sql">SQL 语句，多个查询以分号分隔</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>两个结果集组成的元组</returns>
        public (List<T1>, List<T2>) SelectMultiple<T1, T2>(string sql, params object[] args)
        {
            return GetDatabase().FetchMultiple<T1, T2>(sql, args);
        }

        /// <summary>
        /// 执行多条 SQL，返回三个结果集。
        /// </summary>
        /// <typeparam name="T1">第一个结果集类型</typeparam>
        /// <typeparam name="T2">第二个结果集类型</typeparam>
        /// <typeparam name="T3">第三个结果集类型</typeparam>
        /// <param name="sql">SQL 语句，多个查询以分号分隔</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>三个结果集组成的元组</returns>
        public (List<T1>, List<T2>, List<T3>) SelectMultiple<T1, T2, T3>(string sql, params object[] args)
        {
            return GetDatabase().FetchMultiple<T1, T2, T3>(sql, args);
        }

        /// <summary>
        /// 执行多条 SQL，返回四个结果集。
        /// </summary>
        /// <typeparam name="T1">第一个结果集类型</typeparam>
        /// <typeparam name="T2">第二个结果集类型</typeparam>
        /// <typeparam name="T3">第三个结果集类型</typeparam>
        /// <typeparam name="T4">第四个结果集类型</typeparam>
        /// <param name="sql">SQL 语句，多个查询以分号分隔</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>四个结果集组成的元组</returns>
        public (List<T1>, List<T2>, List<T3>, List<T4>) SelectMultiple<T1, T2, T3, T4>(string sql, params object[] args)
        {
            return GetDatabase().FetchMultiple<T1, T2, T3, T4>(sql, args);
        }

        /// <summary>
        /// 执行多条 SQL，并通过回调组合多个结果集。
        /// <para>var tuple = helper.SelectMultiple&lt;User, Address, Tuple&lt;List&lt;User&gt;, List&lt;Address&gt;&gt;&gt;( (u, a) =&gt; Tuple.Create(u, a), sql);</para>
        /// </summary>
        /// <typeparam name="T1">第一个结果集类型</typeparam>
        /// <typeparam name="T2">第二个结果集类型</typeparam>
        /// <typeparam name="TRet">回调返回类型</typeparam>
        /// <param name="cb">组合回调</param>
        /// <param name="sql">SQL 语句，多个查询以分号分隔</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>经回调组合后的结果</returns>
        public TRet SelectMultiple<T1, T2, TRet>(Func<List<T1>, List<T2>, TRet> cb, string sql, params object[] args)
        {
            return GetDatabase().FetchMultiple(cb, sql, args);
        }

        /// <summary>
        /// 执行多条 SQL，并通过回调组合三个结果集。
        /// </summary>
        /// <typeparam name="T1">第一个结果集类型</typeparam>
        /// <typeparam name="T2">第二个结果集类型</typeparam>
        /// <typeparam name="T3">第三个结果集类型</typeparam>
        /// <typeparam name="TRet">回调返回类型</typeparam>
        /// <param name="cb">组合回调</param>
        /// <param name="sql">SQL 语句，多个查询以分号分隔</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>经回调组合后的结果</returns>
        public TRet SelectMultiple<T1, T2, T3, TRet>(Func<List<T1>, List<T2>, List<T3>, TRet> cb, string sql, params object[] args)
        {
            return GetDatabase().FetchMultiple(cb, sql, args);
        }

        /// <summary>
        /// 执行多条 SQL，并通过回调组合四个结果集。
        /// </summary>
        /// <typeparam name="T1">第一个结果集类型</typeparam>
        /// <typeparam name="T2">第二个结果集类型</typeparam>
        /// <typeparam name="T3">第三个结果集类型</typeparam>
        /// <typeparam name="T4">第四个结果集类型</typeparam>
        /// <typeparam name="TRet">回调返回类型</typeparam>
        /// <param name="cb">组合回调</param>
        /// <param name="sql">SQL 语句，多个查询以分号分隔</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>经回调组合后的结果</returns>
        public TRet SelectMultiple<T1, T2, T3, T4, TRet>(Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> cb, string sql, params object[] args)
        {
            return GetDatabase().FetchMultiple(cb, sql, args);
        }

        #endregion SelectMultiple

        #region Obsolete
        /// <summary>
        /// 执行SQL 查询, 返回单个
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="columnSql">查询列 SQL 语句</param>
        /// <param name="tableSql">表名 SQL 语句</param>
        /// <param name="whereSql">WHERE 条件 SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>查询结果中的第一条记录，无结果时返回 null</returns>
        public T SQL_FirstOrDefault<T>(string columnSql, string tableSql, string whereSql, params object[] args)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(columnSql)) { throw new ArgumentNullException("columnSql is null."); }
            if (string.IsNullOrWhiteSpace(tableSql)) { throw new ArgumentNullException("tableSql is null."); }

            columnSql = RemoveStart(columnSql, "SELECT ");
            tableSql = RemoveStart(tableSql, "FROM ");
            whereSql = RemoveStart(whereSql, "WHERE ");

            var sql = string.IsNullOrWhiteSpace(whereSql)
                ? $"SELECT {columnSql} FROM {tableSql}"
                : $"SELECT {columnSql} FROM {tableSql} WHERE {whereSql}";

            return GetDatabase().Query<T>(sql, args).FirstOrDefault()!;
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="columnSql">查询列 SQL语句</param>
        /// <param name="tableSql">TABLE SQL语句</param>
        /// <param name="orderSql">ORDER BY SQL语句</param>
        /// <param name="whereSql">WHERE SQL语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>查询结果集合</returns>
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
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="limit">每页数量</param>
        /// <param name="columnSql">查询列 SQL语句</param>
        /// <param name="tableSql">TABLE SQL语句</param>
        /// <param name="orderSql">ORDER BY SQL语句</param>
        /// <param name="whereSql">WHERE SQL语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>查询结果集合</returns>
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
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="columnSql">查询列 SQL语句</param>
        /// <param name="tableSql">TABLE SQL语句</param>
        /// <param name="orderSql">ORDER BY SQL语句</param>
        /// <param name="whereSql">WHERE SQL语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>查询结果集合</returns>
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
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="columnSql">查询列 SQL语句</param>
        /// <param name="tableSql">TABLE SQL语句</param>
        /// <param name="orderSql">ORDER BY SQL语句</param>
        /// <param name="whereSql">WHERE SQL语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>分页结果</returns>
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
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>查询结果中的第一条记录，无结果时返回 null</returns>
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
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="list">要插入的实体集合</param>
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
        /// 批量更新，每个对象按主键更新全部列。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="list">要更新的实体集合</param>
        /// <returns>受影响的总行数</returns>
        public int UpdateList<T>(List<T> list) where T : class
        {
            if (list == null) throw new ArgumentNullException("list is null.");
            if (list.Count == 0) return 0;

            return GetDatabase().UpdateBatch(list.Select(x => UpdateBatch.For(x)));
        }

        /// <summary>
        /// 批量更新，仅更新快照中发生变更的列。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="list">要更新的实体集合</param>
        /// <param name="snapshots">与 list 一一对应的快照集合（来自 StartSnapshot）</param>
        /// <returns>受影响的总行数</returns>
        public int UpdateList<T>(List<T> list, List<Snapshot<T>> snapshots) where T : class
        {
            if (list == null) throw new ArgumentNullException("list is null.");
            if (snapshots == null) throw new ArgumentNullException("snapshots is null.");
            if (list.Count == 0) return 0;
            if (list.Count != snapshots.Count) throw new ArgumentException("list.Count must equal snapshots.Count.");

            var batches = new List<UpdateBatch<T>>(list.Count);
            for (int i = 0; i < list.Count; i++) {
                batches.Add(UpdateBatch.For(list[i], snapshots[i]));
            }
            return GetDatabase().UpdateBatch(batches);
        }

        /// <summary>
        /// 插入，支持主键自动获取。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="poco">对象</param>
        /// <returns>插入后自动生成的主键值</returns>
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
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="poco">对象</param>
        /// <returns>受影响的行数</returns>
        public int Update<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            int r = GetDatabase().Update(poco);
            return r;
        }

        /// <summary>
        /// 开始跟踪对象快照，之后对对象属性的修改将被记录。
        /// <para>var snapshot = helper.StartSnapshot(user);</para>
        /// <para>user.Name = "Bobby";</para>
        /// <para>helper.Update(user, snapshot.UpdatedColumns()); // 仅更新变更的列</para>
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="poco">要跟踪的对象</param>
        /// <returns>快照</returns>
        public Snapshot<T> StartSnapshot<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            return Snapshotter.StartSnapshot(GetDatabase(), poco);
        }

        /// <summary>
        /// 更新指定列
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="poco">对象</param>
        /// <param name="columns">要更新的列名集合</param>
        /// <returns>受影响的行数</returns>
        public int Update<T>(T poco, IEnumerable<string> columns) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            return GetDatabase().Update(poco, columns);
        }

        /// <summary>
        /// 更新，仅更新快照中发生变更的列
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="poco">对象</param>
        /// <param name="snapshot">快照</param>
        /// <returns>受影响的行数</returns>
        public int Update<T>(T poco, Snapshot<T> snapshot) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (snapshot == null) throw new ArgumentNullException("snapshot is null");
            return GetDatabase().Update(poco, snapshot.UpdatedColumns());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="poco">对象</param>
        /// <returns>受影响的行数</returns>
        public int Delete<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");

            var t = GetDatabase().Delete(poco);

            return t;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>受影响的行数</returns>
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
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns>受影响的行数</returns>
        public int DeleteById<T>(object primaryKey) where T : class
        {
            return GetDatabase().Delete<T>(primaryKey);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="poco">对象</param>
        public void Save<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            GetDatabase().Save(poco);
        }

        /// <summary>
        /// 批量保存：新对象（主键为默认值）执行批量插入，已存在对象执行批量更新。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="list">要保存的实体集合</param>
        public void SaveList<T>(List<T> list) where T : class
        {
            if (list == null) throw new ArgumentNullException("list is null.");
            if (list.Count == 0) return;

            var toInsert = new List<T>();
            var toUpdate = new List<T>();
            foreach (var item in list) {
                if (GetDatabase().IsNew(item)) {
                    toInsert.Add(item);
                } else {
                    toUpdate.Add(item);
                }
            }
            if (toInsert.Count > 0) GetDatabase().InsertBatch(toInsert);
            if (toUpdate.Count > 0) GetDatabase().UpdateBatch(toUpdate.Select(x => UpdateBatch.For(x)));
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>受影响的行数</returns>
        public int Update<T>(string sql, params object[] args) where T : class
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

        /// <summary>
        /// 获取动态表名，适合绑定数据表列名
        /// <para>var so = helper.GetTableName(typeof(DbSaleOrder), "so");</para>
        /// <para>var select = $"select {so.Code} from {so} where {so.Id}='123'";</para>
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="asName">表别名</param>
        /// <returns>动态表名对象，可通过其成员访问列名</returns>
        public dynamic GetTableName(Type type, string asName = null)
        {
            var pd = GetDatabase().PocoDataFactory.ForType(type);
            return new TableName(pd, GetDatabase().DatabaseType, asName);
        }

        /// <summary>
        /// 获取动态表名，适合绑定数据表列名
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="asName">表别名</param>
        /// <returns>动态表名对象，可通过其成员访问列名</returns>
        public TableName<T> GetTableName<T>(string asName = null) where T : class, new()
        {
            var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
            return new TableName<T>(pd, GetDatabase().DatabaseType, asName);
        }
    }
}
