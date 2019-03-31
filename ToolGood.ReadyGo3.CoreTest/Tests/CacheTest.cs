using PetaTest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using System.Collections.Concurrent;

namespace ToolGood.ReadyGo3.CoreTest.Tests
{
    [TestFixture]
    public class CacheTest
    {
        [Test]
        public void Test_IsAllowType()
        {
            for (int i = 0; i < 100000; i++) {
                IsAllowType(typeof(Int16));
                IsAllowType(typeof(Int32));
                IsAllowType(typeof(Int64));
                IsAllowType(typeof(UInt16));
                IsAllowType(typeof(UInt32));
                IsAllowType(typeof(UInt64));
                IsAllowType(typeof(string));
                IsAllowType(typeof(Guid));
                IsAllowType(typeof(float));
                IsAllowType(typeof(double));
                IsAllowType(typeof(decimal));
                IsAllowType(typeof(DateTime));
                IsAllowType(typeof(bool));


                IsAllowType(typeof(Int16?));
                IsAllowType(typeof(Int32?));
                IsAllowType(typeof(Int64?));
                IsAllowType(typeof(UInt16?));
                IsAllowType(typeof(UInt32?));
                IsAllowType(typeof(UInt64?));
                IsAllowType(typeof(string));
                IsAllowType(typeof(Guid?));
                IsAllowType(typeof(float?));
                IsAllowType(typeof(double?));
                IsAllowType(typeof(decimal?));
                IsAllowType(typeof(DateTime?));
                IsAllowType(typeof(bool?));
            }
        }
        [Test]
        public void Test_IsAllowType2()
        {
            for (int i = 0; i < 100000; i++) {
                IsAllowType2(typeof(Int16));
                IsAllowType2(typeof(Int32));
                IsAllowType2(typeof(Int64));
                IsAllowType2(typeof(UInt16));
                IsAllowType2(typeof(UInt32));
                IsAllowType2(typeof(UInt64));
                IsAllowType2(typeof(string));
                IsAllowType2(typeof(Guid));
                IsAllowType2(typeof(float));
                IsAllowType2(typeof(double));
                IsAllowType2(typeof(decimal));
                IsAllowType2(typeof(DateTime));
                IsAllowType2(typeof(bool));


                IsAllowType2(typeof(Int16?));
                IsAllowType2(typeof(Int32?));
                IsAllowType2(typeof(Int64?));
                IsAllowType2(typeof(UInt16?));
                IsAllowType2(typeof(UInt32?));
                IsAllowType2(typeof(UInt64?));
                IsAllowType2(typeof(string));
                IsAllowType2(typeof(Guid?));
                IsAllowType2(typeof(float?));
                IsAllowType2(typeof(double?));
                IsAllowType2(typeof(decimal?));
                IsAllowType2(typeof(DateTime?));
                IsAllowType2(typeof(bool?));

            }
        }
        [Test]
        public void Test_IsAllowType3()
        {
            for (int i = 0; i < 100000; i++) {
                IsAllowType3(typeof(Int16));
                IsAllowType3(typeof(Int32));
                IsAllowType3(typeof(Int64));
                IsAllowType3(typeof(UInt16));
                IsAllowType3(typeof(UInt32));
                IsAllowType3(typeof(UInt64));
                IsAllowType3(typeof(string));
                IsAllowType3(typeof(Guid));
                IsAllowType3(typeof(float));
                IsAllowType3(typeof(double));
                IsAllowType3(typeof(decimal));
                IsAllowType3(typeof(DateTime));
                IsAllowType3(typeof(bool));

                IsAllowType3(typeof(Int16?));
                IsAllowType3(typeof(Int32?));
                IsAllowType3(typeof(Int64?));
                IsAllowType3(typeof(UInt16?));
                IsAllowType3(typeof(UInt32?));
                IsAllowType3(typeof(UInt64?));
                IsAllowType3(typeof(string));
                IsAllowType3(typeof(Guid?));
                IsAllowType3(typeof(float?));
                IsAllowType3(typeof(double?));
                IsAllowType3(typeof(decimal?));
                IsAllowType3(typeof(DateTime?));
                IsAllowType3(typeof(bool?));
            }
        }
        [Test]
        public void Test_IsAllowType4()
        {
            for (int i = 0; i < 100000; i++) {
                IsAllowType4(typeof(Int16));
                IsAllowType4(typeof(Int32));
                IsAllowType4(typeof(Int64));
                IsAllowType4(typeof(UInt16));
                IsAllowType4(typeof(UInt32));
                IsAllowType4(typeof(UInt64));
                IsAllowType4(typeof(string));
                IsAllowType4(typeof(Guid));
                IsAllowType4(typeof(float));
                IsAllowType4(typeof(double));
                IsAllowType4(typeof(decimal));
                IsAllowType4(typeof(DateTime));
                IsAllowType4(typeof(bool));
            }
        }
        [Test]
        public void Test_IsAllowType5()
        {
            for (int i = 0; i < 100000; i++) {
                IsAllowType5(typeof(Int16));
                IsAllowType5(typeof(Int32));
                IsAllowType5(typeof(Int64));
                IsAllowType5(typeof(UInt16));
                IsAllowType5(typeof(UInt32));
                IsAllowType5(typeof(UInt64));
                IsAllowType5(typeof(string));
                IsAllowType5(typeof(Guid));
                IsAllowType5(typeof(float));
                IsAllowType5(typeof(double));
                IsAllowType5(typeof(decimal));
                IsAllowType5(typeof(DateTime));
                IsAllowType5(typeof(bool));
            }
        }
        [Test]
        public void Test_IsAllowType6()
        {
            for (int i = 0; i < 100000; i++) {
                IsAllowType6(typeof(Int16));
                IsAllowType6(typeof(Int32));
                IsAllowType6(typeof(Int64));
                IsAllowType6(typeof(UInt16));
                IsAllowType6(typeof(UInt32));
                IsAllowType6(typeof(UInt64));
                IsAllowType6(typeof(string));
                IsAllowType6(typeof(Guid));
                IsAllowType6(typeof(float));
                IsAllowType6(typeof(double));
                IsAllowType6(typeof(decimal));
                IsAllowType6(typeof(DateTime));
                IsAllowType6(typeof(bool));

                IsAllowType6(typeof(Int16?));
                IsAllowType6(typeof(Int32?));
                IsAllowType6(typeof(Int64?));
                IsAllowType6(typeof(UInt16?));
                IsAllowType6(typeof(UInt32?));
                IsAllowType6(typeof(UInt64?));
                IsAllowType6(typeof(string));
                IsAllowType6(typeof(Guid?));
                IsAllowType6(typeof(float?));
                IsAllowType6(typeof(double?));
                IsAllowType6(typeof(decimal?));
                IsAllowType6(typeof(DateTime?));
                IsAllowType6(typeof(bool?));
            }
        }
        [Test]
        public void Test_IsAllowType7()
        {
            for (int i = 0; i < 100000; i++) {
                IsAllowType7(typeof(Int16));
                IsAllowType7(typeof(Int32));
                IsAllowType7(typeof(Int64));
                IsAllowType7(typeof(UInt16));
                IsAllowType7(typeof(UInt32));
                IsAllowType7(typeof(UInt64));
                IsAllowType7(typeof(string));
                IsAllowType7(typeof(Guid));
                IsAllowType7(typeof(float));
                IsAllowType7(typeof(double));
                IsAllowType7(typeof(decimal));
                IsAllowType7(typeof(DateTime));
                IsAllowType7(typeof(bool));

                IsAllowType7(typeof(Int16?));
                IsAllowType7(typeof(Int32?));
                IsAllowType7(typeof(Int64?));
                IsAllowType7(typeof(UInt16?));
                IsAllowType7(typeof(UInt32?));
                IsAllowType7(typeof(UInt64?));
                IsAllowType7(typeof(string));
                IsAllowType7(typeof(Guid?));
                IsAllowType7(typeof(float?));
                IsAllowType7(typeof(double?));
                IsAllowType7(typeof(decimal?));
                IsAllowType7(typeof(DateTime?));
                IsAllowType7(typeof(bool?));
            }
        }

