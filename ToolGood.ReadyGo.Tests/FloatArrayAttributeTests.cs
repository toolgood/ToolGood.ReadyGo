using System;
using System.Collections.Generic;
using System.Linq;
using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_FloatArrayTest")]
    [PrimaryKey("Id")]
    public class Tb_FloatArrayTest
    {
        public int Id { get; set; }

        [FloatArray]
        public float[] Floats { get; set; }

        [FloatArray]
        public List<float> ValueList { get; set; }
    }

    /// <summary>
    /// [FloatArray] 属性：float[] / List&lt;float&gt; 以 byte[] 保存
    /// </summary>
    public class FloatArrayAttributeTests
    {
        [Fact]
        public void Insert_float数组以byte保存_读回一致()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_FloatArrayTest));

            var item = new Tb_FloatArrayTest {
                Floats = new[] { 1.5f, -2.25f, 3.125f },
                ValueList = new List<float> { 0.1f, 10f, 100.5f }
            };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_FloatArrayTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Floats, loaded.Floats);
            Assert.Equal(item.ValueList, loaded.ValueList);

            // 数据库中实际以 byte[] 存储（每元素 4 字节）
            var raw = helper.ExecuteScalar<byte[]>("SELECT Floats FROM Tb_FloatArrayTest WHERE Id = @0", item.Id);
            Assert.Equal(3 * 4, raw.Length);
            Assert.Equal(item.Floats, ToFloats(raw));
        }

        [Fact]
        public void Update_float数组()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_FloatArrayTest));

            var item = new Tb_FloatArrayTest {
                Floats = new[] { 1f, 2f },
                ValueList = new List<float> { 3f }
            };
            helper.Insert(item);

            item.Floats = new[] { 9.5f, 8.25f };
            item.ValueList = new List<float> { 7f, 6f, 5f };
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_FloatArrayTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Floats, loaded.Floats);
            Assert.Equal(item.ValueList, loaded.ValueList);
        }

        [Fact]
        public void Insert_null()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_FloatArrayTest));

            var item = new Tb_FloatArrayTest { Floats = null, ValueList = null };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_FloatArrayTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Floats);
            Assert.Null(loaded.ValueList);
        }

        [Fact]
        public void 存储字节数_等于元素数乘4()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_FloatArrayTest));

            var arr = new float[] { 1.5f, -2.25f, 3.125f, 100f, 0.001f };   // 5 个元素
            var list = new List<float> { 0.5f, 1f };                        // 2 个元素
            var item = new Tb_FloatArrayTest { Floats = arr, ValueList = list };
            helper.Insert(item);

            // 数据库中实际存储的 byte[]
            var arrBytes = helper.ExecuteScalar<byte[]>("SELECT Floats FROM Tb_FloatArrayTest WHERE Id = @0", item.Id);
            var listBytes = helper.ExecuteScalar<byte[]>("SELECT ValueList FROM Tb_FloatArrayTest WHERE Id = @0", item.Id);

            // 字节数 = 元素数 × 4（每个 float 4 字节）
            Assert.NotNull(arrBytes);
            Assert.Equal(arr.Length * 4, arrBytes.Length);
            Assert.Equal(list.Count * 4, listBytes.Length);

            // 字节内容按顺序与 BitConverter 一致（可精确还原）
            for (int i = 0; i < arr.Length; i++) {
                Assert.Equal(BitConverter.GetBytes(arr[i]), arrBytes.Skip(i * 4).Take(4));
            }
            for (int i = 0; i < list.Count; i++) {
                Assert.Equal(BitConverter.GetBytes(list[i]), listBytes.Skip(i * 4).Take(4));
            }
        }

        private static float[] ToFloats(byte[] bytes)
        {
            var floats = new float[bytes.Length / 4];
            for (int i = 0; i < floats.Length; i++) {
                floats[i] = BitConverter.ToSingle(bytes, i * 4);
            }
            return floats;
        }
    }
}
