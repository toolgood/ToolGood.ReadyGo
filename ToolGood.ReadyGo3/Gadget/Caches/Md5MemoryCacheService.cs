using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !NETSTANDARD2_0
namespace ToolGood.ReadyGo3.Gadget.Caches
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Md5MemoryCacheService : MemoryCacheService
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
        public override T Get<T>(string name, Func<T> func, int expiredSecond, string regionName)
        {
            name = getMd5String(name);
            return base.Get<T>(name, func, expiredSecond, regionName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override bool Has(string name)
        {
            name = getMd5String(name);
            return base.Has(name);
        }

        private string getMd5String(string str)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] resultEncrypt = md5CSP.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
            md5CSP.Dispose();
            return BitConverter.ToString(resultEncrypt);//.Replace("-", "");
        }

    }
}
#endif