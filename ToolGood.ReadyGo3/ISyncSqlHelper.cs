using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.PetaPoco;

namespace ToolGood.ReadyGo3
{
    public partial class SqlHelper : ISyncSqlHelper
    {

    }
    /// <summary>
    /// 同步
    /// </summary>
    public partial interface ISyncSqlHelper : IDisposable
    {
        Transaction UseTransaction();


        #region Select Update

        
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T FirstOrDefault<T>(object condition) where T : class;

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        List<T> Select<T>(int limit, int offset, object condition) where T : class;


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        List<T> Select<T>(int limit, object condition) where T : class;

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        List<T> Select<T>(object condition) where T : class;


        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        List<T> SelectPage<T>(int page, int itemsPerPage, object condition)
           where T : class;

        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Page<T> Page<T>(int page, int itemsPerPage, object condition)
           where T : class;


        /// <summary>
        /// 根据条件更新对象
        /// </summary>
        /// <param name="set"></param>
        /// <param name="condition">条件</param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        int Update<T>(object set, object condition, IEnumerable<string> ignoreFields = null) where T : class;



        /// <summary>
        /// 根据条件从数据库中删除对象
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        int Delete<T>(object condition) where T : class;

        /// <summary>
        /// 根据条件查询个数
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        int Count<T>(object condition) where T : class;

        /// <summary>
        /// 根据条件判断是否存在
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        bool Exists<T>(object condition) where T : class;


        #endregion

        #region FirstOrDefault PK
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T FirstOrDefault<T>(int condition) where T : class;

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T FirstOrDefault<T>(uint condition) where T : class;


        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T FirstOrDefault<T>(long condition) where T : class;

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T FirstOrDefault<T>(ulong condition) where T : class;


        #endregion

        #region FirstOrDefault PK
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T FirstOrDefault<T>(int? condition) where T : class;

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T FirstOrDefault<T>(uint? condition) where T : class;


        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T FirstOrDefault<T>(long? condition) where T : class;

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T FirstOrDefault<T>(ulong? condition) where T : class;


        #endregion

        #region Table_FirstOrDefault PK

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T Table_FirstOrDefault<T>(string table, int condition) where T : class;
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T Table_FirstOrDefault<T>(string table, uint condition) where T : class;

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T Table_FirstOrDefault<T>(string table, long condition) where T : class;
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T Table_FirstOrDefault<T>(string table, ulong condition) where T : class;

        #endregion

        #region Table_FirstOrDefault PK

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T Table_FirstOrDefault<T>(string table, int? condition) where T : class;
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T Table_FirstOrDefault<T>(string table, uint? condition) where T : class;

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T Table_FirstOrDefault<T>(string table, long? condition) where T : class;
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T Table_FirstOrDefault<T>(string table, ulong? condition) where T : class;

        #endregion

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        T Table_FirstOrDefault<T>(string table, object condition) where T : class;


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        List<T> Table_Select<T>(string table, int limit, int offset, object condition) where T : class;

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        List<T> Table_Select<T>(string table, int limit, object condition) where T : class;

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        List<T> Table_Select<T>(string table, object condition) where T : class;



        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        List<T> Table_SelectPage<T>(string table, int page, int itemsPerPage, object condition)
           where T : class;


        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Page<T> Table_Page<T>(string table, int page, int itemsPerPage, object condition)
           where T : class;


        /// <summary>
        /// 更新表
        /// </summary>
        /// <param name="table"></param>
        /// <param name="set"></param>
        /// <param name="condition"></param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        int Table_Update(string table, object set, object condition, IEnumerable<string> ignoreFields = null);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        int Table_Delete(string table, object condition);


        /// <summary>
        /// 根据条件查询个数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        int Table_Count(string table, object condition);


        /// <summary>
        /// 根据条件判断是否存在
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        bool Table_Exists(string table, object condition);


        #region Execute ExecuteScalar ExecuteDataTable ExecuteDataSet Exists

        /// <summary>
        /// 执行 SQL 语句，并返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回受影响的行数</returns>
        int Execute(string sql, params object[] args);

        /// <summary>
        /// 执行SQL 查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回查询所返回的结果集中第一行的第一列。忽略额外的列或行</returns>
        T ExecuteScalar<T>(string sql = "", params object[] args);
        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回 DataTable</returns>
        DataTable ExecuteDataTable(string sql, params object[] args);

        /// <summary>
        /// 执行SQL 查询,返回 DataSet
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>返回 DataSet</returns>
        DataSet ExecuteDataSet(string sql, params object[] args);

        /// <summary>
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        bool Exists<T>(string sql, params object[] args);
        /// <summary>
        /// 执行SQL 查询,判断是否存在，返回bool类型
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        bool Table_Exists(string table, string sql, params object[] args);

