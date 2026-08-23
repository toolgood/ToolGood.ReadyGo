using System;

namespace ToolGood.ReadyGo.NPoco.Expressions
{
    /// <summary>
    /// Firebird 数据库方言的 SQL 表达式生成器。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public class FirebirdSqlExpression<T> : SqlExpression<T>
    {
        /// <summary>
        /// 使用指定数据库、Poco 元数据与表名前缀标志初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pocoData">实体的 Poco 元数据。</param>
        /// <param name="prefixTableName">是否在字段前添加表名前缀。</param>
        public FirebirdSqlExpression(IDatabase database, PocoData pocoData, bool prefixTableName) : base(database, pocoData, prefixTableName)
        {
        }

        /// <summary>
        /// 使用指定数据库与 Poco 元数据初始化实例，默认不带表名前缀。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pocoData">实体的 Poco 元数据。</param>
        public FirebirdSqlExpression(IDatabase database, PocoData pocoData) : base(database, pocoData, false)
        {
        }

        /// <summary>
        /// 生成 Firebird 的 substring 子串 SQL。
        /// </summary>
        /// <param name="columnName">列名 SQL 片段。</param>
        /// <param name="startIndex">起始位置。</param>
        /// <param name="length">子串长度，小于 0 表示取到末尾。</param>
        /// <returns>substring 子串 SQL。</returns>
        protected override string SubstringStatement(PartialSqlString columnName, int startIndex, int length)
        {
            // Substring function doesn't work with parameters
            if (length >= 0)
                return string.Format("substring({0} FROM {1} FOR {2})", columnName, startIndex, length);
            else
                return string.Format("substring({0} FROM {1})", columnName, startIndex);
        }

        /// <summary>
        /// 根据成员名生成 Firebird 的日期时间取值 SQL。
        /// </summary>
        /// <param name="memberName">DateTime 成员名，如 Year、Month、Day 等。</param>
        /// <param name="m">字段的 SQL 片段。</param>
        /// <returns>对应的日期时间取值 SQL。</returns>
        protected override string GetDateTimeSql(string memberName, object m)
        {
            //  http://www.firebirdsql.org/refdocs/langrefupd21.html
            string sql;
            switch (memberName)
            {
                case "Year": sql = $"EXTRACT(YEAR FROM {m})"; break;
                case "Month": sql = $"EXTRACT(MONTH FROM {m})"; break;
                case "Day": sql = $"EXTRACT(DAY FROM {m})"; break;
                case "Hour": sql = $"EXTRACT(HOUR FROM {m})"; break;
                case "Minute": sql = $"EXTRACT(MINUTE FROM {m})"; break;
                case "Second": sql = $"EXTRACT(SECOND FROM {m})"; break;
                default: throw new NotSupportedException("Not Supported " + memberName);
            }
            return sql;
        }
    }
}