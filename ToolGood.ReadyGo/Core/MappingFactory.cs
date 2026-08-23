using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
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
            _rowMapper = RowMappers.Select(mapper => mapper(_pocoData.Mapper)).First(x => x.ShouldMap(pocoData));
            _rowMapper.Init(dataReader, pocoData);
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
