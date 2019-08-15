//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Reflection.Emit;
//using ToolGood.ReadyGo3.Internals;
//using ToolGood.ReadyGo3.PetaPoco.Core;

//namespace ToolGood.ReadyGo3.Internals
//{
//    interface IUpdateChange
//    {
//        Dictionary<string, object> __GetChanges__();
//        void __ClearChanges__();
//    }
//}

//namespace ToolGood.ReadyGo3
//{
//    /// <summary>
//    /// 代理类用于生成，
//    /// 属性使用virtual
//    /// </summary>
//    public static class UpdateData
//    {
//        private static ModuleBuilder moduleBldr;
//        private static readonly Cache<Type, Type> typeDict = new Cache<Type, Type>();
//        private static readonly Type typedict = typeof(Dictionary<string, object>);
//        private static readonly MethodInfo _setItemMethod = typedict.GetMethod("set_Item");
//        private static readonly MethodInfo _clearMethod = typedict.GetMethod("Clear");


//        /// <summary>
//        /// 生成一个新对象，该对象继承IUpdateChange
//        /// </summary>
//        /// <returns></returns>
//        public static T New<T>() where T : class
//        {
//            Type type = typeof(T);
//            var t = CreateType(type);
//            var obj = Activator.CreateInstance(t);
//            return obj as T;
//        }

//        public static Type GetProxyType(Type type)
//        {
//            if (type.IsClass) {
//                return CreateType(type);
//            }
//            return type;
//        }

//        private static Type CreateType(Type type)
//        {
//            return typeDict.Get(type, () => {
//                if (moduleBldr == null) {
//#if !NET40
//                    var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("ToolGood.ReadyGo.Temp"), AssemblyBuilderAccess.Run);
//#else
//                    var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ToolGood.ReadyGo.Temp"), AssemblyBuilderAccess.Run);
//#endif
//                    moduleBldr = asm.DefineDynamicModule("ToolGood.ReadyGo.Temp.dll");
//                }
//                Type interfaceType = typeof(IUpdateChange);
//                //得到类型生成器
//                var typeBldr = moduleBldr.DefineType(type.Name + "_Proxy", TypeAttributes.Public | TypeAttributes.Class, type, new Type[] { typeof(IUpdateChange) });
//                typeBldr.AddInterfaceImplementation(typeof(IUpdateChange));
//                //定义
//                var fb = typeBldr.DefineField("__sql_update_dict__", typedict, FieldAttributes.Private);

//                //定义构造函数
//                ConstructorBuilder constructorBuilder = typeBldr.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, null);
//                ILGenerator ctorIL = constructorBuilder.GetILGenerator();
//                ctorIL.Emit(OpCodes.Ldarg_0);
//                ctorIL.Emit(OpCodes.Newobj, typedict.GetConstructors()[0]);
//                ctorIL.Emit(OpCodes.Stfld, fb);
//                ctorIL.Emit(OpCodes.Ldarg_0);
//                ctorIL.Emit(OpCodes.Call, type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)[0]);
//                ctorIL.Emit(OpCodes.Ret);

