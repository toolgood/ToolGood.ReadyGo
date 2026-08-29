using System;
using System.Text;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 字符串转字节列序列化器：string 以 UTF-8 编码的 byte[]（BLOB 列）保存，读取时还原。
    /// 支持 null（null → NULL，NULL → null）。
    /// </summary>
    public class String2BytesColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// 序列化：将 string 按 UTF-8 编码为 byte[]
        /// </summary>
        /// <param name="value">string 值</param>
        /// <returns>UTF-8 编码的 byte[]，null 输入返回 null</returns>
        /// <exception cref="NotSupportedException">值为非 string 类型时抛出</exception>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case string s:
                    return Encoding.UTF8.GetBytes(s);
                default:
                    throw new NotSupportedException($"String2BytesColumnSerializer 不支持类型 {value.GetType().Name}，仅支持 string。");
            }
        }

        /// <summary>
        /// 反序列化：从 byte[] 还原 string
        /// </summary>
        /// <param name="value">数据库中的 byte[]</param>
        /// <param name="targetType">目标类型（应为 string）</param>
        /// <returns>还原的 string；值为 null/DBNull 时返回 null</returns>
        /// <exception cref="NotSupportedException">目标类型非 string 或值为非 byte[] 类型时抛出</exception>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null || value is DBNull) {
                return null;
            }
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (t != typeof(string)) {
                throw new NotSupportedException($"String2BytesColumnSerializer 不支持目标类型 {targetType.Name}，仅支持 string。");
            }
            if (value is byte[] bytes) {
                return Encoding.UTF8.GetString(bytes);
            }
            throw new NotSupportedException($"String2BytesColumnSerializer 无法从 {value.GetType().Name} 反序列化，仅支持 byte[]。");
        }
    }
}