        internal static Cache<Type, bool> _IsAllowType = new Cache<Type, bool>();
        internal static bool IsAllowType(Type type)
        {
            return _IsAllowType.Get(type, () => {
                if (type == null) return false;
                if (type.IsEnum) return true;
                if (type == typeof(byte[])) return true;
                if (type == typeof(sbyte[])) return true;
                if (type.FullName == "Microsoft.SqlServer.Types.SqlGeography") return true;
                if (type.FullName == "Microsoft.SqlServer.Types.SqlGeometry") return true;

                if (type.IsGenericType) {
                    if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                        type = type.GetGenericArguments()[0];
                    } else {
                        return false;
                    }
                }

                if (type == typeof(Guid)) return true;
                if (type == typeof(AnsiString)) return true;
                if (type == typeof(TimeSpan)) return true;
                if (type == typeof(DateTimeOffset)) return true;

                var tc = Type.GetTypeCode(type);
                switch (tc) {
                    case TypeCode.Boolean:
                    case TypeCode.Byte:
                    case TypeCode.Char:
                    case TypeCode.DateTime:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Single:
                    case TypeCode.String:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;

                    case TypeCode.Object:
                    default:
                        break;
                }
                return false;
            });

        }
        internal static bool IsAllowType2(Type type)
        {

            if (type == null) return false;
            if (type.IsEnum) return true;
            if (type == typeof(byte[])) return true;
            if (type == typeof(sbyte[])) return true;
            if (type.FullName == "Microsoft.SqlServer.Types.SqlGeography") return true;
            if (type.FullName == "Microsoft.SqlServer.Types.SqlGeometry") return true;

            if (type.IsGenericType) {
                if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                    type = type.GetGenericArguments()[0];
                } else {
                    return false;
                }
            }

