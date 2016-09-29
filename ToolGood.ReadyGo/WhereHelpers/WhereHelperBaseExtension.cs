using System;
using System.Collections.Generic;
using System.Data;
using ToolGood.ReadyGo.Caches;
using ToolGood.ReadyGo.SqlBuilding;
using ToolGood.ReadyGo.WhereHelpers;

namespace ToolGood.ReadyGo
{
    public interface IUseCache
    {
        /// <summary>
        /// 使用缓存
        /// </summary>
        /// <param name="second"></param>
        /// <param name="cacheTag"></param>
        /// <param name="cacheService"></param>
        void useCache(int second, string cacheTag = null, ICacheService cacheService = null);
    }

    public static partial class WhereHelperBaseExtension
    {
        #region 判断

        /// <summary>
        /// IfTrue 如果为假会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="ifTrue"></param>
        /// <returns></returns>
        public static T IfTrue<T>(this T t, bool ifTrue) where T : HelperBase
        {
            t.ifTrue(ifTrue);
            return t;
        }

        /// <summary>
        /// IfFalse 如果为真会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="ifTrue"></param>
        /// <returns></returns>
        public static T IfFalse<T>(this T t, bool ifTrue) where T : HelperBase
        {
            t.ifTrue(ifTrue == false);
            return t;
        }

        /// <summary>
        /// IfSet  如果字符串未设置，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfSet<T>(this T t, string txt) where T : HelperBase
        {
            t.ifTrue(string.IsNullOrEmpty(txt) == false);
            return t;
        }

        /// <summary>
        /// IfNotSet  如果字符串已设置，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotSet<T>(this T t, string txt) where T : HelperBase
        {
            t.ifTrue(string.IsNullOrEmpty(txt));
            return t;
        }

        /// <summary>
        /// IfNull  如果字符串不为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNull<T>(this T t, string txt) where T : HelperBase
        {
            t.ifTrue(txt == null);
            return t;
        }

        /// <summary>
        /// IfNotNull  如果字符串为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotNull<T>(this T t, string txt) where T : HelperBase
        {
            t.ifTrue(txt != null);
            return t;
        }

        /// <summary>
        /// IfNull  如果字符串不为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNull<T>(this T t, short? txt) where T : HelperBase
        {
            t.ifTrue(txt == null);
            return t;
        }

        /// <summary>
        /// IfNotNull  如果字符串为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotNull<T>(this T t, short? txt) where T : HelperBase
        {
            t.ifTrue(txt != null);
            return t;
        }

        /// <summary>
        /// IfNull  如果字符串不为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNull<T>(this T t, int? txt) where T : HelperBase
        {
            t.ifTrue(txt == null);
            return t;
        }

        /// <summary>
        /// IfNotNull  如果字符串为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotNull<T>(this T t, int? txt) where T : HelperBase
        {
            t.ifTrue(txt != null);
            return t;
        }

        /// <summary>
        /// IfNull  如果字符串不为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNull<T>(this T t, long? txt) where T : HelperBase
        {
            t.ifTrue(txt == null);
            return t;
        }

        /// <summary>
        /// IfNotNull  如果字符串为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotNull<T>(this T t, long? txt) where T : HelperBase
        {
            t.ifTrue(txt != null);
            return t;
        }

        /// <summary>
        /// IfNull  如果字符串不为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNull<T>(this T t, ushort? txt) where T : HelperBase
        {
            t.ifTrue(txt == null);
            return t;
        }

        /// <summary>
        /// IfNotNull  如果字符串为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotNull<T>(this T t, ushort? txt) where T : HelperBase
        {
            t.ifTrue(txt != null);
            return t;
        }

        /// <summary>
        /// IfNull  如果字符串不为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNull<T>(this T t, uint? txt) where T : HelperBase
        {
            t.ifTrue(txt == null);
            return t;
        }

        /// <summary>
        /// IfNotNull  如果字符串为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotNull<T>(this T t, uint? txt) where T : HelperBase
        {
            t.ifTrue(txt != null);
            return t;
        }

        /// <summary>
        /// IfNull  如果字符串不为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNull<T>(this T t, ulong? txt) where T : HelperBase
        {
            t.ifTrue(txt == null);
            return t;
        }

