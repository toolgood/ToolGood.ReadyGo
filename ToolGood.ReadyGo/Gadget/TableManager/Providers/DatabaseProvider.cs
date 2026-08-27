using System;
using ToolGood.ReadyGo.Gadget.TableManager.Providers;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget.TableManager
{
    /// <summary>
    /// 数据库提供程序基类（生成各数据库方言的建表/删表/清空表 SQL）
    /// </summary>
    public abstract class DatabaseProvider
    {
        /// <summary>
        /// 获取“表不存在时创建”的建表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="withIndex">是否同时生成索引</param>
        /// <returns>建表 SQL，默认返回 null</returns>
        public virtual string GetTryCreateTable(Type type, bool withIndex = true)
        {
            return null;
        }

        /// <summary>
        /// 获取建表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="withIndex">是否同时生成索引</param>
        /// <returns>建表 SQL</returns>
        public virtual string GetCreateTable(Type type, bool withIndex = true)
        {
            return GetTryCreateTable(type, withIndex);
        }

        /// <summary>
        /// 获取创建索引 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>创建索引 SQL，默认返回 null</returns>
        public virtual string GetCreateIndex(Type type)
        {
            return null;
        }

        /// <summary>
        /// 获取删除表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>删除表 SQL，默认返回 null</returns>
        public virtual string GetDropTable(Type type)
        {
            return null;
        }

        /// <summary>
        /// 获取删除表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>删除表 SQL，默认返回 null</returns>
        public virtual string GetDropTable(string tableName)
        {
            return null;
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>清空表 SQL，默认返回 null</returns>
        public virtual string GetTruncateTable(Type type)
        {
            return null;
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>清空表 SQL，默认返回 null</returns>
        public virtual string GetTruncateTable(string tableName)
        {
            return null;
        }

        //protected virtual string EscapeSqlIdentifier(string name)
        //{
        //    return "[" + name + "]";
        //}

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="data">表结构信息</param>
        /// <returns>转义后的表名</returns>
        public string GetTableName(TableInfo data)
        {
            var ti = data;
            var schemaName = ti.SchemaName;
            var tableName = ti.TableName;
            return GetTableName(schemaName, tableName);
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="schemaName">模式名</param>
        /// <param name="tableName">表名</param>
        /// <returns>转义后的表名</returns>
        public virtual string GetTableName(string schemaName, string tableName)
        {
            if (string.IsNullOrEmpty(schemaName) == false) {
                return $"[{EscapeBrackets(schemaName)}].[{EscapeBrackets(tableName)}]";
            }
            return $"[{EscapeBrackets(tableName)}]";
        }

        /// <summary>
        /// 对方括号引用风格的标识符做内部转义（"]]" 表示字面 "]"）
        /// </summary>
        protected static string EscapeBrackets(string name)
        {
            return name?.Replace("]", "]]") ?? string.Empty;
        }

        /// <summary>
        /// 确保表结构存在可映射列，避免生成无效的建表 SQL
        /// </summary>
        /// <param name="ti">表结构信息</param>
        protected static void EnsureColumns(TableInfo ti)
        {
            if (ti.Columns == null || ti.Columns.Count == 0) {
                throw new InvalidOperationException($"类型 {ti.TableName} 没有可映射的列，无法生成建表 SQL。");
            }
        }

        internal static DatabaseProvider Resolve(SqlType type)
        {
            switch (type) {
                case SqlType.SqlServer: return Singleton<SqlServerDatabaseProvider>.Instance;
                case SqlType.MySql: return Singleton<MySqlDatabaseProvider>.Instance;
                case SqlType.SQLite: return Singleton<SQLiteDatabaseProvider>.Instance;
                case SqlType.DuckDb: return Singleton<DuckDbDatabaseProvider>.Instance;
                case SqlType.MsAccessDb: return Singleton<MsAccessDbDatabaseProvider>.Instance;
                case SqlType.Oracle: return Singleton<OracleDatabaseProvider>.Instance;
                case SqlType.PostgreSQL: return Singleton<PostgreSQLDatabaseProvider>.Instance;
                case SqlType.FirebirdDb: return Singleton<FirebirdDbDatabaseProvider>.Instance;
                case SqlType.MariaDb: return Singleton<MariaDbDatabaseProvider>.Instance;
                //case SqlType.SqlServerCE: return Singleton<SqlServerCEDatabaseProviders>.Instance;
                default: break;
            }
            throw new NotSupportedException();
            //return Singleton<SqlServerDatabaseProvider>.Instance;
        }
    }
}
