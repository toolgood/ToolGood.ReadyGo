using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco.RowMappers
{
    /// <summary>
    /// 表示动态对象（IDictionary）中的成员，通过字典键读写其值。
    /// </summary>
    public class DynamicPocoMember : PocoMember
    {
        /// <summary>
        /// 以成员名称作为键，将值写入目标字典。
        /// </summary>
        /// <param name="target">目标动态对象（字典）。</param>
        /// <param name="value">要设置的值。</param>
        public override void SetValue(object target, object value)
        {
            ((IDictionary) target)[Name] = value;
        }

        /// <summary>
        /// 从目标字典中读取以成员名称为键的值。
        /// </summary>
        /// <param name="target">目标动态对象（字典）。</param>
        /// <returns>字典中对应键的值；若不存在则返回 null。</returns>
        public override object GetValue(object target)
        {
            var val = ((IDictionary)target).Contains(Name) ? ((IDictionary)target)[Name] : null;
            return val;
        }
    }
}