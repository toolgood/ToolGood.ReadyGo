using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_Bool2IntTest")]
    [PrimaryKey("Id")]
    public class Tb_Bool2IntTest
    {
        public int Id { get; set; }

        [Bool2Int]
        public bool Active { get; set; }

        [Bool2Int]
        public bool? Enable { get; set; }
    }

    /// <summary>
    /// [Bool2Int] 属性：bool 以 0/1 整数保存
    /// </summary>
    public class Bool2IntAttributeTests
    {
        [Fact]
        public void 可空布尔_Null值序列化与反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_Bool2IntTest));

            var item = new Tb_Bool2IntTest { Active = false, Enable = null };
            helper.Insert(item);

            // 数据库中实际存 NULL
            var raw = helper.ExecuteScalar<object>("SELECT Enable FROM Tb_Bool2IntTest WHERE Id = @0", item.Id);
            Assert.True(raw == null || raw == DBNull.Value);

            var loaded = helper.FirstOrDefault<Tb_Bool2IntTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Enable);
        }

        [Fact]
        public void 可空布尔_数据库NULL值读取()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_Bool2IntTest));

            // 直接 SQL 插入 NULL，模拟历史数据
            helper.Execute("INSERT INTO Tb_Bool2IntTest (Id, Active, Enable) VALUES (@0, 1, NULL)", 1);

            var loaded = helper.FirstOrDefault<Tb_Bool2IntTest>(1);
            Assert.NotNull(loaded);
            Assert.True(loaded.Active);
            Assert.Null(loaded.Enable);
        }

        [Fact]
        public void 布尔_以0和1整数存储()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_Bool2IntTest));

            var item = new Tb_Bool2IntTest { Active = true, Enable = true };
            helper.Insert(item);

            // 存储格式：true → 1
            Assert.Equal(1, System.Convert.ToInt32(helper.ExecuteScalar<object>("SELECT Active FROM Tb_Bool2IntTest WHERE Id = @0", item.Id)));

            var loaded = helper.FirstOrDefault<Tb_Bool2IntTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.True(loaded.Active);
            Assert.True(loaded.Enable);

            // false → 0
            item.Active = false;
            item.Enable = false;
            helper.Update(item);

            Assert.Equal(0, System.Convert.ToInt32(helper.ExecuteScalar<object>("SELECT Active FROM Tb_Bool2IntTest WHERE Id = @0", item.Id)));

            var loaded2 = helper.FirstOrDefault<Tb_Bool2IntTest>(item.Id);
            Assert.NotNull(loaded2);
            Assert.False(loaded2.Active);
            Assert.False(loaded2.Enable);
        }
    }
}
