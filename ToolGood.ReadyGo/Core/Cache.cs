using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 基于读写锁实现的线程安全缓存字典。
    /// </summary>
    /// <typeparam name="TKey">缓存的键类型。</typeparam>
    /// <typeparam name="TValue">缓存的值类型。</typeparam>
    public class Cache<TKey, TValue>
    {
        /// <summary>
        /// Creates a cache that uses static storage
        /// </summary>
        /// <returns>新建的缓存实例。</returns>
        public static Cache<TKey, TValue> CreateStaticCache()
        {
            return new Cache<TKey, TValue>();
        }

        readonly Dictionary<TKey, TValue> _map = new Dictionary<TKey, TValue>();
        readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        
        /// <summary>
        /// 获取缓存中当前存储的条目数量。
        /// </summary>
        public int Count => _map.Count;

        /// <summary>
        /// 获取指定键对应的值；若不存在则调用工厂方法创建并缓存后返回。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="factory">当键不存在时用于创建值的工厂方法。</param>
        /// <returns>与键关联的值。</returns>
        public TValue Get(TKey key, Func<TValue> factory)
        {
            // Check cache
            _lock.EnterReadLock();
            TValue val;
            try
            {
                if (_map.TryGetValue(key, out val))
                    return val;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            // Cache it
            _lock.EnterWriteLock();
            try
            {
                // Check again
                if (_map.TryGetValue(key, out val))
                    return val;

                // Create it
                val = factory();

                // Store it
                _map.Add(key, val);

                // Done
                return val;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 若指定键不存在则添加缓存条目；若键已存在则不添加。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="value">要缓存的值。</param>
        /// <returns>若键已存在返回 true，否则返回 false。</returns>
        public bool AddIfNotExists(TKey key, TValue value)
        {
            // Cache it
            _lock.EnterWriteLock();
            try
            {
                // Check again
                TValue val;
                if (_map.TryGetValue(key, out val))
                    return true;

                // Create it
                val = value;

                // Store it
                _map.Add(key, val);

                // Done
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 清空缓存中的所有条目。
        /// </summary>
        public void Flush()
        {
            // Cache it
            _lock.EnterWriteLock();
            try
            {
                _map.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }

        }
    }
}
