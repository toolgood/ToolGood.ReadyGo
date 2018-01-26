using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt.Exceptions
{
    /// <summary>
    /// SQL执行异常
    /// </summary>
    public class SqlExecuteException : Exception
    {
        /// <summary>
        /// SQL执行异常
        /// </summary>
        /// <param name="x"></param>
        /// <param name="sql"></param>
        public SqlExecuteException(Exception x, string sql) : base(x.Message + "\r\nSQL: " + sql)
        {
        }
    }
}