        /// <summary>
        /// IfNotNull  如果字符串为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotNull<T>(this T t, ulong? txt) where T : HelperBase
        {
            t.ifTrue(txt != null);
            return t;
        }

        /// <summary>
        /// IfNull  如果字符串不为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNull<T>(this T t, float? txt) where T : HelperBase
        {
            t.ifTrue(txt == null);
            return t;
        }

        /// <summary>
        /// IfNotNull  如果字符串为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotNull<T>(this T t, float? txt) where T : HelperBase
        {
            t.ifTrue(txt != null);
            return t;
        }

        /// <summary>
        /// IfNull  如果字符串不为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNull<T>(this T t, double? txt) where T : HelperBase
        {
            t.ifTrue(txt == null);
            return t;
        }

        /// <summary>
        /// IfNotNull  如果字符串为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotNull<T>(this T t, double? txt) where T : HelperBase
        {
            t.ifTrue(txt != null);
            return t;
        }

        /// <summary>
        /// IfNull  如果字符串不为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNull<T>(this T t, decimal? txt) where T : HelperBase
        {
            t.ifTrue(txt == null);
            return t;
        }

        /// <summary>
        /// IfNotNull  如果字符串为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotNull<T>(this T t, decimal? txt) where T : HelperBase
        {
            t.ifTrue(txt != null);
            return t;
        }

        /// <summary>
        /// IfNull  如果字符串不为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNull<T>(this T t, DateTime? txt) where T : HelperBase
        {
            t.ifTrue(txt == null);
            return t;
        }

        /// <summary>
        /// IfNotNull  如果字符串为空，会影响 Where、OrderBy、AddSelect GroupBy Having On方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static T IfNotNull<T>(this T t, DateTime? txt) where T : HelperBase
        {
            t.ifTrue(txt != null);
            return t;
        }

        #endregion 判断

        #region Sql 拼接
        /// <summary>
        /// 自动添加 “NOT EXISTS ” 也会自动添加 “SELECT * ”
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="where"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T AddNotExistsSql<T>(this T t, string where, params object[] args) where T : HelperBase
        {
            where = where.TrimStart();
            if (where.StartsWith("NOT EXISTS ", StringComparison.CurrentCultureIgnoreCase) == false) {
                if (where.StartsWith("SELECT ", StringComparison.CurrentCultureIgnoreCase) == false) {
                    where = string.Format("NOT EXISTS(SELECT * {0})", where);
                } else {
                    where = string.Format("NOT EXISTS({0})", where);
                }
            }
            t.where(where, args);
            return t;
        }
        /// <summary>
        /// 自动添加 “EXISTS ” 也会自动添加 “SELECT * ”
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="where"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T AddExistsSql<T>(this T t, string where, params object[] args) where T : HelperBase
        {
            where = where.TrimStart();
            if (where.StartsWith("EXISTS ", StringComparison.CurrentCultureIgnoreCase) == false) {
                if (where.StartsWith("SELECT ", StringComparison.CurrentCultureIgnoreCase) == false) {
                    where = string.Format("EXISTS(SELECT * {0})", where);
                } else {
                    where = string.Format("EXISTS({0})", where);
                }
            }
            t.where(where, args);
            return t;
        }
        /// <summary>
        /// 添加 Where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="where"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T AddWhereSql<T>(this T t, string where, params object[] args) where T : HelperBase
        {
            t.where(where, args);
            return t;
        }
        /// <summary>
        /// 添加 Order By SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static T AddOrderBySql<T>(this T t, string order) where T : WhereHelperBase
        {
            t.orderBySql(order);
            return t;
        }
        /// <summary>
        /// 添加 Group By SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="groupby"></param>
        /// <returns></returns>
        public static T AddGroupBySql<T>(this T t, string groupby) where T : WhereHelperBase
        {
            t.groupBy(groupby);
            return t;
        }
        /// <summary>
        /// 添加 Having SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="having"></param>
        /// <returns></returns>
        public static T AddHavingSql<T>(this T t, string having) where T : WhereHelperBase
        {
            t.having(having);
            return t;
        }
        /// <summary>
        /// 添加 Join On SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="having"></param>
        /// <returns></returns>
        public static T AddJoinSql<T>(this T t, string joinWithOn) where T : WhereHelperBase
        {
            t.join(joinWithOn);
            return t;
        }

