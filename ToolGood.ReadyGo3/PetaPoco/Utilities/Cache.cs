using System;
using System.Collections.Generic;
using System.Threading;

namespace ToolGood.ReadyGo3.PetaPoco.Internal
{
    internal class Cache<TKey, TValue>
    {
        private long _lastTicks;//最后Ticks
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly ReaderWriterLockSlim _slimLock = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _map = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TKey, AntiDupLockSlim> _lockDict = new Dictionary<TKey, AntiDupLockSlim>();
        class AntiDupLockSlim : ReaderWriterLockSlim { public int UseCount; }


        //public int Count {
        //    get { return _map.Count; }
        //}

        public TValue Get(TKey key, Func<TValue> factory)
        {
            long lastTicks;
            // Check cache
            _lock.EnterReadLock();
            TValue val;
            try {
                if (_map.TryGetValue(key, out val)) return val;
                lastTicks = _lastTicks;
            } finally { _lock.ExitReadLock(); }

            AntiDupLockSlim slim;
            _slimLock.EnterUpgradeableReadLock();
            try {
                _lock.EnterReadLock();
                try {
                    if (_lastTicks != lastTicks && _map.TryGetValue(key, out val)) return val;
                } finally {
                    _lock.ExitReadLock();
                }
                _slimLock.EnterWriteLock();
                if (_lockDict.TryGetValue(key, out slim) == false) {
                    slim = new AntiDupLockSlim();
                    _lockDict[key] = slim;
                }
                slim.UseCount++;
                _slimLock.ExitWriteLock();
            } finally { _slimLock.ExitUpgradeableReadLock(); }


            slim.EnterWriteLock();
            try {
                _lock.EnterReadLock();
                try {
                    if (_lastTicks != lastTicks && _map.TryGetValue(key, out val)) return val;
                } finally { _lock.ExitReadLock(); }

                val = factory();
                _lock.EnterWriteLock();
                _lastTicks = DateTime.Now.Ticks;
                _map[key] = val;
                _lock.ExitWriteLock();
                return val;
            } finally {
                slim.ExitWriteLock();
                _slimLock.EnterWriteLock();
                slim.UseCount--;
                if (slim.UseCount == 0) {
                    _lockDict.Remove(key);
                    slim.Dispose();
                }
                _slimLock.ExitWriteLock();
            }
        }

        public void Flush()
        {
            // Cache it
            _lock.EnterWriteLock();
            try {
                _map.Clear();
            } finally {
                _lock.ExitWriteLock();
            }
        }
    }
}