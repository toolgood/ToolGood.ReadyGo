using System;

namespace ToolGood.ReadyGo3.Gadget.Caches
{
    /// <summary>
    /// 
    /// </summary>
    public partial class NullCacheService : ICacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void Clear(string name)
        {
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
        public T Get<T>(string name, Func<T> func, int expiredSecond, string regionName )
        {
            return func();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Has(string name)
        {
            return false;
        }
    }
}