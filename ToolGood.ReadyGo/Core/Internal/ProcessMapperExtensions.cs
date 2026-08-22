#nullable enable
using System;

namespace ToolGood.ReadyGo.NPoco.Internal
{
    public static class ProcessMapperExtensions
    {
        public static bool TryGetMapper(this IDatabase database, PocoColumn pc, out Func<object?, object> converter)
        {
            converter = database.Mappers.FindToDbConverter(pc.ColumnType, pc.MemberInfoData.MemberInfo);
            return converter is not null;
        }

        public static object ProcessMapper(this IDatabase database, PocoColumn pc, object? value)
        {
            if (TryGetMapper(database, pc, out var converter))
            {
                return converter(value);
            }
            return ProcessDefaultMappings(database, pc, value);
        }

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