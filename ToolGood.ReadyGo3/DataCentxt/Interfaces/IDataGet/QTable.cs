using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QTable<T> //: IDataGet<QTable<T>>
    {
        /// <summary>
        /// 查询语句添加【Distinct】 
        /// </summary>
        /// <returns></returns>
        public QTable<T> Distinct()
        {
            GetSqlBuilder().Distinct();
            return this;
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <returns></returns>
        public int SelectCount()
        {
            return GetSqlBuilder().SelectCount();
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="distinctColumn">列</param>
        /// <returns></returns>
        public int SelectCount(string distinctColumn)
        {
            return GetSqlBuilder().SelectCount(distinctColumn);
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="distinctColumn">列</param>
        /// <returns></returns>
        public int SelectCount(QColumn distinctColumn)
        {
            return GetSqlBuilder().SelectCount(distinctColumn);
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table Single<Table>()
        {
            return GetSqlBuilder().Single<Table>();
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table Single<Table>(params string[] columns)
        {
            return GetSqlBuilder().Single<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table Single<Table>(params QColumn[] columns)
        {
            return GetSqlBuilder().Single<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table SingleOrDefault<Table>()
        {
            return GetSqlBuilder().SingleOrDefault<Table>();
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table SingleOrDefault<Table>(params string[] columns)
        {
            return GetSqlBuilder().SingleOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table SingleOrDefault<Table>(params QColumn[] columns)
        {
            return GetSqlBuilder().SingleOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table First<Table>()
        {
            return GetSqlBuilder().First<Table>();
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table First<Table>(params string[] columns)
        {
            return GetSqlBuilder().First<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table First<Table>(params QColumn[] columns)
        {
            return GetSqlBuilder().First<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table FirstOrDefault<Table>()
        {
            return GetSqlBuilder().FirstOrDefault<Table>();
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table FirstOrDefault<Table>(params string[] columns)
        {
            return GetSqlBuilder().FirstOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table FirstOrDefault<Table>(params QColumn[] columns)
        {
            return GetSqlBuilder().FirstOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public List<Table> Select<Table>()
        {
            return GetSqlBuilder().Select<Table>();
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> Select<Table>(params string[] columns)
        {
            return GetSqlBuilder().Select<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> Select<Table>(params QColumn[] columns)
        {
            return GetSqlBuilder().Select<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <returns></returns>
        public List<Table> Select<Table>(int limit)
        {
            return GetSqlBuilder().Select<Table>(limit);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> Select<Table>(int limit, params string[] columns)
        {
            return GetSqlBuilder().Select<Table>(limit, columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> Select<Table>(int limit, params QColumn[] columns)
        {
            return GetSqlBuilder().Select<Table>(limit, columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <returns></returns>
        public List<Table> Select<Table>(int limit, int offset)
        {
            return GetSqlBuilder().Select<Table>(limit, offset);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> Select<Table>(int limit, int offset, params string[] columns)
        {
            return GetSqlBuilder().Select<Table>(limit, offset, columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> Select<Table>(int limit, int offset, params QColumn[] columns)
        {
            return GetSqlBuilder().Select<Table>(limit, offset, columns);
        }
        /// <summary>
        /// 获取对象集合,【页集合】
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Page<Table> Page<Table>(int page, int size)
        {
            return GetSqlBuilder().Page<Table>(page, size);
        }
        /// <summary>
        /// 获取对象集合,【页集合】
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Page<Table> Page<Table>(int page, int size, params string[] columns)
        {
            return GetSqlBuilder().Page<Table>(page, size, columns);
        }
        /// <summary>
        /// 获取对象集合,【页集合】
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Page<Table> Page<Table>(int page, int size, params QColumn[] columns)
        {
            return GetSqlBuilder().Page<Table>(page, size, columns);
        }

        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(params string[] columns)
        {
            return GetSqlBuilder().ExecuteDataTable(columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(params QColumn[] columns)
        {
            return GetSqlBuilder().ExecuteDataTable(columns);
        }

        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(int limit, params string[] columns)
        {
            return GetSqlBuilder().ExecuteDataTable(limit, columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(int limit, params QColumn[] columns)
        {
            return GetSqlBuilder().ExecuteDataTable(limit, columns);
        }

        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(int limit, int offset, params string[] columns)
        {
            return GetSqlBuilder().ExecuteDataTable(limit, offset, columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(int limit, int offset, params QColumn[] columns)
        {
            return GetSqlBuilder().ExecuteDataTable(limit, offset, columns);
        }

        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(params string[] columns)
        {
            return GetSqlBuilder().ExecuteDataSet(columns);
        }
        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(params QColumn[] columns)
        {
            return GetSqlBuilder().ExecuteDataSet(columns);
        }

        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(int limit, params string[] columns)
        {
            return GetSqlBuilder().ExecuteDataSet(limit, columns);
        }
        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(int limit, params QColumn[] columns)
        {
            return GetSqlBuilder().ExecuteDataSet(limit, columns);
        }

        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(int limit, int offset, params string[] columns)
        {
            return GetSqlBuilder().ExecuteDataSet(limit, offset, columns);
        }
        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(int limit, int offset, params QColumn[] columns)
        {
            return GetSqlBuilder().ExecuteDataSet(limit, offset, columns);
        }
    }
}