using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// uint→uint 字典列序列化器：将 Dictionary&lt;uint, uint&gt; 以 byte[]（BLOB 列）保存。
    /// 价格键升序排列，差值 + 变长整数（VLQ）压缩编码，减少存储空间。
    /// </summary>
    public class DictionaryUintUint2BytesColumnSerializer : NPoco.IColumnSerializer
    {
        /// <summary>
        /// 反序列化：从 byte[] 还原 Dictionary&lt;uint, uint&gt;
        /// </summary>
        /// <param name="value">数据库中读取的 byte[]</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>还原的字典；若值为 null 则返回 null</returns>
        /// <exception cref="ArgumentException">值为非 byte[] 类型时抛出</exception>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null) {
                return null;
            }
            if (value is byte[] bytes) {
                using (var ms = new MemoryStream(bytes)) {
                    var br = new BinaryReader(ms);
                    var len = Bytes2Uint(br);
                    if (len > bytes.Length) {
                        throw new ArgumentException($"数据损坏：声明元素个数 {len} 超过数据长度 {bytes.Length}。", nameof(value));
                    }
                    var result = new Dictionary<uint, uint>();
                    if (len > 0) {
                        var price = Bytes2Uint(br);
                        var volume = Bytes2Uint(br);
                        var lastPrice = price;
                        result.Add(lastPrice, volume);
                        for (var i = 1; i < len; i++) {
                            price = Bytes2Uint(br);
                            volume = Bytes2Uint(br);
                            lastPrice = lastPrice + price;
                            result.Add(lastPrice, volume);
                        }
                    }
                    return result;
                }
            }
            throw new ArgumentException($"Deserialize 仅支持 byte[]，实际类型为 {value.GetType().FullName}。", nameof(value));
        }

        /// <summary>
        /// 序列化：将 Dictionary&lt;uint, uint&gt; 压缩编码为 byte[]
        /// </summary>
        /// <param name="value">待序列化的字典</param>
        /// <returns>压缩后的 byte[]；若值为 null 则返回 null</returns>
        /// <exception cref="ArgumentException">值为非 Dictionary&lt;uint, uint&gt; 类型时抛出</exception>
        public object Serialize(object value)
        {
            if (value == null) {
                return null;
            }
            if (value is Dictionary<uint, uint> dict) {
                var prices = dict.Keys.OrderBy(q => q).ToList();
                using (var ms = new MemoryStream()) {
                    var bw = new BinaryWriter(ms);
                    bw.Write(Uint2Bytes((uint)dict.Count));
                    if (prices.Count > 0) {
                        var lastPrice = (uint)prices[0];
                        var bytes = Uint2Bytes(lastPrice);
                        bw.Write(bytes);
                        bytes = Uint2Bytes(dict[prices[0]]);
                        bw.Write(bytes);
                        for (var i = 1; i < prices.Count; i++) {
                            bytes = Uint2Bytes((uint)prices[i] - lastPrice);
                            bw.Write(bytes);
                            bytes = Uint2Bytes(dict[prices[i]]);
                            bw.Write(bytes);
                            lastPrice = (uint)prices[i];
                        }
                    }
                    return ms.ToArray();
                }
            }
            throw new ArgumentException($"Serialize 仅支持 Dictionary<uint, uint>，实际类型为 {value.GetType().FullName}。", nameof(value));
        }

        private static byte[] Uint2Bytes(uint value)
        {
            // uint 最多 5 字节（35 位），倒序填充为变长大端序，避免 List+Reverse 的多次分配
            var buffer = new byte[5];
            var index = 5;
            var remaining = value;
            buffer[--index] = (byte)(remaining & 0x7F);
            remaining >>= 7;
            while (remaining > 0) {
                buffer[--index] = (byte)(remaining & 0x7F | 0x80);
                remaining >>= 7;
            }
            var result = new byte[5 - index];
            Array.Copy(buffer, index, result, 0, result.Length);
            return result;
        }

        private static uint Bytes2Uint(BinaryReader br)
        {
            var result = 0u;
            var count = 0;
            byte b;
            try {
                b = br.ReadByte();
            } catch (EndOfStreamException) {
                throw new ArgumentException("数据损坏：uint 变长编码缺少数据。", nameof(br));
            }
            var hasMore = b & 0x80;
            b = (byte)(b & 0x7F);
            result = result | b;
            while (hasMore == 128) {
                if (++count > 4) {
                    throw new ArgumentException("数据损坏：uint 变长编码超过 5 字节。", nameof(br));
                }
                try {
                    b = br.ReadByte();
                } catch (EndOfStreamException) {
                    throw new ArgumentException("数据损坏：uint 变长编码不完整。", nameof(br));
                }
                hasMore = b & 0x80;
                b = (byte)(b & 0x7F);
                result = result << 7;
                result = result | b;
            }
            return result;
        }
    }
}
