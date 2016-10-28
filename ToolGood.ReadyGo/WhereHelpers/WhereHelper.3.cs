using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.Providers;
using ToolGood.ReadyGo.SqlBuilding;

namespace ToolGood.ReadyGo.WhereHelpers
{
    public partial class WhereHelper<T1, T2, T3> : WhereHelperBase
        where T1 : class, new()
        where T2 : class, new()
        where T3 : class, new()
    {
        #region 私有变量

        internal JoinType join2;
        internal JoinType join3;
        private string on2;
        private string on3;

        #endregion 私有变量

        internal WhereHelper(SqlHelper helper)
        {
            this._sqlhelper = helper;
            SqlExpression = SqlExpression.Resolve(_sqlhelper._sqlType);
        }

        #region Where OrderBy GroupBy Having

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3> WhereNotIn<T>(Expression<Func<T1, T2, T3, T>> field, ICollection args)
        {
            base.whereNotIn(field, args);
            return this;
        }

        public WhereHelper<T1, T2, T3> WhereNotIn<T>(Expression<Func<T1, T2, T3, T>> field, params T[] args)
        {
            return WhereNotIn(field, (ICollection)args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3> WhereIn<T>(Expression<Func<T1, T2, T3, T>> field, ICollection args)
        {
            base.whereIn(field, args);
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3> WhereIn<T>(Expression<Func<T1, T2, T3, T>> field, params T[] args)
        {
            return WhereIn(field, (ICollection)args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool>> where)
        {
            base.where(where);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="order"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3> OrderBy<T>(Expression<Func<T1, T2, T3, T>> order, OrderType type = OrderType.Asc)
        {
            base.orderBy(order, type);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3> GroupBy<T>(Expression<Func<T1, T2, T3, T>> group)
        {
            groupBy(group);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="having"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3> Having(Expression<Func<T1, T2, T3, bool>> having)
        {
            base.having(having);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="asName"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3> SelectColumn<T>(Expression<Func<T1, T2, T3, T>> column, string asName = null)
        {
            selectColumn(column, asName);
            return this;
        }

        #endregion Where OrderBy GroupBy Having

        #region Select Page SkipTake Single SingleOrDefault First FirstOrDefault
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public List<TableRow<T1, T2, T3>> Select(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3> row = new TableRow<T1, T2, T3>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return db.Query(row, sql, args).ToList();
            }, "Select");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Page<TableRow<T1, T2, T3>> Page(long page, long itemsPerPage, string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return db.Page<T1, T2, T3>(page, itemsPerPage, sql, args);
            }, "Page");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public List<TableRow<T1, T2, T3>> SkipTake(long skip, long take, string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3> row = new TableRow<T1, T2, T3>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return db.SkipTake<T1, T2, T3>(skip, take, sql, args);
            }, "SkipTake");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public TableRow<T1, T2, T3> Single(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3> row = new TableRow<T1, T2, T3>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).Single();
            }, "Single");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public TableRow<T1, T2, T3> SingleOrDefault(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3> row = new TableRow<T1, T2, T3>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).SingleOrDefault();
            }, "SingleOrDefault");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public TableRow<T1, T2, T3> First(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3> row = new TableRow<T1, T2, T3>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).First();
            }, "First");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public TableRow<T1, T2, T3> FirstOrDefault(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3> row = new TableRow<T1, T2, T3>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).FirstOrDefault();
            }, "FirstOrDefault");
        }

        #endregion Select Page SkipTake Single SingleOrDefault First FirstOrDefault

        #region On
        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3> On2(Expression<Func<T1, T2, bool>> where)
        {
            string sql;
            SqlExpression.Analysis(where, out sql);
            on2 = sql;
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3> On3(Expression<Func<T1, T2, T3, bool>> where)
        {
            string sql;
            SqlExpression.Analysis(where, out sql);
            on3 = sql;
            return this;
        }

        #endregion On

        #region SetValue

        /// <summary>
        /// 只支持简单的赋值
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public UpdateHelper<T1> SetValue(Action<T1> action)
        {
            UpdateHelper<T1> where = new UpdateHelper<T1>(_sqlhelper);
            where._where = this._where;
            where._joinOnString = this.GetFromAndJoinOn();
            where._args.AddRange(this._args);
            where._doNext = this._doNext;
            where.SetValue(action);
            where._headers = this._headers;
            //where.typeCount = 3;

            this._doNext = true;
            return where;
        }

        #endregion SetValue

        protected internal override List<Type> GetTypes()
        {
            return new List<Type>() { typeof(T1), typeof(T2), typeof(T3) };
        }

        protected internal override string GetFromAndJoinOn()
        {
            var dp = DatabaseProvider.Resolve(_sqlhelper._sqlType);
            var pd1 = PocoData.ForType(typeof(T1));
            var pd2 = PocoData.ForType(typeof(T2));
            var pd3 = PocoData.ForType(typeof(T3));

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("FROM {0} AS t1 ", dp.GetTableName(pd1, _sqlhelper._tableNameManger));
            if (string.IsNullOrEmpty(on2)) {
                sb.AppendFormat(",{0} AS t2", dp.GetTableName(pd2, _sqlhelper._tableNameManger));
            }
            if (string.IsNullOrEmpty(on3)) {
                sb.AppendFormat(",{0} AS t3", dp.GetTableName(pd3, _sqlhelper._tableNameManger));
            }
            if (string.IsNullOrEmpty(on2) == false) {
                sb.AppendFormat(" {1} JOIN {0} AS t2 ON {2}", dp.GetTableName(pd2, _sqlhelper._tableNameManger), join2.ToString(), on2);
            }
            if (string.IsNullOrEmpty(on3) == false) {
                sb.AppendFormat(" {1} JOIN {0} AS t3 ON {2}", dp.GetTableName(pd3, _sqlhelper._tableNameManger), join3.ToString(), on3);
            }
            sb.Append(" ");
            sb.Append(_joinOnString);
            return sb.ToString();
        }
    }
}