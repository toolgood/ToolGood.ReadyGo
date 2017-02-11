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
    public partial class WhereHelper<T1, T2> : WhereHelperBase, IDeepCloneable<WhereHelper<T1, T2>>
        where T1 : class, new()
        where T2 : class, new()
    {
        #region 私有变量

        internal JoinType join2;
        private string on2;

        #endregion 私有变量

        internal WhereHelper(SqlHelper helper)
        {
            this._sqlhelper = helper;
            this._provider = DatabaseProvider.Resolve(_sqlhelper._sqlType);
            this._paramPrefix = _provider.GetParameterPrefix(_sqlhelper._writeConnectionString);
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
        public WhereHelper<T1, T2> WhereNotIn<T>(Expression<Func<T1, T2, T>> field, ICollection args)
        {
            base.whereNotIn(field, args);
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2> WhereNotIn<T>(Expression<Func<T1, T2, T>> field, params T[] args)
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
        public WhereHelper<T1, T2> WhereIn<T>(Expression<Func<T1, T2, T>> field, ICollection args)
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
        public WhereHelper<T1, T2> WhereIn<T>(Expression<Func<T1, T2, T>> field, params T[] args)
        {
            return WhereIn(field, (ICollection)args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2> Where(Expression<Func<T1, T2, bool>> where)
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
        public WhereHelper<T1, T2> OrderBy<T>(Expression<Func<T1, T2, T>> order, OrderType type = OrderType.Asc)
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
        public WhereHelper<T1, T2> GroupBy<T>(Expression<Func<T1, T2, T>> group)
        {
            groupBy(group);
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="having"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2> Having(Expression<Func<T1, T2, bool>> having)
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
        public WhereHelper<T1, T2> SelectColumn<T>(Expression<Func<T1, T2, T>> column, string asName = null)
        {
            selectColumn(column, asName);
            return this;
        }

        #endregion Where OrderBy GroupBy Having

        #region On
        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2> On2(Expression<Func<T1, T2, bool>> where)
        {
            string sql;
            SqlExpression.Analysis(where, out sql);
            on2 = sql;
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
            //where.typeCount = 2;
            this._doNext = true;
            return where;
        }

        #endregion SetValue

        #region Select Page SkipTake Single SingleOrDefault First FirstOrDefault

        public List<TableRow<T1, T2>> Select(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2> row = new TableRow<T1, T2>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).ToList();
            }, "Select");
        }

        public Page<TableRow<T1, T2>> Page(long page, long itemsPerPage, string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return db.Page<T1, T2>(page, itemsPerPage, sql, args);
            }, "Page");
        }

        public List<TableRow<T1, T2>> SkipTake(long skip, long take, string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2> row = new TableRow<T1, T2>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return db.SkipTake<T1, T2>(skip, take, sql, args);
            }, "SkipTake");
        }

        public TableRow<T1, T2> Single(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2> row = new TableRow<T1, T2>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).Single();
            }, "Single");
        }

        public TableRow<T1, T2> SingleOrDefault(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2> row = new TableRow<T1, T2>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).SingleOrDefault();
            }, "SingleOrDefault");
        }

        public TableRow<T1, T2> First(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2> row = new TableRow<T1, T2>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).First();
            }, "First");
        }

        public TableRow<T1, T2> FirstOrDefault(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2> row = new TableRow<T1, T2>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).FirstOrDefault();
            }, "FirstOrDefault");
        }

        #endregion Select Page SkipTake Single SingleOrDefault First FirstOrDefault

        #region Select Page SkipTake Single SingleOrDefault First FirstOrDefault
        public List<T> Select<T>(Expression<Func<T1, T2, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.Select<T>(GetFullSelectSql(sql), _args);
        }
        public T Single<T>(Expression<Func<T1, T2, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.Single<T>(GetFullSelectSql(sql), _args);
        }
        public T SingleOrDefault<T>(Expression<Func<T1, T2, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.SingleOrDefault<T>(GetFullSelectSql(sql), _args);
        }
        public T First<T>(Expression<Func<T1, T2, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.First<T>(GetFullSelectSql(sql), _args);
        }
        public T FirstOrDefault<T>(Expression<Func<T1, T2, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.FirstOrDefault<T>(GetFullSelectSql(sql), _args);
        }
        public List<T> SkipTake<T>(long skip, long take, Expression<Func<T1, T2, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.SkipTake<T>(skip, take, GetFullSelectSql(sql), _args);
        }
        public Page<T> Page<T>(long page, long itemsPerPage, Expression<Func<T1, T2, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.Page<T>(page, itemsPerPage, GetFullSelectSql(sql), _args);
        }

        #endregion


        protected internal override List<Type> GetTypes()
        {
            return new List<Type>() { typeof(T1), typeof(T2) };
        }

        protected internal override string GetFromAndJoinOn()
        {
            var dp = DatabaseProvider.Resolve(_sqlhelper._sqlType);
            var pd1 = PocoData.ForType(typeof(T1));
            var pd2 = PocoData.ForType(typeof(T2));

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("FROM {0} AS t1 ", dp.GetTableName(pd1, _sqlhelper._tableNameManger));
            if (string.IsNullOrEmpty(on2)) {
                sb.AppendFormat(",{0} AS t2", dp.GetTableName(pd2, _sqlhelper._tableNameManger));
            } else {
                sb.AppendFormat(" {1} JOIN {0} AS t2 ON {2}", dp.GetTableName(pd2, _sqlhelper._tableNameManger), join2.ToString(), on2);
            }
            sb.Append(" ");
            sb.Append(_joinOnString);
            return sb.ToString();
        }

        /// <summary>
        /// 深度复制
        /// </summary>
        /// <returns></returns>
        public WhereHelper<T1, T2> DeepClone()
        {
            WhereHelper<T1, T2> newWhere = new WhereHelper<T1, T2>(this._sqlhelper);
            newWhere._args.AddRange(this._args);
            newWhere._doNext = this._doNext;
            newWhere._groupby = this._groupby;
            newWhere._having = this._having;
            newWhere._headers.AddRange(this._headers);
            newWhere._joinOnString = this._joinOnString;
            newWhere._order = this._order;
            newWhere._where = this._where;

            return newWhere;
        }
    }
}