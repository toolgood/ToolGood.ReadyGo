using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 字符串列表标签：List&lt;string&gt; / string[] 以分隔符文本保存（需文本列）。
    /// 默认逗号分隔，支持自定义分隔符与转义（\ 与分隔符前加 \）。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class StringArray2StringAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public override StringArray2StringColumnSerializer Serializer { get; }

        /// <summary>
        /// 字符串数组标签
        /// </summary>
        /// <param name="separator">分隔符，默认逗号</param>
        public StringArray2StringAttribute(string separator = ",")
        {
            Serializer = new StringArray2StringColumnSerializer(separator);
        }

        /// <summary>
        /// 字符串数组标签
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="separator">分隔符，默认逗号</param>
        public StringArray2StringAttribute(string name, string separator) : base(name)
        {
            Serializer = new StringArray2StringColumnSerializer(separator);
        }
    }
}
