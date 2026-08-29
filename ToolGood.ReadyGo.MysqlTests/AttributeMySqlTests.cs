using Xunit;

namespace ToolGood.ReadyGo.MysqlTests
{
    /// <summary>
    /// byte[] 保存与读取（基于 MySQL）
    /// </summary>
    [Collection("MySql")]
    public class ByteArrayMySqlTests
    {
        private static MySqlTestDb CreateTable()
        {
            var db = MySqlTestDb.Create();
            db.Helper.TableHelper.DropTable(typeof(Tb_BlobTest));
            db.Helper.TableHelper.TryCreateTable(typeof(Tb_BlobTest));
            return db;
        }

        [Fact]
        public void Insert_读回_byte数组相等()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var data = new byte[] { 0x00, 0x01, 0x02, 0xFF, 0x10, 0x7F, 0x80 };
            var item = new Tb_BlobTest { Name = "blob1", Data = data };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_BlobTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal("blob1", loaded.Name);
            Assert.Equal(data, loaded.Data);
        }

        [Fact]
        public void Insert_空数组_和_null()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var empty = new Tb_BlobTest { Name = "empty", Data = Array.Empty<byte>() };
            helper.Insert(empty);
            var loadedEmpty = helper.FirstOrDefault<Tb_BlobTest>(empty.Id);
            Assert.NotNull(loadedEmpty);
            Assert.Empty(loadedEmpty.Data);

            var nullItem = new Tb_BlobTest { Name = "null", Data = null };
            helper.Insert(nullItem);
            var loadedNull = helper.FirstOrDefault<Tb_BlobTest>(nullItem.Id);
            Assert.NotNull(loadedNull);
            Assert.Null(loadedNull.Data);
        }

        [Fact]
        public void Update_byte数组()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var item = new Tb_BlobTest { Name = "old", Data = new byte[] { 1, 2, 3 } };
            helper.Insert(item);

            item.Data = new byte[] { 9, 8, 7, 6 };
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_BlobTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(new byte[] { 9, 8, 7, 6 }, loaded.Data);
        }

        [Fact]
        public async Task Insert_异步_读回_byte数组()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var data = new byte[] { 0x11, 0x22, 0x33, 0x44 };
            var item = new Tb_BlobTest { Name = "async-blob", Data = data };
            await helper.Insert_Async(item);

            var loaded = await helper.FirstOrDefault_Async<Tb_BlobTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(data, loaded.Data);
        }
    }

    /// <summary>
    /// [Numeric2Int] / [Numeric2Long] 属性：小数转整数存储（基于 MySQL）
    /// </summary>
    [Collection("MySql")]
    public class Decimal2MySqlTests
    {
        private static MySqlTestDb CreateTable()
        {
            var db = MySqlTestDb.Create();
            db.Helper.TableHelper.DropTable(typeof(Tb_MoneyTest));
            db.Helper.TableHelper.TryCreateTable(typeof(Tb_MoneyTest));
            return db;
        }

        [Fact]
        public void Insert_小数转整数保存_读取还原()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var item = new Tb_MoneyTest { Money = 1.23m, Weight = 1.2345 };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_MoneyTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(1.23m, loaded.Money);
            Assert.Equal(1.235, loaded.Weight);
        }

        [Fact]
        public void Update_小数转整数保存()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var item = new Tb_MoneyTest { Money = 1.00m, Weight = 1.0 };
            helper.Insert(item);

            item.Money = 999.99m;
            item.Weight = 1.2346;
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_MoneyTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(999.99m, loaded.Money);
            Assert.Equal(1.235, loaded.Weight);
        }
    }

    /// <summary>
    /// [NumericArray] 属性：数值数组以 byte[] 保存（基于 MySQL）
    /// </summary>
    [Collection("MySql")]
    public class NumericArrayMySqlTests
    {
        private static MySqlTestDb CreateTable()
        {
            var db = MySqlTestDb.Create();
            db.Helper.TableHelper.DropTable(typeof(Tb_NumericArrayTest));
            db.Helper.TableHelper.TryCreateTable(typeof(Tb_NumericArrayTest));
            return db;
        }

        [Fact]
        public void Insert_float数组以byte保存_读回一致()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var item = new Tb_NumericArrayTest {
                Floats = new[] { 1.5f, -2.25f, 3.125f },
                ValueList = new List<float> { 0.1f, 10f, 100.5f }
            };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_NumericArrayTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Floats, loaded.Floats);
            Assert.Equal(item.ValueList, loaded.ValueList);
        }

        [Fact]
        public void Update_float数组()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var item = new Tb_NumericArrayTest {
                Floats = new[] { 1f, 2f },
                ValueList = new List<float> { 3f }
            };
            helper.Insert(item);

            item.Floats = new[] { 9.5f, 8.25f };
            item.ValueList = new List<float> { 7f, 6f, 5f };
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_NumericArrayTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Floats, loaded.Floats);
            Assert.Equal(item.ValueList, loaded.ValueList);
        }

        [Fact]
        public void Insert_null()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var item = new Tb_NumericArrayTest { Floats = null, ValueList = null };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_NumericArrayTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Floats);
            Assert.Null(loaded.ValueList);
        }

        [Fact]
        public void double_int_数组_读回一致()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var doubles = new double[] { 1.5, -2.25, 3.141592653589793 };
            var ints = new List<int> { -1, 0, 1, 100 };
            var item = new Tb_NumericArrayTest { Doubles = doubles, Ints = ints };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_NumericArrayTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(doubles, loaded.Doubles);
            Assert.Equal(ints, loaded.Ints);
        }
    }
}
