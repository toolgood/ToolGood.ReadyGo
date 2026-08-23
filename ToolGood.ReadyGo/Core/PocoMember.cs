using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示实体类型中的一个成员（字段或属性），可包含子成员以支持复杂映射。
    /// </summary>
    public class PocoMember
    {
        /// <summary>
        /// 初始化 PocoMember 类的新实例。
        /// </summary>
        public PocoMember()
        {
            PocoMemberChildren = new List<PocoMember>();
            ReferenceType = ReferenceType.None;
        }

        /// <summary>
        /// 成员名称。
        /// </summary>
        public string Name
        {
            get
            {
                return MemberInfoData.Name;
            }
        }

        /// <summary>
        /// 成员信息数据。
        /// </summary>
        public MemberInfoData MemberInfoData
        {
            get;
            set;
        }

        /// <summary>
        /// 该成员对应的数据列（非复杂映射时有效）。
        /// </summary>
        public PocoColumn PocoColumn
        {
            get;
            set;
        }

        /// <summary>
        /// 该成员的子成员集合。
        /// </summary>
        public List<PocoMember> PocoMemberChildren
        {
            get;
            set;
        }

        /// <summary>
        /// 该成员的引用关系类型。
        /// </summary>
        public ReferenceType ReferenceType
        {
            get;
            set;
        }

        /// <summary>
        /// 外键引用时所指向的成员名称。
        /// </summary>
        public string ReferenceMemberName
        {
            get;
            set;
        }

        /// <summary>
        /// 指示该成员是否为列表类型。
        /// </summary>
        public bool IsList
        {
            get;
            set;
        }

        /// <summary>
        /// 指示该成员是否为动态类型。
        /// </summary>
        public bool IsDynamic
        {
            get;
            set;
        }

        /// <summary>
        /// 从根对象到该成员的成员信息链。
        /// </summary>
        public List<MemberInfo> MemberInfoChain
        {
            get;
            set;
        }

        private IFastCreate _creator;
        private MemberAccessor _memberAccessor;
        private Type _listType;

        /// <summary>
        /// 使用数据读取器创建该成员对应的对象。
        /// </summary>
        /// <param name="dataReader">数据读取器。</param>
        /// <returns>创建出的对象。</returns>
        public virtual object Create(DbDataReader dataReader)
        {
            return _creator.Create(dataReader);
        }

        /// <summary>
        /// 创建该列表类型成员对应的列表实例。
        /// </summary>
        /// <returns>列表实例。</returns>
        public IList CreateList()
        {
            var list = Activator.CreateInstance(_listType);
            return (IList)list;
        }

        /// <summary>
        /// 设置该成员的访问器、对象创建器与列表类型。
        /// </summary>
        /// <param name="memberAccessor">成员访问器。</param>
        /// <param name="fastCreate">对象创建器。</param>
        /// <param name="listType">列表类型。</param>
        public void SetMemberAccessor(MemberAccessor memberAccessor, IFastCreate fastCreate, Type listType)
        {
            _listType = listType;
            _memberAccessor = memberAccessor;
            _creator = fastCreate;
        }

        /// <summary>
        /// 设置动态成员的创建器。
        /// </summary>
        /// <param name="fastCreate">对象创建器。</param>
        public void SetDynamicMemberAccessor(IFastCreate fastCreate)
        {
            _creator = fastCreate;
        }

        /// <summary>
        /// 将值设置到目标对象的对应成员上。
        /// </summary>
        /// <param name="target">目标对象。</param>
        /// <param name="value">要设置的值。</param>
        public virtual void SetValue(object target, object value)
        {
            _memberAccessor.Set(target, value);
        }

        /// <summary>
        /// 获取目标对象上该成员的值。
        /// </summary>
        /// <param name="target">目标对象。</param>
        /// <returns>成员的值。</returns>
        public virtual object GetValue(object target)
        {
            return _memberAccessor.Get(target);
        }
    }
}
