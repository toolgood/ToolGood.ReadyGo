namespace ToolGood.ReadyGo.NPoco.DbSpecific.Postgresql
{
    /// <summary>
    /// 修改插入语句，为其追加 ON CONFLICT DO NOTHING 子句的语句钩子。
    /// </summary>
    public class OnConflictDoNothingStatementHook : AlterStatementHook
    {
        /// <summary>
        /// 修改已准备的插入语句，追加 ON CONFLICT DO NOTHING。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="preparedInsertStatement">已准备的插入语句。</param>
        /// <returns>修改后的插入语句。</returns>
        public override PreparedInsertStatement AlterInsert(IDatabase database, PreparedInsertStatement preparedInsertStatement)
        {
            preparedInsertStatement.Sql += " ON CONFLICT DO NOTHING";
            return preparedInsertStatement;
        }
    }
}