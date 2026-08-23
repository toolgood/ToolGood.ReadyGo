using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 结果列标签：用于标记仅用于接收查询结果、不参与写入的列。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ResultColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 结果列标签
        /// </summary>
        public ResultColumnAttribute() { }

        /// <summary>
        /// 结果列标签
        /// </summary>
        /// <param name="name">列名</param>
        public ResultColumnAttribute(string name) : base(name) { }
    }
}