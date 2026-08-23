using System;
using System.Collections.Specialized;
using System.Data.Common;

namespace ToolGood.ReadyGo.NPoco.RowMappers;

/// <summary>
/// 将查询结果行映射为 OrderedDictionary 的行映射器。
/// </summary>
public class OrderedDictionaryMapper : RowMapper
{
    private PosName[] _posNames;

    /// <summary>
    /// 判断目标 POCO 类型是否为 OrderedDictionary，以决定是否由当前映射器处理。
    /// </summary>
    /// <param name="pocoData">POCO 元数据。</param>
    /// <returns>若类型为 OrderedDictionary 则返回 true，否则返回 false。</returns>
    public override bool ShouldMap(PocoData pocoData)
    {
        return pocoData.Type == typeof(OrderedDictionary);
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
    /// 将当前数据行转换为 OrderedDictionary 实例。
    /// </summary>
    /// <param name="dataReader">数据读取器。</param>
    /// <param name="context">行映射上下文。</param>
    /// <returns>映射得到的 OrderedDictionary 实例。</returns>
    public override object Map(DbDataReader dataReader, RowMapperContext context)
    {
        var target = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < _posNames.Length; i++)
        {
            var converter = context.PocoData.Mapper.FindFromDbConverter(typeof(object), dataReader.GetFieldType(_posNames[i].Pos)) ?? (x => x);
            target.Add(_posNames[i].Name, dataReader.IsDBNull(_posNames[i].Pos) ? null : converter(dataReader.GetValue(_posNames[i].Pos)));
        }

        return target;
    }
}