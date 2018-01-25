using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.LinQ;

namespace ToolGood.ReadyGo3
{
    public partial class SqlHelper
    {
        /// <summary>
        /// 创建 WhereHelper
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public WhereHelper<T> CreateWhere<T>() where T : class, new()
        {
            var where = new WhereHelper<T>(this);
            return where;
        }
         
    }

}
