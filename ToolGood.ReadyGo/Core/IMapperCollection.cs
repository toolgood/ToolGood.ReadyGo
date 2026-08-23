using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 定义映射器集合接口，用于管理 <see cref="IMapper"/> 列表、对象工厂以及各类值转换委托。
    /// </summary>
    public interface IMapperCollection : IList<IMapper>
    {
        /// <summary>
        /// 根据数据读取器创建对象的工厂委托。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <returns>创建的对象。</returns>
        public delegate object ObjectFactoryDelegate(DbDataReader dataReader);

        /// <summary>
        /// 获取或设置默认的列序列化器。
        /// </summary>
        IColumnSerializer ColumnSerializer { get; set; }

        /// <summary>
        /// 清除对象工厂缓存。
        /// </summary>
        /// <param name="type">要清除的类型，为 null 时清除全部工厂。</param>
        void ClearFactories(Type type = null);
        /// <summary>
        /// 查找第一个匹配的转换委托。
        /// </summary>
        /// <param name="predicate">用于获取每个映射器转换委托的谓词。</param>
        /// <returns>第一个非空转换委托，若无匹配则返回 null。</returns>
        Func<object, object> Find(Func<IMapper, Func<object, object>> predicate);
        /// <summary>
        /// 查找转换委托并立即对值执行转换。
        /// </summary>
        /// <param name="predicate">用于获取每个映射器转换委托的谓词。</param>
        /// <param name="value">待转换的值。</param>
        /// <returns>转换后的值，若未找到转换器则返回原值。</returns>
        object FindAndExecute(Func<IMapper, Func<object, object>> predicate, object value);
        /// <summary>
        /// 查找从数据库值到目标成员类型的转换委托。
        /// </summary>
        /// <param name="destInfo">目标成员信息。</param>
        /// <param name="srcType">数据库值的源类型。</param>
        /// <returns>转换委托，若无匹配则返回 null。</returns>
        Func<object, object> FindFromDbConverter(MemberInfo destInfo, Type srcType);
        /// <summary>
        /// 查找从数据库值到目标类型的转换委托。
        /// </summary>
        /// <param name="destType">目标类型。</param>
        /// <param name="srcType">数据库值的源类型。</param>
        /// <returns>转换委托，若无匹配则返回 null。</returns>
        Func<object, object> FindFromDbConverter(Type destType, Type srcType);
        /// <summary>
        /// 查找从源成员值到数据库目标类型的转换委托。
        /// </summary>
        /// <param name="destType">数据库目标类型。</param>
        /// <param name="srcInfo">源成员信息。</param>
        /// <returns>转换委托，若无匹配则返回 null。</returns>
        Func<object, object> FindToDbConverter(Type destType, MemberInfo srcInfo);
        /// <summary>
        /// 获取指定类型的对象工厂委托。
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <returns>该类型对应的工厂委托，若不存在则返回 null。</returns>
        ObjectFactoryDelegate GetFactory(Type type);
        /// <summary>
        /// 判断指定类型是否已注册对象工厂。
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <returns>已注册返回 true，否则返回 false。</returns>
        bool HasFactory(Type type);
        /// <summary>
        /// 注册指定类型的对象工厂。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="factory">用于创建该类型对象的工厂委托。</param>
        void RegisterFactory<T>(Func<DbDataReader, T> factory);
    }
}
