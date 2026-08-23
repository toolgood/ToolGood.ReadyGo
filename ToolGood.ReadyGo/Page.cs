using System.Collections.Generic;

namespace ToolGood.ReadyGo
{
    public class Page<T> 
    {
        public long CurrentPage { get; set; }
        public long TotalPages { get; set; }
        public long TotalItems { get; set; }
        public long PageSize { get; set; }
        public List<T> Items { get; set; }
    }
}