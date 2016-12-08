using System;
using System.Collections.Generic;

namespace ToolGood.ReadyGo
{
    /// <summary>
    /// ITableRow类型
    /// </summary>
    public interface ITableRow
    {
        /// <summary>
        /// 获取表数据
        /// </summary>
        int GetTableCount { get; }
        /// <summary>
        /// 设置表
        /// </summary>
        /// <param name="i"></param>
        /// <param name="obj"></param>
        void SetTable(int i, dynamic obj);
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        ITableRow Clone();
        /// <summary>
        /// 获取表类型
        /// </summary>
        /// <returns></returns>
        List<Type> GetTypes();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    [Serializable]
    public class TableRow<T1, T2> : ITableRow
    {
        /// <summary>
        /// 表一
        /// </summary>
        public T1 t1 { get; set; }
        /// <summary>
        /// 表二
        /// </summary>
        public T2 t2 { get; set; }

        int ITableRow.GetTableCount { get { return 2; } }

        ITableRow ITableRow.Clone()
        {
            return (ITableRow)new TableRow<T1, T2>();
        }

        List<Type> ITableRow.GetTypes()
        {
            return new List<Type>() { typeof(T1), typeof(T2) };
        }

        void ITableRow.SetTable(int i, dynamic obj)
        {
            if (i == 0) {
                t1 = (T1)obj;
            } else if (i == 1) {
                t2 = (T2)obj;
            }
        }
        /// <summary>
        /// 转Tuple
        /// </summary>
        /// <param name="row"></param>
        public static implicit operator Tuple<T1, T2>(TableRow<T1, T2> row)
        {
            return Tuple.Create(row.t1, row.t2);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    [Serializable]
    public class TableRow<T1, T2, T3> : ITableRow
    {
        /// <summary>
        /// 表一
        /// </summary>
        public T1 t1 { get; set; }
        /// <summary>
        /// 表二
        /// </summary>
        public T2 t2 { get; set; }
        /// <summary>
        /// 表三
        /// </summary>
        public T3 t3 { get; set; }

        int ITableRow.GetTableCount { get { return 3; } }

        ITableRow ITableRow.Clone()
        {
            return (ITableRow)new TableRow<T1, T2, T3>();
        }

        List<Type> ITableRow.GetTypes()
        {
            return new List<Type>() { typeof(T1), typeof(T2), typeof(T3) };
        }

        void ITableRow.SetTable(int i, dynamic obj)
        {
            if (i == 0) {
                t1 = (T1)obj;
            } else if (i == 1) {
                t2 = (T2)obj;
            } else if (i == 2) {
                t3 = (T3)obj;
            }
        }
        /// <summary>
        /// 转Tuple
        /// </summary>
        /// <param name="row"></param>
        public static implicit operator Tuple<T1, T2, T3>(TableRow<T1, T2, T3> row)
        {
            return Tuple.Create(row.t1, row.t2, row.t3);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    [Serializable]
    public class TableRow<T1, T2, T3, T4> : ITableRow
    {
        /// <summary>
        /// 表一
        /// </summary>
        public T1 t1 { get; set; }
        /// <summary>
        /// 表二
        /// </summary>
        public T2 t2 { get; set; }
        /// <summary>
        /// 表三
        /// </summary>
        public T3 t3 { get; set; }
        /// <summary>
        /// 表四
        /// </summary>
        public T4 t4 { get; set; }

        int ITableRow.GetTableCount { get { return 4; } }

        ITableRow ITableRow.Clone()
        {
            return (ITableRow)new TableRow<T1, T2, T3, T4>();
        }

        List<Type> ITableRow.GetTypes()
        {
            return new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
        }

        void ITableRow.SetTable(int i, dynamic obj)
        {
            if (i == 0) {
                t1 = (T1)obj;
            } else if (i == 1) {
                t2 = (T2)obj;
            } else if (i == 2) {
                t3 = (T3)obj;
            } else if (i == 4) {
                t4 = (T4)obj;
            }
        }
        /// <summary>
        /// 转Tuple
        /// </summary>
        /// <param name="row"></param>
        public static implicit operator Tuple<T1, T2, T3, T4>(TableRow<T1, T2, T3, T4> row)
        {
            return Tuple.Create(row.t1, row.t2, row.t3, row.t4);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    [Serializable]
    public class TableRow<T1, T2, T3, T4, T5> : ITableRow
    {
        /// <summary>
        /// 表一
        /// </summary>
        public T1 t1 { get; set; }
        /// <summary>
        /// 表二
        /// </summary>
        public T2 t2 { get; set; }
        /// <summary>
        /// 表三
        /// </summary>
        public T3 t3 { get; set; }
        /// <summary>
        /// 表四
        /// </summary>
        public T4 t4 { get; set; }
        /// <summary>
        /// 表五
        /// </summary>
        public T5 t5 { get; set; }

        int ITableRow.GetTableCount { get { return 5; } }

        ITableRow ITableRow.Clone()
        {
            return (ITableRow)new TableRow<T1, T2, T3, T4, T5>();
        }

        List<Type> ITableRow.GetTypes()
        {
            return new List<Type>() { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
        }

        void ITableRow.SetTable(int i, dynamic obj)
        {
            if (i == 0) {
                t1 = (T1)obj;
            } else if (i == 1) {
                t2 = (T2)obj;
            } else if (i == 2) {
                t3 = (T3)obj;
            } else if (i == 4) {
                t4 = (T4)obj;
            } else {
                t5 = (T5)obj;
            }
        }
        /// <summary>
        /// 转
        /// </summary>
        /// <param name="row"></param>
        public static implicit operator Tuple<T1, T2, T3, T4, T5>(TableRow<T1, T2, T3, T4, T5> row)
        {
            return Tuple.Create(row.t1, row.t2, row.t3, row.t4, row.t5);
        }
    }
}