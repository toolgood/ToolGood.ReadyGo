using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt.Exceptions
{
    /// <summary>
    /// 数据库不支付异常
    /// </summary>
    public class DatabaseUnsupportException : Exception
    {
        public DatabaseUnsupportException() { }

        public DatabaseUnsupportException(string databaseName, string type)
        {
            
        }


    }
}
