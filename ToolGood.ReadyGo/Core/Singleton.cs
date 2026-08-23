using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.NPoco
{
    static class Singleton<T> where T : new()
    {
        /// <summary>
        /// 该泛型单例的实例。
        /// </summary>
        public static T Instance = new T();
    }

    class DynamicDatabaseType
    {
        /// <summary>
        /// 数据库类型缓存。
        /// </summary>
        public static Cache<string, DatabaseType> cache = Cache<string, DatabaseType>.CreateStaticCache();

        /// <summary>
        /// 根据类型名称创建对应的 SqlServer 数据库类型实例。
        /// </summary>
        /// <param name="type">数据库类型名称。</param>
        /// <returns>对应的数据库类型实例。</returns>
        public static DatabaseType MakeSqlServerType(string type)
        {
            try
            {
                return cache.Get(type, () =>
                {
                    var newType = Type.GetType($"ToolGood.ReadyGo.NPoco.DatabaseTypes.{type}, ToolGood.ReadyGo.NPoco.SqlServer") 
                                  ?? Type.GetType($"ToolGood.ReadyGo.NPoco.DatabaseTypes.{type}, ToolGood.ReadyGo.NPoco.SqlServer.SystemData");

                    var gen = typeof(Singleton<>).MakeGenericType(newType);
                    return (DatabaseType)gen.GetField("Instance").GetValue(null);
                });                
            }
            catch (Exception ex)
            {
                throw new Exception($"No database type found for the type string specified: '{type}'. Make sure the relevant assembly ToolGood.ReadyGo.NPoco.SqlServer.* is referenced.", ex);
            }
        }
    }
}
