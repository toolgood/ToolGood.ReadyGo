using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ToolGood.ReadyGo.Caches;
using ToolGood.ReadyGo.Providers;
using ToolGood.ReadyGo.SqlBuilding;

namespace ToolGood.ReadyGo.WhereHelpers
{
    public interface IDeepCloneable<T>
    {
        T DeepClone();
    }


    public abstract class HelperBase : IDisposable
    {
        #region 私有变量
        protected SqlExpression SqlExpression;
        protected internal SqlHelper _sqlhelper;
        protected internal List<object> _args = new List<object>();
        protected internal StringBuilder _where = new StringBuilder();
        protected internal string _joinOnString = "";
        protected internal List<SelectHeader> _headers = new List<SelectHeader>();
        internal DatabaseProvider _provider;
        protected internal string _paramPrefix;

        #endregion 私有变量


        protected internal abstract List<Type> GetTypes();

        protected internal abstract string GetFromAndJoinOn();

        #region SQL拼接方法

        protected internal bool _doNext = true;

        protected internal bool jump()
        {
            if (_doNext == false) {
                _doNext = true;
                return true;
            }
            return false;
        }

        protected internal void ifTrue(bool iftrue)
        {
            _doNext = iftrue;
        }

        protected internal void whereNotIn(LambdaExpression field, ICollection args)
        {
            if (jump()) return;
            var column = SqlExpression.GetColumnName(field);

            if (_where.Length > 0) {
                _where.Append(" AND ");
            }
            if (args.Count == 0) {
                _where.Append("1=1");
                return;
            }
            _where.Append(column);
            if (args.Count == 1) {
                _where.Append(" <> ");
                _where.Append(_paramPrefix);
                _where.Append(_args.Count.ToString());
                return;
            } else {
                _where.Append(" NOT IN (");
                for (int i = 0; i < args.Count; i++) {
                    if (i > 0) {
                        _where.Append(",");
                    }
                    _where.Append(_paramPrefix);
                    _where.Append((_args.Count + i).ToString());
                }
                _where.Append(")");
            }
            foreach (var item in args) {
                _args.Add(item);
            }
        }

        protected internal void whereIn(LambdaExpression field, ICollection args)
        {
            if (jump()) return;
            var column = SqlExpression.GetColumnName(field);

            if (_where.Length > 0) {
                _where.Append(" AND ");
            }
            if (args.Count == 0) {
                _where.Append("1=2");
                return;
            }
            _where.Append(column);
            if (args.Count == 1) {
                _where.Append(" = ");
                _where.Append(_paramPrefix);
                _where.Append(_args.Count.ToString());
                return;
            } else {
                _where.Append(" IN (");
                for (int i = 0; i < args.Count; i++) {
                    if (i > 0) {
                        _where.Append(",");
                    }
                    _where.Append(_paramPrefix);
                    _where.Append((_args.Count + i).ToString());
                }
                _where.Append(")");
            }
            foreach (var item in args) {
                _args.Add(item);
            }
        }

        protected internal void where(string where, ICollection args)
        {
            if (jump()) return;
            where = where.Trim();
            if (_where.Length > 0) _where.Append(" AND ");

            int start = 0;
            if (where.StartsWith("where ", StringComparison.CurrentCultureIgnoreCase)) start = 6;

            bool isInText = false, isStart = false;
            var c = 'a';
            var text = "";

            for (int i = start; i < where.Length; i++) {
                var t = where[i];
                if (isInText) {
                    if (t == c) isInText = false;
                } else if ("\"'`".Contains(t)) {
                    isInText = true;
                    c = t;
                    isStart = false;
                } else if (isStart == false) {
                    if (t == '@') {
                        isStart = true;
                        text = "@";
                        continue;
                    }
                } else if ("@1234567890".Contains(t)) {
                    text += t;
                    continue;
                } else {
                    whereTranslate(_where, text);
                    isStart = false;
                }
                _where.Append(t);
            }
            if (isStart) whereTranslate(_where, text);


            foreach (var item in args) {
                _args.Add(item);
            }
        }
        private void whereTranslate(StringBuilder where, string text)
        {
            if (text == "@@") {
                where.Append(_paramPrefix);
                where.Append(this._args.Count.ToString());
            } else if (text == "@@@") {
                where.Append("@@");
            } else if (text.StartsWith("@@")) {
                int p = this._args.Count + int.Parse(text.Replace("@", ""));
                where.Append(_paramPrefix);
                where.Append(p.ToString());
            } else if (text.Length==1) {
                where.Append(text);
            } else {
                int p = int.Parse(text.Replace("@", ""));
                where.Append(_paramPrefix);
                where.Append(p.ToString());
            }
        }



