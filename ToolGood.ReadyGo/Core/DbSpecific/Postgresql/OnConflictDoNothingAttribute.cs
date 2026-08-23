using ToolGood.ReadyGo.Attributes;

namespace ToolGood.ReadyGo.NPoco.DbSpecific.Postgresql
{
    /// <summary>
    /// 用于在 PostgreSQL 插入语句上追加 ON CONFLICT DO NOTHING 的语句钩子特性。
    /// </summary>
    public class OnConflictDoNothingAttribute : StatementPreparationHookAttribute
    {
        /// <summary>
        /// 获取对应的语句修改钩子。
        /// </summary>
        public override IAlterStatementHook AlterStatementHook => new OnConflictDoNothingStatementHook();
    }
}
