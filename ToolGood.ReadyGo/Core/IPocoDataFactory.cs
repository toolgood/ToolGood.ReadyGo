using System;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 定义 POCO 元数据工厂接口，用于按类型或对象构建 <see cref="PocoData"/> 与 <see cref="TableInfo"/>。
    /// </summary>
    public interface IPocoDataFactory
    {
        /// <summary>
        /// 获取指定类型对应的 <see cref="PocoData"/> 元数据。
        /// </summary>
        /// <param name="type">POCO 类型。</param>
        /// <returns>该类型对应的元数据。</returns>
        PocoData ForType(Type type);
        /// <summary>
        /// 获取指定类型对应的 <see cref="TableInfo"/> 表信息。
        /// </summary>
        /// <param name="type">POCO 类型。</param>
        /// <returns>该类型对应的表信息。</returns>
        TableInfo TableInfoForType(Type type);
        /// <summary>
        /// 根据对象及主键信息获取对应的 <see cref="PocoData"/> 元数据。
        /// </summary>
        /// <param name="o">POCO 对象。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="autoIncrement">主键是否自增。</param>
        /// <returns>该对象对应的元数据。</returns>
        PocoData ForObject(object o, string primaryKeyName, bool autoIncrement);
    }
}
