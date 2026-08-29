using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 表特征
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : TableNameAttribute
    {
        /// <summary>
        /// Schema名
        /// </summary>
        public string SchemaName { get; }

        /// <summary>
        /// 数据表
        /// </summary>
        /// <param name="tableName">数据表名</param>
        public TableAttribute(string tableName) : base(tableName)
        {
        }

        /// <summary>
        /// 数据表
        /// </summary>
        /// <param name="tableName">数据表名</param>
        /// <param name="schemaName">Schema 名</param>
        public TableAttribute(string tableName, string schemaName) : base(tableName)
        {
            if (schemaName == null) {
                throw new ArgumentNullException(nameof(schemaName));
            }
            SchemaName = schemaName.Trim();
        }
    }
}
