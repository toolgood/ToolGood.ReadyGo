using System.Collections.Generic;
using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_StringListTest")]
    [PrimaryKey("Id")]
    public class Tb_StringListTest
    {
        public int Id { get; set; }

        [StringList]
        public List<string> Tags { get; set; }

        [StringList("|")]
        public List<string> Flags { get; set; }

        [StringList]
        public string[] Codes { get; set; }
    }

    /// <summary>
    /// [StringList] 属性：List&lt;string&gt; / string[] 以分隔符文本保存
    /// </summary>
    public class StringListAttributeTests
    {
        [Fact]
        public void 可空列表_Null值序列化与反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_StringListTest));

            var item = new Tb_StringListTest { Tags = null, Flags = null, Codes = null };
            helper.Insert(item);

            // 数据库中实际存 NULL
            var raw = helper.ExecuteScalar<object>("SELECT Tags FROM Tb_StringListTest WHERE Id = @0", item.Id);
            Assert.True(raw == null || raw == DBNull.Value);

            var loaded = helper.FirstOrDefault<Tb_StringListTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Tags);
            Assert.Null(loaded.Flags);
            Assert.Null(loaded.Codes);
        }

        [Fact]
        public void 可空列表_数据库NULL值读取()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_StringListTest));

            // 直接 SQL 插入 NULL，模拟历史数据
            helper.Execute("INSERT INTO Tb_StringListTest (Id, Tags, Flags, Codes) VALUES (@0, NULL, NULL, NULL)", 1);

            var loaded = helper.FirstOrDefault<Tb_StringListTest>(1);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Tags);
            Assert.Null(loaded.Flags);
            Assert.Null(loaded.Codes);
        }

        [Fact]
        public void 列表_存储与还原()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_StringListTest));

            var item = new Tb_StringListTest {
                Tags = new List<string> { "a", "b", "c" },
                Flags = new List<string> { "x", "y" },
                Codes = new[] { "k1", "k2" }
            };
            helper.Insert(item);

            // 存储格式：默认逗号分隔 / 自定义 | 分隔
            Assert.Equal("a,b,c", helper.ExecuteScalar<string>("SELECT Tags FROM Tb_StringListTest WHERE Id = @0", item.Id));
            Assert.Equal("x|y", helper.ExecuteScalar<string>("SELECT Flags FROM Tb_StringListTest WHERE Id = @0", item.Id));
            Assert.Equal("k1,k2", helper.ExecuteScalar<string>("SELECT Codes FROM Tb_StringListTest WHERE Id = @0", item.Id));

            var loaded = helper.FirstOrDefault<Tb_StringListTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Tags, loaded.Tags);
            Assert.Equal(item.Flags, loaded.Flags);
            Assert.Equal(item.Codes, loaded.Codes);
        }

        [Fact]
        public void 列表_含分隔符与反斜杠元素转义()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_StringListTest));

            var item = new Tb_StringListTest {
                Tags = new List<string> { "a,b", @"c\d", "x" },
                Flags = new List<string> { "p|q" }
            };
            helper.Insert(item);

            // 存储格式：分隔符与反斜杠前加 \ 转义
            Assert.Equal(@"a\,b,c\\d,x", helper.ExecuteScalar<string>("SELECT Tags FROM Tb_StringListTest WHERE Id = @0", item.Id));
            Assert.Equal(@"p\|q", helper.ExecuteScalar<string>("SELECT Flags FROM Tb_StringListTest WHERE Id = @0", item.Id));

            var loaded = helper.FirstOrDefault<Tb_StringListTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Tags, loaded.Tags);
            Assert.Equal(item.Flags, loaded.Flags);
        }

        [Fact]
        public void 列表_空列表存空串()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_StringListTest));

            var item = new Tb_StringListTest { Tags = new List<string>() };
            helper.Insert(item);

            Assert.Equal("", helper.ExecuteScalar<string>("SELECT Tags FROM Tb_StringListTest WHERE Id = @0", item.Id));

            var loaded = helper.FirstOrDefault<Tb_StringListTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.NotNull(loaded.Tags);
            Assert.Empty(loaded.Tags);
        }
    }
}
