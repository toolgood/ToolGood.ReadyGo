#if !NET40

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.PetaPoco;

namespace ToolGood.ReadyGo3
{
    public partial class SqlHelper : IAsyncSqlHelper
    {

    }
    /// <summary>
    /// 异步
    /// </summary>
    public interface IAsyncSqlHelper
    {
        Transaction UseTransaction();

        #region FirstOrDefault_Async PK
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Async<T>(int condition) where T : class;

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Async<T>(uint condition) where T : class;

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Async<T>(long condition) where T : class;
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Async<T>(ulong condition) where T : class;
        #endregion

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Async<T>(object condition) where T : class;

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<List<T>> Select_Async<T>(int limit, int offset, object condition) where T : class;

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<List<T>> Select_Async<T>(int limit, object condition) where T : class;

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<List<T>> Select_Async<T>(object condition) where T : class;

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<List<T>> SelectPage_Async<T>(int page, int itemsPerPage, object condition)
           where T : class;

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<Page<T>> Page_Async<T>(int page, int itemsPerPage, object condition)
           where T : class;

        /// <summary>
        /// 根据条件更新对象
        /// </summary>
        /// <param name="set"></param>
        /// <param name="condition">条件</param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        Task<int> Update_Async<T>(object set, object condition, IEnumerable<string> ignoreFields = null) where T : class;

        /// <summary>
        /// 根据条件从数据库中删除对象 
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<int> Delete_Async<T>(object condition) where T : class;

        /// <summary>
        /// 根据条件查询个数，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<int> Count_Async<T>(object condition) where T : class;

        /// <summary>
        /// 根据条件是判断否存在，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<bool> Exists_Async<T>(object condition) where T : class;




        #region FirstOrDefault_Async PK

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Table_Async<T>(string table, int condition) where T : class;

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Table_Async<T>(string table, uint condition) where T : class;

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Table_Async<T>(string table, long condition) where T : class;

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Table_Async<T>(string table, ulong condition) where T : class;

        #endregion

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Table_Async<T>(string table, object condition) where T : class;

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<List<T>> Select_Table_Async<T>(string table, int limit, int offset, object condition) where T : class;

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<List<T>> Select_Table_Async<T>(string table, int limit, object condition) where T : class;

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<List<T>> Select_Table_Async<T>(string table, object condition) where T : class;

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<List<T>> SelectPage_Table_Async<T>(string table, int page, int itemsPerPage, object condition)
           where T : class;


        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<Page<T>> Page_Table_Async<T>(string table, int page, int itemsPerPage, object condition)
           where T : class;

        /// <summary>
        /// 更新表
        /// </summary>
        /// <param name="table"></param>
        /// <param name="set"></param>
        /// <param name="condition"></param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        Task<int> Update_Table_Async(string table, object set, object condition, IEnumerable<string> ignoreFields = null);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<int> Delete_Table_Async(string table, object condition);


        /// <summary>
        /// 根据条件查询个数，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<int> Count_Table_Async(string table, object condition);

        /// <summary>
        /// 根据条件是判断否存在，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<bool> Exists_Table_Async(string table, object condition);



        #region Execute ExecuteScalar ExecuteDataTable ExecuteDataSet Exists

        /// <summary>
        /// 执行 SQL 语句，并返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回受影响的行数</returns>
        Task<int> Execute_Async(string sql, params object[] args);
        /// <summary>
        /// 执行SQL 查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回查询所返回的结果集中第一行的第一列。忽略额外的列或行</returns>
        Task<T> ExecuteScalar_Async<T>(string sql = "", params object[] args);

        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回 Data_Table</returns>
        Task<DataTable> ExecuteDataTable_Async(string sql, params object[] args);



        /// <summary>
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<bool> Exists_Async<T>(string sql, params object[] args);
        /// <summary>
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<bool> Exists_Table_Async(string table, string sql, params object[] args);
        /// <summary>
        ///  执行SQL 查询,返回数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<int> Count_Async<T>(string sql = "", params object[] args);
        /// <summary>
        /// 执行SQL 查询,返回数量
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<int> Count_Async(string sql, params object[] args);
        /// <summary>
        /// 执行SQL 查询,返回数量
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<int> Count_Table_Async(string table, string sql = "", params object[] args);

        #endregion Execute ExecuteScalar ExecuteDataTable ExecuteDataSet Exists

