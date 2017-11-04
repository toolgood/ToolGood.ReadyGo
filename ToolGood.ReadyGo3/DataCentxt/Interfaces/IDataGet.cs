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

        int SelectCount();
        int SelectCount(string distinctColumn);
        int SelectCount(QColumnBase distinctColumn);

        T Single<T>();
        T Single<T>(params string[] columns);
        T Single<T>(params QColumnBase[] columns);

        T SingleOrDefault<T>();
        T SingleOrDefault<T>(params string[] columns);
        T SingleOrDefault<T>(params QColumnBase[] columns);

        T First<T>();
        T First<T>(params string[] columns);
        T First<T>(params QColumnBase[] columns);

        T FirstOrDefault<T>();
        T FirstOrDefault<T>(params string[] columns);
        T FirstOrDefault<T>(params QColumnBase[] columns);

        List<T> Select<T>();
        List<T> Select<T>(params string[] columns);
        List<T> Select<T>(params QColumnBase[] columns);
   
        List<T> Select<T>(int limit);
        List<T> Select<T>(int limit, params string[] columns);
        List<T> Select<T>(int limit, params QColumnBase[] columns);

        List<T> Select<T>(int limit, int offset);
        List<T> Select<T>(int limit, int offset, params string[] columns);
        List<T> Select<T>(int limit, int offset, params QColumnBase[] columns);

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
