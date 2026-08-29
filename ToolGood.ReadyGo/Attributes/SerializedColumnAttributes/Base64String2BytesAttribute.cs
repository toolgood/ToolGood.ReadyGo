using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// Base64 字符串转字节标签：byte[] 以 Base64 字符串（VARCHAR/TEXT 列）保存。
    /// 基于 SerializedColumn + IColumnSerializer 实现，数据库中仅存 Base64 文本。
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class Base64String2BytesAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public override Base64String2BytesColumnSerializer Serializer => DefaultSerializer;

        private static readonly Base64String2BytesColumnSerializer DefaultSerializer = new Base64String2BytesColumnSerializer();

        /// <summary>
        /// Base64 字符串转字节标签
        /// </summary>
        public Base64String2BytesAttribute()
        {
        }

        /// <summary>
        /// Base64 字符串转字节标签
        /// </summary>
        /// <param name="name">列名</param>
        public Base64String2BytesAttribute(string name) : base(name)
        {
        }
    }
}
