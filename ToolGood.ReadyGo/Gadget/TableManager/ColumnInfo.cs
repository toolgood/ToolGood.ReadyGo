using System;
using System.Reflection;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.Internals;
using AnsiString = ToolGood.ReadyGo.NPoco.AnsiString;

namespace ToolGood.ReadyGo.Gadget.TableManager
{
    /// <summary>
    /// 列信息
    /// </summary>
    public class ColumnInfo
    {
        private ColumnInfo()
        { }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName;

        /// <summary>
        /// 列注释
        /// </summary>
        public string Comment;

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue;

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required;

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType;

        /// <summary>
        /// 字段长度
        /// </summary>
        public string FieldLength;

        /// <summary>
        /// 是否为文本类型
        /// </summary>
        public bool IsText;

        /// <summary>
        /// 是否为中等文本类型
        /// </summary>
        public bool IsMediumText;

        /// <summary>
        /// 是否为长文本类型
        /// </summary>
        public bool IsLongText;

        internal static ColumnInfo FromProperty(PropertyInfo pi)
        {
            if (pi.CanRead == false || pi.CanWrite == false) return null;
            if (Types.IsAllowType(pi.PropertyType) == false) return null;
            var a = pi.GetCustomAttributes(typeof(IgnoreAttribute), true);
            if (a.Length > 0) return null;
            a = pi.GetCustomAttributes(typeof(ResultColumnAttribute), true);
            if (a.Length > 0) return null;

            ColumnInfo ci = new ColumnInfo {
                PropertyType = pi.PropertyType
            };

            a = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
            ci.ColumnName = a.Length == 0 ? pi.Name : (a[0] as ColumnAttribute).Name;
            if (string.IsNullOrEmpty(ci.ColumnName)) ci.ColumnName = pi.Name;
            ci.Comment = a.Length == 0 ? null : (a[0] as ColumnAttribute).Comment;

            a = pi.GetCustomAttributes(typeof(DefaultValueAttribute), true);
            ci.DefaultValue = a.Length == 0 ? null : (a[0] as DefaultValueAttribute).DefaultValue;

            a = pi.GetCustomAttributes(typeof(FieldLengthAttribute), true);
            if (a.Length > 0) {
                ci.IsText = (a[0] as FieldLengthAttribute).IsText;
                ci.IsMediumText = (a[0] as FieldLengthAttribute).IsMediumText;
                ci.IsLongText = (a[0] as FieldLengthAttribute).IsLongText;
                ci.FieldLength = (a[0] as FieldLengthAttribute).FieldLength;
            }
            var atts = pi.GetCustomAttributes(typeof(RequiredAttribute), true);
            if (atts.Length > 0) {
                ci.Required = (atts[0] as RequiredAttribute).Required;
            } else {
                if (pi.PropertyType == typeof(string) || pi.PropertyType == typeof(AnsiString)) {
                    ci.Required = false;
                } else {
                    ci.Required = Types.IsNullType(ci.PropertyType) == false;
                }
            }
            ci.PropertyType = Types.GetBaseType(ci.PropertyType);
            return ci;
        }
    }
}
