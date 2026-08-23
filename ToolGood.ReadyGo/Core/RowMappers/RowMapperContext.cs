using System;

namespace ToolGood.ReadyGo.NPoco.RowMappers
{
    /// <summary>
    /// 行映射上下文，携带当前映射的目标实例及其 POCO 元数据。
    /// </summary>
    public struct RowMapperContext
    {
        /// <summary>
        /// 当前映射的目标实例。
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// 目标类型的 POCO 元数据。
        /// </summary>
        public PocoData PocoData { get; set; }

        /// <summary>
        /// 目标类型。
        /// </summary>
        public Type Type { get { return PocoData.Type; } }
    }
}