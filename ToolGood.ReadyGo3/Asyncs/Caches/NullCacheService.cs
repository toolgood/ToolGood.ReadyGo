using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo3.Gadget.Caches
{
    /// <summary>
    /// 
    /// </summary>
    partial class NullCacheService : ICacheService
    {
        public async Task<T> GetAsync<T>(string name, Func<Task<T>> func, int expiredSecond, string regionName)
        {
            return await func();
        }

    }
}