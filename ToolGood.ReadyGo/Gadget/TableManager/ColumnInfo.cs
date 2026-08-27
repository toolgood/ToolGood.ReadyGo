using System;
using System.Linq;
using System.Reflection;
using ToolGood.ReadyGo.Attributes;
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
        public string ColumnName { get; internal set; }

        /// <summary>
        /// 列注释
        /// </summary>
        public string Comment { get; internal set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; internal set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required { get; internal set; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get; internal set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public string FieldLength { get; internal set; }

        /// <summary>
        /// 是否为文本类型
        /// </summary>
        public bool IsText { get; internal set; }

        /// <summary>
        /// 是否为中等文本类型
        /// </summary>
        public bool IsMediumText { get; internal set; }

        /// <summary>
        /// 是否为长文本类型
        /// </summary>
        public bool IsLongText { get; internal set; }

        /// <summary>
        /// 是否为序列化列（带 Serializer 的 SerializedColumnAttribute 子类，如 [DictionaryUintUint]）
        /// </summary>
        public bool IsSerialized { get; internal set; }

        /// <summary>
        /// 是否为序列化后保存为 int 的列（[Numeric2Int] / [Date2Int] / [Enum2Int] / [Bool2Int]，建表时保存为 int）
        /// </summary>
        public bool IsSerializedAsInt { get; internal set; }

        /// <summary>
        /// 是否为序列化后保存为 long 的列（[Numeric2Long] / [DateTime2Long] / [Enum2Long] / [DateTime2Timestamp]，建表时保存为 long）
        /// </summary>
        public bool IsSerializedAsLong { get; internal set; }



        /// <summary>
        /// 是否为序列化后保存为 string 的列（[Bool2String] / [Date2String] / [DateTime2String] / [NumericArray2String] / [StringArray2String]，建表时保存为文本）
        /// </summary>
        public bool IsSerializedAsString { get; internal set; }

        internal static ColumnInfo FromProperty(PropertyInfo pi)
        {
            if (pi.CanRead == false || pi.CanWrite == false) return null;
            var isSerialized = pi.GetCustomAttributes(typeof(SerializedColumnAttribute), true)
                .OfType<SerializedColumnAttribute>()
                .Any(a => a.Serializer != null);
            if (isSerialized == false && Types.IsAllowType(pi.PropertyType) == false) return null;
            var a = pi.GetCustomAttributes(typeof(IgnoreAttribute), true);
            if (a.Length > 0) return null;
            a = pi.GetCustomAttributes(typeof(ResultColumnAttribute), true);
            if (a.Length > 0) return null;

            ColumnInfo ci = new ColumnInfo {
                PropertyType = pi.PropertyType,
                IsSerialized = isSerialized,
                IsSerializedAsInt = pi.GetCustomAttributes(typeof(Numeric2IntAttribute), true).Length > 0
                    || pi.GetCustomAttributes(typeof(Date2IntAttribute), true).Length > 0
                    || pi.GetCustomAttributes(typeof(Enum2IntAttribute), true).Length > 0
                    || pi.GetCustomAttributes(typeof(Bool2IntAttribute), true).Length > 0,
                IsSerializedAsLong = pi.GetCustomAttributes(typeof(Numeric2LongAttribute), true).Length > 0
                    || pi.GetCustomAttributes(typeof(DateTime2LongAttribute), true).Length > 0
                    || pi.GetCustomAttributes(typeof(Enum2LongAttribute), true).Length > 0
                    || pi.GetCustomAttributes(typeof(DateTime2TimestampAttribute), true).Length > 0,
                IsSerializedAsString = pi.GetCustomAttributes(typeof(Bool2StringAttribute), true).Length > 0
                    || pi.GetCustomAttributes(typeof(Date2StringAttribute), true).Length > 0
                    || pi.GetCustomAttributes(typeof(DateTime2StringAttribute), true).Length > 0
                    || pi.GetCustomAttributes(typeof(NumericArray2StringAttribute), true).Length > 0
                    || pi.GetCustomAttributes(typeof(StringArray2StringAttribute), true).Length > 0
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
