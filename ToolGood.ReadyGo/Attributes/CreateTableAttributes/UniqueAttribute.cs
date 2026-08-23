using System;
using System.Collections.Generic;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 唯一特征
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class UniqueAttribute : Attribute
    {
        /// <summary>
        /// 唯一特征
        /// </summary>
        /// <param name="column">唯一列名</param>
        /// <param name="columns">其它唯一列名</param>
        public UniqueAttribute(string column, params string[] columns)
        {
            ColumnNames = new List<string>();
            ColumnNames.Add(column.Trim());
            foreach (var item in columns) {
                ColumnNames.Add(item.Trim());
            }
        }

        /// <summary>
        /// 列名
        /// </summary>
        public List<string> ColumnNames;
    }
}
