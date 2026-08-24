using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_TimestampNullableTest")]
    [PrimaryKey("Id")]
    public class Tb_TimestampNullableTest
    {
        public int Id { get; set; }

        [Timestamp]
        public DateTime? CreateTime { get; set; }

        [Timestamp(TimestampPrecision.Milliseconds)]
        public DateTime? UpdateTime { get; set; }
    }

    /// <summary>
    /// [Timestamp] 属性：以 Unix 时间戳（UTC 基准）保存，支持 DateTime?
    /// </summary>
    public class TimestampAttributeTests
    {
        [Fact]
        public void 可空时间戳_Null值序列化与反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_TimestampNullableTest));

            var item = new Tb_TimestampNullableTest { CreateTime = null, UpdateTime = null };
            helper.Insert(item);

            // 数据库中实际存 NULL
            var raw = helper.ExecuteScalar<object>("SELECT CreateTime FROM Tb_TimestampNullableTest WHERE Id = @0", item.Id);
            Assert.True(raw == null || raw == DBNull.Value);

            // 反序列化回 DateTime? 应为 null
            var loaded = helper.FirstOrDefault<Tb_TimestampNullableTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.CreateTime);
            Assert.Null(loaded.UpdateTime);
        }

        [Fact]
        public void 可空时间戳_数据库NULL值读取()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_TimestampNullableTest));

            // 直接 SQL 插入 NULL，模拟历史数据
            helper.Execute("INSERT INTO Tb_TimestampNullableTest (Id, CreateTime, UpdateTime) VALUES (@0, NULL, NULL)", 1);

            var loaded = helper.FirstOrDefault<Tb_TimestampNullableTest>(1);
            Assert.NotNull(loaded);
            Assert.Null(loaded.CreateTime);
            Assert.Null(loaded.UpdateTime);
        }

        [Fact]
        public void 可空时间戳_有值反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_TimestampNullableTest));

            var utc = new DateTime(2026, 8, 23, 15, 30, 45, DateTimeKind.Utc);
            var item = new Tb_TimestampNullableTest { CreateTime = utc, UpdateTime = utc.AddMilliseconds(123) };
            helper.Insert(item);

            // 库中存的是 long 时间戳（非 NULL）
            var raw = helper.ExecuteScalar<long>("SELECT CreateTime FROM Tb_TimestampNullableTest WHERE Id = @0", item.Id);
            Assert.True(raw > 0);

            var loaded = helper.FirstOrDefault<Tb_TimestampNullableTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.NotNull(loaded.CreateTime);
            Assert.Equal(utc, loaded.CreateTime.Value);                        // 秒级
            Assert.Equal(utc.AddMilliseconds(123), loaded.UpdateTime.Value);   // 毫秒级
        }
    }
}
