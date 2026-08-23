using System.Collections.Generic;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 保存一次更新操作所需的上下文信息。
    /// </summary>
    public class UpdateContext
    {
        /// <summary>
        /// 初始化 UpdateContext 类的新实例。
        /// </summary>
        /// <param name="poco">要更新的对象。</param>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键名。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <param name="columnsToUpdate">要更新的列集合。</param>
        public UpdateContext(object poco, string tableName, string primaryKeyName, object primaryKeyValue, IEnumerable<string> columnsToUpdate)
        {
            Poco = poco;
            TableName = tableName;
            PrimaryKeyName = primaryKeyName;
            PrimaryKeyValue = primaryKeyValue;
            ColumnsToUpdate = columnsToUpdate;
        }

        /// <summary>
        /// 要更新的对象。
        /// </summary>
        public object Poco { get; private set; }
        /// <summary>
        /// 表名。
        /// </summary>
        public string TableName { get; private set; }
        /// <summary>
        /// 主键名。
        /// </summary>
        public string PrimaryKeyName { get; private set; }
        /// <summary>
        /// 主键值。
        /// </summary>
        public object PrimaryKeyValue { get; private set; }
        /// <summary>
        /// 要更新的列集合。
        /// </summary>
        public IEnumerable<string> ColumnsToUpdate { get; private set; }
    }
}
