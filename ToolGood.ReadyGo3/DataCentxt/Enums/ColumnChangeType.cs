using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt.Enums
{
    /// <summary>
    /// 列修改状态
    /// </summary>
    public enum ColumnChangeType
    {
        /// <summary>
        /// 未修改
        /// </summary>
        None,
        /// <summary>
        /// 使用新值
        /// </summary>
        NewValue,
        /// <summary>
        /// 新SQL
        /// </summary>
        NewSql
    }
}
