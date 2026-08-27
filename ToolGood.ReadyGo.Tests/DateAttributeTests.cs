using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_DateTest")]
    [PrimaryKey("Id")]
    public class Tb_DateTest
    {
        public int Id { get; set; }

        [Date2String]
        public DateTime BirthDay { get; set; }

        public DateTime CreateTime { get; set; }
    }

    [Table("Tb_DateNullableTest")]
    [PrimaryKey("Id")]
    public class Tb_DateNullableTest
    {
        public int Id { get; set; }

        [Date2String]
        public DateTime? NullableDay { get; set; }
    }

    /// <summary>
    /// [Date] 属性：只保存日期，不保存时间
    /// </summary>
    public class DateAttributeTests
    {
        [Fact]
        public void Insert_只保存日期()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTest));

            var birthDay = new DateTime(2026, 8, 22, 23, 59, 59, 999);
            var createTime = new DateTime(2026, 1, 1, 10, 30, 45, 123);
            var item = new Tb_DateTest { BirthDay = birthDay, CreateTime = createTime };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_DateTest>(item.Id);
            Assert.NotNull(loaded);
            // [Date] 列：时间部分被截断为 00:00:00
            Assert.Equal(birthDay.Date, loaded.BirthDay);
            Assert.Equal(TimeSpan.Zero, loaded.BirthDay.TimeOfDay);
            // 普通列：完整时间保留
            Assert.Equal(createTime, loaded.CreateTime);

            // 数据库中实际只存 "yyyy-MM-dd" 纯日期文本
            Assert.Equal("2026-08-22", helper.ExecuteScalar<string>("SELECT BirthDay FROM Tb_DateTest WHERE Id = @0", item.Id));
            Assert.StartsWith("2026-01-01", helper.ExecuteScalar<string>("SELECT CreateTime FROM Tb_DateTest WHERE Id = @0", item.Id));
        }

        [Fact]
        public void Read_兼容带时间部分的日期字符串()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTest));

            // 模拟历史数据：数据库中存在 "yyyy-MM-dd HH:mm:ss" 格式的日期
            helper.Execute("INSERT INTO Tb_DateTest (Id, BirthDay, CreateTime) VALUES (@0, '1991-04-03 00:00:00', '2020-01-01 10:00:00')", 1);

            var loaded = helper.FirstOrDefault<Tb_DateTest>(1);
            Assert.NotNull(loaded);
            Assert.Equal(new DateTime(1991, 4, 3), loaded.BirthDay);
        }

        [Fact]
        public void Update_只保存日期()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateTest));

            var item = new Tb_DateTest { BirthDay = new DateTime(2026, 1, 1, 8, 0, 0), CreateTime = DateTime.Now };
            helper.Insert(item);

            item.BirthDay = new DateTime(2026, 12, 31, 23, 59, 59);
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_DateTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(new DateTime(2026, 12, 31), loaded.BirthDay);
        }

        [Fact]
        public void 可空日期_Null值序列化与反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateNullableTest));

            var item = new Tb_DateNullableTest { NullableDay = null };
            helper.Insert(item);

            // 数据库中实际存 NULL
            var raw = helper.ExecuteScalar<object>("SELECT NullableDay FROM Tb_DateNullableTest WHERE Id = @0", item.Id);
            Assert.True(raw == null || raw == DBNull.Value);

            // 反序列化回 DateTime? 应为 null
            var loaded = helper.FirstOrDefault<Tb_DateNullableTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.NullableDay);
        }

        [Fact]
        public void 可空日期_数据库NULL值读取()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateNullableTest));

            // 直接 SQL 插入 NULL，模拟历史数据
            helper.Execute("INSERT INTO Tb_DateNullableTest (Id, NullableDay) VALUES (@0, NULL)", 1);

            var loaded = helper.FirstOrDefault<Tb_DateNullableTest>(1);
            Assert.NotNull(loaded);
            Assert.Null(loaded.NullableDay);
        }

        [Fact]
        public void 可空日期_有值反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_DateNullableTest));

            var item = new Tb_DateNullableTest { NullableDay = new DateTime(1991, 4, 3, 23, 59, 59) };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_DateNullableTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.NotNull(loaded.NullableDay);
            Assert.Equal(new DateTime(1991, 4, 3), loaded.NullableDay.Value);
        }
    }
}
