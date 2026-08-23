using System.Collections.Generic;

namespace ToolGood.ReadyGo
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T">数据项类型</typeparam>
    public class Page<T> 
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public long CurrentPage { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPages { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public long TotalItems { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public long PageSize { get; set; }

        /// <summary>
        /// 当前页数据
        /// </summary>
        public List<T> Items { get; set; }
    }
}