using System;
using System.Collections.Generic;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget
{
    internal static class EnumHelper
    {
        private static readonly Cache<Type, Dictionary<string, object>> _types = new Cache<Type, Dictionary<string, object>>();
        private static readonly Cache<Type, bool> _useString = new Cache<Type, bool>();

        /// <summary>
        /// 根据枚举值名称（忽略大小写）查找对应的枚举值
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="value">枚举值名称</param>
        /// <returns>对应的枚举值</returns>
        public static object EnumFromString(Type enumType, string value)
        {
            Dictionary<string, object> map = _types.Get(enumType, () => {
                var values = Enum.GetValues(enumType);
                var newmap = new Dictionary<string, object>(values.Length, StringComparer.InvariantCultureIgnoreCase);
                foreach (var v in values) {
                    newmap.Add(v.ToString(), v);
                }
                return newmap;
            });
            if (string.IsNullOrEmpty(value)) {
                throw new ArgumentException($"枚举类型 {enumType.Name} 的值不能为空。");
            }
            if (map.TryGetValue(value, out var result)) {
                return result;
            }
            throw new ArgumentException($"'{value}' 不是枚举类型 {enumType.Name} 的有效值，有效值：{string.Join(", ", map.Keys)}。");
        }

        public static bool UseEnumString(Type enumType)
        {
            return _useString.Get(enumType, () => {
                var atts = enumType.GetCustomAttributes(typeof(Enum2StringAttribute), true);
                return atts.Length > 0;
            });
        }
    }
}
