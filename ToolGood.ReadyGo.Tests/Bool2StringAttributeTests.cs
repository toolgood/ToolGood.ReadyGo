using System;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_Bool2StringTest")]
    [PrimaryKey("Id")]
    public class Tb_Bool2StringTest
    {
        public int Id { get; set; }

        [Bool2String]
        public bool Active { get; set; }

        [Bool2String]
        public bool? Enable { get; set; }
    }

    /// <summary>
    /// [Bool2String] 属性：bool 以 "true"/"false" 文本保存
    /// </summary>
    public class Bool2StringAttributeTests
    {
        [Fact]
        public void 布尔_以true和false文本存储()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Bool2StringTest));

            var item = new Tb_Bool2StringTest { Active = true, Enable = true };
            helper.Insert(item);

            // 存储格式：true → "true"
            Assert.Equal("true", helper.ExecuteScalar<string>("SELECT Active FROM Tb_Bool2StringTest WHERE Id = @0", item.Id));

            var loaded = helper.FirstOrDefault<Tb_Bool2StringTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.True(loaded.Active);
            Assert.True(loaded.Enable);

            // false → "false"
            item.Active = false;
            item.Enable = false;
            helper.Update(item);

            Assert.Equal("false", helper.ExecuteScalar<string>("SELECT Active FROM Tb_Bool2StringTest WHERE Id = @0", item.Id));

            var loaded2 = helper.FirstOrDefault<Tb_Bool2StringTest>(item.Id);
            Assert.NotNull(loaded2);
            Assert.False(loaded2.Active);
            Assert.False(loaded2.Enable);
        }

        [Fact]
        public void 可空布尔_Null值序列化与反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Bool2StringTest));

            var item = new Tb_Bool2StringTest { Active = false, Enable = null };
            helper.Insert(item);

            // 数据库中实际存 NULL
            var raw = helper.ExecuteScalar<object>("SELECT Enable FROM Tb_Bool2StringTest WHERE Id = @0", item.Id);
            Assert.True(raw == null || raw == DBNull.Value);

            var loaded = helper.FirstOrDefault<Tb_Bool2StringTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Enable);
        }

        [Fact]
        public void 可空布尔_数据库NULL值读取()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Bool2StringTest));

            // 直接 SQL 插入 NULL，模拟历史数据
            helper.Execute("INSERT INTO Tb_Bool2StringTest (Id, Active, Enable) VALUES (@0, 'true', NULL)", 1);

            var loaded = helper.FirstOrDefault<Tb_Bool2StringTest>(1);
            Assert.NotNull(loaded);
            Assert.True(loaded.Active);
            Assert.Null(loaded.Enable);
        }

        [Fact]
        public void 读取_兼容0和1整数文本()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Bool2StringTest));

            // 直接 SQL 插入 "1"/"0"，模拟历史数据
            helper.Execute("INSERT INTO Tb_Bool2StringTest (Id, Active, Enable) VALUES (@0, '1', '0')", 1);

            var loaded = helper.FirstOrDefault<Tb_Bool2StringTest>(1);
            Assert.NotNull(loaded);
            Assert.True(loaded.Active);
            Assert.False(loaded.Enable);
        }
    }

    /// <summary>
    /// Bool2StringColumnSerializer：bool ↔ "true"/"false" 文本
    /// </summary>
    public class Bool2StringColumnSerializerTests
    {
        [Fact]
        public void Serialize_true_false()
        {
            var serializer = new Bool2StringColumnSerializer();
            Assert.Equal("true", serializer.Serialize(true));
            Assert.Equal("false", serializer.Serialize(false));
        }

        [Fact]
        public void Deserialize_true_false()
        {
            var serializer = new Bool2StringColumnSerializer();
            Assert.Equal(true, serializer.Deserialize("true", typeof(bool)));
            Assert.Equal(false, serializer.Deserialize("false", typeof(bool)));
        }

        [Fact]
        public void Deserialize_兼容0和1()
        {
            var serializer = new Bool2StringColumnSerializer();
            Assert.Equal(true, serializer.Deserialize("1", typeof(bool)));
            Assert.Equal(false, serializer.Deserialize("0", typeof(bool)));
        }

        [Fact]
        public void Serialize_Deserialize_null()
        {
            var serializer = new Bool2StringColumnSerializer();
            Assert.Null(serializer.Serialize(null));
            Assert.Null(serializer.Deserialize(null, typeof(bool?)));
        }

        [Fact]
        public void Deserialize_非法输入抛出异常()
        {
            var serializer = new Bool2StringColumnSerializer();
            Assert.Throws<FormatException>(() => serializer.Deserialize("abc", typeof(bool)));
        }
    }
}
