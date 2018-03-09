using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class SqlBuilder
    {
        #region UpdateAsync DeleteAsync InsertAsync
        public async Task<int> DeleteAsync()
        {
            var sql = (this).GetFullDeleteSql(Provider);
            return await GetSqlHelper().ExecuteAsync(sql);
        }

        public async Task<object> InsertAsync(bool returnInsertId = false)
        {
            var sql = (this).GetFullInsertSql(Provider);
            if (returnInsertId) {
                var pk = (_tables[0]).GetPrimaryKey();
                if (object.Equals(pk, null)) {
                    throw new NoPrimaryKeyException();
                }
                return await GetSqlHelper().InsertAsync(sql, pk._columnName);
            }
            return await GetSqlHelper().ExecuteAsync(sql);
        }

        public async Task<int> UpdateAsync()
        {
            var sql = (this).GetFullUpdateSql(Provider);
            return await GetSqlHelper().ExecuteAsync(sql);
        }
        #endregion

        #region SelectCount

        public async Task<int> SelectCountAsync()
        {
            return await getCountAsync(null);
        }

        public async Task<int> SelectCountAsync(string distinctColumn)
        {
            return await getCountAsync(distinctColumn);
        }

        public async Task<int> SelectCountAsync(QColumn distinctColumn)
        {
            var column = (distinctColumn).ToSql(Provider, _tables.Count);
            return await getCountAsync(column);
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
            var sql = (this).GetFullSelectSql(Provider, 0, 0, columns);
            var count = await GetSqlHelper().ExecuteScalarAsync<int>(sql);

            if (t) _useDistinct = true;
            return count;
        }

        #endregion



        #region Single SingleOrDefault First FirstOrDefault
        public async Task<T> SingleAsync<T>()
        {
            return await getSingleAsync<T>(GetSelectColumns<T>());
        }
        public async Task<T> SingleAsync<T>(params string[] columns)
        {
            return await getSingleAsync<T>(columns.ToList());
        }
        public async Task<T> SingleAsync<T>(params QColumn[] columns)
        {
            return await getSingleAsync<T>(ToSelectColumns(columns));
        }
        public async Task<T> getSingleAsync<T>(List<string> columns)
        {
            var sql = (this).GetFullSelectSql(Provider, 2, 0, columns);
            _useDistinct = false;
            return await GetSqlHelper().SingleAsync<T>(sql);
        }



        public async Task<T> SingleOrDefaultAsync<T>()
        {
            return await getSingleOrDefaultAsync<T>(GetSelectColumns<T>());
        }
        public async Task<T> SingleOrDefaultAsync<T>(params string[] columns)
        {
            return await getSingleOrDefaultAsync<T>(columns.ToList());
        }
        public async Task<T> SingleOrDefaultAsync<T>(params QColumn[] columns)
        {
            return await getSingleOrDefaultAsync<T>(ToSelectColumns(columns));
        }
        public async Task<T> getSingleOrDefaultAsync<T>(List<string> columns)
        {
            var sql = (this).GetFullSelectSql(Provider, 2, 0, columns);
            _useDistinct = false;
            return await GetSqlHelper().SingleOrDefaultAsync<T>(sql);
        }



        public async Task<T> FirstAsync<T>()
        {
            return await getFirstAsync<T>(GetSelectColumns<T>());
        }
        public async Task<T> FirstAsync<T>(params string[] columns)
        {
            return await getFirstAsync<T>(columns.ToList());
        }
        public async Task<T> FirstAsync<T>(params QColumn[] columns)
        {
            return await getFirstAsync<T>(ToSelectColumns(columns));
        }
        public async Task<T> getFirstAsync<T>(List<string> columns)
        {
            var sql = (this).GetFullSelectSql(Provider, 1, 0, columns);
            _useDistinct = false;
            return await GetSqlHelper().FirstAsync<T>(sql);
        }



        public async Task<T> FirstOrDefaultAsync<T>()
        {
            return await getFirstOrDefaultAsync<T>(GetSelectColumns<T>());
        }
        public async Task<T> FirstOrDefaultAsync<T>(params string[] columns)
        {
            return await getFirstOrDefaultAsync<T>(columns.ToList());
        }
        public async Task<T> FirstOrDefaultAsync<T>(params QColumn[] columns)
        {
            return await getFirstOrDefaultAsync<T>(ToSelectColumns(columns));
        }
        public async Task<T> getFirstOrDefaultAsync<T>(List<string> columns)
        {
            var sql = (this).GetFullSelectSql(Provider, 1, 0, columns);
            _useDistinct = false;
            return await GetSqlHelper().FirstOrDefaultAsync<T>(sql);
        }

        #endregion

        #region Select
        public async Task<List<T>> SelectAsync<T>()
        {
            var columns = GetSelectColumns<T>();
            return await getListAsync<T>(-1, -1, columns);
        }

        public async Task<List<T>> SelectAsync<T>(params string[] columns)
        {
            return await getListAsync<T>(-1, -1, columns.ToList());
        }

        public async Task<List<T>> SelectAsync<T>(params QColumn[] columns)
        {
            return await getListAsync<T>(-1, -1, ToSelectColumns(columns));
        }

        public async Task<List<T>> SelectAsync<T>(int limit)
        {
            var columns = GetSelectColumns<T>();
            return await getListAsync<T>(limit, -1, columns);
        }

        public async Task<List<T>> SelectAsync<T>(int limit, params string[] columns)
        {
            return await getListAsync<T>(limit, -1, columns.ToList());
        }

        public async Task<List<T>> SelectAsync<T>(int limit, params QColumn[] columns)
        {
            return await getListAsync<T>(limit, -1, ToSelectColumns(columns));
        }

        public async Task<List<T>> SelectAsync<T>(int limit, int offset)
        {
            var columns = GetSelectColumns<T>();
            return await getListAsync<T>(limit, offset, columns);
        }

        public async Task<List<T>> SelectAsync<T>(int limit, int offset, params string[] columns)
        {
            return await getListAsync<T>(limit, offset, columns.ToList());
        }

        public async Task<List<T>> SelectAsync<T>(int limit, int offset, params QColumn[] columns)
        {
            return await getListAsync<T>(limit, offset, ToSelectColumns(columns));
        }

        public async Task<List<T>> getListAsync<T>(int limit, int offset, List<string> columns)
        {
            var sql = (this).GetFullSelectSql(Provider, limit, offset, columns);
            _useDistinct = false;
            return await GetSqlHelper().SelectAsync<T>(sql);
        }

        #endregion

        #region Page
        public async Task<Page<T>> PageAsync<T>(int page, int size)
        {
            var columns = GetSelectColumns<T>();
            return await getPageAsync<T>(page, size, columns);
        }

        public async Task<Page<T>> PageAsync<T>(int page, int size, params string[] columns)
        {
            return await getPageAsync<T>(page, size, columns.ToList());
        }

        public async Task<Page<T>> PageAsync<T>(int page, int size, params QColumn[] columns)
        {
            return await getPageAsync<T>(page, size, ToSelectColumns(columns));
        }
        private async Task<Page<T>> getPageAsync<T>(int page, int size, List<string> columns)
        {
            var offset = (page - 1) * size;
            var limit = size;
            var sql = (this).GetFullSelectSql(Provider, limit, offset, columns);
            _useDistinct = false;

            var count = await getCountAsync(null);
            Page<T> pt = new Page<T>();
            pt.TotalItems = count;
            pt.CurrentPage = page;
            pt.PageSize = size;
            pt.Items =await GetSqlHelper().SelectAsync<T>(sql);
            return  pt;
        }

        #endregion

        #region ExecuteDataTable
        public async Task<DataTable> ExecuteDataTableAsync()
        {
            var columns = GetSelectColumns();
            return await getDataTableAsync(-1, -1, columns);
        }

        public async Task<DataTable> ExecuteDataTableAsync(params string[] columns)
        {
            return await getDataTableAsync(-1, -1, columns.ToList());
        }

        public async Task<DataTable> ExecuteDataTableAsync(params QColumn[] columns)
        {
            return await getDataTableAsync(-1, -1, ToSelectColumns(columns));
        }

        public async Task<DataTable> ExecuteDataTableAsync(int limit)
        {
            var columns = GetSelectColumns();
            return await getDataTableAsync(limit, -1, columns);
        }

        public async Task<DataTable> ExecuteDataTableAsync(int limit, params string[] columns)
        {
            return await getDataTableAsync(limit, -1, columns.ToList());
        }

        public async Task<DataTable> ExecuteDataTableAsync(int limit, params QColumn[] columns)
        {
            return await getDataTableAsync(limit, -1, ToSelectColumns(columns));
        }

        public async Task<DataTable> ExecuteDataTableAsync(int limit, int offset)
        {
            var columns = GetSelectColumns();
            return await getDataTableAsync(limit, offset, columns);
        }

        public async Task<DataTable> ExecuteDataTableAsync(int limit, int offset, params string[] columns)
        {
            return await getDataTableAsync(limit, offset, columns.ToList());
        }

        public async Task<DataTable> ExecuteDataTableAsync(int limit, int offset, params QColumn[] columns)
        {
            return await getDataTableAsync(limit, offset, ToSelectColumns(columns));
        }

        private async Task<DataTable> getDataTableAsync(int limit, int offset, List<string> columns)
        {
            var sql = (this).GetFullSelectSql(Provider, limit, offset, columns);
            _useDistinct = false;
            return await GetSqlHelper().ExecuteDataTableAsync(sql);
        }

        #endregion


    }
}
