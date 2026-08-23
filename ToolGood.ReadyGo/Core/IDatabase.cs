#nullable enable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ToolGood.ReadyGo.NPoco.Linq;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 定义数据库操作接口，提供 POCO 对象的插入、更新、删除、保存等核心功能。
    /// </summary>
    public interface IDatabase : IAsyncDatabase, IDatabaseQuery
    {
        /// <summary>
        /// Insert POCO into the table, primary key and autoincrement specified
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="autoIncrement">主键是否自增。</param>
        /// <param name="poco">待插入的对象。</param>
        /// <returns>插入后的主键值。</returns>
        object Insert<T>(string tableName, string primaryKeyName, bool autoIncrement, T poco);

        /// <summary>
        /// Insert POCO into the table, primary key specified
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">待插入的对象。</param>
        /// <returns>插入后的主键值。</returns>
        object Insert<T>(string tableName, string primaryKeyName, T poco);

        /// <summary>
        /// Insert POCO into the table by convention or configuration
        /// </summary>
        /// <param name="poco">待插入的对象。</param>
        /// <returns>插入后的主键值。</returns>
        object Insert<T>(T poco);
              
        /// <summary>
        /// Insert POCO's into database using SqlBulkCopy for SqlServer (other DB's currently fall back to looping each row)
        /// </summary>
        /// <param name="pocos">待插入的对象集合。</param>
        /// <param name="options">批量插入选项，可为 null。</param>
        void InsertBulk<T>(IEnumerable<T> pocos, InsertBulkOptions? options = null);

        /// <summary>
        /// Insert POCO's into database by concatenating sql using the provided batch options
        /// </summary>
        /// <param name="pocos">待插入的对象集合。</param>
        /// <param name="options">批处理选项，可为 null。</param>
        /// <returns>受影响的行数。</returns>
        int InsertBatch<T>(IEnumerable<T> pocos, BatchOptions? options = null);

        /// <summary>
        /// Update POCO in the specified table, primary key and primarkey value
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">待更新的对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <returns>受影响的行数。</returns>
        int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        /// Update POCO in the specified table, primary key
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">待更新的对象。</param>
        /// <returns>受影响的行数。</returns>
        int Update(string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Update POCO in the specified table, primary key, primarkey value for only the columns specified
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">待更新的对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <param name="columns">要更新的列集合。</param>
        /// <returns>受影响的行数。</returns>
        int Update(string tableName, string primaryKeyName, object poco, object? primaryKeyValue, IEnumerable<string>? columns);

        /// <summary>
        /// Update POCO in the specified table, primary key for only the columns specified
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">待更新的对象。</param>
        /// <param name="columns">要更新的列集合。</param>
        /// <returns>受影响的行数。</returns>
        int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string>? columns);

        /// <summary>
        /// Update POCO by convention or configuration for only the columns specified
        /// </summary>
        /// <param name="poco">待更新的对象。</param>
        /// <param name="columns">要更新的列集合。</param>
        /// <returns>受影响的行数。</returns>
        int Update(object poco, IEnumerable<string> columns);

        /// <summary>
        /// Update POCO by primary key for only the columns specified
        /// </summary>
        /// <param name="poco">待更新的对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <param name="columns">要更新的列集合。</param>
        /// <returns>受影响的行数。</returns>
        int Update(object poco, object primaryKeyValue, IEnumerable<string>? columns);

        /// <summary>
        /// Update POCO by convention or configuration
        /// </summary>
        /// <param name="poco">待更新的对象。</param>
        /// <returns>受影响的行数。</returns>
        int Update(object poco);

        /// <summary>
        /// Update POCO by convention or configuration specifying the properties to update
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="poco">待更新的对象。</param>
        /// <param name="fields">指定要更新属性的表达式。</param>
        /// <returns>受影响的行数。</returns>
        int Update<T>(T poco, Expression<Func<T, object>> fields);

        /// <summary>
        /// Update POCO by primary key
        /// </summary>
        /// <param name="poco">待更新的对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <returns>受影响的行数。</returns>
        int Update(object poco, object primaryKeyValue);

        /// <summary>
        /// Runs an update statement deriving the table name from T and appending the sql provided. 
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">要拼接的更新 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>受影响的行数。</returns>
        /// <example>
        /// Update&lt;User&gt;("set name = @0 where id = @1", "John", 1);
        /// </example>        
        int Update<T>(string sql, params object[] args);

        /// <summary>
        /// Runs an update statement deriving the table name from T and appending the sql provided. 
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">要拼接的更新 SQL。</param>
        /// <returns>受影响的行数。</returns>
        /// <example>
        /// Update&lt;User&gt;("set name = @0 where id = @1", "John", 1);
        /// </example>
        int Update<T>(Sql sql);

        /// <summary>
        /// Update POCO's into database by concatenating sql using the provided batch options
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="pocos">待更新的对象集合。</param>
        /// <param name="options">批处理选项，可为 null。</param>
        /// <returns>受影响的行数。</returns>
        int UpdateBatch<T>(IEnumerable<UpdateBatch<T>> pocos, BatchOptions? options = null);

        /// <summary>
        /// Generate an update statement using a Fluent syntax. Remember to call Execute.
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>流式更新查询提供程序。</returns>
        IUpdateQueryProvider<T> UpdateMany<T>();

        /// <summary>
        /// Delete POCO specifying the table name and primary key
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">待删除的对象。</param>
        /// <returns>受影响的行数。</returns>
        int Delete(string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Delete POCO specifying the table name, primary key name and primary key value
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">待删除的对象，可为 null。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <returns>受影响的行数。</returns>
        int Delete(string tableName, string primaryKeyName, object? poco, object? primaryKeyValue);

        /// <summary>
        /// Delete POCO using convention or configuration
        /// </summary>
        /// <param name="poco">待删除的对象。</param>
        /// <returns>受影响的行数。</returns>
        int Delete(object poco);

        /// <summary>
        /// Runs an delete statement deriving the table name from T and appending the sql provided. 
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">要拼接的删除 SQL 语句。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>受影响的行数。</returns>
        /// <example>
        /// Delete&lt;User&gt;("where id = @0", 1);
        /// </example>     
        int Delete<T>(string sql, params object[] args);

        /// <summary>
        /// Runs an delete statement deriving the table name from T and appending the sql provided. 
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">要拼接的删除 SQL。</param>
        /// <returns>受影响的行数。</returns>
        /// <example>
        /// Delete&lt;User&gt;("where id = @0", 1);
        /// </example>
        int Delete<T>(Sql sql);

        /// <summary>
        /// Delete POCO deriving the table name from T and generating sql using the primary key
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="pocoOrPrimaryKey">待删除的对象或主键值。</param>
        /// <returns>受影响的行数。</returns>
        int Delete<T>(object pocoOrPrimaryKey);

        /// <summary>
        /// Generate a delete statement using a Fluent syntax. Remember to call Execute.
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>流式删除查询提供程序。</returns>
        IDeleteQueryProvider<T> DeleteMany<T>();

        /// <summary>
        /// Performs an insert or an update depending on whether the POCO already exists. (i.e. an upsert/merge)
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="poco">待保存的对象。</param>
        void Save<T>(T poco);

        /// <summary>
        /// Determines whether the POCO already exists
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="poco">要判断的对象。</param>
        /// <returns>对象已存在返回 true，否则返回 false。</returns>
        bool IsNew<T>(T poco);
    }

    /// <summary>
    /// 定义数据库配置接口，提供映射器、元数据工厂、数据库类型、拦截器与连接字符串。
    /// </summary>
    public interface IDatabaseConfig
    {
        /// <summary>
        /// A collection of mappers used for converting values on inserting or on mapping
        /// </summary>        
        IMapperCollection Mappers { get; set; }
        /// <summary>
        /// The PocoData factory used to build the meta data used by ToolGood.ReadyGo.NPoco
        /// </summary>        
        IPocoDataFactory PocoDataFactory { get; set; }
        /// <summary>
        /// The target database used to handle different oddities in the different database providers
        /// </summary>        
        IDatabaseType DatabaseType { get; }
        /// <summary>
        /// A list of IInterceptor's which can run at different times in the CRUD lifecyle
        /// </summary>        
        List<IInterceptor> Interceptors { get; }
        /// <summary>
        /// Retrieves current connection string
        /// </summary>        
        string ConnectionString { get; }
    }
}
