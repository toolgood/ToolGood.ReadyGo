using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
#if !NET40

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class SqlBuilder
    {
        #region UpdateAsync DeleteAsync InsertAsync
        public Task<int> DeleteAsync()
        {
            var sql = GetFullDeleteSql(Provider);
            return GetSqlHelper().ExecuteAsync(sql);
        }

        public async Task<object> InsertAsync(bool returnInsertId = false)
        {
            var sql = GetFullInsertSql(Provider);
            if (returnInsertId) {
                var pk = (_tables[0]).GetPrimaryKey();
                if (object.Equals(pk, null)) {
                    throw new NoPrimaryKeyException();
                }
                return await GetSqlHelper().InsertAsync(sql, pk._columnName);
            }
            return await GetSqlHelper().ExecuteAsync(sql);
        }

        public Task<int> UpdateAsync()
        {
            var sql = GetFullUpdateSql(Provider);
            return GetSqlHelper().ExecuteAsync(sql);
        }
        #endregion

        #region SelectCount

        public Task<int> SelectCountAsync()
        {
            return getCountAsync(null);
        }

        public Task<int> SelectCountAsync(string distinctColumn)
        {
            return getCountAsync(distinctColumn);
        }

        public Task<int> SelectCountAsync(QColumn distinctColumn)
        {
            var column = (distinctColumn).ToSql(Provider, _tables.Count);
            return getCountAsync(column);
        }
        private async Task<int> getCountAsync(string column)
        {
            var t = _useDistinct;
            if (_useDistinct) _useDistinct = false;

            List<string> columns = new List<string>();
            if (string.IsNullOrEmpty(column)) {
                columns.Add("COUNT(1)");
            } else {
                columns.Add("COUNT(DISTINCT " + column + ")");
            }
            var sql = GetFullSelectSql(Provider, 0, 0, columns);
            var count = await GetSqlHelper().ExecuteScalarAsync<int>(sql);

            if (t) _useDistinct = true;
            return count;
        }

        #endregion

        #region Single SingleOrDefault First FirstOrDefault
        public Task<T> SingleAsync<T>()
        {
            return getSingleAsync<T>(GetSelectColumns<T>());
        }
        public Task<T> SingleAsync<T>(params string[] columns)
        {
            return getSingleAsync<T>(columns.ToList());
        }
        public Task<T> SingleAsync<T>(params QColumn[] columns)
        {
            return getSingleAsync<T>(ToSelectColumns(columns));
        }
        private Task<T> getSingleAsync<T>(List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, 2, 0, columns);
            _useDistinct = false;
            return GetSqlHelper()._SingleAsync<T>(sql);
        }



        public Task<T> SingleOrDefaultAsync<T>()
        {
            return getSingleOrDefaultAsync<T>(GetSelectColumns<T>());
        }
        public Task<T> SingleOrDefaultAsync<T>(params string[] columns)
        {
            return getSingleOrDefaultAsync<T>(columns.ToList());
        }
        public Task<T> SingleOrDefaultAsync<T>(params QColumn[] columns)
        {
            return getSingleOrDefaultAsync<T>(ToSelectColumns(columns));
        }
        private Task<T> getSingleOrDefaultAsync<T>(List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, 2, 0, columns);
            _useDistinct = false;
            return GetSqlHelper()._SingleOrDefaultAsync<T>(sql);
        }



        public Task<T> FirstAsync<T>()
        {
            return getFirstAsync<T>(GetSelectColumns<T>());
        }
        public Task<T> FirstAsync<T>(params string[] columns)
        {
            return getFirstAsync<T>(columns.ToList());
        }
        public Task<T> FirstAsync<T>(params QColumn[] columns)
        {
            return getFirstAsync<T>(ToSelectColumns(columns));
        }
        private Task<T> getFirstAsync<T>(List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, 1, 0, columns);
            _useDistinct = false;
            return GetSqlHelper()._FirstAsync<T>(sql);
        }



        public Task<T> FirstOrDefaultAsync<T>()
        {
            return getFirstOrDefaultAsync<T>(GetSelectColumns<T>());
        }
        public Task<T> FirstOrDefaultAsync<T>(params string[] columns)
        {
            return getFirstOrDefaultAsync<T>(columns.ToList());
        }
        public Task<T> FirstOrDefaultAsync<T>(params QColumn[] columns)
        {
            return getFirstOrDefaultAsync<T>(ToSelectColumns(columns));
        }
        private Task<T> getFirstOrDefaultAsync<T>(List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, 1, 0, columns);
            _useDistinct = false;
            return GetSqlHelper()._FirstOrDefaultAsync<T>(sql);
        }

        #endregion

        #region Select
        public Task<List<T>> SelectAsync<T>()
        {
            var columns = GetSelectColumns<T>();
            return getListAsync<T>(-1, -1, columns);
        }

        public Task<List<T>> SelectAsync<T>(params string[] columns)
        {
            return getListAsync<T>(-1, -1, columns.ToList());
        }

        public Task<List<T>> SelectAsync<T>(params QColumn[] columns)
        {
            return getListAsync<T>(-1, -1, ToSelectColumns(columns));
        }

        public Task<List<T>> SelectAsync<T>(int limit)
        {
            var columns = GetSelectColumns<T>();
            return getListAsync<T>(limit, -1, columns);
        }

        public Task<List<T>> SelectAsync<T>(int limit, params string[] columns)
        {
            return getListAsync<T>(limit, -1, columns.ToList());
        }

        public Task<List<T>> SelectAsync<T>(int limit, params QColumn[] columns)
        {
            return getListAsync<T>(limit, -1, ToSelectColumns(columns));
        }

        public Task<List<T>> SelectAsync<T>(int limit, int offset)
        {
            var columns = GetSelectColumns<T>();
            return getListAsync<T>(limit, offset, columns);
        }

        public Task<List<T>> SelectAsync<T>(int limit, int offset, params string[] columns)
        {
            return getListAsync<T>(limit, offset, columns.ToList());
        }

        public Task<List<T>> SelectAsync<T>(int limit, int offset, params QColumn[] columns)
        {
            return getListAsync<T>(limit, offset, ToSelectColumns(columns));
        }

        private Task<List<T>> getListAsync<T>(int limit, int offset, List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, limit, offset, columns);
            _useDistinct = false;
            return GetSqlHelper().SelectAsync<T>(sql);
        }

        #endregion

        #region Page
        public Task<Page<T>> PageAsync<T>(int page, int size)
        {
            var columns = GetSelectColumns<T>();
            return getPageAsync<T>(page, size, columns);
        }

        public Task<Page<T>> PageAsync<T>(int page, int size, params string[] columns)
        {
            return getPageAsync<T>(page, size, columns.ToList());
        }

        public Task<Page<T>> PageAsync<T>(int page, int size, params QColumn[] columns)
        {
            return getPageAsync<T>(page, size, ToSelectColumns(columns));
        }
        private async Task<Page<T>> getPageAsync<T>(int page, int size, List<string> columns)
        {
            var offset = (page - 1) * size;
            var limit = size;
            var sql = GetFullSelectSql(Provider, limit, offset, columns);
            _useDistinct = false;

            var count = await getCountAsync(null);
            Page<T> pt = new Page<T>();
            pt.TotalItems = count;
            pt.CurrentPage = page;
            pt.PageSize = size;
            pt.Items = await GetSqlHelper().SelectAsync<T>(sql);
            return pt;
        }

        #endregion

        #region ExecuteDataTable
        public Task<DataTable> ExecuteDataTableAsync()
        {
            var columns = GetSelectColumns();
            return getDataTableAsync(-1, -1, columns);
        }

        public Task<DataTable> ExecuteDataTableAsync(params string[] columns)
        {
            return getDataTableAsync(-1, -1, columns.ToList());
        }

        public Task<DataTable> ExecuteDataTableAsync(params QColumn[] columns)
        {
            return getDataTableAsync(-1, -1, ToSelectColumns(columns));
        }

        public Task<DataTable> ExecuteDataTableAsync(int limit)
        {
            var columns = GetSelectColumns();
            return getDataTableAsync(limit, -1, columns);
        }

        public Task<DataTable> ExecuteDataTableAsync(int limit, params string[] columns)
        {
            return getDataTableAsync(limit, -1, columns.ToList());
        }

        public Task<DataTable> ExecuteDataTableAsync(int limit, params QColumn[] columns)
        {
            return getDataTableAsync(limit, -1, ToSelectColumns(columns));
        }

        public Task<DataTable> ExecuteDataTableAsync(int limit, int offset)
        {
            var columns = GetSelectColumns();
            return getDataTableAsync(limit, offset, columns);
        }

        public Task<DataTable> ExecuteDataTableAsync(int limit, int offset, params string[] columns)
        {
            return getDataTableAsync(limit, offset, columns.ToList());
        }

        public Task<DataTable> ExecuteDataTableAsync(int limit, int offset, params QColumn[] columns)
        {
            return getDataTableAsync(limit, offset, ToSelectColumns(columns));
        }

        private Task<DataTable> getDataTableAsync(int limit, int offset, List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, limit, offset, columns);
            _useDistinct = false;
            return GetSqlHelper().ExecuteDataTableAsync(sql);
        }

        #endregion

        #region SelectInsert
        /// <summary>
        /// Insert Into T(*)  Select * from T
        /// </summary>
        /// <param name="insertTableName"></param>
        /// <param name="replaceColumns"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<int> SelectInsertAsync(string insertTableName, string replaceColumns, object[] args)
        {
            var sql = CreateSelectInsertSql(_tables[0].GetTableType(), insertTableName, replaceColumns, args);
            return GetSqlHelper().ExecuteAsync(sql);
        }




        /// <summary>
        /// Insert Into T1(*)  Select * from T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="insertTableName"></param>
        /// <param name="replaceColumns"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<int> SelectInsertAsync<T>(string insertTableName, string replaceColumns, object[] args)
        {
            var sql = CreateSelectInsertSql(typeof(T), insertTableName, replaceColumns, args);
            return GetSqlHelper().ExecuteAsync(sql);
        }
        #endregion
    }
}
#endif