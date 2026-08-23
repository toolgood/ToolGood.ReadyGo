using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 提供针对类型与成员的反射辅助方法。
    /// </summary>
    public static class ReflectionUtils
    {

        /// <summary>
        /// 获取指定类型的公开字段与属性；对值类型、字符串、字节数组、字典与数组返回空集合。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <returns>成员信息列表。</returns>
        public static List<MemberInfo> GetFieldsAndPropertiesForClasses(Type type)
        {
            if (type.GetTypeInfo().IsValueType || type == typeof(string) || type == typeof(byte[]) || type == typeof(Dictionary<string, object>) || type.IsArray)
                return new List<MemberInfo>();

            return GetFieldsAndProperties(type);
        }

        /// <summary>
        /// 获取指定类型的非公开属性；对值类型、字符串、字节数组、字典与数组返回空集合。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <returns>成员信息列表。</returns>
        public static List<MemberInfo> GetPrivatePropertiesForClasses(Type type)
        {
            if (type.GetTypeInfo().IsValueType || type == typeof(string) || type == typeof(byte[]) || type == typeof(Dictionary<string, object>) || type.IsArray)
                return new List<MemberInfo>();

            return GetFieldsAndProperties(type, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// 获取指定类型的实例公开字段与属性。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <returns>成员信息列表。</returns>
        public static List<MemberInfo> GetFieldsAndProperties(Type type)
        {
            return GetFieldsAndProperties(type, BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// 按指定绑定标志获取类型的字段与属性。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <param name="bindingAttr">绑定标志。</param>
        /// <returns>成员信息列表。</returns>
        public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
        {
            List<MemberInfo> targetMembers = new List<MemberInfo>();

            targetMembers.AddRange(type.GetFields(bindingAttr).Where(x => !x.IsInitOnly).ToArray());
            targetMembers.AddRange(type.GetProperties(bindingAttr));

            return targetMembers;
        }

        /// <summary>
        /// 获取成员对应的类型（字段类型或属性类型）。
        /// </summary>
        /// <param name="member">成员信息。</param>
        /// <returns>成员的类型。</returns>
        public static Type GetMemberInfoType(this MemberInfo member)
        {
            Type type;
            if (member is FieldInfo)
                type = ((FieldInfo) member).FieldType;
            else if (member is PropertyInfo)
                type = ((PropertyInfo) member).PropertyType;
            else if (member == null)
                type = typeof (object);
            else
                throw new NotSupportedException();

            return type;
        }

        /// <summary>
        /// 判断成员是否声明为动态类型。
        /// </summary>
        /// <param name="member">成员信息。</param>
        /// <returns>若是动态类型则返回 true，否则返回 false。</returns>
        public static bool IsDynamic(this MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(DynamicAttribute), true).Any();
        }

        /// <summary>
        /// 判断成员是否为字段。
        /// </summary>
        /// <param name="member">成员信息。</param>
        /// <returns>若是字段则返回 true，否则返回 false。</returns>
        public static bool IsField(this MemberInfo member)
        {
            return member is FieldInfo;
        }

        /// <summary>
        /// 获取属性在声明类型上的 Set 方法（必要时回退到声明类型上的同名属性）。
        /// </summary>
        /// <param name="propertyInfo">属性信息。</param>
        /// <returns>属性的 Set 方法。</returns>
        public static MethodInfo GetSetMethodOnDeclaringType(this PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.GetSetMethod(true);
            return methodInfo ?? propertyInfo
                                    .DeclaringType
                                    .GetProperty(propertyInfo.Name)
                                    .GetSetMethod(true);
        }

        /// <summary>
        /// 判断类型是否实现或本身即为指定的泛型接口。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <param name="genericTypeDefinition">泛型接口定义。</param>
        /// <returns>若匹配则返回 true，否则返回 false。</returns>
        public static bool IsOrHasGenericInterfaceTypeOf(this Type type, Type genericTypeDefinition)
        {
            return type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition) != null;
        }

        /// <summary>
        /// 获取类型实现或本身即为指定泛型接口定义时的具体泛型类型。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <param name="genericTypeDefinition">泛型接口定义。</param>
        /// <returns>匹配的泛型类型；未找到时返回 null。</returns>
        public static Type GetTypeWithGenericTypeDefinitionOf(this Type type, Type genericTypeDefinition)
        {
            foreach (var t in type.GetInterfaces())
            {
                if (t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return t;
                }
            }

            var genericType = type.GetGenericType();
            if (genericType != null && genericType.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return genericType;
            }

            return null;
        }

        /// <summary>
        /// 沿类型层次向上查找最近的泛型类型。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <returns>最近的泛型类型；未找到时返回 null。</returns>
        public static Type GetGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.GetTypeInfo().IsGenericType)
                    return type;

                type = type.GetTypeInfo().BaseType;
            }
            return null;
        }

        /// <summary>
        /// 判断实例类型是否为指定泛型类型（检查自身及基类、接口）。
        /// </summary>
        /// <param name="instanceType">实例类型。</param>
        /// <param name="genericType">泛型类型定义。</param>
        /// <returns>若匹配则返回 true，否则返回 false。</returns>
        public static bool IsOfGenericType(this Type instanceType, Type genericType)
        {
            Type type = instanceType;
            while (type != null)
            {
                if (type.GetTypeInfo().IsGenericType &&
                    type.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
                type = type.GetTypeInfo().BaseType;
            }

            foreach (var i in instanceType.GetInterfaces())
            {
                if (i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取成员上的所有自定义特性。
        /// </summary>
        /// <param name="memberInfo">成员信息。</param>
        /// <returns>自定义特性集合。</returns>
        public static IEnumerable<Attribute> GetCustomAttributes(MemberInfo memberInfo)
        {
            var attrs = memberInfo.GetCustomAttributes();
            return attrs;
        }
 
    }
}
