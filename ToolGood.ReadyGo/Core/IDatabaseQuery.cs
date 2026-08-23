#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using ToolGood.ReadyGo.NPoco.Linq;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 定义查询数据库接口，提供执行、分页、抓取、单行查询及多结果集查询等功能。
    /// </summary>
    public interface IDatabaseQuery : IAsyncQueryDatabase, IBaseDatabase
    {
        /// <summary>
        /// Builds a paged query from a non-paged query
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="skip">要跳过的记录数。</param>
        /// <param name="take">要获取的记录数。</param>
        /// <param name="sql">原始查询 SQL。</param>
        /// <param name="args">查询参数，按引用传递。</param>
        /// <param name="sqlCount">输出：用于统计总行数的 SQL。</param>
        /// <param name="sqlPage">输出：用于分页查询的 SQL。</param>
        void BuildPageQueries<T>(long skip, long take, string sql, ref object[] args, out string sqlCount, out string sqlPage);
        
        /// <summary>
        /// Executes the provided sql and parameters
        /// </summary>
        /// <param name="sql">要执行的 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>受影响的行数。</returns>
        int Execute(string sql, params object[] args);
        
        /// <summary>
        /// Executes the provided sql and parameters
        /// </summary>
        /// <param name="sql">要执行的 SQL。</param>
        /// <returns>受影响的行数。</returns>
        int Execute(Sql sql);

        /// <summary>
        /// Executes the provided sql and parameters with the specified command type
        /// </summary>
        /// <param name="sql">要执行的 SQL 语句。</param>
        /// <param name="commandType">命令类型。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>受影响的行数。</returns>
        int Execute(string sql, CommandType commandType, params object[] args);

        /// <summary>
        /// Executes the provided sql and parameters and casts the result to T
        /// </summary>
        /// <typeparam name="T">结果类型。</typeparam>
        /// <param name="sql">要执行的 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>结果集中第一行第一列的值，转换为 T 类型。</returns>
        T ExecuteScalar<T>(string sql, params object[] args);
        
        /// <summary>
        /// Executes the provided sql and parameters and casts the result to T
        /// </summary>
        /// <typeparam name="T">结果类型。</typeparam>
        /// <param name="sql">要执行的 SQL。</param>
        /// <returns>结果集中第一行第一列的值，转换为 T 类型。</returns>
        T ExecuteScalar<T>(Sql sql);

        /// <summary>
        /// Executes the provided sql and parameters with the specified commandType and casts the result to T
        /// </summary>
        /// <typeparam name="T">结果类型。</typeparam>
        /// <param name="sql">要执行的 SQL 语句。</param>
        /// <param name="commandType">命令类型。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>结果集中第一行第一列的值，转换为 T 类型。</returns>
        T ExecuteScalar<T>(string sql, CommandType commandType, params object[] args);

        /// <summary>
        /// Non generic Fetch which returns a list of objects of the given type provided
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>对象列表。</returns>
        List<object> Fetch(Type type, string sql, params object[] args);
        
        /// <summary>
        /// Non generic Fetch which returns a list of objects of the given type provided
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <param name="Sql">查询 SQL。</param>
        /// <returns>对象列表。</returns>
        List<object> Fetch(Type type, Sql Sql);
        
        /// <summary>
        /// Non generic Query which returns a list of objects of the given type provided. 
        /// Caution: This query will only be executed once you start iterating the result
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>对象的延迟枚举结果。</returns>
        IEnumerable<object> Query(Type type, string sql, params object[] args);
        
        /// <summary>
        /// Non generic Query which returns a list of objects of the given type provided. 
        /// Caution: This query will only be executed once you start iterating the result
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <param name="Sql">查询 SQL。</param>
        /// <returns>对象的延迟枚举结果。</returns>
        IEnumerable<object> Query(Type type, Sql Sql);

        /// <summary>
        /// Fetch all objects of type T from the database using the conventions or configuration on the type T. 
        /// Caution: This will retrieve ALL objects in the table
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>对象列表。</returns>
        List<T> Fetch<T>();
        
        /// <summary>
        /// Fetch objects of type T from the database using the sql and parameters specified. 
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>对象列表。</returns>
        List<T> Fetch<T>(string sql, params object[] args);
        
        /// <summary>
        /// Fetch objects of type T from the database using the sql and parameters specified
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>对象列表。</returns>
        List<T> Fetch<T>(Sql sql);
        
        /// <summary>
        /// Fetch objects of type T from the database using the sql and parameters specified. 
        /// The sql provided will be converted so that only the results for the page and itemsPerPage values specified will be returned.
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="page">页码。</param>
        /// <param name="itemsPerPage">每页记录数。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>对象列表。</returns>
        List<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args);
        
        /// <summary>
        /// Fetch objects of type T from the database using the sql and parameters specified. 
        /// The sql provided will be converted so that only the results for the page and itemsPerPage values specified will be returned.
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="page">页码。</param>
        /// <param name="itemsPerPage">每页记录数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>对象列表。</returns>
        List<T> Fetch<T>(long page, long itemsPerPage, Sql sql);
        
        /// <summary>
        /// Fetch objects of type T from the database using the sql and parameters specified. 
        /// The sql provided will be converted so that only the results for the page and itemsPerPage specified will be returned.
        /// Extra metadata in the Page class will also be returned.
        /// Note: This will perform two queries. One for the paged results and one for the count of all results.
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="page">页码。</param>
        /// <param name="itemsPerPage">每页记录数。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>包含分页结果与元数据的分页对象。</returns>
        Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args);
        
        /// <summary>
        /// Fetch objects of type T from the database using the sql and parameters specified. 
        /// The sql provided will be converted so that only the results for the page and itemsPerPage specified will be returned.
        /// Extra metadata in the Page class will also be returned.
        /// Note: This will perform two queries. One for the paged results and one for the count of all results.
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="page">页码。</param>
        /// <param name="itemsPerPage">每页记录数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>包含分页结果与元数据的分页对象。</returns>
        Page<T> Page<T>(long page, long itemsPerPage, Sql sql);

        /// <summary>
        /// Fetch objects of type T from the database using the sql and parameters specified. 
        /// The sql provided will be converted so that only the results for the skip and take values specified will be returned.
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="skip">要跳过的记录数。</param>
        /// <param name="take">要获取的记录数。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>对象列表。</returns>
        List<T> SkipTake<T>(long skip, long take, string sql, params object[] args);
        
        /// <summary>
        /// Fetch objects of type T from the database using the sql and parameters specified. 
        /// The sql provided will be converted so that only the results for the skip and take values specified will be returned.
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="skip">要跳过的记录数。</param>
        /// <param name="take">要获取的记录数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>对象列表。</returns>
        List<T> SkipTake<T>(long skip, long take, Sql sql);

        /// <summary>
        /// Fetch objects of type T using the sql provided, but also retrieve the many property's data using the sql provided.
        /// The one columns should come first then the many columns. 
        /// eg. select one.*, many.* from one inner join many on one.id = many.oneid
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="many">指定集合属性的表达式。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>对象列表。</returns>
        List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, string sql, params object[] args);
        
        /// <summary>
        /// Fetch objects of type T using the sql provided, but also retrieve the many property's data using the sql provided.
        /// The one columns should come first then the many columns. 
        /// eg. select one.*, many.* from one inner join many on one.id = many.oneid
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="many">指定集合属性的表达式。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>对象列表。</returns>
        List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, Sql sql);
        
        /// <summary>
        /// Fetch objects of type T using the sql provided, but also retrieve the many property's data using the sql provided.
        /// The one columns should come first then the many columns. 
        /// eg. select one.*, many.* from one inner join many on one.id = many.oneid
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="many">指定集合属性的表达式。</param>
        /// <param name="idFunc">用于获取对象主键的委托。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>对象列表。</returns>
        List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, Func<T, object> idFunc, string sql, params object[] args);
        
        /// <summary>
        /// Fetch objects of type T using the sql provided, but also retrieve the many property's data using the sql provided.
        /// The one columns should come first then the many columns. 
        /// eg. select one.*, many.* from one inner join many on one.id = many.oneid
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="many">指定集合属性的表达式。</param>
        /// <param name="idFunc">用于获取对象主键的委托。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>对象列表。</returns>
        List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, Func<T, object> idFunc, Sql sql);

        /// <summary>
        /// Fetch objects of type T from the database using the sql and parameters specified. 
        /// Caution: This query will only be executed once you start iterating the result
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>对象的延迟枚举结果。</returns>
        IEnumerable<T> Query<T>(string sql, params object[] args);
        
        /// <summary>
        /// Fetch objects of type T from the database using the sql and parameters specified. 
        /// Caution: This query will only be executed once you start iterating the result
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>对象的延迟枚举结果。</returns>
        IEnumerable<T> Query<T>(Sql sql);
        
        /// <summary>
        /// Entry point for LINQ queries
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>支持 Include 的 LINQ 查询提供程序。</returns>
        IQueryProviderWithIncludes<T> Query<T>();
        
        /// <summary>
        /// Get an object of type T by primary key value
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="primaryKey">主键值。</param>
        /// <returns>查询到的对象。</returns>
        T SingleById<T>(object primaryKey);
        
        /// <summary>
        /// Fetch the only row of type T using the sql and parameters specified
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>查询到的唯一对象。</returns>
        T Single<T>(string sql, params object[] args);
        
        /// <summary>
        /// Fetch the only row of type T using the sql and parameters specified into the T instance provided
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">要填充的对象实例。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>填充后的对象。</returns>
        T SingleInto<T>(T instance, string sql, params object[] args);
        
        /// <summary>
        /// Get an object of type T by primary key value where the row may not be there
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="primaryKey">主键值。</param>
        /// <returns>查询到的对象，若不存在则返回 null。</returns>
        T? SingleOrDefaultById<T>(object primaryKey);
        
        /// <summary>
        /// Fetch the only row of type T using the sql and parameters specified
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>查询到的唯一对象，若不存在则返回 null。</returns>
        T? SingleOrDefault<T>(string sql, params object[] args);
        
        /// <summary>
        /// Fetch the only row of type T using the sql and parameters specified into the T instance provided
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">要填充的对象实例。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>填充后的对象，若不存在则返回 null。</returns>
        T? SingleOrDefaultInto<T>(T instance, string sql, params object[] args);

        /// <summary>
        /// Fetch the first row of type T using the sql and parameters specified
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>查询到的第一行对象。</returns>
        T First<T>(string sql, params object[] args);
        
        /// <summary>
        /// Fetch the first row of type T using the sql and parameters specified into the T instance provided
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">要填充的对象实例。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>填充后的对象。</returns>
        T FirstInto<T>(T instance, string sql, params object[] args);
        
        /// <summary>
        /// Fetch the first row of type T using the sql and parameters specified
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>查询到的第一行对象，若不存在则返回 null。</returns>
        T? FirstOrDefault<T>(string sql, params object[] args);
        
        /// <summary>
        /// Fetch the first row of type T using the sql and parameters specified into the T instance provided
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">要填充的对象实例。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>填充后的对象，若不存在则返回 null。</returns>
        T? FirstOrDefaultInto<T>(T instance, string sql, params object[] args);

        /// <summary>
        /// Fetch the only row of type T using the Sql specified
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>查询到的唯一对象。</returns>
        T Single<T>(Sql sql);
        
        /// <summary>
        /// Fetch the only row of type T using the Sql specified into the T instance provided
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">要填充的对象实例。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>填充后的对象。</returns>
        T SingleInto<T>(T instance, Sql sql);
        
        /// <summary>
        /// Fetch the only row of type T using the Sql specified
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>查询到的唯一对象，若不存在则返回 null。</returns>
        T? SingleOrDefault<T>(Sql sql);
        
        /// <summary>
        /// Fetch the only row of type T using the Sql specified
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">要填充的对象实例。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>填充后的对象，若不存在则返回 null。</returns>
        T? SingleOrDefaultInto<T>(T instance, Sql sql);
        
        /// <summary>
        /// Fetch the first row of type T using the Sql specified
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>查询到的第一行对象。</returns>
        T First<T>(Sql sql);
        
        /// <summary>
        /// Fetch the first row of type T using the Sql specified into the T instance provided
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">要填充的对象实例。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>填充后的对象。</returns>
        T FirstInto<T>(T instance, Sql sql);
        
        /// <summary>
        /// Fetch the first row of type T using the Sql specified
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>查询到的第一行对象，若不存在则返回 null。</returns>
        T? FirstOrDefault<T>(Sql sql);
        
        /// <summary>
        /// Fetch the first row of type T using the Sql specified
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">要填充的对象实例。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>填充后的对象，若不存在则返回 null。</returns>
        T? FirstOrDefaultInto<T>(T instance, Sql sql);

        /// <summary>
        /// Fetches the first two columns into a dictionary using the first value as the key and the second as the value
        /// </summary>
        /// <typeparam name="TKey">字典键类型。</typeparam>
        /// <typeparam name="TValue">字典值类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>由结果集前两列构成的字典。</returns>
        Dictionary<TKey, TValue> Dictionary<TKey, TValue>(Sql sql) where TKey : notnull;
        
        /// <summary>
        /// Fetches the first two columns into a dictionary using the first value as the key and the second as the value
        /// </summary>
        /// <typeparam name="TKey">字典键类型。</typeparam>
        /// <typeparam name="TValue">字典值类型。</typeparam>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>由结果集前两列构成的字典。</returns>
        Dictionary<TKey, TValue> Dictionary<TKey, TValue>(string sql, params object[] args) where TKey : notnull;
        
        /// <summary>
        /// Checks if the POCO of type T exists by using the primary key value
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="primaryKey">主键值。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        bool Exists<T>(object primaryKey);
        
        /// <summary>
        /// Fetches multiple result sets into the one object.
        /// In this method you must provide how you will take the results and combine them
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">合并后的返回类型。</typeparam>
        /// <param name="cb">用于合并结果集的回调。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>合并后的结果。</returns>
        TRet FetchMultiple<T1, T2, TRet>(Func<List<T1>, List<T2>, TRet> cb, string sql, params object[] args);
        
        /// <summary>
        /// Fetches multiple result sets into the one object.
        /// In this method you must provide how you will take the results and combine them
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">合并后的返回类型。</typeparam>
        /// <param name="cb">用于合并结果集的回调。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>合并后的结果。</returns>
        TRet FetchMultiple<T1, T2, T3, TRet>(Func<List<T1>, List<T2>, List<T3>, TRet> cb, string sql, params object[] args);
        
        /// <summary>
        /// Fetches multiple result sets into the one object.
        /// In this method you must provide how you will take the results and combine them
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">合并后的返回类型。</typeparam>
        /// <param name="cb">用于合并结果集的回调。</param>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>合并后的结果。</returns>
        TRet FetchMultiple<T1, T2, T3, T4, TRet>(Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> cb, string sql, params object[] args);
        
        /// <summary>
        /// Fetches multiple result sets into the one object.
        /// In this method you must provide how you will take the results and combine them
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">合并后的返回类型。</typeparam>
        /// <param name="cb">用于合并结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>合并后的结果。</returns>
        TRet FetchMultiple<T1, T2, TRet>(Func<List<T1>, List<T2>, TRet> cb, Sql sql);
        
        /// <summary>
        /// Fetches multiple result sets into the one object.
        /// In this method you must provide how you will take the results and combine them
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">合并后的返回类型。</typeparam>
        /// <param name="cb">用于合并结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>合并后的结果。</returns>
        TRet FetchMultiple<T1, T2, T3, TRet>(Func<List<T1>, List<T2>, List<T3>, TRet> cb, Sql sql);
        
        /// <summary>
        /// Fetches multiple result sets into the one object.
        /// In this method you must provide how you will take the results and combine them
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">合并后的返回类型。</typeparam>
        /// <param name="cb">用于合并结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>合并后的结果。</returns>
        TRet FetchMultiple<T1, T2, T3, T4, TRet>(Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> cb, Sql sql);

        /// <summary>
        /// Fetches multiple result sets into the one Tuple.
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>包含两个结果集列表的元组。</returns>
        (List<T1>, List<T2>) FetchMultiple<T1, T2>(string sql, params object[] args);
        
        /// <summary>
        /// Fetches multiple result sets into the one Tuple.
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>包含三个结果集列表的元组。</returns>
        (List<T1>, List<T2>, List<T3>) FetchMultiple<T1, T2, T3>(string sql, params object[] args);
        
        /// <summary>
        /// Fetches multiple result sets into the one Tuple.
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>包含四个结果集列表的元组。</returns>
        (List<T1>, List<T2>, List<T3>, List<T4>) FetchMultiple<T1, T2, T3, T4>(string sql, params object[] args);
        
        /// <summary>
        /// Fetches multiple result sets into the one Tuple.
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>包含两个结果集列表的元组。</returns>
        (List<T1>, List<T2>) FetchMultiple<T1, T2>(Sql sql);
        
        /// <summary>
        /// Fetches multiple result sets into the one Tuple.
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>包含三个结果集列表的元组。</returns>
        (List<T1>, List<T2>, List<T3>) FetchMultiple<T1, T2, T3>(Sql sql);
        
        /// <summary>
        /// Fetches multiple result sets into the one Tuple.
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <returns>包含四个结果集列表的元组。</returns>
        (List<T1>, List<T2>, List<T3>, List<T4>) FetchMultiple<T1, T2, T3, T4>(Sql sql);
    }
}
