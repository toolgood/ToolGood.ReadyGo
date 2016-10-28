namespace ToolGood.ReadyGo.Internals
{
    /// <summary>
    /// 辅助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryColumn<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 加一
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static QueryColumn<T> operator ++(QueryColumn<T> left)
        {
            return left;
        }
        /// <summary>
        /// 减一
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static QueryColumn<T> operator --(QueryColumn<T> left)
        {
            return left;
        }
        /// <summary>
        /// 相加
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static QueryColumn<T> operator +(QueryColumn<T> left, T right)
        {
            return left;
        }
        /// <summary>
        /// 相减
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static QueryColumn<T> operator -(QueryColumn<T> left, T right)
        {
            return left;
        }
        /// <summary>
        /// 相乘
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static QueryColumn<T> operator *(QueryColumn<T> left, T right)
        {
            return left;
        }
        /// <summary>
        /// 相除
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static QueryColumn<T> operator /(QueryColumn<T> left, T right)
        {
            return left;
        }
        /// <summary>
        /// mod
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static QueryColumn<T> operator %(QueryColumn<T> left, T right)
        {
            return left;
        }
        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(QueryColumn<T> left, T right)
        {
            return false;
        }
        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(QueryColumn<T> left, T right)
        {
            return false;
        }
        /// <summary>
        /// 大于或等于
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(QueryColumn<T> left, T right)
        {
            return false;
        }
        /// <summary>
        /// 小于或等于
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(QueryColumn<T> left, T right)
        {
            return false;
        }
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(QueryColumn<T> left, T right)
        {
            return false;
        }
        /// <summary>
        /// 不等于
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(QueryColumn<T> left, T right)
        {
            return false;
        }
    }
}