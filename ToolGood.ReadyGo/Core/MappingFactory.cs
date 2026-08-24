using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using ToolGood.ReadyGo.NPoco.RowMappers;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 映射工厂，根据 POCO 元数据与数据读取器选择合适的行映射器，并将数据行映射为对象。
    /// </summary>
    public class MappingFactory
    {
        /// <summary>
        /// 获取或设置行映射器工厂列表。
        /// </summary>
        public static List<Func<IMapperCollection, IRowMapper>> RowMappers { get; private set; } 
        private readonly PocoData _pocoData;
        private readonly IRowMapper _rowMapper;

        static MappingFactory()
        {
            RowMappers = new List<Func<IMapperCollection, IRowMapper>>()
            {
                x => new ValueTupleRowMapper(x),
                _ => new DictionaryMapper(),
                _ => new OrderedDictionaryMapper(),
                _ => new ValueTypeMapper(),
                _ => new ArrayMapper(),
                _ => new PropertyMapper()
            };
        }

        /// <summary>
        /// 使用 POCO 元数据与数据读取器初始化 <see cref="MappingFactory"/> 实例，并选中匹配的行映射器。
        /// </summary>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <param name="dataReader">数据读取器。</param>
        public MappingFactory(PocoData pocoData, DbDataReader dataReader)
        {
            _pocoData = pocoData;
            _rowMapper = SelectRowMapper(pocoData);
            _rowMapper.Init(dataReader, pocoData);
        }

        /// <summary>
        /// 按 POCO 类型直接短路选择对应的行映射器，避免每查询实例化全部候选映射器再丢弃。
        /// 判断条件与原 <see cref="RowMappers"/> 列表的逐项匹配顺序保持一致。
        /// </summary>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <returns>选中的行映射器实例。</returns>
        private static IRowMapper SelectRowMapper(PocoData pocoData)
        {
            // 若外部曾修改过默认的映射器工厂列表，回退到原逐项匹配逻辑以保持兼容
            if (RowMappers.Count != 6)
                return RowMappers.Select(mapper => mapper(pocoData.Mapper)).First(x => x.ShouldMap(pocoData));

            var type = pocoData.Type;

            if (ValueTupleRowMapper.IsValueTuple(type))
                return new ValueTupleRowMapper(pocoData.Mapper);
            if (type == typeof(object) || type == typeof(Dictionary<string, object>) || type == typeof(IDictionary<string, object>))
                return new DictionaryMapper();
            if (type == typeof(OrderedDictionary))
                return new OrderedDictionaryMapper();
            if (type.GetTypeInfo().IsValueType || type == typeof(string) || type == typeof(byte[]))
                return new ValueTypeMapper();
            if (type.IsArray)
                return new ArrayMapper();
            return new PropertyMapper();
        }

        /// <summary>
        /// 将数据读取器的当前行映射到指定对象实例。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="instance">要填充的目标对象实例，可为 null。</param>
        /// <returns>映射后的对象。</returns>
        public object Map(DbDataReader dataReader, object instance)
        {
            return _rowMapper.Map(dataReader, new RowMapperContext()
            {
                Instance = instance,
                PocoData = _pocoData
            });
        }
    }
}
