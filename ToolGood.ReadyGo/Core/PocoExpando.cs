using System;
using System.Collections;
using System.Collections.Generic;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 动态可扩展对象，同时实现动态成员访问与字符串键字典接口（键忽略大小写）。
    /// </summary>
    public class PocoExpando : System.Dynamic.DynamicObject, IDictionary<string, object>, IDictionary
    {
        private readonly IDictionary<string, object> Dictionary =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 添加一个键值对。
        /// </summary>
        /// <param name="item">要添加的键值对。</param>
        public void Add(KeyValuePair<string, object> item)
        {
            Dictionary.Add(item);
        }

        /// <summary>
        /// 判断是否包含指定键。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>若包含则返回 true，否则返回 false。</returns>
        public bool Contains(object key)
        {
            return ((IDictionary)Dictionary).Contains(key);
        }

        /// <summary>
        /// 添加指定键与值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        public void Add(object key, object value)
        {
            ((IDictionary)Dictionary).Add(key, value);
        }

        /// <summary>
        /// 清空所有键值对。
        /// </summary>
        public void Clear()
        {
            Dictionary.Clear();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)Dictionary).GetEnumerator();
        }

        /// <summary>
        /// 移除指定键对应的项。
        /// </summary>
        /// <param name="key">键。</param>
        public void Remove(object key)
        {
            ((IDictionary)Dictionary).Remove(key);
        }

        /// <summary>
        /// 获取或设置指定键对应的值。
        /// </summary>
        /// <param name="key">键。</param>
        public object this[object key]
        {
            get => ((IDictionary)Dictionary)[key];
            set => ((IDictionary)Dictionary)[key] = value;
        }

        /// <summary>
        /// 判断是否包含指定键值对。
        /// </summary>
        /// <param name="item">要检查的键值对。</param>
        /// <returns>若包含则返回 true，否则返回 false。</returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return Dictionary.Contains(item);
        }

        /// <summary>
        /// 将键值对复制到数组中。
        /// </summary>
        /// <param name="array">目标数组。</param>
        /// <param name="arrayIndex">起始索引。</param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 移除指定键值对。
        /// </summary>
        /// <param name="item">要移除的键值对。</param>
        /// <returns>若移除成功则返回 true，否则返回 false。</returns>
        public bool Remove(KeyValuePair<string, object> item)
        {
            return Dictionary.Remove(item);
        }

        /// <summary>
        /// 将键值对复制到数组中。
        /// </summary>
        /// <param name="array">目标数组。</param>
        /// <param name="index">起始索引。</param>
        public void CopyTo(Array array, int index)
        {
            ((IDictionary)Dictionary).CopyTo(array, index);
        }

        /// <summary>
        /// 键值对数量。
        /// </summary>
        public int Count
        {
            get { return this.Dictionary.Keys.Count; }
        }

        /// <summary>
        /// 用于线程同步的根对象。
        /// </summary>
        public object SyncRoot => ((IDictionary)Dictionary).SyncRoot;
        /// <summary>
        /// 指示是否线程安全。
        /// </summary>
        public bool IsSynchronized => ((IDictionary)Dictionary).IsSynchronized;

        ICollection IDictionary.Values => ((IDictionary)Dictionary).Values;

        /// <summary>
        /// 指示是否只读。
        /// </summary>
        public bool IsReadOnly
        {
            get { return Dictionary.IsReadOnly; }
        }

        /// <summary>
        /// 指示是否具有固定大小。
        /// </summary>
        public bool IsFixedSize => ((IDictionary)Dictionary).IsFixedSize;

        /// <summary>
        /// 尝试获取动态成员的值。
        /// </summary>
        /// <param name="binder">成员绑定信息。</param>
        /// <param name="result">成员的值。</param>
        /// <returns>若成员存在则返回 true，否则返回 false。</returns>
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            if (this.Dictionary.ContainsKey(binder.Name))
            {
                result = this.Dictionary[binder.Name];
                return true;
            }
            return base.TryGetMember(binder, out result);
        }

        /// <summary>
        /// 尝试设置动态成员的值。
        /// </summary>
        /// <param name="binder">成员绑定信息。</param>
        /// <param name="value">要设置的值。</param>
        /// <returns>始终返回 true。</returns>
        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            if (!this.Dictionary.ContainsKey(binder.Name))
                this.Dictionary.Add(binder.Name, value);
            else
                this.Dictionary[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// 尝试调用动态成员（若其为委托则执行调用）。
        /// </summary>
        /// <param name="binder">成员绑定信息。</param>
        /// <param name="args">调用参数。</param>
        /// <param name="result">调用结果。</param>
        /// <returns>若调用成功则返回 true，否则返回 false。</returns>
        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {
            if (this.Dictionary.ContainsKey(binder.Name) && this.Dictionary[binder.Name] is Delegate)
            {
                Delegate del = this.Dictionary[binder.Name] as Delegate;
                result = del.DynamicInvoke(args);
                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }

        /// <summary>
        /// 尝试删除动态成员。
        /// </summary>
        /// <param name="binder">成员绑定信息。</param>
        /// <returns>若删除成功则返回 true，否则返回 false。</returns>
        public override bool TryDeleteMember(System.Dynamic.DeleteMemberBinder binder)
        {
            if (this.Dictionary.ContainsKey(binder.Name))
            {
                this.Dictionary.Remove(binder.Name);
                return true;
            }
            return base.TryDeleteMember(binder);
        }

        /// <summary>
        /// 返回枚举器以遍历键值对。
        /// </summary>
        /// <returns>键值对枚举器。</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 判断是否包含指定键。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>若包含则返回 true，否则返回 false。</returns>
        public bool ContainsKey(string key)
        {
            return Dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 添加指定键与值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        public void Add(string key, object value)
        {
            Dictionary.Add(key, value);
        }

        /// <summary>
        /// 移除指定键对应的项。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>若移除成功则返回 true，否则返回 false。</returns>
        public bool Remove(string key)
        {
            return Dictionary.Remove(key);
        }

        /// <summary>
        /// 尝试获取指定键对应的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">获取到的值。</param>
        /// <returns>若键存在则返回 true，否则返回 false。</returns>
        public bool TryGetValue(string key, out object value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// 获取或设置指定键对应的值。
        /// </summary>
        /// <param name="key">键。</param>
        public object this[string key]
        {
            get { return Dictionary[key]; }
            set { Dictionary[key] = value; }
        }

        /// <summary>
        /// 所有键的集合。
        /// </summary>
        public ICollection<string> Keys
        {
            get { return Dictionary.Keys; }
        }

        ICollection IDictionary.Keys => ((IDictionary)Dictionary).Keys;

        /// <summary>
        /// 所有值的集合。
        /// </summary>
        public ICollection<object> Values
        {
            get { return Dictionary.Values; }
        }
    }
}
