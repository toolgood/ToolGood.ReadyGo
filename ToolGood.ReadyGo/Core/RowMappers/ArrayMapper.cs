using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace ToolGood.ReadyGo.NPoco.RowMappers
{
    /// <summary>
    /// 将查询结果行映射为数组（Array）的行映射器。
    /// </summary>
    public class ArrayMapper : RowMapper
    {
        private PosName[] _posNames;

        /// <summary>
        /// 判断目标 POCO 类型是否为数组，以决定是否由当前映射器处理。
        /// </summary>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <returns>若类型为数组则返回 true，否则返回 false。</returns>
        public override bool ShouldMap(PocoData pocoData)
        {
            return pocoData.Type.IsArray;
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
        /// 将当前数据行转换为指定元素类型的数组实例。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="context">行映射上下文。</param>
        /// <returns>映射得到的数组对象。</returns>
        public override object Map(DbDataReader dataReader, RowMapperContext context)
        {
            var arrayType = context.Type.GetElementType();
            var array = Array.CreateInstance(arrayType, _posNames.Length);

            for (int i = 0; i < _posNames.Length; i++)
            {
                if (!dataReader.IsDBNull(_posNames[i].Pos))
                {
                    array.SetValue(dataReader.GetValue(_posNames[i].Pos), i);
                }
            }

            return array;
        }
    }
}