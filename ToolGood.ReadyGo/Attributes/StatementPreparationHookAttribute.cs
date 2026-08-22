using System;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class StatementPreparationHookAttribute : Attribute
    {
        public abstract IAlterStatementHook AlterStatementHook { get; }
    }
}