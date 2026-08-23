using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 枚举与字符串之间的映射器，用于在枚举值与字符串之间相互转换。
    /// </summary>
    public class EnumMapper : IDisposable
    {
        readonly Dictionary<Type, Dictionary<string, object>> _stringsToEnums = new Dictionary<Type, Dictionary<string, object>>();
        readonly Dictionary<Type, Dictionary<int, string>> _enumNumbersToStrings = new Dictionary<Type, Dictionary<int, string>>();
        readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        
        /// <summary>
        /// 根据枚举类型与字符串值查找对应的枚举值。
        /// </summary>
        /// <param name="type">枚举类型。</param>
        /// <param name="value">要查找的字符串值。</param>
        /// <returns>对应的枚举值。</returns>
        public object EnumFromString(Type type, string value)
        {
            PopulateIfNotPresent(type);
            if (!_stringsToEnums[type].ContainsKey(value))
            {
                throw new Exception(string.Format("The value '{0}' could not be found for Enum '{1}'", value, type));
            }
            return _stringsToEnums[type][value];
        }

        /// <summary>
        /// 将枚举值转换为对应的字符串表示。
        /// </summary>
        /// <param name="theEnum">要转换的枚举值。</param>
        /// <returns>枚举值对应的字符串。</returns>
        public string StringFromEnum(object theEnum)
        {
            Type typeOfEnum = theEnum.GetType();
            PopulateIfNotPresent(typeOfEnum);
            return _enumNumbersToStrings[typeOfEnum][(int)theEnum];
        }

        void PopulateIfNotPresent(Type type)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                if (!_stringsToEnums.ContainsKey(type))
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        Populate(type);
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        void Populate(Type type)
        {
            Array values = Enum.GetValues(type);
            _stringsToEnums[type] = new Dictionary<string, object>(values.Length);
            _enumNumbersToStrings[type] = new Dictionary<int, string>(values.Length);

            for (int i = 0; i < values.Length; i++)
            {
                object value = values.GetValue(i);
                _stringsToEnums[type].Add(value.ToString(), value);
                _enumNumbersToStrings[type].Add((int)value, value.ToString());
            }
        }

        /// <summary>
        /// 释放映射资源。
        /// </summary>
        public void Dispose()
        {
            _lock.Dispose();
        }
    }
}
