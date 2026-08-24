using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// uint→uint 字典标签：将 Dictionary&lt;uint, uint&gt; 以 byte[]（BLOB 列）保存。
    /// 价格按键升序、差值压缩存储，基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DictionaryUintUint2BytesAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public static DictionaryUintUint2BytesColumnSerializer Serializer { get; } = new DictionaryUintUint2BytesColumnSerializer();

        /// <summary>
        /// uint→uint 字典标签
        /// </summary>
        public DictionaryUintUint2BytesAttribute()
        {
        }

        /// <summary>
        /// uint→uint 字典标签
        /// </summary>
        /// <param name="name">列名</param>
        public DictionaryUintUint2BytesAttribute(string name) : base(name)
        {
        }
    }
}