        protected internal void where(LambdaExpression where)
        {
            if (jump()) return;
            string sql;
            SqlExpression.Analysis(where, out sql);
            if (_where.Length > 0) {
                _where.Append(" AND ");
            }
            _where.Append(sql);
        }


        protected internal void join(string join)
        {
            if (jump()) return;
            _joinOnString += " " + join;
        }

        protected internal void on(string on)
        {
            if (jump()) return;
            on = on.Trim();
            if (on.StartsWith("on ", StringComparison.CurrentCultureIgnoreCase))
                on = on.Substring(3).Trim();

            _joinOnString += " ON " + on;
        }
        protected internal void selectColumn(LambdaExpression column, string asName)
        {
            if (jump()) return;
            var col = SqlExpression.GetColumnName(column);
            selectColumn(col, asName);
        }
        protected internal void selectColumn(string col, string asName)
        {
            if (jump()) return;
            var index = col.IndexOf('.');
            string table = null;
            if (index > -1) {
                table = col.Substring(0, index);
                col = col.Substring(index + 1);
            }
            if (asName==null) {
                _headers.Insert(0, new SelectHeader() {
                    AsName = col,
                    Table = table,
                    QuerySql = col
                });
            } else {
                _headers.Insert(0, new SelectHeader() {
                    AsName = asName,
                    Table = table,
                    QuerySql = col
                });
            }
 
        }

        #endregion SQL拼接方法

        #region 04 获取Sql和args方法

        /// <summary>
        ///
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public virtual string GetFullSelectSql(string select = null)
        {
            if (select == null) {
                select = SelectHelper.CreateSelectHeader(_headers, GetTypes());
            }
            if (select.TrimStart().StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase) == false) {
                select = "SELECT " + select;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(select);
            sb.Append(" ");
            sb.Append(GetFromAndJoinOn());
            sb.Append(_joinOnString);

            if (_where.Length > 0) {
                sb.Append(" WHERE ");
                sb.Append(_where);
            }

            return sb.ToString();
        }

