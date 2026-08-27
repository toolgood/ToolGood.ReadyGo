using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_MoneyTest")]
    [PrimaryKey("Id")]
    public class Tb_MoneyTest
    {
        public int Id { get; set; }

        [Numeric2Int(2)]
        public decimal Money { get; set; }

        [Numeric2Long(3)]
        public double Weight { get; set; }
    }

    [Table("Tb_MoneyNullableTest")]
    [PrimaryKey("Id")]
    public class Tb_MoneyNullableTest
    {
        public int Id { get; set; }

        [Numeric2Int(2)]
        public decimal? Money { get; set; }

        [Numeric2Long(3)]
        public double? Weight { get; set; }
    }

    /// <summary>
    /// [Numeric2Int] / [Numeric2Long] 属性：小数转整数存储
    /// </summary>
    public class Decimal2AttributeTests
    {
        [Fact]
        public void Insert_小数转整数保存_读取还原()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_MoneyTest));

            var item = new Tb_MoneyTest { Money = 1.23m, Weight = 1.2345 };
            helper.Insert(item);

            // 数据库中实际保存为整数（×10^scale，四舍五入）
            Assert.Equal(123.0, helper.ExecuteScalar<double>("SELECT Money FROM Tb_MoneyTest WHERE Id = @0", item.Id));
            Assert.Equal(1235.0, helper.ExecuteScalar<double>("SELECT Weight FROM Tb_MoneyTest WHERE Id = @0", item.Id));

            // 读取时 ÷10^scale 并四舍五入还原
            var loaded = helper.FirstOrDefault<Tb_MoneyTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(1.23m, loaded.Money);
            Assert.Equal(1.235, loaded.Weight);
        }

        [Fact]
        public void Update_小数转整数保存()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_MoneyTest));

            var item = new Tb_MoneyTest { Money = 1.00m, Weight = 1.0 };
            helper.Insert(item);

            item.Money = 999.99m;
            item.Weight = 1.2346;
            helper.Update(item);

            Assert.Equal(99999.0, helper.ExecuteScalar<double>("SELECT Money FROM Tb_MoneyTest WHERE Id = @0", item.Id));
            Assert.Equal(1235.0, helper.ExecuteScalar<double>("SELECT Weight FROM Tb_MoneyTest WHERE Id = @0", item.Id));

            var loaded = helper.FirstOrDefault<Tb_MoneyTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(999.99m, loaded.Money);
            Assert.Equal(1.235, loaded.Weight);
        }

        [Fact]
        public void 可空小数_Null值序列化与反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_MoneyNullableTest));

            var item = new Tb_MoneyNullableTest { Money = null, Weight = null };
            helper.Insert(item);

            // 数据库中实际存 NULL
            var raw = helper.ExecuteScalar<object>("SELECT Money FROM Tb_MoneyNullableTest WHERE Id = @0", item.Id);
            Assert.True(raw == null || raw == DBNull.Value);

            // 反序列化回 decimal? / double? 应为 null
            var loaded = helper.FirstOrDefault<Tb_MoneyNullableTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Money);
            Assert.Null(loaded.Weight);
        }

        [Fact]
        public void 可空小数_数据库NULL值读取()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_MoneyNullableTest));

            // 直接 SQL 插入 NULL，模拟历史数据
            helper.Execute("INSERT INTO Tb_MoneyNullableTest (Id, Money, Weight) VALUES (@0, NULL, NULL)", 1);

            var loaded = helper.FirstOrDefault<Tb_MoneyNullableTest>(1);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Money);
            Assert.Null(loaded.Weight);
        }

        [Fact]
        public void 可空小数_有值反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_MoneyNullableTest));

            var item = new Tb_MoneyNullableTest { Money = 1.23m, Weight = 1.2345 };
            helper.Insert(item);

            // 库中存整数（×10^scale）
            Assert.Equal(123.0, helper.ExecuteScalar<double>("SELECT Money FROM Tb_MoneyNullableTest WHERE Id = @0", item.Id));
            Assert.Equal(1235.0, helper.ExecuteScalar<double>("SELECT Weight FROM Tb_MoneyNullableTest WHERE Id = @0", item.Id));

            var loaded = helper.FirstOrDefault<Tb_MoneyNullableTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(1.23m, loaded.Money.Value);
            Assert.Equal(1.235, loaded.Weight.Value);
        }
    }
}
