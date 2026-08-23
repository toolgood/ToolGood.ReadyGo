using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ToolGood.ReadyGo.NPoco.RowMappers
{
    /// <summary>
    /// 将查询结果行映射为 ValueTuple（值元组）的行映射器。
    /// </summary>
    public class ValueTupleRowMapper : IRowMapper
    {
        private Func<DbDataReader, object> mapper = default!;
        private IMapperCollection mappers;

        private static Cache<(Type, string, IMapperCollection), Func<DbDataReader, object>> cache = new();

        /// <summary>
        /// 初始化 ValueTuple 行映射器。
        /// </summary>
        /// <param name="mappers">映射器集合。</param>
        public ValueTupleRowMapper(IMapperCollection mappers)
        {
            this.mappers = mappers;
        }

        /// <summary>
        /// 初始化映射器，构建当前元组类型的行映射委托。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        public void Init(DbDataReader dataReader, PocoData pocoData)
        {
            mapper = GetRowMapper(pocoData.Type, this.mappers, dataReader);
        }

        /// <summary>
        /// 将当前数据行映射为 ValueTuple 实例。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="context">行映射上下文。</param>
        /// <returns>映射得到的 ValueTuple 实例。</returns>
        public object Map(DbDataReader dataReader, RowMapperContext context)
        {
            return mapper(dataReader);
        }

        /// <summary>
        /// 判断指定类型是否为 ValueTuple 类型。
        /// </summary>
        /// <param name="type">待判断的类型。</param>
        /// <returns>若为 ValueTuple 类型则返回 true，否则返回 false。</returns>
        public static bool IsValueTuple(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var baseType = type.GetGenericTypeDefinition();
            return (
                baseType == typeof(ValueTuple<>) ||
                baseType == typeof(ValueTuple<,>) ||
                baseType == typeof(ValueTuple<,,>) ||
                baseType == typeof(ValueTuple<,,,>) ||
                baseType == typeof(ValueTuple<,,,,>) ||
                baseType == typeof(ValueTuple<,,,,,>) ||
                baseType == typeof(ValueTuple<,,,,,,>) ||
                baseType == typeof(ValueTuple<,,,,,,,>)
            );
        }

        /// <summary>
        /// 判断该映射器是否适用于指定 POCO 类型。
        /// </summary>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <returns>若类型为 ValueTuple 则返回 true，否则返回 false。</returns>
        public bool ShouldMap(PocoData pocoData)
        {
            return IsValueTuple(pocoData.Type);
        }

        private static Func<DbDataReader, object> GetRowMapper(Type type, IMapperCollection mappers, DbDataReader dataReader)
        {
            StringBuilder columnTypes = new();
            for (var i = 0; i < dataReader.VisibleFieldCount; i++)
                columnTypes.AppendLine(dataReader.GetFieldType(i)?.ToString());

            return cache.Get((type, columnTypes.ToString(), mappers), () => CreateRowMapper(type, mappers, dataReader));
        }

        private static Func<DbDataReader, object> CreateRowMapper(Type type, IMapperCollection mappers, DbDataReader dataReader)
        {
            var reader = Expression.Parameter(typeof(DbDataReader), "reader");
            var (tupleExpr, _) = CreateTupleExpression(type, mappers, dataReader, reader, 0);

            // reader => (object)new ValueTuple<T1, T2, ...>(value1, value2, ...);
            var expr = Expression.Lambda(
                Expression.Convert(tupleExpr, typeof(object)),
                new[] { reader }
            );
            return (Func<DbDataReader, object>)expr.Compile();
        }

        private static (NewExpression expr, int fieldsIndex) CreateTupleExpression(Type type, IMapperCollection mappers, DbDataReader dataReader, ParameterExpression reader, int fieldIndex)
        {
            var argTypes = type.GetGenericArguments();
            var ctor = type.GetConstructor(argTypes);
            var getValue = typeof(DbDataReader).GetMethod("GetValue")!;
            var isDBNull = typeof(DbDataReader).GetMethod("IsDBNull")!;

            if (argTypes.Count() > dataReader.FieldCount)
                throw new InvalidOperationException("SQL query does not return enough fields to fill the tuple");

            var args = new List<Expression>();

            foreach (var argType in argTypes)
            {
                if (IsValueTuple(argType))
                {
                    // It's tuples all the way down
                    var (expr, newFieldIndex) = CreateTupleExpression(argType, mappers, dataReader, reader, fieldIndex);
                    args.Add(expr);
                    fieldIndex += newFieldIndex;
                }
                else
                {
                    if (fieldIndex >= dataReader.FieldCount)
                        throw new InvalidOperationException($"SQL query does not return enough fields to fill the tuple (missing type: {argType.FullName})");

                    var rawValue = Expression.Call(reader, getValue, new[] { Expression.Constant(fieldIndex) });
                    var converter = MappingHelper.GetConverter(mappers, null, dataReader.GetFieldType(fieldIndex), argType);

                    // reader.IsDBNull(i) ? (T)null : converter(reader.GetValue(i))
                    args.Add(Expression.Condition(
                        Expression.Call(reader, isDBNull, new[] { Expression.Constant(fieldIndex) }),
                        Expression.Convert(Expression.Constant(null), argType),
                        Expression.Convert(
                            converter != null
                                ? (Expression)Expression.Invoke(Expression.Constant(converter), new[] { rawValue })
                                : (Expression)rawValue,
                            argType
                        )
                    ));

                    fieldIndex++;
                }
            }

            return (Expression.New(ctor, args), fieldIndex);
        }
    }
}
