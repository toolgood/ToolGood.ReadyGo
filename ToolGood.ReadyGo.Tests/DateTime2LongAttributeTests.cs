using System;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_DateTime2LongTest")]
    [PrimaryKey("Id")]
    public class Tb_DateTime2LongTest
    {
        public int Id { get; set; }

        [DateTime2Long]
        public DateTime TradeTime { get; set; }

        [DateTime2Long]
        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// [DateTime2Long] 属性：时间以 yyyyMMddHHmmss 整数保存
    /// </summary>
    public class DateTime2LongAttributeTests
    {
        [Fact]
        public void Insert_时间转整数保存_读取还原()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTime2LongTest));

            // 带毫秒，保存时截断到秒
            var tradeTime = new DateTime(2026, 8, 23, 15, 30, 45, 999);
            var item = new Tb_DateTime2LongTest { TradeTime = tradeTime };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_DateTime2LongTest>(item.Id);
            Assert.NotNull(loaded);
            // 毫秒部分被截断，时分秒保留
            Assert.Equal(new DateTime(2026, 8, 23, 15, 30, 45), loaded.TradeTime);
        }

        [Fact]
        public void Update_时间转整数保存()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTime2LongTest));

            var item = new Tb_DateTime2LongTest { TradeTime = new DateTime(2026, 1, 1, 0, 0, 0) };
            helper.Insert(item);

            item.TradeTime = new DateTime(2026, 12, 31, 23, 59, 59);
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_DateTime2LongTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(new DateTime(2026, 12, 31, 23, 59, 59), loaded.TradeTime);
        }

        [Fact]
        public void Insert_null()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTime2LongTest));

            var item = new Tb_DateTime2LongTest { TradeTime = new DateTime(2026, 8, 23, 15, 30, 45), EndTime = null };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_DateTime2LongTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.EndTime);
        }

        [Fact]
        public void 存储格式_yyyyMMddHHmmss整数()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTime2LongTest));

            var item = new Tb_DateTime2LongTest { TradeTime = new DateTime(2026, 8, 23, 15, 30, 45) };
            helper.Insert(item);

            // 数据库中实际存 20260823153045
            Assert.Equal("20260823153045", helper.ExecuteScalar<string>("SELECT TradeTime FROM Tb_DateTime2LongTest WHERE Id = @0", item.Id));
        }
    }

    /// <summary>
    /// DateTime2LongColumnSerializer：DateTime / DateTimeOffset / DateOnly 支持
    /// </summary>
    public class DateTime2LongColumnSerializerTests
    {
        [Fact]
        public void Serialize_Deserialize_DateTime()
        {
            var serializer = new DateTime2LongColumnSerializer();
            var time = new DateTime(2026, 8, 23, 15, 30, 45, 500);

            var serialized = serializer.Serialize(time);
            Assert.Equal(20260823153045L, serialized);

            var restored = (DateTime)serializer.Deserialize(20260823153045L, typeof(DateTime));
            Assert.Equal(new DateTime(2026, 8, 23, 15, 30, 45), restored);
        }

        [Fact]
        public void Serialize_Deserialize_DateTimeOffset()
        {
            var serializer = new DateTime2LongColumnSerializer();
            var time = new DateTimeOffset(2026, 12, 1, 23, 59, 59, TimeSpan.FromHours(8));

            var serialized = serializer.Serialize(time);
            Assert.Equal(20261201235959L, serialized);

            var restored = (DateTimeOffset)serializer.Deserialize(20261201235959L, typeof(DateTimeOffset));
            Assert.Equal(new DateTime(2026, 12, 1, 23, 59, 59), restored.DateTime);
        }

        [Fact]
        public void Serialize_Deserialize_DateOnly()
        {
            var serializer = new DateTime2LongColumnSerializer();
            var date = new DateOnly(2026, 3, 9);

            var serialized = serializer.Serialize(date);
            // 无时间部分，时分秒为 0
            Assert.Equal(20260309000000L, serialized);

            var restored = (DateOnly)serializer.Deserialize(20260309000000L, typeof(DateOnly));
            Assert.Equal(date, restored);
        }

        [Fact]
        public void Serialize_Deserialize_null()
        {
            var serializer = new DateTime2LongColumnSerializer();
            Assert.Null(serializer.Serialize(null));
            Assert.Null(serializer.Deserialize(null, typeof(DateTime)));
        }

        [Fact]
        public void Deserialize_字符串输入()
        {
            var serializer = new DateTime2LongColumnSerializer();
            var restored = (DateTime)serializer.Deserialize("20260823153045", typeof(DateTime));
            Assert.Equal(new DateTime(2026, 8, 23, 15, 30, 45), restored);
        }
    }
}
