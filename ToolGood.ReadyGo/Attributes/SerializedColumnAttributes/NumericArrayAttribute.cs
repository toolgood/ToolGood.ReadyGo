using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 数值数组标签：将 float[] / double[] / int[] 及其 List&lt;T&gt; 以 byte[]（BLOB 列）保存。
    /// 基于 SerializedColumn + IColumnSerializer 实现，数据库中仅存二进制数据。
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NumericArrayAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public static NumericArrayColumnSerializer Serializer { get; } = new NumericArrayColumnSerializer();

        /// <summary>
        /// 数值数组标签
        /// </summary>
        public NumericArrayAttribute()
        {
        }

        /// <summary>
        /// 数值数组标签
        /// </summary>
        /// <param name="name">列名</param>
        public NumericArrayAttribute(string name) : base(name)
        {
        }
    }
}
