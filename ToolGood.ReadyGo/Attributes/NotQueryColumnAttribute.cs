using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 查询时 不返回的列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotQueryColumnAttribute: ColumnAttribute
    {

    }
}
