using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.ReadyGo.Providers;
using ToolGood.ReadyGo.SqlBuilding;

namespace ToolGood.ReadyGo.WhereHelpers
{
    /// <summary>
    /// 更新助手
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class UpdateHelper<T1> : HelperBase
             where T1 : class, new()
    {
        private List<Action<T1>> actions = new List<Action<T1>>();


        internal UpdateHelper(SqlHelper helper)
        {
            this._sqlhelper = helper;
        }

        /// <summary>
        /// 注：Update方法只支持直接赋值
        /// 注：UpdateWithSelect支持 拼接,计算操作
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public UpdateHelper<T1> SetValue(Action<T1> action)
        {
            if (jump()) return this;
            actions.Add(action);
            return this;
        }

        /// <summary>
        /// 更新 使用sql语名更新
        /// 注意SetValue方法中，使用直接赋值操作
        /// 不要使用 SetValue(q=>q.num=q.num*99+22)
        /// </summary>
        /// <returns></returns>
        public int Update()
        {
            T1 obj1 = Activator.CreateInstance<T1>();
            T1 obj2 = UpdateProxy<T1>.Create();
            foreach (var action in actions) {
                action(obj1);
                action(obj2);
            }

            var cs = (obj2 as IUpdateChange<T1>).__GetChanges__(obj1);
            var dp = DatabaseProvider.Resolve(_sqlhelper._sqlType);

            StringBuilder sb = new StringBuilder();
            foreach (var item in cs) {
                //if (pd.TableInfo.PrimaryKey == item.Key) continue;
                if (sb.Length > 0) sb.Append(", ");
                sb.AppendFormat("t1.{0} = {1}{2}", dp.EscapeSqlIdentifier(item.Key),
                       dp.GetParameterPrefix(_sqlhelper._connectionString),
                       _args.Count);
                _args.Add(item.Value);
            }
            sb.Insert(0, "UPDATE " + _joinOnString.ToString().Trim().Substring(5) + "  SET ");
            sb.Append(GetWhereSql());


            return _sqlhelper.Update<T1>(sb.ToString(), _args.ToArray());
        }

        /// <summary>
        /// 更新 先Select 再赋值 再Update 最后返回 List
        /// 相对Update功能更加强大，速度更慢
        /// </summary>
        /// <returns></returns>
        public List<T1> UpdateAndSelect()
        {
            var ts = _sqlhelper.Select<T1>(GetFullSelectSql(null), _args.ToArray());
            foreach (var item in ts) {
                foreach (var a in actions) {
                    a(item);
                }
                _sqlhelper.Save(item);
            }
            return ts;
        }

        protected internal override List<Type> GetTypes()
        {
            return new List<Type>() { typeof(T1) };
        }

        protected internal override string GetFromAndJoinOn()
        {
            return _joinOnString.ToString();
        }

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
            //sb.Append(_joinOnString);

            if (_where.Length > 0) {
                sb.Append(" WHERE ");
                sb.Append(_where);
            }

            return sb.ToString();
        }
    }
}