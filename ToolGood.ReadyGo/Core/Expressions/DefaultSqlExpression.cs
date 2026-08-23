namespace ToolGood.ReadyGo.NPoco.Expressions
{
    /// <summary>
    /// 默认 SQL 表达式生成器，用于构建标准数据库方言的查询、更新与删除 SQL。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public class DefaultSqlExpression<T> : SqlExpression<T>
    {
        /// <summary>
        /// 使用指定数据库、Poco 元数据与表名前缀标志初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pocoData">实体的 Poco 元数据。</param>
        /// <param name="prefixTableName">是否在字段前添加表名前缀。</param>
        public DefaultSqlExpression(IDatabase database, PocoData pocoData, bool prefixTableName) : base(database, pocoData, prefixTableName)
        {
        }

        /// <summary>
        /// 使用指定数据库与 Poco 元数据初始化实例，默认不带表名前缀。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pocoData">实体的 Poco 元数据。</param>
        public DefaultSqlExpression(IDatabase database, PocoData pocoData) : base(database, pocoData, false)
        {
        }

        /// <summary>
        /// 使用指定数据库与表名前缀标志初始化实例，自动获取 <typeparamref name="T"/> 的 Poco 元数据。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="prefixTableName">是否在字段前添加表名前缀。</param>
        public DefaultSqlExpression(IDatabase database, bool prefixTableName)
            : this(database, database.PocoDataFactory.ForType(typeof(T)), prefixTableName)
        {
        }

        /// <summary>
        /// 使用指定数据库初始化实例，自动获取 <typeparamref name="T"/> 的 Poco 元数据。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        public DefaultSqlExpression(IDatabase database) 
            : this(database, database.PocoDataFactory.ForType(typeof(T)))
        {
        }
    }
}