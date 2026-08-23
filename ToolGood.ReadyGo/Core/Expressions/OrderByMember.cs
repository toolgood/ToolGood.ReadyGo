using System;

namespace ToolGood.ReadyGo.NPoco.Expressions
{
    /// <summary>
    /// 表示排序成员信息，用于记录排序所涉及的实体类型、列以及升序/降序方向。
    /// </summary>
    public class OrderByMember
    {
        /// <summary>
        /// 成员所属的实体类型。
        /// </summary>
        public Type EntityType { get; set; }
        /// <summary>
        /// 排序对应的列信息。
        /// </summary>
        public PocoColumn PocoColumn { get; set; }
        /// <summary>
        /// 排序关联的列信息数组。
        /// </summary>
        public PocoColumn[] PocoColumns { get; set; }
        /// <summary>
        /// 排序方向（ASC 或 DESC）。
        /// </summary>
        public string AscDesc { get; set; }
    }
}
