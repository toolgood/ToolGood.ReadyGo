using System.Collections.Generic;
using System.Data;

namespace ToolGood.ReadyGo3.DataCentxt
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
        /// <summary>
        /// 获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <returns></returns>
        public T Single()
        {
            return Single<T>(getSelectColumn());
        }
        /// <summary>
        /// 获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public T Single(params string[] columns)
        {
            return Single<T>(columns);
        }
        /// <summary>
        /// 获取唯一一个类型，若数量不为1，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public T Single(params QColumn[] columns)
        {
            return Single<T>(columns);
        }
        /// <summary>
        /// 获取唯一一个类型，可为空，若数量大于1，则抛出异常
        /// </summary>
        /// <returns></returns>
        public T SingleOrDefault()
        {
            return SingleOrDefault<T>(getSelectColumn());
        }
        /// <summary>
        /// 获取唯一一个类型，可为空，若数量大于1，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public T SingleOrDefault(params string[] columns)
        {
            return SingleOrDefault<T>(columns);
        }
        /// <summary>
        /// 获取唯一一个类型，可为空，若数量大于1，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public T SingleOrDefault(params QColumn[] columns)
        {
            return SingleOrDefault<T>(columns);
        }
        /// <summary>
        /// 获取第一个类型，若数量为0，则抛出异常
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            return First<T>(getSelectColumn());
        }
        /// <summary>
        /// 获取第一个类型，若数量为0，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public T First(params string[] columns)
        {
            return First<T>(columns);
        }
        /// <summary>
        /// 获取第一个类型，若数量为0，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public T First(params QColumn[] columns)
        {
            return First<T>(columns);
        }
        /// <summary>
        /// 获取第一个类型，可为空，若数量为0，则抛出异常
        /// </summary>
        /// <returns></returns>
        public T FirstOrDefault()
        {
            return FirstOrDefault<T>(getSelectColumn());
        }
        /// <summary>
        /// 获取第一个类型，可为空，若数量为0，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public T FirstOrDefault(params string[] columns)
        {
            return FirstOrDefault<T>(columns);
        }
        /// <summary>
        /// 获取第一个类型，可为空，若数量为0，则抛出异常
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public T FirstOrDefault(params QColumn[] columns)
        {
            return FirstOrDefault<T>(columns);
        }
        #endregion

        #region Select
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <returns></returns>
        public List<T> Select()
        {
            return Select<T>(getSelectColumn());
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public List<T> Select(params string[] columns)
        {
            return Select<T>(columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public List<T> Select(params QColumn[] columns)
        {
            return Select<T>(columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<T> Select(int limit)
        {
            return Select<T>(limit, getSelectColumn());
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public List<T> Select(int limit, params string[] columns)
        {
            return Select<T>(limit, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public List<T> Select(int limit, params QColumn[] columns)
        {
            return Select<T>(limit, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public List<T> Select(int limit, int offset)
        {
            return Select<T>(limit, offset, getSelectColumn());
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public List<T> Select(int limit, int offset, params string[] columns)
        {
            return Select<T>(limit, offset, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public List<T> Select(int limit, int offset, params QColumn[] columns)
        {
            return Select<T>(limit, offset, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Page<T> Page(int page, int size)
        {
            return Page<T>(page, size, getSelectColumn());
        }
        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Page<T> Page(int page, int size, params string[] columns)
        {
            return Page<T>(page, size, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回Page类型
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Page<T> Page(int page, int size, params QColumn[] columns)
        {
            return Page<T>(page, size, columns);
        }
        #endregion

        #region ExecuteDataTable
        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ExecuteDataTable()
        {
            var columns = getSelectColumn();
            return ExecuteDataTable(-1, -1, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(int limit)
        {
            var columns = getSelectColumn();
            return ExecuteDataTable(limit, -1, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回 DataTable
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(int limit, int offset)
        {
            var columns = getSelectColumn();
            return ExecuteDataTable(limit, offset, columns);
        }

        #endregion

        #region ExecuteDataSet
#if !NETSTANDARD2_0
        /// <summary>
        /// 执行SQL 查询,返回 DataSet
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteDataSet()
        {
            var columns = getSelectColumn();
            return ExecuteDataSet(-1, -1, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回 DataSet
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(int limit)
        {
            var columns = getSelectColumn();
            return ExecuteDataSet(limit, -1, columns);
        }
        /// <summary>
        /// 执行SQL 查询,返回 DataSet
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(int limit, int offset)
        {
            var columns = getSelectColumn();
            return ExecuteDataSet(limit, offset, columns);
        }
#endif
        #endregion
    }
}
