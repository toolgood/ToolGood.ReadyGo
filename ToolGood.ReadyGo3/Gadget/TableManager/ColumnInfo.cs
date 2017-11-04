using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ToolGood.ReadyGo3.Attributes;

namespace ToolGood.ReadyGo3.Gadget.TableManager
{
    public class ColumnInfo
    {
        public string ColumnName;
        public string Comment;

        public string DefaultValue;
        public bool Required;


        public Type PropertyType;

        public string FieldLength;
        public bool IsText;

        internal static ColumnInfo FromProperty(PropertyInfo pi)
        {
            if (pi.CanRead == false || pi.CanWrite == false) return null;
            if (IsAllowType(pi.PropertyType) == false) return null;
            var a = pi.GetCustomAttributes(typeof(IgnoreAttribute), true);
            if (a.Length > 0) return null;
            a = pi.GetCustomAttributes(typeof(ResultColumnAttribute), true);
            if (a.Length > 0) return null;

            ColumnInfo ci = new ColumnInfo();
            ci.PropertyType = pi.PropertyType;

            a = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
            ci.ColumnName = a.Length == 0 ? pi.Name : (a[0] as ColumnAttribute).Name;
            if (string.IsNullOrEmpty(ci.ColumnName)) ci.ColumnName = pi.Name;
            ci.Comment = a.Length == 0 ? null : (a[0] as ColumnAttribute).Comment;

            a = pi.GetCustomAttributes(typeof(DefaultValueAttribute), true);
            ci.DefaultValue = a.Length == 0 ? null : (a[0] as DefaultValueAttribute).DefaultValue;


            a = pi.GetCustomAttributes(typeof(FieldLengthAttribute), true);
            if (a.Length > 0) {
                ci.IsText = (a[0] as FieldLengthAttribute).IsText;
                ci.FieldLength = (a[0] as FieldLengthAttribute).FieldLength;
            }
            var atts = pi.GetCustomAttributes(typeof(RequiredAttribute), true);
            if (atts.Length > 0) {
                ci.Required = (atts[0] as RequiredAttribute).Required;
            } else {
                if (pi.PropertyType == typeof(string) || pi.PropertyType == typeof(AnsiString)) {
                    ci.Required = false;
                } else {
                    ci.Required = IsNullType(ci.PropertyType) == false;
                }
            }
            ci.PropertyType = GetBaseType(ci.PropertyType);

            return ci;
        }
        private static bool IsAllowType(Type type)
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

        private static bool IsNullType(Type type)
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

        private static Type GetBaseType(Type type)
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
