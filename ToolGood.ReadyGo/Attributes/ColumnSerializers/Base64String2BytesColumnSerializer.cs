using System;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// Base64 字符串转字节列序列化器：byte[] 以 Base64 字符串（VARCHAR/TEXT 列）保存，读取时还原。
    /// 支持 null（null → NULL，NULL → null）。
    /// </summary>
    public class Base64String2BytesColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// 序列化：将 byte[] 编码为 Base64 字符串
        /// </summary>
        /// <param name="value">byte[] 值</param>
        /// <returns>Base64 字符串，null 输入返回 null</returns>
        /// <exception cref="NotSupportedException">值为非 byte[] 类型时抛出</exception>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case byte[] bytes:
                    return Convert.ToBase64String(bytes);
                default:
                    throw new NotSupportedException($"Base64String2BytesColumnSerializer 不支持类型 {value.GetType().Name}，仅支持 byte[]。");
            }
        }

        /// <summary>
        /// 反序列化：从 Base64 字符串还原 byte[]
        /// </summary>
        /// <param name="value">数据库中的 Base64 字符串</param>
        /// <param name="targetType">目标类型（应为 byte[]）</param>
        /// <returns>还原的 byte[]；值为 null/DBNull 时返回 null</returns>
        /// <exception cref="NotSupportedException">目标类型非 byte[] 或值为非 string 类型时抛出</exception>
        /// <exception cref="FormatException">字符串不是合法的 Base64 时抛出</exception>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null || value is DBNull) {
                return null;
            }
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (t != typeof(byte[])) {
                throw new NotSupportedException($"Base64String2BytesColumnSerializer 不支持目标类型 {targetType.Name}，仅支持 byte[]。");
            }
            if (value is string s) {
                return Convert.FromBase64String(s);
            }
            throw new NotSupportedException($"Base64String2BytesColumnSerializer 无法从 {value.GetType().Name} 反序列化，仅支持 string。");
        }
    }
}
