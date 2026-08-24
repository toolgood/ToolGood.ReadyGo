using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		/// <returns>还原的字典；若值为 null 或非 byte[] 则返回 null</returns>
		public object Deserialize(object value, Type targetType)
		{
			if(value == null) {
				return null;
			}
			if(value is byte[] bytes) {
				using(var ms = new MemoryStream(bytes)) {
					BinaryReader br = new BinaryReader(ms);
					var len = Bytes2Uint(br);
					Dictionary<uint, uint> result = new Dictionary<uint, uint>();
					if(len > 0) {
						var price = Bytes2Uint(br);
						var vol = Bytes2Uint(br);
						var lastPrice = price;
						if(lastPrice > 0) {
							result.Add(lastPrice, vol);
						}
						for(int i = 1; i < len; i++) {
							price = Bytes2Uint(br);
							vol = Bytes2Uint(br);
							lastPrice = lastPrice + price;
							result.Add(lastPrice, vol);
						}
					}
					return result;
				}
			}
			return null;
		}

		/// <summary>
		/// 序列化：将 Dictionary&lt;uint, uint&gt; 压缩编码为 byte[]
		/// </summary>
		/// <param name="value">待序列化的字典</param>
		/// <returns>压缩后的 byte[]；若值为 null 或非字典则返回 null</returns>
		public object Serialize(object value)
		{
			if(value == null) {
				return null;
			}
			if(value is Dictionary<uint, uint> dict) {
				var prices = dict.Keys.OrderBy(q => q).ToList();
				using(var ms = new MemoryStream()) {
					BinaryWriter bw = new BinaryWriter(ms);
					bw.Write(Uint2Bytes((uint)dict.Count));
					if(prices.Count > 0) {
						var lastPrice = (uint)prices[0];
						var bs = Uint2Bytes(lastPrice);
						bw.Write(bs);
						bs = Uint2Bytes((uint)dict[prices[0]]);
						bw.Write(bs);
						for(int i = 1; i < prices.Count; i++) {
							bs = Uint2Bytes((uint)prices[i] - lastPrice);
							bw.Write(bs);
							bs = Uint2Bytes((uint)dict[prices[i]]);
							bw.Write(bs);
							lastPrice = (uint)prices[i];
						}
					}
					return ms.ToArray();
				}
			}
			return null;
		}
		private byte[] Uint2Bytes(uint s)
		{
			List<byte> bytes = new List<byte>();
			var k = s;
			var temp = (byte)(k & 0x7F);
			bytes.Add(temp);
			k = k >> 7;
			while(k > 0) {
				temp = (byte)(k & 0x7F | 0x80);
				bytes.Add(temp);
				k = k >> 7;
			}
			bytes.Reverse();
			return bytes.ToArray();
		}


		private uint Bytes2Uint(BinaryReader br)
		{
			uint result = 0;
			var b = br.ReadByte();
			var s = b & 0x80;
			b = (byte)(b & 0x7F);
			result = result | b;
			while(s == 128) {
				b = br.ReadByte();
				s = b & 0x80;
				b = (byte)(b & 0x7F);
				result = result << 7;
				result = result | b;
			}
			return result;
		}

	}
}
