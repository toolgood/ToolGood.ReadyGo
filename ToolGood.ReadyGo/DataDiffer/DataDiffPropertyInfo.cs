using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ToolGood.ReadyGo
{
    internal class DataDiffPropertyInfo
    {
        private const string NullValueText = "(NULL)";
        private const string Separator = "，";
        private const string NameSeparator = "：";
        private const string Arrow = "->";
        private const string Pipe = "|";
        private const string EqualsSign = "=";
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private const string TimeSpanFormat = "d HH:mm:ss";
        private const string TimeOnlyFormat = "HH:mm:ss";
        private const string DateOnlyFormat = "yyyy-MM-dd";

        private readonly Func<object, object> _getter;

        public DataDiffPropertyInfo(PropertyInfo property)
            : this(property, CreateGetter(property))
        {
        }

        private DataDiffPropertyInfo(PropertyInfo property, Func<object, object> getter)
        {
            Property = property;
            _getter = getter;
        }

        public PropertyInfo Property { get; }
        public string DisplayName { get; set; }
        public string Sql { get; set; }
        public Dictionary<string, string> EnumNames { get; set; }

        public object GetValue(object instance) => _getter(instance);

        public DataDiffPropertyInfo Clone()
        {
            return new DataDiffPropertyInfo(Property, _getter)
            {
                DisplayName = DisplayName,
                Sql = Sql,
                EnumNames = EnumNames
            };
        }

        private static Func<object, object> CreateGetter(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var convert = Expression.Convert(instance, property.DeclaringType);
            var access = Expression.Property(convert, property);
            var box = Expression.Convert(access, typeof(object));
            return Expression.Lambda<Func<object, object>>(box, instance).Compile();
        }

        public bool IsChange<T>(T left, T right)
        {
            var leftValue = _getter(left);
            var rightValue = _getter(right);
            if (object.Equals(leftValue, rightValue)) { return false; }
            return true;
        }

        public void NewValue(object right, StringBuilder stringBuilder)
        {
            var rightValue = _getter(right);
            if (null == rightValue) { return; }
            if (string.Equals(rightValue as string, "")) { return; }

            AppendPrefix(stringBuilder);
            AppendValue(stringBuilder, rightValue);
        }

        public void Diff<T>(T left, T right, StringBuilder stringBuilder)
        {
            var leftValue = _getter(left);
            var rightValue = _getter(right);

            if (null == leftValue && null == rightValue) { return; }
            if (null != leftValue && leftValue.Equals(rightValue)) { return; }
            if (string.Equals(leftValue as string, "") && null == rightValue) { return; }
            if (null == leftValue && string.Equals(rightValue as string, "")) { return; }

            AppendPrefix(stringBuilder);
            AppendValue(stringBuilder, leftValue);
            stringBuilder.Append(Arrow);
            AppendValue(stringBuilder, rightValue);
        }

        private void AppendPrefix(StringBuilder stringBuilder)
        {
            if (stringBuilder.Length != 0) { stringBuilder.Append(Separator); }
            if (string.IsNullOrEmpty(DisplayName) == false) {
                stringBuilder.Append(DisplayName);
                stringBuilder.Append(NameSeparator);
            }
        }

        private void AppendValue(StringBuilder stringBuilder, object value)
        {
            if (EnumNames == null) {
                AppendRawValue(stringBuilder, value);
            } else {
                AppendMappedValue(stringBuilder, value);
            }
        }

        private void AppendRawValue(StringBuilder stringBuilder, object value)
        {
            if (Property.PropertyType == typeof(DateTime)) {
                stringBuilder.Append(((DateTime)value).ToString(DateTimeFormat));
            } else if (Property.PropertyType == typeof(DateTimeOffset)) {
                stringBuilder.Append(((DateTimeOffset)value).ToString(DateTimeFormat));
            } else if (Property.PropertyType == typeof(TimeSpan)) {
                stringBuilder.Append(((TimeSpan)value).ToString(TimeSpanFormat));
            } else if (Property.PropertyType == typeof(DateTime?)) {
                var v = (DateTime?)value;
                stringBuilder.Append(v.HasValue ? v.Value.ToString(DateTimeFormat) : "");
            } else if (Property.PropertyType == typeof(DateTimeOffset?)) {
                var v = (DateTimeOffset?)value;
                stringBuilder.Append(v.HasValue ? v.Value.ToString(DateTimeFormat) : "");
            } else if (Property.PropertyType == typeof(TimeSpan?)) {
                var v = (TimeSpan?)value;
                stringBuilder.Append(v.HasValue ? v.Value.ToString(TimeSpanFormat) : "");
#if NET8_0_OR_GREATER
            } else if (Property.PropertyType == typeof(TimeOnly)) {
                stringBuilder.Append(((TimeOnly)value).ToString(TimeOnlyFormat));
            } else if (Property.PropertyType == typeof(DateOnly)) {
                stringBuilder.Append(((DateOnly)value).ToString(DateOnlyFormat));
            } else if (Property.PropertyType == typeof(TimeOnly?)) {
                var v = (TimeOnly?)value;
                stringBuilder.Append(v.HasValue ? v.Value.ToString(TimeOnlyFormat) : "");
            } else if (Property.PropertyType == typeof(DateOnly?)) {
                var v = (DateOnly?)value;
                stringBuilder.Append(v.HasValue ? v.Value.ToString(DateOnlyFormat) : "");
#endif
            } else {
                stringBuilder.Append(value ?? NullValueText);
            }
        }

        private void AppendMappedValue(StringBuilder stringBuilder, object value)
        {
            if (Property.PropertyType.IsEnum) {
                stringBuilder.Append(value);
                if (TryGetEnumName(value, out string enumName) && string.IsNullOrEmpty(enumName) == false) {
                    stringBuilder.Append(EqualsSign);
                    stringBuilder.Append(enumName);
                }
            } else if (Property.PropertyType == typeof(bool) || Property.PropertyType == typeof(bool?)) {
                if (null == value) {
                    stringBuilder.Append(NullValueText);
                } else if (EnumNames.TryGetValue(((bool)value ? "1" : "0"), out string boolName) && string.IsNullOrEmpty(boolName) == false) {
                    stringBuilder.Append(boolName);
                } else {
                    stringBuilder.Append(value);
                }
            } else if (Property.PropertyType == typeof(string)) {
                if (value == null) {
                    stringBuilder.Append(NullValueText);
                } else {
                    var items = value.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < items.Length; i++) {
                        if (i > 0) { stringBuilder.Append(Pipe); }
                        stringBuilder.Append(items[i]);
                        if (EnumNames.TryGetValue(items[i], out string itemName) && string.IsNullOrEmpty(itemName) == false) {
                            stringBuilder.Append(EqualsSign);
                            stringBuilder.Append(itemName);
                        }
                    }
                }
            } else if (IsIntegerType) {
                if (value == null) {
                    stringBuilder.Append(NullValueText);
                } else {
                    stringBuilder.Append(value);
                    if (EnumNames.TryGetValue(value.ToString(), out string intName) && string.IsNullOrEmpty(intName) == false) {
                        stringBuilder.Append(EqualsSign);
                        stringBuilder.Append(intName);
                    }
                }
            } else {
                stringBuilder.Append(value ?? NullValueText);
            }
        }

        /// <summary>
        /// 获取枚举对应的显示名称。
        /// <see cref="Attributes.DataEnumAttribute"/> 使用枚举底层数值作为键（从 0 开始），
        /// 而 <see cref="Attributes.DataNameAttribute"/> 使用枚举名称作为键，因此两者都要尝试。
        /// </summary>
        private bool TryGetEnumName(object value, out string enumName)
        {
            if (value != null && EnumNames.TryGetValue(Convert.ToInt64(value).ToString(), out enumName)) {
                return true;
            }

            return EnumNames.TryGetValue(value?.ToString(), out enumName);
        }

        private bool IsIntegerType
        {
            get
            {
                var type = Property.PropertyType;
                return type == typeof(byte) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong)
                    || type == typeof(sbyte) || type == typeof(short) || type == typeof(int) || type == typeof(long)
                    || type == typeof(byte?) || type == typeof(ushort?) || type == typeof(uint?) || type == typeof(ulong?)
                    || type == typeof(sbyte?) || type == typeof(short?) || type == typeof(int?) || type == typeof(long?);
            }
        }
    }
}
