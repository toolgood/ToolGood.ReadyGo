using System;
using System.Collections.Generic;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 索引特征
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class IndexAttribute : Attribute
    {
        /// <summary>
        /// 索引特征
        /// </summary>
        /// <param name="column">索引列名</param>
        /// <param name="columns">其它索引列名</param>
        public IndexAttribute(string column, params string[] columns)
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
