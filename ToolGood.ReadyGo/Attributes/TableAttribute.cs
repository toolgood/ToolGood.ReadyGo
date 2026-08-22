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
        /// 数据表名
        /// </summary>
        public string TableName;

        /// <summary>
        /// Schema名
        /// </summary>
        public string SchemaName;

        /// <summary>
        /// 数据库名
        /// </summary>
        public string DatabaseName;

        /// <summary>
        /// 配置名
        /// </summary>
        public string SettingName;

        /// <summary>
        /// 数据表
        /// </summary>
        /// <param name="tableName"></param>
        public TableAttribute(string tableName) : base(tableName)
        {
            TableName = tableName.Trim();
        }

        /// <summary>
        /// 数据表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schemaName"></param>
        public TableAttribute(string tableName, string schemaName) : base(tableName)
        {
            SchemaName = schemaName.Trim();
            TableName = tableName.Trim();
        }

        /// <summary>
        /// 数据表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schemaName"></param>
        /// <param name="databaseName"></param>
        public TableAttribute(string tableName, string schemaName, string databaseName) : base(tableName)
        {
            SchemaName = schemaName.Trim();
            TableName = tableName.Trim();
            DatabaseName = databaseName.Trim();
        }
    }
}
