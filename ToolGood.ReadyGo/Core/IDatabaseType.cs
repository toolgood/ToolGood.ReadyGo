using ToolGood.ReadyGo.NPoco.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 定义数据库类型抽象接口，用于屏蔽不同数据库提供程序在 SQL 生成、参数处理、插入及类型映射上的差异。
    /// </summary>
    public interface IDatabaseType
    {
        /// <summary>
        /// 构建分页查询语句。
        /// </summary>
        /// <param name="skip">要跳过的记录数。</param>
        /// <param name="take">要获取的记录数。</param>
        /// <param name="parts">拆分后的 SQL 片段。</param>
        /// <param name="args">查询参数数组，按引用传递。</param>
        /// <returns>分页查询语句。</returns>
        string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args);
        /// <summary>
        /// 转义 SQL 标识符。
        /// </summary>
        /// <param name="str">待转义的标识符。</param>
        /// <returns>转义后的标识符。</returns>
        string EscapeSqlIdentifier(string str);
        /// <summary>
        /// 转义表名。
        /// </summary>
        /// <param name="tableName">待转义的表名。</param>
        /// <returns>转义后的表名。</returns>
        string EscapeTableName(string tableName);
        /// <summary>
        /// 执行插入并返回新生成的主键值。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">当前数据库实例。</param>
        /// <param name="cmd">插入命令。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="useOutputClause">是否使用 OUTPUT 子句返回主键。</param>
        /// <param name="poco">待插入的对象。</param>
        /// <param name="args">命令参数。</param>
        /// <returns>新生成的主键值。</returns>
        object ExecuteInsert<T>(IDatabase db, DbCommand cmd, string primaryKeyName, bool useOutputClause, T poco, object[] args);
        /// <summary>
        /// 异步执行插入并返回新生成的主键值。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">当前数据库实例。</param>
        /// <param name="cmd">插入命令。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="useOutputClause">是否使用 OUTPUT 子句返回主键。</param>
        /// <param name="poco">待插入的对象。</param>
        /// <param name="args">命令参数。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>表示异步操作的任务，结果为新生成的主键值。</returns>
        Task<object> ExecuteInsertAsync<T>(IDatabase db, DbCommand cmd, string primaryKeyName, bool useOutputClause, T poco, object[] args, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步执行命令并返回受影响的行数。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        /// <param name="cmd">要执行的命令。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>表示异步操作的任务，结果为受影响的行数。</returns>
        Task<int> ExecuteNonQueryAsync(IDatabase database, DbCommand cmd, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步执行命令并返回数据读取器。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        /// <param name="cmd">要执行的命令。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>表示异步操作的任务，结果为数据读取器。</returns>
        Task<DbDataReader> ExecuteReaderAsync(IDatabase database, DbCommand cmd, CancellationToken cancellationToken = default);
        /// <summary>
        /// 异步执行命令并返回结果集中第一行第一列的值。
        /// </summary>
        /// <param name="database">当前数据库实例。</param>
        /// <param name="cmd">要执行的命令。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>表示异步操作的任务，结果为结果集中第一行第一列的值。</returns>
        Task<object> ExecuteScalarAsync(IDatabase database, DbCommand cmd, CancellationToken cancellationToken = default);
        /// <summary>
        /// 创建 SQL 表达式访问器。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">当前数据库实例。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <returns>SQL 表达式访问器。</returns>
        ISqlExpression<T> ExpressionVisitor<T>(IDatabase db, PocoData pocoData);
        /// <summary>
        /// 创建 SQL 表达式访问器。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">当前数据库实例。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <param name="prefixTableName">是否为列名添加表名前缀。</param>
        /// <returns>SQL 表达式访问器。</returns>
        ISqlExpression<T> ExpressionVisitor<T>(IDatabase db, PocoData pocoData, bool prefixTableName);
        /// <summary>
        /// 格式化数据库命令。
        /// </summary>
        /// <param name="cmd">要格式化的命令。</param>
        /// <returns>格式化后的命令文本。</returns>
        string FormatCommand(DbCommand cmd);
        /// <summary>
        /// 格式化 SQL 语句与参数。
        /// </summary>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>格式化后的 SQL 文本。</returns>
        string FormatCommand(string sql, object[] args);
        /// <summary>
        /// 获取自增主键表达式。
        /// </summary>
        /// <param name="ti">表信息。</param>
        /// <returns>自增主键表达式，若不需要则返回 null。</returns>
        string GetAutoIncrementExpression(TableInfo ti);
        /// <summary>
        /// 获取默认的插入语句。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="useOutputClause">是否使用 OUTPUT 子句。</param>
        /// <param name="names">列名数组。</param>
        /// <param name="parameters">参数名数组。</param>
        /// <returns>默认的插入语句。</returns>
        string GetDefaultInsertSql(string tableName, string primaryKeyName, bool useOutputClause, string[] names, string[] parameters);
        /// <summary>
        /// 获取默认的事务隔离级别。
        /// </summary>
        /// <returns>默认的事务隔离级别。</returns>
        IsolationLevel GetDefaultTransactionIsolationLevel();
        /// <summary>
        /// 获取判断记录是否存在的 SQL 语句。
        /// </summary>
        /// <returns>判断记录是否存在的 SQL 语句。</returns>
        string GetExistsSql();
        /// <summary>
        /// 获取插入时的 OUTPUT 子句。
        /// </summary>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="useOutputClause">是否使用 OUTPUT 子句。</param>
        /// <returns>OUTPUT 子句文本。</returns>
        string GetInsertOutputClause(string primaryKeyName, bool useOutputClause);
        /// <summary>
        /// 获取参数前缀。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>参数前缀。</returns>
        string GetParameterPrefix(string connectionString);
        /// <summary>
        /// 获取提供程序名称。
        /// </summary>
        /// <returns>提供程序名称。</returns>
        string GetProviderName();
        /// <summary>
        /// 批量插入对象集合。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">当前数据库实例。</param>
        /// <param name="pocos">待插入的对象集合。</param>
        /// <param name="options">批量插入选项。</param>
        void InsertBulk<T>(IDatabase db, IEnumerable<T> pocos, InsertBulkOptions options);
        /// <summary>
        /// 异步批量插入对象集合。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">当前数据库实例。</param>
        /// <param name="pocos">待插入的对象集合。</param>
        /// <param name="options">批量插入选项。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>表示异步操作的任务。</returns>
        Task InsertBulkAsync<T>(IDatabase db, IEnumerable<T> pocos, InsertBulkOptions options, CancellationToken cancellationToken = default);
        /// <summary>
        /// 查找指定类型与列名对应的数据库类型。
        /// </summary>
        /// <param name="type">CLR 类型。</param>
        /// <param name="name">列名。</param>
        /// <returns>对应的数据库类型，若无法确定则返回 null。</returns>
        DbType? LookupDbType(Type type, string name);
        /// <summary>
        /// 将参数值映射为数据库可接受的类型。
        /// </summary>
        /// <param name="value">原始参数值。</param>
        /// <returns>映射后的参数值。</returns>
        object MapParameterValue(object value);
        /// <summary>
        /// 在命令执行前进行预处理。
        /// </summary>
        /// <param name="cmd">要预处理的命令。</param>
        void PreExecute(DbCommand cmd);
        /// <summary>
        /// 对列值应用默认映射处理。
        /// </summary>
        /// <param name="pocoColumn">列信息。</param>
        /// <param name="value">原始列值。</param>
        /// <returns>映射处理后的列值。</returns>
        object ProcessDefaultMappings(PocoColumn pocoColumn, object value);
        /// <summary>
        /// 判断是否使用列别名。
        /// </summary>
        /// <returns>使用列别名返回 true，否则返回 false。</returns>
        bool UseColumnAliases();
    }
}
