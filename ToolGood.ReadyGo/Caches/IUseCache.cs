using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.Caches
{
    public interface IUseCache
    {
        /// <summary>
        /// 使用缓存
        /// </summary>
        /// <param name="second"></param>
        /// <param name="cacheTag"></param>
        /// <param name="cacheService"></param>
        void useCache(int second, string cacheTag = null, ICacheService cacheService = null);
    }
}
