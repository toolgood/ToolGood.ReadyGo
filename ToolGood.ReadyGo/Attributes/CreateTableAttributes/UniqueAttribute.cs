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
        /// <param name="column"></param>
        /// <param name="columns"></param>
        public UniqueAttribute(string column, params string[] columns)
        {
            ColumnNames = new List<string>();
            ColumnNames.Add(column);
            ColumnNames.AddRange(columns);
        }
        /// <summary>
        /// 列名
        /// </summary>
        public List<string> ColumnNames { get; private set; }
    }
}