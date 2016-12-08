namespace ToolGood.ReadyGo.Internals
{
    /// <summary>
    /// SQL 类型
    /// </summary>
    public enum SqlType
    {
        None,
        /// <summary>
        /// Sql Server 数据库
        /// </summary>
        SqlServer,
        /// <summary>
        /// MySql 数据库
        /// </summary>
        MySql,
        /// <summary>
        /// SQLite 数据库
        /// </summary>
        SQLite,
        /// <summary>
        /// Access 数据库
        /// </summary>
        MsAccessDb,
        /// <summary>
        /// Oracle 数据库
        /// </summary>
        Oracle,
        /// <summary>
        /// PostgreSQL 数据库
        /// </summary>
        PostgreSQL,
        /// <summary>
        /// Firebird 数据库
        /// </summary>
        FirebirdDb,
        /// <summary>
        /// Maria 数据库
        /// </summary>
        MariaDb,
        /// <summary>
        /// Sql Server CE 数据库
        /// </summary>
        SqlServerCE,
        /// <summary>
        /// Sql Server 2012 数据库
        /// </summary>
        SqlServer2012
    }
}