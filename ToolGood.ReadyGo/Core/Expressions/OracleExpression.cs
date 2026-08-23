using System;
using System.Collections.Generic;
using System.Text;

namespace ToolGood.ReadyGo.NPoco.Expressions
{
    /// <summary>
    /// Oracle 数据库方言的 SQL 表达式生成器。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public class OracleExpression<T> : SqlExpression<T>
    {
        /// <summary>
        /// 使用指定数据库、Poco 元数据与表名前缀标志初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pocoData">实体的 Poco 元数据。</param>
        /// <param name="prefixTableName">是否在字段前添加表名前缀。</param>
        public OracleExpression(IDatabase database, PocoData pocoData, bool prefixTableName) : base(database, pocoData, prefixTableName)
        {
        }

        /// <summary>
        /// 根据成员名生成 Oracle 的日期时间取值 SQL。
        /// </summary>
        /// <param name="memberName">DateTime 成员名，如 Year、Month、Day 等。</param>
        /// <param name="m">字段的 SQL 片段。</param>
        /// <returns>对应的日期时间取值 SQL。</returns>
        protected override string GetDateTimeSql(string memberName, object m)
        {
            //Oracle
            // http://blog.csdn.net/gccr/article/details/1802740
            string sql;
            switch (memberName)
            {
                case "Year": sql = $"EXTRACT(YEAR FROM TIMESTAMP {m})"; break;
                case "Month": sql = $"EXTRACT(MONTH FROM TIMESTAMP {m})"; break;
                case "Day": sql = $"EXTRACT(DAY FROM TIMESTAMP {m})"; break;
                case "Hour": sql = $"EXTRACT(HOUR FROM TIMESTAMP {m})"; break;
                case "Minute": sql = $"EXTRACT(MINUTE FROM TIMESTAMP {m})"; break;
                case "Second": sql = $"EXTRACT(SECOND FROM TIMESTAMP {m})"; break;
                default: throw new NotSupportedException("Not Supported " + memberName);
            }
            return sql;
        }
    }
}
