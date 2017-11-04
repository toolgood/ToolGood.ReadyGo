using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class SqlBuilder : IDataGet<SqlBuilder>
    {
        /// <summary>
        /// 只能用一次，用过后请重新设置，不包括 GetCount
        /// </summary>
        /// <returns></returns>
        public SqlBuilder Distinct()
        {
            _useDistinct = true;
            return this;
        }

        #region GetCount
        public int GetCount()
        {
            return getCount(null);
        }

        public int GetCount(string distinctColumn)
        {
            return getCount(distinctColumn);
        }

        public int GetCount(QColumnBase distinctColumn)
        {
            var column = ((IColumnConvert)distinctColumn).ToSql(_provider, _tables.Count);
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

            var sql = ((ISqlBuilderConvert)this).GetFullSelectSql(_provider, 0, 0, columns);
            // TODO:


            if (t) _useDistinct = true;
            return 1;
        }

        #endregion

        private List<string> ToSelectColumns(QColumnBase[] columns)
        {
            List<string> selectColumns = new List<string>();
            foreach (var item in columns) {
                var column = ((IColumnConvert)item).ToSelectColumn(_provider, _tables.Count);
                selectColumns.Add(column);
            }
            return selectColumns;
        }
        private List<string> GetSelectColumns()
        {
            List<string> selectColumns = new List<string>();
            List<string> columnNames = new List<string>();

            foreach (var table in _tables) {
                foreach (var item in table._columns) {
                    var col = item.Value;

                    if (columnNames.Contains(col._asName)) {
                        var column = ((IColumnConvert)col).ToSelectColumn(_provider, _tables.Count);
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

            var pd = PetaPoco.Core.PocoData.ForType(typeof(T), null);
            var columns = pd.Columns.Select(q => q.Value.ColumnName).ToList();
            if (columns.Count == 0) throw new NoColumnException();

            foreach (var table in _tables) {
                foreach (var item in table._columns) {
                    var col = item.Value;
                    if (columns.Contains(col._columnName)) {
                        columns.Remove(col._columnName);
                        selectColumns.Add(((IColumnConvert) col).ToSql(_provider, _tables.Count));
                    }
                }
            }

            return selectColumns;
        }


        #region GetSingle GetSingleOrDefault GetFirst GetFirstOrDefault
        public T GetSingle<T>()
        {
            return getSingle<T>(GetSelectColumns<T>());
        }
        public T GetSingle<T>(params string[] columns)
        {
            return getSingle<T>(columns.ToList());
        }
        public T GetSingle<T>(params QColumnBase[] columns)
        {
            return getSingle<T>(ToSelectColumns(columns));
        }
        public T getSingle<T>(List<string> columns)
        {
            var sql = ((ISqlBuilderConvert)this).GetFullSelectSql(_provider, 2, 0, columns);
            _useDistinct = false;
            return _sqlHelper.Single<T>(sql);
        }



        public T GetSingleOrDefault<T>()
        {
            return getSingleOrDefault<T>(GetSelectColumns<T>());
        }
        public T GetSingleOrDefault<T>(params string[] columns)
        {
            return getSingleOrDefault<T>(columns.ToList());
        }
        public T GetSingleOrDefault<T>(params QColumnBase[] columns)
        {
            return getSingleOrDefault<T>(ToSelectColumns(columns));
        }
        public T getSingleOrDefault<T>(List<string> columns)
        {
            var sql = ((ISqlBuilderConvert)this).GetFullSelectSql(_provider, 2, 0, columns);
            _useDistinct = false;
            return _sqlHelper.SingleOrDefault<T>(sql);
        }



        public T GetFirst<T>()
        {
            return getFirst<T>(GetSelectColumns<T>());
        }
        public T GetFirst<T>(params string[] columns)
        {
            return getFirst<T>(columns.ToList());
        }
        public T GetFirst<T>(params QColumnBase[] columns)
        {
            return getFirst<T>(ToSelectColumns(columns));
        }
        public T getFirst<T>(List<string> columns)
        {
            var sql = ((ISqlBuilderConvert)this).GetFullSelectSql(_provider, 1, 0, columns);
            _useDistinct = false;
            return _sqlHelper.First<T>(sql);
        }



        public T GetFirstOrDefault<T>()
        {
            return getFirstOrDefault<T>(GetSelectColumns<T>());
        }
        public T GetFirstOrDefault<T>(params string[] columns)
        {
            return getFirstOrDefault<T>(columns.ToList());
        }
        public T GetFirstOrDefault<T>(params QColumnBase[] columns)
        {
            return getFirstOrDefault<T>(ToSelectColumns(columns));
        }
        public T getFirstOrDefault<T>(List<string> columns)
        {
            var sql = ((ISqlBuilderConvert)this).GetFullSelectSql(_provider, 1, 0, columns);
            _useDistinct = false;
            return _sqlHelper.FirstOrDefault<T>(sql);
        }

        #endregion

        #region GetList
        public List<T> GetList<T>()
        {
            var columns = GetSelectColumns<T>();
            return getList<T>(-1, -1, columns);
        }

        public List<T> GetList<T>(params string[] columns)
        {
            return getList<T>(-1, -1, columns.ToList());
        }

        public List<T> GetList<T>(params QColumnBase[] columns)
        {
            return getList<T>(-1, -1, ToSelectColumns(columns));
        }

        public List<T> GetList<T>(int limit)
        {
            var columns = GetSelectColumns<T>();
            return getList<T>(limit, -1, columns);
        }

        public List<T> GetList<T>(int limit, params string[] columns)
        {
            return getList<T>(limit, -1, columns.ToList());
        }

        public List<T> GetList<T>(int limit, params QColumnBase[] columns)
        {
            return getList<T>(limit, -1, ToSelectColumns(columns));
        }

        public List<T> GetList<T>(int limit, int offset)
        {
            var columns = GetSelectColumns<T>();
            return getList<T>(limit, offset, columns);
        }

        public List<T> GetList<T>(int limit, int offset, params string[] columns)
        {
            return getList<T>(limit, offset, columns.ToList());
        }

        public List<T> GetList<T>(int limit, int offset, params QColumnBase[] columns)
        {
            return getList<T>(limit, offset, ToSelectColumns(columns));
        }

        public List<T> getList<T>(int limit, int offset, List<string> columns)
        {
            var sql = ((ISqlBuilderConvert)this).GetFullSelectSql(_provider, limit, offset, columns);
            _useDistinct = false;
            return _sqlHelper.Select<T>(sql);
        }

        #endregion

        #region GetPage
        public Page<T> GetPage<T>(int page, int size)
        {
            var columns = GetSelectColumns();
            return getPage<T>(page, size, columns);
        }

        public Page<T> GetPage<T>(int page, int size, params string[] columns)
        {
            return getPage<T>(page, size, columns.ToList());
        }

        public Page<T> GetPage<T>(int page, int size, params QColumnBase[] columns)
        {
            return getPage<T>(page, size, ToSelectColumns(columns));
        }
        private Page<T> getPage<T>(int page, int size, List<string> columns)
        {
            var offset = (page - 1) * size;
            var limit = size;
            var sql = ((ISqlBuilderConvert)this).GetFullSelectSql(_provider, limit, offset, columns);
            _useDistinct = false;

            var count = getCount(null);
            Page<T> pt = new Page<T>();
            pt.TotalItems = count;
            pt.CurrentPage = page;
            pt.PageSize = size;
            pt.Items = _sqlHelper.Select<T>(sql);
            return pt;
        }

        #endregion

        #region GetDataTable
        public DataTable GetDataTable()
        {
            var columns = GetSelectColumns();
            return getDataTable(-1, -1, columns);
        }

        public DataTable GetDataTable(params string[] columns)
        {
            return getDataTable(-1, -1, columns.ToList());
        }

        public DataTable GetDataTable(params QColumnBase[] columns)
        {
            return getDataTable(-1, -1, ToSelectColumns(columns));
        }

        public DataTable GetDataTable(int limit)
        {
            var columns = GetSelectColumns();
            return getDataTable(limit, -1, columns);
        }

        public DataTable GetDataTable(int limit, params string[] columns)
        {
            return getDataTable(limit, -1, columns.ToList());
        }

        public DataTable GetDataTable(int limit, params QColumnBase[] columns)
        {
            return getDataTable(limit, -1, ToSelectColumns(columns));
        }

        public DataTable GetDataTable(int limit, int offset)
        {
            var columns = GetSelectColumns();
            return getDataTable(limit, offset, columns);
        }

        public DataTable GetDataTable(int limit, int offset, params string[] columns)
        {
            return getDataTable(limit, offset, columns.ToList());
        }

        public DataTable GetDataTable(int limit, int offset, params QColumnBase[] columns)
        {
            return getDataTable(limit, offset, ToSelectColumns(columns));
        }

        private DataTable getDataTable(int limit, int offset, List<string> columns)
        {
            var sql = ((ISqlBuilderConvert)this).GetFullSelectSql(_provider, limit, offset, columns);
            _useDistinct = false;
            return _sqlHelper.ExecuteDataTable(sql);
        }

        #endregion

        #region GetDataSet
        public DataSet GetDataSet()
        {
            var columns = GetSelectColumns();
            return getDataSet(-1, -1, columns);
        }

        public DataSet GetDataSet(params string[] columns)
        {
            return getDataSet(-1, -1, columns.ToList());
        }

        public DataSet GetDataSet(params QColumnBase[] columns)
        {
            return getDataSet(-1, -1, ToSelectColumns(columns));
        }

        public DataSet GetDataSet(int limit)
        {
            var columns = GetSelectColumns();
            return getDataSet(limit, -1, columns);
        }

        public DataSet GetDataSet(int limit, params string[] columns)
        {
            return getDataSet(limit, -1, columns.ToList());
        }

        public DataSet GetDataSet(int limit, params QColumnBase[] columns)
        {
            return getDataSet(limit, -1, ToSelectColumns(columns));
        }

        public DataSet GetDataSet(int limit, int offset)
        {
            var columns = GetSelectColumns();
            return getDataSet(limit, offset, columns);
        }

        public DataSet GetDataSet(int limit, int offset, params string[] columns)
        {
            return getDataSet(limit, offset, columns.ToList());
        }

        public DataSet GetDataSet(int limit, int offset, params QColumnBase[] columns)
        {
            return getDataSet(limit, offset, ToSelectColumns(columns));
        }
        private DataSet getDataSet(int limit, int offset, List<string> columns)
        {
            var sql = ((ISqlBuilderConvert)this).GetFullSelectSql(_provider, limit, offset, columns);
            _useDistinct = false;
            return _sqlHelper.ExecuteDataSet(sql);
        }
        #endregion
    }
}
