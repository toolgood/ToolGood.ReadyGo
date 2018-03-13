using System;
using System.Collections.Generic;

#if !NETSTANDARD2_0 && !NET40
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo3.Gadget.Caches
{
    /// <summary>
    /// 
    /// </summary>
     partial class MemoryCacheService : ICacheService
    {
        public virtual async Task<T> GetAsync<T>(string name, Func<Task<T>> func, int expiredSecond, string regionName)
        {
            var cache = GetMemoryCache();
            if (cache.Contains(name)) return (T)cache.Get(name);
            expiredSecond = Math.Max(1, expiredSecond);

            var t = await func();
            if (t != null) {
                cache.Set(name, t, DateTimeOffset.Now.AddSeconds(expiredSecond));
            }
            if (regionName != null) {
                var tagCache = GetTagCache();
                HashSet<string> list;
                if (tagCache.Contains("regionName")) {
                    list = (HashSet<string>)cache.Get(regionName);
                } else {
                    list = new HashSet<string>();
                }
                list.Add(name);
                tagCache.Set(regionName, list, DateTimeOffset.Now.AddSeconds(expiredSecond));
            }

            return t;
        }


         
        
    }
}
#endif