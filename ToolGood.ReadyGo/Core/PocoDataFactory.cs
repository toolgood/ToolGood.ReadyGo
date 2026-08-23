using System;
using System.Collections.Generic;

namespace ToolGood.ReadyGo.NPoco
{

    /// <summary>
    /// 负责按类型构建并缓存 POCO 映射数据的工厂。
    /// </summary>
    public class PocoDataFactory : IPocoDataFactory
    {
        private readonly static Cache<Type, InitializedPocoDataBuilder> _pocoDatas = Cache<Type, InitializedPocoDataBuilder>.CreateStaticCache();
        private readonly IMapperCollection _mapper;

        /// <summary>
        /// 初始化 PocoDataFactory 类的新实例。
        /// </summary>
        /// <param name="mapper">映射器集合。</param>
        public PocoDataFactory(IMapperCollection mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// 获取指定类型对应的 POCO 数据。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <returns>该类型对应的 POCO 数据。</returns>
        public PocoData ForType(Type type)
        {
            Guard(type);
            var pocoDataBuilder = _pocoDatas.Get(type, () => BaseClassFallbackPocoDataBuilder(type));
            return pocoDataBuilder.Build();
        }

        /// <summary>
        /// 获取指定类型对应的表信息。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <returns>该类型对应的表信息。</returns>
        public TableInfo TableInfoForType(Type type)
        {
            Guard(type);
            var pocoDataBuilder = _pocoDatas.Get(type, () => BaseClassFallbackPocoDataBuilder(type));
            return pocoDataBuilder.BuildTableInfo();
        }

        /// <summary>
        /// 根据对象实例获取其 POCO 数据（对动态对象按主键与自增信息构建）。
        /// </summary>
        /// <param name="o">实体对象。</param>
        /// <param name="primaryKeyName">主键名称。</param>
        /// <param name="autoIncrement">主键是否自增。</param>
        /// <returns>对应的 POCO 数据。</returns>
        public PocoData ForObject(object o, string primaryKeyName, bool autoIncrement)
        {
            return ForObjectStatic(o, primaryKeyName, autoIncrement, ForType, _mapper);
        }

        private InitializedPocoDataBuilder BaseClassFallbackPocoDataBuilder(Type type)
        {
            var builder = new PocoDataBuilder(type, _mapper).Init();
            var persistedType = builder.BuildTableInfo().PersistedType;
            if (persistedType == null || persistedType == type)
            {
                return builder;
            }
            return new PocoDataBuilder(persistedType, _mapper).Init();
        }

        /// <summary>
        /// 静态地根据对象实例获取其 POCO 数据；动态对象使用其键值构建列，否则回退到指定获取器。
        /// </summary>
        /// <param name="o">实体对象。</param>
        /// <param name="primaryKeyName">主键名称。</param>
        /// <param name="autoIncrement">主键是否自增。</param>
        /// <param name="fallback">非动态对象时使用的获取器。</param>
        /// <param name="mapper">映射器集合。</param>
        /// <returns>对应的 POCO 数据。</returns>
        public static PocoData ForObjectStatic(object o, string primaryKeyName, bool autoIncrement, Func<Type, PocoData> fallback, IMapperCollection mapper)
        {
            var t = o.GetType();
            if (t == typeof (System.Dynamic.ExpandoObject) || t == typeof (PocoExpando))
            {
                var pd = new PocoData(t, mapper, Singleton<NullFastCreate>.Instance)
                {
                    TableInfo = new TableInfo
                    {
                        PrimaryKey = primaryKeyName,
                        AutoIncrement = autoIncrement
                    },
                    Columns = new Dictionary<string, PocoColumn>(StringComparer.OrdinalIgnoreCase)
                };
                foreach (var col in ((IDictionary<string, object>)o))
                {
                    pd.Columns.Add(col.Key, new ExpandoColumn
                    {
                        ColumnName = col.Key,
                        MemberInfoData = new MemberInfoData(col.Key, col.Value.GetTheType() ?? typeof(object), typeof(object)),
                    });
                }
                if (!pd.Columns.ContainsKey(primaryKeyName))
                {
                    pd.Columns.Add(primaryKeyName, new ExpandoColumn { ColumnName = primaryKeyName, ColumnType = typeof(object) });
                }
                return pd;
            }
            else
                return fallback(t);
        }

        /// <summary>
        /// 校验类型是否可用于当前方法，动态类型将抛出异常。
        /// </summary>
        /// <param name="type">要校验的类型。</param>
        public static void Guard(Type type)
        {
            if (type == typeof(System.Dynamic.ExpandoObject) || type == typeof(PocoExpando))
                throw new InvalidOperationException("Can't use dynamic types with this method");
        }

    }
}
