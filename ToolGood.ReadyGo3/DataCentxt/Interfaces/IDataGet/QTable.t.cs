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

        #region GetSingle GetSingleOrDefault GetFirst GetFirstOrDefault

        public T GetSingle()
        {
            return GetSingle<T>(getSelectColumn());
        }

        public T GetSingle(params string[] columns)
        {
            return GetSingle<T>(columns);
        }

        public T GetSingle(params QColumnBase[] columns)
        {
            return GetSingle<T>(columns);
        }

        public T GetSingleOrDefault()
        {
            return GetSingleOrDefault<T>(getSelectColumn());
        }

        public T GetSingleOrDefault(params string[] columns)
        {
            return GetSingleOrDefault<T>(columns);
        }

        public T GetSingleOrDefault(params QColumnBase[] columns)
        {
            return GetSingleOrDefault<T>(columns);
        }

        public T GetFirst()
        {
            return GetFirst<T>(getSelectColumn());
        }

        public T GetFirst(params string[] columns)
        {
            return GetFirst<T>(columns);
        }

        public T GetFirst(params QColumnBase[] columns)
        {
            return GetFirst<T>(columns);
        }

        public T GetFirstOrDefault()
        {
            return GetFirstOrDefault<T>(getSelectColumn());
        }

        public T GetFirstOrDefault(params string[] columns)
        {
            return GetFirstOrDefault<T>(columns);
        }

        public T GetFirstOrDefault(params QColumnBase[] columns)
        {
            return GetFirstOrDefault<T>(columns);
        }
        #endregion

        #region GetList

        public List<T> GetList()
        {
            return GetList<T>(getSelectColumn());
        }

        public List<T> GetList(params string[] columns)
        {
            return GetList<T>(columns);
        }

        public List<T> GetList(params QColumnBase[] columns)
        {
            return GetList<T>(columns);
        }

        public List<T> GetList(int limit)
        {
            return GetList<T>(limit, getSelectColumn());
        }

        public List<T> GetList(int limit, params string[] columns)
        {
            return GetList<T>(limit, columns);
        }

        public List<T> GetList(int limit, params QColumnBase[] columns)
        {
            return GetList<T>(limit, columns);
        }

        public List<T> GetList(int limit, int offset)
        {
            return GetList<T>(limit, offset, getSelectColumn());
        }

        public List<T> GetList(int limit, int offset, params string[] columns)
        {
            return GetList<T>(limit, offset, columns);
        }

        public List<T> GetList(int limit, int offset, params QColumnBase[] columns)
        {
            return GetList<T>(limit, offset, columns);
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
