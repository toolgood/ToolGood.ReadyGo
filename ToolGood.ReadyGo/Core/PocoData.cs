using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 保存某个实体类型的映射元数据，包括表信息、列集合与成员集合等。
    /// </summary>
    public class PocoData
    {
        /// <summary>
        /// 列名之间的分隔符。
        /// </summary>
        public static string Separator = "__";

        /// <summary>
        /// 该数据对应的实体类型。
        /// </summary>
        public Type Type { get; private set; }
        /// <summary>
        /// 使用的映射器集合。
        /// </summary>
        public IMapperCollection Mapper { get; private set; }

        /// <summary>
        /// 用于自动查询的列集合。
        /// </summary>
        public KeyValuePair<string, PocoColumn>[] QueryColumns { get; protected internal set; }
        /// <summary>
        /// 表信息。
        /// </summary>
        public TableInfo TableInfo { get; protected internal set; }
        /// <summary>
        /// 列名到列映射的字典（忽略大小写）。
        /// </summary>
        public Dictionary<string, PocoColumn> Columns { get; protected internal set; }
        /// <summary>
        /// 成员集合。
        /// </summary>
        public List<PocoMember> Members { get; protected internal set; }
        private IFastCreate CreateDelegate { get; }

        // This is used on a per query basis, if we have cache PocoData then this will need to change.
        /// <summary>
        /// 指示该数据是否由查询动态生成。
        /// </summary>
        public bool IsQueryGenerated { get; set; }

        /// <summary>
        /// 初始化 PocoData 类的新实例。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <param name="mapper">映射器集合。</param>
        /// <param name="creator">对象创建器。</param>
        public PocoData(Type type, IMapperCollection mapper, IFastCreate creator)
        {
            CreateDelegate = creator;
            Type = type;
            Mapper = mapper;
        }
        
        /// <summary>
        /// 获取指定对象的主键值数组。
        /// </summary>
        /// <param name="obj">实体对象。</param>
        /// <returns>主键值数组。</returns>
        public object[] GetPrimaryKeyValues(object obj)
        {
            return PrimaryKeyValues(obj);
        }

        /// <summary>
        /// 获取所有成员（包括各级子成员）。
        /// </summary>
        /// <returns>所有成员的枚举。</returns>
        public IEnumerable<PocoMember> GetAllMembers()
        {
            return GetAllMembers(Members);
        }

        private IEnumerable<PocoMember> GetAllMembers(IEnumerable<PocoMember> pocoMembers)
        {
            foreach (var member in pocoMembers)
            {
                yield return member;
                foreach(var childmember in GetAllMembers(member.PocoMemberChildren))
                {
                    yield return childmember;
                }
            }
        }

        private Func<object, object[]> _primaryKeyValues;
        private Func<object, object[]> PrimaryKeyValues
        {
            get
            {
                if (_primaryKeyValues == null)
                {
                    var multiplePrimaryKeysNames = TableInfo.PrimaryKey.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                    var members = multiplePrimaryKeysNames
                        .Select(x => Members.FirstOrDefault(y => y.PocoColumn != null
                                && y.ReferenceType == ReferenceType.None
                                && string.Equals(x, y.PocoColumn.ColumnName, StringComparison.OrdinalIgnoreCase)))
                        .Where(x => x != null);
                    _primaryKeyValues = obj => members.Select(x => x.PocoColumn.GetValue(obj)).ToArray();
                }
                return _primaryKeyValues;
            }
        }

        /// <summary>
        /// 使用数据读取器创建实体对象。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <returns>创建的实体对象。</returns>
        public object CreateObject(DbDataReader dataReader)
        {
            return CreateDelegate.Create(dataReader);
        }
    }
}
