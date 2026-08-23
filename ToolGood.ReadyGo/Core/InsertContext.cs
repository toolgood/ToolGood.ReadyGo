namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 描述一次插入操作的上下文，包含待插入对象、表名、主键名以及主键是否自增。
    /// </summary>
    public class InsertContext
    {
        /// <summary>
        /// 初始化 <see cref="InsertContext"/> 实例。
        /// </summary>
        /// <param name="poco">待插入的 POCO 对象。</param>
        /// <param name="tableName">表名。</param>
        /// <param name="autoIncrement">主键是否自增。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        public InsertContext(object poco, string tableName, bool autoIncrement, string primaryKeyName)
        {
            Poco = poco;
            TableName = tableName;
            AutoIncrement = autoIncrement;
            PrimaryKeyName = primaryKeyName;
        }

        /// <summary>
        /// 获取待插入的 POCO 对象。
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
        /// 获取主键是否自增。
        /// </summary>
        public bool AutoIncrement { get; private set; }
    }
}
