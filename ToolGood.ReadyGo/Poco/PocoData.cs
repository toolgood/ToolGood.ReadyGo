using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.Internals;
using System.Text;

namespace ToolGood.ReadyGo.Poco
{
    public class PocoData
    {
        #region PocoData 初始化 及常用属性

        private static Cache<Type, PocoData> _pocoDatas = new Cache<Type, PocoData>();
        public Type Type { get; private set; }
        public PocoTable TableInfo { get; private set; }
        public List<PocoColumn> Columns { get; private set; }

        private PocoData(Type type)
        {
            Type = type;
            TableInfo = FromPoco(type);
            Columns = new List<PocoColumn>();
            var pis = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var pi in pis) {
                var ci = FromProperty(pi);
                if (ci == null) continue;
                Columns.Add(ci);
            }
        }

        public static PocoData ForType(Type type)
        {
            if (type == typeof(System.Dynamic.ExpandoObject))
                throw new InvalidOperationException("Can't use dynamic types with this method");
            return _pocoDatas.Get(type, () => new PocoData(type));
        }

        private PocoTable FromPoco(Type type)
        {
            PocoTable ti = new PocoTable();
            var a = type.GetCustomAttributes(typeof(TableAttribute), true);
            if (a.Length > 0) {
                var ta = (a[0] as TableAttribute);
                ti.SchemaName = ta.SchemaName;
                ti.TableName = ta.TableName;
                ti.FixTag = ta.FixTag;
            } else {
                ti.TableName = type.Name;
            }
            var primaryKey = type.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).FirstOrDefault() as PrimaryKeyAttribute;

            if (primaryKey != null) {
                ti.PrimaryKey = primaryKey.PrimaryKey;
                ti.SequenceName = primaryKey.SequenceName;
                ti.AutoIncrement = primaryKey.AutoIncrement;
                return ti;
            }

