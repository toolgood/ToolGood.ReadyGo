namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 描述一次删除操作的上下文，包含待删除对象、表名、主键名与主键值。
    /// </summary>
    public class DeleteContext
    {
        /// <summary>
        /// 初始化 <see cref="DeleteContext"/> 实例。
        /// </summary>
        /// <param name="poco">待删除的 POCO 对象，可能为 null。</param>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        public DeleteContext(object poco, string tableName, string primaryKeyName, object primaryKeyValue)
        {
            Poco = poco;
            TableName = tableName;
            PrimaryKeyName = primaryKeyName;
            PrimaryKeyValue = primaryKeyValue;
        }

        /// <summary>
        /// 获取待删除的 POCO 对象。
        /// </summary>
        public object Poco { get; private set; }

        /// <summary>
        /// 获取表名。
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// 获取主键列名。
        /// </summary>
        public string PrimaryKeyName { get; private set; }

        /// <summary>
        /// 获取主键值。
        /// </summary>
        public object PrimaryKeyValue { get; private set; }
    }
}