using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.Gadget.Internals;
using ToolGood.ReadyGo3.PetaPoco.Core;
#if !NET40

namespace ToolGood.ReadyGo3
{
    partial class SqlHelper
    {
        internal Task<T> RunAsync<T>(string sql, object[] args, Func<Task<T>> func, params string[] methodtags)
        {
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
            if (_usedCacheServiceOnce) {
                return RunAsync(sql, args, () => {
                    return getDatabase().ExecuteAsync(sql, args);
                }, "Execute");
            }
            return getDatabase().ExecuteAsync(sql, args);
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
            if (_usedCacheServiceOnce) {
                return RunAsync(sql, args, () => {
                    return getDatabase().ExecuteScalarAsync<T>(sql, args);
                }, "ExecuteScalar");
            }
            return getDatabase().ExecuteScalarAsync<T>(sql, args);
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
            if (_usedCacheServiceOnce) {
                return RunAsync(sql, args, () => {
                    return getDatabase().ExecuteDataTableAsync(sql, args);
                }, "ExecuteDataTable");
            }
            return getDatabase().ExecuteDataTableAsync(sql, args);

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
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键值</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync<T>(object primaryKey)
        {
            var pd = PocoData.ForType(typeof(T));
            var table = _provider.EscapeSqlIdentifier(pd.TableInfo.TableName);
            var pk = _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
            var sql = $"SELECT COUNT(*) FROM {table} WHERE {pk}=@0";

            var args = new object[] { primaryKey };
            if (_usedCacheServiceOnce) {
                return await RunAsync(sql, args, async () => {
                    return await getDatabase().ExecuteScalarAsync<int>(sql, args) > 0;
                }, "Count");
            }
            return await getDatabase().ExecuteScalarAsync<int>(sql, args) > 0;
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
                var table = _provider.EscapeSqlIdentifier(pd.TableInfo.TableName);
                sql = formatSql(sql);
                sql = $"SELECT COUNT(*) FROM {table} {sql}";
            }

            if (_usedCacheServiceOnce) {
                return RunAsync(sql, args, () => {
                    return getDatabase().ExecuteScalarAsync<int>(sql, args);
                }, "Count");
            }
            return getDatabase().ExecuteScalarAsync<int>(sql, args);
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
            if (_usedCacheServiceOnce) {
                return await RunAsync(sql, args, async () => {
                    return (await getDatabase().QueryAsync<T>(sql, args)).ToList();
                }, "Select");
            }
            return (await getDatabase().QueryAsync<T>(sql, args)).ToList();
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
            if (_usedCacheServiceOnce) {
                return await RunAsync(sql, args, async () => {
                    return (await getDatabase().QueryAsync<T>(0, limit, sql, args)).ToList();
                }, "Select", limit.ToString());
            }
            return (await getDatabase().QueryAsync<T>(0, limit, sql, args)).ToList();
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
            if (_usedCacheServiceOnce) {
                return await RunAsync<List<T>>(sql, args, async () => {
                    return (await getDatabase().QueryAsync<T>(offset, limit, sql, args)).ToList();
                }, "Select", offset.ToString(), limit.ToString());
            }
            return (await getDatabase().QueryAsync<T>(offset, limit, sql, args)).ToList();
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
            sql = formatSql(sql);
            if (_usedCacheServiceOnce) {
                return RunAsync(sql, args, () => {
                    return getDatabase().PageAsync<T>(page, itemsPerPage, sql, args);
                }, "Page", page.ToString(), itemsPerPage.ToString());
            }
            return getDatabase().PageAsync<T>(page, itemsPerPage, sql, args);
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
            if (_sql_singleWithLimit2 == false) { return await _SingleAsync<T>(sql, args); }
            sql = formatSql(sql);
            if (_usedCacheServiceOnce) {
                return await RunAsync<T>(sql, args, async () => {
                    return (await getDatabase().QueryAsync<T>(0, 2, sql, args)).Single();
                }, "Single");
            }
            return (await getDatabase().QueryAsync<T>(0, 2, sql, args)).Single();
        }
        internal async Task<T> _SingleAsync<T>(string sql = "", params object[] args)
        {
            sql = formatSql(sql);
            if (_usedCacheServiceOnce) {
                return await RunAsync<T>(sql, args, async () => {
                    return (await getDatabase().QueryAsync<T>(sql, args)).Single();
                }, "Single");
            }
            return (await getDatabase().QueryAsync<T>(sql, args)).Single();
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
            if (_sql_singleWithLimit2 == false) { return await _SingleOrDefaultAsync<T>(sql, args); }

            sql = formatSql(sql);
            if (_usedCacheServiceOnce) {
                return await RunAsync<T>(sql, args, async () => {
                    return (await getDatabase().QueryAsync<T>(0, 2, sql, args)).SingleOrDefault();
                }, "SingleOrDefault");
            }
            return (await getDatabase().QueryAsync<T>(0, 2, sql, args)).SingleOrDefault();
        }
        internal async Task<T> _SingleOrDefaultAsync<T>(string sql = "", params object[] args)
        {
            sql = formatSql(sql);
            if (_usedCacheServiceOnce) {
                return await RunAsync<T>(sql, args, async () => {
                    return (await getDatabase().QueryAsync<T>(sql, args)).SingleOrDefault();
                }, "SingleOrDefault");
            }
            return (await getDatabase().QueryAsync<T>(sql, args)).SingleOrDefault();
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
            if (_sql_firstWithLimit1 == false) { return await _FirstAsync<T>(sql, args); }

            sql = formatSql(sql);
            if (_usedCacheServiceOnce) {
                return await RunAsync<T>(sql, args, async () => {
                    return (await getDatabase().QueryAsync<T>(0, 1, sql, args)).First();
                }, "First");
            }
            return (await getDatabase().QueryAsync<T>(0, 1, sql, args)).First();
        }
        internal async Task<T> _FirstAsync<T>(string sql = "", params object[] args)
        {
            sql = formatSql(sql);
            if (_usedCacheServiceOnce) {
                return await RunAsync<T>(sql, args, async () => {
                    return (await getDatabase().QueryAsync<T>(sql, args)).First();
                }, "First");
            }
            return (await getDatabase().QueryAsync<T>(sql, args)).First();
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
            if (_sql_firstWithLimit1 == false) { return await _FirstOrDefaultAsync<T>(sql, args); }

            sql = formatSql(sql);
            if (_usedCacheServiceOnce) {
                return await RunAsync<T>(sql, args, async () => {
                    return (await getDatabase().QueryAsync<T>(0, 1, sql, args)).FirstOrDefault();
                }, "FirstOrDefault");
            }
            return (await getDatabase().QueryAsync<T>(0, 1, sql, args)).FirstOrDefault();
        }


