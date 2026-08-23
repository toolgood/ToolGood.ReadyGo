using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco.Expressions
{

    /// <summary>
    /// 表示查询中涉及的通用成员信息，用于记录实体类型及其对应的数据列。
    /// </summary>
    public class GeneralMember
    {
        /// <summary>
        /// 成员所属的实体类型。
        /// </summary>
        public Type EntityType { get; set; }
        /// <summary>
        /// 成员对应的列信息。
        /// </summary>
        public PocoColumn PocoColumn { get; set; }
        /// <summary>
        /// 成员关联的列信息数组（用于成员访问链）。
        /// </summary>
        public PocoColumn[] PocoColumns { get; set; }
    }
}
