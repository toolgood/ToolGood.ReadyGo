using ToolGood.ReadyGo.Internals;

namespace ToolGood.ReadyGo
{
    /// <summary>
    /// 支持SelectColumn、GroupBy、Having
    /// </summary>
    public static class SQL
    {
        /// <summary>
        /// 求合，
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static QueryColumn<T> Sum<T>(T t)
        {
            return new QueryColumn<T>();
        }
        /// <summary>
        /// 取个数
        /// </summary>
        /// <returns></returns>
        public static QueryColumn<long> CountAll()
        {
            return new QueryColumn<long>();
        }
        /// <summary>
        /// 获取非重复个数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static QueryColumn<long> CountOfDistinct<T>(T t)
        {
            return new QueryColumn<long>();
        }
        /// <summary>
        /// 求个数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static QueryColumn<long> Count<T>(T t)
        {
            return new QueryColumn<long>();
        }
        /// <summary>
        /// 求最小值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static QueryColumn<T> Min<T>(T t)
        {
            return new QueryColumn<T>();
        }
        /// <summary>
        /// 求最大值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static QueryColumn<T> Max<T>(T t)
        {
            return new QueryColumn<T>();
        }
        /// <summary>
        /// 求非重复值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static QueryColumn<T> Distinct<T>(T t)
        {
            return new QueryColumn<T>();
        }
        /// <summary>
        /// 求平均
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static QueryColumn<T> Avg<T>(T t)
        {
            return new QueryColumn<T>();
        }
    }
}