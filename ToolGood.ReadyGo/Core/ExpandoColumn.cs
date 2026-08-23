using System.Collections;
using System.Collections.Generic;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 用于动态对象（ExpandoObject/PocoExpando）的列映射，按列名读写字典。
    /// </summary>
    public class ExpandoColumn : PocoColumn
    {
        /// <summary>
        /// 将值写入目标动态对象的对应列。
        /// </summary>
        /// <param name="target">目标动态对象。</param>
        /// <param name="val">要写入的值。</param>
        public override void SetValue(object target, object val)
        {
            ((IDictionary) target)[ColumnName] = val;
        }

        /// <summary>
        /// 从目标动态对象读取对应列的值。
        /// </summary>
        /// <param name="target">源动态对象。</param>
        /// <returns>列对应的值；不存在时返回 null。</returns>
        public override object GetValue(object target) 
        {
            var val = ((IDictionary)target).Contains(ColumnName) ? ((IDictionary)target)[ColumnName] : null;
            return val;
        }

        /// <summary>
        /// 直接返回原值，不做类型转换。
        /// </summary>
        /// <param name="val">要处理的值。</param>
        /// <returns>原样返回的值。</returns>
        public override object ChangeType(object val) { return val; }
    }
}