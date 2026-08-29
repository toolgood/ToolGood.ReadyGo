using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 映射帮助类，用于按列信息、源类型与目标类型构建值转换委托。
    /// </summary>
    public class MappingHelper
    {
        static readonly EnumMapper EnumMapper = new EnumMapper();
        static readonly Cache<Type, Type> UnderlyingTypes = Cache<Type, Type>.CreateStaticCache();

        /// <summary>
        /// 根据列信息、源类型和目标类型获取值转换委托。
        /// </summary>
        /// <param name="mapper">映射器集合，可为 null。</param>
        /// <param name="pc">列信息，可为 null。</param>
        /// <param name="srcType">源类型。</param>
        /// <param name="dstType">目标类型。</param>
        /// <returns>值转换委托，若无需转换则返回 null。</returns>
        public static Func<object, object> GetConverter(IMapperCollection mapper, PocoColumn pc, Type srcType, Type dstType)
        {
            Func<object, object> converter = null;

            // Get converter from the mapper
            if (mapper != null)
            {
                converter = pc != null && pc.MemberInfoData != null ? mapper.FindFromDbConverter(pc.MemberInfoData.MemberInfo, srcType) : mapper.FindFromDbConverter(dstType, srcType);
                if (converter != null)
                    return converter;
            }

            if (pc != null && pc.SerializedColumn && (pc.ColumnSerializer != null || mapper?.ColumnSerializer != null))
            {
                var serializer = pc.ColumnSerializer ?? mapper.ColumnSerializer;
                if (typeof(System.IO.Stream).IsAssignableFrom(srcType))
                    converter = src => serializer.Deserialize(StreamToBytes(src), dstType);
                else
                    converter = src => serializer.Deserialize(src, dstType);
                return converter;
            }

            // Standard DateTime->Utc mapper
            if (pc != null && pc.ForceToUtc && srcType == typeof(DateTime) && (dstType == typeof(DateTime) || dstType == typeof(DateTime?)))
            {
                converter = src => new DateTime(((DateTime) src).Ticks, DateTimeKind.Utc);
                return converter;
            }

#if NET6_0_OR_GREATER
            if (srcType == typeof(DateTime) && (dstType == typeof(DateOnly) || dstType == typeof(DateOnly?)))
            {
                converter = src => DateOnly.FromDateTime((DateTime)src);
                return converter;
            }
#endif

            // Forced type conversion including integral types -> enum
            var underlyingType = UnderlyingTypes.Get(dstType, () => Nullable.GetUnderlyingType(dstType));
            if (dstType.GetTypeInfo().IsEnum || (underlyingType != null && underlyingType.GetTypeInfo().IsEnum))
            {
                if (srcType == typeof(string))
                {
                    converter = src => EnumMapper.EnumFromString((underlyingType ?? dstType), (string)src);
                    return converter;
                }

                if (IsIntegralType(srcType))
                {
                    converter = src => Enum.ToObject((underlyingType ?? dstType), src);
                    return converter;
                }
            }
            else if (srcType == typeof(string) && (dstType == typeof(Guid) || dstType == typeof(Guid?)))
            {
                converter = src => Guid.Parse((string)src);
            }
            else if (dstType == typeof(byte[]) && typeof(System.IO.Stream).IsAssignableFrom(srcType))
            {
                converter = src => StreamToBytes(src);
            }
            else if ((!pc?.ValueObjectColumn ?? true) && !dstType.IsAssignableFrom(srcType))
            {
                converter = src => Convert.ChangeType(src, (underlyingType ?? dstType), null);
            }

            return converter;
        }

        static bool IsIntegralType(Type t)
        {
            //var tc = Type.GetTypeCode(t);
            //return tc >= TypeCode.SByte && tc <= TypeCode.UInt64;
            //Not available for now
            return new[]
            {
                typeof(SByte), typeof(Byte),
                typeof(Int16), typeof(UInt16),
                typeof(Int32), typeof(UInt32),
                typeof(Int64), typeof(UInt64)
            }.Contains(t);
        }

        /// <summary>
        /// 把数据库驱动返回的 Stream（如 DuckDB.NET 的 UnmanagedMemoryStream）转为 byte[]。
        /// 已是 byte[] 时原样返回，供 BLOB/序列化列读取统一使用。
        /// </summary>
        /// <param name="src">驱动返回的原始值。</param>
        /// <returns>byte[] 数据。</returns>
        static byte[] StreamToBytes(object src)
        {
            if (src is byte[] bytes) {
                return bytes;
            }
            using (var ms = new System.IO.MemoryStream())
            {
                ((System.IO.Stream)src).CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 获取指定类型的默认值。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <returns>值类型返回其默认实例，引用类型返回 null。</returns>
        public static object GetDefault(Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
