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
    public partial class WhereHelper<T1, T2, T3, T4> : WhereHelperBase, IDeepCloneable<WhereHelper<T1, T2, T3, T4>>
        where T1 : class, new()
        where T2 : class, new()
        where T3 : class, new()
        where T4 : class, new()
    {
        #region 私有变量

        internal JoinType join2;
        internal JoinType join3;
        internal JoinType join4;
        private string on2;
        private string on3;
        private string on4;

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
        /// 添加 Not In 语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> WhereNotIn<T>(Expression<Func<T1, T2, T3, T4, T>> field, ICollection args)
        {
            base.whereNotIn(field, args);
            return this;
        }
        /// <summary>
        /// 添加 Not In 语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> WhereNotIn<T>(Expression<Func<T1, T2, T3, T4, T>> field, params T[] args)
        {
            return WhereNotIn(field, (ICollection)args);
        }
        /// <summary>
        /// 添加 In 语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> WhereIn<T>(Expression<Func<T1, T2, T3, T4, T>> field, ICollection args)
        {
            base.whereIn(field, args);
            return this;
        }
        /// <summary>
        /// 添加 In 语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> WhereIn<T>(Expression<Func<T1, T2, T3, T4, T>> field, params T[] args)
        {
            return WhereIn(field, (ICollection)args);
        }
        /// <summary>
        /// 添加 Where 语句
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, bool>> where)
        {
            base.where(where);
            return this;
        }
        /// <summary>
        /// 添加 Order By 语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="order"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> OrderBy<T>(Expression<Func<T1, T2, T3, T4, T>> order, OrderType type = OrderType.Asc)
        {
            base.orderBy(order, type);
            return this;
        }
        /// <summary>
        /// 添加 Group By 语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> GroupBy<T>(Expression<Func<T1, T2, T3, T4, T>> group)
        {
            groupBy(group);
            return this;
        }
        /// <summary>
        /// 添加 Having 语句
        /// </summary>
        /// <param name="having"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> Having(Expression<Func<T1, T2, T3, bool>> having)
        {
            base.having(having);
            return this;
        }
        /// <summary>
        /// 添加 Select Column 语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="asName"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> SelectColumn<T>(Expression<Func<T1, T2, T3, T4, T>> column, string asName = null)
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
        public List<TableRow<T1, T2, T3, T4>> Select(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3, T4> row = new TableRow<T1, T2, T3, T4>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).ToList();
            }, "Select");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Page<TableRow<T1, T2, T3, T4>> Page(long page, long itemsPerPage, string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return db.Page<T1, T2, T3, T4>(page, itemsPerPage, sql, args);
            }, "Page");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public List<TableRow<T1, T2, T3, T4>> SkipTake(long skip, long take, string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3, T4> row = new TableRow<T1, T2, T3, T4>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return db.SkipTake<T1, T2, T3, T4>(skip, take, sql, args);
            }, "SkipTake");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public TableRow<T1, T2, T3, T4> Single(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3, T4> row = new TableRow<T1, T2, T3, T4>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).Single();
            }, "Single");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public TableRow<T1, T2, T3, T4> SingleOrDefault(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3, T4> row = new TableRow<T1, T2, T3, T4>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).SingleOrDefault();
            }, "SingleOrDefault");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public TableRow<T1, T2, T3, T4> First(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3, T4> row = new TableRow<T1, T2, T3, T4>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).First();
            }, "First");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public TableRow<T1, T2, T3, T4> FirstOrDefault(string selectSql = null)
        {
            setCache();
            var sql = GetFullSelectSql(selectSql);
            var args = _args.ToArray();
            return _sqlhelper.Run(sql, args, () => {
                TableRow<T1, T2, T3, T4> row = new TableRow<T1, T2, T3, T4>();
                var db = _sqlhelper.getDatabase(ConnectionType.Read);
                return (db.Query(row, sql, args)).FirstOrDefault();
            }, "FirstOrDefault");
        }

        #endregion Select Page SkipTake Single SingleOrDefault First FirstOrDefault

        #region Select Page SkipTake Single SingleOrDefault First FirstOrDefault
        public List<T> Select<T>(Expression<Func<T1, T2, T3, T4, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.Select<T>(GetFullSelectSql(sql), _args);
        }
        public T Single<T>(Expression<Func<T1, T2, T3, T4, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.Single<T>(GetFullSelectSql(sql), _args);
        }
        public T SingleOrDefault<T>(Expression<Func<T1, T2, T3, T4, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.SingleOrDefault<T>(GetFullSelectSql(sql), _args);
        }
        public T First<T>(Expression<Func<T1, T2, T3, T4, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.First<T>(GetFullSelectSql(sql), _args);
        }
        public T FirstOrDefault<T>(Expression<Func<T1, T2, T3, T4, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.FirstOrDefault<T>(GetFullSelectSql(sql), _args);
        }
        public List<T> SkipTake<T>(long skip, long take, Expression<Func<T1, T2, T3, T4, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.SkipTake<T>(skip, take, GetFullSelectSql(sql), _args);
        }
        public Page<T> Page<T>(long page, long itemsPerPage, Expression<Func<T1, T2, T3, T4, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.Page<T>(page, itemsPerPage, GetFullSelectSql(sql), _args);
        }

        #endregion

        #region On
        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> On2(Expression<Func<T1, T2, bool>> where)
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
        public WhereHelper<T1, T2, T3, T4> On3(Expression<Func<T1, T2, T3, bool>> where)
        {
            string sql;
            SqlExpression.Analysis(where, out sql);
            on3 = sql;
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> On4(Expression<Func<T1, T2, T3, T4, bool>> where)
        {
            string sql;
            SqlExpression.Analysis(where, out sql);
            on4 = sql;
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
            //where.typeCount = 4;

            this._doNext = true;
            return where;
        }

        #endregion SetValue

        protected internal override List<Type> GetTypes()
        {
            return new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };

            throw new NotImplementedException();
        }

        protected internal override string GetFromAndJoinOn()
        {
            var dp = DatabaseProvider.Resolve(_sqlhelper._sqlType);
            var pd1 = PocoData.ForType(typeof(T1));
            var pd2 = PocoData.ForType(typeof(T2));
            var pd3 = PocoData.ForType(typeof(T3));
            var pd4 = PocoData.ForType(typeof(T4));

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("FROM {0} AS t1 ", dp.GetTableName(pd1, _sqlhelper._tableNameManger));
            if (string.IsNullOrEmpty(on2)) {
                sb.AppendFormat(",{0} AS t2", dp.GetTableName(pd2, _sqlhelper._tableNameManger));
            }
            if (string.IsNullOrEmpty(on3)) {
                sb.AppendFormat(",{0} AS t3", dp.GetTableName(pd3, _sqlhelper._tableNameManger));
            }
            if (string.IsNullOrEmpty(on4)) {
                sb.AppendFormat(",{0} AS t4", dp.GetTableName(pd4, _sqlhelper._tableNameManger));
            }
            if (string.IsNullOrEmpty(on2) == false) {
                sb.AppendFormat(" {1} JOIN {0} AS t2 ON {2}", dp.GetTableName(pd2, _sqlhelper._tableNameManger), join2.ToString(), on2);
            }
            if (string.IsNullOrEmpty(on3) == false) {
                sb.AppendFormat(" {1} JOIN {0} AS t3 ON {2}", dp.GetTableName(pd3, _sqlhelper._tableNameManger), join3.ToString(), on3);
            }
            if (string.IsNullOrEmpty(on4) == false) {
                sb.AppendFormat(" {1} JOIN {0} AS t4 ON {2}", dp.GetTableName(pd4, _sqlhelper._tableNameManger), join4.ToString(), on4);
            }
            sb.Append(" ");
            sb.Append(_joinOnString);
            return sb.ToString();
        }
        /// <summary>
        /// 深度复制
        /// </summary>
        /// <returns></returns>
        public WhereHelper<T1, T2, T3, T4> DeepClone()
        {
            WhereHelper<T1, T2, T3, T4> newWhere = new WhereHelper<T1, T2, T3, T4>(this._sqlhelper);
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