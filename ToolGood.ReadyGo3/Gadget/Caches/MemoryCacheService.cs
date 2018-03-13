using System;
using System.Collections.Generic;

#if !NETSTANDARD2_0
using System.Runtime.Caching;

namespace ToolGood.ReadyGo3.Gadget.Caches
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MemoryCacheService : ICacheService
    {
        private static MemoryCache _cacheObject;
        private static MemoryCache _TagObject;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected MemoryCache GetMemoryCache()
        {
            if (_cacheObject == null) {
                _cacheObject = new MemoryCache("ToolGood.ReadyGo.MemoryCacheService");
            }
            return _cacheObject;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        protected MemoryCache GetTagCache()
        {
            if (_TagObject == null) {
                _TagObject = new MemoryCache("ToolGood.ReadyGo.MemoryCacheService.Tag");
            }
            return _TagObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void Clear(string name)
        {
            var tagCache = GetTagCache();
            if (tagCache.Contains(name)) {
                var list = (HashSet<string>)tagCache.Get(name);
                var cache = GetMemoryCache();
                foreach (var item in list) {
                    cache.Remove(item);
                }
                tagCache.Remove(name);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="func"></param>
        /// <param name="expiredSecond"></param>
        /// <param name="regionName"></param>
        /// <returns></returns>
        public virtual T Get<T>(string name, Func<T> func, int expiredSecond, string regionName )
        {
            var cache = GetMemoryCache();
            if (cache.Contains(name)) return (T)cache.Get(name);
            expiredSecond = Math.Max(1, expiredSecond);

            var t = func();
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool Has(string name)
        {
            var tagCache = GetTagCache();
            return tagCache.Contains(name);
        }
    }
}
#endif