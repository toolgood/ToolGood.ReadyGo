using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.Poco;

namespace ToolGood.ReadyGo.WhereHelpers
{
    public interface IUpdateChange
    {

    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUpdateChange<T> where T : class, new()
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void __InitObject__();
        /// <summary>
        /// 获取区别
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Dictionary<string, object> __GetChanges__(T t);
    }
    /// <summary>
    /// 
    /// </summary>
    public  static class UpdateChangeHelper
    {
        #region 赋值

        #region bool

        public static Boolean CreateBoolean()
        {
            return true;
        }

        public static Boolean? CreateNullBoolean()
        {
            return true;
        }

        #endregion bool

        #region 字符串

        public static String CreateString()
        {
            return "a";
        }

        public static char CreateChar()
        {
            return 'a';
        }

        public static char? CreateNullChar()
        {
            return 'a';
        }

        public static AnsiString CreateAnsiString()
        {
            return new AnsiString("a");
        }

        #endregion 字符串

        #region 数字

        public static Int16 CreateInt16()
        {
            return short.MaxValue;
        }

        public static Int32 CreateInt32()
        {
            return Int32.MaxValue;
        }

        public static Int64 CreateInt64()
        {
            return Int64.MaxValue;
        }

        public static UInt16 CreateUInt16()
        {
            return ushort.MaxValue;
        }

        public static UInt32 CreateUInt32()
        {
            return UInt32.MaxValue;
        }

        public static UInt64 CreateUInt64()
        {
            return UInt64.MaxValue;
        }

        public static Int16? CreateNullInt16()
        {
            return short.MaxValue;
        }

        public static Int32? CreateNullInt32()
        {
            return Int32.MaxValue;
        }

        public static Int64? CreateNullInt64()
        {
            return Int64.MaxValue;
        }

        public static UInt16? CreateNullUInt16()
        {
            return ushort.MaxValue;
        }

        public static UInt32? CreateNullUInt32()
        {
            return UInt32.MaxValue;
        }

        public static UInt64? CreateNullUInt64()
        {
            return UInt64.MaxValue;
        }

        #endregion 数字

        #region 小数

        public static Single CreateSingle()
        {
            return 1f;
        }

        public static Single? CreateNullSingle()
        {
            return 1f;
        }

        public static Double CreateDouble()
        {
            return 1d;
        }

        public static Double? CreateNullDouble()
        {
            return 1d;
        }

        public static Decimal CreateDecimal()
        {
            return 1M;
        }

        public static Decimal? CreateNullDecimal()
        {
            return 1M;
        }

        #endregion 小数

        #region bytes

        public static byte CreateByte()
        {
            return 1;
        }

        public static byte? CreateNullByte()
        {
            return 1;
        }

        public static sbyte CreateSByte()
        {
            return 1;
        }

        public static sbyte? CreateNullSByte()
        {
            return 1;
        }

        public static byte[] CreateListByte()
        {
            return new byte[] { 1 };
        }

        public static sbyte[] CreateListSByte()
        {
            return new SByte[] { 1 };
        }

        #endregion bytes

        #region 时间

        public static DateTime CreateDateTime()
        {
            return DateTime.MaxValue;
        }

        public static DateTime? CreateNullDateTime()
        {
            return DateTime.MaxValue;
        }

        public static TimeSpan CreateTimeSpan()
        {
            return TimeSpan.MaxValue;
        }

        public static TimeSpan? CreateNullTimeSpan()
        {
            return TimeSpan.MaxValue;
        }

        public static DateTimeOffset CreateDateTimeOffset()
        {
            return DateTimeOffset.MaxValue;
        }

        public static DateTimeOffset? CreateNullDateTimeOffset()
        {
            return DateTimeOffset.MaxValue;
        }

        #endregion 时间

        #region Guid

        public static Guid CreateGuid()
        {
            return Guid.NewGuid();
        }

        public static Guid? CreateNullGuid()
        {
            return Guid.NewGuid();
        }

        #endregion Guid

        public static T Create<T>()
        {
            return Activator.CreateInstance<T>();
        }

        #endregion 赋值

        #region EqualsObject

        #region bool

