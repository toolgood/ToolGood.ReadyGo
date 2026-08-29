using System;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_String2BytesTest")]
    [PrimaryKey("Id")]
    public class Tb_String2BytesTest
    {
        public int Id { get; set; }

        [String2Bytes]
        public string Content { get; set; }

        [Base64String2Bytes]
        public byte[] Data { get; set; }
    }

    /// <summary>
    /// [String2Bytes] string ↔ byte[]（UTF-8 BLOB 列）与 [Base64String2Bytes] byte[] ↔ Base64 文本列
    /// </summary>
    public class String2BytesAttributeTests
    {
        [Fact]
        public void 存储与还原_中文文本与二进制往返一致()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_String2BytesTest));

            var content = "中文文本 Test \uD83D\uDE00 结束";   // 含 emoji，验证 UTF-8 多字节
            var data = new byte[] { 0x00, 0x01, 0x7F, 0x80, 0xFF, 0x10, 0x20 };
            var item = new Tb_String2BytesTest { Content = content, Data = data };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_String2BytesTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(content, loaded.Content);
            Assert.Equal(data, loaded.Data);
        }

        [Fact]
        public void 数据库存储格式_Content为UTF8字节_Data为Base64文本()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_String2BytesTest));

            var content = "你好，ReadyGo";
            var data = new byte[] { 1, 2, 3, 4, 5 };
            var item = new Tb_String2BytesTest { Content = content, Data = data };
            helper.Insert(item);

            // Content 列实际存 UTF-8 字节（BLOB）
            var contentBytes = helper.ExecuteScalar<byte[]>("SELECT Content FROM Tb_String2BytesTest WHERE Id = @0", item.Id);
            Assert.NotNull(contentBytes);
            Assert.Equal(Encoding.UTF8.GetBytes(content), contentBytes);

            // Data 列实际存 Base64 文本
            var dataText = helper.ExecuteScalar<string>("SELECT Data FROM Tb_String2BytesTest WHERE Id = @0", item.Id);
            Assert.Equal(Convert.ToBase64String(data), dataText);
        }

        [Fact]
        public void 更新_文本与二进制()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_String2BytesTest));

            var item = new Tb_String2BytesTest { Content = "old", Data = new byte[] { 1 } };
            helper.Insert(item);

            item.Content = "new 内容";
            item.Data = new byte[] { 9, 8, 7, 6 };
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_String2BytesTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Content, loaded.Content);
            Assert.Equal(item.Data, loaded.Data);
        }

        [Fact]
        public void 可空_Null值序列化与反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_String2BytesTest));

            var item = new Tb_String2BytesTest { Content = null, Data = null };
            helper.Insert(item);

            // 数据库中实际存 NULL
            var raw = helper.ExecuteScalar<object>("SELECT Content FROM Tb_String2BytesTest WHERE Id = @0", item.Id);
            Assert.True(raw == null || raw == DBNull.Value);

            var loaded = helper.FirstOrDefault<Tb_String2BytesTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Content);
            Assert.Null(loaded.Data);
        }

        [Fact]
        public void 可空_数据库NULL值读取()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_String2BytesTest));

            // 直接 SQL 插入 NULL，模拟历史数据
            helper.Execute("INSERT INTO Tb_String2BytesTest (Id, Content, Data) VALUES (@0, NULL, NULL)", 1);

            var loaded = helper.FirstOrDefault<Tb_String2BytesTest>(1);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Content);
            Assert.Null(loaded.Data);
        }
    }
}
