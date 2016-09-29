using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.SqlBuilding;

namespace ToolGood.ReadyGo.Providers
{
    public abstract partial class DatabaseProvider
    {
        /// <summary>
        ///     Gets the DbProviderFactory for this database provider.
        /// </summary>
        /// <returns>The provider factory.</returns>
        public abstract DbProviderFactory GetFactory();

        /// <summary>
        ///     Gets a flag for whether the DB has native support for GUID/UUID.
        /// </summary>
        public virtual bool HasNativeGuidSupport
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets the <seealso cref="IPagingHelper" /> this provider supplies.
        /// </summary>
        public virtual PagingHelper PagingUtility
        {
            get { return PagingHelper.Instance; }
        }

        /// <summary>
        ///     Escape and arbitary SQL identifier into a format suitable for the associated database provider
        /// </summary>
        /// <param name="sqlIdentifier">The SQL identifier to be escaped</param>
        /// <returns>The escaped identifier</returns>
        public virtual string EscapeSqlIdentifier(string sqlIdentifier)
        {
            return "[" + sqlIdentifier + "]";
            //return string.Format("[{0}]", sqlIdentifier);
        }

        /// <summary>
        ///     Returns the prefix used to delimit parameters in SQL query strings.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The providers character for prefixing a query parameter.</returns>
        public virtual string GetParameterPrefix(string connectionString)
        {
            return "@";
        }

        /// <summary>
        ///     Converts a supplied C# object value into a value suitable for passing to the database
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value</returns>
        public virtual object MapParameterValue(object value)
        {
            if (value is bool)
                return ((bool)value) ? 1 : 0;

            return value;
        }

        /// <summary>
        ///     Called immediately before a command is executed, allowing for modification of the IDbCommand before it's passed to
        ///     the database provider
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void PreExecute(IDbCommand cmd)
        {
        }

        /// <summary>
        ///     Builds an SQL query suitable for performing page based queries to the database
        /// </summary>
        /// <param name="skip">The number of rows that should be skipped by the query</param>
        /// <param name="take">The number of rows that should be retruend by the query</param>
        /// <param name="parts">The original SQL query after being parsed into it's component parts</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL query</param>
        /// <returns>The final SQL query that should be executed.</returns>
        public virtual string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var sql = string.Format("{0}\nLIMIT @{1} OFFSET @{2}", parts.Sql, args.Length, args.Length + 1);
            args = args.Concat(new object[] { take, skip }).ToArray();
            return sql;
        }

        ///// <summary>
        /////     Returns an SQL Statement that can check for the existence of a row in the database.
        ///// </summary>
        ///// <returns></returns>
        //public virtual string GetExistsSql()
        //{
        //    return "SELECT COUNT(*) FROM {0} WHERE {1}";
        //}

        /// <summary>
        ///     Return an SQL expression that can be used to populate the primary key column of an auto-increment column.
        /// </summary>
        /// <param name="tableInfo">Table info describing the table</param>
        /// <returns>An SQL expressions</returns>
        /// <remarks>See the Oracle database type for an example of how this method is used.</remarks>
        public virtual string GetAutoIncrementExpression(PocoTable tableInfo)
        {
            return null;
        }

        /// <summary>
        ///     Returns an SQL expression that can be used to specify the return value of auto incremented columns.
        /// </summary>
        /// <param name="primaryKeyName">The primary key of the row being inserted.</param>
        /// <returns>An expression describing how to return the new primary key value</returns>
        /// <remarks>See the SQLServer database provider for an example of how this method is used.</remarks>
        public virtual string GetInsertOutputClause(string primaryKeyName)
        {
            return string.Empty;
        }

        /// <summary>
        ///     Performs an Insert operation
        /// </summary>
        /// <param name="database">The calling Database object</param>
        /// <param name="cmd">The insert command to be executed</param>
        /// <param name="primaryKeyName">The primary key of the table being inserted into</param>
        /// <returns>The ID of the newly inserted record</returns>
        public virtual object ExecuteInsert(Database database, IDbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            return database.ExecuteScalarHelper(cmd);
        }

        public virtual string EscapeTableName(string tableName)
        {
            if (tableName.IndexOf('.') >= 0 || tableName[0] == '`' || tableName[0] == '[') {
                return tableName;
            }
            return EscapeSqlIdentifier(tableName);
        }

        public virtual string EscapeTableName(PocoData pd, TableNameManger tn)
        {
            var ti = pd.TableInfo;
            var index = ti.TableName.IndexOf('.');
            if (index > -1) {
                return ti.TableName.Substring(index);
            }
            var tag = ti.FixTag;
            var tableName = ti.TableName;

            var name = tn.Get(tag);
            if (name != null) {
                tableName = name.TablePrefix + tableName + name.TableSuffix;
            }
            return EscapeSqlIdentifier(tableName);
        }

        public virtual string GetTableName(PocoData pd, TableNameManger tn)
        {
            var ti = pd.TableInfo;
            if (ti.TableName.Contains(".")) {
                return ti.TableName;
            }
            var tag = ti.FixTag;
            var tableName = ti.TableName;

            var name = tn.Get(tag);
            if (name != null) {
                tableName = name.TablePrefix + tableName + name.TableSuffix;
            }
            var schemaName = ti.SchemaName;

            if (string.IsNullOrEmpty(schemaName)) {
                return this.EscapeSqlIdentifier(tableName);
            }
            return string.Format("{0}.{1}", EscapeSqlIdentifier(schemaName), EscapeSqlIdentifier(tableName));
        }

        /// <summary>
        ///     Returns the .net standard conforming DbProviderFactory.
        /// </summary>
        /// <param name="assemblyQualifiedName">The assembly qualified name of the provider factory.</param>
        /// <returns>The db provider factory.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="assemblyQualifiedName" /> does not match a type.</exception>
        protected DbProviderFactory GetFactory(string assemblyQualifiedName)
        {
            var ft = Type.GetType(assemblyQualifiedName);

            if (ft == null)
                throw new ArgumentException("Could not load the " + GetType().Name + " DbProviderFactory.");

            return (DbProviderFactory)ft.GetField("Instance").GetValue(null);
        }

        internal static DatabaseProvider Resolve(SqlType type)
        {
            switch (type) {
                case SqlType.SqlServer: return Singleton<SqlServerDatabaseProvider>.Instance;
                case SqlType.MySql: return Singleton<MySqlDatabaseProvider>.Instance;
                case SqlType.SQLite: return Singleton<SQLiteDatabaseProvider>.Instance;
                case SqlType.MsAccessDb: return Singleton<MsAccessDbDatabaseProvider>.Instance;
                case SqlType.Oracle: return Singleton<OracleDatabaseProvider>.Instance;
                case SqlType.PostgreSQL: return Singleton<PostgreSQLDatabaseProvider>.Instance;
                case SqlType.FirebirdDb: return Singleton<FirebirdDbDatabaseProvider>.Instance;
                case SqlType.MariaDb: return Singleton<MariaDbDatabaseProvider>.Instance;
                case SqlType.SqlServerCE: return Singleton<SqlServerCEDatabaseProviders>.Instance;
                default: break;
            }
            return Singleton<SqlServerDatabaseProvider>.Instance;
        }
    }
}