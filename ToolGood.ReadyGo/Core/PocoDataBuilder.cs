using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.NPoco.RowMappers;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示已初始化的 POCO 数据构建器，可据此构建表信息或 POCO 数据。
    /// </summary>
    public interface InitializedPocoDataBuilder
    {
        /// <summary>
        /// 构建表信息。
        /// </summary>
        /// <returns>构建出的表信息。</returns>
        TableInfo BuildTableInfo();
        /// <summary>
        /// 构建 POCO 数据。
        /// </summary>
        /// <returns>构建出的 POCO 数据。</returns>
        PocoData Build();
    }

    /// <summary>
    /// 根据实体类型构建 POCO 映射数据（表信息、列与成员结构）。
    /// </summary>
    public class PocoDataBuilder : InitializedPocoDataBuilder
    {
        private readonly Cache<string, Type> _aliasToType = Cache<string, Type>.CreateStaticCache();
        private IFastCreate _generator;

        /// <summary>
        /// 正在构建映射的实体类型。
        /// </summary>
        protected Type Type { get; set; }
        private IMapperCollection Mapper { get; set; }

        private List<PocoMemberPlan> _memberPlans { get; set; }
        private TableInfoPlan _tableInfoPlan { get; set; }

        /// <summary>
        /// 根据表信息构建成员的计划委托。
        /// </summary>
        /// <param name="tableInfo">表信息。</param>
        /// <returns>构建出的成员。</returns>
        public delegate PocoMember PocoMemberPlan(TableInfo tableInfo);
        /// <summary>
        /// 构建表信息的计划委托。
        /// </summary>
        /// <returns>构建出的表信息。</returns>
        protected delegate TableInfo TableInfoPlan();

        /// <summary>
        /// 初始化 PocoDataBuilder 类的新实例。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <param name="mapper">映射器集合。</param>
        public PocoDataBuilder(Type type, IMapperCollection mapper)
        {
            Type = type;
            Mapper = mapper;
        }

        /// <summary>
        /// 初始化构建器，收集列信息并生成表信息计划与成员计划。
        /// </summary>
        /// <returns>当前构建器实例。</returns>
        public InitializedPocoDataBuilder Init()
        {
            var memberInfos = new List<MemberInfo>();
            var columnInfos = GetColumnInfos(Type);

            // init the generator
            _generator = new FastCreate(Type, Mapper);

            // Get table info plan
            _tableInfoPlan = GetTableInfo(Type, columnInfos, memberInfos);

            // Get pocomember plan
            _memberPlans = GetPocoMembers(columnInfos, memberInfos).ToList();

            return this;
        }

        /// <summary>
        /// 判断是否应将某个私有成员纳入列映射（默认仅当带有 ColumnAttribute 特性时）。
        /// </summary>
        /// <param name="mi">成员信息。</param>
        /// <param name="t">所属类型。</param>
        /// <returns>若应纳入则返回 true，否则返回 false。</returns>
        protected virtual bool ShouldIncludePrivateColumn(MemberInfo mi, Type t) => mi.GetCustomAttribute<ColumnAttribute>() != null;

        /// <summary>
        /// 获取指定类型的列信息集合（包含公开字段、属性以及符合条件的私有成员）。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <returns>列信息数组。</returns>
        public ColumnInfo[] GetColumnInfos(Type type)
        {
            return ReflectionUtils.GetFieldsAndPropertiesForClasses(type)
                .Where(x => !IsDictionaryType(x.DeclaringType))
                .Concat(ReflectionUtils.GetPrivatePropertiesForClasses(type)
                    .Where(x => ShouldIncludePrivateColumn(x, type)))
                .Select(x => GetColumnInfo(x, type))
                .ToArray();
        }

        /// <summary>
        /// 判断指定类型是否为字典类型。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>若是字典类型则返回 true，否则返回 false。</returns>
        public static bool IsDictionaryType(Type type)
        {
            return new[] {typeof(object), typeof(IDictionary<string, object>), typeof(Dictionary<string, object>), typeof(OrderedDictionary)}.Contains(type)
                || (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>) && type.GetGenericArguments().First() == typeof(string));
        }

        TableInfo InitializedPocoDataBuilder.BuildTableInfo()
        {
            return _tableInfoPlan();
        }

        PocoData InitializedPocoDataBuilder.Build()
        {
            var pocoData = new PocoData(Type, Mapper, _generator);

            pocoData.TableInfo = _tableInfoPlan();
            pocoData.Members = _memberPlans.Select(plan => plan(pocoData.TableInfo)).ToList();
            pocoData.Columns = GetPocoColumns(pocoData.Members).Where(x => x != null).ToDictionary(x => x.ColumnName, x => x, StringComparer.OrdinalIgnoreCase);

            //Build column list for automatic select
            pocoData.QueryColumns = pocoData.Columns.Where(c => !c.Value.ResultColumn && c.Value.ReferenceType == ReferenceType.None).ToArray();

            return pocoData;
        }

        /// <summary>
        /// 生成表信息计划（根据类型解析表名、主键等并生成表别名）。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <param name="columnInfos">列信息集合。</param>
        /// <param name="memberInfos">成员信息集合。</param>
        /// <returns>表信息计划委托。</returns>
        protected virtual TableInfoPlan GetTableInfo(Type type, ColumnInfo[] columnInfos, List<MemberInfo> memberInfos)
        {
            var alias = CreateAlias(type.Name, type);
            var tableInfo = TableInfoCreator.FromPoco(type);
            tableInfo.AutoAlias = alias;
            return () => tableInfo.Clone();
        }

        /// <summary>
        /// 根据成员信息生成对应的列信息。
        /// </summary>
        /// <param name="mi">成员信息。</param>
        /// <param name="type">所属类型。</param>
        /// <returns>列信息。</returns>
        protected virtual ColumnInfo GetColumnInfo(MemberInfo mi, Type type)
        {
            return ColumnInfoCreator.FromMemberInfo(mi);
        }

        private static IEnumerable<PocoColumn> GetPocoColumns(IEnumerable<PocoMember> members)
        {
            foreach (var member in members)
            {
                switch (member.ReferenceType)
                {
                    case ReferenceType.Foreign:
                        yield return member.PocoColumn;
                        break;
                    case ReferenceType.None:
                    {
                        yield return member.PocoColumn;
                        foreach (var pocoMemberChild in GetPocoColumns(member.PocoMemberChildren))
                        {
                            yield return pocoMemberChild;
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 递归生成成员计划集合。
        /// </summary>
        /// <param name="columnInfos">列信息集合。</param>
        /// <param name="memberInfos">成员信息集合。</param>
        /// <param name="prefix">列名前缀。</param>
        /// <returns>成员计划集合。</returns>
        public IEnumerable<PocoMemberPlan> GetPocoMembers(ColumnInfo[] columnInfos, List<MemberInfo> memberInfos, string prefix = null)
        {
            var capturedMembers = memberInfos.ToArray();
            var capturedPrefix = prefix;
            foreach (var columnInfo in columnInfos)
            {
                if (columnInfo.IgnoreColumn)
                    continue;

                var memberInfoType = columnInfo.MemberInfo.GetMemberInfoType();
                if (columnInfo.ReferenceType == ReferenceType.Many)
                {
                    var genericArguments = memberInfoType.GetGenericArguments();
                    memberInfoType = genericArguments.Any() 
                        ? genericArguments.First() 
                        : memberInfoType.GetTypeWithGenericTypeDefinitionOf(typeof(IList<>)).GetGenericArguments().First();
                }

                var childrenPlans = new List<PocoMemberPlan>();
                TableInfoPlan childTableInfoPlan = null;
                var members = new List<MemberInfo>(capturedMembers) { columnInfo.MemberInfo };

                if (columnInfo.ComplexMapping || columnInfo.ReferenceType != ReferenceType.None)
                {
                    if (capturedMembers.GroupBy(x => x.GetMemberInfoType()).Any(x => x.Count() >= 2))
                    {
                        continue;
                    }

                    var childColumnInfos = GetColumnInfos(memberInfoType);

                    if (columnInfo.ReferenceType != ReferenceType.None)
                    {
                        childTableInfoPlan = GetTableInfo(memberInfoType, childColumnInfos, members);
                    }

                    var newPrefix = JoinStrings(capturedPrefix, columnInfo.ReferenceType != ReferenceType.None ? "" : (columnInfo.ComplexPrefix ?? columnInfo.MemberInfo.Name));

                    childrenPlans.AddRange(GetPocoMembers(childColumnInfos, members, newPrefix));
                }

                MemberInfo capturedMemberInfo = columnInfo.MemberInfo;
                ColumnInfo capturedColumnInfo = columnInfo;

                var accessors = GetMemberAccessors(members);
                var memberType = capturedMemberInfo.GetMemberInfoType();
                var isList = IsList(capturedMemberInfo);
                var listType = GetListType(memberType, isList);
                var isDynamic = capturedMemberInfo.IsDynamic();
                var fastCreate = GetFastCreate(memberType, Mapper, isList, isDynamic);
                var columnName = GetColumnName(capturedPrefix, capturedColumnInfo.ColumnName ?? capturedMemberInfo.Name);
                var memberInfoData = new MemberInfoData(capturedMemberInfo);
                
                yield return tableInfo =>
                {
                    var pc = new PocoColumn
                    {
                        ReferenceType = capturedColumnInfo.ReferenceType,
                        TableInfo = tableInfo,
                        MemberInfoData = memberInfoData,
                        MemberInfoChain = members,
                        ColumnName = columnName,
                        ResultColumn = capturedColumnInfo.ResultColumn,
                        ExactColumnNameMatch = capturedColumnInfo.ExactColumnNameMatch,
                        ForceToUtc = capturedColumnInfo.ForceToUtc,
                        ColumnSerializer = capturedColumnInfo.ColumnSerializer,
                        ComputedColumn = capturedColumnInfo.ComputedColumn,
                        ComputedColumnType = capturedColumnInfo.ComputedColumnType,
                        ColumnType = capturedColumnInfo.ColumnType,
                        ColumnAlias = capturedColumnInfo.ColumnAlias,
                        VersionColumn = capturedColumnInfo.VersionColumn,
                        VersionColumnType = capturedColumnInfo.VersionColumnType,
                        SerializedColumn = capturedColumnInfo.SerializedColumn,
                        ValueObjectColumn = capturedColumnInfo.ValueObjectColumn,
                    };

                    if (pc.ValueObjectColumn)
                    {
                        SetupValueObject(pc, fastCreate);
                    }

                    pc.SetMemberAccessors(accessors);

                    var childrenTableInfo = childTableInfoPlan == null ? tableInfo : childTableInfoPlan();
                    var children = childrenPlans.Select(plan => plan(childrenTableInfo)).ToList();

                    // Cascade ResultColumn down
                    foreach (var child in children.Where(child => child.PocoColumn != null && pc.ResultColumn))
                    {
                        child.PocoColumn.ResultColumn = true;
                    }

                    var pocoMember = new PocoMember()
                    {
                        MemberInfoData = memberInfoData,
                        MemberInfoChain = members,
                        IsList = isList,
                        IsDynamic = isDynamic,
                        PocoColumn = capturedColumnInfo.ComplexMapping ? null : pc,
                        ReferenceType = capturedColumnInfo.ReferenceType,
                        ReferenceMemberName = capturedColumnInfo.ReferenceMemberName,
                        PocoMemberChildren = children,
                    };

                    pocoMember.SetMemberAccessor(accessors[accessors.Count - 1], fastCreate, listType);

                    return pocoMember;
                };
            }
        }

        private static void SetupValueObject(PocoColumn pc, FastCreate fastCreate)
        {
            var memberName = "Value";
            var hasIValueObject = pc.MemberInfoData.MemberType.GetTypeWithGenericTypeDefinitionOf(typeof(IValueObject<>));
            MemberInfo property = string.IsNullOrEmpty(pc.ValueObjectColumnName)
                ? pc.MemberInfoData.MemberType.GetProperties().FirstOrDefault(x => x.Name.IndexOf(memberName, StringComparison.OrdinalIgnoreCase) >= 0)
                  ?? pc.MemberInfoData.MemberType.GetProperties().First()
                : ReflectionUtils.GetFieldsAndProperties(pc.MemberInfoData.MemberType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).First(x => x.Name == pc.ValueObjectColumnName);
            var type = hasIValueObject != null ? hasIValueObject.GetGenericArguments().First() : property.GetMemberInfoType();
            var memberAccessor = hasIValueObject != null ? new MemberAccessor(typeof(IValueObject<>).MakeGenericType(type), memberName) : new MemberAccessor(pc.MemberInfoData.MemberType, property.Name);
            pc.SetValueObjectAccessors(fastCreate, (target, value) => memberAccessor.Set(target, value), target => memberAccessor.Get(target));
            pc.ColumnType = type;
        }

        private static FastCreate GetFastCreate(Type memberType, IMapperCollection mapperCollection, bool isList, bool isDynamic)
        {
            return memberType.IsAClass() || isDynamic
                       ? (new FastCreate(isList
                            ? (memberType.GetGenericArguments().Any() ? memberType.GetGenericArguments().First() : memberType.GetTypeWithGenericTypeDefinitionOf(typeof(IList<>)).GetGenericArguments().First())
                            : memberType, mapperCollection))
                       : null;
        }

        private static Type GetListType(Type memberType, bool isList)
        {
            return isList
                ? (memberType.GetGenericArguments().Length > 0
                    ? typeof(List<>).MakeGenericType(memberType.GetGenericArguments().First())
                    : memberType)
                : null;
        }

        /// <summary>
        /// 根据成员信息链生成成员访问器列表。
        /// </summary>
        /// <param name="memberInfos">成员信息链。</param>
        /// <returns>成员访问器列表。</returns>
        public List<MemberAccessor> GetMemberAccessors(IEnumerable<MemberInfo> memberInfos)
        {
            return memberInfos
                .Select(memberInfo => new MemberAccessor(memberInfo.DeclaringType, memberInfo.Name))
                .ToList();
        }

        /// <summary>
        /// 判断成员类型是否为 IList 列表类型（且不是数组）。
        /// </summary>
        /// <param name="mi">成员信息。</param>
        /// <returns>若是列表类型则返回 true，否则返回 false。</returns>
        public static bool IsList(MemberInfo mi)
        {
            return mi.GetMemberInfoType().IsOfGenericType(typeof(IList<>)) && !mi.GetMemberInfoType().IsArray;
        }

        /// <summary>
        /// 拼接前缀与列名生成最终列名。
        /// </summary>
        /// <param name="prefix">列名前缀。</param>
        /// <param name="columnName">列名。</param>
        /// <returns>拼接后的列名。</returns>
        protected virtual string GetColumnName(string prefix, string columnName)
        {
            return JoinStrings(prefix, columnName);
        }

        /// <summary>
        /// 使用分隔符连接前缀与结尾字符串。
        /// </summary>
        /// <param name="prefix">前缀。</param>
        /// <param name="end">结尾字符串。</param>
        /// <returns>连接后的字符串。</returns>
        public static string JoinStrings(string prefix, string end)
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(prefix))
                list.Add(prefix);
            if (!string.IsNullOrEmpty(end))
                list.Add(end);
            return string.Join(PocoData.Separator, list.ToArray());
        }

        /// <summary>
        /// 根据类型名生成唯一表别名。
        /// </summary>
        /// <param name="typeName">类型名称。</param>
        /// <param name="typeIn">类型。</param>
        /// <returns>生成的别名。</returns>
        protected string CreateAlias(string typeName, Type typeIn)
        {
            string alias;
            int i = 0;
            bool result = false;
            string name = string.Join(string.Empty, typeName.BreakUpCamelCase().Split(' ').Select(x => x.Substring(0, 1)).ToArray());
            do
            {
                alias = name + (i == 0 ? string.Empty : i.ToString());
                i++;

                if (_aliasToType.AddIfNotExists(alias, typeIn))
                {
                    continue;
                }

                result = true;
            } while (result == false);

            return alias;
        }
    }
}
