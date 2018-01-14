using System;
using System.Collections.Generic;

namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// 页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Page<T> //: Page
    {
        private long _PageStart = -1;
        private long _PageEnd = -1;

        /// <summary>
        /// 列表
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// 当前页数
        /// </summary>
        public long CurrentPage { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPages { get
            {
                return (long)Math.Ceiling(TotalItems / (double)PageSize);
            }

        }

        /// <summary>
        /// 总数
        /// </summary>
        public long TotalItems { get; set; }

        /// <summary>
        /// 每一页数量
        /// </summary>
        public long PageSize { get; set; }

        /// <summary>
        /// 从1开始
        /// </summary>
        public long PageStart
        {
            get
            {
                if (_PageStart == -1) {
                    SetShowPage(7);
                }
                return _PageStart;
            }
        }

        /// <summary>
        /// 结束 包含TotalPages
        /// </summary>
        public long PageEnd
        {
            get
            {
                if (_PageEnd == -1) {
                    SetShowPage(7);
                }
                return _PageEnd;
            }
        }

        /// <summary>
        /// 用于上下文传输
        /// </summary>
        public object Context { get; set; }

        /// <summary>
        /// 设置显示页
        /// </summary>
        /// <param name="size"></param>
        public void SetShowPage(int size)
        {
            int mod2 = size / 2;
            var end = Math.Min(TotalPages, CurrentPage + mod2);
            var start = Math.Max(1, end - size);
            end = Math.Min(TotalPages, start + size);
            _PageStart = start;
            _PageEnd = end;
        }
    }
}