using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.SqlBuilding;

namespace ToolGood.ReadyGo
{
    partial class SqlHelper
    {
        private string buildMulSql(string sql, Type outType, List<Type> types)
        {
            sql = sql.Trim();
            var ss = SqlSearch.Search(sql);

            if (ss.HasSelect || ss.HasFrom) {
                for (int i = 0; i < types.Count; i++) {
                    sql = sql.Replace("{t" + (i + 1).ToString() + "}", GetTableName(types[i]));
                }
            }
            if (ss.HasSelect) return sql;

            var select = SelectHelper.CreateSelectHeader(outType, null, types);

            string from = " ";
            if (ss.HasFrom==false) {
                if (types.Count==5) {
                    from = string.Format(" FROM {0} AS t1, {1} AS t2, {2} AS t3, {3} AS t4, {4} AS t5 ",
                         GetTableName(types[0]), GetTableName(types[1]), GetTableName(types[2]), GetTableName(types[3]), GetTableName(types[4])
                        );
                } else if (types.Count == 4) {
                    from = string.Format(" FROM {0} AS t1, {1} AS t2, {2} AS t3, {3} AS t4 ",
                       GetTableName(types[0]), GetTableName(types[1]), GetTableName(types[2]), GetTableName(types[3])
                      );
                } else if (types.Count == 3) {
                    from = string.Format(" FROM {0} AS t1, {1} AS t2, {2} AS t3 ",
                      GetTableName(types[0]), GetTableName(types[1]), GetTableName(types[2])
                     );
                } else if (types.Count == 2) {
                    from = string.Format(" FROM {0} AS t1, {1} AS t2 ",
                      GetTableName(types[0]), GetTableName(types[1])
                     );
                }
            }
            return select + from + sql;
        }
        private string buildOneSql(string sql, Type outType, Type type)
        {
            sql = sql.Trim();
            sql = sql.Replace("{t1}", GetTableName(type));

            var ss = SqlSearch.Search(sql);
            if (ss.HasSelect) return sql;

            var select = SelectHelper.CreateSelectHeader(outType, null, new List<Type>() { type });

            if (ss.HasFrom== false) {
                if (ss.HasT1) {
                    return select + " From " + GetTableName(type) + " as t1 " + sql;
                }
                return select.Replace("t1.", "") + " From " + GetTableName(type) + " " + sql;
            }

            if (ss.HasT1) return select + sql;
            return select.Replace("t1.", "") + " " + sql;
        }


