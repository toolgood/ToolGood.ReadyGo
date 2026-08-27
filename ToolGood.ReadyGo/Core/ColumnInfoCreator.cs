using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToolGood.ReadyGo.Attributes;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 根据成员上的特性创建对应的 <see cref="ColumnInfo"/> 元数据。
    /// </summary>
    public class ColumnInfoCreator
    {
        /// <summary>
        /// 从成员信息（属性或字段）中读取映射特性并构造列元数据。
        /// </summary>
        /// <param name="mi">成员信息。</param>
        /// <returns>构造出的列元数据。</returns>
        public static ColumnInfo FromMemberInfo(MemberInfo mi)
        {
            var ci = new ColumnInfo { MemberInfo = mi };
            var attrs = ReflectionUtils.GetCustomAttributes(mi).ToArray();
            var colAttrs = attrs.OfType<ColumnAttribute>().ToArray();
            var columnTypeAttrs = attrs.OfType<ColumnTypeAttribute>().ToArray();
            var ignoreAttrs = attrs.OfType<IgnoreAttribute>().ToArray();
            var complexMapping = attrs.OfType<ComplexMappingAttribute>().ToArray();
            var serializedColumnAttributes = attrs.OfType<SerializedColumnAttribute>().ToArray();
            var reference = attrs.OfType<ReferenceAttribute>().ToArray();
            var aliasColumn = attrs.OfType<AliasAttribute>().FirstOrDefault();
            // 探测带序列化器的 SerializedColumn 子类（如 [Date2Int] / [Numeric2Int]）
            var customColumnSerializer = serializedColumnAttributes
                .Select(a => a.Serializer)
                .FirstOrDefault(s => s != null);

            // Check if declaring poco has [ExplicitColumns] attribute
            var explicitColumns = mi.DeclaringType.GetTypeInfo().GetCustomAttributes(typeof(ExplicitColumnsAttribute), true).Any();

            // Ignore column if declarying poco has [ExplicitColumns] attribute
            // and property doesn't have an explicit [Column] attribute,
            // or property has an [Ignore] attribute
            if ((explicitColumns && !colAttrs.Any() && !reference.Any() && !complexMapping.Any()) || ignoreAttrs.Any())
            {
                ci.IgnoreColumn = true;
            }

            var complexMappingAttribute = mi.GetMemberInfoType().GetCustomAttribute<ComplexMappingAttribute>();

            if (complexMapping.Any())
            {
                ci.ComplexMapping = complexMapping.First().ComplexMapping;
                ci.ComplexPrefix = complexMapping.First().CustomPrefix;
            }
            else if (complexMappingAttribute != null)
            {
                ci.ComplexMapping = complexMappingAttribute.ComplexMapping;
                ci.ComplexPrefix = complexMappingAttribute.CustomPrefix;
            }
            else if (mi.GetMemberInfoType().GetInterfaces().Any(x => x == typeof(IValueObject)))
            {
                ci.ValueObjectColumn = true;
            }
            else if (serializedColumnAttributes.Any())
            {
                ci.SerializedColumn = true;
            }
            else if (IsNumericArrayType(mi.GetMemberInfoType()))
            {
                // float[] / double[] / int[] / decimal[] 及其 List<T> 默认以 byte[]（BLOB）保存
                ci.SerializedColumn = true;
            }
            else if (reference.Any())
            {
                ci.ReferenceType = reference.First().ReferenceType;
                ci.ReferenceMemberName = reference.First().ReferenceMemberName ?? "Id";
                ci.ColumnName = reference.First().ColumnName ?? GetReferenceColumnName(mi.Name);
                return ci;
            }
            else if (mi.GetMemberInfoType().IsOfGenericType(typeof(IList<>)) && !mi.GetMemberInfoType().IsArray)
            {
                ci.ReferenceType = ReferenceType.Many;
                return ci;
            }
            else if (mi.GetMemberInfoType().IsAClass() && !colAttrs.Any())
            {
                ci.ComplexMapping = true;
            }

            // Read attribute
            if (colAttrs.Any())
            {
                ci.ColumnName = colAttrs.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name))?.Name ?? mi.Name;
                ci.ForceToUtc = colAttrs.All(x => x.ForceToUtc);
                ci.ExactColumnNameMatch = colAttrs.All(x => x.ExactNameMatch);

                var resultAttr = colAttrs.OfType<ResultColumnAttribute>().FirstOrDefault();
                ci.ResultColumn = resultAttr != null;

                if (!ci.ResultColumn)
                {
                    var versionAttr = colAttrs.OfType<VersionColumnAttribute>().FirstOrDefault();
                    ci.VersionColumn = versionAttr != null;
                    ci.VersionColumnType = versionAttr?.VersionColumnType ?? ci.VersionColumnType;
                }

                if (!ci.VersionColumn && !ci.ResultColumn)
                {
                    var computedAttr = colAttrs.OfType<ComputedColumnAttribute>().FirstOrDefault();
                    ci.ComputedColumn = computedAttr != null;
                    ci.ComputedColumnType = computedAttr?.ComputedColumnType ?? ComputedColumnType.Always;
                }
            }
            else
            {
                ci.ColumnName = mi.Name;
            }

            ci.ColumnAlias = aliasColumn?.Alias;

            if (customColumnSerializer != null)
            {
                ci.SerializedColumn = true;
                ci.ColumnSerializer = customColumnSerializer;
            }
            else if (ci.SerializedColumn && ci.ColumnSerializer == null && IsNumericArrayType(mi.GetMemberInfoType()))
            {
                // 未显式指定序列化器的数值数组类型，默认按 NumericArray2Bytes（byte[]）保存
                ci.ColumnSerializer = new NumericArray2BytesAttribute().Serializer;
            }

            if (columnTypeAttrs.Any())
            {
                ci.ColumnType = columnTypeAttrs.First().Type;
            }

            return ci;
        }

        /// <summary>
        /// 生成默认引用外键列名：成员名已以 "Id" 结尾时直接使用，否则追加 "Id"。
        /// </summary>
        /// <param name="memberName">成员名。</param>
        /// <returns>默认外键列名。</returns>
        private static string GetReferenceColumnName(string memberName)
        {
            return memberName.EndsWith("Id", StringComparison.Ordinal) ? memberName : memberName + "Id";
        }

        /// <summary>
        /// 判断类型是否为默认按 byte[]（BLOB 列）保存的数值数组类型。
        /// </summary>
        /// <param name="type">成员类型。</param>
        /// <returns>若为 float[] / double[] / int[] / decimal[] 及其 List&lt;T&gt; 则返回 true。</returns>
        private static bool IsNumericArrayType(Type type)
        {
            return type == typeof(float[]) || type == typeof(double[]) || type == typeof(int[]) || type == typeof(decimal[])
                || type == typeof(List<float>) || type == typeof(List<double>) || type == typeof(List<int>) || type == typeof(List<decimal>);
        }
    }
}
