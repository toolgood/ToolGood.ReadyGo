using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 字符串转字节标签：string 以 UTF-8 编码的 byte[]（BLOB 列）保存。
    /// 基于 SerializedColumn + IColumnSerializer 实现，数据库中仅存二进制数据。
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class String2BytesAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public override String2BytesColumnSerializer Serializer => DefaultSerializer;

        private static readonly String2BytesColumnSerializer DefaultSerializer = new String2BytesColumnSerializer();

        /// <summary>
        /// 字符串转字节标签
        /// </summary>
        public String2BytesAttribute()
        {
        }

        /// <summary>
        /// 字符串转字节标签
        /// </summary>
        /// <param name="name">列名</param>
        public String2BytesAttribute(string name) : base(name)
        {
        }
    }
}
