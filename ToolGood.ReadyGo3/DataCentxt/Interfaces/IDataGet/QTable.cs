using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QTable<T> : IDataGet<QTable<T>>
    {
        /// <summary>
        /// 查询语句添加【Distinct】 
        /// </summary>
        /// <returns></returns>
        public QTable<T> Distinct()
        {
            getSqlBuilder().Distinct();
            return this;
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <returns></returns>
        public int SelectCount()
        {
            return getSqlBuilder().SelectCount();
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="distinctColumn">列</param>
        /// <returns></returns>
        public int SelectCount(string distinctColumn)
        {
            return getSqlBuilder().SelectCount(distinctColumn);
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="distinctColumn">列</param>
        /// <returns></returns>
        public int SelectCount(QColumnBase distinctColumn)
        {
            return getSqlBuilder().SelectCount(distinctColumn);
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table Single<Table>()
        {
            return getSqlBuilder().Single<Table>();
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table Single<Table>(params string[] columns)
        {
            return getSqlBuilder().Single<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table Single<Table>(params QColumnBase[] columns)
        {
            return getSqlBuilder().Single<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table SingleOrDefault<Table>()
        {
            return getSqlBuilder().SingleOrDefault<Table>();
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table SingleOrDefault<Table>(params string[] columns)
        {
            return getSqlBuilder().SingleOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table SingleOrDefault<Table>(params QColumnBase[] columns)
        {
            return getSqlBuilder().SingleOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table First<Table>()
        {
            return getSqlBuilder().First<Table>();
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table First<Table>(params string[] columns)
        {
            return getSqlBuilder().First<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table First<Table>(params QColumnBase[] columns)
        {
            return getSqlBuilder().First<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table FirstOrDefault<Table>()
        {
            return getSqlBuilder().FirstOrDefault<Table>();
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table FirstOrDefault<Table>(params string[] columns)
        {
            return getSqlBuilder().FirstOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table FirstOrDefault<Table>(params QColumnBase[] columns)
        {
            return getSqlBuilder().FirstOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public List<Table> Select<Table>()
        {
            return getSqlBuilder().Select<Table>();
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> Select<Table>(params string[] columns)
        {
            return getSqlBuilder().Select<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> Select<Table>(params QColumnBase[] columns)
        {
            return getSqlBuilder().Select<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <returns></returns>
        public List<Table> Select<Table>(int limit)
        {
            return getSqlBuilder().Select<Table>(limit);
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
            return getSqlBuilder().Select<Table>(limit, columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> Select<Table>(int limit, params QColumnBase[] columns)
        {
            return getSqlBuilder().Select<Table>(limit, columns);
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
            return getSqlBuilder().Select<Table>(limit, offset);
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
            return getSqlBuilder().Select<Table>(limit, offset, columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> Select<Table>(int limit, int offset, params QColumnBase[] columns)
        {
            return getSqlBuilder().Select<Table>(limit, offset, columns);
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
            return getSqlBuilder().Page<Table>(page, size);
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
            return getSqlBuilder().Page<Table>(page, size, columns);
        }
        /// <summary>
        /// 获取对象集合,【页集合】
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Page<Table> Page<Table>(int page, int size, params QColumnBase[] columns)
        {
            return getSqlBuilder().Page<Table>(page, size, columns);
        }

        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(params string[] columns)
        {
            return getSqlBuilder().ExecuteDataTable(columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(params QColumnBase[] columns)
        {
            return getSqlBuilder().ExecuteDataTable(columns);
        }

        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(int limit, params string[] columns)
        {
            return getSqlBuilder().ExecuteDataTable(limit, columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(int limit, params QColumnBase[] columns)
        {
            return getSqlBuilder().ExecuteDataTable(limit, columns);
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
            return getSqlBuilder().ExecuteDataTable(limit, offset, columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(int limit, int offset, params QColumnBase[] columns)
        {
            return getSqlBuilder().ExecuteDataTable(limit, offset, columns);
        }

        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(params string[] columns)
        {
            return getSqlBuilder().ExecuteDataSet(columns);
        }
        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(params QColumnBase[] columns)
        {
            return getSqlBuilder().ExecuteDataSet(columns);
        }

        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(int limit, params string[] columns)
        {
            return getSqlBuilder().ExecuteDataSet(limit, columns);
        }
        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(int limit, params QColumnBase[] columns)
        {
            return getSqlBuilder().ExecuteDataSet(limit, columns);
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
            return getSqlBuilder().ExecuteDataSet(limit, offset, columns);
        }
        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(int limit, int offset, params QColumnBase[] columns)
        {
            return getSqlBuilder().ExecuteDataSet(limit, offset, columns);
        }
    }
}