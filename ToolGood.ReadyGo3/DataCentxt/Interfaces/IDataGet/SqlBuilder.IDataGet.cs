using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;


namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class SqlBuilder //: IDataGet<SqlBuilder>
    {
        /// <summary>
        /// 只能用一次，用过后请重新设置，不包括 SelectCount
        /// </summary>
        /// <returns></returns>
        public SqlBuilder Distinct()
        {
            _useDistinct = true;
            return this;
        }

        #region SelectCount
        /// <summary>
        /// 执行SQL 查询，返回个数
        /// </summary>
        /// <returns></returns>
        public int SelectCount()
        {
            return getCount(null);
        }
        /// <summary>
        /// 执行SQL 查询，返回个数
        /// </summary>
        /// <param name="distinctColumn"></param>
        /// <returns></returns>
        public int SelectCount(string distinctColumn)
        {
            return getCount(distinctColumn);
        }
        /// <summary>
        /// 执行SQL 查询，返回个数
        /// </summary>
        /// <param name="distinctColumn"></param>
        /// <returns></returns>
        public int SelectCount(QColumn distinctColumn)
        {
            var column = (distinctColumn).ToSql(Provider, _tables.Count);
            return getCount(column);
        }
        private int getCount(string column)
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
            var count = GetSqlHelper().ExecuteScalar<int>(sql);

            if (t) _useDistinct = true;
            return count;
        }

        #endregion

        private List<string> ToSelectColumns(QColumn[] columns)
        {
            var count = string.IsNullOrEmpty(_joinOnText) ? 0 : 1;

            List<string> selectColumns = new List<string>();
            foreach (var item in columns) {
                var column = (item).ToSelectColumn(Provider, _tables.Count+ count);
                selectColumns.Add(column);
            }
            return selectColumns;
        }
        private List<string> GetSelectColumns()
        {
            List<string> selectColumns = new List<string>();
            List<string> columnNames = new List<string>();

            var count = string.IsNullOrEmpty(_joinOnText) ? 0 : 1;

            foreach (var table in _tables) {
                foreach (var item in table._columns) {
                    var col = item.Value;

                    if (columnNames.Contains(col._asName)) {
                        var column = (col).ToSelectColumn(Provider, _tables.Count+ count);
                        columnNames.Add(col._asName);
                        selectColumns.Add(column);
                    }
                }
            }
            return selectColumns;
        }
        private List<string> GetSelectColumns<T>()
        {
            var type = typeof(T);
            var tc = Type.GetTypeCode(type);
            if (tc != TypeCode.Object) throw new NoColumnException();

            List<string> selectColumns = new List<string>();

            var pd = PetaPoco.Core.PocoData.ForType(typeof(T));
            var columns = pd.Columns.Select(q => q.Key).ToList();
            if (columns.Count == 0) throw new NoColumnException();

            var count = string.IsNullOrEmpty(_joinOnText) ? 0 : 1;

            foreach (var table in _tables) {
                foreach (var item in table._columns) {
                    var col = item.Value;
                    if (columns.Contains(col._columnName)) {
                        columns.Remove(col._columnName);
                        selectColumns.Add((col).ToSelectColumn(Provider, _tables.Count+ count));
                    } else if (columns.Contains(col._asName)) {
                        columns.Remove(col._asName);
                        selectColumns.Add((col).ToSelectColumn(Provider, _tables.Count+ count));
                    }
                }
            }

            return selectColumns;
        }


        #region Single SingleOrDefault First FirstOrDefault
        public T Single<T>()
        {
            return getSingle<T>(GetSelectColumns<T>());
        }
        public T Single<T>(params string[] columns)
        {
            return getSingle<T>(columns.ToList());
        }
        public T Single<T>(params QColumn[] columns)
        {
            return getSingle<T>(ToSelectColumns(columns));
        }
        public T getSingle<T>(List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, 2, 0, columns);
            _useDistinct = false;
            return GetSqlHelper()._Single<T>(sql);
        }



        public T SingleOrDefault<T>()
        {
            return getSingleOrDefault<T>(GetSelectColumns<T>());
        }
        public T SingleOrDefault<T>(params string[] columns)
        {
            return getSingleOrDefault<T>(columns.ToList());
        }
        public T SingleOrDefault<T>(params QColumn[] columns)
        {
            return getSingleOrDefault<T>(ToSelectColumns(columns));
        }
        private T getSingleOrDefault<T>(List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, 2, 0, columns);
            _useDistinct = false;
            return GetSqlHelper()._SingleOrDefault<T>(sql);
        }



        public T First<T>()
        {
            return getFirst<T>(GetSelectColumns<T>());
        }
        public T First<T>(params string[] columns)
        {
            return getFirst<T>(columns.ToList());
        }
        public T First<T>(params QColumn[] columns)
        {
            return getFirst<T>(ToSelectColumns(columns));
        }
        private T getFirst<T>(List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, 1, 0, columns);
            _useDistinct = false;
            return GetSqlHelper()._First<T>(sql);
        }



        public T FirstOrDefault<T>()
        {
            return getFirstOrDefault<T>(GetSelectColumns<T>());
        }
        public T FirstOrDefault<T>(params string[] columns)
        {
            return getFirstOrDefault<T>(columns.ToList());
        }
        public T FirstOrDefault<T>(params QColumn[] columns)
        {
            return getFirstOrDefault<T>(ToSelectColumns(columns));
        }
        private T getFirstOrDefault<T>(List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, 1, 0, columns);
            _useDistinct = false;
            return GetSqlHelper()._FirstOrDefault<T>(sql);
        }

        #endregion

        #region Select
        public List<T> Select<T>()
        {
            var columns = GetSelectColumns<T>();
            return getList<T>(-1, -1, columns);
        }

        public List<T> Select<T>(params string[] columns)
        {
            return getList<T>(-1, -1, columns.ToList());
        }

        public List<T> Select<T>(params QColumn[] columns)
        {
            return getList<T>(-1, -1, ToSelectColumns(columns));
        }

        public List<T> Select<T>(int limit)
        {
            var columns = GetSelectColumns<T>();
            return getList<T>(limit, -1, columns);
        }

        public List<T> Select<T>(int limit, params string[] columns)
        {
            return getList<T>(limit, -1, columns.ToList());
        }

        public List<T> Select<T>(int limit, params QColumn[] columns)
        {
            return getList<T>(limit, -1, ToSelectColumns(columns));
        }

        public List<T> Select<T>(int limit, int offset)
        {
            var columns = GetSelectColumns<T>();
            return getList<T>(limit, offset, columns);
        }

        public List<T> Select<T>(int limit, int offset, params string[] columns)
        {
            return getList<T>(limit, offset, columns.ToList());
        }

        public List<T> Select<T>(int limit, int offset, params QColumn[] columns)
        {
            return getList<T>(limit, offset, ToSelectColumns(columns));
        }

        private List<T> getList<T>(int limit, int offset, List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, limit, offset, columns);
            _useDistinct = false;
            return GetSqlHelper().Select<T>(sql);
        }

        #endregion

        #region Page
        public Page<T> Page<T>(int page, int size)
        {
            var columns = GetSelectColumns<T>();
            return getPage<T>(page, size, columns);
        }

        public Page<T> Page<T>(int page, int size, params string[] columns)
        {
            return getPage<T>(page, size, columns.ToList());
        }

        public Page<T> Page<T>(int page, int size, params QColumn[] columns)
        {
            return getPage<T>(page, size, ToSelectColumns(columns));
        }
        private Page<T> getPage<T>(int page, int size, List<string> columns)
        {
            var offset = (page - 1) * size;
            var limit = size;
            var sql = GetFullSelectSql(Provider, limit, offset, columns);
            _useDistinct = false;

            var count = getCount(null);
            Page<T> pt = new Page<T>();
            pt.TotalItems = count;
            pt.CurrentPage = page;
            pt.PageSize = size;
            pt.Items = GetSqlHelper().Select<T>(sql);
            return pt;
        }

        #endregion

        #region ExecuteDataTable
        public DataTable ExecuteDataTable()
        {
            var columns = GetSelectColumns();
            return getDataTable(-1, -1, columns);
        }

        public DataTable ExecuteDataTable(params string[] columns)
        {
            return getDataTable(-1, -1, columns.ToList());
        }

        public DataTable ExecuteDataTable(params QColumn[] columns)
        {
            return getDataTable(-1, -1, ToSelectColumns(columns));
        }

        public DataTable ExecuteDataTable(int limit)
        {
            var columns = GetSelectColumns();
            return getDataTable(limit, -1, columns);
        }

        public DataTable ExecuteDataTable(int limit, params string[] columns)
        {
            return getDataTable(limit, -1, columns.ToList());
        }

        public DataTable ExecuteDataTable(int limit, params QColumn[] columns)
        {
            return getDataTable(limit, -1, ToSelectColumns(columns));
        }

        public DataTable ExecuteDataTable(int limit, int offset)
        {
            var columns = GetSelectColumns();
            return getDataTable(limit, offset, columns);
        }

        public DataTable ExecuteDataTable(int limit, int offset, params string[] columns)
        {
            return getDataTable(limit, offset, columns.ToList());
        }

        public DataTable ExecuteDataTable(int limit, int offset, params QColumn[] columns)
        {
            return getDataTable(limit, offset, ToSelectColumns(columns));
        }

        private DataTable getDataTable(int limit, int offset, List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, limit, offset, columns);
            _useDistinct = false;
            return GetSqlHelper().ExecuteDataTable(sql);
        }

        #endregion

