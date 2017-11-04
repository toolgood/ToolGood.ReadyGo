using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QTable<T>
    {

        private QColumnBase[] getSelectColumn()
        {
            List<QColumnBase> list = new List<QColumnBase>();
            foreach (var item in _columns) {
                var col = item.Value;
                list.Add(col);
            }
            return list.ToArray();
        }

        #region Single SingleOrDefault First FirstOrDefault

        public T GetSingle()
        {
            return Single<T>(getSelectColumn());
        }

        public T GetSingle(params string[] columns)
        {
            return Single<T>(columns);
        }

        public T GetSingle(params QColumnBase[] columns)
        {
            return Single<T>(columns);
        }

        public T GetSingleOrDefault()
        {
            return SingleOrDefault<T>(getSelectColumn());
        }

        public T GetSingleOrDefault(params string[] columns)
        {
            return SingleOrDefault<T>(columns);
        }

        public T GetSingleOrDefault(params QColumnBase[] columns)
        {
            return SingleOrDefault<T>(columns);
        }

        public T GetFirst()
        {
            return First<T>(getSelectColumn());
        }

        public T GetFirst(params string[] columns)
        {
            return First<T>(columns);
        }

        public T GetFirst(params QColumnBase[] columns)
        {
            return First<T>(columns);
        }

        public T GetFirstOrDefault()
        {
            return FirstOrDefault<T>(getSelectColumn());
        }

        public T GetFirstOrDefault(params string[] columns)
        {
            return FirstOrDefault<T>(columns);
        }

        public T GetFirstOrDefault(params QColumnBase[] columns)
        {
            return FirstOrDefault<T>(columns);
        }
        #endregion

        #region Select

        public List<T> GetList()
        {
            return Select<T>(getSelectColumn());
        }

        public List<T> GetList(params string[] columns)
        {
            return Select<T>(columns);
        }

        public List<T> GetList(params QColumnBase[] columns)
        {
            return Select<T>(columns);
        }

        public List<T> GetList(int limit)
        {
            return Select<T>(limit, getSelectColumn());
        }

        public List<T> GetList(int limit, params string[] columns)
        {
            return Select<T>(limit, columns);
        }

        public List<T> GetList(int limit, params QColumnBase[] columns)
        {
            return Select<T>(limit, columns);
        }

        public List<T> GetList(int limit, int offset)
        {
            return Select<T>(limit, offset, getSelectColumn());
        }

        public List<T> GetList(int limit, int offset, params string[] columns)
        {
            return Select<T>(limit, offset, columns);
        }

        public List<T> GetList(int limit, int offset, params QColumnBase[] columns)
        {
            return Select<T>(limit, offset, columns);
        }

        public Page<T> GetPage(int page, int size)
        {
            return GetPage<T>(page, size, getSelectColumn());
        }

        public Page<T> GetPage(int page, int size, params string[] columns)
        {
            return GetPage<T>(page, size, columns);
        }

        public Page<T> GetPage(int page, int size, params QColumnBase[] columns)
        {
            return GetPage<T>(page, size, columns);
        }
        #endregion



        #region GetDataTable
        public DataTable GetDataTable()
        {
            var columns = getSelectColumn();
            return GetDataTable(-1, -1, columns);
        }

        public DataTable GetDataTable(int limit)
        {
            var columns = getSelectColumn();
            return GetDataTable(limit, -1, columns);
        }

        public DataTable GetDataTable(int limit, int offset)
        {
            var columns = getSelectColumn();
            return GetDataTable(limit, offset, columns);
        }

        #endregion

        #region GetDataSet
        public DataSet GetDataSet()
        {
            var columns = getSelectColumn();
            return GetDataSet(-1, -1, columns);
        }

        public DataSet GetDataSet(int limit)
        {
            var columns = getSelectColumn();
            return GetDataSet(limit, -1, columns);
        }

        public DataSet GetDataSet(int limit, int offset)
        {
            var columns = getSelectColumn();
            return GetDataSet(limit, offset, columns);
        }

        #endregion
    }
}
