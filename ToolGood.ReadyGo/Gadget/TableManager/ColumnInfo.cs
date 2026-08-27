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
        /// 序列化列存储形态
        /// </summary>
        public enum SerializedKind
        {
            /// <summary>
            /// 非序列化列
            /// </summary>
            None,

            /// <summary>
            /// 序列化后保存为 int（[Numeric2Int] / [Date2Int] / [Enum2Int] / [Bool2Int]）
            /// </summary>
            Int,

            /// <summary>
            /// 序列化后保存为 long（[Numeric2Long] / [DateTime2Long] / [Enum2Long] / [DateTime2Timestamp]）
            /// </summary>
            Long,

            /// <summary>
            /// 序列化后保存为字符串（[Bool2String] / [Date2String] / [DateTime2String] / [NumericArray2String] / [StringArray2String]）
            /// </summary>
            String,

            /// <summary>
            /// 序列化后保存为二进制（BLOB/bytea 等），包括裸 [SerializedColumn] 与自定义带 Serializer 的序列化列
            /// </summary>
            Bytes,
        }

        /// <summary>
        /// 是否为序列化列（声明了 SerializedColumnAttribute 或其子类；未指定 Serializer 时使用全局默认序列化器）
        /// </summary>
        public bool IsSerialized => SerializedAs != SerializedKind.None;

        /// <summary>
        /// 序列化列的存储形态
        /// </summary>
        public SerializedKind SerializedAs { get; internal set; }

        internal static ColumnInfo FromProperty(PropertyInfo pi)
        {
            if (pi.CanRead == false || pi.CanWrite == false) return null;
            var serializedColumnAttributes = pi.GetCustomAttributes(typeof(SerializedColumnAttribute), true)
                .OfType<SerializedColumnAttribute>()
                .ToArray();
            // 与 Core/ColumnInfoCreator 语义保持一致：声明了 SerializedColumnAttribute（含基类裸用）即视为序列化列，
            // 未指定 Serializer 时使用全局默认序列化器（FastJsonColumnSerializer），不能仅凭 Serializer == null 静默丢弃该列
            var isSerialized = serializedColumnAttributes.Length > 0;
            if (isSerialized == false && Types.IsAllowType(pi.PropertyType) == false) return null;
            var a = pi.GetCustomAttributes(typeof(IgnoreAttribute), true);
            if (a.Length > 0) return null;
            a = pi.GetCustomAttributes(typeof(ResultColumnAttribute), true);
            if (a.Length > 0) return null;

            ColumnInfo ci = new ColumnInfo {
                PropertyType = pi.PropertyType,
                SerializedAs = GetSerializedKind(pi, serializedColumnAttributes),
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

        /// <summary>
        /// 根据声明序列化特性，确定序列化列的存储形态
        /// </summary>
        private static SerializedKind GetSerializedKind(PropertyInfo pi, SerializedColumnAttribute[] serializedColumnAttributes)
        {
            if (serializedColumnAttributes.Length == 0) return SerializedKind.None;
            if (HasAny(pi, typeof(Numeric2IntAttribute), typeof(Date2IntAttribute), typeof(Enum2IntAttribute), typeof(Bool2IntAttribute))) return SerializedKind.Int;
            if (HasAny(pi, typeof(Numeric2LongAttribute), typeof(DateTime2LongAttribute), typeof(Enum2LongAttribute), typeof(DateTime2TimestampAttribute))) return SerializedKind.Long;
            if (HasAny(pi, typeof(Bool2StringAttribute), typeof(Date2StringAttribute), typeof(DateTime2StringAttribute), typeof(NumericArray2StringAttribute), typeof(StringArray2StringAttribute))) return SerializedKind.String;
            // 裸 [SerializedColumn] 或自定义带 Serializer 的序列化列，默认按二进制存储
            return SerializedKind.Bytes;
        }

        private static bool HasAny(PropertyInfo pi, params Type[] attributeTypes)
        {
            foreach (var attributeType in attributeTypes) {
                if (pi.GetCustomAttributes(attributeType, true).Length > 0) return true;
            }
            return false;
        }
    }
}
