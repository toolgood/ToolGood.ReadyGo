using System;
using System.Collections.Generic;
using System.Linq;

namespace ToolGood.ReadyGo.NPoco.RowMappers
{
    /// <summary>
    /// 提供按字符串分隔符对元素进行多级分组分层的扩展方法。
    /// </summary>
    public static class MyEnumerableExtensions
    {
        /// <summary>
        /// 按字符串函数的结果以指定分隔符进行多级分组，返回分组结果集合。
        /// </summary>
        /// <typeparam name="TKey">元素类型。</typeparam>
        /// <param name="elements">待分组的元素集合。</param>
        /// <param name="stringFunc">从元素中提取用于分组的字符串的函数。</param>
        /// <param name="splitBy">分隔符。</param>
        /// <param name="i">当前分组层级（从 0 开始）。</param>
        /// <returns>分组结果的集合，包含各级子分组。</returns>
        public static IEnumerable<GroupResult<TKey>> GroupByMany<TKey>(this IEnumerable<TKey> elements, Func<TKey, string> stringFunc, string splitBy, int i = 0)
        {
            return elements
                .Select(x => new { Item = x, Parts = stringFunc(x).Split(new[] { splitBy }, StringSplitOptions.RemoveEmptyEntries) })
                .GroupBy(x => x.Parts.Skip(i).FirstOrDefault())
                .Where(x => x.Key != null)
                .Select(g => new GroupResult<TKey>
                {
                    Item = g.Key,
                    Key = g.Select(x => x.Item).First(),
                    Count = g.Count(),
                    SubItems = g.Select(x => x.Item).GroupByMany(stringFunc, splitBy, i + 1).ToList()
                });
        }
    }
}