using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ToolGood.ReadyGo.NPoco.ArrayExtensions;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 提供对象的深拷贝与类型判断扩展方法。
    /// </summary>
    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// 判断类型是否为基础类型或字符串。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>若是字符串或基础值类型返回 true，否则返回 false。</returns>
        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.GetTypeInfo().IsValueType & type.GetTypeInfo().IsPrimitive);
        }

        /// <summary>
        /// 对对象进行深拷贝。
        /// </summary>
        /// <param name="originalObject">要拷贝的源对象。</param>
        /// <returns>拷贝出的新对象。</returns>
        public static Object Copy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }
        private static Object InternalCopy(Object originalObject, IDictionary<object, object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }
            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.GetTypeInfo().BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.GetTypeInfo().BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.GetTypeInfo().BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        /// <summary>
        /// 对对象进行深拷贝并返回指定类型。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        /// <param name="original">要拷贝的源对象。</param>
        /// <returns>拷贝出的新对象。</returns>
        public static T Copy<T>(this T original)
        {
            return (T)Copy((Object)original);
        }

        /// <summary>
        /// 尝试将对象转换为指定类型。
        /// </summary>
        /// <typeparam name="TCastType">目标类型。</typeparam>
        /// <param name="inValue">源对象。</param>
        /// <param name="value">转换成功时输出转换后的值。</param>
        /// <returns>转换是否成功。</returns>
        public static bool CanBeCastTo<TCastType>(this object inValue, out TCastType value)
        {
            var result = inValue is TCastType;
            value = result ? (TCastType)inValue : default(TCastType);
            return result;
        }
    }

    /// <summary>
    /// 基于引用相等的对象比较器，用于深拷贝时跟踪已访问对象。
    /// </summary>
    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        /// <summary>
        /// 判断两个对象是否为同一引用。
        /// </summary>
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        /// <summary>
        /// 返回对象的哈希码。
        /// </summary>
        public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }

    namespace ArrayExtensions
    {
        /// <summary>
        /// 提供遍历多维数组的扩展方法。
        /// </summary>
        public static class ArrayExtensions
        {
            /// <summary>
            /// 遍历数组中的每个元素并对其位置执行操作。
            /// </summary>
            /// <param name="array">要遍历的数组。</param>
            /// <param name="action">对数组及元素索引执行的操作。</param>
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if (array.Length == 0) return;
                ArrayTraverse walker = new ArrayTraverse(array);
                do action(array, walker.Position);
                while (walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
