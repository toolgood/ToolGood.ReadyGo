using System;
using System.Collections.Generic;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Internals
{
    internal class Types
    {
        private static readonly Cache<Type, bool> IsAllowTypeCache = new Cache<Type, bool>();
        private static readonly Cache<Type, bool> IsNullTypeCache = new Cache<Type, bool>();
        private static readonly Cache<Type, Type> GetBaseTypeCache = new Cache<Type, Type>();

        /// <summary>
        /// 判断类型是否为受支持的数据库字段类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>若为受支持类型则返回 true，否则返回 false</returns>
        public static bool IsAllowType(Type type)
        {
            if (type == null) return false;
            return IsAllowTypeCache.Get(type, () => {
                if (type.IsEnum) return true;
                if (type == typeof(byte[])) return true;
                if (type == typeof(sbyte[])) return true;
                if (type == typeof(UInt16[])) return true;
                if (type == typeof(UInt32[])) return true;
                if (type == typeof(UInt64[])) return true;
                if (type == typeof(Int16[])) return true;
                if (type == typeof(Int32[])) return true;
                if (type == typeof(Int64[])) return true;
                if (type == typeof(Single[])) return true;
                if (type == typeof(Double[])) return true;
                if (type == typeof(bool[])) return true;

                if (type == typeof(List<byte>)) return true;
                if (type == typeof(List<sbyte>)) return true;
                if (type == typeof(List<UInt16>)) return true;
                if (type == typeof(List<UInt32>)) return true;
                if (type == typeof(List<UInt64>)) return true;
                if (type == typeof(List<Int16>)) return true;
                if (type == typeof(List<Int32>)) return true;
                if (type == typeof(List<Int64>)) return true;
                if (type == typeof(List<Single>)) return true;
                if (type == typeof(List<Double>)) return true;
                if (type == typeof(List<bool>)) return true;

                if (type.FullName == "Microsoft.SqlServer.Types.SqlGeography") return true;
                if (type.FullName == "Microsoft.SqlServer.Types.SqlGeometry") return true;

                if (type.IsGenericType) {
                    if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                        type = type.GetGenericArguments()[0];
                    } else {
                        return false;
                    }
                }

                if (type == typeof(Guid)) return true;
                if (type == typeof(AnsiString)) return true;
                if (type == typeof(TimeSpan)) return true;
                if (type == typeof(DateTimeOffset)) return true;

                var tc = Type.GetTypeCode(type);
                switch (tc) {
                    case TypeCode.Boolean:
                    case TypeCode.Byte:
                    case TypeCode.Char:
                    case TypeCode.DateTime:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Single:
                    case TypeCode.String:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;

                    case TypeCode.Object:
                    default:
                        break;
                }
                return false;
            });
        }

        public static bool IsNullType(Type type)
        {
            if (type == null) return true;
            return IsNullTypeCache.Get(type, () => {
                if (type.IsEnum) return false;
                if (type.IsGenericType) {
                    if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                        return true;
                    }
                }
                return false;
            });
        }

        /// <summary>
        /// 获取类型的基础类型（去掉 Nullable 包装）
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>基础类型</returns>
        public static Type GetBaseType(Type type)
        {
            return GetBaseTypeCache.Get(type, () => {
                if (type.IsEnum) return type;
                if (type.IsGenericType) {
                    if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                        type = type.GetGenericArguments()[0];
                    }
                }
                return type;
            });
        }
    }
}
