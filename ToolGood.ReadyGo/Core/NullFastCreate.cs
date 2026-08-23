using System;
using System.Data.Common;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// <see cref="IFastCreate"/> 的空实现，仅用于占位，调用其创建方法会抛出 <see cref="NotImplementedException"/>。
    /// </summary>
    public class NullFastCreate : IFastCreate
    {
        /// <summary>
        /// 该方法未实现，调用会抛出 <see cref="NotImplementedException"/>。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <returns>不会返回。</returns>
        public object Create(DbDataReader dataReader)
        {
            throw new NotImplementedException();
        }
    }
}