            if (type == typeof(Guid)) return true;
            if (type == typeof(AnsiString)) return true;
            if (type == typeof(TimeSpan)) return true;
            if (type == typeof(DateTimeOffset)) return true;

            var tc = Type.GetTypeCode(type);
            switch (tc) {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;

                case TypeCode.Object:
                default:
                    break;
            }
            return false;

        }


        internal static ConcurrentDictionary<Type, bool> _IsAllowType3;
        private static ConcurrentDictionary<Type, bool> GetDict()
        {
            if (_IsAllowType3 == null) {
                var dict = new ConcurrentDictionary<Type, bool>();
                dict[typeof(byte[])] = true;
                dict[typeof(sbyte[])] = true;
                dict[typeof(Guid)] = true;
                dict[typeof(AnsiString)] = true;
                dict[typeof(TimeSpan)] = true;
                dict[typeof(DateTimeOffset)] = true;
                dict[typeof(DateTime)] = true;
                dict[typeof(bool)] = true;
                dict[typeof(byte)] = true;
                dict[typeof(sbyte)] = true;
                dict[typeof(char)] = true;
                dict[typeof(Int16)] = true;
                dict[typeof(Int32)] = true;
                dict[typeof(Int64)] = true;
                dict[typeof(UInt16)] = true;
                dict[typeof(UInt32)] = true;
                dict[typeof(UInt64)] = true;

                dict[typeof(Single)] = true;
                dict[typeof(Double)] = true;
                dict[typeof(Decimal)] = true;
                dict[typeof(string)] = true;

                dict[typeof(Guid?)] = true;
                dict[typeof(TimeSpan?)] = true;
                dict[typeof(DateTimeOffset?)] = true;
                dict[typeof(DateTime?)] = true;
                dict[typeof(bool?)] = true;
                dict[typeof(byte?)] = true;
                dict[typeof(sbyte?)] = true;
                dict[typeof(char?)] = true;
                dict[typeof(Int16?)] = true;
                dict[typeof(Int32?)] = true;
                dict[typeof(Int64?)] = true;
                dict[typeof(UInt16?)] = true;
                dict[typeof(UInt32?)] = true;
                dict[typeof(UInt64?)] = true;

                dict[typeof(Single?)] = true;
                dict[typeof(Double?)] = true;
                dict[typeof(Decimal?)] = true;
                _IsAllowType3 = dict;
            }
            return _IsAllowType3;
        }

        internal static bool IsAllowType3(Type type)
        {
            if (type == null) return false;
            if (type.IsEnum) return true;
            if (type.IsGenericType) {
                if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                    type = type.GetGenericArguments()[0];
                } else {
                    return false;
                }
            }
            bool b;
            if (GetDict().TryGetValue(type, out b)) {
                return b;
            }
            if (type.FullName == "Microsoft.SqlServer.Types.SqlGeography") return true;
            if (type.FullName == "Microsoft.SqlServer.Types.SqlGeometry") return true;
            return false;

        }
        internal static bool IsAllowType7(Type type)
        {
            if (type == null) return false;
            var dict = GetDict();
            bool b;
            if (dict.TryGetValue(type, out b)) { return b; }

            if (type.IsEnum) { dict[type] = true; return true; }
            if (type.IsGenericType) {
                if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                    type = type.GetGenericArguments()[0];
                } else {
                    dict[type] = false;
                    return false;
                }
            }
            if (dict.TryGetValue(type, out b)) { return b; }
            b = type.FullName == "Microsoft.SqlServer.Types.SqlGeography" || type.FullName == "Microsoft.SqlServer.Types.SqlGeometry";
            dict[type] = b;
            return b;
        }



        private static Type[] _IsAllowType4 = new Type[] {
                 typeof(byte[]),
                typeof(sbyte[]),
                typeof(Guid),
                typeof(AnsiString),
                typeof(TimeSpan),
                typeof(DateTimeOffset),
                typeof(DateTime),
                typeof(bool),
                typeof(byte),
                typeof(sbyte),
                typeof(char),
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(UInt16),
                typeof(UInt32),
                typeof(UInt64),

                typeof(Single),
                typeof(Double),
                typeof(Decimal),
                typeof(string),
            };
        internal static bool IsAllowType4(Type type)
        {
            if (type == null) return false;
            if (type.IsEnum) return true;
            if (type.IsGenericType) {
                if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                    type = type.GetGenericArguments()[0];
                } else {
                    return false;
                }
            }
            if (_IsAllowType4.Contains(type)) {
                return true;
            }
            //bool b;
            //if (GetDict().TryGetValue(type, out b)) {
            //    return b;
            //}
            if (type.FullName == "Microsoft.SqlServer.Types.SqlGeography") return true;
            if (type.FullName == "Microsoft.SqlServer.Types.SqlGeometry") return true;
            return false;

        }

        private static HashSet<Type> _IsAllowType5;
        private static HashSet<Type> GetTypes()
        {
            if (_IsAllowType5 == null) {
                _IsAllowType5 = new HashSet<Type>() {
                typeof(byte[]),
                typeof(sbyte[]),
                typeof(Guid),
                typeof(AnsiString),
                typeof(TimeSpan),
                typeof(DateTimeOffset),
                typeof(DateTime),
                typeof(bool),
                typeof(byte),
                typeof(sbyte),
                typeof(char),
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(UInt16),
                typeof(UInt32),
                typeof(UInt64),

                typeof(Single),
                typeof(Double),
                typeof(Decimal),
                typeof(string),
            };
                //foreach (var item in types) {
                //    _IsAllowType5.Add(item);
                //}
            }
            return _IsAllowType5;
        }
        internal static bool IsAllowType5(Type type)
        {
            if (type == null) return false;
            if (type.IsEnum) return true;
            if (type.IsGenericType) {
                if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                    type = type.GetGenericArguments()[0];
                } else {
                    return false;
                }
            }
            if (GetTypes().Contains(type)) {
                return true;
            }
            //bool b;
            //if (GetDict().TryGetValue(type, out b)) {
            //    return b;
            //}
            if (type.FullName == "Microsoft.SqlServer.Types.SqlGeography") return true;
            if (type.FullName == "Microsoft.SqlServer.Types.SqlGeometry") return true;
            return false;
        }

        internal static bool IsAllowType6(Type type)
        {
            if (type == null) return false;
            if (GetTypes().Contains(type)) {
                return true;
            }
            if (type.IsEnum) return true;
            if (type.IsGenericType) {
                if (type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                    type = type.GetGenericArguments()[0];
                    if (GetTypes().Contains(type)) {
                        GetTypes().Add(type);
                        return true;
                    }
                } else {
                    return false;
                }
            }

            //bool b;
            //if (GetDict().TryGetValue(type, out b)) {
            //    return b;
            //}
            if (type.FullName == "Microsoft.SqlServer.Types.SqlGeography") return true;
            if (type.FullName == "Microsoft.SqlServer.Types.SqlGeometry") return true;
            return false;
        }


        internal class Cache<TKey, TValue>
        {
            private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
            private Dictionary<TKey, TValue> _map = new Dictionary<TKey, TValue>();

            public int Count {
                get { return _map.Count; }
            }

            public TValue Get(TKey key, Func<TKey, TValue> factory)
            {
                // Check cache
                _lock.EnterReadLock();
                TValue val;
                try {
                    if (_map.TryGetValue(key, out val))
                        return val;
                } finally {
                    _lock.ExitReadLock();
                }

                // Cache it
                _lock.EnterWriteLock();
                try {
                    // Check again
                    if (_map.TryGetValue(key, out val))
                        return val;

                    // Create it
                    val = factory(key);

                    // Store it
                    _map.Add(key, val);

                    // Done
                    return val;
                } finally {
                    _lock.ExitWriteLock();
                }
            }


            public TValue Get(TKey key, Func<TValue> factory)
            {
                // Check cache
                _lock.EnterReadLock();
                TValue val;
                try {
                    if (_map.TryGetValue(key, out val))
                        return val;
                } finally {
                    _lock.ExitReadLock();
                }

                // Cache it
                _lock.EnterWriteLock();
                try {
                    // Check again
                    if (_map.TryGetValue(key, out val))
                        return val;

                    // Create it
                    val = factory();

                    // Store it
                    _map.Add(key, val);

                    // Done
                    return val;
                } finally {
                    _lock.ExitWriteLock();
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
}
