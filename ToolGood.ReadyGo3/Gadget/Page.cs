using System;
using System.Collections.Generic;

namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// 页
    /// </summary>
    [Serializable]
    public abstract class Page
    {
        private bool _hasComplete;
        private long _pageStart = -1;
        private long _pageEnd = -1;
        private long _currentPage = 0;
        private long _totalItems = 0;
        private long _pageSize = 0;

        /// <summary>
        /// 当前页数
        /// </summary>
        public long CurrentPage { get => _currentPage; set { _hasComplete = false; _currentPage = value; } }

        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPages => (long)Math.Ceiling(TotalItems / (double)PageSize);

        /// <summary>
        /// 总数
        /// </summary>
        public long TotalItems { get => _totalItems; set { _hasComplete = false; _totalItems = value; } }

        /// <summary>
        /// 每一页数量
        /// </summary>
        public long PageSize { get => _pageSize; set { _hasComplete = false; _pageSize = value; } }

        /// <summary>
        /// 从1开始
        /// </summary>
        public long PageStart { get { if (_hasComplete == false) { SetShowPage(7); } return _pageStart; } }

        /// <summary>
        /// 结束 包含TotalPages
        /// </summary>
        public long PageEnd { get { if (_hasComplete == false) { SetShowPage(7); } return _pageEnd; } }

        /// <summary>
        /// 用于上下文传输
        /// </summary>
        public object Context { get; set; }

        /// <summary>
        /// 设置显示页
        /// </summary>
        /// <param name="tagSize"></param>
        public void SetShowPage(int tagSize)
        {
            int mod2 = tagSize / 2;
            var end = Math.Min(TotalPages, CurrentPage + mod2);
            var start = Math.Max(1, end - tagSize);
            end = Math.Min(TotalPages, start + tagSize);
            _pageStart = start;
            _pageEnd = end;
            _hasComplete = true;
        }
    }
    /// <summary>
    /// 页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Page<T> : Page
    {
        /// <summary>
        /// 列表
        /// </summary>
        public List<T> Items { get; set; }
    }
}