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
        public int GetCount()
        {
            return getSqlBuilder().GetCount();
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="distinctColumn">列</param>
        /// <returns></returns>
        public int GetCount(string distinctColumn)
        {
            return getSqlBuilder().GetCount(distinctColumn);
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="distinctColumn">列</param>
        /// <returns></returns>
        public int GetCount(QColumnBase distinctColumn)
        {
            return getSqlBuilder().GetCount(distinctColumn);
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table GetSingle<Table>()
        {
            return getSqlBuilder().GetSingle<Table>();
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table GetSingle<Table>(params string[] columns)
        {
            return getSqlBuilder().GetSingle<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table GetSingle<Table>(params QColumnBase[] columns)
        {
            return getSqlBuilder().GetSingle<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table GetSingleOrDefault<Table>()
        {
            return getSqlBuilder().GetSingleOrDefault<Table>();
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table GetSingleOrDefault<Table>(params string[] columns)
        {
            return getSqlBuilder().GetSingleOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table GetSingleOrDefault<Table>(params QColumnBase[] columns)
        {
            return getSqlBuilder().GetSingleOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table GetFirst<Table>()
        {
            return getSqlBuilder().GetFirst<Table>();
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table GetFirst<Table>(params string[] columns)
        {
            return getSqlBuilder().GetFirst<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table GetFirst<Table>(params QColumnBase[] columns)
        {
            return getSqlBuilder().GetFirst<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public Table GetFirstOrDefault<Table>()
        {
            return getSqlBuilder().GetFirstOrDefault<Table>();
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table GetFirstOrDefault<Table>(params string[] columns)
        {
            return getSqlBuilder().GetFirstOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Table GetFirstOrDefault<Table>(params QColumnBase[] columns)
        {
            return getSqlBuilder().GetFirstOrDefault<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public List<Table> GetList<Table>()
        {
            return getSqlBuilder().GetList<Table>();
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> GetList<Table>(params string[] columns)
        {
            return getSqlBuilder().GetList<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> GetList<Table>(params QColumnBase[] columns)
        {
            return getSqlBuilder().GetList<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <returns></returns>
        public List<Table> GetList<Table>(int limit)
        {
            return getSqlBuilder().GetList<Table>(limit);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> GetList<Table>(int limit, params string[] columns)
        {
            return getSqlBuilder().GetList<Table>(limit, columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> GetList<Table>(int limit, params QColumnBase[] columns)
        {
            return getSqlBuilder().GetList<Table>(limit, columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <returns></returns>
        public List<Table> GetList<Table>(int limit, int offset)
        {
            return getSqlBuilder().GetList<Table>(limit, offset);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> GetList<Table>(int limit, int offset, params string[] columns)
        {
            return getSqlBuilder().GetList<Table>(limit, offset, columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public List<Table> GetList<Table>(int limit, int offset, params QColumnBase[] columns)
        {
            return getSqlBuilder().GetList<Table>(limit, offset, columns);
        }
        /// <summary>
        /// 获取对象集合,【页集合】
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Page<Table> GetPage<Table>(int page, int size)
        {
            return getSqlBuilder().GetPage<Table>(page, size);
        }
        /// <summary>
        /// 获取对象集合,【页集合】
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Page<Table> GetPage<Table>(int page, int size, params string[] columns)
        {
            return getSqlBuilder().GetPage<Table>(page, size, columns);
        }
        /// <summary>
        /// 获取对象集合,【页集合】
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public Page<Table> GetPage<Table>(int page, int size, params QColumnBase[] columns)
        {
            return getSqlBuilder().GetPage<Table>(page, size, columns);
        }

        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable GetDataTable(params string[] columns)
        {
            return getSqlBuilder().GetDataTable(columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable GetDataTable(params QColumnBase[] columns)
        {
            return getSqlBuilder().GetDataTable(columns);
        }

        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable GetDataTable(int limit, params string[] columns)
        {
            return getSqlBuilder().GetDataTable(limit, columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable GetDataTable(int limit, params QColumnBase[] columns)
        {
            return getSqlBuilder().GetDataTable(limit, columns);
        }

        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable GetDataTable(int limit, int offset, params string[] columns)
        {
            return getSqlBuilder().GetDataTable(limit, offset, columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataTable GetDataTable(int limit, int offset, params QColumnBase[] columns)
        {
            return getSqlBuilder().GetDataTable(limit, offset, columns);
        }

        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet GetDataSet(params string[] columns)
        {
            return getSqlBuilder().GetDataSet(columns);
        }
        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet GetDataSet(params QColumnBase[] columns)
        {
            return getSqlBuilder().GetDataSet(columns);
        }

        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet GetDataSet(int limit, params string[] columns)
        {
            return getSqlBuilder().GetDataSet(limit, columns);
        }
        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet GetDataSet(int limit, params QColumnBase[] columns)
        {
            return getSqlBuilder().GetDataSet(limit, columns);
        }

        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet GetDataSet(int limit, int offset, params string[] columns)
        {
            return getSqlBuilder().GetDataSet(limit, offset, columns);
        }
        /// <summary>
        /// 获取DataSet对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public DataSet GetDataSet(int limit, int offset, params QColumnBase[] columns)
        {
            return getSqlBuilder().GetDataSet(limit, offset, columns);
        }
    }
}