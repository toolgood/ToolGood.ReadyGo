using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QTable<T>
    {
        private QColumn[] getSelectColumn()
        {
            List<QColumn> list = new List<QColumn>();
            foreach (var item in _columns) {
                var col = item.Value;
                list.Add(col);
            }
            return list.ToArray();
        }

        #region Single SingleOrDefault First FirstOrDefault

        public T Single()
        {
            return Single<T>(getSelectColumn());
        }

        public T Single(params string[] columns)
        {
            return Single<T>(columns);
        }

        public T Single(params QColumn[] columns)
        {
            return Single<T>(columns);
        }

        public T SingleOrDefault()
        {
            return SingleOrDefault<T>(getSelectColumn());
        }

        public T SingleOrDefault(params string[] columns)
        {
            return SingleOrDefault<T>(columns);
        }

        public T SingleOrDefault(params QColumn[] columns)
        {
            return SingleOrDefault<T>(columns);
        }

        public T First()
        {
            return First<T>(getSelectColumn());
        }

        public T First(params string[] columns)
        {
            return First<T>(columns);
        }

        public T First(params QColumn[] columns)
        {
            return First<T>(columns);
        }

        public T FirstOrDefault()
        {
            return FirstOrDefault<T>(getSelectColumn());
        }

        public T FirstOrDefault(params string[] columns)
        {
            return FirstOrDefault<T>(columns);
        }

        public T FirstOrDefault(params QColumn[] columns)
        {
            return FirstOrDefault<T>(columns);
        }
        #endregion

        #region Select

        public List<T> Select()
        {
            return Select<T>(getSelectColumn());
        }

        public List<T> Select(params string[] columns)
        {
            return Select<T>(columns);
        }

        public List<T> Select(params QColumn[] columns)
        {
            return Select<T>(columns);
        }

        public List<T> Select(int limit)
        {
            return Select<T>(limit, getSelectColumn());
        }

        public List<T> Select(int limit, params string[] columns)
        {
            return Select<T>(limit, columns);
        }

        public List<T> Select(int limit, params QColumn[] columns)
        {
            return Select<T>(limit, columns);
        }

        public List<T> Select(int limit, int offset)
        {
            return Select<T>(limit, offset, getSelectColumn());
        }

        public List<T> Select(int limit, int offset, params string[] columns)
        {
            return Select<T>(limit, offset, columns);
        }

        public List<T> Select(int limit, int offset, params QColumn[] columns)
        {
            return Select<T>(limit, offset, columns);
        }

        public Page<T> Page(int page, int size)
        {
            return Page<T>(page, size, getSelectColumn());
        }

        public Page<T> Page(int page, int size, params string[] columns)
        {
            return Page<T>(page, size, columns);
        }

        public Page<T> Page(int page, int size, params QColumn[] columns)
        {
            return Page<T>(page, size, columns);
        }
        #endregion

        #region ExecuteDataTable
        public DataTable ExecuteDataTable()
        {
            var columns = getSelectColumn();
            return ExecuteDataTable(-1, -1, columns);
        }

        public DataTable ExecuteDataTable(int limit)
        {
            var columns = getSelectColumn();
            return ExecuteDataTable(limit, -1, columns);
        }

        public DataTable ExecuteDataTable(int limit, int offset)
        {
            var columns = getSelectColumn();
            return ExecuteDataTable(limit, offset, columns);
        }

        #endregion

        #region ExecuteDataSet
        public DataSet ExecuteDataSet()
        {
            var columns = getSelectColumn();
            return ExecuteDataSet(-1, -1, columns);
        }

        public DataSet ExecuteDataSet(int limit)
        {
            var columns = getSelectColumn();
            return ExecuteDataSet(limit, -1, columns);
        }

        public DataSet ExecuteDataSet(int limit, int offset)
        {
            var columns = getSelectColumn();
            return ExecuteDataSet(limit, offset, columns);
        }

        #endregion
    }
}
