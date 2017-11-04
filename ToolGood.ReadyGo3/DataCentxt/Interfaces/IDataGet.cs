using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt.Interfaces
{
    /// <summary>
    /// 数据获取接口
    /// </summary>
    /// <typeparam name="Table"></typeparam>
    public interface IDataGet<Table>
    {
        Table Distinct();

        int GetCount();
        int GetCount(string distinctColumn);
        int GetCount(QColumnBase distinctColumn);

        T GetSingle<T>();
        T GetSingle<T>(params string[] columns);
        T GetSingle<T>(params QColumnBase[] columns);

        T GetSingleOrDefault<T>();
        T GetSingleOrDefault<T>(params string[] columns);
        T GetSingleOrDefault<T>(params QColumnBase[] columns);

        T GetFirst<T>();
        T GetFirst<T>(params string[] columns);
        T GetFirst<T>(params QColumnBase[] columns);

        T GetFirstOrDefault<T>();
        T GetFirstOrDefault<T>(params string[] columns);
        T GetFirstOrDefault<T>(params QColumnBase[] columns);

        List<T> GetList<T>();
        List<T> GetList<T>(params string[] columns);
        List<T> GetList<T>(params QColumnBase[] columns);
   
        List<T> GetList<T>(int limit);
        List<T> GetList<T>(int limit, params string[] columns);
        List<T> GetList<T>(int limit, params QColumnBase[] columns);

        List<T> GetList<T>(int limit, int offset);
        List<T> GetList<T>(int limit, int offset, params string[] columns);
        List<T> GetList<T>(int limit, int offset, params QColumnBase[] columns);

        Page<T> GetPage<T>(int page, int size);
        Page<T> GetPage<T>(int page, int size, params string[] columns);
        Page<T> GetPage<T>(int page, int size, params QColumnBase[] columns);


        DataTable GetDataTable();
        DataTable GetDataTable(params string[] columns);
        DataTable GetDataTable(params QColumnBase[] columns);

        DataTable GetDataTable(int limit);
        DataTable GetDataTable(int limit, params string[] columns);
        DataTable GetDataTable(int limit, params QColumnBase[] columns);

        DataTable GetDataTable(int limit, int offset);
        DataTable GetDataTable(int limit, int offset, params string[] columns);
        DataTable GetDataTable(int limit, int offset, params QColumnBase[] columns);


        DataSet GetDataSet();
        DataSet GetDataSet(params string[] columns);
        DataSet GetDataSet(params QColumnBase[] columns);

        DataSet GetDataSet(int limit);
        DataSet GetDataSet(int limit, params string[] columns);
        DataSet GetDataSet(int limit, params QColumnBase[] columns);

        DataSet GetDataSet(int limit, int offset);
        DataSet GetDataSet(int limit, int offset, params string[] columns);
        DataSet GetDataSet(int limit, int offset, params QColumnBase[] columns);

    }
}