        public static void EqualsObject(bool left, bool right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(bool? left, bool? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        #endregion bool

        #region 字符串

        public static void EqualsObject(string left, string right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(char left, char right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(char? left, char? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(AnsiString left, AnsiString right, string columnName, Dictionary<string, object> dic)
        {
            if (left == null && right == null) {
                dic[columnName] = left;
            } else if (left != null && right != null) {
                if (left.Value == right.Value) {
                    dic[columnName] = left;
                }
            }
        }

        #endregion 字符串

        #region 数字

        public static void EqualsObject(short left, short right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(int left, int right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(long left, long right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(ushort left, ushort right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(uint left, uint right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(ulong left, ulong right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        #endregion 数字

        #region 可空数字

        public static void EqualsObject(short? left, short? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(int? left, int? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(long? left, long? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(ushort? left, ushort? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(uint? left, uint? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(ulong? left, ulong? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        #endregion 可空数字

        #region 小数

        public static void EqualsObject(float left, float right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(double left, double right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(decimal left, decimal right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(float? left, float? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(double? left, double? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(decimal? left, decimal? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        #endregion 小数

        #region bytes

        public static void EqualsObject(byte left, byte right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(sbyte left, sbyte right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(byte? left, byte? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(sbyte? left, sbyte? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(byte[] left, byte[] right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(sbyte[] left, sbyte[] right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        #endregion bytes

        #region 时间

        public static void EqualsObject(DateTime left, DateTime right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(DateTime? left, DateTime? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(TimeSpan? left, TimeSpan? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(TimeSpan left, TimeSpan right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(DateTimeOffset left, DateTimeOffset right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(DateTimeOffset? left, DateTimeOffset? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        #endregion 时间

        #region Guid

        public static void EqualsObject(Guid left, Guid right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(Guid? left, Guid? right, string columnName, Dictionary<string, object> dic)
        {
            if (left == right) {
                dic[columnName] = left;
            }
        }

        #endregion Guid

        public static void EqualsObject(Enum left, Enum right, string columnName, Dictionary<string, object> dic)
        {
            if (left.Equals(right)) {
                dic[columnName] = left;
            }
        }

        public static void EqualsObject(object left, object right, string columnName, Dictionary<string, object> dic)
        {
            if (left == null && right == null) {
                dic[columnName] = left;
            } else if (left != null && right != null) {
                if (left == right) {
                    dic[columnName] = left;
                }
            }
        }

        #endregion EqualsObject
    }

    /// <summary>
    /// 代理类用于生成
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class UpdateProxy<T> where T : class, new()
    {
        private static ModuleBuilder moduleBldr;
        private static Cache<Type, Type> typeDict = new Cache<Type, Type>();
        private static Cache<Type, MethodInfo> InitDict = new Cache<Type, MethodInfo>();
        private static Cache<Type, MethodInfo> EqualsDict = new Cache<Type, MethodInfo>();
        private static Type typedict = typeof(Dictionary<string, object>);
        private static MethodInfo clearMethod = typedict.GetMethod("Clear");


        /// <summary>
        /// 生成一个新对象，该对象继承IUpdateChange &lt;T>
        /// </summary>
        /// <returns></returns>
        public static T Create()
        {
            Type type = typeof(T);
            var t = CreateType(type);
            var obj = Activator.CreateInstance(t);
            ((IUpdateChange<T>)obj).__InitObject__();
            return obj as T;
        }

        private static Type CreateType(Type type)
        {
            return typeDict.Get(type, () => {
                if (moduleBldr == null) {
                    var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ToolGood.ReadyGo.Temp"), AssemblyBuilderAccess.Run);
                    moduleBldr = asm.DefineDynamicModule("ToolGood.ReadyGo.Temp.dll");
                }
                Type interfaceType = typeof(IUpdateChange<T>);
                //得到类型生成器
                var typeBldr = moduleBldr.DefineType(type.Name + "Proxy", TypeAttributes.Public | TypeAttributes.Class, type, new Type[] { typeof(IUpdateChange<T>) });

                typeBldr.AddInterfaceImplementation(typeof(IUpdateChange<T>));

                var fb = typeBldr.DefineField("__update_dict__", typedict, FieldAttributes.Private);

                //定义构造函数
                ConstructorBuilder constructorBuilder = typeBldr.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, null);
                ILGenerator ctorIL = constructorBuilder.GetILGenerator();
                ctorIL.Emit(OpCodes.Ldarg_0);
                ctorIL.Emit(OpCodes.Call, type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)[0]);
                ctorIL.Emit(OpCodes.Ldarg_0);
                ctorIL.Emit(OpCodes.Newobj, typedict.GetConstructors()[0]);
                ctorIL.Emit(OpCodes.Stfld, fb);
                ctorIL.Emit(OpCodes.Ret);

                //生成接口方法
                var atts = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot;
                var cc = CallingConventions.HasThis | CallingConventions.Standard;

                var initMethod = typeBldr.DefineMethod("__InitObject__", atts, cc, null, Type.EmptyTypes);
                CreateInitMethod(initMethod);

                var getChangesMethod = typeBldr.DefineMethod("__GetChanges__", atts, cc, typedict, new Type[] { type });
                CreateGetChangesMethod(getChangesMethod, fb);

                return typeBldr.CreateType();
            });
        }

        private static void CreateInitMethod(MethodBuilder initMethod)
        {
            var pd = PocoData.ForType(typeof(T));
            var il = initMethod.GetILGenerator();
            foreach (var item in pd.Columns) {
                var pi = item.PropertyInfo;
                var piType = pi.PropertyType;
                if (piType.IsEnum) {
                    var enumNames = Enum.GetNames(piType);
                    if (enumNames.Length > 1) {
                        int e = (int)Enum.Parse(piType, enumNames[1]);
                        il.Emit(OpCodes.Ldarg_0);   // IL_006a: ldarg.0
                        il.Emit(OpCodes.Ldc_I4, e); //IL_006b: ldc.i4 1000
                        il.Emit(OpCodes.Call, pi.GetSetMethod()); //IL_0070: call instance void ConsoleApplication1.User::set_k(valuetype ConsoleApplication1.kk)
                    }
                } else {
                    MethodInfo mi = getInitMethod(piType);
                    il.Emit(OpCodes.Ldarg_0);  // IL_0019: ldarg.0
                    il.Emit(OpCodes.Call, mi); //IL_001a: call valuetype[mscorlib]System.Nullable`1 < int32 > ConsoleApplication1.UpdateChangeHelper::CreateNullInt32()
                    il.Emit(OpCodes.Call, pi.GetSetMethod()); //IL_001f: call instance void ConsoleApplication1.User::set_int2(valuetype[mscorlib]System.Nullable`1 < int32 >)
                }
            }
            il.Emit(OpCodes.Ret);
        }

        private static MethodInfo getInitMethod(Type type)
        {
            var me = InitDict.Get(type, () => {
                if (type == typeof(byte[])) {
                    return typeof(UpdateChangeHelper).GetMethod("CreateListByte", BindingFlags.Public | BindingFlags.Static);
                }
                if (type == typeof(sbyte[])) {
                    return typeof(UpdateChangeHelper).GetMethod("CreateListSByte", BindingFlags.Public | BindingFlags.Static);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("Create");
                if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                    sb.Append("Null");
                    type = type.GetGenericArguments()[0];
                }
                sb.Append(type.Name);
                var mh = typeof(UpdateChangeHelper).GetMethod(sb.ToString(), BindingFlags.Public | BindingFlags.Static);
                if (mh != null) {
                    return mh;
                }
                var m = typeof(UpdateChangeHelper).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
                m.MakeGenericMethod(type);

                return m;
            });
            return me;
        }

        private static void CreateGetChangesMethod(MethodBuilder getChangesMethod, FieldBuilder fb)
        {
            var pd = PocoData.ForType(typeof(T));
            var il = getChangesMethod.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);  //            IL_0001: ldarg.0
            il.Emit(OpCodes.Ldfld, fb);   //    IL_0002: ldfld class [mscorlib]        System.Collections.Generic.Dictionary`2<string, object> ConsoleApplication1.UserProxy::dict
            il.Emit(OpCodes.Callvirt, clearMethod);   //IL_0007: callvirt instance void class [mscorlib]       System.Collections.Generic.Dictionary`2<string, object>::Clear()

            foreach (var item in pd.Columns) {
                var pi = item.PropertyInfo;
                var piType = pi.PropertyType;
                var m = getEqualsMethod(piType);

                il.Emit(OpCodes.Ldarg_0); //                    IL_0054: ldarg.0
                il.Emit(OpCodes.Call, pi.GetGetMethod());//    IL_0055: call instance class ConsoleApplication1.TestClass ConsoleApplication1.User::get_ss()
                if (piType.IsEnum) il.Emit(OpCodes.Box, piType);
                il.Emit(OpCodes.Ldarg_1);//    IL_005a: ldarg.1
                il.Emit(OpCodes.Callvirt, pi.GetGetMethod());  //	IL_005b: callvirt instance class ConsoleApplication1.TestClass ConsoleApplication1.User::get_ss()
                if (piType.IsEnum) il.Emit(OpCodes.Box, piType);
                il.Emit(OpCodes.Ldstr, item.ColumnName); //    IL_0060: ldstr "ss"
                il.Emit(OpCodes.Ldarg_0);    //	IL_0065: ldarg.0
                il.Emit(OpCodes.Ldfld, fb);   //	IL_0066: ldfld class [mscorlib]       System.Collections.Generic.Dictionary`2<string, object> ConsoleApplication1.UserProxy::dict
                il.Emit(OpCodes.Call, m);  //IL_006b: call void ConsoleApplication1.UpdateChangeHelper::EqualsObject(object, object, string, class [mscorlib]      System.Collections.Generic.Dictionary`2<string, object>)
            }
            il.Emit(OpCodes.Ldarg_0); //        IL_0054: ldarg.0
            il.Emit(OpCodes.Ldfld, fb);  //IL_0055: ldfld class [mscorlib]   System.Collections.Generic.Dictionary`2<string, object> ConsoleApplication1.UserProxy::dict
            il.Emit(OpCodes.Ret);
        }

        private static MethodInfo getEqualsMethod(Type type)
        {
            var me = EqualsDict.Get(type, () => {
                if (type.IsEnum) {
                    return typeof(UpdateChangeHelper).GetMethod("EqualsObject", new Type[] { type, type, typeof(string), typedict });
                }
                var m = typeof(UpdateChangeHelper).GetMethod("EqualsObject", new Type[] { type, type, typeof(string), typedict });
                if (m != null) {
                    return m;
                }
                return typeof(UpdateChangeHelper).GetMethod("EqualsObject", new Type[] { typeof(object), typeof(object), typeof(string), typedict });
            });
            return me;
        }
    }
}