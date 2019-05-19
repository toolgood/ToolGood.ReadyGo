using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ToolGood.ReadyGo3.LinQ;

namespace ToolGood.ReadyGo3
{
    public partial class SqlHelper
    {
        /// <summary>
        /// 动态Sql拼接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public WhereHelper<T> Where<T>() where T : class//, new()
        {
            var where = new WhereHelper<T>(this);
            return where;
        }

        /// <summary>
        /// 动态Sql拼接， 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereHelper<T> Where<T>(string where) where T : class//, new()
        {
            var whereHelper = new WhereHelper<T>(this);
            whereHelper.Where(where);

            return whereHelper;
        }

        /// <summary>
        /// 动态Sql拼接， 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public WhereHelper<T> Where<T>(string where, params object[] args) where T : class//, new()
        {
            var whereHelper = new WhereHelper<T>(this);
            if (string.IsNullOrEmpty(where)) throw new ArgumentNullException("where");
            whereHelper.where(where, args);
            return whereHelper;
        }

        /// <summary>
        /// 动态Sql拼接
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereHelper<T> Where<T>(Expression<Func<T, bool>> where) where T : class//, new()
        {
            var whereHelper = new WhereHelper<T>(this);
            whereHelper.Where(where);
            return whereHelper;
        }

    }

}
