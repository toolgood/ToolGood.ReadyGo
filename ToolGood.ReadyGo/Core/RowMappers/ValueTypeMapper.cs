using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco.RowMappers
{
    /// <summary>
    /// 将查询结果行映射为值类型、字符串或字节数组的行映射器。
    /// </summary>
    public class ValueTypeMapper : RowMapper
    {
        private Func<object, object> _converter;

        /// <summary>
        /// 判断目标 POCO 类型是否为值类型、字符串或字节数组，以决定是否由当前映射器处理。
        /// </summary>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <returns>若类型为值类型、字符串或字节数组则返回 true，否则返回 false。</returns>
        public override bool ShouldMap(PocoData pocoData)
        {
            return pocoData.Type.GetTypeInfo().IsValueType || pocoData.Type == typeof (string) || pocoData.Type == typeof (byte[]);
        }

        /// <summary>
        /// 初始化映射器，获取首列值到目标类型的转换器。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        public override void Init(DbDataReader dataReader, PocoData pocoData)
        {
            _converter = GetConverter(pocoData, null, dataReader.GetFieldType(0), pocoData.Type) ?? (x => x);
            base.Init(dataReader, pocoData);
        }

        /// <summary>
        /// 将当前数据行首列的值转换为目标类型。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="context">行映射上下文。</param>
        /// <returns>转换后的值；若首列为 NULL 则返回 null。</returns>
        public override object Map(DbDataReader dataReader, RowMapperContext context)
        {
            if (dataReader.IsDBNull(0))
                return null;

            return _converter(dataReader.GetValue(0));
        }
    }
}