//                var pd = PocoData.ForType(type);
//                var atts = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot;
//                var cc = CallingConventions.HasThis | CallingConventions.Standard;
//                foreach (var item in pd.Columns) {
//                    if (item.Value.ResultColumn) { continue; }
//                    var pi = item.Value.PropertyInfo;
//                    if (pi.GetGetMethod().IsVirtual) {
//                        var getMethod = typeBldr.DefineMethod("get_" + item.Value.PropertyName, atts, cc, pi.PropertyType, null);
//                        {
//                            var il = getMethod.GetILGenerator();
//                            il.Emit(OpCodes.Ldarg_0);                   // IL_0001: ldarg.0
//                            if (pi.PropertyType.IsValueType) {
//                                il.Emit(OpCodes.Call, pi.GetGetMethod()); // stack is now [target]
//                            } else {
//                                il.Emit(OpCodes.Callvirt, pi.GetGetMethod()); // stack is now [target]
//                            }
//                            il.Emit(OpCodes.Ret);                       // IL_000B: ret
//                        }
//                        typeBldr.DefineMethodOverride(getMethod, pi.GetGetMethod());
//                    }
//                    if (pi.GetSetMethod().IsVirtual) {
//                        var setMethod = typeBldr.DefineMethod("set_" + item.Value.PropertyName, atts, cc, null, new Type[] { pi.PropertyType });
//                        {
//                            var il = setMethod.GetILGenerator();
//                            il.Emit(OpCodes.Ldarg_0);                       // IL_0001: ldarg.0
//                            il.Emit(OpCodes.Ldfld, fb);                     // IL_0002: ldfld     class [System.Collections] System.Collections.Generic.Dictionary`2<string, object> ToolGood.ReadyGo3.CoreTest.DbMemberUpdateChange::__dict__
//                            il.Emit(OpCodes.Ldstr, item.Value.ColumnName);  // IL_0007: ldstr     "Id"
//                            il.Emit(OpCodes.Ldarg_1);                       // IL_000C: ldarg.1
//                            il.Emit(OpCodes.Box, pi.PropertyType);          // IL_000D: box[System.Runtime] System.Int32
//                            il.Emit(OpCodes.Callvirt, _setItemMethod);       // IL_0012: callvirt instance void class [System.Collections] System.Collections.Generic.Dictionary`2<string, object>::set_Item(!0, !1)
//                            il.Emit(OpCodes.Ldarg_0);                       // IL_0018: ldarg.0
//                            il.Emit(OpCodes.Ldarg_1);                       // IL_0019: ldarg.1
//                            if (pi.PropertyType.IsValueType) {
//                                il.Emit(OpCodes.Call, pi.GetSetMethod());   // stack is now [target]
//                            } else {
//                                il.Emit(OpCodes.Callvirt, pi.GetSetMethod()); // stack is now [target]
//                            }
//                            il.Emit(OpCodes.Ret);                           // IL_0020: ret
//                        }
//                        typeBldr.DefineMethodOverride(setMethod, pi.GetSetMethod());
//                    }

//                }
//                var getChangesMethod = typeBldr.DefineMethod("__GetChanges__", atts, cc, typedict, null);
//                {
//                    var il = getChangesMethod.GetILGenerator();
//                    il.Emit(OpCodes.Ldarg_0);           // IL_0001: ldarg.0
//                    il.Emit(OpCodes.Ldfld, fb);         // IL_0002: ldfld     class [System.Collections] System.Collections.Generic.Dictionary`2<string, object> ToolGood.ReadyGo3.CoreTest.DbMemberUpdateChange::__dict__
//                    il.Emit(OpCodes.Ret);               // IL_000B: ret
//                }

//                var clearMethod = typeBldr.DefineMethod("__ClearChanges__", atts, cc, null, null);
//                {
//                    var il = clearMethod.GetILGenerator();
//                    il.Emit(OpCodes.Ldarg_0);                       // IL_0000: ldarg.0
//                    il.Emit(OpCodes.Ldfld, fb);                     // IL_0001: ldfld     class [System.Collections] System.Collections.Generic.Dictionary`2<string, object> ToolGood.ReadyGo3.CoreTest.DbMemberUpdateChange2::__dict__
//                    il.Emit(OpCodes.Callvirt, _clearMethod);        // IL_0006: callvirt instance void class [System.Collections] System.Collections.Generic.Dictionary`2<string, object>::Clear()
//                    il.Emit(OpCodes.Ret);                           // IL_000B: ret
//                }


//#if !NET40
//                return typeBldr.CreateTypeInfo();
//#else
//                return typeBldr.CreateType();
//#endif
//            });
//        }

//    }
//    /// <summary>
//    /// 代理类用于生成，
//    /// 属性使用virtual
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public sealed class UpdateData<T> where T : class
//    {
//        /// <summary>
//        /// 生成一个新对象，该对象继承IUpdateChange
//        /// </summary>
//        /// <returns></returns>
//        public static T New()
//        {
//            return UpdateData.New<T>();
//        }
//    }

//}