        internal async Task<T> _FirstOrDefaultAsync<T>(string sql = "", params object[] args)
        {
            sql = formatSql(sql);
            if (_usedCacheServiceOnce) {
                return await RunAsync<T>(sql, args, async () => {
                    return (await getDatabase().QueryAsync<T>(sql, args)).FirstOrDefault();
                }, "FirstOrDefault");
            }
            return (await getDatabase().QueryAsync<T>(sql, args)).FirstOrDefault();
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

            await getDatabase().InsertAsync(list);
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

            var obj = await getDatabase().InsertAsync(poco);
            _Events.OnAfterInsert(poco);
            return obj;
        }


        internal Task<object> InsertAsync(string sql, string primaryKeyName)
        {
            return getDatabase().ExecuteInsertAsync(sql, primaryKeyName);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync<T>(T poco) where T : class
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            if (_Events.OnBeforeUpdate(poco)) return -1;

            int r = await getDatabase().UpdateAsync(poco);

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

            var t = await getDatabase().DeleteAsync(poco);

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
            return getDatabase().DeleteAsync<T>(sql, args);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public Task<int> DeleteByIdAsync<T>(object primaryKey)
        {
            return getDatabase().DeleteAsync<T>(primaryKey);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="poco"></param>
        public Task SaveAsync(object poco)
        {
            if (poco == null) throw new ArgumentNullException("poco is null");
            return getDatabase().SaveAsync(poco);
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
            return getDatabase().UpdateAsync<T>(sql, args);
        }

        #endregion Object  Insert Update Delete DeleteById Save

    }
}
#endif