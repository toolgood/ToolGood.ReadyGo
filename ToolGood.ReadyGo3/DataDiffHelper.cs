using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using ToolGood.ReadyGo3;
using ToolGood.ReadyGo3.Attributes;
using ToolGood.ReadyGo3.DataDiffer.JsonDiffer;
using ToolGood.ReadyGo3.DataDiffer.YamlToJson;

namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// 数据变动转成文本 帮助类
    /// </summary>
    public static class DataDiffHelper
    {
        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="right">新数据</param>
        /// <returns></returns>
        public static string Diff<T>(T right) where T : class
        {
            DataDiffTypeInfo myTypeInfo = new DataDiffTypeInfo(typeof(T));
            return myTypeInfo.DiffMessage(right);
        }
        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="right">新数据</param>
        /// <param name="sqlHelper"></param>
        /// <returns></returns>
        public static string Diff<T>(T right, SqlHelper sqlHelper) where T : class
        {
            DataDiffTypeInfo myTypeInfo = new DataDiffTypeInfo(typeof(T));
            myTypeInfo.SetEnumNameFromDatabase(sqlHelper);
            return myTypeInfo.DiffMessage(right);
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="left">原数据</param>
        /// <param name="right">新数据</param>
        /// <returns></returns>
        public static string Diff<T>(T left, T right) where T : class
        {
            DataDiffTypeInfo myTypeInfo = new DataDiffTypeInfo(typeof(T));
            return myTypeInfo.DiffMessage(left, right);
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="left">原数据</param>
        /// <param name="right">新数据</param>
        /// <param name="sqlHelper"></param>
        /// <returns></returns>
        public static string Diff<T>(T left, T right, SqlHelper sqlHelper) where T : class
        {
            DataDiffTypeInfo myTypeInfo = new DataDiffTypeInfo(typeof(T));
            myTypeInfo.SetEnumNameFromDatabase(sqlHelper);
            return myTypeInfo.DiffMessage(left, right);
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="lefts">原数据</param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public static string Diff(string name, List<string> lefts, List<string> rights)
        {
            lefts.RemoveAll(x => string.IsNullOrEmpty(x));
            rights.RemoveAll(x => string.IsNullOrEmpty(x));
            var removes = lefts.Except(rights).ToList();
            var adds = rights.Except(lefts).ToList();

            if (removes.Count == 0 && adds.Count == 0) { return ""; }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(name);
            stringBuilder.Append('：');
            stringBuilder.AppendJoin('|', lefts);
            stringBuilder.Append("->");
            stringBuilder.AppendJoin('|', rights);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="lefts">原数据</param>
        /// <param name="rights">新数据</param>
        /// <returns></returns>
        public static string Diff<T>(string name, List<T> lefts, List<T> rights) where T : struct
        {
            var removes = lefts.Except(rights).ToList();
            var adds = rights.Except(lefts).ToList();

            if (removes.Count == 0 && adds.Count == 0) { return ""; }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(name);
            stringBuilder.Append('：');
            stringBuilder.AppendJoin('|', lefts);
            stringBuilder.Append("->");
            stringBuilder.AppendJoin('|', rights);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="lefts">原数据</param>
        /// <param name="rights">新数据</param>
        /// <param name="dict">字典</param>
        /// <returns></returns>
        public static string Diff<T>(string name, List<T> lefts, List<T> rights, Dictionary<T, string> dict) where T : struct
        {
            var removes = lefts.Except(rights).ToList();
            var adds = rights.Except(lefts).ToList();
            if (removes.Count == 0 && adds.Count == 0) { return ""; }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(name);
            stringBuilder.Append('：');
            for (int i = 0; i < lefts.Count; i++) {
                if (i > 0) { stringBuilder.Append('|'); }
                stringBuilder.Append(lefts[i]);
                if (dict.TryGetValue(lefts[i], out string n)) {
                    if (string.IsNullOrEmpty(n) == false) {
                        stringBuilder.Append('=');
                        stringBuilder.Append(n);
                    }
                }
            }
            stringBuilder.Append("->");
            for (int i = 0; i < removes.Count; i++) {
                if (i > 0) { stringBuilder.Append('|'); }
                stringBuilder.Append(removes[i]);
                if (dict.TryGetValue(removes[i], out string n)) {
                    if (string.IsNullOrEmpty(n) == false) {
                        stringBuilder.Append('=');
                        stringBuilder.Append(n);
                    }
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="lefts">原数据</param>
        /// <param name="rights">新数据</param>
        /// <param name="func"></param>
        /// <param name="dict">字典</param>
        /// <returns></returns>
        public static string Diff<T, T1>(string name, List<T> lefts, List<T> rights, Func<T, T1> func, Dictionary<T1, string> dict)
            where T : class
            where T1 : struct
        {
            var left = new List<T1>(lefts.Count);
            foreach (var item in lefts) {
                left.Add(func(item));
            }
            var right = new List<T1>(rights.Count);
            foreach (var item in rights) {
                right.Add(func(item));
            }
            return Diff(name, left, right, dict);
        }
        /// <summary>
        /// json格式 差异
        /// </summary>
        /// <param name="left">原数据</param>
        /// <param name="right">新数据</param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public static string JsonDiff(string left, string right, Formatting formatting = Formatting.None)
        {
            var j1 = JToken.Parse(left);
            var j2 = JToken.Parse(right);

            var diff = JsonDifferentiator.Differentiate(j2, j1);
            return diff.ToString(formatting);
        }
        /// <summary>
        /// yaml格式 差异
        /// </summary>
        /// <param name="left">原数据</param>
        /// <param name="right">新数据</param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public static string YamlDiff(string left, string right, Formatting formatting = Formatting.None)
        {
            var leftStr = StringHelper.ToJson(left);
            var j1 = JToken.Parse(leftStr);

            var rightStr = StringHelper.ToJson(right);
            var j2 = JToken.Parse(rightStr);

            var diff = JsonDifferentiator.Differentiate(j2, j1);
            return diff.ToString(formatting);
        }
    }

    internal class DataDiffTypeInfo
    {
        public string Name { get; set; }
        public DataDiffPropertyInfo IdPropertyInfo { get; set; }
        public List<DataDiffPropertyInfo> PropertyInfos { get; set; }

        public DataDiffTypeInfo(Type type)
        {
            PropertyInfos = new List<DataDiffPropertyInfo>();

            var cas = type.GetCustomAttributes();
            foreach (var ca in cas) {
                if (ca is DataNameAttribute attribute) {
                    Name = attribute.DisplayName;
                    break;
                }
            }

            var ps = type.GetProperties();
            foreach (var p in ps) {
                if (p.Name.Equals("id", StringComparison.OrdinalIgnoreCase)) {
                    DataDiffPropertyInfo myPropertyInfo = new DataDiffPropertyInfo();
                    myPropertyInfo.Property = p;
                    IdPropertyInfo = myPropertyInfo;
                    continue;
                }
                var ass = p.GetCustomAttributes();
                foreach (var a in ass) {
                    if (a is DataEnumSqlAttribute dataEnumSql) {
                        DataDiffPropertyInfo myPropertyInfo = new DataDiffPropertyInfo();
                        myPropertyInfo.Property = p;
                        myPropertyInfo.DisplayName = dataEnumSql.DisplayName;
                        myPropertyInfo.Sql = dataEnumSql.Sql;
                        PropertyInfos.Add(myPropertyInfo);
                    } else if (a is DataEnumAttribute dataEnum) {
                        DataDiffPropertyInfo myPropertyInfo = new DataDiffPropertyInfo();
                        myPropertyInfo.Property = p;
                        myPropertyInfo.DisplayName = dataEnum.DisplayName;
                        myPropertyInfo.EnumNames = new Dictionary<string, string>();
                        for (int i = 0; i < dataEnum.EnumName.Length; i++) {
                            myPropertyInfo.EnumNames[i.ToString()] = dataEnum.EnumName[i];
                        }
                        PropertyInfos.Add(myPropertyInfo);
                    } else if (a is DataNameAttribute dataName) {
                        DataDiffPropertyInfo myPropertyInfo = new DataDiffPropertyInfo();
                        myPropertyInfo.Property = p;
                        myPropertyInfo.DisplayName = dataName.DisplayName;
                        if (p.PropertyType.IsEnum) {
                            myPropertyInfo.EnumNames = GetDescriptions(p.PropertyType);
                        }
                        PropertyInfos.Add(myPropertyInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Enum  获取枚举值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetDescriptions(Type type)
        {
            var typeHandle = type.TypeHandle;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var enumList = Enum.GetValues(type);
            foreach (int item in enumList) {
                var field = type.GetField(Enum.GetName(type, item));
                if (field == null) continue;

                var attr = field.GetCustomAttributes(typeof(DataNameAttribute), false) as DataNameAttribute[];
                if ((attr != null) && (attr.Length == 1)) {
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
                    var table = helper.ExecuteDataTable(item.Sql);
                    item.EnumNames = new Dictionary<string, string>();
                    foreach (DataRow row in table.Rows) {
                        var key = row[0].ToString().Trim();
                        var value = row[1].ToString().Trim();
                        item.EnumNames[key] = value;
                    }
                } catch (Exception) { }
            }
        }

        public string DiffMessage<T>(T left, T right)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (IdPropertyInfo != null) {
                if (IdPropertyInfo.IsChange(left, right)) {
                    var id = IdPropertyInfo.Property.GetValue(right);
                    stringBuilder.Append($"新增[{Name ?? "id"}]{id}");
                    foreach (var propertyInfo in PropertyInfos) {
                        propertyInfo.NewValue(right, stringBuilder);
                    }
                } else {
                    var id = IdPropertyInfo.Property.GetValue(right);
                    stringBuilder.Append($"修改[{Name ?? "id"}]{id}");
                    foreach (var propertyInfo in PropertyInfos) {
                        propertyInfo.Diff(left, right, stringBuilder);
                    }
                }
            } else {
                if (string.IsNullOrEmpty(Name)) {
                    stringBuilder.Append($"修改");
                } else {
                    stringBuilder.Append($"修改[{Name}]");
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
                stringBuilder.Append($"新增[{Name ?? "id"}]{id}");
                foreach (var propertyInfo in PropertyInfos) {
                    propertyInfo.NewValue(right, stringBuilder);
                }
            } else {
                if (string.IsNullOrEmpty(Name)) {
                    stringBuilder.Append($"新增");
                } else {
                    stringBuilder.Append($"新增[{Name}]");
                }
                foreach (var propertyInfo in PropertyInfos) {
                    propertyInfo.NewValue(right, stringBuilder);
                }
            }
            return stringBuilder.ToString();
        }
    }

    internal class DataDiffPropertyInfo
    {
        public PropertyInfo Property { get; set; }
        public string DisplayName { get; set; }
        public string Sql { get; set; }
        public Dictionary<string, string> EnumNames { get; set; }

        public bool IsChange<T>(T left, T right)
        {
            var leftValue = Property.GetValue(left);
            var rightValue = Property.GetValue(right);
            if (leftValue.Equals(rightValue)) { return false; }
            return true;
        }

        public void NewValue(object right, StringBuilder stringBuilder)
        {
            var rightValue = Property.GetValue(right);
            if (null == rightValue) { return; }

            if (stringBuilder.Length != 0) { stringBuilder.Append('，'); }
            if (string.IsNullOrEmpty(DisplayName) == false) {
                stringBuilder.Append(DisplayName);
                stringBuilder.Append('：');
            }

            if (EnumNames == null) {
                if (Property.PropertyType == typeof(DateTime)) {
                    stringBuilder.Append($"{(DateTime)rightValue:yyyy-MM-dd HH:mm:ss}");
                } else if (Property.PropertyType == typeof(DateTimeOffset)) {
                    stringBuilder.Append($"{(DateTimeOffset)rightValue:yyyy-MM-dd HH:mm:ss}");
                } else if (Property.PropertyType == typeof(TimeSpan)) {
                    stringBuilder.Append($"{(TimeSpan)rightValue:d HH:mm:ss}");
                } else if (Property.PropertyType == typeof(DateTime?)) {
                    stringBuilder.Append($"{(DateTime?)rightValue:yyyy-MM-dd HH:mm:ss}");
                } else if (Property.PropertyType == typeof(DateTimeOffset?)) {
                    stringBuilder.Append($"{(DateTimeOffset?)rightValue:yyyy-MM-dd HH:mm:ss}");
                } else if (Property.PropertyType == typeof(TimeSpan?)) {
                    stringBuilder.Append($"{(TimeSpan?)rightValue:d HH:mm:ss}");
#if NET8_0_OR_GREATER
                } else if (Property.PropertyType == typeof(TimeOnly)) {
                    stringBuilder.Append($"{(TimeOnly)rightValue:HH:mm:ss}");
                } else if (Property.PropertyType == typeof(DateOnly)) {
                    stringBuilder.Append($"{(DateOnly)rightValue:yyyy-MM-dd}");
                } else if (Property.PropertyType == typeof(TimeOnly?)) {
                    stringBuilder.Append($"{(TimeOnly?)rightValue:HH:mm:ss}");
                } else if (Property.PropertyType == typeof(DateOnly?)) {
                    stringBuilder.Append($"{(DateOnly?)rightValue:yyyy-MM-dd}");
#endif
                } else {
                    stringBuilder.Append($"{rightValue ?? "(NULL)"}");
                }
                return;
            }
            if (Property.PropertyType.IsEnum) {
                stringBuilder.Append(rightValue);
                if (EnumNames.TryGetValue(rightValue.ToString(), out string rv)) {
                    if (string.IsNullOrEmpty(rv) == false) {
                        stringBuilder.Append('=');
                        stringBuilder.Append(rv);
                    }
                }
            } else if (Property.PropertyType == typeof(bool) || Property.PropertyType == typeof(bool?)) {
                if (null == rightValue) {
                    stringBuilder.Append("(NULL)");
                } else if (EnumNames.TryGetValue(((bool)rightValue ? "1" : "0"), out string rv) && string.IsNullOrEmpty(rv) == false) {
                    stringBuilder.Append(rv);
                } else {
                    stringBuilder.Append(rightValue);
                }
            } else if (Property.PropertyType == typeof(string)) {
                if (rightValue == null) {
                    stringBuilder.Append("(NULL)");
                } else {
                    var rs = rightValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < rs.Length; i++) {
                        if (i > 0) { stringBuilder.Append('|'); }
                        var r = rs[i];
                        stringBuilder.Append(r);
                        if (EnumNames.TryGetValue(r.ToString(), out string rv)) {
                            if (string.IsNullOrEmpty(rv) == false) {
                                stringBuilder.Append('=');
                                stringBuilder.Append(rv);
                            }
                        }
                    }
                }
            } else if (Property.PropertyType == typeof(byte)
                || Property.PropertyType == typeof(ushort)
                || Property.PropertyType == typeof(uint)
                || Property.PropertyType == typeof(ulong)
                || Property.PropertyType == typeof(sbyte)
                || Property.PropertyType == typeof(short)
                || Property.PropertyType == typeof(int)
                || Property.PropertyType == typeof(long)

                || Property.PropertyType == typeof(byte?)
                || Property.PropertyType == typeof(ushort?)
                || Property.PropertyType == typeof(uint?)
                || Property.PropertyType == typeof(ulong?)
                || Property.PropertyType == typeof(sbyte?)
                || Property.PropertyType == typeof(short?)
                || Property.PropertyType == typeof(int?)
                || Property.PropertyType == typeof(long?)
                ) {
                if (rightValue == null) {
                    stringBuilder.Append("(NULL)");
                } else {
                    stringBuilder.Append(rightValue);
                    if (EnumNames.TryGetValue(rightValue.ToString(), out string rv)) {
                        if (string.IsNullOrEmpty(rv) == false) {
                            stringBuilder.Append('=');
                            stringBuilder.Append(rv);
                        }
                    }
                }
            } else {
                stringBuilder.Append($"{rightValue ?? "(NULL)"}");
            }
        }

        public void Diff<T>(T left, T right, StringBuilder stringBuilder)
        {
            var leftValue = Property.GetValue(left);
            var rightValue = Property.GetValue(right);

            if (null == leftValue && null == rightValue) { return; }
            if (null != leftValue && leftValue.Equals(rightValue)) { return; }
            if (stringBuilder.Length != 0) { stringBuilder.Append('，'); }
            if (string.IsNullOrEmpty(DisplayName) == false) {
                stringBuilder.Append(DisplayName);
                stringBuilder.Append('：');
            }

            if (EnumNames == null) {
                if (Property.PropertyType == typeof(DateTime)) {
                    stringBuilder.Append($"{(DateTime)leftValue:yyyy-MM-dd HH:mm:ss}->{(DateTime)rightValue:yyyy-MM-dd HH:mm:ss}");
                } else if (Property.PropertyType == typeof(DateTimeOffset)) {
                    stringBuilder.Append($"{(DateTimeOffset)leftValue:yyyy-MM-dd HH:mm:ss}->{(DateTimeOffset)rightValue:yyyy-MM-dd HH:mm:ss}");
                } else if (Property.PropertyType == typeof(TimeSpan)) {
                    stringBuilder.Append($"{(TimeSpan)leftValue:d HH:mm:ss}->{(TimeSpan)rightValue:d HH:mm:ss}");
                } else if (Property.PropertyType == typeof(DateTime?)) {
                    stringBuilder.Append($"{(DateTime?)leftValue:yyyy-MM-dd HH:mm:ss}->{(DateTime?)rightValue:yyyy-MM-dd HH:mm:ss}");
                } else if (Property.PropertyType == typeof(DateTimeOffset?)) {
                    stringBuilder.Append($"{(DateTimeOffset?)leftValue:yyyy-MM-dd HH:mm:ss}->{(DateTimeOffset?)rightValue:yyyy-MM-dd HH:mm:ss}");
                } else if (Property.PropertyType == typeof(TimeSpan?)) {
                    stringBuilder.Append($"{(TimeSpan?)leftValue:d HH:mm:ss}->{(TimeSpan?)rightValue:d HH:mm:ss}");
#if NET8_0_OR_GREATER
                } else if (Property.PropertyType == typeof(TimeOnly)) {
                    stringBuilder.Append($"{(TimeOnly)leftValue:HH:mm:ss}->{(TimeOnly)rightValue:HH:mm:ss}");
                } else if (Property.PropertyType == typeof(DateOnly)) {
                    stringBuilder.Append($"{(DateOnly)leftValue:yyyy-MM-dd}->{(DateOnly)rightValue:yyyy-MM-dd}");
                } else if (Property.PropertyType == typeof(TimeOnly?)) {
                    stringBuilder.Append($"{(TimeOnly?)leftValue:HH:mm:ss}->{(TimeOnly?)rightValue:HH:mm:ss}");
                } else if (Property.PropertyType == typeof(DateOnly?)) {
                    stringBuilder.Append($"{(DateOnly?)leftValue:yyyy-MM-dd}->{(DateOnly?)rightValue:yyyy-MM-dd}");
#endif
                } else {
                    stringBuilder.Append($"{leftValue ?? "(NULL)"}->{rightValue ?? "(NULL)"}");
                }
                return;
            }
            if (Property.PropertyType.IsEnum) {
                stringBuilder.Append(leftValue);
                if (EnumNames.TryGetValue(leftValue.ToString(), out string lv)) {
                    if (string.IsNullOrEmpty(lv) == false) {
                        stringBuilder.Append('=');
                        stringBuilder.Append(lv);
                    }
                }
                stringBuilder.Append("->");

                stringBuilder.Append(rightValue);
                if (EnumNames.TryGetValue(rightValue.ToString(), out string rv)) {
                    if (string.IsNullOrEmpty(rv) == false) {
                        stringBuilder.Append(rv);
                    }
                }
            } else if (Property.PropertyType == typeof(bool) || Property.PropertyType == typeof(bool?)) {
                if (null == leftValue) {
                    stringBuilder.Append("(NULL)");
                } else if (EnumNames.TryGetValue(((bool)leftValue ? "1" : "0"), out string lv) && string.IsNullOrEmpty(lv) == false) {
                    stringBuilder.Append(lv);
                } else {
                    stringBuilder.Append(leftValue);
                }

                stringBuilder.Append("->");

                if (null == rightValue) {
                    stringBuilder.Append("(NULL)");
                } else if (EnumNames.TryGetValue(((bool)rightValue ? "1" : "0"), out string rv) && string.IsNullOrEmpty(rv) == false) {
                    stringBuilder.Append(rv);
                } else {
                    stringBuilder.Append(rightValue);
                }

            } else if (Property.PropertyType == typeof(string)) {
                if (null == leftValue) {
                    stringBuilder.Append("(NULL)");
                } else {
                    var ls = leftValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < ls.Length; i++) {
                        if (i > 0) { stringBuilder.Append('|'); }
                        var l = ls[i];
                        stringBuilder.Append(l);
                        if (EnumNames.TryGetValue(l.ToString(), out string lv)) {
                            if (string.IsNullOrEmpty(lv) == false) {
                                stringBuilder.Append('=');
                                stringBuilder.Append(lv);
                            }
                        }
                    }
                }
                stringBuilder.Append("->");

                if (null == rightValue) {
                    stringBuilder.Append("(NULL)");
                } else {
                    var rs = rightValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < rs.Length; i++) {
                        if (i > 0) { stringBuilder.Append('|'); }
                        var r = rs[i];
                        stringBuilder.Append(r);
                        if (EnumNames.TryGetValue(r.ToString(), out string rv)) {
                            if (string.IsNullOrEmpty(rv) == false) {
                                stringBuilder.Append('=');
                                stringBuilder.Append(rv);
                            }
                        }
                    }
                }
            } else if (Property.PropertyType == typeof(byte)
                || Property.PropertyType == typeof(ushort)
                || Property.PropertyType == typeof(uint)
                || Property.PropertyType == typeof(ulong)
                || Property.PropertyType == typeof(sbyte)
                || Property.PropertyType == typeof(short)
                || Property.PropertyType == typeof(int)
                || Property.PropertyType == typeof(long)

                || Property.PropertyType == typeof(byte?)
                || Property.PropertyType == typeof(ushort?)
                || Property.PropertyType == typeof(uint?)
                || Property.PropertyType == typeof(ulong?)
                || Property.PropertyType == typeof(sbyte?)
                || Property.PropertyType == typeof(short?)
                || Property.PropertyType == typeof(int?)
                || Property.PropertyType == typeof(long?)
                ) {
                if (leftValue == null) {
                    stringBuilder.Append("(NULL)");
                } else {
                    stringBuilder.Append(leftValue);
                    if (EnumNames.TryGetValue(leftValue.ToString(), out string lv)) {
                        if (string.IsNullOrEmpty(lv) == false) {
                            stringBuilder.Append('=');
                            stringBuilder.Append(lv);
                        }
                    }
                }

                stringBuilder.Append("->");

                if (rightValue == null) {
                    stringBuilder.Append("(NULL)");
                } else {
                    stringBuilder.Append(rightValue);
                    if (EnumNames.TryGetValue(rightValue.ToString(), out string rv)) {
                        if (string.IsNullOrEmpty(rv) == false) {
                            stringBuilder.Append('=');
                            stringBuilder.Append(rv);
                        }
                    }
                }
            } else {
                stringBuilder.Append($"{leftValue ?? "(NULL)"}->{rightValue ?? "(NULL)"}");
            }
        }
    }

}