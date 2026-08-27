using System;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_Date2IntTest")]
    [PrimaryKey("Id")]
    public class Tb_Date2IntTest
    {
        public int Id { get; set; }

        [Date2Int]
        public DateTime TradeDate { get; set; }

        [Date2Int]
        public DateTime? SettleDate { get; set; }
    }

    /// <summary>
    /// [Date2Int] 属性：日期以 yyyyMMdd 整数保存
    /// </summary>
    public class Date2IntAttributeTests
    {
        [Fact]
        public void Insert_日期转整数保存_读取还原()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Date2IntTest));

            // 带时间部分，保存时只保留 yyyyMMdd
            var tradeDate = new DateTime(2026, 8, 23, 23, 59, 59, 999);
            var item = new Tb_Date2IntTest { TradeDate = tradeDate };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_Date2IntTest>(item.Id);
            Assert.NotNull(loaded);
            // 时间部分被截断，仅保留日期
            Assert.Equal(new DateTime(2026, 8, 23), loaded.TradeDate);
        }

        [Fact]
        public void Update_日期转整数保存()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Date2IntTest));

            var item = new Tb_Date2IntTest { TradeDate = new DateTime(2026, 1, 1, 8, 0, 0) };
            helper.Insert(item);

            item.TradeDate = new DateTime(2026, 12, 31, 23, 59, 59);
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_Date2IntTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(new DateTime(2026, 12, 31), loaded.TradeDate);
        }

        [Fact]
        public void Insert_null()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Date2IntTest));

            var item = new Tb_Date2IntTest { TradeDate = new DateTime(2026, 8, 23), SettleDate = null };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_Date2IntTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.SettleDate);
        }

        [Fact]
        public void 存储格式_yyyyMMdd整数()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Date2IntTest));

            var item = new Tb_Date2IntTest { TradeDate = new DateTime(2026, 8, 23, 15, 30, 0) };
            helper.Insert(item);

            // 数据库中实际存 20260823
            Assert.Equal("20260823", helper.ExecuteScalar<string>("SELECT TradeDate FROM Tb_Date2IntTest WHERE Id = @0", item.Id));
        }
    }

    /// <summary>
    /// Date2IntColumnSerializer：DateOnly / DateTimeOffset 支持
    /// </summary>
    public class Date2IntColumnSerializerTests
    {
        [Fact]
        public void Serialize_Deserialize_DateOnly()
        {
            var serializer = new Date2IntColumnSerializer();
            var date = new DateOnly(2026, 3, 9);

            var serialized = serializer.Serialize(date);
            Assert.Equal(20260309, serialized);

            var restored = (DateOnly)serializer.Deserialize(20260309, typeof(DateOnly));
            Assert.Equal(date, restored);
        }

        [Fact]
        public void Serialize_Deserialize_DateTimeOffset()
        {
            var serializer = new Date2IntColumnSerializer();
            var date = new DateTimeOffset(2026, 12, 1, 23, 59, 59, TimeSpan.FromHours(8));

            var serialized = serializer.Serialize(date);
            Assert.Equal(20261201, serialized);

            var restored = (DateTimeOffset)serializer.Deserialize(20261201, typeof(DateTimeOffset));
            Assert.Equal(new DateTime(2026, 12, 1), restored.DateTime);
        }

        [Fact]
        public void Serialize_Deserialize_DateTime()
        {
            var serializer = new Date2IntColumnSerializer();
            var date = new DateTime(2026, 8, 23, 10, 20, 30, 500);

            var serialized = serializer.Serialize(date);
            Assert.Equal(20260823, serialized);

            var restored = (DateTime)serializer.Deserialize(20260823, typeof(DateTime));
            Assert.Equal(new DateTime(2026, 8, 23), restored);
        }

        [Fact]
        public void Serialize_Deserialize_null()
        {
            var serializer = new Date2IntColumnSerializer();
            Assert.Null(serializer.Serialize(null));
            Assert.Null(serializer.Deserialize(null, typeof(DateTime)));
        }

        [Fact]
        public void Deserialize_字符串输入()
        {
            var serializer = new Date2IntColumnSerializer();
            var restored = (DateTime)serializer.Deserialize("20260823", typeof(DateTime));
            Assert.Equal(new DateTime(2026, 8, 23), restored);
        }
    }
}
