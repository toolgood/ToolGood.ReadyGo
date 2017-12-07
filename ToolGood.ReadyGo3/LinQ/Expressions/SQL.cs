using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.LinQ
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
