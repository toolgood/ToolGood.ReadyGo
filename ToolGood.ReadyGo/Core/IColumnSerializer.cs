using System;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 定义列值的序列化与反序列化接口，用于在数据库存储值与对象值之间进行转换。
    /// </summary>
    public interface IColumnSerializer
    {
        /// <summary>
        /// 将对象值序列化为可存储到数据库列中的值。
        /// </summary>
        /// <param name="value">待序列化的对象值。</param>
        /// <returns>序列化后的列值。</returns>
        object Serialize(object value);
        /// <summary>
        /// 将从数据库列读取的值反序列化为目标类型的对象。
        /// </summary>
        /// <param name="value">从数据库列读取的值。</param>
        /// <param name="targetType">目标对象类型。</param>
        /// <returns>反序列化后的对象。</returns>
        object Deserialize(object value, Type targetType);
    }
}
