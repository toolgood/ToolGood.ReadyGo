using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 数值数组转字符串标签：将 int[] / long[] / double[] / decimal[] 等数值数组及其 List&lt;T&gt;
    /// 以分隔符文本保存（需文本列）。默认逗号分隔，支持自定义分隔符。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NumericArray2StringAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public NumericArray2StringColumnSerializer Serializer { get; }

        /// <summary>
        /// 数值数组转字符串标签
        /// </summary>
        /// <param name="separator">分隔符，默认逗号</param>
        public NumericArray2StringAttribute(string separator = ",")
        {
            Serializer = new NumericArray2StringColumnSerializer(separator);
        }

        /// <summary>
        /// 数值数组转字符串标签
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="separator">分隔符，默认逗号</param>
        public NumericArray2StringAttribute(string name, string separator) : base(name)
        {
            Serializer = new NumericArray2StringColumnSerializer(separator);
        }
    }
}
