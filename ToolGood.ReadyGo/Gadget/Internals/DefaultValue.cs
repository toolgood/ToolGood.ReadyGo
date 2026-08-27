using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget.Internals
{
    /// <summary>
    /// 默认值生成
    /// </summary>
    internal class DefaultValue
    {
        private static readonly Cache<string, Delegate> _setDefault = new Cache<string, Delegate>();

        /// <summary>
        /// 设置默认值
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="obj">要设置默认值的对象</param>
        /// <param name="setString">是否将空字符串列设为非空字符串</param>
        /// <param name="setDateTime">是否将默认时间列设为当前时间</param>
        /// <param name="setGuid">是否为空的 Guid 列生成新值</param>
        /// <param name="pd">POCO 元数据</param>
        public static void SetDefaultValue<T>(T obj, bool setString, bool setDateTime, bool setGuid, PocoData pd)
        {
            if (pd == null) throw new ArgumentNullException(nameof(pd));
            // 缓存 key 需包含 PocoData 维度：同一类型映射到不同表（不同 PocoData）时，列集合不同；
            // AssemblyQualifiedName 用于区分跨程序集的同名类型，避免缓存串用
            var key = typeof(T).AssemblyQualifiedName + "|" + pd.TableInfo.TableName + "|"
                + string.Join(",", pd.Columns.Keys.OrderBy(k => k, StringComparer.Ordinal));
            var action = _setDefault.Get(key, () => CreateDefaultFunction<T>(pd));
            var a = (action as Action<T, bool, bool, bool>);
            a(obj, setString, setDateTime, setGuid);
        }

        private static Delegate CreateDefaultFunction<T>(PocoData pd)
        {
            #region 初始时间

            List<PropertyInfo> datetimes = new List<PropertyInfo>();
            List<PropertyInfo> datetimeoffsets = new List<PropertyInfo>();
            List<PropertyInfo> strings = new List<PropertyInfo>();
            List<PropertyInfo> ansiStrings = new List<PropertyInfo>();
            List<PropertyInfo> guids = new List<PropertyInfo>();
            foreach (var item in pd.Columns) {
                if (item.Value.ResultColumn) continue;
                var pi = item.Value.MemberInfoData?.MemberInfo as PropertyInfo;
                if (pi == null) continue;
                if (pi.PropertyType == typeof(DateTime)) {
                    datetimes.Add(pi);
                } else if (pi.PropertyType == typeof(DateTimeOffset)) {
                    datetimeoffsets.Add(pi);
                } else if (pi.PropertyType == typeof(string)) {
                    strings.Add(pi);
                } else if (pi.PropertyType == typeof(Guid)) {
                    guids.Add(pi);
                } else if (pi.PropertyType == typeof(AnsiString)) {
                    ansiStrings.Add(pi);
                }
            }

            #endregion 初始时间

            #region dateTimeType dateTimeOffsetType AnsiString

            var dateTimeType = typeof(DateTime);
            //var getYear = dateTimeType.GetProperty("Year");
            var getNow = dateTimeType.GetProperty("Now", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            var getMinValue = dateTimeType.GetField("MinValue", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            var getop_Equality = dateTimeType.GetMethod("op_Equality", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);

            var dateTimeOffsetType = typeof(DateTimeOffset);
            var getNow2 = dateTimeOffsetType.GetProperty("Now", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            var getMinValue2 = dateTimeOffsetType.GetField("MinValue", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            var getop_Equality2 = dateTimeOffsetType.GetMethod("op_Equality", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);

            var guidType = typeof(Guid);
            var getEmpty = guidType.GetField("Empty", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            var getNewGuid = guidType.GetMethod("NewGuid", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            var getop_Equality3 = guidType.GetMethod("op_Equality", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);

            var asctor = typeof(AnsiString).GetConstructor(new Type[] { typeof(string) });

            #endregion dateTimeType dateTimeOffsetType AnsiString

            var m = new DynamicMethod("tg_def_" + Guid.NewGuid().ToString().Replace("-", ""), typeof(void), new Type[] { typeof(T), typeof(bool), typeof(bool), typeof(bool) }, true);
            var il = m.GetILGenerator();

            #region string

            if (strings.Count > 0) {
                il.Emit(OpCodes.Ldarg_1);
                var lab1 = il.DefineLabel();
                il.Emit(OpCodes.Brfalse, lab1);
                for (int i = 0; i < strings.Count; i++) {
                    var item = strings[i];

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Callvirt, item.GetGetMethod());
                    var lab = il.DefineLabel();
                    il.Emit(OpCodes.Brtrue, lab);

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldstr, "");
                    il.Emit(OpCodes.Callvirt, item.GetSetMethod());
                    il.MarkLabel(lab);
                }
                il.MarkLabel(lab1);
            }

            #endregion string

            #region AnsiString

            if (ansiStrings.Count > 0) {
                il.Emit(OpCodes.Ldarg_1);
                var lab1 = il.DefineLabel();
                il.Emit(OpCodes.Brfalse, lab1);

                for (int i = 0; i < ansiStrings.Count; i++) {
                    var item = ansiStrings[i];
                    var lab = il.DefineLabel();

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Callvirt, item.GetGetMethod());
                    il.Emit(OpCodes.Brtrue, lab);

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldstr, "");
                    il.Emit(OpCodes.Newobj, asctor);
                    il.Emit(OpCodes.Callvirt, item.GetSetMethod());
                    il.MarkLabel(lab);
                }
                il.MarkLabel(lab1);
            }

            #endregion AnsiString

            #region date

            if (datetimes.Count + datetimeoffsets.Count > 0) {
                il.Emit(OpCodes.Ldarg_2);
                var lab2 = il.DefineLabel();
                il.Emit(OpCodes.Brfalse, lab2);

                #region datetimes

                foreach (var item in datetimes) {
                    var lab = il.DefineLabel();
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Callvirt, item.GetGetMethod());
                    il.Emit(OpCodes.Ldsfld, getMinValue);
                    il.Emit(OpCodes.Call, getop_Equality);
                    il.Emit(OpCodes.Brfalse, lab);

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Call, getNow.GetGetMethod());
                    il.Emit(OpCodes.Callvirt, item.GetSetMethod());
                    il.MarkLabel(lab);
                }

                #endregion datetimes

                #region datetimeoffsets

                foreach (var item in datetimeoffsets) {
                    var lab = il.DefineLabel();
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Callvirt, item.GetGetMethod());
                    il.Emit(OpCodes.Ldsfld, getMinValue2);
                    il.Emit(OpCodes.Call, getop_Equality2);
                    il.Emit(OpCodes.Brfalse, lab);

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Call, getNow2.GetGetMethod());
                    il.Emit(OpCodes.Callvirt, item.GetSetMethod());
                    il.MarkLabel(lab);
                }

                #endregion datetimeoffsets

                il.MarkLabel(lab2);
            }

            #endregion date

            #region guid

            if (guids.Count > 0) {
                il.Emit(OpCodes.Ldarg_3);
                var lab3 = il.DefineLabel();
                il.Emit(OpCodes.Brfalse, lab3);

                foreach (var item in guids) {
                    var lab = il.DefineLabel();

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Callvirt, item.GetGetMethod());
                    il.Emit(OpCodes.Ldsfld, getEmpty);
                    il.Emit(OpCodes.Call, getop_Equality3);
                    il.Emit(OpCodes.Brfalse, lab);

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Call, getNewGuid);
                    il.Emit(OpCodes.Callvirt, item.GetSetMethod());
                    il.MarkLabel(lab);
                }
                il.MarkLabel(lab3);
            }

            #endregion guid

            il.Emit(OpCodes.Ret);
            return m.CreateDelegate(Expression.GetActionType(typeof(T), typeof(bool), typeof(bool), typeof(bool)));
        }
    }
}
