using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using ToolGood.ReadyGo3.PetaPoco.Core;
using ToolGood.ReadyGo3.PetaPoco.Internal;

namespace ToolGood.ReadyGo3.PetaPoco
{
    public static class Mappers
    {
        private static Dictionary<object, StandardMapper> _mappers = new Dictionary<object, StandardMapper>();
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        ///     Registers a mapper for all types in a specific assembly
        /// </summary>
        /// <param name="assembly">The assembly whose types are to be managed by this mapper</param>
        /// <param name="mapper">The IMapper implementation</param>
        public static void Register(Assembly assembly, StandardMapper mapper)
        {
            RegisterInternal(assembly, mapper);
        }

        /// <summary>
        ///     Registers a mapper for a single POCO type
        /// </summary>
        /// <param name="type">The type to be managed by this mapper</param>
        /// <param name="mapper">The IMapper implementation</param>
        public static void Register(Type type, StandardMapper mapper)
        {
            RegisterInternal(type, mapper);
        }

        /// <summary>
        ///     Remove all mappers for all types in a specific assembly
        /// </summary>
        /// <param name="assembly">The assembly whose mappers are to be revoked</param>
        public static void Revoke(Assembly assembly)
        {
            RevokeInternal(assembly);
        }

        /// <summary>
        ///     Remove the mapper for a specific type
        /// </summary>
        /// <param name="type">The type whose mapper is to be removed</param>
        public static void Revoke(Type type)
        {
            RevokeInternal(type);
        }

        /// <summary>
        ///     Revoke an instance of a mapper
        /// </summary>
        /// <param name="mapper">The IMapper to be revkoed</param>
        public static void Revoke(StandardMapper mapper)
        {
            _lock.EnterWriteLock();
            try
            {
                foreach (var i in _mappers.Where(kvp => kvp.Value == mapper).ToList())
                    _mappers.Remove(i.Key);
            }
            finally
            {
                _lock.ExitWriteLock();
                FlushCaches();
            }
        }

        /// <summary>
        ///     Revokes all registered mappers.
        /// </summary>
        public static void RevokeAll()
        {
            _lock.EnterWriteLock();
            try
            {
                _mappers.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
                FlushCaches();
            }
        }

        /// <summary>
        ///     Retrieve the IMapper implementation to be used for a specified POCO type.
        /// </summary>
        /// <param name="entityType">The entity type to get the mapper for.</param>
        /// <param name="defaultMapper">The default mapper to use when non is registered for the type.</param>
        /// <returns>The mapper for the given type.</returns>
        public static StandardMapper GetMapper(Type entityType, StandardMapper defaultMapper)
        {
            _lock.EnterReadLock();
            try
            {
                StandardMapper val;
                if (_mappers.TryGetValue(entityType, out val))
                    return val;
                if (_mappers.TryGetValue(entityType.Assembly, out val))
                    return val;
                if (defaultMapper==null) {
                    return new StandardMapper();
                }


                return defaultMapper;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private static void RegisterInternal(object typeOrAssembly, StandardMapper mapper)
        {
            _lock.EnterWriteLock();
            try
            {
                _mappers.Add(typeOrAssembly, mapper);
            }
            finally
            {
                _lock.ExitWriteLock();
                FlushCaches();
            }
        }

        private static void RevokeInternal(object typeOrAssembly)
        {
            _lock.EnterWriteLock();
            try
            {
                _mappers.Remove(typeOrAssembly);
            }
            finally
            {
                _lock.ExitWriteLock();
                FlushCaches();
            }
        }

        private static void FlushCaches()
        {
            // Whenever a mapper is registered or revoked, we have to assume any generated code is no longer valid.
            // Since this should be a rare occurrence, the simplest approach is to simply dump everything and start over.
            PocoData.FlushCaches();
        }
    }
}