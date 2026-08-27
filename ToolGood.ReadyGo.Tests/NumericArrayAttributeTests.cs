using System;
using System.Collections.Generic;
using System.Linq;
using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_NumericArrayTest")]
    [PrimaryKey("Id")]
    public class Tb_NumericArrayTest
    {
        public int Id { get; set; }

        [NumericArray2Bytes]
        public float[] Floats { get; set; }

        [NumericArray2Bytes]
        public List<float> ValueList { get; set; }

        [NumericArray2Bytes]
        public double[] Doubles { get; set; }

        [NumericArray2Bytes]
        public List<int> Ints { get; set; }
    }

    /// <summary>
    /// [NumericArray] 属性：float[] / double[] / int[] 及其 List&lt;T&gt; 以 byte[] 保存
    /// </summary>
    public class NumericArrayAttributeTests
    {
        [Fact]
        public void Insert_float数组以byte保存_读回一致()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArrayTest));

            var item = new Tb_NumericArrayTest {
                Floats = new[] { 1.5f, -2.25f, 3.125f },
                ValueList = new List<float> { 0.1f, 10f, 100.5f }
            };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_NumericArrayTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Floats, loaded.Floats);
            Assert.Equal(item.ValueList, loaded.ValueList);

            // 数据库中实际以 byte[] 存储（前 4 字节元素个数 + 每元素 4 字节）
            var raw = helper.ExecuteScalar<byte[]>("SELECT Floats FROM Tb_NumericArrayTest WHERE Id = @0", item.Id);
            Assert.Equal(4 + 3 * 4, raw.Length);
            Assert.Equal(item.Floats, ToFloats(raw));
        }

        [Fact]
        public void Update_float数组()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArrayTest));

            var item = new Tb_NumericArrayTest {
                Floats = new[] { 1f, 2f },
                ValueList = new List<float> { 3f }
            };
            helper.Insert(item);

            item.Floats = new[] { 9.5f, 8.25f };
            item.ValueList = new List<float> { 7f, 6f, 5f };
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_NumericArrayTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Floats, loaded.Floats);
            Assert.Equal(item.ValueList, loaded.ValueList);
        }

        [Fact]
        public void Insert_null()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArrayTest));

            var item = new Tb_NumericArrayTest { Floats = null, ValueList = null };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_NumericArrayTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Floats);
            Assert.Null(loaded.ValueList);
        }

        [Fact]
        public void 存储字节数_等于元素数乘4()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArrayTest));

            var arr = new float[] { 1.5f, -2.25f, 3.125f, 100f, 0.001f };   // 5 个元素
            var list = new List<float> { 0.5f, 1f };                        // 2 个元素
            var item = new Tb_NumericArrayTest { Floats = arr, ValueList = list };
            helper.Insert(item);

            // 数据库中实际存储的 byte[]
            var arrBytes = helper.ExecuteScalar<byte[]>("SELECT Floats FROM Tb_NumericArrayTest WHERE Id = @0", item.Id);
            var listBytes = helper.ExecuteScalar<byte[]>("SELECT ValueList FROM Tb_NumericArrayTest WHERE Id = @0", item.Id);

            // 字节数 = 4（元素个数） + 元素数 × 4（每个 float 4 字节）
            Assert.NotNull(arrBytes);
            Assert.Equal(4 + arr.Length * 4, arrBytes.Length);
            Assert.Equal(4 + list.Count * 4, listBytes.Length);

            // 字节内容按顺序与 BitConverter 一致（前 4 字节为元素个数，之后可精确还原）
            Assert.Equal(arr.Length, BitConverter.ToInt32(arrBytes, 0));
            for (int i = 0; i < arr.Length; i++) {
                Assert.Equal(BitConverter.GetBytes(arr[i]), arrBytes.Skip(4 + i * 4).Take(4));
            }
            for (int i = 0; i < list.Count; i++) {
                Assert.Equal(BitConverter.GetBytes(list[i]), listBytes.Skip(4 + i * 4).Take(4));
            }
        }

        [Fact]
        public void 存储字节数_double8字节_int4字节()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArrayTest));

            var doubles = new double[] { 1.5, -2.25, 3.141592653589793 };  // 3 个元素
            var ints = new List<int> { -1, 0, 1, 100 };                    // 4 个元素
            var item = new Tb_NumericArrayTest { Doubles = doubles, Ints = ints };
            helper.Insert(item);

            var doubleBytes = helper.ExecuteScalar<byte[]>("SELECT Doubles FROM Tb_NumericArrayTest WHERE Id = @0", item.Id);
            var intBytes = helper.ExecuteScalar<byte[]>("SELECT Ints FROM Tb_NumericArrayTest WHERE Id = @0", item.Id);

            // double 8 字节/元素，int 4 字节/元素（前 4 字节为元素个数）
            Assert.NotNull(doubleBytes);
            Assert.Equal(4 + doubles.Length * 8, doubleBytes.Length);
            Assert.Equal(4 + ints.Count * 4, intBytes.Length);

            // 字节内容可精确还原
            Assert.Equal(doubles.Length, BitConverter.ToInt32(doubleBytes, 0));
            for (int i = 0; i < doubles.Length; i++) {
                Assert.Equal(BitConverter.GetBytes(doubles[i]), doubleBytes.Skip(4 + i * 8).Take(8));
            }
            for (int i = 0; i < ints.Count; i++) {
                Assert.Equal(BitConverter.GetBytes(ints[i]), intBytes.Skip(4 + i * 4).Take(4));
            }

            // 读回对象一致
            var loaded = helper.FirstOrDefault<Tb_NumericArrayTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(doubles, loaded.Doubles);
            Assert.Equal(ints, loaded.Ints);
        }

        private static float[] ToFloats(byte[] bytes)
        {
            // 跳过前 4 字节（元素个数）
            var floats = new float[(bytes.Length - 4) / 4];
            for (int i = 0; i < floats.Length; i++) {
                floats[i] = BitConverter.ToSingle(bytes, 4 + i * 4);
            }
            return floats;
        }
    }

    /// <summary>
    /// 不加 [NumericArray2Bytes] 特性时，float[] / double[] / int[] / decimal[] 及 List&lt;T&gt; 默认按 byte[]（BLOB）保存
    /// </summary>
    [Table("Tb_NumericArrayDefaultTest")]
    [PrimaryKey("Id")]
    public class Tb_NumericArrayDefaultTest
    {
        public int Id { get; set; }

        public float[] Floats { get; set; }

        public double[] Doubles { get; set; }

        public int[] Ints { get; set; }

        public decimal[] Decimals { get; set; }

        public List<float> FloatList { get; set; }

        public List<int> IntList { get; set; }

        public List<decimal> DecimalList { get; set; }
    }

    public class NumericArrayDefaultTests
    {
        [Fact]
        public void 默认_数值数组以byte保存_读回一致()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArrayDefaultTest));

            var item = new Tb_NumericArrayDefaultTest {
                Floats = new[] { 1.5f, -2.25f, 3.125f },
                Doubles = new[] { 1.5, -2.25, 3.141592653589793 },
                Ints = new[] { -1, 0, 100 },
                Decimals = new[] { 1.5m, -2.25m, 3.141592653589793238462643383m },
                FloatList = new List<float> { 0.1f, 10f, 100.5f },
                IntList = new List<int> { 1, 2, 3, 4 },
                DecimalList = new List<decimal> { 0.1m, 10m, 100.5m }
            };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_NumericArrayDefaultTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Floats, loaded.Floats);
            Assert.Equal(item.Doubles, loaded.Doubles);
            Assert.Equal(item.Ints, loaded.Ints);
            Assert.Equal(item.Decimals, loaded.Decimals);
            Assert.Equal(item.FloatList, loaded.FloatList);
            Assert.Equal(item.IntList, loaded.IntList);
            Assert.Equal(item.DecimalList, loaded.DecimalList);

            // 数据库中以 byte[] 存储：前 4 字节元素个数 + 每元素固定字节数
            var floatBytes = helper.ExecuteScalar<byte[]>("SELECT Floats FROM Tb_NumericArrayDefaultTest WHERE Id = @0", item.Id);
            var intBytes = helper.ExecuteScalar<byte[]>("SELECT IntList FROM Tb_NumericArrayDefaultTest WHERE Id = @0", item.Id);
            var decimalBytes = helper.ExecuteScalar<byte[]>("SELECT Decimals FROM Tb_NumericArrayDefaultTest WHERE Id = @0", item.Id);
            Assert.Equal(4 + item.Floats.Length * 4, floatBytes.Length);
            Assert.Equal(4 + item.IntList.Count * 4, intBytes.Length);
            // decimal 每元素 16 字节（decimal.GetBits 4 个 int）
            Assert.Equal(4 + item.Decimals.Length * 16, decimalBytes.Length);
            // decimal 字节内容可精确还原
            Assert.Equal(item.Decimals.Length, BitConverter.ToInt32(decimalBytes, 0));
            for (int i = 0; i < item.Decimals.Length; i++) {
                var bits = decimal.GetBits(item.Decimals[i]);
                for (int j = 0; j < 4; j++) {
                    Assert.Equal(BitConverter.GetBytes(bits[j]), decimalBytes.Skip(4 + i * 16 + j * 4).Take(4));
                }
            }
        }

        [Fact]
        public void 默认_null_可读回()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArrayDefaultTest));

            var item = new Tb_NumericArrayDefaultTest { Floats = null, IntList = null };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_NumericArrayDefaultTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Floats);
            Assert.Null(loaded.IntList);
        }
    }
}
