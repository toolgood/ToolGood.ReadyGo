using System;
using System.Data;
using System.Data.Common;
using ToolGood.ReadyGo.NPoco.Expressions;

namespace ToolGood.ReadyGo.NPoco.DatabaseTypes
{
    /// <summary>
    /// MySQL 数据库类型实现。
    /// </summary>
    public class MySqlDatabaseType : DatabaseType
    {
        /// <summary>
        /// 获取 MySQL 的参数前缀；启用用户变量时使用“?”，否则使用“@”。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>参数前缀字符串。</returns>
        public override string GetParameterPrefix(string connectionString)
        {
            if (connectionString != null && connectionString.Replace(" ", string.Empty).IndexOf("AllowUserVariables=true", StringComparison.OrdinalIgnoreCase) >= 0)
                return "?";

            return "@";
        }

        /// <summary>
        /// 在执行命令前预处理命令文本，将 poco_dual 占位符替换为 MySQL 的 dual 表。
        /// </summary>
        /// <param name="cmd">数据库命令。</param>
        public override void PreExecute(DbCommand cmd)
        {
            cmd.CommandText = cmd.CommandText.Replace("/*poco_dual*/", "from dual");
        }

        /// <summary>
        /// MySQL 字符串字面量中反斜杠为转义字符，需双写反斜杠表示一个反斜杠。
        /// </summary>
        public override string LikeEscapeLiteral { get { return "'\\\\'"; } }

        /// <summary>
        /// 使用反引号包裹 SQL 标识符。
        /// </summary>
        /// <param name="str">标识符。</param>
        /// <returns>转义后的标识符。</returns>
        public override string EscapeSqlIdentifier(string str)
        {
            return string.Format("`{0}`", str);
        }

        /// <summary>
        /// 获取 EXISTS 查询的 SQL 模板。
        /// </summary>
        /// <returns>EXISTS 查询 SQL 模板。</returns>
        public override string GetExistsSql()
        {
            return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
        }

        /// <summary>
        /// 获取默认的 INSERT 语句。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键名称。</param>
        /// <param name="useOutputClause">是否使用 OUTPUT 子句。</param>
        /// <param name="names">列名数组。</param>
        /// <param name="parameters">参数名数组。</param>
        /// <returns>INSERT 语句。</returns>
        public override string GetDefaultInsertSql(string tableName, string primaryKeyName, bool useOutputClause, string[] names, string[] parameters)
        {
            return string.Format("INSERT INTO {0} ({1}) VALUES ({2})", EscapeTableName(tableName), string.Join(",", names), string.Join(",", parameters));
        }

        /// <summary>
        /// 获取 MySQL 驱动提供程序名称。
        /// </summary>
        /// <returns>提供程序名称。</returns>
        public override string GetProviderName()
        {
            return "MySql.Data.MySQLClient";
        }

        /// <summary>
        /// 获取默认的事务隔离级别。
        /// </summary>
        /// <returns>默认为 RepeatableRead。</returns>
        public override IsolationLevel GetDefaultTransactionIsolationLevel()
        {
            return IsolationLevel.RepeatableRead;
        }

        /// <summary>
        /// 创建 MySQL SQL 表达式访问器。
        /// </summary>
        /// <typeparam name="T">POCO 类型。</typeparam>
        /// <param name="db">数据库实例。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <param name="prefixTableName">是否添加表名前缀。</param>
        /// <returns>SQL 表达式访问器。</returns>
        public override ISqlExpression<T> ExpressionVisitor<T>(IDatabase db, PocoData pocoData, bool prefixTableName)
        {
            return new MySqlSqlExpression<T>(db, pocoData, prefixTableName);
        }
    }
}