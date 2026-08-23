using System.Data.Common;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 定义根据数据读取器快速创建对象的工厂接口。
    /// </summary>
    public interface IFastCreate
    {
        /// <summary>
        /// 根据当前数据读取器的记录创建对象。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <returns>创建的对象。</returns>
        object Create(DbDataReader dataReader);
    }
}