            var prop = type.GetProperties().FirstOrDefault(p => {
                if (p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (p.Name.Equals(type.Name + "Id", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (p.Name.Equals(type.Name + "_Id", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (p.Name.Equals(ti.TableName + "Id", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (p.Name.Equals(ti.TableName + "_Id", StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            });
            if (prop != null) {
                ti.PrimaryKey = prop.Name;
                ti.AutoIncrement = ColumnType.IsNumericType(prop.PropertyType);
                ti.SequenceName = null;
            }

            return ti;
        }

        private PocoColumn FromProperty(PropertyInfo pi)
        {
            if (pi.CanRead == false || pi.CanWrite == false) return null;
            if (ColumnType.IsAllowType(pi.PropertyType) == false) return null;
            var a = pi.GetCustomAttributes(typeof(IgnoreAttribute), true);
            if (a.Length > 0) return null;

            var ci = new PocoColumn();
            ci.PropertyName = pi.Name;
            var colAttrs = pi.GetCustomAttributes(typeof(ColumnAttribute), true);

            if (colAttrs.Length > 0) {
                var colattr = (ColumnAttribute)colAttrs[0];

                ci.ColumnName = colattr.Name == null ? pi.Name : colattr.Name;
                ci.ForceToUtc = colattr.ForceToUtc;
                if ((colattr as ResultColumnAttribute) != null) {
                    ci.ResultColumn = true;
                    ci.ResultSql = (colattr as ResultColumnAttribute).Definition;
                }
            } else {
                ci.ColumnName = pi.Name;
                ci.ForceToUtc = false;
                ci.ResultColumn = false;
            }
            ci.PropertyInfo = pi;
            return ci;
        }

        #endregion PocoData 初始化 及常用属性

        #region 私有静态变量

        private static List<Func<object, object>> _converters = new List<Func<object, object>>();
        private static MethodInfo fnGetValue;//= typeof(IDataRecord).GetMethod("GetValue", new Type[] { typeof(int) });
        private static MethodInfo fnIsDBNull;//= typeof(IDataRecord).GetMethod("IsDBNull");
        private static FieldInfo fldConverters;// = typeof(PocoData).GetField("_converters", BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic);
        private static MethodInfo fnListGetItem;//= typeof(List<Func<object, object>>).GetProperty("Item").GetGetMethod();
        private static MethodInfo fnInvoke = typeof(Func<object, object>).GetMethod("Invoke");

        private Cache<string, Delegate> PocoFactories = new Cache<string, Delegate>();

        static PocoData()
        {
            var type = typeof(List<Func<object, object>>);
            fnListGetItem = type.GetProperty("Item").GetGetMethod();
            //var fields = typeof(PocoData).GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic);
            foreach (var field in typeof(PocoData).GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic)) {
                if (field.FieldType == type) {
                    fldConverters = field;
                    break;
                }
            }
            var type2 = typeof(IDataRecord);
            fnGetValue = type2.GetMethod("GetValue", new Type[] { typeof(int) });
            fnIsDBNull = type2.GetMethod("IsDBNull");

        }

        #endregion 私有静态变量

        #region GetFactory

        public Delegate GetFactory(SqlType type, string prefix, IDataReader reader)
        {
            var countColumns = reader.FieldCount;

            #region 创建Key
            SortedSet<string> list = new SortedSet<string>();
            for (int i = 0; i < countColumns; i++) {
                list.Add(reader.GetName(i));
            }
            list.Add(prefix);
            list.Add(Type.FullName);
            list.Add(type.ToString());
            StringBuilder sb = new StringBuilder();
            foreach (var item in list) {
                sb.Append(item);
                sb.Append("|");
            }
            var key = sb.ToString();
            #endregion 创建Key

            return PocoFactories.Get(key, () => {
                // Create the method
                var m = new DynamicMethod("toolgood_" + PocoFactories.Count.ToString(), Type, new Type[] { typeof(IDataReader), typeof(string) }, true);
                var il = m.GetILGenerator();

                if (Type == typeof(object)) {
                    // var poco=new T()
                    il.Emit(OpCodes.Newobj, typeof(System.Dynamic.ExpandoObject).GetConstructor(Type.EmptyTypes));          // obj

                    MethodInfo fnAdd = typeof(IDictionary<string, object>).GetMethod("Add");

                    // Enumerate all fields generating a set assignment for the column
                    for (int i = 0; i < countColumns; i++) {
                        var srcType = reader.GetFieldType(i);

                        il.Emit(OpCodes.Dup); // obj, obj
                        il.Emit(OpCodes.Ldstr, reader.GetName(i)); // obj, obj, fieldname

                        // r[i]
                        il.Emit(OpCodes.Ldarg_0); // obj, obj, fieldname, converter?,    rdr
                        il.Emit(OpCodes.Ldc_I4, i); // obj, obj, fieldname, converter?,  rdr,i
                        il.Emit(OpCodes.Callvirt, fnGetValue); // obj, obj, fieldname, converter?,  value

                        // Convert DBNull to null
                        il.Emit(OpCodes.Dup); // obj, obj, fieldname, converter?,  value, value
                        il.Emit(OpCodes.Isinst, typeof(DBNull)); // obj, obj, fieldname, converter?,  value, (value or null)
                        var lblNotNull = il.DefineLabel();
                        il.Emit(OpCodes.Brfalse_S, lblNotNull); // obj, obj, fieldname, converter?,  value
                        il.Emit(OpCodes.Pop); // obj, obj, fieldname, converter?
                                              //if (converter != null)
                                              //	il.Emit(OpCodes.Pop); // obj, obj, fieldname,
                        il.Emit(OpCodes.Ldnull); // obj, obj, fieldname, null
                                                 //if (converter != null) {
                                                 //	var lblReady = il.DefineLabel();
                                                 //	il.Emit(OpCodes.Br_S, lblReady);
                                                 //	il.MarkLabel(lblNotNull);
                                                 //	il.Emit(OpCodes.Callvirt, fnInvoke);
                                                 //	il.MarkLabel(lblReady);
                                                 //} else {
                        il.MarkLabel(lblNotNull);
                        //}

                        il.Emit(OpCodes.Callvirt, fnAdd);
                    }
                } else if (Type.IsValueType || Type == typeof(string) || Type == typeof(byte[])) {
                    // Do we need to install a converter?
                    var srcType = reader.GetFieldType(0);
                    var converter = GetConverter(null, srcType, Type);

                    // "if (!rdr.IsDBNull(i))"
                    il.Emit(OpCodes.Ldarg_0); // rdr
                    il.Emit(OpCodes.Ldc_I4_0); // rdr,0
                    il.Emit(OpCodes.Callvirt, fnIsDBNull); // bool
                    var lblCont = il.DefineLabel();
                    il.Emit(OpCodes.Brfalse_S, lblCont);
                    il.Emit(OpCodes.Ldnull); // null
                    var lblFin = il.DefineLabel();
                    il.Emit(OpCodes.Br_S, lblFin);

                    il.MarkLabel(lblCont);

                    // Setup stack for call to converter
                    AddConverterToStack(il, converter);

                    il.Emit(OpCodes.Ldarg_0); // rdr
                    il.Emit(OpCodes.Ldc_I4_0); // rdr,0
                    il.Emit(OpCodes.Callvirt, fnGetValue); // value

                    // Call the converter
                    if (converter != null)
                        il.Emit(OpCodes.Callvirt, fnInvoke);

                    il.MarkLabel(lblFin);
                    il.Emit(OpCodes.Unbox_Any, Type); // value converted
                } else {
                    // var poco=new T()
                    var ctor = Type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
                    if (ctor == null)
                        throw new InvalidOperationException("Type [" + Type.FullName + "] should have default public or non-public constructor");

                    il.Emit(OpCodes.Newobj, ctor);

                    // Enumerate all fields generating a set assignment for the column
                    for (int i = 0; i < countColumns; i++) {
                        var name = reader.GetName(i);
                        PocoColumn pc;
                        pc = Columns.FirstOrDefault(q => prefix + "_" + q.ColumnName == name);
                        if (pc == null && prefix == "t1") {
                            pc = Columns.FirstOrDefault(q => q.ColumnName == name);
                        }
                        if (pc == null) continue;

                        // Get the source type for this column
                        var srcType = reader.GetFieldType(i);
                        var dstType = pc.PropertyInfo.PropertyType;

                        // "if (!rdr.IsDBNull(i))"
                        il.Emit(OpCodes.Ldarg_0); // poco,rdr
                        il.Emit(OpCodes.Ldc_I4, i); // poco,rdr,i
                        il.Emit(OpCodes.Callvirt, fnIsDBNull); // poco,bool
                        var lblNext = il.DefineLabel();
                        il.Emit(OpCodes.Brtrue_S, lblNext); // poco

                        il.Emit(OpCodes.Dup); // poco,poco

                        // Do we need to install a converter?
                        var converter = GetConverter(pc, srcType, dstType);

                        // Fast
                        bool Handled = false;
                        if (converter == null) {
                            var valuegetter = typeof(IDataRecord).GetMethod("Get" + srcType.Name, new Type[] { typeof(int) });
                            if (valuegetter != null
                                && valuegetter.ReturnType == srcType
                                && (valuegetter.ReturnType == dstType || valuegetter.ReturnType == Nullable.GetUnderlyingType(dstType))) {
                                il.Emit(OpCodes.Ldarg_0); // *,rdr
                                il.Emit(OpCodes.Ldc_I4, i); // *,rdr,i
                                il.Emit(OpCodes.Callvirt, valuegetter); // *,value

                                // Convert to Nullable
                                if (Nullable.GetUnderlyingType(dstType) != null) {
                                    il.Emit(OpCodes.Newobj, dstType.GetConstructor(new Type[] { Nullable.GetUnderlyingType(dstType) }));
                                }

                                il.Emit(OpCodes.Callvirt, pc.PropertyInfo.GetSetMethod(true)); // poco
                                Handled = true;
                            }
                        }

                        // Not so fast
                        if (!Handled) {
                            // Setup stack for call to converter
                            AddConverterToStack(il, converter);

                            // "value = rdr.GetValue(i)"
                            il.Emit(OpCodes.Ldarg_0); // *,rdr
                            il.Emit(OpCodes.Ldc_I4, i); // *,rdr,i
                            il.Emit(OpCodes.Callvirt, fnGetValue); // *,value

                            // Call the converter
                            if (converter != null)
                                il.Emit(OpCodes.Callvirt, fnInvoke);

                            // Assign it
                            il.Emit(OpCodes.Unbox_Any, pc.PropertyInfo.PropertyType); // poco,poco,value
                            il.Emit(OpCodes.Callvirt, pc.PropertyInfo.GetSetMethod(true)); // poco
                        }

                        il.MarkLabel(lblNext);
                    }

                    var fnOnLoaded = RecurseInheritedTypes<MethodInfo>(Type,
                        (x) => x.GetMethod("OnLoaded", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null));
                    if (fnOnLoaded != null) {
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Callvirt, fnOnLoaded);
                    }
                }

                il.Emit(OpCodes.Ret);

                return m.CreateDelegate(Expression.GetFuncType(typeof(IDataReader), typeof(string), Type));
            }
               );
        }

        #endregion GetFactory

        #region 私有静态方法

        private static bool IsIntegralType(Type type)
        {
            var tc = Type.GetTypeCode(type);
            return tc >= TypeCode.SByte && tc <= TypeCode.UInt64;
        }

        private static void AddConverterToStack(ILGenerator il, Func<object, object> converter)
        {
            if (converter != null) {
                // Add the converter
                int converterIndex = _converters.Count;
                _converters.Add(converter);

                // Generate IL to push the converter onto the stack
                il.Emit(OpCodes.Ldsfld, fldConverters);
                il.Emit(OpCodes.Ldc_I4, converterIndex);
                il.Emit(OpCodes.Callvirt, fnListGetItem); // Converter
            }
        }

        private static Func<object, object> GetConverter(PocoColumn pc, Type srcType, Type dstType)
        {
            if (srcType == dstType) return null;

            // Standard DateTime->Utc mapper
            if (pc != null && pc.ForceToUtc && srcType == typeof(DateTime) && (dstType == typeof(DateTime) || dstType == typeof(DateTime?))) {
                return delegate (object src) { return new DateTime(((DateTime)src).Ticks, DateTimeKind.Utc); };
            }

            // unwrap nullable types
            Type underlyingDstType = Nullable.GetUnderlyingType(dstType);
            if (underlyingDstType != null) {
                dstType = underlyingDstType;
            }

            // Forced type conversion including integral types -> enum
            if (dstType.IsEnum && IsIntegralType(srcType)) {
                var backingDstType = Enum.GetUnderlyingType(dstType);
                if (underlyingDstType != null) {
                    // if dstType is Nullable<Enum>, convert to enum value
                    return delegate (object src) { return Enum.ToObject(dstType, src); };
                } else if (srcType != backingDstType) {
                    return delegate (object src) { return Convert.ChangeType(src, backingDstType, null); };
                }
            } else if (!dstType.IsAssignableFrom(srcType)) {
                if (dstType.IsEnum && srcType == typeof(string)) {
                    return delegate (object src) { return EnumMapper.EnumFromString(dstType, (string)src); };
                } else if (dstType == typeof(Guid) && srcType == typeof(string)) {
                    return delegate (object src) { return Guid.Parse((string)src); };
                } else {
                    return delegate (object src) { return Convert.ChangeType(src, dstType, null); };
                }
            }

            return null;
        }

        private static T RecurseInheritedTypes<T>(Type t, Func<Type, T> cb)
        {
            while (t != null) {
                T info = cb(t);
                if (info != null)
                    return info;
                t = t.BaseType;
            }
            return default(T);
        }

        #endregion 私有静态方法

        internal static void FlushCaches()
        {
            _pocoDatas.Flush();
        }
    }
}