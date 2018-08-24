using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.Gadget.Caches;
using ToolGood.ReadyGo3.Gadget.Events;
using ToolGood.ReadyGo3.Gadget.Internals;
using ToolGood.ReadyGo3.Gadget.TableManager;
using ToolGood.ReadyGo3.Interfaces;
using ToolGood.ReadyGo3.PetaPoco;

#if !NET40

namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// 异步 SqlHelper
    /// </summary>
    public interface ISqlHelperAsync: ISqlHelperBase
    {
        /// <summary>
        /// 执行SQL 查询,返回数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<int> CountAsync<T>(string sql = "", params object[] args);
        /// <summary>
        /// 执行SQL 查询,返回数量
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<int> CountAsync(string sql, params object[] args);

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<int> DeleteAsync<T>(string sql, params object[] args);
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        Task<int> DeleteAsync<T>(T poco) where T : class;

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        Task<int> DeleteByIdAsync<T>(object primaryKey);

        /// <summary>
        /// 执行 SQL 语句，并返回受影响的行数
        /// </summary>
        /// <param name="sql"> SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string sql, params object[] args);

        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<DataTable> ExecuteDataTableAsync(string sql, params object[] args);

        /// <summary>
        /// 执行SQL 查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回查询所返回的结果集中第一行的第一列。忽略额外的列或行</returns>
        Task<T> ExecuteScalarAsync<T>(string sql = "", params object[] args);

        /// <summary>
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<bool> ExistsAsync<T>(string sql, params object[] args);
        /// <summary>
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键值</param>
        /// <returns></returns>
        Task<bool> ExistsAsync<T>(object primaryKey);
        /// <summary>
        /// 获取第一个类型，若数量为0，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<T> FirstAsync<T>(string sql = "", params object[] args);

        /// <summary>
        /// 获取第一个类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync<T>(string sql = "", params object[] args);
        /// <summary>
        /// 插入，支持主键自动获取。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco"></param>
        /// <returns></returns>
        Task<object> InsertAsync<T>(T poco) where T : class;
        /// <summary>
        /// 插入集合，不返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        Task InsertListAsync<T>(List<T> list) where T : class;

        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sql = "", params object[] args);
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="poco"></param>
        /// <returns></returns>
        Task SaveAsync(object poco);
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">获取个数</param>
        /// <param name="offset">跳过</param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<List<T>> SelectAsync<T>(long limit, long offset, string sql = "", params object[] args);
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">获取个数</param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<List<T>> SelectAsync<T>(long limit, string sql = "", params object[] args);
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<List<T>> SelectAsync<T>(string sql = "", params object[] args);
        /// <summary>
        /// 获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<T> SingleAsync<T>(string sql = "", params object[] args);
        /// <summary>
        /// 获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键名</param>
        /// <returns></returns>
        Task<T> SingleByIdAsync<T>(object primaryKey);
        /// <summary>
        /// 获取唯一一个类型，若数量大于1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<T> SingleOrDefaultAsync<T>(string sql = "", params object[] args);
        /// <summary>
        /// 获取唯一一个类型，若数量大于1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键名</param>
        /// <returns></returns>
        Task<T> SingleOrDefaultByIdAsync<T>(object primaryKey);
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(string sql, params object[] args);
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(T poco) where T : class;
 
    }
}
#endif