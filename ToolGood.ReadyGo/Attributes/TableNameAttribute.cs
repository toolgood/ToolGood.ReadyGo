using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 表名标签：用于指定类映射到的数据表名。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// 表名标签
        /// </summary>
        /// <param name="tableName">数据表名</param>
        public TableNameAttribute(string tableName)
        {
            if (tableName == null) {
                throw new ArgumentNullException(nameof(tableName));
            }
            TableName = tableName;
        }

        /// <summary>
        /// 数据表名
        /// </summary>
        public string TableName { get; private set; }
    }
}