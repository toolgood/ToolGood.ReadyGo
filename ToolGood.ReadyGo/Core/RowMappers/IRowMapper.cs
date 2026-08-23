using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace ToolGood.ReadyGo.NPoco.RowMappers
{
    /// <summary>
    /// 定义行映射器的基本契约。
    /// </summary>
    public interface IRowMapper
    {
        /// <summary>
        /// 判断该映射器是否适用于指定 POCO 类型。
        /// </summary>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <returns>若适用则返回 true，否则返回 false。</returns>
        bool ShouldMap(PocoData pocoData);

        /// <summary>
        /// 将当前数据行映射为目标实例。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="context">行映射上下文。</param>
        /// <returns>映射得到的目标实例。</returns>
        object Map(DbDataReader dataReader, RowMapperContext context);

        /// <summary>
        /// 初始化映射器。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        void Init(DbDataReader dataReader, PocoData pocoData);
    }

    /// <summary>
    /// 行映射器的抽象基类，提供列名解析与转换器获取的公共逻辑。
    /// </summary>
    public abstract class RowMapper : IRowMapper
    {
        /// <summary>
        /// 判断该映射器是否适用于指定 POCO 类型。
        /// </summary>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <returns>若适用则返回 true，否则返回 false。</returns>
        public abstract bool ShouldMap(PocoData pocoData);

        /// <summary>
        /// 初始化映射器。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        public virtual void Init(DbDataReader dataReader, PocoData pocoData)
        {
        }

        private PosName[] _columnNames;

        /// <summary>
        /// 解析并缓存结果集的列名（转换为 POCO 成员键）。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <returns>解析后的列信息数组。</returns>
        protected PosName[] GetColumnNames(DbDataReader dataReader, PocoData pocoData)
        {
            if (_columnNames != null)
                return _columnNames;

            var cols = Enumerable.Range(0, dataReader.FieldCount)
                .Select(x => new PosName { Pos = x, Name = dataReader.GetName(x) })
                .Where(x => !string.Equals("poco_rn", x.Name))
                .ToArray();

            if (pocoData.IsQueryGenerated)
            {
                return cols;
            }

            if (cols.Any(x => x.Name.StartsWith(PropertyMapperNameConvention.SplitPrefix, StringComparison.OrdinalIgnoreCase)))
            {
                return (_columnNames = cols.ConvertFromNewConvention(pocoData).ToArray());
            }

            return (_columnNames = cols.ConvertFromOldConvention(pocoData.Members).ToArray());
        }

        /// <summary>
        /// 将当前数据行映射为目标实例。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="context">行映射上下文。</param>
        /// <returns>映射得到的目标实例。</returns>
        public abstract object Map(DbDataReader dataReader, RowMapperContext context);

        /// <summary>
        /// 获取从源类型到目标类型的值转换器。
        /// </summary>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <param name="pocoColumn">列信息（可为 null）。</param>
        /// <param name="sourceType">源类型。</param>
        /// <param name="desType">目标类型。</param>
        /// <returns>值转换器委托。</returns>
        public static Func<object, object> GetConverter(PocoData pocoData, PocoColumn pocoColumn, Type sourceType, Type desType)
        {
            var converter = MappingHelper.GetConverter(pocoData.Mapper, pocoColumn, sourceType, desType);
            return converter;
        }
    }
}