using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QTable<T>
    {
        #region UpdateAsync DeleteAsync InsertAsync
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateAsync()
        {
            return await GetSqlBuilder().UpdateAsync();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAsync()
        {
            return await GetSqlBuilder().DeleteAsync();
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="returnInsertId">是否返回插入的ID</param>
        /// <returns></returns>
        public async Task<object> InsertAsync(bool returnInsertId = false)
        {
            var config = GetSqlHelper()._Config;
            if (config.Insert_DateTime_Default_Now
                || config.Insert_Guid_Default_New
                || config.Insert_String_Default_NotNull) {
                (this).SetDefaultValue(config.Insert_DateTime_Default_Now,
                    config.Insert_String_Default_NotNull, config.Insert_Guid_Default_New);
            }
            return await GetSqlBuilder().InsertAsync(returnInsertId);
        }
        #endregion

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> SelectCountAsync()
        {
            return await GetSqlBuilder().SelectCountAsync();
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="distinctColumn">列</param>
        /// <returns></returns>
        public async Task<int> SelectCountAsync(string distinctColumn)
        {
            return await GetSqlBuilder().SelectCountAsync(distinctColumn);
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="distinctColumn">列</param>
        /// <returns></returns>
        public async Task<int> SelectCountAsync(QColumn distinctColumn)
        {
            return await GetSqlBuilder().SelectCountAsync(distinctColumn);
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public async Task<Table> SingleAsync<Table>()
        {
            return await GetSqlBuilder().SingleAsync<Table>();
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<Table> SingleAsync<Table>(params string[] columns)
        {
            return await GetSqlBuilder().SingleAsync<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,不能为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<Table> SingleAsync<Table>(params QColumn[] columns)
        {
            return await GetSqlBuilder().SingleAsync<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public async Task<Table> SingleOrDefaultAsync<Table>()
        {
            return await GetSqlBuilder().SingleOrDefaultAsync<Table>();
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<Table> SingleOrDefaultAsync<Table>(params string[] columns)
        {
            return await GetSqlBuilder().SingleOrDefaultAsync<Table>(columns);
        }
        /// <summary>
        /// 获取唯一对象,可以为Null，如果返回2列则报错，查询时添加limit 2
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<Table> SingleOrDefaultAsync<Table>(params QColumn[] columns)
        {
            return await GetSqlBuilder().SingleOrDefaultAsync<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public async Task<Table> FirstAsync<Table>()
        {
            return await GetSqlBuilder().FirstAsync<Table>();
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<Table> FirstAsync<Table>(params string[] columns)
        {
            return await GetSqlBuilder().FirstAsync<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,不能为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<Table> FirstAsync<Table>(params QColumn[] columns)
        {
            return await GetSqlBuilder().FirstAsync<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public async Task<Table> FirstOrDefaultAsync<Table>()
        {
            return await GetSqlBuilder().FirstOrDefaultAsync<Table>();
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<Table> FirstOrDefaultAsync<Table>(params string[] columns)
        {
            return await GetSqlBuilder().FirstOrDefaultAsync<Table>(columns);
        }
        /// <summary>
        /// 获取第一个对象,可以为Null，查询时添加limit 1
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<Table> FirstOrDefaultAsync<Table>(params QColumn[] columns)
        {
            return await GetSqlBuilder().FirstOrDefaultAsync<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public async Task<List<Table>> SelectAsync<Table>()
        {
            return await GetSqlBuilder().SelectAsync<Table>();
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<List<Table>> SelectAsync<Table>(params string[] columns)
        {
            return await GetSqlBuilder().SelectAsync<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<List<Table>> SelectAsync<Table>(params QColumn[] columns)
        {
            return await GetSqlBuilder().SelectAsync<Table>(columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <returns></returns>
        public async Task<List<Table>> SelectAsync<Table>(int limit)
        {
            return await GetSqlBuilder().SelectAsync<Table>(limit);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<List<Table>> SelectAsync<Table>(int limit, params string[] columns)
        {
            return await GetSqlBuilder().SelectAsync<Table>(limit, columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<List<Table>> SelectAsync<Table>(int limit, params QColumn[] columns)
        {
            return await GetSqlBuilder().SelectAsync<Table>(limit, columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <returns></returns>
        public async Task<List<Table>> SelectAsync<Table>(int limit, int offset)
        {
            return await GetSqlBuilder().SelectAsync<Table>(limit, offset);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<List<Table>> SelectAsync<Table>(int limit, int offset, params string[] columns)
        {
            return await GetSqlBuilder().SelectAsync<Table>(limit, offset, columns);
        }
        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<List<Table>> SelectAsync<Table>(int limit, int offset, params QColumn[] columns)
        {
            return await GetSqlBuilder().SelectAsync<Table>(limit, offset, columns);
        }
        /// <summary>
        /// 获取对象集合,【页集合】
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<Page<Table>> PageAsync<Table>(int page, int size)
        {
            return await GetSqlBuilder().PageAsync<Table>(page, size);
        }
        /// <summary>
        /// 获取对象集合,【页集合】
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<Page<Table>> PageAsync<Table>(int page, int size, params string[] columns)
        {
            return await GetSqlBuilder().PageAsync<Table>(page, size, columns);
        }
        /// <summary>
        /// 获取对象集合,【页集合】
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<Page<Table>> PageAsync<Table>(int page, int size, params QColumn[] columns)
        {
            return await GetSqlBuilder().PageAsync<Table>(page, size, columns);
        }

        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(params string[] columns)
        {
            return await GetSqlBuilder().ExecuteDataTableAsync(columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(params QColumn[] columns)
        {
            return await GetSqlBuilder().ExecuteDataTableAsync(columns);
        }

        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(int limit, params string[] columns)
        {
            return await GetSqlBuilder().ExecuteDataTableAsync(limit, columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(int limit, params QColumn[] columns)
        {
            return await GetSqlBuilder().ExecuteDataTableAsync(limit, columns);
        }

        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(int limit, int offset, params string[] columns)
        {
            return await GetSqlBuilder().ExecuteDataTableAsync(limit, offset, columns);
        }
        /// <summary>
        /// 获取DataTable对象
        /// </summary>
        /// <param name="limit">限制个数</param>
        /// <param name="offset">偏移数量</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(int limit, int offset, params QColumn[] columns)
        {
            return await GetSqlBuilder().ExecuteDataTableAsync(limit, offset, columns);
        }



        #region Single SingleOrDefault First FirstOrDefault
        /// <summary>
        /// 获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <returns></returns>
        public async Task<T> SingleAsync()
        {
            return await SingleAsync<T>(getSelectColumn());
        }
        /// <summary>
        /// 获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> SingleAsync(params string[] columns)
        {
            return await SingleAsync<T>(columns);
        }
        /// <summary>
        /// 获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> SingleAsync(params QColumn[] columns)
        {
            return await SingleAsync<T>(columns);
        }
        /// <summary>
        /// 获取唯一一个类型，可为空，若数量大于1，则抛出异常
        /// </summary>
        /// <returns></returns>
        public async Task<T> SingleOrDefaultAsync()
        {
            return await SingleOrDefaultAsync<T>(getSelectColumn());
        }
        /// <summary>
        /// 获取唯一一个类型，可为空，若数量大于1，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> SingleOrDefaultAsync(params string[] columns)
        {
            return await SingleOrDefaultAsync<T>(columns);
        }
        /// <summary>
        /// 获取唯一一个类型，可为空，若数量大于1，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> SingleOrDefaultAsync(params QColumn[] columns)
        {
            return await SingleOrDefaultAsync<T>(columns);
        }
        /// <summary>
        /// 获取第一个类型，若数量为0，则抛出异常
        /// </summary>
        /// <returns></returns>
        public async Task<T> FirstAsync()
        {
            return await FirstAsync<T>(getSelectColumn());
        }
        /// <summary>
        /// 获取第一个类型，若数量为0，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> FirstAsync(params string[] columns)
        {
            return await FirstAsync<T>(columns);
        }
        /// <summary>
        /// 获取第一个类型，若数量为0，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> FirstAsync(params QColumn[] columns)
        {
            return await FirstAsync<T>(columns);
        }
        /// <summary>
        /// 获取第一个类型，可为空，若数量为0，则抛出异常
        /// </summary>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync()
        {
            return await FirstOrDefaultAsync<T>(getSelectColumn());
        }
        /// <summary>
        /// 获取第一个类型，可为空，若数量为0，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync(params string[] columns)
        {
            return await FirstOrDefaultAsync<T>(columns);
        }
        /// <summary>
        /// 获取第一个类型，可为空，若数量为0，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync(params QColumn[] columns)
        {
            return await FirstOrDefaultAsync<T>(columns);
        }
        #endregion

        #region Select
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync()
        {
            return await SelectAsync<T>(getSelectColumn());
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync(params string[] columns)
        {
            return await SelectAsync<T>(columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync(params QColumn[] columns)
        {
            return await SelectAsync<T>(columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync(int limit)
        {
            return await SelectAsync<T>(limit, getSelectColumn());
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync(int limit, params string[] columns)
        {
            return await SelectAsync<T>(limit, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync(int limit, params QColumn[] columns)
        {
            return await SelectAsync<T>(limit, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync(int limit, int offset)
        {
            return await SelectAsync<T>(limit, offset, getSelectColumn());
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync(int limit, int offset, params string[] columns)
        {
            return await SelectAsync<T>(limit, offset, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync(int limit, int offset, params QColumn[] columns)
        {
            return await SelectAsync<T>(limit, offset, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<Page<T>> PageAsync(int page, int size)
        {
            return await PageAsync<T>(page, size, getSelectColumn());
        }
        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<Page<T>> PageAsync(int page, int size, params string[] columns)
        {
            return await PageAsync<T>(page, size, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<Page<T>> PageAsync(int page, int size, params QColumn[] columns)
        {
            return await PageAsync<T>(page, size, columns);
        }
        #endregion

        #region ExecuteDataTable
        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync()
        {
            var columns = getSelectColumn();
            return await ExecuteDataTableAsync(-1, -1, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(int limit)
        {
            var columns = getSelectColumn();
            return await ExecuteDataTableAsync(limit, -1, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(int limit, int offset)
        {
            var columns = getSelectColumn();
            return await ExecuteDataTableAsync(limit, offset, columns);
        }

        #endregion


    }
}