#region ExecuteDataSet
#if !NETSTANDARD2_0
        public DataSet ExecuteDataSet()
        {
            var columns = GetSelectColumns();
            return getDataSet(-1, -1, columns);
        }

        public DataSet ExecuteDataSet(params string[] columns)
        {
            return getDataSet(-1, -1, columns.ToList());
        }

        public DataSet ExecuteDataSet(params QColumn[] columns)
        {
            return getDataSet(-1, -1, ToSelectColumns(columns));
        }

        public DataSet ExecuteDataSet(int limit)
        {
            var columns = GetSelectColumns();
            return getDataSet(limit, -1, columns);
        }

        public DataSet ExecuteDataSet(int limit, params string[] columns)
        {
            return getDataSet(limit, -1, columns.ToList());
        }

        public DataSet ExecuteDataSet(int limit, params QColumn[] columns)
        {
            return getDataSet(limit, -1, ToSelectColumns(columns));
        }

        public DataSet ExecuteDataSet(int limit, int offset)
        {
            var columns = GetSelectColumns();
            return getDataSet(limit, offset, columns);
        }

        public DataSet ExecuteDataSet(int limit, int offset, params string[] columns)
        {
            return getDataSet(limit, offset, columns.ToList());
        }

        public DataSet ExecuteDataSet(int limit, int offset, params QColumn[] columns)
        {
            return getDataSet(limit, offset, ToSelectColumns(columns));
        }
        private DataSet getDataSet(int limit, int offset, List<string> columns)
        {
            var sql = GetFullSelectSql(Provider, limit, offset, columns);
            _useDistinct = false;
            return GetSqlHelper().ExecuteDataSet(sql);
        }
#endif
#endregion
    }
}
