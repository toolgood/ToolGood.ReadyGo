using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 提供类型判断相关的扩展方法。
    /// </summary>
    public static class TypeHelpers
    {
        /// <summary>
        /// Gets an object's type even if it is null.
        /// </summary>
        /// <param name="that">The object being extended.</param>
        /// <returns>The objects type.</returns>
        public static Type GetTheType(this object that)
        {
            if (that != null)
            {
                return that.GetType();
            }

            return null;
        }
        
        /// <summary>
        /// 判断类型是否为类（排除 Type、字符串、object 及数组等特殊类型）。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>若是类则返回 true，否则返回 false。</returns>
        public static bool IsAClass(this Type type)
        {
            return type != typeof(Type) && !type.GetTypeInfo().IsValueType && (type.GetTypeInfo().IsClass || type.GetTypeInfo().IsInterface) && type != typeof (string) && type != typeof(object) && !type.IsArray;
        }
    }
}
