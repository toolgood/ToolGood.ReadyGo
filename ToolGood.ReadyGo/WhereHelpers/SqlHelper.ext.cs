using ToolGood.ReadyGo.WhereHelpers;

namespace ToolGood.ReadyGo
{
    public partial class SqlHelper
    {
        /// <summary>
        /// 创建 WhereHelper
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public WhereHelper<T> CreateWhere<T>() where T : class, new()
        {
            var where = new WhereHelper<T>(this);
            return where;
        }

        public WhereHelper<T1, T2> CreateWhere<T1, T2>(JoinType join2 = JoinType.Inner)
            where T1 : class, new()
            where T2 : class, new()
        {
            var where = new WhereHelper<T1, T2>(this);
            where.join2 = join2;
            return where;
        }

        public WhereHelper<T1, T2, T3> CreateWhere<T1, T2, T3>(JoinType join2 = JoinType.Inner, JoinType join3 = JoinType.Inner)
                where T1 : class, new()
                where T2 : class, new()
                where T3 : class, new()
        {
            var where = new WhereHelper<T1, T2, T3>(this);
            where.join2 = join2;
            where.join3 = join3;
            return where;
        }

        public WhereHelper<T1, T2, T3, T4> CreateWhere<T1, T2, T3, T4>(JoinType join2 = JoinType.Inner, JoinType join3 = JoinType.Inner, JoinType join4 = JoinType.Inner)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
        {
            var where = new WhereHelper<T1, T2, T3, T4>(this);
            where.join2 = join2;
            where.join3 = join3;
            where.join4 = join4;
            return where;
        }

        public WhereHelper<T1, T2, T3, T4, T5> CreateWhere<T1, T2, T3, T4, T5>(JoinType join2 = JoinType.Inner, JoinType join3 = JoinType.Inner, JoinType join4 = JoinType.Inner, JoinType join5 = JoinType.Inner)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
        {
            var where = new WhereHelper<T1, T2, T3, T4, T5>(this);
            where.join2 = join2;
            where.join3 = join3;
            where.join4 = join4;
            where.join5 = join5;
            return where;
        }
    }
}