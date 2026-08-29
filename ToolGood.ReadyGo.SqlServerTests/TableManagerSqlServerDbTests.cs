using Xunit;

namespace ToolGood.ReadyGo.SqlServerTests
{
    /// <summary>
    /// TableManager（SqlHelper.TableHelper）单元测试（基于 SQL Server）
    /// </summary>
    [Collection("SqlServerDb")]
    public class TableManagerSqlServerDbTests
    {
        private static SqlServerTestDb CreateTable()
        {
            var db = SqlServerTestDb.Create();
            db.Helper.TableHelper.DropTable(typeof(Tb_Order));
            db.Helper.TableHelper.TryCreateTable(typeof(Tb_Order));
            return db;
        }

        [Fact]
        public void GetTryCreateTable_SqlServer_包含关键片段()
        {
            using var db = CreateTable();
            var helper = db.Helper;

            var sql = helper.TableHelper.GetTryCreateTable(typeof(Tb_Order));

            Assert.Contains("IF NOT EXISTS (SELECT 1 FROM sys.tables", sql);
            Assert.Contains("BEGIN", sql);
            Assert.Contains("CREATE TABLE [Tb_Order](", sql);
            Assert.Contains("identity(1,1)", sql);
            Assert.Contains("PRIMARY KEY", sql);
            Assert.Contains("[OrderNo] nvarchar(50)", sql);
            Assert.Contains("[Remark] Text", sql);
            Assert.Contains("CONSTRAINT u_OrderNo UNIQUE ([OrderNo])", sql);
            Assert.Contains("CREATE INDEX i_UserId ON [Tb_Order]([UserId])", sql);
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

            Assert.Contains("DROP TABLE IF EXISTS [Tb_Order];", helper.TableHelper.GetDropTable(typeof(Tb_Order)));

            helper.TableHelper.DropTable(typeof(Tb_Order));

            var exists = helper.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM sys.tables WHERE name = 'Tb_Order'");
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

            Assert.Contains("TRUNCATE TABLE [Tb_Order];", helper.TableHelper.GetTruncateTable(typeof(Tb_Order)));

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
