using System;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 语句准备钩子标签：用于在生成建表语句时注入自定义修改逻辑。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class StatementPreparationHookAttribute : Attribute
    {
        /// <summary>
        /// 语句修改钩子
        /// </summary>
        public abstract IAlterStatementHook AlterStatementHook { get; }
    }
}