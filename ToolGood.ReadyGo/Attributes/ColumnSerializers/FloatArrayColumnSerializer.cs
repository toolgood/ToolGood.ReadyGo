using System;
using System.Collections.Generic;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// float 数组列序列化器：将 float[] / List&lt;float&gt; 序列化为 byte[]（每元素 4 字节）保存，反序列化还原。
    /// </summary>
    public class FloatArrayColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// 序列化为 byte[]
        /// </summary>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case float[] array:
                    return ToBytes(array);
                case List<float> list:
                    return ToBytes(list.ToArray());
                default:
                    throw new NotSupportedException($"FloatArrayColumnSerializer 不支持类型 {value.GetType().Name}，仅支持 float[] / List<float>。");
            }
        }

        /// <summary>
        /// 从 byte[] 反序列化
        /// </summary>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null || value is DBNull) {
                return null;
            }
            byte[] bytes;
            if (value is byte[] b) {
                bytes = b;
            } else if (value is string s) {
                // 兼容字符串形式（如 base64）
                bytes = Convert.FromBase64String(s);
            } else {
                throw new NotSupportedException($"FloatArrayColumnSerializer 无法从 {value.GetType().Name} 反序列化，仅支持 byte[]。");
            }

            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            var floats = FromBytes(bytes);
            if (t == typeof(List<float>)) {
                return new List<float>(floats);
            }
            if (t == typeof(float[])) {
                return floats;
            }
            throw new NotSupportedException($"FloatArrayColumnSerializer 不支持目标类型 {targetType.Name}，仅支持 float[] / List<float>。");
        }

        private static byte[] ToBytes(float[] values)
        {
            var bytes = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++) {
                BitConverter.GetBytes(values[i]).CopyTo(bytes, i * 4);
            }
            return bytes;
        }

        private static float[] FromBytes(byte[] bytes)
        {
            if (bytes.Length % 4 != 0) {
                throw new ArgumentException("byte[] 长度必须是 4 的倍数。", nameof(bytes));
            }
            var floats = new float[bytes.Length / 4];
            for (int i = 0; i < floats.Length; i++) {
                floats[i] = BitConverter.ToSingle(bytes, i * 4);
            }
            return floats;
        }
    }
}
