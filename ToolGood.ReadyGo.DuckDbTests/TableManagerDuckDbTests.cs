using Xunit;

namespace ToolGood.ReadyGo.DuckDbTests
{
    /// <summary>
    /// TableManager（SqlHelper.TableHelper）单元测试（基于 DuckDB）
    /// </summary>
    [Collection("DuckDb")]
    public class TableManagerDuckDbTests
    {
        private static DuckDbTestDb CreateTable()
        {
            var db = DuckDbTestDb.Create();
            db.Helper.TableHelper.DropTable(typeof(Tb_Order));
            db.Helper.TableHelper.TryCreateTable(typeof(Tb_Order));
            return db;
        }

        [Fact]
        public void GetTryCreateTable_DuckDB_包含关键片段()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var sql = helper.TableHelper.GetTryCreateTable(typeof(Tb_Order));

            Assert.Contains("CREATE SEQUENCE IF NOT EXISTS seq_Tb_Order START 1;", sql);
            Assert.Contains("CREATE TABLE IF NOT EXISTS \"Tb_Order\"(", sql);
            Assert.Contains("PRIMARY KEY", sql);
            Assert.Contains("DEFAULT NEXTVAL('seq_Tb_Order')", sql);
            Assert.Contains("\"OrderNo\" Text", sql);
            Assert.Contains("\"Remark\" Text", sql);
            Assert.Contains("CREATE INDEX IF NOT EXISTS i_Tb_Order_UserId", sql);
            Assert.Contains("CREATE UNIQUE INDEX IF NOT EXISTS u_Tb_Order_OrderNo", sql);
        }

        [Fact]
        public void TryCreateTable_然后_Insert_查询()
        {
            using var db = CreateTable();
            var helper = db.Helper;

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
            using var db = CreateTable();
            var helper = db.Helper;

            helper.TableHelper.TryCreateTable(typeof(Tb_Order));
            helper.TableHelper.TryCreateTable(typeof(Tb_Order));
        }

        [Fact]
        public void DropTable_与_GetDropTable()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            Assert.Contains("DROP TABLE IF EXISTS \"Tb_Order\";", helper.TableHelper.GetDropTable(typeof(Tb_Order)));

            helper.TableHelper.DropTable(typeof(Tb_Order));

            var exists = helper.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'Tb_Order'");
            Assert.Equal(0, exists);
        }

        [Fact]
        public void TruncateTable_清空数据()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            helper.Insert(new Tb_Order { UserId = 1, OrderNo = "A001", Money = 1m, CreateTime = DateTime.Now });
            helper.Insert(new Tb_Order { UserId = 2, OrderNo = "A002", Money = 2m, CreateTime = DateTime.Now });
            Assert.Equal(2, helper.Count<Tb_Order>());

            Assert.Contains("DELETE FROM \"Tb_Order\";", helper.TableHelper.GetTruncateTable(typeof(Tb_Order)));

            helper.TableHelper.TruncateTable(typeof(Tb_Order));
            Assert.Equal(0, helper.Count<Tb_Order>());
        }

        [Fact]
        public void CreateTableIndex_生成索引()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            helper.TableHelper.DropTable(typeof(Tb_Order));
            helper.TableHelper.TryCreateTable(typeof(Tb_Order), false);
            var sql = helper.TableHelper.GetCreateTableIndex(typeof(Tb_Order));
            Assert.Contains("i_Tb_Order_UserId", sql);
            Assert.Contains("u_Tb_Order_OrderNo", sql);

            helper.TableHelper.CreateTableIndex(typeof(Tb_Order));
        }
    }
}
