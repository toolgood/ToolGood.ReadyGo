using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.LinQ
{
    public partial class WhereHelper<T1> : WhereHelperBase, IDeepCloneable<WhereHelper<T1>>
        where T1 : class, new()
    {
        internal WhereHelper(SqlHelper helper)
        {
            this._sqlhelper = helper;
            this._provider = DatabaseProvider.Resolve(_sqlhelper._sqlType);
            this._paramPrefix = _provider.GetParameterPrefix(_sqlhelper._connectionString);
            SqlExpression = ToolGood.ReadyGo3.LinQ.Expressions.SqlExpression.Resolve(_sqlhelper._sqlType);
        }

        #region WhereIn Where OrderBy Having
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public WhereHelper<T1> WhereNotIn<T>(Expression<Func<T1, T>> field, ICollection args)
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
        public WhereHelper<T1> WhereNotIn<T>(Expression<Func<T1, T>> field, params T[] args)
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
        public WhereHelper<T1> WhereIn<T>(Expression<Func<T1, T>> field, ICollection args)
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
        public WhereHelper<T1> WhereIn<T>(Expression<Func<T1, T>> field, params T[] args)
        {
            return WhereIn(field, (ICollection)args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereHelper<T1> Where(Expression<Func<T1, bool>> where)
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
        public WhereHelper<T1> OrderBy<T>(Expression<Func<T1, T>> order, OrderType type = OrderType.Asc)
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
        public WhereHelper<T1> GroupBy<T>(Expression<Func<T1, T>> group)
        {
            groupBy(group);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="having"></param>
        /// <returns></returns>
        public WhereHelper<T1> Having(Expression<Func<T1, bool>> having)
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
        public WhereHelper<T1> SelectColumn<T>(Expression<Func<T1, T>> column, string asName = null)
        {
            selectColumn(column, asName);
            return this;
        }



        #endregion WhereIn Where OrderBy Having

        #region Select Page SkipTake Single SingleOrDefault First FirstOrDefault
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public List<T1> Select(string selectSql = null)
        {
            return _sqlhelper.Select<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Page<T1> Page(long page, long itemsPerPage, string selectSql = null)
        {
            return _sqlhelper.Page<T1>(page, itemsPerPage, GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public List<T1> Select(long top, string selectSql = null)
        {
            return _sqlhelper.Select<T1>(top, 0, GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public List<T1> Select(long skip, long take, string selectSql = null)
        {
            return _sqlhelper.Select<T1>(skip, take, GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public T1 Single(string selectSql = null)
        {
            return _sqlhelper.Single<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public T1 SingleOrDefault(string selectSql = null)
        {
            return _sqlhelper.SingleOrDefault<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public T1 First(string selectSql = null)
        {
            return _sqlhelper.First<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public T1 FirstOrDefault(string selectSql = null)
        {
            return _sqlhelper.FirstOrDefault<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }

        #endregion Select Page SkipTake Single SingleOrDefault First FirstOrDefault

        #region Select Page SkipTake Single SingleOrDefault First FirstOrDefault
        public List<T> Select<T>(Expression<Func<T1, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.Select<T>(GetFullSelectSql(sql), _args);
        }
        public T Single<T>(Expression<Func<T1, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.Single<T>(GetFullSelectSql(sql), _args);
        }
        public T SingleOrDefault<T>(Expression<Func<T1, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.SingleOrDefault<T>(GetFullSelectSql(sql), _args);
        }
        public T First<T>(Expression<Func<T1, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.First<T>(GetFullSelectSql(sql), _args);
        }
        public T FirstOrDefault<T>(Expression<Func<T1, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.FirstOrDefault<T>(GetFullSelectSql(sql), _args);
        }
        public List<T> SkipTake<T>(long skip, long take, Expression<Func<T1, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.Select<T>(skip, take, GetFullSelectSql(sql), _args);
        }
        public Page<T> Page<T>(long page, long itemsPerPage, Expression<Func<T1, T>> columns)
        {
            string sql;
            SqlExpression.GetColumns(columns, out sql);
            return _sqlhelper.Page<T>(page, itemsPerPage, GetFullSelectSql(sql), _args);
        }

        #endregion


        protected internal override List<Type> GetTypes()
        {
            return new List<Type>() { typeof(T1) };
        }

        protected internal override string GetFromAndJoinOn()
        {
            var pd1 = PocoData.ForType(typeof(T1));
            var dp = DatabaseProvider.Resolve(_sqlhelper._sqlType);
            StringBuilder sb = new StringBuilder();
            sb.Append("FROM ");



            //sb.Append(dp.GetTableName(pd1, _sqlhelper._tableNameManger));
            sb.Append(" AS t1 ");
            sb.Append(" ");
            sb.Append(_joinOnString);
            return sb.ToString();
        }

        /// <summary>
        /// 深度复制
        /// </summary>
        /// <returns></returns>
        public WhereHelper<T1> DeepClone()
        {
            WhereHelper<T1> newWhere = new WhereHelper<T1>(this._sqlhelper);
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
