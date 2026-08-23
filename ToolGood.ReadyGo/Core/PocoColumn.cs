using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示一个映射到实体成员（字段或属性）的数据列，保存列名、成员访问链以及列的各种映射行为信息。
    /// </summary>
    public class PocoColumn
    {
        /// <summary>
        /// 默认情况下是否将日期时间值强制转换为 UTC 时间。
        /// </summary>
        public static bool ForceToUTCDefault { get; set; } = true;

        /// <summary>
        /// 初始化 PocoColumn 类的新实例。
        /// </summary>
        public PocoColumn()
        {
            MemberInfoChain = new List<MemberInfo>();
        }

        /// <summary>
        /// 根据成员信息链生成该列的唯一键。
        /// </summary>
        /// <param name="memberInfoChain">组成该列的成员信息链。</param>
        /// <returns>使用分隔符连接各成员名称后得到的键字符串。</returns>
        public static string GenerateKey(IEnumerable<MemberInfo> memberInfoChain)
        {
            return string.Join(PocoData.Separator, memberInfoChain.Select(x => x.Name).ToArray());
        }

        /// <summary>
        /// 该列所属的表信息。
        /// </summary>
        public TableInfo TableInfo;
        /// <summary>
        /// 该列在数据库中的列名。
        /// </summary>
        public string ColumnName;

        /// <summary>
        /// 从根对象到该列成员的成员信息链。
        /// </summary>
        public List<MemberInfo> MemberInfoChain { get; set; }

        private string _memberInfoKey;
        /// <summary>
        /// 该列成员信息链对应的唯一键。
        /// </summary>
        public string MemberInfoKey { get { return _memberInfoKey ?? (_memberInfoKey = GenerateKey(MemberInfoChain)); } }

        /// <summary>
        /// 该列对应的成员信息数据。
        /// </summary>
        public MemberInfoData MemberInfoData { get; set; }

        /// <summary>
        /// 指示该列是否为仅查询结果列（不参与插入与更新）。
        /// </summary>
        public bool ResultColumn;
        /// <summary>
        /// 指示该列是否为版本列。
        /// </summary>
        public bool VersionColumn;
        /// <summary>
        /// 版本列的类型（数字版本或行版本）。
        /// </summary>
        public VersionColumnType VersionColumnType;
        /// <summary>
        /// 指示该列是否为计算列。
        /// </summary>
        public bool ComputedColumn;
        /// <summary>
        /// 计算列的类型。
        /// </summary>
        public ComputedColumnType ComputedColumnType;
        private Type _columnType;
        private MemberAccessor _memberAccessor;
        private List<MemberAccessor> _memberAccessorChain = new List<MemberAccessor>();
        private Action<object, object> valueObjectSetter;
        private Func<object, object> valueObjectGetter;
        private IFastCreate fastCreate;
        private bool? forceToUtc;

        /// <summary>
        /// 该列对应的数据类型；未显式指定时返回成员自身的类型。
        /// </summary>
        public Type ColumnType
        {
            get { return _columnType ?? MemberInfoData.MemberType; }
            set { _columnType = value; }
        }

        /// <summary>
        /// 指示该列的日期时间值是否强制转换为 UTC 时间；未设置时使用 ForceToUTCDefault 的值。
        /// </summary>
        public bool ForceToUtc { get => forceToUtc ?? ForceToUTCDefault; set => forceToUtc = value; }
        /// <summary>
        /// 该列的自定义序列化器。
        /// </summary>
        public IColumnSerializer ColumnSerializer { get; set; }
        /// <summary>
        /// 该列的别名。
        /// </summary>
        public string ColumnAlias { get; set; }

        /// <summary>
        /// 该列的引用关系类型（无、一对一、外键、一对多）。
        /// </summary>
        public ReferenceType ReferenceType { get; set; }
        /// <summary>
        /// 指示该列是否为序列化列。
        /// </summary>
        public bool SerializedColumn { get; set; }
        /// <summary>
        /// 指示该列是否为值对象列。
        /// </summary>
        public bool ValueObjectColumn { get; set; }
        /// <summary>
        /// 值对象列中实际保存值的属性名称。
        /// </summary>
        public string ValueObjectColumnName { get; set; }
        /// <summary>
        /// 指示该列是否要求精确匹配列名。
        /// </summary>
        public bool ExactColumnNameMatch { get; set; }

        internal void SetMemberAccessors(List<MemberAccessor> memberAccessors)
        {
            _memberAccessor = memberAccessors[memberAccessors.Count - 1];
            _memberAccessorChain = memberAccessors;
        }

        internal void SetValueObjectAccessors(IFastCreate fastCreate, Action<object, object> setter, Func<object, object> getter)
        {
            this.fastCreate = fastCreate;
            valueObjectSetter = setter;
            valueObjectGetter = getter;
        }

        /// <summary>
        /// 将指定值设置到目标对象的对应成员上。
        /// </summary>
        /// <param name="target">目标对象。</param>
        /// <param name="val">要设置的值。</param>
        public virtual void SetValue(object target, object val)
        {
            if (valueObjectGetter != null)
            {
                var property = GetRecursiveValue(target) ?? fastCreate.Create(null);
                valueObjectSetter?.Invoke(property, val);
                val = property;
            }

            _memberAccessor.Set(target, val);
        }

        /// <summary>
        /// 获取目标对象上该列成员的值。
        /// </summary>
        /// <param name="target">目标对象。</param>
        /// <returns>该列成员的值。</returns>
        public virtual object GetValue(object target)
        {
            return valueObjectGetter != null 
                ? valueObjectGetter(GetRecursiveValue(target) ?? fastCreate.Create(null)) 
                : GetRecursiveValue(target);
        }

        /// <summary>
        /// 从值对象中获取实际保存的值。
        /// </summary>
        /// <param name="valueObject">值对象实例。</param>
        /// <returns>值对象中保存的值。</returns>
        public object GetValueObjectValue(object valueObject)
        {
            return valueObjectGetter?.Invoke(valueObject);
        }

        private object GetRecursiveValue(object target)
        {
            foreach (var memberAccessor in _memberAccessorChain)
            {
                target = target == null ? null : memberAccessor.Get(target);
            }
            return target;
        }

        /// <summary>
        /// 将值转换为该列成员的目标类型。
        /// </summary>
        /// <param name="val">要转换的值。</param>
        /// <returns>转换后的值。</returns>
        public virtual object ChangeType(object val)
        {
            var type = Nullable.GetUnderlyingType(MemberInfoData.MemberType) ?? MemberInfoData.MemberType;
            return Convert.ChangeType(val, type);
        }

        /// <summary>
        /// 获取该列在目标对象上的列值，并可通过回调对值进行处理。
        /// </summary>
        /// <param name="pd">关联的 POCO 数据。</param>
        /// <param name="target">目标对象。</param>
        /// <param name="callback">可选的值处理回调，默认直接返回原值。</param>
        /// <returns>处理后的列值。</returns>
        public object GetColumnValue(PocoData pd, object target, Func<PocoColumn, object, object> callback = null)
        {
            callback = callback ?? ((_, o) => o);
            if (ReferenceType == ReferenceType.Foreign)
            {
                var member = pd.Members.Single(x => x.MemberInfoData == MemberInfoData);
                var column = member.PocoMemberChildren.SingleOrDefault(x => x.Name == member.ReferenceMemberName);
                if (column == null)
                {
                    throw new Exception(string.Format("Could not find member on '{0}' with name '{1}'", member.MemberInfoData.MemberType, member.ReferenceMemberName));
                }
                return callback(column.PocoColumn, column.PocoColumn.GetValue(target));
            }
            else
            {
                return callback(this, GetValue(target));
            }
        }
    }
}
