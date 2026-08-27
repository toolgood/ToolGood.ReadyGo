using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 数值数组标签：将 float[] / double[] / int[] / decimal[] 及其 List&lt;T&gt; 以 byte[]（BLOB 列）保存。
    /// 基于 SerializedColumn + IColumnSerializer 实现，数据库中仅存二进制数据。
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class NumericArray2BytesAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public override NumericArray2BytesColumnSerializer Serializer => DefaultSerializer;

        private static readonly NumericArray2BytesColumnSerializer DefaultSerializer = new NumericArray2BytesColumnSerializer();

        /// <summary>
        /// 数值数组标签
        /// </summary>
        public NumericArray2BytesAttribute()
        {
        }

        /// <summary>
        /// 数值数组标签
        /// </summary>
        /// <param name="name">列名</param>
        public NumericArray2BytesAttribute(string name) : base(name)
        {
        }
    }
}
