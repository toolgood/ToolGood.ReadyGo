using System;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 版本列标签：用于标记乐观并发控制所使用的版本列。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class VersionColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 版本列类型
        /// </summary>
        public VersionColumnType VersionColumnType { get; private set; }

        /// <summary>
        /// 版本列标签（默认使用数值型版本）
        /// </summary>
        public VersionColumnAttribute() : this(VersionColumnType.Number) {}

        /// <summary>
        /// 版本列标签
        /// </summary>
        /// <param name="versionColumnType">版本列类型</param>
        public VersionColumnAttribute(VersionColumnType versionColumnType)
        {
            VersionColumnType = versionColumnType;
        }

        /// <summary>
        /// 版本列标签
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="versionColumnType">版本列类型</param>
        public VersionColumnAttribute(string name, VersionColumnType versionColumnType) : base(name)
        {
            VersionColumnType = versionColumnType;
        }
    }
}