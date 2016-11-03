using System;
using System.Reflection;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.Poco;

namespace ToolGood.ReadyGo.SqlBuilding.TableCreate
{
    public class Column
    {
        public string ColumnName { get; private set; }
        public string Comment { get; private set; }

        public string DefaultValue { get; private set; }
        public bool Required { get; private set; }


        public Type PropertyType { get; private set; }

        public string FieldLength { get; private set; }
        public bool IsText { get; private set; }

        internal static Column FromProperty(PropertyInfo pi)
        {
            if (pi.CanRead == false || pi.CanWrite == false) return null;
            if (ColumnType.IsAllowType(pi.PropertyType) == false) return null;
            var a = pi.GetCustomAttributes(typeof(IgnoreAttribute), true);
            if (a.Length > 0) return null;
            a = pi.GetCustomAttributes(typeof(ResultColumnAttribute), true);
            if (a.Length > 0) return null;

            Column ci = new Column();
            ci.PropertyType = pi.PropertyType;

            a = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
            ci.ColumnName = a.Length == 0 ? pi.Name : (a[0] as ColumnAttribute).Name;
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
                ci.Required = ColumnType.IsNullType(ci.PropertyType) == false;
            }
            ci.PropertyType = ColumnType.GetBaseType(ci.PropertyType);

            return ci;
        }
    }
}