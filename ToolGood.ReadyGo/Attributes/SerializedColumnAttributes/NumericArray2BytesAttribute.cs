using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 数值数组标签：将 float[] / double[] / int[] 及其 List&lt;T&gt; 以 byte[]（BLOB 列）保存。
    /// 基于 SerializedColumn + IColumnSerializer 实现，数据库中仅存二进制数据。
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NumericArray2BytesAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public static NumericArray2BytesColumnSerializer Serializer { get; } = new NumericArray2BytesColumnSerializer();

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
