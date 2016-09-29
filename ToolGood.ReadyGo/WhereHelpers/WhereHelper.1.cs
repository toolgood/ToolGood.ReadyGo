using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.Providers;
using ToolGood.ReadyGo.SqlBuilding;

namespace ToolGood.ReadyGo.WhereHelpers
{
    public partial class WhereHelper<T1> : WhereHelperBase
         where T1 : class, new()
    {
        public WhereHelper(SqlHelper helper)
        {
            this._sqlhelper = helper;
            SqlExpression = SqlExpression.Resolve(_sqlhelper._sqlType);
        }

        #region WhereIn Where OrderBy Having

        public WhereHelper<T1> WhereNotIn<T>(Expression<Func<T1, T>> field, ICollection args)
        {
            base.whereNotIn(field, args);
            return this;
        }

        public WhereHelper<T1> WhereNotIn<T>(Expression<Func<T1, T>> field, params T[] args)
        {
            return WhereNotIn(field, (ICollection)args);
        }

        public WhereHelper<T1> WhereIn<T>(Expression<Func<T1, T>> field, ICollection args)
        {
            base.whereIn(field, args);
            return this;
        }

        public WhereHelper<T1> WhereIn<T>(Expression<Func<T1, T>> field, params T[] args)
        {
            return WhereIn(field, (ICollection)args);
        }

        public WhereHelper<T1> Where(Expression<Func<T1, bool>> where)
        {
            base.where(where);
            return this;
        }

        public WhereHelper<T1> OrderBy<T>(Expression<Func<T1, T>> order, OrderType type = OrderType.Asc)
        {
            base.orderBy(order, type);
            return this;
        }

        public WhereHelper<T1> GroupBy<T>(Expression<Func<T1, T>> group)
        {
            groupBy(group);
            return this;
        }

        public WhereHelper<T1> Having(Expression<Func<T1, bool>> having)
        {
            base.having(having);
            return this;
        }

        public WhereHelper<T1> SelectColumn<T>(Expression<Func<T1, T>> column, string asName = null)
        {
            selectColumn(column, asName);
            return this;
        }



        #endregion WhereIn Where OrderBy Having

        #region Select Page SkipTake Single SingleOrDefault First FirstOrDefault

        public List<T1> Select(string selectSql = null)
        {
            setCache();
            return _sqlhelper.Select<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }

        public Page<T1> Page(long page, long itemsPerPage, string selectSql = null)
        {
            return _sqlhelper.Page<T1>(page, itemsPerPage, GetFullSelectSql(selectSql), _args.ToArray());
        }

        public List<T1> SkipTake(long skip, long take, string selectSql = null)
        {
            setCache();
            return _sqlhelper.SkipTake<T1>(skip, take, GetFullSelectSql(selectSql), _args.ToArray());
        }

        public T1 Single(string selectSql = null)
        {
            setCache();
            return _sqlhelper.Single<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }

        public T1 SingleOrDefault(string selectSql = null)
        {
            setCache();
            return _sqlhelper.SingleOrDefault<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }

        public T1 First(string selectSql = null)
        {
            setCache();
            return _sqlhelper.First<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }

        public T1 FirstOrDefault(string selectSql = null)
        {
            setCache();
            return _sqlhelper.FirstOrDefault<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }

        #endregion Select Page SkipTake Single SingleOrDefault First FirstOrDefault

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
            this._doNext = true;
            return where;
        }

        #endregion SetValue



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
            sb.Append(dp.GetTableName(pd1, _sqlhelper._tableNameManger));
            sb.Append(" AS t1 ");
            sb.Append(" ");
            sb.Append(_joinOnString);
            return sb.ToString();
        }
    }
}