        internal virtual string GetFormWhereSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetFromAndJoinOn());
            sb.Append(_joinOnString);
            if (_where.Length > 0) {
                sb.Append(" WHERE ");
                sb.Append(_where);
            }
            return sb.ToString();
        }

        public string GetWhereSql()
        {
            StringBuilder sb = new StringBuilder();
            if (_where.Length > 0) {
                sb.Append(" WHERE ");
                sb.Append(_where);
            }
            return sb.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public object[] GetArgs()
        {
            return _args.ToArray();
        }

        #endregion 04 获取Sql和args方法

        public virtual void Dispose()
        {
            _args = null;
            _where = null;
            _joinOnString = null;
        }


    }

    public abstract class WhereHelperBase : HelperBase, IUseCache
    {
        #region 02 私有变量

        protected internal string _order = "";
        protected internal string _groupby = "";
        protected internal string _having = "";

        #endregion 02 私有变量

        #region 03 SQL拼接方法

        protected internal void orderBy(LambdaExpression order, OrderType type)
        {
            if (jump()) return;
            var column = SqlExpression.GetColumnName(order);
            if (type == OrderType.Asc) {
                orderBySql(column + " ASC");
            } else {
                orderBySql(column + " DESC");
            }
        }


        protected internal void orderBySql(string order)
        {
            if (jump()) return;
            if (_order.Length > 0) {
                _order += ",";
            }
            _order += order;
        }

        protected internal void groupBy(LambdaExpression group)
        {
            if (jump()) return;
            var column = SqlExpression.GetColumnName(group);
            this.groupBy(column);
        }

        protected internal void groupBy(string groupby)
        {
            if (jump()) return;
            groupby = groupby.Trim().Trim(',');
            if (groupby.StartsWith("group by ", StringComparison.CurrentCultureIgnoreCase))
                groupby = groupby.Substring(9).Trim();
            if (string.IsNullOrWhiteSpace(groupby)) return;
            if (_groupby.Length > 0) {
                _groupby += ",";
            }
            _groupby += groupby;
        }
        protected internal void having(LambdaExpression having)
        {
            if (jump()) return;
            string sql;
            SqlExpression.Analysis(having, out sql);
            this.having(sql);
        }
        protected internal void having(string having)
        {
            if (jump()) return;
            having = having.Trim().Trim(',');
            if (having.StartsWith("having ", StringComparison.CurrentCultureIgnoreCase))
                having = having.Substring(7).Trim();

            if (string.IsNullOrWhiteSpace(having)) return;
            if (_having.Length > 0) {
                _having += ",";
            }
            _having += having;
        }

        #endregion 03 SQL拼接方法

        #region 04 获取Sql和args方法

        /// <summary>
        ///
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public override string GetFullSelectSql(string select = null)
        {
            if (select == null) {
                select = SelectHelper.CreateSelectHeader(_headers, GetTypes());
            }
            if (select.TrimStart().StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase) == false) {
                select = "SELECT " + select;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(select);
            sb.Append(" ");
            sb.Append(GetFromAndJoinOn());
            sb.Append(_joinOnString);

            if (_where.Length > 0) {
                sb.Append(" WHERE ");
                sb.Append(_where);
            }

            if (_groupby.Length > 0) {
                sb.Append(" GROUP BY ");
                sb.Append(_groupby);
                if (_having.Length > 0) {
                    sb.Append(" HAVING ");
                    sb.Append(_having);
                }
            }
            if (_order.Length > 0) {
                sb.Append(" ORDER BY ");
                sb.Append(_order);
            }

            return sb.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public string GetCountSql(string select = null)
        {
            if (select == null) {
                select = "SELECT Count(1) ";
            }
            if (select.TrimStart().StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase) == false) {
                select = "SELECT " + select;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(select);
            sb.Append(_joinOnString);
            if (_where.Length > 0) {
                sb.Append(" WHERE ");
                sb.Append(_where);
            }

            if (_groupby.Length > 0) {
                sb.Append(" GROUP BY ");
                sb.Append(_groupby);
                if (_having.Length > 0) {
                    sb.Append(" HAVING ");
                    sb.Append(_having);
                }
            }
            return sb.ToString();
        }

        //internal override string GetFormWhereSql()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(_joinOnString);
        //    if (_where.Count > 0) {
        //        sb.Append(" WHERE ");
        //        sb.Append(string.Join(" AND ", _where));
        //    }
        //    return sb.ToString();
        //}

        #endregion 04 获取Sql和args方法

        #region 05 缓存设置

        private ICacheService _cacheService;
        private bool _usedCacheService;
        private int _cacheTime;
        private string _cacheTag;

        void IUseCache.useCache(int second, string cacheTag, ICacheService cacheService)
        {
            _usedCacheService = true;
            _cacheTime = second;
            _cacheTag = cacheTag;
            if (cacheService != null) {
                _cacheService = cacheService;
            }
        }

        protected internal void setCache()
        {
            if (_usedCacheService) {
                _sqlhelper.useCache(_cacheTime, _cacheTag, _cacheService);
            }
        }

        #endregion 05 缓存设置

        public override void Dispose()
        {
            _order = null;
            _groupby = null;
            _having = null;
            base.Dispose();
        }


    }
}