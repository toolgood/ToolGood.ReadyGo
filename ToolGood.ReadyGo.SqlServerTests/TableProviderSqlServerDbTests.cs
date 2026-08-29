using ToolGood.ReadyGo.Gadget.TableManager.Providers;
using Xunit;

namespace ToolGood.ReadyGo.SqlServerTests
{
    /// <summary>
    /// SqlServerDatabaseProvider 生成 SQL 的正确性验证
    /// </summary>
    public class TableProviderSqlServerDbTests
    {
        [Fact]
        public void SqlServer_自增主键_包含IDENTITY()
        {
            var sql = new SqlServerDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);

            Assert.Contains("IF NOT EXISTS (SELECT 1 FROM sys.tables", sql);
            Assert.Contains("CREATE TABLE [Tb_Provider_Test](", sql);
            Assert.Contains("[Id] int identity(1,1)", sql);
            Assert.Contains("PRIMARY KEY", sql);
        }

        [Fact]
        public void SqlServer_char映射为NCHAR()
        {
            var sql = new SqlServerDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);
            Assert.Contains("[Code] nchar(1)", sql);
        }

        [Fact]
        public void SqlServer_非自增主键_不含IDENTITY()
        {
            var sql = new SqlServerDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_NoAutoInc), false);
            Assert.Contains("[Id] int NOT NULL PRIMARY KEY", sql);
            Assert.DoesNotContain("identity", sql);
        }

        [Fact]
        public void SqlServer_Truncate()
        {
            Assert.Equal("TRUNCATE TABLE [Tb_Provider_Test];",
                new SqlServerDatabaseProvider().GetTruncateTable(typeof(Tb_Provider_AutoInc)));

            Assert.Equal("TRUNCATE TABLE [Tb_Any];",
                new SqlServerDatabaseProvider().GetTruncateTable("Tb_Any"));
        }

        [Fact]
        public void SqlServer_DropTable()
        {
            Assert.Equal("DROP TABLE IF EXISTS [Tb_Provider_Test];",
                new SqlServerDatabaseProvider().GetDropTable(typeof(Tb_Provider_AutoInc)));

            Assert.Equal("DROP TABLE IF EXISTS [Tb_Any];",
                new SqlServerDatabaseProvider().GetDropTable("Tb_Any"));
        }

        [Fact]
        public void SqlServer_GetCreateIndex()
        {
            var sql = new SqlServerDatabaseProvider().GetCreateIndex(typeof(Tb_Order));
            Assert.Contains("CREATE INDEX i_UserId ON [Tb_Order]([UserId]);", sql);
            Assert.Contains("CREATE UNIQUE INDEX u_OrderNo ON [Tb_Order]([OrderNo]);", sql);
        }
    }
}
