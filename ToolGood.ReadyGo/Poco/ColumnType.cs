using System;
using System.Data;

namespace ToolGood.ReadyGo.Poco
{
    internal static class ColumnType
    {
        public static bool IsAllowType(Type type)
        {
            if (type == null) return false;
            if (type.IsEnum) return true;
            if (type == typeof(byte[])) return true;
            if (type == typeof(sbyte[])) return true;
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
        }

        public static bool IsNumericType(Type type)
        {
            if (type != null) {
                if (type.IsEnum) {
                    return true;
                }
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = type.GetGenericArguments()[0];

                switch (Type.GetTypeCode(type)) {
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
            }
            return false;
        }

        public static bool IsNullType( Type type)
        {
            if (type == null) return true;
            if (type.IsEnum) return false;

            if (type.IsGenericType) {
                if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                    return true;
                }
            }
            return false;
        }

        public static Type GetBaseType(Type type)
        {
            if (type.IsEnum) return type;

            if (type.IsGenericType) {
                if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                    type = type.GetGenericArguments()[0];
                }
            }
            return type;
        }

    }
}