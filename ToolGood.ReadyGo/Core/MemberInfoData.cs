using System;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示成员的元数据信息，用于按名称、成员类型和声明类型比较成员是否相等。
    /// </summary>
    public class MemberInfoData : IEquatable<MemberInfoData>
    {
        /// <summary>
        /// 获取成员信息。
        /// </summary>
        public MemberInfo MemberInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取成员声明类型。
        /// </summary>
        public Type DeclaringType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取成员类型。
        /// </summary>
        public Type MemberType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取成员名称。
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 使用名称、成员类型和声明类型初始化 <see cref="MemberInfoData"/> 实例。
        /// </summary>
        /// <param name="name">成员名称。</param>
        /// <param name="memberType">成员类型。</param>
        /// <param name="declaringType">声明类型。</param>
        public MemberInfoData(string name, Type memberType, Type declaringType)
        {
            Name = name;
            MemberType = memberType;
            DeclaringType = declaringType;
        }

        /// <summary>
        /// 使用成员信息初始化 <see cref="MemberInfoData"/> 实例。
        /// </summary>
        /// <param name="memberInfo">成员信息。</param>
        public MemberInfoData(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
            Name = memberInfo.Name;
            MemberType = memberInfo.GetMemberInfoType();
            DeclaringType = memberInfo.DeclaringType;
        }

        /// <summary>
        /// 判断当前实例是否与另一个 <see cref="MemberInfoData"/> 相等。
        /// </summary>
        /// <param name="other">要比较的另一个实例。</param>
        /// <returns>名称、成员类型和声明类型均相同返回 true，否则返回 false。</returns>
        public bool Equals(MemberInfoData other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Name, other.Name) && Equals(MemberType, other.MemberType) && Equals(DeclaringType, other.DeclaringType);
        }

        /// <summary>
        /// 判断两个 <see cref="MemberInfoData"/> 实例是否相等。
        /// </summary>
        /// <param name="left">左侧实例。</param>
        /// <param name="right">右侧实例。</param>
        /// <returns>相等返回 true，否则返回 false。</returns>
        public static bool operator ==(MemberInfoData left, MemberInfoData right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 判断两个 <see cref="MemberInfoData"/> 实例是否不相等。
        /// </summary>
        /// <param name="left">左侧实例。</param>
        /// <param name="right">右侧实例。</param>
        /// <returns>不相等返回 true，否则返回 false。</returns>
        public static bool operator !=(MemberInfoData left, MemberInfoData right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// 判断当前实例是否与指定对象相等。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns>对象为同类型的 <see cref="MemberInfoData"/> 且相等返回 true，否则返回 false。</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((MemberInfoData)obj);
        }

        /// <summary>
        /// 返回当前实例的哈希代码。
        /// </summary>
        /// <returns>基于名称、成员类型和声明类型计算的哈希代码。</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MemberType != null ? MemberType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (DeclaringType != null ? DeclaringType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
