using ToolGood.ReadyGo.Attributes;

namespace ToolGood.ReadyGo.NPoco.DbSpecific.Postgresql
{
    public class OnConflictDoNothingAttribute : StatementPreparationHookAttribute
    {
        public override IAlterStatementHook AlterStatementHook => new OnConflictDoNothingStatementHook();
    }
}
