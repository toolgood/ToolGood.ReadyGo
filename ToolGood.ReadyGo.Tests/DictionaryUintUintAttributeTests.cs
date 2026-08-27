using System;
using System.Collections.Generic;
using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_DictUintTest")]
    [PrimaryKey("Id")]
    public class Tb_DictUintTest
    {
        public int Id { get; set; }

        [DictionaryUintUint2Bytes]
        public Dictionary<uint, uint> PriceVolume { get; set; }
    }

    /// <summary>
    /// [DictionaryUintUint] 属性：Dictionary&lt;uint, uint&gt; 以 byte[]（BLOB 列）保存
    /// </summary>
    public class DictionaryUintUintAttributeTests
    {
        [Fact]
        public void Insert_字典以byte保存_读回一致()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DictUintTest));

            // 无序输入，序列化时按键升序
            var dict = new Dictionary<uint, uint> { { 300, 1 }, { 100, 2 }, { 250, 5 } };
            var item = new Tb_DictUintTest { PriceVolume = dict };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_DictUintTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.NotNull(loaded.PriceVolume);
            Assert.Equal(dict, loaded.PriceVolume);
        }

        [Fact]
        public void Update_更新字典()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DictUintTest));

            var item = new Tb_DictUintTest { PriceVolume = new Dictionary<uint, uint> { { 1, 1 }, { 2, 2 } } };
            helper.Insert(item);

            item.PriceVolume = new Dictionary<uint, uint> { { 999, 9 }, { 123, 3 }, { 456, 6 } };
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_DictUintTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.PriceVolume, loaded.PriceVolume);
        }

        [Fact]
        public void Insert_null()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DictUintTest));

            var item = new Tb_DictUintTest { PriceVolume = null };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_DictUintTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.PriceVolume);
        }

        [Fact]
        public void Insert_空字典()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DictUintTest));

            var item = new Tb_DictUintTest { PriceVolume = new Dictionary<uint, uint>() };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_DictUintTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.NotNull(loaded.PriceVolume);
            Assert.Empty(loaded.PriceVolume);
        }

        [Fact]
        public void 存储格式_键升序差值VLQ编码()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DictUintTest));

            var item = new Tb_DictUintTest { PriceVolume = new Dictionary<uint, uint> { { 300, 1 }, { 100, 2 } } };
            helper.Insert(item);

            var raw = helper.ExecuteScalar<byte[]>("SELECT PriceVolume FROM Tb_DictUintTest WHERE Id = @0", item.Id);
            Assert.NotNull(raw);

            // 格式：元素个数(1字节) + 首键完整 + 数量 + (差值 + 数量)...
            // {100:2, 300:1} → count=2, 键100[0x64], 量2[0x02], 差200[0x81,0x48], 量1[0x01]
            var expected = new byte[] { 0x02, 0x64, 0x02, 0x81, 0x48, 0x01 };
            Assert.Equal(expected, raw);
        }

        [Fact]
        public void 存储格式_大键多字节VLQ()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DictUintTest));

            var item = new Tb_DictUintTest { PriceVolume = new Dictionary<uint, uint> { { 1000000, 9 } } };
            helper.Insert(item);

            var raw = helper.ExecuteScalar<byte[]>("SELECT PriceVolume FROM Tb_DictUintTest WHERE Id = @0", item.Id);
            Assert.NotNull(raw);

            // 1000000 → VLQ 三字节 [0xBD, 0x84, 0x40]
            var expected = new byte[] { 0x01, 0xBD, 0x84, 0x40, 0x09 };
            Assert.Equal(expected, raw);
        }

        [Fact]
        public void 键0_正确读取往返()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DictUintTest));

            // 首键 0 也能正确往返（此前存在键 0 被丢弃的 bug）
            var dict = new Dictionary<uint, uint> { { 0, 5 }, { 10, 3 } };
            var item = new Tb_DictUintTest { PriceVolume = dict };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_DictUintTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(dict, loaded.PriceVolume);
        }
    }
}
