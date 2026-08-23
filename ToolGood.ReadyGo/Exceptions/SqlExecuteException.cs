using System;

namespace ToolGood.ReadyGo.Exceptions
{
    /// <summary>
    /// SQL执行异常
    /// </summary>
    public class SqlExecuteException : Exception
    {
        /// <summary>
        /// SQL执行异常
        /// </summary>
        /// <param name="x">内部异常</param>
        /// <param name="sql">执行出错的 SQL 语句</param>
        public SqlExecuteException(Exception x, string sql) : base(x.Message + "\r\nSQL: " + sql, x)
        {
        }
    }
}
