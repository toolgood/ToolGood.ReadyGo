using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.Gadget.Internals;
using ToolGood.ReadyGo3.Internals;
using ToolGood.ReadyGo3.PetaPoco.Core;

#if !NET40

namespace ToolGood.ReadyGo3
{
    partial class SqlHelper
    {
        #region UseCancellationToken
        private CancellationToken _token = CancellationToken.None;

        /// <summary>
        /// 使用 CancellationToken
        /// </summary>
        /// <param name="token"></param>
        public void UseCancellationToken(CancellationToken token)
        {
            this._token = token;
        }
        #endregion

        #region Execute ExecuteScalar ExecuteDataTable ExecuteDataSet Exists

        /// <summary>
        /// 执行 SQL 语句，并返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回受影响的行数</returns>
        public Task<int> ExecuteAsync(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            sql = formatSql(sql);
            return GetDatabase().ExecuteAsync(sql, args);
        }

        /// <summary>
        /// 执行SQL 查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回查询所返回的结果集中第一行的第一列。忽略额外的列或行</returns>
        public Task<T> ExecuteScalarAsync<T>(string sql = "", params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            sql = formatSql(sql);
            return GetDatabase().ExecuteScalarAsync<T>(sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回 DataTable</returns>
        public Task<DataTable> ExecuteDataTableAsync(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            sql = formatSql(sql);
            return GetDatabase().ExecuteDataTableAsync(sql, args);

        }



        /// <summary>
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync<T>(string sql, params object[] args)
        {
            sql = formatSql(sql);
            return await CountAsync<T>(sql, args) > 0;
        }

        /// <summary>
        ///  执行SQL 查询,返回数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public Task<int> CountAsync<T>(string sql = "", params object[] args)
        {
            sql = sql.Trim();
            if (sql.StartsWith("SELECT ", StringComparison.CurrentCultureIgnoreCase) == false) {
                var pd = PocoData.ForType(typeof(T));
                var table = _provider.GetTableName(pd, _tableNameManager);
                sql = formatSql(sql);
                sql = $"SELECT COUNT(*) FROM {table} {sql}";
            }
            return GetDatabase().ExecuteScalarAsync<int>(sql, args);
        }

        /// <summary>
        /// 执行SQL 查询,返回数量
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<int> CountAsync(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            return GetDatabase().ExecuteScalarAsync<int>(sql, args);
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
        public async Task<List<T>> SelectAsync<T>(string sql = "", params object[] args)
        {
            sql = formatSql(sql);
            return (await GetDatabase().QueryAsync<T>(sql, args)).ToList();
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync<T>(long limit, string sql = "", params object[] args)
        {
            sql = formatSql(sql);
            return (await GetDatabase().QueryAsync<T>(0, limit, sql, args)).ToList();
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
        public async Task<List<T>> SelectAsync<T>(long limit, long offset, string sql = "", params object[] args)
        {
            sql = formatSql(sql);
            return (await GetDatabase().QueryAsync<T>(offset, limit, sql, args)).ToList();
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
        public async Task<List<T>> SelectPageAsync<T>(long page, long itemsPerPage, string sql = "", params object[] args)
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            sql = formatSql(sql);
            return (await GetDatabase().QueryAsync<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args)).ToList();
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
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sql = "", params object[] args)
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            sql = formatSql(sql);
            return GetDatabase().PageAsync<T>(page, itemsPerPage, sql, args);
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
        public async Task<List<T>> SelectSqlAsync<T>(long page, long itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
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
            sql = formatSql(sql);
            return (await GetDatabase().QueryAsync<T>(sql, args)).ToList();
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
        public Task<Page<T>> PageSqlAsync<T>(long page, long itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
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
            countSql = formatSql(countSql);

            var sql = _provider.CreateSql((int)itemsPerPage, (int)((Math.Max(0, page - 1)) * itemsPerPage), columnSql, tableSql, orderSql, whereSql);
            sql = formatSql(sql);

            return GetDatabase().PageSqlAsync<T>(page, itemsPerPage, sql, countSql, args);
        }

        #endregion Select Page Select

        #region Single SingleOrDefault First FirstOrDefault

        /// <summary>
        /// 获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键名</param>
        /// <returns></returns>
        public Task<T> SingleByIdAsync<T>(object primaryKey)
        {
            var pd = PocoData.ForType(typeof(T));
            var pk = _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
            var sql = $"WHERE {pk}=@0";

            return SingleAsync<T>(sql, primaryKey);
        }

        /// <summary>
        /// 获取唯一一个类型，若数量大于1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键名</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultByIdAsync<T>(object primaryKey)
        {
            var pd = PocoData.ForType(typeof(T));
            var pk = _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
            var sql = $"WHERE {pk}=@0";
            return SingleOrDefaultAsync<T>(sql, primaryKey);
        }

        /// <summary>
        ///获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<T> SingleAsync<T>(string sql = "", params object[] args)
        {
            sql = formatSql(sql);
            if (_sql_singleWithLimit2 == false) {
                return (await GetDatabase().QueryAsync<T>(sql, args)).Single();
            }
            return (await GetDatabase().QueryAsync<T>(0, 2, sql, args)).Single();
        }


        /// <summary>
        /// 获取唯一一个类型，若数量大于1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<T> SingleOrDefaultAsync<T>(string sql = "", params object[] args)
        {
            sql = formatSql(sql);
            if (_sql_singleWithLimit2 == false) {
                return (await GetDatabase().QueryAsync<T>(sql, args)).SingleOrDefault();
            }
            return (await GetDatabase().QueryAsync<T>(0, 2, sql, args)).SingleOrDefault();
        }


        /// <summary>
        /// 获取第一个类型，若数量为0，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<T> FirstAsync<T>(string sql = "", params object[] args)
        {
            sql = formatSql(sql);
            if (_sql_firstWithLimit1 == false) {
                return (await GetDatabase().QueryAsync<T>(sql, args)).First();
            }
            return (await GetDatabase().QueryAsync<T>(0, 1, sql, args)).First();
        }

        /// <summary>
        /// 获取第一个类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync<T>(string sql = "", params object[] args)
        {
            sql = formatSql(sql);
            if (_sql_firstWithLimit1 == false) {
                return (await GetDatabase().QueryAsync<T>(sql, args)).FirstOrDefault();
            }
            return (await GetDatabase().QueryAsync<T>(0, 1, sql, args)).FirstOrDefault();
        }

        #endregion Single SingleOrDefault First FirstOrDefault

        #region Object  Insert Update Delete DeleteById Save

        /// <summary>
        /// 插入集合，不返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public async Task InsertListAsync<T>(List<T> list) where T : class
        {
            if (list == null) throw new ArgumentNullException("list is null.");
            if (list.Count == 0) return;
            if (_setDateTimeDefaultNow || _setStringDefaultNotNull || _setGuidDefaultNew) {
                foreach (var item in list) {
                    DefaultValue.SetDefaultValue<T>(item, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew);
                }
            }
            if (_Events.OnBeforeInsert(list)) return;

            await GetDatabase().InsertAsync(list);
            _Events.OnAfterInsert(list);
        }

        /// <summary>
        /// 插入，支持主键自动获取。
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public async Task<object> InsertAsync<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (poco is IList) throw new ArgumentException("poco is a list type, use InsertList methon .");

            if (_setDateTimeDefaultNow || _setStringDefaultNotNull || _setGuidDefaultNew) {
                DefaultValue.SetDefaultValue<T>(poco, _setStringDefaultNotNull, _setDateTimeDefaultNow, _setGuidDefaultNew);
            }
            if (_Events.OnBeforeInsert(poco)) return null;

            var obj = await GetDatabase().InsertAsync(poco);
            _Events.OnAfterInsert(poco);
            return obj;
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (poco is IUpdateChange) {
                var pd = PocoData.ForType(typeof(T));
                StringBuilder stringBuilder = new StringBuilder();
                ObjectToSql(stringBuilder, poco, ",", new string[] { pd.TableInfo.PrimaryKey });
                if (stringBuilder.Length == 0) { return 0; }
                stringBuilder.Insert(0, "SET ");
                object primaryKeyValue = null;
                foreach (var i in pd.Columns) {
                    if (i.Value.ResultColumn) continue;
                    if (string.Compare(i.Value.ColumnName, pd.TableInfo.PrimaryKey, true) == 0) {
                        if (primaryKeyValue == null) primaryKeyValue = i.Value.GetValue(poco);
                    }
                }
                stringBuilder.Append($" WHERE [{pd.TableInfo.PrimaryKey}]=@0");
                return await UpdateAsync<T>(stringBuilder.ToString(), primaryKeyValue);
            }

            if (_Events.OnBeforeUpdate(poco)) return -1;

            int r = await GetDatabase().UpdateAsync(poco);

            _Events.OnAfterUpdate(poco);
            return r;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (_Events.OnBeforeDelete(poco)) return -1;

            var t = await GetDatabase().DeleteAsync(poco);

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
        public Task<int> DeleteAsync<T>(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            sql = formatSql(sql);
            return GetDatabase().DeleteAsync<T>(sql, args);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public Task<int> DeleteByIdAsync<T>(object primaryKey)
        {
            return GetDatabase().DeleteAsync<T>(primaryKey);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="poco"></param>
        public async Task SaveAsync<T>(T poco)
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (poco is IUpdateChange) {
                var pd = PocoData.ForType(typeof(T));
                StringBuilder stringBuilder = new StringBuilder();
                ObjectToSql(stringBuilder, poco, ",", new string[] { pd.TableInfo.PrimaryKey });
                if (stringBuilder.Length == 0) { return; }
                stringBuilder.Insert(0, "SET ");
                object primaryKeyValue = null;
                foreach (var i in pd.Columns) {
                    if (i.Value.ResultColumn) continue;
                    if (string.Compare(i.Value.ColumnName, pd.TableInfo.PrimaryKey, true) == 0) {
                        if (primaryKeyValue == null) primaryKeyValue = i.Value.GetValue(poco);
                    }
                }
                stringBuilder.Append($" WHERE [{pd.TableInfo.PrimaryKey}]=@0");
                await UpdateAsync<T>(stringBuilder.ToString(), primaryKeyValue);
                return;
            }
            await GetDatabase().SaveAsync(poco);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        public Task<int> UpdateAsync<T>(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql is empty.");
            sql = formatSql(sql);
            return GetDatabase().UpdateAsync<T>(sql, args);
        }

        #endregion Object  Insert Update Delete DeleteById Save

    }
}
#endif