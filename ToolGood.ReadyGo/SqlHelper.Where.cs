using System;
using System.Linq.Expressions;
using ToolGood.ReadyGo.NPoco.Linq;

namespace ToolGood.ReadyGo
{
    public partial class SqlHelper
    {
        #region Where

        /// <summary>
        /// 链式查询入口（基于 NPOCO QueryProvider）。
        /// 之后可链式使用 Where/WhereSql/OrderBy/Limit 及扩展 IfTrue* / WhereIn / WhereLike 等方法，最后用 ToList()/Count() 等执行。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>链式查询提供程序</returns>
        public IQueryProvider<T> Where<T>() where T : class
        {
            return GetDatabase().Query<T>();
        }

        /// <summary>
        /// 链式查询入口，并添加原始 SQL 条件。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="where">原始 SQL 条件，如 "Age &gt; 18"</param>
        /// <returns>链式查询提供程序</returns>
        public IQueryProvider<T> Where<T>(string where) where T : class
        {
            if (string.IsNullOrEmpty(where)) throw new ArgumentNullException(nameof(where));
            return GetDatabase().Query<T>().WhereSql(where);
        }

        /// <summary>
        /// 链式查询入口，并添加带参数的原始 SQL 条件。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="where">原始 SQL 条件，如 "Age &gt; @0 AND Name = @1"</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>链式查询提供程序</returns>
        public IQueryProvider<T> Where<T>(string where, params object[] args) where T : class
        {
            if (string.IsNullOrEmpty(where)) throw new ArgumentNullException(nameof(where));
            return GetDatabase().Query<T>().WhereSql(where, args);
        }

        /// <summary>
        /// 链式查询入口，并添加表达式条件。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="where">表达式条件，如 q =&gt; q.Age &gt;= 30</param>
        /// <returns>链式查询提供程序</returns>
        public IQueryProvider<T> Where<T>(Expression<Func<T, bool>> where) where T : class
        {
            if (where == null) throw new ArgumentNullException(nameof(where));
            return GetDatabase().Query<T>().Where(where);
        }

        #endregion Where

        #region UpdateMany

        /// <summary>
        /// 批量更新入口（基于 NPOCO UpdateQueryProvider）。
        /// 用法：helper.UpdateMany&lt;User&gt;().Where(x =&gt; x.Age &gt; 30).ExcludeDefaults().Execute(new User { Vip = true });
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>批量更新提供程序</returns>
        public IUpdateQueryProvider<T> UpdateMany<T>() where T : class
        {
            return GetDatabase().UpdateMany<T>();
        }

        #endregion UpdateMany

        #region DeleteMany

        /// <summary>
        /// 批量删除入口（基于 NPOCO DeleteQueryProvider）。
        /// 用法：helper.DeleteMany&lt;User&gt;().Where(x =&gt; x.Age &gt; 30).Execute();
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>批量删除提供程序</returns>
        public IDeleteQueryProvider<T> DeleteMany<T>() where T : class
        {
            return GetDatabase().DeleteMany<T>();
        }

        #endregion DeleteMany
    }
}
