using System;
using System.Collections.Generic;
using System.Text;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 批量插入操作的配置选项。
    /// </summary>
    public class InsertBulkOptions
    {
        /// <summary>
        /// 获取或设置批量复制操作的超时时间（秒）。
        /// </summary>
        public int? BulkCopyTimeout { get; set; }
    }
}
