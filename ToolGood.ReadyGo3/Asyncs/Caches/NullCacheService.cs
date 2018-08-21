using System;
using System.Threading.Tasks;

#if !NET40

namespace ToolGood.ReadyGo3.Gadget.Caches
{
    /// <summary>
    /// 
    /// </summary>
    partial class NullCacheService : ICacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="func"></param>
        /// <param name="expiredSecond"></param>
        /// <param name="regionName"></param>
        /// <returns></returns>
        public Task<T> GetAsync<T>(string name, Func<Task<T>> func, int expiredSecond, string regionName)
        {
            return func();
        }

    }
}
#endif