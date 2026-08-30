using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco.RowMappers
{
    /// <summary>
    /// 将查询结果行映射到 POCO 对象属性/字段的默认行映射器。
    /// </summary>
    public class PropertyMapper : RowMapper
    {
        private List<GroupResult<PosName>> _groupedNames;
        private MapPlan _mapPlan;
        private bool _mappingOntoExistingInstance;

        /// <summary>
        /// 判断该映射器是否适用；作为默认映射器，始终返回 true。
        /// </summary>
        /// <param name="pocoData">POCO 元数据。</param>
        /// <returns>始终返回 true。</returns>
        public override bool ShouldMap(PocoData pocoData)
        {
            return true;
        }

        /// <summary>
        /// 初始化映射器，对结果集列进行分组并构建映射计划。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="pocoData">POCO 元数据。</param>
        public override void Init(DbDataReader dataReader, PocoData pocoData)
        {
            var fields = GetColumnNames(dataReader, pocoData);

            _groupedNames = fields
                .GroupByMany(x => x.Name, PocoData.Separator)
                .ToList();

            _mapPlan = BuildMapPlan(dataReader, pocoData);
        }

        /// <summary>
        /// 将当前数据行映射到目标 POCO 实例并填充其成员值。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="context">行映射上下文。</param>
        /// <returns>映射后的目标实例。</returns>
        public override object Map(DbDataReader dataReader, RowMapperContext context)
        {
            if (context.Instance == null)
            {
                context.Instance = context.PocoData.CreateObject(dataReader);
                if (context.Instance == null)
                    throw new Exception(string.Format("Cannot create POCO '{0}'. It may be an interface or abstract class without a Mapper factory.", context.Type.FullName));
            }
            else
            {
                _mappingOntoExistingInstance = true;
            }

            object[] values = new object[dataReader.FieldCount];
            dataReader.GetValues(values);
            _mapPlan(dataReader, values, context.Instance);

            var result = context.Instance as IOnLoaded;
            if (result != null)
            {
                result.OnLoaded();
            }

            return context.Instance;
        }

        /// <summary>
        /// 表示将数据行值写入目标实例的映射计划委托。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <param name="values">当前行的原始值数组。</param>
        /// <param name="instance">目标实例。</param>
        /// <returns>是否成功写入了值。</returns>
        public delegate bool MapPlan(DbDataReader dataReader, object[] values, object instance);

        private MapPlan BuildMapPlan(DbDataReader dataReader, PocoData pocoData)
        {
            var plans = _groupedNames.SelectMany(x => BuildMapPlans(x, dataReader, pocoData, pocoData.Members)).ToArray();
            return (reader, values, instance) =>
            {
                foreach (MapPlan plan in plans)
                {
                    plan(reader, values, instance);
                }
                return true;
            };
        }

        private IEnumerable<MapPlan> BuildMapPlans(GroupResult<PosName> groupedName, DbDataReader dataReader, PocoData pocoData, List<PocoMember> pocoMembers)
        {
            // find pocomember by property name
            var pocoMember = pocoMembers.FirstOrDefault(x => IsEqual(groupedName.Item, x.Name, x.PocoColumn?.ExactColumnNameMatch ?? false) 
                                       || string.Equals(groupedName.Item, x.PocoColumn?.ColumnAlias, StringComparison.OrdinalIgnoreCase));

            if (pocoMember == null)
            {
                yield break;
            }

            if (groupedName.SubItems.Any())
            {
                var memberInfoType = pocoMember.MemberInfoData.MemberType;
                if (memberInfoType.IsAClass() || pocoMember.IsDynamic)
                {
                    var children = PocoDataBuilder.IsDictionaryType(memberInfoType)
                        ? CreateDynamicDictionaryPocoMembers(groupedName.SubItems, pocoData, memberInfoType)
                        : pocoMember.PocoMemberChildren;

                    var subPlans = groupedName.SubItems.SelectMany(x => BuildMapPlans(x, dataReader, pocoData, children)).ToArray();

                    yield return (reader, values, instance) =>
                    {
                        var newObject = pocoMember.IsList ? pocoMember.Create(dataReader) : (pocoMember.GetValue(instance) ?? pocoMember.Create(dataReader));

                        var shouldSetNestedObject = false;
                        foreach (var subPlan in subPlans)
                        {
                            shouldSetNestedObject |= subPlan(reader, values, newObject);
                        }

                        if (shouldSetNestedObject)
                        {
                            if (pocoMember.IsList)
                            {
                                var list = pocoMember.CreateList();
                                list.Add(newObject);
                                newObject = list;
                            }

                            pocoMember.SetValue(instance, newObject);
                            return true;
                        }
                        return false;
                    };
                }
            }
            else
            {
                var destType = pocoMember.MemberInfoData.MemberType;
                var defaultValue = MappingHelper.GetDefault(destType);
                var converter = GetConverter(pocoData, pocoMember.PocoColumn, dataReader.GetFieldType(groupedName.Key.Pos), destType);
                yield return (reader, values, instance) => MapValue(groupedName, values, converter, instance, pocoMember.PocoColumn, defaultValue);
            }
        }

        /// <summary>
        /// 判断列名与成员名是否相等；非精确匹配时忽略下划线。
        /// </summary>
        /// <param name="name">列名。</param>
        /// <param name="value">成员名（或列别名）。</param>
        /// <param name="exactMatch">是否要求精确匹配。</param>
        /// <returns>若相等则返回 true，否则返回 false。</returns>
        public static bool IsEqual(string name, string value, bool exactMatch)
        {
            if (value is null)
                return false;

            return string.Equals(value, name, StringComparison.OrdinalIgnoreCase)
                || (!exactMatch && string.Equals(value, name.Replace("_", ""), StringComparison.OrdinalIgnoreCase));
        }

        private bool MapValue(GroupResult<PosName> posName, object[] values, Func<object, object> converter, object instance, PocoColumn pocoColumn, object defaultValue)
        {
            var value = values[posName.Key.Pos];
            if (!Equals(value, DBNull.Value))
            {
                if (converter == null)
                {
                    // 部分驱动声明的字段类型与实际返回类型不一致（如 Firebird 的 DECIMAL(9,0) 返回 Int32），
                    // 按成员类型做兜底转换，避免直接强转失败（如 Int32 -> Decimal）。
                    var memberType = Nullable.GetUnderlyingType(pocoColumn.MemberInfoData.MemberType) ?? pocoColumn.MemberInfoData.MemberType;
                    if (value != null && !memberType.IsInstanceOfType(value))
                    {
                        value = Convert.ChangeType(value, memberType, null);
                    }
                }
                pocoColumn.SetValue(instance, converter != null ? converter(value) : value);
                return true;
            }

            if (_mappingOntoExistingInstance && defaultValue == null)
            {
                pocoColumn.SetValue(instance, null);
            }

            return false;
        }

        private static List<PocoMember> CreateDynamicDictionaryPocoMembers(IEnumerable<GroupResult<PosName>> subItems, PocoData pocoData, Type type)
        {
            var isDict = type != typeof(object);
            var dataType = isDict ? type.GetGenericArguments().Last() : type;
            
            return subItems.Select(x =>
            {
                var member = new DynamicPocoMember
                {
                    MemberInfoData = new MemberInfoData(x.Item, dataType, type),
                    PocoColumn = new ExpandoColumn
                    {
                        ColumnName = x.Item
                    }
                };

                if (isDict)
                {
                    var pocoDataBuilder = new PocoDataBuilder(dataType, pocoData.Mapper);
                    member.PocoMemberChildren = pocoDataBuilder.GetPocoMembers(pocoDataBuilder.GetColumnInfos(dataType), new List<MemberInfo>()).Select(plan => plan(pocoData.TableInfo)).ToList();
                    member.SetDynamicMemberAccessor(new FastCreate(dataType, pocoData.Mapper));
                }

                return (PocoMember)member;

            }).ToList();
        }
    }
}