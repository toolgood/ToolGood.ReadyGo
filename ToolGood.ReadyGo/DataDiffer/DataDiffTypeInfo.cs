using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using ToolGood.ReadyGo.Attributes;

namespace ToolGood.ReadyGo
{
    internal class DataDiffTypeInfo
    {
        private const string IdPropertyName = "id";
        private const string IdDisplayName = "id";
        private const string AddedText = "新增";
        private const string ModifiedText = "修改";
        private const string DeletedText = "删除";

        // 缓存枚举字典查询结果，避免相同 SQL 重复执行（枚举字典通常不频繁变化）。
        private static readonly ConcurrentDictionary<string, Dictionary<string, string>> EnumNameCache = new();

        public string Name { get; set; }
        public DataDiffPropertyInfo IdPropertyInfo { get; set; }
        public List<DataDiffPropertyInfo> PropertyInfos { get; }

        public DataDiffTypeInfo(Type type)
        {
            PropertyInfos = new List<DataDiffPropertyInfo>();

            var customAttributes = type.GetCustomAttributes();
            foreach (var customAttribute in customAttributes) {
                if (customAttribute is DataNameAttribute dataName) {
                    Name = dataName.DisplayName;
                    break;
                }
            }

            var properties = type.GetProperties();
            foreach (var property in properties) {
                if (property.Name.Equals(IdPropertyName, StringComparison.OrdinalIgnoreCase)) {
                    IdPropertyInfo = new DataDiffPropertyInfo(property);
                    continue;
                }

                var propertyAttributes = property.GetCustomAttributes();
                foreach (var attribute in propertyAttributes) {
                    if (attribute is DataEnumSqlAttribute dataEnumSql) {
                        var propertyInfo = new DataDiffPropertyInfo(property);
                        propertyInfo.DisplayName = dataEnumSql.DisplayName;
                        propertyInfo.Sql = dataEnumSql.Sql;
                        PropertyInfos.Add(propertyInfo);
                    } else if (attribute is DataEnumAttribute dataEnum) {
                        var propertyInfo = new DataDiffPropertyInfo(property);
                        propertyInfo.DisplayName = dataEnum.DisplayName;
                        propertyInfo.EnumNames = new Dictionary<string, string>();
                        for (int i = 0; i < dataEnum.EnumName.Length; i++) {
                            propertyInfo.EnumNames[i.ToString()] = dataEnum.EnumName[i];
                        }
                        PropertyInfos.Add(propertyInfo);
                    } else if (attribute is DataNameAttribute dataName) {
                        var propertyInfo = new DataDiffPropertyInfo(property);
                        propertyInfo.DisplayName = dataName.DisplayName;
                        if (property.PropertyType.IsEnum) {
                            propertyInfo.EnumNames = GetDescriptions(property.PropertyType);
                        }
                        PropertyInfos.Add(propertyInfo);
                    }
                }
            }
        }

        /// <summary>
        /// 获取枚举字段上的 <see cref="DataNameAttribute"/> 显示名称。
        /// </summary>
        private Dictionary<string, string> GetDescriptions(Type type)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var enumList = Enum.GetValues(type);
            foreach (var item in enumList) {
                var name = Enum.GetName(type, item);
                if (name == null) continue;
                var field = type.GetField(name);
                if (field == null) continue;

                if (field.GetCustomAttributes(typeof(DataNameAttribute), false) is DataNameAttribute[] attr && attr.Length == 1) {
                    dict.Add(field.Name, attr[0].DisplayName);
                }
            }
            return dict;
        }

        public void SetEnumNameFromDatabase(SqlHelper helper)
        {
            foreach (var item in PropertyInfos) {
                if (string.IsNullOrEmpty(item.Sql)) { continue; }
                try {
                    if (EnumNameCache.TryGetValue(item.Sql, out var cached)) {
                        item.EnumNames = cached;
                        continue;
                    }

                    var table = helper.ExecuteDataTable(item.Sql);
                    var enumNames = new Dictionary<string, string>();
                    foreach (DataRow row in table.Rows) {
                        var key = row[0].ToString().Trim();
                        var value = row[1].ToString().Trim();
                        enumNames[key] = value;
                    }
                    EnumNameCache[item.Sql] = enumNames;
                    item.EnumNames = enumNames;
                } catch (Exception) { }
            }
        }

        public string DiffMessage<T>(T left, T right)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (IdPropertyInfo != null) {
                if (IdPropertyInfo.IsChange(left, right)) {
                    var id = IdPropertyInfo.Property.GetValue(right);
                    stringBuilder.Append($"{AddedText}[{Name ?? IdDisplayName}]{id}");
                    foreach (var propertyInfo in PropertyInfos) {
                        propertyInfo.NewValue(right, stringBuilder);
                    }
                } else {
                    var id = IdPropertyInfo.Property.GetValue(right);
                    stringBuilder.Append($"{ModifiedText}[{Name ?? IdDisplayName}]{id}");
                    foreach (var propertyInfo in PropertyInfos) {
                        propertyInfo.Diff(left, right, stringBuilder);
                    }
                }
            } else {
                if (string.IsNullOrEmpty(Name)) {
                    stringBuilder.Append(ModifiedText);
                } else {
                    stringBuilder.Append($"{ModifiedText}[{Name}]");
                }
                foreach (var propertyInfo in PropertyInfos) {
                    propertyInfo.Diff(left, right, stringBuilder);
                }
            }
            return stringBuilder.ToString();
        }

        public string DiffMessage<T>(T right)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (IdPropertyInfo != null) {
                var id = IdPropertyInfo.Property.GetValue(right);
                stringBuilder.Append($"{AddedText}[{Name ?? IdDisplayName}]{id}");
                foreach (var propertyInfo in PropertyInfos) {
                    propertyInfo.NewValue(right, stringBuilder);
                }
            } else {
                if (string.IsNullOrEmpty(Name)) {
                    stringBuilder.Append(AddedText);
                } else {
                    stringBuilder.Append($"{AddedText}[{Name}]");
                }
                foreach (var propertyInfo in PropertyInfos) {
                    propertyInfo.NewValue(right, stringBuilder);
                }
            }
            return stringBuilder.ToString();
        }

        public string DeleteMessage<T>(T left)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (IdPropertyInfo != null) {
                var id = IdPropertyInfo.Property.GetValue(left);
                stringBuilder.Append($"{DeletedText}[{Name ?? IdDisplayName}]{id}");
            } else {
                if (string.IsNullOrEmpty(Name)) {
                    stringBuilder.Append(DeletedText);
                } else {
                    stringBuilder.Append($"{DeletedText}[{Name}]");
                }
            }
            foreach (var propertyInfo in PropertyInfos) {
                propertyInfo.NewValue(left, stringBuilder);
            }
            return stringBuilder.ToString();
        }
    }
}
