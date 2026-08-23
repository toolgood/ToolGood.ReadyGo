using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ToolGood.ReadyGo.Gadget.Internals;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo
{
    public partial class SqlHelper
    {
        #region Execute ExecuteScalar ExecuteDataTable ExecuteDataSet Exists

        /// <summary>
        /// 执行 SQL 语句，并返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回受影响的行数</returns>
        public async Task<int> Execute_Async(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");

            return await GetDatabase().ExecuteAsync(sql, args);
        }

        /// <summary>
        /// 执行SQL 查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回查询所返回的结果集中第一行的第一列。忽略额外的列或行</returns>
        public async Task<T> ExecuteScalar_Async<T>(string sql = "", params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");

            return await GetDatabase().ExecuteScalarAsync<T>(sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回 DataTable</returns>
        public async Task<DataTable> ExecuteDataTable_Async(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");

            return await GetDatabase().ExecuteDataTable_Async(sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<bool> Exists_Async<T>(string sql, params object[] args)
        {
            return await Count_Async<T>(sql, args) > 0;
        }

        /// <summary>
        ///  执行SQL 查询,返回数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<int> Count_Async<T>(string sql = "", params object[] args)
        {
            sql = sql.Trim();
            if (sql.StartsWith("SELECT ", StringComparison.CurrentCultureIgnoreCase) == false) {
                var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
                var table = GetDatabase().DatabaseType.EscapeTableName(pd.TableInfo.TableName);

                sql = $"SELECT COUNT(*) FROM {table} {sql}";
            }
            return await GetDatabase().ExecuteScalarAsync<int>(sql, args);
        }

        /// <summary>
        ///  执行SQL 查询,返回数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<int> Select_Count_Async<T>(string sql = "", params object[] args)
        {
            return await Count_Async<T>(sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回数量
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<int> Count_Async(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            return await GetDatabase().ExecuteScalarAsync<int>(sql, args);
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
        public async Task<List<T>> Select_Async<T>(string sql = "", params object[] args)
        {
            return await GetDatabase().QueryAsync<T>(sql, args).ToListAsync();
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<List<T>> Select_Async<T>(int limit, string sql = "", params object[] args) where T : class
        {
            return await GetDatabase().SkipTakeAsync<T>(0, limit, sql, args);
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
        public async Task<List<T>> Select_Async<T>(int limit, int offset, string sql = "", params object[] args) where T : class
        {
            return await GetDatabase().SkipTakeAsync<T>(offset, limit, sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<List<T>> SelectPage_Async<T>(int page, int itemsPerPage, string sql = "", params object[] args)
             where T : class
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            return await GetDatabase().SkipTakeAsync<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);
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
        public async Task<Page<T>> Page_Async<T>(int page, int itemsPerPage, string sql = "", params object[] args) where T : class
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            return await GetDatabase().PageAsync<T>(page, itemsPerPage, sql, args);
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
        public async Task<T> SQL_FirstOrDefault_Async<T>(string columnSql, string tableSql, string whereSql, params object[] args)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(columnSql)) { throw new ArgumentNullException("columnSql is null."); }
            if (string.IsNullOrWhiteSpace(tableSql)) { throw new ArgumentNullException("tableSql is null."); }

            columnSql = RemoveStart(columnSql, "SELECT ");
            tableSql = RemoveStart(tableSql, "FROM ");
            whereSql = RemoveStart(whereSql, "WHERE ");

            var sql = $"SELECT {columnSql} FROM {tableSql} WHERE {whereSql}";

            return await GetDatabase().QueryAsync<T>(sql, args).FirstOrDefaultAsync();
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
        public async Task<List<T>> SQL_Select_Async<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
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

            return await GetDatabase().SkipTakeAsync<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);
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
        public async Task<List<T>> SQL_Select_Async<T>(int limit, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
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

            return await GetDatabase().SkipTakeAsync<T>(0, limit, sql, args);
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
        public async Task<List<T>> SQL_Select_Async<T>(string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(columnSql)) { throw new ArgumentNullException("columnSql is null."); }
            if (string.IsNullOrWhiteSpace(tableSql)) { throw new ArgumentNullException("tableSql is null."); }

            columnSql = RemoveStart(columnSql, "SELECT ");
            tableSql = RemoveStart(tableSql, "FROM ");
            orderSql = RemoveStart(orderSql, "ORDER BY ");
            whereSql = RemoveStart(whereSql, "WHERE ");

            var sql = BuildSelectSql(columnSql, tableSql, orderSql, whereSql);

            return await GetDatabase().QueryAsync<T>(sql, args).ToListAsync();
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
        public async Task<Page<T>> SQL_Page_Async<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
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
            int total = await db.ExecuteScalarAsync<int>(countSql, args);
            var items = await db.SkipTakeAsync<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);

            return new Page<T> {
                CurrentPage = page,
                PageSize = itemsPerPage,
                TotalItems = total,
                Items = items,
            };
        }

        #endregion

        #endregion Select Page Select

        #region SelectOneToMany

        /// <summary>
        /// 一对多查询，将子表数据合并到主表的集合属性中（many 指定主表的子集合属性）
        /// </summary>
        /// <typeparam name="T">主表类型</typeparam>
        /// <param name="many">主表存放子表集合的属性</param>
        /// <param name="sql">SQL 语句，须同时返回主表与子表列</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<List<T>> SelectOneToMany_Async<T>(Expression<Func<T, IList>> many, string sql, params object[] args)
        {
            var result = new List<T>();
            await foreach (var item in GetDatabase().QueryAsync<T>(default!, many, null, new Sql(sql, args)))
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// 一对多查询，idFunc 指定主表唯一标识，用于合并行
        /// </summary>
        /// <typeparam name="T">主表类型</typeparam>
        /// <param name="many">主表存放子表集合的属性</param>
        /// <param name="idFunc">主表唯一标识选择器</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<List<T>> SelectOneToMany_Async<T>(Expression<Func<T, IList>> many, Func<T, object> idFunc, string sql, params object[] args)
        {
            var result = new List<T>();
            await foreach (var item in GetDatabase().QueryAsync<T>(default!, many, x => new[] { idFunc(x) }, new Sql(sql, args)))
            {
                result.Add(item);
            }
            return result;
        }

        #endregion SelectOneToMany

        #region SelectMultiple

        /// <summary>
        /// 执行多条 SQL，返回多个结果集。
        /// <para>var (users, addresses) = await helper.SelectMultiple_Async&lt;User, Address&gt;("select * from users;select * from addresses;");</para>
        /// <para>var data = await helper.SelectMultiple_Async&lt;User, Address&gt;(sql);</para>
        /// <para>var users = data.Item1; var addresses = data.Item2;</para>
        /// </summary>
        /// <typeparam name="T1">第一个结果集类型</typeparam>
        /// <typeparam name="T2">第二个结果集类型</typeparam>
        /// <param name="sql">SQL 语句，多个查询以分号分隔</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<(List<T1>, List<T2>)> SelectMultiple_Async<T1, T2>(string sql, params object[] args)
        {
            return await GetDatabase().FetchMultipleAsync<T1, T2>(sql, args);
        }

        /// <summary>
        /// 执行多条 SQL，返回三个结果集。
        /// </summary>
        /// <typeparam name="T1">第一个结果集类型</typeparam>
        /// <typeparam name="T2">第二个结果集类型</typeparam>
        /// <typeparam name="T3">第三个结果集类型</typeparam>
        /// <param name="sql">SQL 语句，多个查询以分号分隔</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<(List<T1>, List<T2>, List<T3>)> SelectMultiple_Async<T1, T2, T3>(string sql, params object[] args)
        {
            return await GetDatabase().FetchMultipleAsync<T1, T2, T3>(sql, args);
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
        /// <returns></returns>
        public async Task<(List<T1>, List<T2>, List<T3>, List<T4>)> SelectMultiple_Async<T1, T2, T3, T4>(string sql, params object[] args)
        {
            return await GetDatabase().FetchMultipleAsync<T1, T2, T3, T4>(sql, args);
        }

        /// <summary>
        /// 执行多条 SQL，并通过回调组合多个结果集。
        /// <para>var tuple = await helper.SelectMultiple_Async&lt;User, Address, Tuple&lt;List&lt;User&gt;, List&lt;Address&gt;&gt;&gt;( (u, a) =&gt; Tuple.Create(u, a), sql);</para>
        /// </summary>
        /// <typeparam name="T1">第一个结果集类型</typeparam>
        /// <typeparam name="T2">第二个结果集类型</typeparam>
        /// <typeparam name="TRet">回调返回类型</typeparam>
        /// <param name="cb">组合回调</param>
        /// <param name="sql">SQL 语句，多个查询以分号分隔</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<TRet> SelectMultiple_Async<T1, T2, TRet>(Func<List<T1>, List<T2>, TRet> cb, string sql, params object[] args)
        {
            return await GetDatabase().FetchMultipleAsync(cb, sql, args);
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
        /// <returns></returns>
        public async Task<TRet> SelectMultiple_Async<T1, T2, T3, TRet>(Func<List<T1>, List<T2>, List<T3>, TRet> cb, string sql, params object[] args)
        {
            return await GetDatabase().FetchMultipleAsync(cb, sql, args);
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
        /// <returns></returns>
        public async Task<TRet> SelectMultiple_Async<T1, T2, T3, T4, TRet>(Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> cb, string sql, params object[] args)
        {
            return await GetDatabase().FetchMultipleAsync(cb, sql, args);
        }

        #endregion SelectMultiple

        #region Single SingleOrDefault First FirstOrDefault

        /// <summary>
        /// 获取唯一一个类型，若数量大于1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键名</param>
        /// <returns></returns>
        private async Task<T> SingleOrDefaultById_Async<T>(object primaryKey) where T : class
        {
            return await GetDatabase().SingleOrDefaultByIdAsync<T>(primaryKey);
        }

        /// <summary>
        /// 获取第一个类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<T> FirstOrDefault_Async<T>(string sql = "", params object[] args)
        {
            if (_sql_firstWithLimit1 == false) {
                return await GetDatabase().FirstOrDefaultAsync<T>(sql, args);
            }
            var list = await GetDatabase().SkipTakeAsync<T>(0, 1, sql, args);
            return list.FirstOrDefault();
        }

        #endregion Single SingleOrDefault First FirstOrDefault

        #region Object  Insert Update Delete DeleteById Save

        /// <summary>
        /// 插入集合，不返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public async Task InsertList_Async<T>(List<T> list) where T : class
        {
            if (list == null) throw new ArgumentNullException("list is null.");
            if (list.Count == 0) return;
            if (_setDateTimeDefaultNow || _setStringDefaultNotNull || _setGuidDefaultNew) {
                var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
                foreach (var item in list) {
                    DefaultValue.SetDefaultValue<T>(item, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew, pd);
                }
            }

            await GetDatabase().InsertBatchAsync(list);
        }

        /// <summary>
        /// 插入，支持主键自动获取。
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public async Task<object> Insert_Async<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (poco is IList) throw new ArgumentException("poco is a list type, use InsertList methon .");

            if (_setDateTimeDefaultNow || _setStringDefaultNotNull || _setGuidDefaultNew) {
                var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
                DefaultValue.SetDefaultValue<T>(poco, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew, pd);
            }

            var obj = await GetDatabase().InsertAsync(poco);
            return obj;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public async Task<int> Update_Async<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");

            int r = await GetDatabase().UpdateAsync(poco);

            return r;
        }

        /// <summary>
        /// 更新指定列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco">对象</param>
        /// <param name="columns">要更新的列名集合</param>
        /// <returns></returns>
        public async Task<int> Update_Async<T>(T poco, IEnumerable<string> columns) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            return await GetDatabase().UpdateAsync(poco, columns);
        }

        /// <summary>
        /// 更新，仅更新快照中发生变更的列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco">对象</param>
        /// <param name="snapshot">快照</param>
        /// <returns></returns>
        public async Task<int> Update_Async<T>(T poco, Snapshot<T> snapshot) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (snapshot == null) throw new ArgumentNullException("snapshot is null");
            return await GetDatabase().UpdateAsync(poco, snapshot.UpdatedColumns());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public async Task<int> Delete_Async<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");

            var t = await GetDatabase().DeleteAsync(poco);

            return t;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<int> Delete_Async<T>(string sql, params object[] args) where T : class
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");

            var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
            var table = GetDatabase().DatabaseType.EscapeTableName(pd.TableInfo.TableName);

            return await GetDatabase().ExecuteAsync($"DELETE FROM {table} {sql}", args);
        }

        /// <summary>
        /// 根据ID 删除表数据, 注： 单独从delete方法，防止出错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public async Task<int> DeleteById_Async<T>(object primaryKey) where T : class
        {
            var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
            var table = GetDatabase().DatabaseType.EscapeTableName(pd.TableInfo.TableName);
            var pk = GetDatabase().DatabaseType.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);

            return await GetDatabase().ExecuteAsync($"DELETE FROM {table} WHERE {pk}=@0", new object[] { primaryKey });
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="poco"></param>
        public async Task Save_Async<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            await GetDatabase().SaveAsync(poco);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<int> Update_Async<T>(string sql, params object[] args) where T : class
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");

            if (sql.StartsWith("UPDATE ", StringComparison.CurrentCultureIgnoreCase)) {
                return await GetDatabase().ExecuteAsync(sql, args);
            }
            var pd = GetDatabase().PocoDataFactory.ForType(typeof(T));
            var table = GetDatabase().DatabaseType.EscapeTableName(pd.TableInfo.TableName);
            return await GetDatabase().ExecuteAsync($"UPDATE {table} {sql}", args);
        }

        #endregion Object  Insert Update Delete DeleteById Save
    }
}
