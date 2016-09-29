using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 表特征
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Schema名
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        /// 数据表名标记
        /// </summary>
        public string FixTag { get; private set; }

        /// <summary>
        /// 数据表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fixTag"></param>
        public TableAttribute(string tableName, string fixTag = "")
        {
            TableName = tableName;
            FixTag = fixTag;
        }
        /// <summary>
        /// 数据表
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <param name="fixTag"></param>
        public TableAttribute(string schemaName, string tableName, string fixTag)
        {
            SchemaName = schemaName;
            TableName = tableName;
            FixTag = fixTag;
        }
    }
}