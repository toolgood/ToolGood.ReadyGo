using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using ToolGood.ReadyGo.NPoco.DatabaseTypes;
using ToolGood.ReadyGo.NPoco.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// Base class for DatabaseType handlers - provides default/common handling for different database engines
    /// </summary>
    public abstract partial class DatabaseType : IDatabaseType
    {
        // Helper Properties
        /// <summary>
        /// 获取 SQL Server 2012 数据库类型处理器实例。
        /// </summary>
        public static DatabaseType SqlServer2012 { get { return DynamicDatabaseType.MakeSqlServerType("SqlServer2012DatabaseType"); } }

        /// <summary>
        /// 获取 PostgreSQL 数据库类型处理器实例。
        /// </summary>
        public static DatabaseType PostgreSQL { get { return Singleton<PostgreSQLDatabaseType>.Instance; } }

        /// <summary>
        /// 获取 Oracle 数据库类型处理器实例。
        /// </summary>
        public static DatabaseType Oracle { get { return Singleton<OracleDatabaseType>.Instance; } }

        /// <summary>
        /// 获取 MySQL 数据库类型处理器实例。
        /// </summary>
        public static DatabaseType MySQL { get { return Singleton<MySqlDatabaseType>.Instance; } }

        /// <summary>
        /// 获取 SQLite 数据库类型处理器实例。
        /// </summary>
        public static DatabaseType SQLite { get { return Singleton<SQLiteDatabaseType>.Instance; } }

        /// <summary>
        /// 获取 Firebird 数据库类型处理器实例。
        /// </summary>
        public static DatabaseType Firebird { get { return Singleton<FirebirdDatabaseType>.Instance; } }

        /// <summary>
        /// 获取 DuckDb 数据库类型处理器实例。
        /// </summary>
        public static DatabaseType DuckDb { get { return Singleton<DuckDbDatabaseType>.Instance; } }

        readonly Dictionary<Type, DbType> typeMap;

        /// <summary>
        /// 初始化 <see cref="DatabaseType"/> 实例，并注册常见 .NET 类型到 <see cref="DbType"/> 的映射。
        /// </summary>
        public DatabaseType()
        {
            typeMap = new Dictionary<Type, DbType>();
            typeMap[typeof(byte)] = DbType.Byte;
            typeMap[typeof(sbyte)] = DbType.SByte;
            typeMap[typeof(short)] = DbType.Int16;
            typeMap[typeof(ushort)] = DbType.UInt16;
            typeMap[typeof(int)] = DbType.Int32;
            typeMap[typeof(uint)] = DbType.UInt32;
            typeMap[typeof(long)] = DbType.Int64;
            typeMap[typeof(ulong)] = DbType.UInt64;
            typeMap[typeof(float)] = DbType.Single;
            typeMap[typeof(double)] = DbType.Double;
            typeMap[typeof(decimal)] = DbType.Decimal;
            typeMap[typeof(bool)] = DbType.Boolean;
            typeMap[typeof(string)] = DbType.String;
            typeMap[typeof(char)] = DbType.StringFixedLength;
            typeMap[typeof(Guid)] = DbType.Guid;
            typeMap[typeof(DateTime)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            typeMap[typeof(TimeSpan)] = DbType.Time;
            typeMap[typeof(byte[])] = DbType.Binary;
            typeMap[typeof(byte?)] = DbType.Byte;
            typeMap[typeof(sbyte?)] = DbType.SByte;
            typeMap[typeof(short?)] = DbType.Int16;
            typeMap[typeof(ushort?)] = DbType.UInt16;
            typeMap[typeof(int?)] = DbType.Int32;
            typeMap[typeof(uint?)] = DbType.UInt32;
            typeMap[typeof(long?)] = DbType.Int64;
            typeMap[typeof(ulong?)] = DbType.UInt64;
            typeMap[typeof(float?)] = DbType.Single;
            typeMap[typeof(double?)] = DbType.Double;
            typeMap[typeof(decimal?)] = DbType.Decimal;
            typeMap[typeof(bool?)] = DbType.Boolean;
            typeMap[typeof(char?)] = DbType.StringFixedLength;
            typeMap[typeof(Guid?)] = DbType.Guid;
            typeMap[typeof(DateTime?)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            typeMap[typeof(TimeSpan?)] = DbType.Time;
            typeMap[typeof(Object)] = DbType.Object;
#if NET6_0_OR_GREATER
            typeMap[typeof(DateOnly)] = DbType.Date;
            typeMap[typeof(DateOnly?)] = DbType.Date;
#endif
        }

        private const string LinqBinary = "System.Data.Linq.Binary";
        /// <summary>
        /// 根据 CLR 类型与列名查找对应的 <see cref="DbType"/>。
        /// </summary>
        /// <param name="type">CLR 类型。</param>
        /// <param name="name">列名。</param>
        /// <returns>对应的 DbType；若无法确定则返回 null。</returns>
        public virtual DbType? LookupDbType(Type type, string name)
        {
            DbType dbType;
            var nullUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullUnderlyingType != null) type = nullUnderlyingType;
            if (type.GetTypeInfo().IsEnum && !typeMap.ContainsKey(type))
            {
                type = Enum.GetUnderlyingType(type);
            }
            if (typeMap.TryGetValue(type, out dbType))
            {
                return dbType;
            }
            if (type.FullName == LinqBinary)
            {
                return DbType.Binary;
            }

            return null;
        }

        /// <summary>
        /// Returns the prefix used to delimit parameters in SQL query strings.
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>参数前缀字符串。</returns>
        public virtual string GetParameterPrefix(string connectionString)
        {
            return "@";
        }

        /// <summary>
        /// Converts a supplied C# object value into a value suitable for passing to the database
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value</returns>
        public virtual object MapParameterValue(object value)
        {
            // Cast bools to integer
            if (value is bool)
            {
                return ((bool)value) ? 1 : 0;
            }

            // Leave it
            return value;
        }

        /// <summary>
        /// Called immediately before a command is executed, allowing for modification of the DbCommand before it's passed to the database provider
        /// </summary>
        /// <param name="cmd">即将执行的数据库命令。</param>
        public virtual void PreExecute(DbCommand cmd)
        {
        }

        /// <summary>
        /// Builds an SQL query suitable for performing page based queries to the database
        /// </summary>
        /// <param name="skip">The number of rows that should be skipped by the query</param>
        /// <param name="take">The number of rows that should be retruend by the query</param>
        /// <param name="parts">The original SQL query after being parsed into it's component parts</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL query</param>
        /// <returns>The final SQL query that should be executed.</returns>
        public virtual string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var sql = string.Format("{0}\nLIMIT @{1} OFFSET @{2}", parts.sql, args.Length, args.Length + 1);
            args = args.Concat(new object[] { take, skip }).ToArray();
            return sql;
        }

        /// <summary>
        /// 指示是否在查询中使用列别名。
        /// </summary>
        /// <returns>默认返回 false。</returns>
        public virtual bool UseColumnAliases()
        {
            return false;
        }

        /// <summary>
        /// Returns an SQL Statement that can check for the existance of a row in the database.
        /// </summary>
        /// <returns>用于检查记录是否存在的 SQL 语句模板。</returns>
        public virtual string GetExistsSql()
        {
            return "SELECT COUNT(*) FROM {0} WHERE {1}";
        }

        /// <summary>
        /// Escape a tablename into a suitable format for the associated database provider.
        /// </summary>
        /// <param name="tableName">The name of the table (as specified by the client program, or as attributes on the associated POCO class.</param>
        /// <returns>The escaped table name</returns>
        public virtual string EscapeTableName(string tableName)
        {
            // Assume table names with "dot" are already escaped
            return tableName.IndexOf('.') >= 0 ? tableName : EscapeSqlIdentifier(tableName);
        }

        /// <summary>
        /// Escape and arbitary SQL identifier into a format suitable for the associated database provider
        /// </summary>
        /// <param name="str">The SQL identifier to be escaped</param>
        /// <returns>The escaped identifier</returns>
        public virtual string EscapeSqlIdentifier(string str)
        {
            return string.Format("[{0}]", str);
        }

        /// <summary>
        /// Return an SQL expression that can be used to populate the primary key column of an auto-increment column.
        /// </summary>
        /// <param name="ti">Table info describing the table</param>
        /// <returns>An SQL expressions</returns>
        /// <remarks>See the Oracle database type for an example of how this method is used.</remarks>
        public virtual string GetAutoIncrementExpression(TableInfo ti)
        {
            return null;
        }

        /// <summary>
        /// Returns an SQL expression that can be used to specify the return value of auto incremented columns.
        /// </summary>
        /// <param name="primaryKeyName">The primary key of the row being inserted.</param>
        /// <param name="useOutputClause">是否使用输出子句。</param>
        /// <returns>An expression describing how to return the new primary key value</returns>
        /// <remarks>See the SQLServer database provider for an example of how this method is used.</remarks>
        public virtual string GetInsertOutputClause(string primaryKeyName, bool useOutputClause)
        {
            return string.Empty;
        }

        /// <summary>
        /// Performs an Insert operation
        /// </summary>
        /// <param name="db">The calling Database object</param>
        /// <param name="cmd">The insert command to be executed</param>
        /// <param name="primaryKeyName">The primary key of the table being inserted into</param>
        /// <param name="useOutputClause">是否使用输出子句返回自增值。</param>
        /// <param name="poco">要插入的 POCO 对象。</param>
        /// <param name="args">插入语句的参数。</param>
        /// <returns>The ID of the newly inserted record</returns>
        public virtual object ExecuteInsert<T>(IDatabase db, DbCommand cmd, string primaryKeyName, bool useOutputClause, T poco, object[] args)
        {
            cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            return ((IDatabaseHelpers)db).ExecuteScalarHelper(cmd);
        }

        /// <summary>
        /// 异步执行插入操作。
        /// </summary>
        /// <param name="db">调用方数据库实例。</param>
        /// <param name="cmd">要执行的插入命令。</param>
        /// <param name="primaryKeyName">表的主键列名。</param>
        /// <param name="useOutputClause">是否使用输出子句返回自增值。</param>
        /// <param name="poco">要插入的 POCO 对象。</param>
        /// <param name="args">插入语句的参数。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>新插入记录的主键值。</returns>
        public virtual async Task<object> ExecuteInsertAsync<T>(IDatabase db, DbCommand cmd, string primaryKeyName, bool useOutputClause, T poco, object[] args, CancellationToken cancellationToken = default)
        {
            cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            return await ((IDatabaseHelpers)db).ExecuteScalarHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 批量插入 POCO 集合，默认逐条插入。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="pocos">要插入的 POCO 集合。</param>
        /// <param name="options">批量插入选项。</param>
        public virtual void InsertBulk<T>(IDatabase db, IEnumerable<T> pocos, InsertBulkOptions options)
        {
            foreach (var poco in pocos)
            {
                db.Insert(poco);
            }
        }

        /// <summary>
        /// 异步批量插入 POCO 集合，默认逐条插入。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="pocos">要插入的 POCO 集合。</param>
        /// <param name="options">批量插入选项。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        public virtual async Task InsertBulkAsync<T>(IDatabase db, IEnumerable<T> pocos, InsertBulkOptions options, CancellationToken cancellationToken = default)
        {
            foreach (var poco in pocos)
            {
                await db.InsertAsync(poco, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Look at the type and provider name being used and instantiate a suitable DatabaseType instance.
        /// </summary>
        /// <param name="typeName">数据库连接类型名称。</param>
        /// <param name="providerName">数据库提供程序名称。</param>
        /// <returns>解析出的数据库类型处理器实例。</returns>
        public static DatabaseType Resolve(string typeName, string providerName)
        {
            // Try using type name first (more reliable)
            if (typeName.StartsWith("MySql"))
                return Singleton<MySqlDatabaseType>.Instance;
            if (typeName.StartsWith("SqlCe"))
                return DynamicDatabaseType.MakeSqlServerType("SqlServerCEDatabaseType");
            if (typeName.StartsWith("Npgsql") || typeName.StartsWith("PgSql"))
                return Singleton<PostgreSQLDatabaseType>.Instance;
            if (typeName.StartsWith("OracleManaged"))
                return Singleton<OracleDatabaseType>.Instance;
            if (typeName.StartsWith("Oracle"))
                return Singleton<OracleDatabaseType>.Instance;
            if (typeName.StartsWith("SQLite", StringComparison.OrdinalIgnoreCase))
                return Singleton<SQLiteDatabaseType>.Instance;
            if (typeName.StartsWith("SqlConnection"))
                return DynamicDatabaseType.MakeSqlServerType("SqlServerDatabaseType");
            if (typeName.StartsWith("Fb") || typeName.StartsWith("Firebird"))
                return Singleton<FirebirdDatabaseType>.Instance;

            if (!string.IsNullOrEmpty(providerName))
            {
                // Try again with provider name
                if (providerName.IndexOf("MySql", StringComparison.OrdinalIgnoreCase) >= 0)
                    return Singleton<MySqlDatabaseType>.Instance;
                if (providerName.IndexOf("SqlServerCe", StringComparison.OrdinalIgnoreCase) >= 0)
                    return DynamicDatabaseType.MakeSqlServerType("SqlServerCEDatabaseType");
                if (providerName.IndexOf("pgsql", StringComparison.OrdinalIgnoreCase) >= 0)
                    return Singleton<PostgreSQLDatabaseType>.Instance;
                if (providerName.IndexOf("Oracle.DataAccess", StringComparison.OrdinalIgnoreCase) >= 0)
                    return Singleton<OracleDatabaseType>.Instance;
                if (providerName.IndexOf("Oracle.ManagedDataAccess", StringComparison.OrdinalIgnoreCase) >= 0)
                    return Singleton<OracleManagedDatabaseType>.Instance;
                if (providerName.IndexOf("SQLite", StringComparison.OrdinalIgnoreCase) >= 0)
                    return Singleton<SQLiteDatabaseType>.Instance;
                if (providerName.IndexOf("Firebird", StringComparison.OrdinalIgnoreCase) >= 0)
                    return Singleton<FirebirdDatabaseType>.Instance;
            }

            // Assume SQL Server
            return DynamicDatabaseType.MakeSqlServerType("SqlServerDatabaseType");
        }

        /// <summary>
        /// 生成默认的插入 SQL 语句。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="useOutputClause">是否使用输出子句。</param>
        /// <param name="names">列名数组。</param>
        /// <param name="parameters">参数名数组。</param>
        /// <returns>默认插入 SQL 语句。</returns>
        public virtual string GetDefaultInsertSql(string tableName, string primaryKeyName, bool useOutputClause, string[] names, string[] parameters)
        {
            return string.Format("INSERT INTO {0} DEFAULT VALUES", EscapeTableName(tableName));
        }

        /// <summary>
        /// 获取默认的事务隔离级别。
        /// </summary>
        /// <returns>默认事务隔离级别（ReadCommitted）。</returns>
        public virtual IsolationLevel GetDefaultTransactionIsolationLevel()
        {
            return IsolationLevel.ReadCommitted;
        }

        /// <summary>
        /// 创建 SQL 表达式访问器实例。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <returns>SQL 表达式访问器实例。</returns>
        public ISqlExpression<T> ExpressionVisitor<T>(IDatabase db, PocoData pocoData)
        {
            return ExpressionVisitor<T>(db, pocoData, false);
        }

        /// <summary>
        /// 创建 SQL 表达式访问器实例。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <param name="prefixTableName">是否为列名添加表名前缀。</param>
        /// <returns>SQL 表达式访问器实例。</returns>
        public virtual ISqlExpression<T> ExpressionVisitor<T>(IDatabase db, PocoData pocoData, bool prefixTableName)
        {
            return new DefaultSqlExpression<T>(db, pocoData, prefixTableName);
        }

        /// <summary>
        /// 获取数据库提供程序名称。
        /// </summary>
        /// <returns>提供程序名称字符串。</returns>
        public virtual string GetProviderName()
        {
            return "Microsoft.Data.SqlClient";
        }

        /// <summary>
        /// 异步执行非查询命令。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="cmd">要执行的命令。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public virtual Task<int> ExecuteNonQueryAsync(IDatabase database, DbCommand cmd, CancellationToken cancellationToken = default)
        {
            return cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <summary>
        /// 异步执行标量查询。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="cmd">要执行的命令。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>查询结果的首行首列值。</returns>
        public virtual Task<object> ExecuteScalarAsync(IDatabase database, DbCommand cmd, CancellationToken cancellationToken = default)
        {
            return cmd.ExecuteScalarAsync(cancellationToken);
        }

        /// <summary>
        /// 异步执行返回数据读取器的查询。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="cmd">要执行的命令。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>数据读取器。</returns>
        public virtual Task<DbDataReader> ExecuteReaderAsync(IDatabase database, DbCommand cmd, CancellationToken cancellationToken = default)
        {
            return cmd.ExecuteReaderAsync(cancellationToken);
        }

        /// <summary>
        /// 对从数据库读取的列值执行默认映射处理。
        /// </summary>
        /// <param name="pocoColumn">列元数据。</param>
        /// <param name="value">列值。</param>
        /// <returns>处理后的值。</returns>
        public virtual object ProcessDefaultMappings(PocoColumn pocoColumn, object value)
        {
            return value;
        }

        /// <summary>
        /// 格式化数据库命令为可读字符串。
        /// </summary>
        /// <param name="cmd">要格式化的命令。</param>
        /// <returns>格式化后的命令文本。</returns>
        public virtual string FormatCommand(DbCommand cmd)
        {
            return FormatCommand(cmd.CommandText, cmd.Parameters.Cast<object>().ToArray());
        }

        /// <summary>
        /// 格式化 SQL 语句与参数为可读字符串。
        /// </summary>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>格式化后的命令文本。</returns>
        public virtual string FormatCommand(string sql, object[] args)
        {
            if (sql == null)
                return "";

            var sb = new StringBuilder();
            sb.Append(sql);
            if (args != null && args.Length > 0)
            {
                sb.Append("\n");
                for (int i = 0; i < args.Length; i++)
                {
                    string type; 
                    string value;

                    if (args[i] is DbParameter dbParameter)
                    {
                        type = $"{dbParameter.GetType().Name}, {dbParameter.DbType.ToString()}";
                        value = dbParameter.Value?.ToString();
                    }
                    else
                    {
                        type = args[i].GetTheType()?.Name;
                        value = args[i]?.ToString();
                    }

                    sb.AppendFormat("\t -> {0}{1} [{2}] = \"{3}\"\n", GetParameterPrefix(string.Empty), i, type, value);
                }
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

    }
}
