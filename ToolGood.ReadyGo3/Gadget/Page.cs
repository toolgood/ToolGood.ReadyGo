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
        private int _currentPage = 0;
        private int _totalItems = 0;
        private int _pageSize = 0;

        /// <summary>
        /// 当前页数
        /// </summary>
        public int CurrentPage { get => _currentPage; set { _currentPage = value; } }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalItems { get => _totalItems; set { _totalItems = value; } }

        /// <summary>
        /// 每一页数量
        /// </summary>
        public int PageSize { get => _pageSize; set { _pageSize = value; } }

        /// <summary>
        /// 用于上下文传输
        /// </summary>
        public object Context { get; set; }

        /// <summary>
        /// 复制 页面数据
        /// </summary>
        /// <param name="page"></param>
        public void CopyFrom(Page page)
        {
            Context = page.Context;
            CurrentPage = page.CurrentPage;
            PageSize = page.PageSize;
            TotalItems = page.TotalItems;
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