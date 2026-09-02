using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.Gadget.TableManager;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_Order")]
    [PrimaryKey("Id")]
    [Index("UserId")]
    [Unique("OrderNo")]
    public class Tb_Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [FieldLength(50)]
        public string OrderNo { get; set; }

        [Text]
        public string Remark { get; set; }

        [DefaultValue("0")]
        public decimal Money { get; set; }

        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// TableManager（SqlHelper._TableHelper）单元测试（SQLite）
    /// </summary>
    public class TableManagerTests
    {
        [Fact]
        public void TableInfo_FromType_解析()
        {
            var ti = TableInfo.FromType(typeof(Tb_Order));

            Assert.Equal("Tb_Order", ti.TableName);
            Assert.Equal("Id", ti.PrimaryKey);
            Assert.True(ti.AutoIncrement);
            Assert.Equal(6, ti.Columns.Count);
            Assert.Single(ti.Indexs);
            Assert.Equal("UserId", ti.Indexs[0][0]);
            Assert.Single(ti.Uniques);
            Assert.Equal("OrderNo", ti.Uniques[0][0]);

            var orderNo = ti.Columns.First(c => c.ColumnName == "OrderNo");
            Assert.Equal("50", orderNo.FieldLength);
            Assert.False(orderNo.IsText);

            var remark = ti.Columns.First(c => c.ColumnName == "Remark");
            Assert.True(remark.IsText);

            var money = ti.Columns.First(c => c.ColumnName == "Money");
            Assert.Equal("0", money.DefaultValue);
        }

        [Fact]
        public void GetTryCreateTable_SQLite_包含关键片段()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            var sql = helper._TableHelper.GetTryCreateTable(typeof(Tb_Order));

            Assert.Contains("CREATE TABLE IF NOT EXISTS [Tb_Order]", sql);
            Assert.Contains("[Id] INTEGER NOT NULL PRIMARY KEY AutoIncrement", sql);
            Assert.Contains("[OrderNo] Text", sql);
            Assert.Contains("DEFAULT(0)", sql);
            Assert.Contains("CREATE INDEX IF NOT EXISTS i_Tb_Order_UserId ON [Tb_Order]", sql);
            Assert.Contains("CREATE UNIQUE INDEX IF NOT EXISTS u_Tb_Order_OrderNo ON [Tb_Order]", sql);
        }

        [Fact]
        public void TryCreateTable_然后_Insert_查询()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            helper._TableHelper.TryCreateTable(typeof(Tb_Order));

            var order = new Tb_Order { UserId = 1, OrderNo = "A001", Remark = "测试", Money = 9.9m, CreateTime = DateTime.Now };
            helper.Insert(order);
            Assert.True(order.Id > 0);

            var loaded = helper.FirstOrDefault<Tb_Order>(order.Id);
            Assert.NotNull(loaded);
            Assert.Equal("A001", loaded.OrderNo);
            Assert.Equal("测试", loaded.Remark);
        }

        [Fact]
        public void TryCreateTable_幂等()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            helper._TableHelper.TryCreateTable(typeof(Tb_Order));
            // 再次调用不应报错
            helper._TableHelper.TryCreateTable(typeof(Tb_Order));
        }

        [Fact]
        public void DropTable_与_GetDropTable()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            helper._TableHelper.TryCreateTable(typeof(Tb_Order));
            Assert.Contains("DROP TABLE IF EXISTS [Tb_Order]", helper._TableHelper.GetDropTable(typeof(Tb_Order)));

            helper._TableHelper.DropTable(typeof(Tb_Order));
            // 表已删除，再查询应报错；验证表确实不存在
            var exists = helper.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Tb_Order'");
            Assert.Equal(0, exists);
        }

        [Fact]
        public void TruncateTable_清空数据_并重置自增()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            helper._TableHelper.TryCreateTable(typeof(Tb_Order));
            helper.Insert(new Tb_Order { UserId = 1, OrderNo = "A001", Money = 1m, CreateTime = DateTime.Now });
            helper.Insert(new Tb_Order { UserId = 2, OrderNo = "A002", Money = 2m, CreateTime = DateTime.Now });
            Assert.Equal(2, helper.Count<Tb_Order>());

            var sql = helper._TableHelper.GetTruncateTable(typeof(Tb_Order));
            Assert.Contains("DELETE FROM [Tb_Order]", sql);
            Assert.Contains("sqlite_sequence", sql);

            helper._TableHelper.TruncateTable(typeof(Tb_Order));
            Assert.Equal(0, helper.Count<Tb_Order>());
        }

        [Fact]
        public void CreateTableIndex_生成索引()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            helper._TableHelper.TryCreateTable(typeof(Tb_Order), false);
            var sql = helper._TableHelper.GetCreateTableIndex(typeof(Tb_Order));
            Assert.Contains("i_Tb_Order_UserId", sql);
            Assert.Contains("u_Tb_Order_OrderNo", sql);

            helper._TableHelper.CreateTableIndex(typeof(Tb_Order));
        }
    }
}
