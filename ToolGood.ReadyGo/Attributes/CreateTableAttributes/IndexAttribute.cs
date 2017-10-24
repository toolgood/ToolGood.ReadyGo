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
        /// <param name="column"></param>
        /// <param name="columns"></param>
        public IndexAttribute(string column, params string[] columns)
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