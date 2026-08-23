using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示一批待更新对象及其快照。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    public class UpdateBatch<T>
    {
        /// <summary>
        /// 要更新的对象。
        /// </summary>
        public T Poco { get; set; }
        /// <summary>
        /// 对象的快照（可为空）。
        /// </summary>
        public Snapshot<T> Snapshot { get; set; }
    }

    /// <summary>
    /// 提供批量更新对象的辅助方法。
    /// </summary>
    public class UpdateBatch
    {
        /// <summary>
        /// 创建包含指定对象及其快照的更新批次。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="poco">要更新的对象。</param>
        /// <param name="snapshot">对象的快照。</param>
        /// <returns>更新批次实例。</returns>
        public static UpdateBatch<T> For<T>(T poco, Snapshot<T> snapshot = null)
        {
            return new UpdateBatch<T> { Poco = poco, Snapshot = snapshot };
        }
    }
}
