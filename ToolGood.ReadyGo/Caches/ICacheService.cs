using System;

namespace ToolGood.ReadyGo.Caches
{
    /// <summary>
    ///
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 是否设置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Has(string name);

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="name"></param>
        void Clear(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="func"></param>
        /// <param name="expiredSecond"></param>
        /// <param name="regionName"></param>
        /// <returns></returns>
        T Get<T>(string name, Func<T> func, int expiredSecond, string regionName );
    }
}