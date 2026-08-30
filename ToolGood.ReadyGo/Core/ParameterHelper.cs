using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 参数处理帮助类，用于解析 SQL 中的命名参数、展开集合参数以及设置命令参数值。
    /// </summary>
    public class ParameterHelper
    {
        /// <summary>
        /// 需要排除在集合展开之外的 <see cref="IEnumerable"/> 类型列表。
        /// </summary>
        public static List<Type> ExcludedIEnumerableTypes = new();

        // Helper to handle named parameters from object properties
        /// <summary>
        /// 用于匹配命名参数的正则表达式。
        /// </summary>
        public static Regex rxParamsPrefix = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);

        /// <summary>
        /// 处理 SQL 中的命名参数，将其替换为目标参数列表中的位置参数。
        /// </summary>
        /// <param name="sql">原始 SQL 语句。</param>
        /// <param name="args_src">原始参数数组。</param>
        /// <param name="args_dest">目标参数列表。</param>
        /// <param name="reuseParameters">是否复用相同参数值。</param>
        /// <returns>处理后的 SQL 语句。</returns>
        public static string ProcessParams(string sql, object[] args_src, List<object> args_dest, bool reuseParameters = false)
        {
            var parameters = new Dictionary<string, string>();
            return rxParamsPrefix.Replace(sql, m =>
            {
                string item;
                if (parameters.TryGetValue(m.Value, out item))
                    return item;

                item = parameters[m.Value] = ProcessParam(ref sql, m.Value, args_src, args_dest, reuseParameters);
                return item;
            });
        }
        
        private static string ProcessParam(ref string sql, string rawParam, object[] args_src, List<object> args_dest, bool reuseParameters)
        {
            string param = rawParam.Substring(1);

            object arg_val;

            int paramIndex;
            if (int.TryParse(param, out paramIndex))
            {
                // Numbered parameter
                if (paramIndex < 0 || paramIndex >= args_src.Length)
                    throw new ArgumentOutOfRangeException(String.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", paramIndex, args_src.Length, sql));
                arg_val = args_src[paramIndex];
            }
            else
            {
                // Look for a property on one of the arguments with this name
                bool found = false;
                arg_val = null;
                foreach (var o in args_src)
                {
                    var dict = o as IDictionary;
                    if (dict != null)
                    {
                        Type[] arguments = dict.GetType().GetGenericArguments();

                        if (arguments[0] == typeof(string))
                        {
                            var val = dict[param];
                            if (val != null)
                            {
                                found = true;
                                arg_val = val;
                                break;
                            }
                        }
                    }

                    var type = o.GetType();
                    var pi = type.GetProperty(param);
                    if (pi != null)
                    {
                        arg_val = pi.GetValue(o, null);
                        found = true;
                        break;
                    }

                    var fi =  type.GetField(param);
                    if (fi != null)
                    {
                        arg_val = fi.GetValue(o);
                        found = true;
                        break;
                    }
                }

                if (!found)
                    throw new ArgumentException(String.Format("Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')", param, sql));
            }

            // Expand collections to parameter lists
            if ((arg_val as System.Collections.IEnumerable) != null &&
                (arg_val as string) == null &&
                (arg_val as byte[]) == null &&
                !ExcludedIEnumerableTypes.Contains(arg_val.GetTheType()))
            {
                var sb = new StringBuilder();
                foreach (var i in arg_val as System.Collections.IEnumerable)
                {
                    var indexOfExistingValue = args_dest.IndexOf(i);
                    if (indexOfExistingValue >= 0)
                    {
                        sb.Append((sb.Length == 0 ? "@" : ",@") + indexOfExistingValue);
                    }
                    else
                    {
                        sb.Append((sb.Length == 0 ? "@" : ",@") + args_dest.Count);
                        args_dest.Add(i);
                    }
                }
                if (sb.Length == 0)
                {
                    Type type = typeof(string);
                    var t = arg_val.GetType();
                    if (t.IsArray)
                        type = t.GetElementType();
                    else if (t.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>)))
                        type = t.GetGenericArguments().First();

                    sb.AppendFormat($"select @{args_dest.Count} /*poco_dual*/ where 1 = 0");
                    args_dest.Add(GetDefault(type));
                }
                return sb.ToString();
            }
            else
            {
                if (reuseParameters)
                {
                    var indexOfExistingValue = args_dest.IndexOf(arg_val);
                    if (indexOfExistingValue >= 0)
                        return "@" + indexOfExistingValue;
                }

                args_dest.Add(arg_val);
                return "@" + (args_dest.Count - 1).ToString();
            }
        }

        /// <summary>
        /// 获取指定类型的默认值。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <returns>值类型返回其默认实例，引用类型返回 null。</returns>
        public static object GetDefault(Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        /// <summary>
        /// 序列化输出为 byte[] 的序列化器类型映射，用于列值为 null 时推断数据库参数类型。
        /// 新增输出 byte[] 的序列化器时需在此登记。
        /// </summary>
        private static readonly Dictionary<Type, Type> SerializerOutputTypes = new() {
            { typeof(NumericArray2BytesColumnSerializer), typeof(byte[]) },
            { typeof(String2BytesColumnSerializer), typeof(byte[]) },
            { typeof(DictionaryUintUint2BytesColumnSerializer), typeof(byte[]) },
        };

        /// <summary>
        /// 携带明确 DbType 的 NULL 参数值。
        /// 框架在构建 Insert/Update 参数时用它包装 null，避免 SqlClient 等驱动将 NULL 默认推断为 nvarchar 而无法写入二进制列。
        /// </summary>
        internal sealed class TypedNullValue
        {
            public TypedNullValue(DbType dbType) { DbType = dbType; }
            public DbType DbType { get; }
        }

        /// <summary>
        /// 包装 null 参数：为 null 值推断并携带明确的 DbType。
        /// 优先使用序列化器的输出类型（白名单，如输出 byte[] 的序列化器 → DbType.Binary），
        /// 否则回退到目标列 CLR 类型推断；仍无法确定时返回 null（保持原行为）。
        /// </summary>
        /// <param name="dbType">数据库类型提供程序。</param>
        /// <param name="pocoColumn">目标列信息。</param>
        /// <param name="value">参数值。</param>
        /// <returns>非 null 原样返回；null 返回带 DbType 的包装值或 null。</returns>
        public static object WrapNullWithDbType(IDatabaseType dbType, PocoColumn pocoColumn, object value)
        {
            if (value != null) return value;
            Type outputType = null;
            if (pocoColumn.ColumnSerializer != null && SerializerOutputTypes.TryGetValue(pocoColumn.ColumnSerializer.GetType(), out var serializedType)) {
                outputType = serializedType;
            }
            outputType ??= pocoColumn.ColumnType;
            var dbTypeLookup = dbType.LookupDbType(outputType, pocoColumn.ColumnName);
            return dbTypeLookup.HasValue ? new TypedNullValue(dbTypeLookup.Value) : null;
        }

        /// <summary>
        /// 将值设置到命令参数中，并根据类型进行必要的转换。
        /// </summary>
        /// <param name="dbType">数据库类型。</param>
        /// <param name="p">要设置的命令参数。</param>
        /// <param name="value">参数值。</param>
        public static void SetParameterValue(IDatabaseType dbType, DbParameter p, object value)
        {
            if (value == null)
            {
                p.Value = DBNull.Value;
                return;
            }

            // Give the database type first crack at converting to DB required type
            value = dbType.MapParameterValue(value);

            var dbtypeSet = false;
            var t = value.GetType();
            var underlyingT = Nullable.GetUnderlyingType(t);
            if (t.GetTypeInfo().IsEnum || (underlyingT != null && underlyingT.GetTypeInfo().IsEnum))        // PostgreSQL .NET driver wont cast enum to int
            {
                p.Value = (int)value;
            }
            else if (t == typeof(Guid))
            {
                p.Value = value;
                p.DbType = DbType.Guid;
                p.Size = 40;
                dbtypeSet = true;
            }
            else if (t == typeof(string))
            {
                var strValue = value as string;
                if (strValue == null)
                {
                    p.Size = 0;
                    p.Value = DBNull.Value;
                }
                else
                {
                    // out of memory exception occurs if trying to save more than 4000 characters to SQL Server CE NText column. Set before attempting to set Size, or Size will always max out at 4000
                    if (strValue.Length + 1 > 4000 && p.GetType().Name == "SqlCeParameter")
                    {
                        p.GetType().GetProperty("SqlDbType").SetValue(p, SqlDbType.NText, null);
                    }

                    p.Size = Math.Max(strValue.Length + 1, 4000); // Help query plan caching by using common size
                    p.Value = value;
                }
            }
            else if (t == typeof(AnsiString))
            {
                var ansistrValue = value as AnsiString;
                if (ansistrValue?.Value == null)
                {
                    p.Size = 0;
                    p.Value = DBNull.Value;
                    p.DbType = DbType.AnsiString;
                }
                else
                {
                    // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                    p.Size = Math.Max(ansistrValue.Value.Length + 1, 4000);
                    p.Value = ansistrValue.Value;
                    p.DbType = DbType.AnsiString;
                }
                dbtypeSet = true;
            }
            else if (value.GetType().Name == "SqlGeography") //SqlGeography is a CLR Type
            {
                p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null); //geography is the equivalent SQL Server Type
                p.Value = value;
            }

            else if (value.GetType().Name == "SqlGeometry") //SqlGeometry is a CLR Type
            {
                p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null); //geography is the equivalent SQL Server Type
                p.Value = value;
            }
            else
            {
                p.Value = value;
            }

            if (!dbtypeSet)
            {
                var dbTypeLookup = dbType.LookupDbType(p.Value.GetTheType(), p.ParameterName);
                if (dbTypeLookup.HasValue)
                {
                    p.DbType = dbTypeLookup.Value;
                }
            }
        }
    }
}
