#nullable enable
using System;

namespace ToolGood.ReadyGo.NPoco.Internal
{
    /// <summary>
    /// 提供 POCO 列值写入数据库前的映射处理扩展方法。
    /// </summary>
    public static class ProcessMapperExtensions
    {
        /// <summary>
        /// 尝试获取指定列从 POCO 类型到数据库类型的转换器。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pc">POCO 列信息。</param>
        /// <param name="converter">输出的转换器委托。</param>
        /// <returns>若存在转换器则返回 true，否则返回 false。</returns>
        public static bool TryGetMapper(this IDatabase database, PocoColumn pc, out Func<object?, object> converter)
        {
            converter = database.Mappers.FindToDbConverter(pc.ColumnType, pc.MemberInfoData.MemberInfo);
            return converter is not null;
        }

        /// <summary>
        /// 对列值应用映射：优先使用自定义转换器，否则应用默认映射。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pc">POCO 列信息。</param>
        /// <param name="value">待映射的值。</param>
        /// <returns>映射后的值。</returns>
        public static object ProcessMapper(this IDatabase database, PocoColumn pc, object? value)
        {
            if (TryGetMapper(database, pc, out var converter))
            {
                return converter(value);
            }
            return ProcessDefaultMappings(database, pc, value);
        }

        /// <summary>
        /// 应用默认映射：处理序列化列与枚举到字符串的转换。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pocoColumn">POCO 列信息。</param>
        /// <param name="value">待映射的值。</param>
        /// <returns>映射后的值。</returns>
        public static object ProcessDefaultMappings(IDatabase database, PocoColumn pocoColumn, object? value)
        {
            if (pocoColumn.SerializedColumn)
            {
                var serializer = pocoColumn.ColumnSerializer ?? database.Mappers.ColumnSerializer;
                return serializer.Serialize(value);
            }
            if (pocoColumn.ColumnType == typeof(string) && Database.IsEnum(pocoColumn.MemberInfoData) && value != null)
            {
                return value.ToString()!;
            }

            return database.DatabaseType.ProcessDefaultMappings(pocoColumn, value);
        }
    }
}