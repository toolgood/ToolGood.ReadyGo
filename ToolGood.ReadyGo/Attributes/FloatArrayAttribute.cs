using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// float 数组标签：将 float[] / List&lt;float&gt; 以 byte[]（BLOB 列）保存。
    /// 基于 SerializedColumn + IColumnSerializer 实现，数据库中仅存二进制数据。
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FloatArrayAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public static FloatArrayColumnSerializer Serializer { get; } = new FloatArrayColumnSerializer();

        /// <summary>
        /// float 数组标签
        /// </summary>
        public FloatArrayAttribute()
        {
        }

        /// <summary>
        /// float 数组标签
        /// </summary>
        /// <param name="name">列名</param>
        public FloatArrayAttribute(string name) : base(name)
        {
        }
    }
}
