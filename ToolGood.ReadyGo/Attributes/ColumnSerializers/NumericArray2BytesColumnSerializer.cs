using System;
using System.Collections.Generic;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 数值数组列序列化器：将 float[] / double[] / int[] / decimal[] 及其 List&lt;T&gt; 序列化为 byte[] 保存，反序列化还原。
    /// 存储格式：前 4 字节保存元素个数，其后为元素数据；字节数按元素类型固定：float 4 字节、double 8 字节、int 4 字节、decimal 16 字节。
    /// </summary>
    public class NumericArray2BytesColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// decimal 占用的字节数（decimal.GetBits 返回 4 个 int，共 16 字节）
        /// </summary>
        private const int DecimalSize = 16;

        /// <summary>
        /// 序列化为 byte[]
        /// </summary>
        /// <param name="value">要序列化的数值数组</param>
        /// <returns>序列化后的 byte[]</returns>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case float[] a:
                    return ToBytes(a, sizeof(float), BitConverter.GetBytes);
                case double[] a:
                    return ToBytes(a, sizeof(double), BitConverter.GetBytes);
                case int[] a:
                    return ToBytes(a, sizeof(int), BitConverter.GetBytes);
                case decimal[] a:
                    return ToBytes(a, DecimalSize, DecimalToBytes);
                case List<float> l:
                    return ToBytes(l.ToArray(), sizeof(float), BitConverter.GetBytes);
                case List<double> l:
                    return ToBytes(l.ToArray(), sizeof(double), BitConverter.GetBytes);
                case List<int> l:
                    return ToBytes(l.ToArray(), sizeof(int), BitConverter.GetBytes);
                case List<decimal> l:
                    return ToBytes(l.ToArray(), DecimalSize, DecimalToBytes);
                default:
                    throw new NotSupportedException($"NumericArray2BytesColumnSerializer 不支持类型 {value.GetType().Name}，仅支持 float[] / double[] / int[] / decimal[] 及其 List<T>。");
            }
        }

        /// <summary>
        /// 从 byte[] 反序列化
        /// </summary>
        /// <param name="value">byte[] 数据</param>
        /// <param name="targetType">目标数组类型</param>
        /// <returns>反序列化后的数值数组</returns>
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
                throw new NotSupportedException($"NumericArray2BytesColumnSerializer 无法从 {value.GetType().Name} 反序列化，仅支持 byte[]。");
            }

            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (t == typeof(List<float>)) {
                return new List<float>(FromBytes(bytes, sizeof(float), BitConverter.ToSingle));
            }
            if (t == typeof(float[])) {
                return FromBytes(bytes, sizeof(float), BitConverter.ToSingle);
            }
            if (t == typeof(List<double>)) {
                return new List<double>(FromBytes(bytes, sizeof(double), BitConverter.ToDouble));
            }
            if (t == typeof(double[])) {
                return FromBytes(bytes, sizeof(double), BitConverter.ToDouble);
            }
            if (t == typeof(List<int>)) {
                return new List<int>(FromBytes(bytes, sizeof(int), BitConverter.ToInt32));
            }
            if (t == typeof(int[])) {
                return FromBytes(bytes, sizeof(int), BitConverter.ToInt32);
            }
            if (t == typeof(List<decimal>)) {
                return new List<decimal>(FromBytes(bytes, DecimalSize, BytesToDecimal));
            }
            if (t == typeof(decimal[])) {
                return FromBytes(bytes, DecimalSize, BytesToDecimal);
            }
            throw new NotSupportedException($"NumericArray2BytesColumnSerializer 不支持目标类型 {targetType.Name}，仅支持 float[] / double[] / int[] / decimal[] 及其 List<T>。");
        }

        private static byte[] ToBytes<T>(T[] values, int size, Func<T, byte[]> getBytes)
        {
            // 前 4 字节保存元素个数，便于反序列化时校验长度
            var bytes = new byte[4 + values.Length * size];
            BitConverter.GetBytes(values.Length).CopyTo(bytes, 0);
            for (int i = 0; i < values.Length; i++) {
                getBytes(values[i]).CopyTo(bytes, 4 + i * size);
            }
            return bytes;
        }

        private static T[] FromBytes<T>(byte[] bytes, int size, Func<byte[], int, T> getValue)
        {
            if (bytes.Length < 4) {
                throw new ArgumentException("byte[] 长度必须大于等于 4（前 4 字节保存元素个数）。", nameof(bytes));
            }
            var count = BitConverter.ToInt32(bytes, 0);
            var expected = 4 + count * size;
            if (bytes.Length != expected) {
                throw new ArgumentException($"byte[] 长度 {bytes.Length} 与声明的元素个数 {count} 不匹配，应为 {expected}。", nameof(bytes));
            }
            var values = new T[count];
            for (int i = 0; i < count; i++) {
                values[i] = getValue(bytes, 4 + i * size);
            }
            return values;
        }

        /// <summary>
        /// decimal 转 byte[]：使用 decimal.GetBits（4 个 int，共 16 字节），保证无损往返
        /// </summary>
        private static byte[] DecimalToBytes(decimal value)
        {
            var bits = decimal.GetBits(value);
            var bytes = new byte[DecimalSize];
            for (int i = 0; i < bits.Length; i++) {
                BitConverter.GetBytes(bits[i]).CopyTo(bytes, i * 4);
            }
            return bytes;
        }

        /// <summary>
        /// byte[] 还原 decimal：与 DecimalToBytes 对称
        /// </summary>
        private static decimal BytesToDecimal(byte[] bytes, int offset)
        {
            var bits = new int[4];
            for (int i = 0; i < bits.Length; i++) {
                bits[i] = BitConverter.ToInt32(bytes, offset + i * 4);
            }
            return new decimal(bits);
        }
    }
}