        /// <summary>
        /// 执行SQL 查询,返回数量
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        int Table_Count(string table, string sql, params object[] args);

        /// <summary>
        ///  执行SQL 查询,返回数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        int Count<T>(string sql = "", params object[] args);

        /// <summary>
        /// 执行SQL 查询,返回数量
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        int Count(string sql, params object[] args);

        #endregion Execute ExecuteScalar ExecuteDataTable ExecuteDataSet Exists

        #region Select Page Select
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        List<T> Select<T>(string sql = "", params object[] args);

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        List<T> Table_Select<T>(string table, string sql = "", params object[] args) where T : class;

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        List<T> Select<T>(int limit, string sql = "", params object[] args) where T : class;
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        List<T> Table_Select<T>(string table, int limit, string sql = "", params object[] args) where T : class;


        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset">跳过</param>
        /// <param name="limit">获取个数</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        List<T> Select<T>(int limit, int offset, string sql = "", params object[] args) where T : class;
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
        List<T> Table_Select<T>(string table, int limit, int offset, string sql = "", params object[] args) where T : class;

        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        List<T> SelectPage<T>(int page, int itemsPerPage, string sql = "", params object[] args) where T : class;
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        List<T> Table_SelectPage<T>(string table, int page, int itemsPerPage, string sql = "", params object[] args) where T : class;

        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页数量</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        Page<T> Page<T>(int page, int itemsPerPage, string sql = "", params object[] args) where T : class;
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
        Page<T> Table_Page<T>(string table, int page, int itemsPerPage, string sql = "", params object[] args) where T : class;

        /// <summary>
        /// 执行SQL 查询, 返回单个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnSql"></param>
        /// <param name="tableSql"></param>
        /// <param name="whereSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        T SQL_FirstOrDefault<T>(string columnSql, string tableSql, string whereSql, params object[] args) where T : class;


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
        List<T> SQL_Select<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
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
        List<T> SQL_Select<T>(int limit, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
           where T : class;


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
        List<T> SQL_Select<T>(string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
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
        Page<T> SQL_Page<T>(int page, int itemsPerPage, string columnSql, string tableSql, string orderSql, string whereSql, params object[] args)
           where T : class;

        #endregion Select Page Select

        #region FirstOrDefault

        /// <summary>
        /// 获取第一个类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        T FirstOrDefault<T>(string sql = "", params object[] args);
        /// <summary>
        /// 获取第一个类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        T Table_FirstOrDefault<T>(string table, string sql = "", params object[] args) where T : class;


        #endregion FirstOrDefault

        #region Object  Insert Update Delete DeleteById Save

        /// <summary>
        /// 插入集合，不返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        void InsertList<T>(List<T> list) where T : class;
        /// <summary>
        /// 插入集合，不返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="list"></param>
        void InsertList<T>(string table, List<T> list) where T : class;


        /// <summary>
        /// 插入，支持主键自动获取。
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        object Insert<T>(T poco) where T : class;
        /// <summary>
        /// 插入，支持主键自动获取。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        object Table_Insert<T>(string table, T poco) where T : class;

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="table"></param>
        /// <param name="poco"></param>
        /// <param name="autoIncrement"></param>
        /// <returns></returns>
        object Table_Insert(string table, object poco, bool autoIncrement);
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="table"></param>
        /// <param name="poco"></param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        object Table_Insert(string table, object poco, IEnumerable<string> ignoreFields);
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="table"></param>
        /// <param name="poco"></param>
        /// <param name="autoIncrement"></param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        object Table_Insert(string table, object poco, bool autoIncrement, IEnumerable<string> ignoreFields);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        int Update<T>(T poco) where T : class;
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="poco"></param>
        /// <returns></returns>
        int Table_Update<T>(string table, T poco) where T : class;

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        int Delete<T>(T poco) where T : class;
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="poco">对象</param>
        /// <returns></returns>
        int Table_Delete<T>(string table, T poco) where T : class;


        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        int Delete<T>(string sql, params object[] args) where T : class;
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        int Table_Delete(string table, string sql, params object[] args);

        /// <summary>
        /// 根据ID 删除表数据, 注： 单独从delete方法，防止出错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        int DeleteById<T>(object primaryKey) where T : class;
        /// <summary>
        /// 根据ID 删除表数据, 注： 单独从delete方法，防止出错
        /// </summary>
        /// <param name="table"></param>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        int Table_DeleteById(string table, object primaryKey);


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="poco"></param>
        void Save<T>(T poco) where T : class;
        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="poco"></param>
        void Table_Save<T>(string table, T poco) where T : class;

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        int Update<T>(string sql, params object[] args);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns></returns>
        int Table_Update(string table, string sql, params object[] args);
        #endregion Object  Insert Update Delete DeleteById Save

    }


    public partial interface ISyncSqlHelper  
    {



    }
}
