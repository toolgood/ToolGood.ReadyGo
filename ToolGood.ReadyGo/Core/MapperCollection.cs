using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Data.Common;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 映射器集合的实现，用于管理映射器列表、对象工厂以及值转换委托。
    /// </summary>
    public class MapperCollection : List<IMapper>, IMapperCollection
    {
        /// <summary>
        /// 获取或设置默认的列序列化器。
        /// </summary>
        public IColumnSerializer ColumnSerializer { get; set; } = DatabaseFactory.ColumnSerializer;
        internal readonly Dictionary<Type, IMapperCollection.ObjectFactoryDelegate> Factories = new Dictionary<Type, IMapperCollection.ObjectFactoryDelegate>();
        

        /// <summary>
        /// 初始化 <see cref="MapperCollection"/> 实例，并注册内置的默认对象工厂。
        /// </summary>
        public MapperCollection()
        {
            Factories.Add(typeof(object), x => new PocoExpando());
            Factories.Add(typeof(IDictionary<string, object>), x => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));
            Factories.Add(typeof(Dictionary<string, object>), x => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));
            Factories.Add(typeof(OrderedDictionary), x => new OrderedDictionary(StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 注册指定类型的对象工厂。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="factory">用于创建该类型对象的工厂委托。</param>
        public void RegisterFactory<T>(Func<DbDataReader, T> factory)
        {
            Factories[typeof(T)] = x => factory(x);
        }

        /// <summary>
        /// 获取指定类型的对象工厂委托。
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <returns>该类型对应的工厂委托，若不存在则返回 null。</returns>
        public IMapperCollection.ObjectFactoryDelegate GetFactory(Type type)
        {
            return Factories.ContainsKey(type) ? Factories[type] : null;
        }

        /// <summary>
        /// 判断指定类型是否已注册对象工厂。
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <returns>已注册返回 true，否则返回 false。</returns>
        public bool HasFactory(Type type)
        {
            return Factories.ContainsKey(type);
        }

        /// <summary>
        /// 清除对象工厂缓存。
        /// </summary>
        /// <param name="type">要清除的类型，为 null 时清除全部工厂。</param>
        public void ClearFactories(Type type = null)
        {
            if (type != null)
            {
                Factories.Remove(type);
            }
            else
            {
                Factories.Clear();
            }
        }

        /// <summary>
        /// 查找第一个匹配的转换委托。
        /// </summary>
        /// <param name="predicate">用于获取每个映射器转换委托的谓词。</param>
        /// <returns>第一个非空转换委托，若无匹配则返回 null。</returns>
        public Func<object, object> Find(Func<IMapper, Func<object, object>> predicate)
        {
            return this.Select(predicate).FirstOrDefault(x => x != null);
        }

        /// <summary>
        /// 查找转换委托并立即对值执行转换。
        /// </summary>
        /// <param name="predicate">用于获取每个映射器转换委托的谓词。</param>
        /// <param name="value">待转换的值。</param>
        /// <returns>转换后的值，若未找到转换器则返回原值。</returns>
        public object FindAndExecute(Func<IMapper, Func<object, object>> predicate, object value)
        {
            var converter = Find(predicate);
            return converter != null ? converter(value) : value;
        }

        // 使用 ValueTuple 作为缓存键（值类型），避免每次查找分配匿名类型对象并装箱。
        private static readonly Cache<(Type, MemberInfo), Func<object, object>> ToDbConverterCache = new();
        private static readonly Cache<(Type, Type), Func<object, object>> FromDbConverterCache = new();
        private static readonly Cache<(MemberInfo, Type), Func<object, object>> FromDbMemberConverterCache = new();

        /// <summary>
        /// 查找从数据库值到目标类型的转换委托。
        /// </summary>
        /// <param name="destType">目标类型。</param>
        /// <param name="srcType">数据库值的源类型。</param>
        /// <returns>转换委托，若无匹配则返回 null。</returns>
        public Func<object, object> FindFromDbConverter(Type destType, Type srcType)
        {
            var key = (destType, srcType);
            return FromDbConverterCache.Get(key, () => Find(x => x.GetFromDbConverter(destType, srcType)));
        }

        /// <summary>
        /// 查找从数据库值到目标成员类型的转换委托。
        /// </summary>
        /// <param name="destInfo">目标成员信息。</param>
        /// <param name="srcType">数据库值的源类型。</param>
        /// <returns>转换委托，若无匹配则返回 null。</returns>
        public Func<object, object> FindFromDbConverter(MemberInfo destInfo, Type srcType)
        {
            var key = (destInfo, srcType);
            return FromDbMemberConverterCache.Get(key, () => Find(x => x.GetFromDbConverter(destInfo, srcType)));
        }

        /// <summary>
        /// 查找从源成员值到数据库目标类型的转换委托。
        /// </summary>
        /// <param name="destType">数据库目标类型。</param>
        /// <param name="srcInfo">源成员信息。</param>
        /// <returns>转换委托，若无匹配则返回 null。</returns>
        public Func<object, object> FindToDbConverter(Type destType, MemberInfo srcInfo)
        {
            var key = (destType, srcInfo);
            return ToDbConverterCache.Get(key, () => Find(x => x.GetToDbConverter(destType, srcInfo)));
        }
    }
}
