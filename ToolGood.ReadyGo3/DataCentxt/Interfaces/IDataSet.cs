using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt.Interfaces
{
    /// <summary>
    /// 数据操作，插入、修改、删除 
    /// </summary>
    public interface IDataSet
    {
        int Update();
        int Delete();
        object Insert(bool returnInsertId = true);
    }

    public interface IDataSet<T>: IDataSet
    {
        T SetValues(Dictionary<string, object> values);

        T SetValue(string name, object value);
    }

}
