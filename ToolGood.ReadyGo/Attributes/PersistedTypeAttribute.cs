using System;
using System.Collections.Generic;
using System.Linq;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 持久化类型标签：用于指定该类型实际持久化到数据库时使用的类型。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PersistedTypeAttribute : Attribute
    {
        /// <summary>
        /// 持久化类型
        /// </summary>
        public Type PersistedType { get; set; }

        /// <summary>
        /// 持久化类型标签
        /// </summary>
        /// <param name="persistedType">持久化类型</param>
        public PersistedTypeAttribute(Type persistedType)
        {
            PersistedType = persistedType;
        }
    }
}
