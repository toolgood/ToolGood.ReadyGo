using System.Collections.Generic;

namespace ToolGood.ReadyGo.Poco
{
    public class PocoColumnExpando : PocoColumn
    {
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="target"></param>
        /// <param name="val"></param>
        public override void SetValue(object target, object val)
        {
            (target as IDictionary<string, object>)[ColumnName] = val;
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override object GetValue(object target)
        {
            object val = null;
            (target as IDictionary<string, object>).TryGetValue(ColumnName, out val);
            return val;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public override object ChangeType(object val)
        {
            return val;
        }
    }
}