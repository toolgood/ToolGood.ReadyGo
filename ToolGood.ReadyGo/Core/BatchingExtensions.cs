using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.NPoco.Extensions
{
    /// <summary>
    /// 提供将序列按指定大小分批的扩展方法。
    /// </summary>
    public static class BatchingExtensions
    {
        /// <summary>
        /// 将序列按指定大小切分为若干批次。
        /// </summary>
        /// <typeparam name="T">序列元素的类型。</typeparam>
        /// <param name="items">要切分的源序列。</param>
        /// <param name="chunkSize">每批包含的元素数量。</param>
        /// <returns>由各批次数组组成的序列。</returns>
        public static IEnumerable<T[]> Chunkify<T>(this IEnumerable<T> items, int chunkSize)
        {
            var enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return Take(enumerator, chunkSize).ToArray();
            }
        }

        private static IEnumerable<T> Take<T>(IEnumerator<T> enumerator, int num)
        {
            do
            {
                yield return enumerator.Current;
            } while (--num > 0 && enumerator.MoveNext());
        }
    }
}
