using System.Collections.Generic;
using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_NumericArray2StringTest")]
    [PrimaryKey("Id")]
    public class Tb_NumericArray2StringTest
    {
        public int Id { get; set; }

        [NumericArray2String]
        public List<int> Scores { get; set; }

        [NumericArray2String(";")]
        public double[] Ratios { get; set; }

        [NumericArray2String]
        public List<decimal> Prices { get; set; }

        [NumericArray2String]
        public long[] BigIds { get; set; }
    }

    /// <summary>
    /// [NumericArray2String] 属性：数值数组 / List&lt;T&gt; 以分隔符文本保存（需文本列）
    /// </summary>
    public class NumericArray2StringAttributeTests
    {
        [Fact]
        public void 可空列表_Null值序列化与反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArray2StringTest));

            var item = new Tb_NumericArray2StringTest { Scores = null, Ratios = null, Prices = null, BigIds = null };
            helper.Insert(item);

            // 数据库中实际存 NULL
            var raw = helper.ExecuteScalar<object>("SELECT Scores FROM Tb_NumericArray2StringTest WHERE Id = @0", item.Id);
            Assert.True(raw == null || raw == DBNull.Value);

            var loaded = helper.FirstOrDefault<Tb_NumericArray2StringTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Scores);
            Assert.Null(loaded.Ratios);
            Assert.Null(loaded.Prices);
            Assert.Null(loaded.BigIds);
        }

        [Fact]
        public void 可空列表_数据库NULL值读取()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArray2StringTest));

            // 直接 SQL 插入 NULL，模拟历史数据
            helper.Execute("INSERT INTO Tb_NumericArray2StringTest (Id, Scores, Ratios, Prices, BigIds) VALUES (@0, NULL, NULL, NULL, NULL)", 1);

            var loaded = helper.FirstOrDefault<Tb_NumericArray2StringTest>(1);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Scores);
            Assert.Null(loaded.Ratios);
            Assert.Null(loaded.Prices);
            Assert.Null(loaded.BigIds);
        }

        [Fact]
        public void 列表_存储与还原()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArray2StringTest));

            var item = new Tb_NumericArray2StringTest {
                Scores = new List<int> { 90, 85, 95 },
                Ratios = new[] { 0.5, 0.25 },
                Prices = new List<decimal> { 9.9m, 19.9m },
                BigIds = new[] { 100L, 200L }
            };
            helper.Insert(item);

            // 存储格式：默认逗号分隔 / 自定义 ; 分隔
            Assert.Equal("90,85,95", helper.ExecuteScalar<string>("SELECT Scores FROM Tb_NumericArray2StringTest WHERE Id = @0", item.Id));
            Assert.Equal("0.5;0.25", helper.ExecuteScalar<string>("SELECT Ratios FROM Tb_NumericArray2StringTest WHERE Id = @0", item.Id));
            Assert.Equal("9.9,19.9", helper.ExecuteScalar<string>("SELECT Prices FROM Tb_NumericArray2StringTest WHERE Id = @0", item.Id));
            Assert.Equal("100,200", helper.ExecuteScalar<string>("SELECT BigIds FROM Tb_NumericArray2StringTest WHERE Id = @0", item.Id));

            var loaded = helper.FirstOrDefault<Tb_NumericArray2StringTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Scores, loaded.Scores);
            Assert.Equal(item.Ratios, loaded.Ratios);
            Assert.Equal(item.Prices, loaded.Prices);
            Assert.Equal(item.BigIds, loaded.BigIds);
        }

        [Fact]
        public void 列表_long大数精确往返()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArray2StringTest));

            // 超过 double 精确表示范围的 long，文本存储保证不丢失精度
            var big = new[] { 9007199254740993L, long.MinValue, long.MaxValue };
            var item = new Tb_NumericArray2StringTest { BigIds = big };
            helper.Insert(item);

            var loaded = helper.FirstOrDefault<Tb_NumericArray2StringTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(big, loaded.BigIds);
        }

        [Fact]
        public void 列表_空列表存空串()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArray2StringTest));

            var item = new Tb_NumericArray2StringTest { Scores = new List<int>() };
            helper.Insert(item);

            Assert.Equal("", helper.ExecuteScalar<string>("SELECT Scores FROM Tb_NumericArray2StringTest WHERE Id = @0", item.Id));

            var loaded = helper.FirstOrDefault<Tb_NumericArray2StringTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.NotNull(loaded.Scores);
            Assert.Empty(loaded.Scores);
        }

        [Fact]
        public void 列表_Update更新()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_NumericArray2StringTest));

            var item = new Tb_NumericArray2StringTest {
                Scores = new List<int> { 1, 2 },
                Ratios = new[] { 0.1, 0.2 }
            };
            helper.Insert(item);

            item.Scores = new List<int> { 9, 8, 7 };
            item.Ratios = new[] { 0.9, 0.8, 0.7 };
            helper.Update(item);

            var loaded = helper.FirstOrDefault<Tb_NumericArray2StringTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(item.Scores, loaded.Scores);
            Assert.Equal(item.Ratios, loaded.Ratios);
        }
    }
}
