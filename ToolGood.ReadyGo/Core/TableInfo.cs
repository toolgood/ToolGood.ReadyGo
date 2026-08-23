using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 保存实体类型对应数据表的结构信息（表名、主键、自增、序列、别名与语句钩子等）。
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// 表名。
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 主键列名（多个主键以逗号分隔）。
        /// </summary>
        public string PrimaryKey { get; set; }
        /// <summary>
        /// 指示主键是否自增。
        /// </summary>
        public bool AutoIncrement { get; set; }
        /// <summary>
        /// 序列名称。
        /// </summary>
        public string SequenceName { get; set; }
        /// <summary>
        /// 自动生成的表别名。
        /// </summary>
        public string AutoAlias { get; set; }
        /// <summary>
        /// 指示插入时是否使用 OUTPUT 子句返回主键。
        /// </summary>
        public bool UseOutputClause { get; set; }
        /// <summary>
        /// 实际持久化的类型。
        /// </summary>
        public Type PersistedType { get; set; }
        /// <summary>
        /// 用于修改生成语句的钩子列表。
        /// </summary>
        public List<IAlterStatementHook> AlterStatementHooks { get; set; } = new();

        /// <summary>
        /// 创建当前表信息的副本。
        /// </summary>
        /// <returns>表信息副本。</returns>
        public TableInfo Clone()
        {
            return new TableInfo
            {
                AutoAlias = AutoAlias,
                AutoIncrement = AutoIncrement,
                TableName = TableName,
                PrimaryKey = PrimaryKey,
                SequenceName = SequenceName,
                UseOutputClause = UseOutputClause,
                PersistedType = PersistedType,
				AlterStatementHooks = AlterStatementHooks,
            };
        }
    }
}