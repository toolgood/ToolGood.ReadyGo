namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 提供在插入或更新语句生成后对其进行修改的钩子。
    /// </summary>
    public interface IAlterStatementHook
    {
        /// <summary>
        /// 修改已生成的插入语句。
        /// </summary>
        /// <param name="database">执行插入的数据库实例。</param>
        /// <param name="preparedInsertStatement">已生成的插入语句。</param>
        /// <returns>修改后的插入语句。</returns>
        PreparedInsertStatement AlterInsert(IDatabase database, PreparedInsertStatement preparedInsertStatement);

        /// <summary>
        /// 修改已生成的更新语句。
        /// </summary>
        /// <param name="database">执行更新的数据库实例。</param>
        /// <param name="preparedUpdateStatement">已生成的更新语句。</param>
        /// <returns>修改后的更新语句。</returns>
        PreparedUpdateStatement AlterUpdate(IDatabase database, PreparedUpdateStatement preparedUpdateStatement);
    }

    /// <summary>
    /// 提供对插入/更新语句不进行任何修改的默认钩子实现。
    /// </summary>
    public abstract class AlterStatementHook : IAlterStatementHook
    {
        /// <summary>
        /// 默认原样返回插入语句。
        /// </summary>
        /// <param name="database">执行插入的数据库实例。</param>
        /// <param name="preparedInsertStatement">已生成的插入语句。</param>
        /// <returns>原样的插入语句。</returns>
        public virtual PreparedInsertStatement AlterInsert(IDatabase database, PreparedInsertStatement preparedInsertStatement) => preparedInsertStatement;

        /// <summary>
        /// 默认原样返回更新语句。
        /// </summary>
        /// <param name="database">执行更新的数据库实例。</param>
        /// <param name="preparedUpdateStatement">已生成的更新语句。</param>
        /// <returns>原样的更新语句。</returns>
        public virtual PreparedUpdateStatement AlterUpdate(IDatabase database, PreparedUpdateStatement preparedUpdateStatement) => preparedUpdateStatement;
    }
}