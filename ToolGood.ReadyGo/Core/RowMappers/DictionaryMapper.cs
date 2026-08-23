using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ToolGood.ReadyGo.NPoco.RowMappers
{
    /// <summary>
    /// 将查询结果行映射为字典（Dictionary/IDictionary）或动态对象（Expando）的行映射器。
    /// </summary>
    public class DictionaryMapper : RowMapper
    {
        private PosName[] _posNames;

        /// <summary>
        /// 判断目标 POCO 类型是否为 object 或字典类型，以决定是否由当前映射器处理。
        /// </summary>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <returns>若类型为 object 或字典类型则返回 true，否则返回 false。</returns>
        public override bool ShouldMap(PocoData pocoData)
        {
            return pocoData.Type == typeof (object)
                   || pocoData.Type == typeof (Dictionary<string, object>)
                   || pocoData.Type == typeof (IDictionary<string, object>);
        }

        /// <summary>
        /// 初始化映射器，解析并缓存结果集的列信息。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        public override void Init(DbDataReader dataReader, PocoData pocoData)
        {
            _posNames = GetColumnNames(dataReader, pocoData);
        }

        /// <summary>
        /// 将当前数据行转换为字典或动态对象实例。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="context">行映射上下文。</param>
        /// <returns>映射得到的字典或动态对象。</returns>
        public override object Map(DbDataReader dataReader, RowMapperContext context)
        {
            IDictionary<string, object> target = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (context.Type == typeof(object))
                target = new PocoExpando();

            for (int i = 0; i < _posNames.Length; i++)
            {
                var converter = context.PocoData.Mapper.FindFromDbConverter(typeof(object), dataReader.GetFieldType(_posNames[i].Pos)) ?? (x => x);
                target.Add(_posNames[i].Name, dataReader.IsDBNull(_posNames[i].Pos) ? null : converter(dataReader.GetValue(_posNames[i].Pos)));
            }

            return target;
        }
    }
}