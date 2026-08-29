using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 数值数组列序列化器：将 float[] / double[] / int[] / decimal[] 及其 List&lt;T&gt; 序列化为 byte[] 保存，反序列化还原。
    /// 存储格式：前 4 字节（小端）保存元素个数，其后为元素数据（小端）；字节数按元素类型固定：float 4 字节、double 8 字节、int 4 字节、decimal 16 字节。
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
                    return ToBytes(a, sizeof(float), (span, v) => BinaryPrimitives.WriteSingleLittleEndian(span, v));
                case double[] a:
                    return ToBytes(a, sizeof(double), (span, v) => BinaryPrimitives.WriteDoubleLittleEndian(span, v));
                case int[] a:
                    return ToBytes(a, sizeof(int), (span, v) => BinaryPrimitives.WriteInt32LittleEndian(span, v));
                case decimal[] a:
                    return ToBytes(a, DecimalSize, WriteDecimal);
                case List<float> l:
                    return ToBytes(l, sizeof(float), (span, v) => BinaryPrimitives.WriteSingleLittleEndian(span, v));
                case List<double> l:
                    return ToBytes(l, sizeof(double), (span, v) => BinaryPrimitives.WriteDoubleLittleEndian(span, v));
                case List<int> l:
                    return ToBytes(l, sizeof(int), (span, v) => BinaryPrimitives.WriteInt32LittleEndian(span, v));
                case List<decimal> l:
                    return ToBytes(l, DecimalSize, WriteDecimal);
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
            if (!(value is byte[] bytes)) {
                throw new NotSupportedException($"NumericArray2BytesColumnSerializer 无法从 {value.GetType().Name} 反序列化，仅支持 byte[]。");
            }

            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (t == typeof(List<float>)) {
                return new List<float>(FromBytes(bytes, sizeof(float), BinaryPrimitives.ReadSingleLittleEndian));
            }
            if (t == typeof(float[])) {
                return FromBytes(bytes, sizeof(float), BinaryPrimitives.ReadSingleLittleEndian);
            }
            if (t == typeof(List<double>)) {
                return new List<double>(FromBytes(bytes, sizeof(double), BinaryPrimitives.ReadDoubleLittleEndian));
            }
            if (t == typeof(double[])) {
                return FromBytes(bytes, sizeof(double), BinaryPrimitives.ReadDoubleLittleEndian);
            }
            if (t == typeof(List<int>)) {
                return new List<int>(FromBytes(bytes, sizeof(int), BinaryPrimitives.ReadInt32LittleEndian));
            }
            if (t == typeof(int[])) {
                return FromBytes(bytes, sizeof(int), BinaryPrimitives.ReadInt32LittleEndian);
            }
            if (t == typeof(List<decimal>)) {
                return new List<decimal>(FromBytes(bytes, DecimalSize, ReadDecimal));
            }
            if (t == typeof(decimal[])) {
                return FromBytes(bytes, DecimalSize, ReadDecimal);
            }
            throw new NotSupportedException($"NumericArray2BytesColumnSerializer 不支持目标类型 {targetType.Name}，仅支持 float[] / double[] / int[] / decimal[] 及其 List<T>。");
        }

        private delegate void SpanWriter<T>(Span<byte> span, T value);
        private delegate T SpanReader<T>(ReadOnlySpan<byte> span);

        private static byte[] ToBytes<T>(IReadOnlyList<T> values, int size, SpanWriter<T> write)
        {
            // 前 4 字节（小端）保存元素个数，便于反序列化时校验长度
            var bytes = new byte[4 + values.Count * size];
            BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(0, 4), values.Count);
            for (int i = 0; i < values.Count; i++) {
                write(bytes.AsSpan(4 + i * size, size), values[i]);
            }
            return bytes;
        }

        private static T[] FromBytes<T>(byte[] bytes, int size, SpanReader<T> read)
        {
            if (bytes.Length < 4) {
                throw new ArgumentException("byte[] 长度必须大于等于 4（前 4 字节保存元素个数）。", nameof(bytes));
            }
            var count = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(0, 4));
            var expected = 4 + count * size;
            if (bytes.Length != expected) {
                throw new ArgumentException($"byte[] 长度 {bytes.Length} 与声明的元素个数 {count} 不匹配，应为 {expected}。", nameof(bytes));
            }
            var values = new T[count];
            for (int i = 0; i < count; i++) {
                values[i] = read(bytes.AsSpan(4 + i * size, size));
            }
            return values;
        }

        /// <summary>
        /// decimal 写入：使用 decimal.GetBits（4 个 int，共 16 字节，小端），保证无损往返
        /// </summary>
        private static void WriteDecimal(Span<byte> span, decimal value)
        {
            var bits = decimal.GetBits(value);
            for (int i = 0; i < bits.Length; i++) {
                BinaryPrimitives.WriteInt32LittleEndian(span.Slice(i * 4, 4), bits[i]);
            }
        }

        /// <summary>
        /// decimal 还原：与 WriteDecimal 对称
        /// </summary>
        private static decimal ReadDecimal(ReadOnlySpan<byte> span)
        {
            var bits = new int[4];
            for (int i = 0; i < bits.Length; i++) {
                bits[i] = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(i * 4, 4));
            }
            return new decimal(bits);
        }
    }
}
