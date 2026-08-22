using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_DateTest")]
    [PrimaryKey("Id")]
    public class Tb_DateTest
    {
        public int Id { get; set; }

        [Date]
        public DateTime BirthDay { get; set; }

        public DateTime CreateTime { get; set; }
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
            helper._TableHelper.TryCreateTable(typeof(Tb_DateTest));

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
        public void Update_只保存日期()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_DateTest));

            var item = new Tb_DateTest { BirthDay = new DateTime(2026, 1, 1, 8, 0, 0), CreateTime = DateTime.Now };
            helper.Insert(item);

            item.BirthDay = new DateTime(2026, 12, 31, 23, 59, 59);
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_DateTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(new DateTime(2026, 12, 31), loaded.BirthDay);
        }
    }
}
