using System;

namespace ToolGood.ReadyGo.NPoco.Expressions
{
    /// <summary>
    /// MySQL 数据库方言的 SQL 表达式生成器。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public class MySqlSqlExpression<T> : SqlExpression<T>
    {
        /// <summary>
        /// 使用指定数据库与 Poco 元数据初始化实例，默认不带表名前缀。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pocoData">实体的 Poco 元数据。</param>
        public MySqlSqlExpression(IDatabase database, PocoData pocoData) : this(database, pocoData, false)
        {

        }

        /// <summary>
        /// 使用指定数据库、Poco 元数据与表名前缀标志初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pocoData">实体的 Poco 元数据。</param>
        /// <param name="prefixTableName">是否在字段前添加表名前缀。</param>
        public MySqlSqlExpression(IDatabase database, PocoData pocoData, bool prefixTableName) : base(database, pocoData, prefixTableName)
        {
            EscapeChar = "\\\\";
        }

        /// <summary>
        /// 对模糊匹配参数中的特殊字符进行转义。
        /// </summary>
        /// <param name="par">待转义的参数。</param>
        /// <returns>转义后的字符串。</returns>
        protected override string EscapeParam(object par)
        {
            var param = par.ToString().ToUpper();
            param = param
                .Replace("\\", EscapeChar)
                .Replace("_", "\\_")
                .Replace("%", "\\%");
            return param;
        }

        /// <summary>
        /// 根据成员名生成 MySQL 的日期时间取值 SQL。
        /// </summary>
        /// <param name="memberName">DateTime 成员名，如 Year、Month、Day 等。</param>
        /// <param name="m">字段的 SQL 片段。</param>
        /// <returns>对应的日期时间取值 SQL。</returns>
        protected override string GetDateTimeSql(string memberName, object m)
        {
            string sql;
            switch (memberName)
            {
                case "Year": sql = $"YEAR({m})"; break;
                case "Month": sql = $"MONTH({m})"; break;
                case "Day": sql = $"DAY({m})"; break;
                case "Hour": sql = $"HOUR({m})"; break;
                case "Minute": sql = $"MINUTE({m})"; break;
                case "Second": sql = $"SECOND({m})"; break;
                default: throw new NotSupportedException("Not Supported " + memberName);
            }
            return sql;
        }
    }
}