using System;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 计算列标签：用于标记由数据库计算生成、不需要显式写入的列。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ComputedColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 计算列类型，默认为 Always。
        /// </summary>
        public ComputedColumnType ComputedColumnType = ComputedColumnType.Always;

        /// <summary>
        /// 计算列标签
        /// </summary>
        public ComputedColumnAttribute() { }

        /// <summary>
        /// 计算列标签
        /// </summary>
        /// <param name="name">列名</param>
        public ComputedColumnAttribute(string name) : base(name) { }

        /// <summary>
        /// 计算列标签
        /// </summary>
        /// <param name="computedColumnType">计算列类型</param>
        public ComputedColumnAttribute(ComputedColumnType computedColumnType)
        {
            ComputedColumnType = computedColumnType;
        }

        /// <summary>
        /// 计算列标签
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="computedColumnType">计算列类型</param>
        public ComputedColumnAttribute(string name, ComputedColumnType computedColumnType) : base(name)
        {
            ComputedColumnType = computedColumnType;
        }
    }
}