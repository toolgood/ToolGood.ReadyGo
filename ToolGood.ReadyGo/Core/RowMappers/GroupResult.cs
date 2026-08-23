using System.Collections.Generic;

namespace ToolGood.ReadyGo.NPoco.RowMappers
{
    /// <summary>
    /// 表示按名称分组后的结果，用于将查询列按 POCO 层级进行分组。
    /// </summary>
    /// <typeparam name="TKey">分组键的类型。</typeparam>
    public class GroupResult<TKey>
    {
        /// <summary>
        /// 当前分组的键。
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// 当前分组的名称。
        /// </summary>
        public string Item { get; set; }

        /// <summary>
        /// 当前分组中的元素数量。
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 当前分组下的子分组集合。
        /// </summary>
        public IEnumerable<GroupResult<TKey>> SubItems { get; set; }

        /// <summary>
        /// 返回形如“名称 (数量)”的字符串表示。
        /// </summary>
        /// <returns>格式化的字符串。</returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", Item, Count);
        }
    }
}