        #endregion Sql 拼接

        #region UseCache 设置缓存
        /// <summary>
        /// 使用缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="second">秒</param>
        /// <returns></returns>
        public static T UseCache<T>(this T t, int second) where T : IUseCache
        {
            t.useCache(second);
            return t;
        }
        /// <summary>
        /// 使用缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="second">秒</param>
        /// <param name="cacheTag">缓存标志</param>
        /// <returns></returns>
        public static T UseCache<T>(this T t, int second, string cacheTag) where T : IUseCache
        {
            t.useCache(second, cacheTag);
            return t;
        }

        //public static T UseCache<T>(this T t, int second, string cacheTag, ICacheService cacheService) where T : IUseCache
        //{
        //    t.useCache(second, cacheTag, cacheService);
        //    return t;
        //}

        #endregion UseCache 设置缓存
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="t"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static int Count(this WhereHelperBase t, string selectSql = null)
        {
            t.setCache();
            return t._sqlhelper.getDatabase(ConnectionType.Read).Execute(t.GetCountSql(selectSql), t._args.ToArray());
        }
        /// <summary>
        /// 执行返回DataTable
        /// </summary>
        /// <param name="t"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(this WhereHelperBase t, string selectSql = null)
        {
            t.setCache();
            return t._sqlhelper.ExecuteDataTable(t.GetFullSelectSql(selectSql), t._args.ToArray());
        }
        /// <summary>
        /// 执行返回DataSet
        /// </summary>
        /// <param name="t"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(this WhereHelperBase t, string selectSql = null)
        {
            t.setCache();
            return t._sqlhelper.ExecuteDataSet(t.GetFullSelectSql(selectSql), t._args.ToArray());
        }
        /// <summary>
        /// 执行返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static List<T> Select<T>(this WhereHelperBase t, string selectSql = null)
        {
            t.setCache();
            var sql = getSelect<T>(t, selectSql);
            return t._sqlhelper.Select<T>(t.GetFullSelectSql(sql), t._args.ToArray());
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static T Single<T>(this WhereHelperBase t, string selectSql = null)
        {
            t.setCache();
            var sql = getSelect<T>(t, selectSql);
            return t._sqlhelper.Single<T>(t.GetFullSelectSql(sql), t._args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static T SingleOrDefault<T>(this WhereHelperBase t, string selectSql = null)
        {
            t.setCache();
            var sql = getSelect<T>(t, selectSql);
            return t._sqlhelper.SingleOrDefault<T>(t.GetFullSelectSql(sql), t._args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static T First<T>(this WhereHelperBase t, string selectSql = null)
        {
            t.setCache();
            var sql = getSelect<T>(t, selectSql);
            return t._sqlhelper.First<T>(t.GetFullSelectSql(sql), t._args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static T FirstOrDefault<T>(this WhereHelperBase t, string selectSql = null)
        {
            t.setCache();
            var sql = getSelect<T>(t, selectSql);
            return t._sqlhelper.FirstOrDefault<T>(t.GetFullSelectSql(sql), t._args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static Page<T> Page<T>(this WhereHelperBase t, long page, long itemsPerPage, string selectSql = null)
        {
            t.setCache();
            var sql = getSelect<T>(t, selectSql);
            return t._sqlhelper.Page<T>(page, itemsPerPage, t.GetFullSelectSql(sql), t._args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static List<T> SkipTake<T>(this WhereHelperBase t, long skip, long take, string selectSql = null)
        {
            t.setCache();
            var sql = getSelect<T>(t, selectSql);
            return t._sqlhelper.SkipTake<T>(skip, take, t.GetFullSelectSql(sql), t._args.ToArray());
        }

        private static string getSelect<T>(WhereHelperBase t, string selectSql)
        {
            if (string.IsNullOrEmpty(selectSql) == false) {
                if (selectSql.StartsWith("Select", StringComparison.CurrentCultureIgnoreCase)) {
                    return selectSql;
                }
                return "SELECT " + selectSql;
            }
            return SelectHelper.CreateSelectHeader(typeof(T), t._headers, t.GetTypes());
        }
    }
}