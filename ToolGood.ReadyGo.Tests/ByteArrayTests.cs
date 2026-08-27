using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_BlobTest")]
    [PrimaryKey("Id")]
    public class Tb_BlobTest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] Data { get; set; }
    }

    /// <summary>
    /// byte[] 保存与读取（SQLite BLOB）
    /// </summary>
    public class ByteArrayTests
    {
        [Fact]
        public void Insert_读回_byte数组相等()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_BlobTest));

            var data = new byte[] { 0x00, 0x01, 0x02, 0xFF, 0x10, 0x7F, 0x80 };
            var item = new Tb_BlobTest { Name = "blob1", Data = data };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_BlobTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal("blob1", loaded.Name);
            Assert.Equal(data, loaded.Data);
        }

        [Fact]
        public void Insert_大数据_读回相等()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_BlobTest));

            var data = new byte[1024 * 100]; // 100KB
            new Random(42).NextBytes(data);
            var item = new Tb_BlobTest { Name = "big", Data = data };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_BlobTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(data, loaded.Data);
        }

        [Fact]
        public void Insert_空数组_和_null()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_BlobTest));

            var empty = new Tb_BlobTest { Name = "empty", Data = Array.Empty<byte>() };
            helper.Insert(empty);
            var loadedEmpty = helper.FirstOrDefault<Tb_BlobTest>(empty.Id);
            Assert.NotNull(loadedEmpty);
            Assert.Empty(loadedEmpty.Data);

            var nullItem = new Tb_BlobTest { Name = "null", Data = null };
            helper.Insert(nullItem);
            var loadedNull = helper.FirstOrDefault<Tb_BlobTest>(nullItem.Id);
            Assert.NotNull(loadedNull);
            Assert.Null(loadedNull.Data);
        }

        [Fact]
        public void Update_byte数组()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_BlobTest));

            var item = new Tb_BlobTest { Name = "old", Data = new byte[] { 1, 2, 3 } };
            helper.Insert(item);

            item.Data = new byte[] { 9, 8, 7, 6 };
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_BlobTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(new byte[] { 9, 8, 7, 6 }, loaded.Data);
        }

        [Fact]
        public async Task Insert_异步_读回_byte数组()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_BlobTest));

            var data = new byte[] { 0x11, 0x22, 0x33, 0x44 };
            var item = new Tb_BlobTest { Name = "async-blob", Data = data };
            await helper.Insert_Async(item);

            var loaded = await helper.FirstOrDefault_Async<Tb_BlobTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(data, loaded.Data);
        }
    }
}