        #region Select Page Select
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<List<T>> Select_Async<T>(string sql = "", params object[] args);
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<List<T>> Select_Table_Async<T>(string table, string sql = "", params object[] args) where T : class;

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<List<T>> Select_Async<T>(int limit, string sql = "", params object[] args) where T : class;
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<List<T>> Select_Table_Async<T>(string table, int limit, string sql = "", params object[] args) where T : class;

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset">跳过</param>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<List<T>> Select_Async<T>(int limit, int offset, string sql = "", params object[] args) where T : class;
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="offset">跳过</param>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<List<T>> Select_Table_Async<T>(string table, int limit, int offset, string sql = "", params object[] args) where T : class;

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<List<T>> SelectPage_Async<T>(int page, int itemsPerPage, string sql = "", params object[] args)
           where T : class;
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<List<T>> SelectPage_Table_Async<T>(string table, int page, int itemsPerPage, string sql = "", params object[] args)
           where T : class;


        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<Page<T>> Page_Async<T>(int page, int itemsPerPage, string sql = "", params object[] args) where T : class;
        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<Page<T>> Page_Table_Async<T>(string table, int page, int itemsPerPage, string sql = "", params object[] args) where T : class;

        /// <summary>
        /// 执行SQL 查询, 返回单个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnSql"></param>
        /// <param name="tableSql"></param>
        /// <param name="whereSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<T> SQL_FirstOrDefault_Async<T>(string columnSql, string tableSql, string whereSql, params object[] args)
          where T : class;

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
        Task<List<T>> SQL_Select_Async<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
          where T : class;

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
        Task<List<T>> SQL_Select_Async<T>(int limit, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
          where T : class;

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="columnSql">查询列 SQL语句</param>
        /// <param name="tableSql">TABLE SQL语句</param>
        /// <param name="orderSql">ORDER BY SQL语句</param>
        /// <param name="whereSql">WHERE SQL语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<List<T>> SQL_Select_Async<T>(string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
          where T : class;



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
        Task<Page<T>> SQL_Page_Async<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
           where T : class;

        #endregion Select Page Select

        #region Single SingleOrDefault First FirstOrDefault

        /// <summary>
        /// 获取第一个类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Async<T>(string sql = "", params object[] args);
        /// <summary>
        /// 获取第一个类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<T> FirstOrDefault_Table_Async<T>(string table, string sql = "", params object[] args) where T : class;

        #endregion Single SingleOrDefault First FirstOrDefault

        #region Object  Insert Update Delete DeleteById Save

        /// <summary>
        /// 插入集合，不返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        Task InsertList_Async<T>(List<T> list) where T : class;
        /// <summary>
        /// 插入集合，不返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task InsertList_Table_Async<T>(string table, List<T> list) where T : class;

        /// <summary>
        /// 插入，支持主键自动获取。
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        Task<object> Insert_Async<T>(T poco) where T : class;

        /// <summary>
        /// 插入，支持主键自动获取。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        Task<object> Insert_Table_Async<T>(string table, T poco) where T : class;


        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="poco">对象</param>
        /// <param name="autoIncrement">是否自增，是，返回自增ID</param>
        /// <returns></returns>
        Task<object> Insert_Table_Async(string table, object poco, bool autoIncrement);

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="table"></param>
        /// <param name="poco"></param>
        /// <param name="ignoreFields">忽略字段，这里填类中的属性名</param>
        /// <returns></returns>
        Task<object> Insert_Table_Async(string table, object poco, IEnumerable<string> ignoreFields);
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="poco"></param>
        /// <param name="autoIncrement">是否自增，是，返回自增ID</param>
        /// <param name="ignoreFields">忽略字段，这里填类中的属性名</param>
        /// <returns></returns>
        Task<object> Insert_Table_Async(string table, object poco, bool autoIncrement, IEnumerable<string> ignoreFields);


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        Task<int> Update_Async<T>(T poco) where T : class;
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        Task<int> Update_Table_Async<T>(string table, T poco) where T : class;
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        Task<int> Delete_Async<T>(T poco) where T : class;
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        Task<int> Delete_Table_Async<T>(string table, T poco) where T : class;
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<int> Delete_Async<T>(string sql, params object[] args) where T : class;

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<int> Delete_Table_Async(string table, string sql, params object[] args);


        /// <summary>
        /// 根据ID 删除表数据, 注： 单独从delete方法，防止出错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        Task<int> DeleteById_Async<T>(object primaryKey) where T : class;

        /// <summary>
        /// 根据ID 删除表数据, 注： 单独从delete方法，防止出错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        Task<int> DeleteById_Table_Async<T>(string table, object primaryKey) where T : class;



        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="poco"></param>
        Task Save_Async<T>(T poco) where T : class;
        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="poco"></param>
        /// <returns></returns>
        Task Save_Table_Async<T>(string table, T poco) where T : class;

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<int> Update_Async<T>(string sql, params object[] args) where T : class;

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Task<int> Update_Table_Async<T>(string table, string sql, params object[] args) where T : class;

        #endregion Object  Insert Update Delete DeleteById Save




    }
}

#endif