        #region Select
        /// <summary>
        /// Select 语句，<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4,{t5} as t5
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<OutT> Select<T1, T2, T3, T4, T5, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
            return Select<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句，<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<OutT> Select<T1, T2, T3, T4, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
            return Select<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句，<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<OutT> Select<T1, T2, T3, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3) });
            return Select<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句，<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<OutT> Select<T1, T2, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2) });
            return Select<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句，<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// 可以省略From部分,系统自动生成From部分<para></para>
        /// From部分 例如： From {t1} as t1
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<OutT> Select<T1, OutT>(string sql = "", params object[] args)
        {
            sql = buildOneSql(sql, typeof(OutT), typeof(T1));
            return Select<OutT>(sql, args);
        }
        #endregion

        #region Single SingleOrDefault
        /// <summary>
        /// Select 语句 获取唯一的对象<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4,{t5} as t5
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT Single<T1, T2, T3, T4, T5, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
            return Single<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取唯一的对象<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT Single<T1, T2, T3, T4, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
            return Single<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取唯一的对象<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3<para></para>
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT Single<T1, T2, T3, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3) });
            return Single<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取唯一的对象<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT Single<T1, T2, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2) });
            return Single<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取唯一的对象<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// 可以省略From部分,系统自动生成From部分<para></para>
        /// From部分 例如： From {t1} as t1
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT Single<T1, OutT>(string sql = "", params object[] args)
        {
            sql = buildOneSql(sql, typeof(OutT), typeof(T1));
            return Single<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取唯一的对象,默认为null<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4,{t5} as t5
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT SingleOrDefault<T1, T2, T3, T4, T5, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
            return SingleOrDefault<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取唯一的对象,默认为null<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT SingleOrDefault<T1, T2, T3, T4, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
            return SingleOrDefault<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取唯一的对象,默认为null<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT SingleOrDefault<T1, T2, T3, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3) });
            return SingleOrDefault<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取唯一的对象,默认为null<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT SingleOrDefault<T1, T2, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2) });
            return SingleOrDefault<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取唯一的对象,默认为null<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// 可以省略From部分,系统自动生成From部分<para></para>
        /// From部分 例如： From {t1} as t1
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT SingleOrDefault<T1, OutT>(string sql = "", params object[] args)
        {
            sql = buildOneSql(sql, typeof(OutT), typeof(T1));
            return SingleOrDefault<OutT>(sql, args);
        }
        #endregion

        #region First FirstOrDefault
        /// <summary>
        /// Select 语句 获取第一个对象<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4,{t5} as t5
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT First<T1, T2, T3, T4, T5, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
            return First<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取第一个对象<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT First<T1, T2, T3, T4, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
            return First<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取第一个对象<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT First<T1, T2, T3, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3) });
            return Single<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取第一个对象<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT First<T1, T2, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2) });
            return First<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取第一个对象<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// 可以省略From部分,系统自动生成From部分<para></para>
        /// From部分 例如： From {t1} as t1
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT First<T1, OutT>(string sql = "", params object[] args)
        {
            sql = buildOneSql(sql, typeof(OutT), typeof(T1));
            return First<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取第一个对象，默认为Null<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4,{t5} as t5
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT FirstOrDefault<T1, T2, T3, T4, T5, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
            return FirstOrDefault<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取第一个对象，默认为Null<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT FirstOrDefault<T1, T2, T3, T4, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
            return FirstOrDefault<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取第一个对象，默认为Null<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT FirstOrDefault<T1, T2, T3, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3) });
            return FirstOrDefault<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取第一个对象，默认为Null<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT FirstOrDefault<T1, T2, OutT>(string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2) });
            return FirstOrDefault<OutT>(sql, args);
        }
        /// <summary>
        /// Select 语句 获取第一个对象，默认为Null<para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// 可以省略From部分,系统自动生成From部分<para></para>
        /// From部分 例如： From {t1} as t1
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OutT FirstOrDefault<T1, OutT>(string sql = "", params object[] args)
        {
            sql = buildOneSql(sql, typeof(OutT), typeof(T1));
            return FirstOrDefault<OutT>(sql, args);
        }
        #endregion

        #region Page
        /// <summary>
        /// Select 语句，返回Page&lt;T&gt;类型 <para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4,{t5} as t5
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Page<OutT> Page<T1, T2, T3, T4, T5, OutT>(long page, long itemsPerPage, string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
            return Page<OutT>(page, itemsPerPage, sql, args);
        }
        /// <summary>
        /// Select 语句，返回Page&lt;T&gt;类型 <para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Page<OutT> Page<T1, T2, T3, T4, OutT>(long page, long itemsPerPage, string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
            return Page<OutT>(page, itemsPerPage, sql, args);
        }
        /// <summary>
        /// Select 语句，返回Page&lt;T&gt;类型 <para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Page<OutT> Page<T1, T2, T3, OutT>(long page, long itemsPerPage, string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3) });
            return Page<OutT>(page, itemsPerPage, sql, args);
        }
        /// <summary>
        /// Select 语句，返回Page&lt;T&gt;类型 <para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Page<OutT> Page<T1, T2, OutT>(long page, long itemsPerPage, string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2) });
            return Page<OutT>(page, itemsPerPage, sql, args);
        }
        /// <summary>
        /// Select 语句，返回Page&lt;T&gt;类型 <para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// 可以省略From部分,系统自动生成From部分<para></para>
        /// From部分 例如： From {t1} as t1
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Page<OutT> Page<T1, OutT>(long page, long itemsPerPage, string sql = "", params object[] args)
        {
            sql = buildOneSql(sql, typeof(OutT), typeof(T1));
            return Page<OutT>(page, itemsPerPage, sql, args);
        }
        #endregion

        #region SkipTake
        /// <summary>
        /// Select 语句 <para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4,{t5} as t5
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<OutT> SkipTake<T1, T2, T3, T4, T5, OutT>(long skip, long take, string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
            return SkipTake<OutT>(skip, take, sql, args);
        }
        /// <summary>
        /// Select 语句 <para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3,{t4} as t4
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<OutT> SkipTake<T1, T2, T3, T4, OutT>(long skip, long take, string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
            return SkipTake<OutT>(skip, take, sql, args);
        }
        /// <summary>
        /// Select 语句 <para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2,{t3} as t3
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<OutT> SkipTake<T1, T2, T3, OutT>(long skip, long take, string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2), typeof(T3) });
            return SkipTake<OutT>(skip, take, sql, args);
        }
        /// <summary>
        /// Select 语句 <para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// From部分 例如： From {t1} as t1,{t2} as t2
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<OutT> SkipTake<T1, T2, OutT>(long skip, long take, string sql = "", params object[] args)
        {
            sql = buildMulSql(sql, typeof(OutT), new List<Type>() { typeof(T1), typeof(T2) });
            return SkipTake<OutT>(skip, take, sql, args);
        }
        /// <summary>
        /// Select 语句 <para></para>
        /// 可以省略Select部分，系统自动生成Select部分<para></para>
        /// 可以省略From部分,系统自动生成From部分<para></para>
        /// From部分 例如： From {t1} as t1
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<OutT> SkipTake<T1, OutT>(long skip, long take, string sql = "", params object[] args)
        {
            sql = buildOneSql(sql, typeof(OutT), typeof(T1));
            return SkipTake<OutT>(skip, take, sql, args);
        }
        #endregion
    }
}
