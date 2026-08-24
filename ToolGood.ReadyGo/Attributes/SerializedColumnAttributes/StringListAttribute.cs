using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 字符串列表标签：List&lt;string&gt; / string[] 以分隔符文本保存（需文本列）。
    /// 默认逗号分隔，支持自定义分隔符与转义（\ 与分隔符前加 \）。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StringListAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 列级序列化器
        /// </summary>
        public StringListColumnSerializer Serializer { get; }

        /// <summary>
        /// 字符串列表标签
        /// </summary>
        /// <param name="separator">分隔符，默认逗号</param>
        public StringListAttribute(string separator = ",")
        {
            Serializer = new StringListColumnSerializer(separator);
        }

        /// <summary>
        /// 字符串列表标签
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="separator">分隔符，默认逗号</param>
        public StringListAttribute(string name, string separator) : base(name)
        {
            Serializer = new StringListColumnSerializer(separator);
        }
    }
}
