using System;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_DateTime2StringTest")]
    [PrimaryKey("Id")]
    public class Tb_DateTime2StringTest
    {
        public int Id { get; set; }

        [DateTime2String]
        public DateTime TradeTime { get; set; }

        [DateTime2String]
        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// [DateTime2String] 属性：时间以 "yyyy-MM-dd HH:mm:ss" 文本保存
    /// </summary>
    public class DateTime2StringAttributeTests
    {
        [Fact]
        public void Insert_时间以完整字符串保存()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTime2StringTest));

            var item = new Tb_DateTime2StringTest { TradeTime = new DateTime(2026, 8, 23, 15, 30, 45) };
            helper.Insert(item);

            // 数据库中实际存 "yyyy-MM-dd HH:mm:ss" 文本
            Assert.Equal("2026-08-23 15:30:45", helper.ExecuteScalar<string>("SELECT TradeTime FROM Tb_DateTime2StringTest WHERE Id = @0", item.Id));

            var loaded = helper.FirstOrDefault<Tb_DateTime2StringTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(new DateTime(2026, 8, 23, 15, 30, 45), loaded.TradeTime);
        }

        [Fact]
        public void Update_时间以完整字符串保存()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTime2StringTest));

            var item = new Tb_DateTime2StringTest { TradeTime = new DateTime(2026, 1, 1, 0, 0, 0) };
            helper.Insert(item);

            item.TradeTime = new DateTime(2026, 12, 31, 23, 59, 59);
            helper.Update(item);

            Assert.Equal("2026-12-31 23:59:59", helper.ExecuteScalar<string>("SELECT TradeTime FROM Tb_DateTime2StringTest WHERE Id = @0", item.Id));

            var loaded = helper.FirstOrDefault<Tb_DateTime2StringTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(new DateTime(2026, 12, 31, 23, 59, 59), loaded.TradeTime);
        }

        [Fact]
        public void 可空时间_Null值序列化与反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTime2StringTest));

            var item = new Tb_DateTime2StringTest { TradeTime = new DateTime(2026, 8, 23, 15, 30, 45), EndTime = null };
            helper.Insert(item);

            // 数据库中实际存 NULL
            var raw = helper.ExecuteScalar<object>("SELECT EndTime FROM Tb_DateTime2StringTest WHERE Id = @0", item.Id);
            Assert.True(raw == null || raw == DBNull.Value);

            var loaded = helper.FirstOrDefault<Tb_DateTime2StringTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.EndTime);
        }

        [Fact]
        public void 可空时间_数据库NULL值读取()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTime2StringTest));

            // 直接 SQL 插入 NULL，模拟历史数据
            helper.Execute("INSERT INTO Tb_DateTime2StringTest (Id, TradeTime, EndTime) VALUES (@0, '2026-08-23 15:30:45', NULL)", 1);

            var loaded = helper.FirstOrDefault<Tb_DateTime2StringTest>(1);
            Assert.NotNull(loaded);
            Assert.Equal(new DateTime(2026, 8, 23, 15, 30, 45), loaded.TradeTime);
            Assert.Null(loaded.EndTime);
        }

        [Fact]
        public void 读取_兼容仅日期字符串()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTime2StringTest));

            // 模拟历史数据：数据库中只有 "yyyy-MM-dd"
            helper.Execute("INSERT INTO Tb_DateTime2StringTest (Id, TradeTime, EndTime) VALUES (@0, '2026-01-01', NULL)", 1);

            var loaded = helper.FirstOrDefault<Tb_DateTime2StringTest>(1);
            Assert.NotNull(loaded);
            Assert.Equal(new DateTime(2026, 1, 1), loaded.TradeTime);
        }
    }

    /// <summary>
    /// DateTime2StringColumnSerializer：DateTime / DateTimeOffset ↔ "yyyy-MM-dd HH:mm:ss" 文本
    /// </summary>
    public class DateTime2StringColumnSerializerTests
    {
        [Fact]
        public void Serialize_Deserialize_DateTime()
        {
            var serializer = new DateTime2StringColumnSerializer();
            var time = new DateTime(2026, 8, 23, 15, 30, 45);

            var serialized = serializer.Serialize(time);
            Assert.Equal("2026-08-23 15:30:45", serialized);

            var restored = (DateTime)serializer.Deserialize("2026-08-23 15:30:45", typeof(DateTime));
            Assert.Equal(time, restored);
        }

        [Fact]
        public void Serialize_Deserialize_DateTimeOffset()
        {
            var serializer = new DateTime2StringColumnSerializer();
            var time = new DateTimeOffset(2026, 12, 1, 23, 59, 59, TimeSpan.FromHours(8));

            var serialized = serializer.Serialize(time);
            Assert.Equal("2026-12-01 23:59:59", serialized);

            var restored = (DateTimeOffset)serializer.Deserialize("2026-12-01 23:59:59", typeof(DateTimeOffset));
            Assert.Equal(new DateTime(2026, 12, 1, 23, 59, 59), restored.DateTime);
        }

        [Fact]
        public void Deserialize_兼容仅日期字符串()
        {
            var serializer = new DateTime2StringColumnSerializer();
            var restored = (DateTime)serializer.Deserialize("2026-08-23", typeof(DateTime));
            Assert.Equal(new DateTime(2026, 8, 23), restored);
        }

        [Fact]
        public void Serialize_Deserialize_null()
        {
            var serializer = new DateTime2StringColumnSerializer();
            Assert.Null(serializer.Serialize(null));
            Assert.Null(serializer.Deserialize(null, typeof(DateTime?)));
        }

        [Fact]
        public void Deserialize_非法输入抛出异常()
        {
            var serializer = new DateTime2StringColumnSerializer();
            Assert.Throws<FormatException>(() => serializer.Deserialize("not-a-date", typeof(DateTime)));
        }
    }
}
