using ToolGood.ReadyGo.Gadget.TableManager.Providers;
using Xunit;

namespace ToolGood.ReadyGo.MsAccessDbTests
{
    /// <summary>
    /// MsAccessDbDatabaseProvider 生成 SQL 的正确性验证
    /// </summary>
    public class TableProviderMsAccessDbTests
    {
        [Fact]
        public void Access_自增主键_包含AUTOINCREMENT()
        {
            var sql = new MsAccessDbDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);

            Assert.Contains("CREATE TABLE [Tb_Provider_Test](", sql);
            Assert.Contains("[Id] AUTOINCREMENT PRIMARY KEY", sql);
        }

        [Fact]
        public void Access_char映射为TEXT()
        {
            var sql = new MsAccessDbDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);
            Assert.Contains("[Code] TEXT(1)", sql);
        }

        [Fact]
        public void Access_非自增主键_不含AUTOINCREMENT()
        {
            var sql = new MsAccessDbDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_NoAutoInc), false);
            Assert.Contains("[Id] INTEGER NOT NULL PRIMARY KEY", sql);
            Assert.DoesNotContain("AUTOINCREMENT", sql);
        }

        [Fact]
        public void Access_Truncate()
        {
            Assert.Equal("DELETE FROM [Tb_Provider_Test];\r\nALTER TABLE [Tb_Provider_Test] ALTER COLUMN [Id] COUNTER(1,1);",
                new MsAccessDbDatabaseProvider().GetTruncateTable(typeof(Tb_Provider_AutoInc)));

            Assert.Equal("DELETE FROM [Tb_Any];",
                new MsAccessDbDatabaseProvider().GetTruncateTable("Tb_Any"));
        }

        [Fact]
        public void Access_DropTable()
        {
            Assert.Equal("DROP TABLE [Tb_Provider_Test];",
                new MsAccessDbDatabaseProvider().GetDropTable(typeof(Tb_Provider_AutoInc)));

            Assert.Equal("DROP TABLE [Tb_Any];",
                new MsAccessDbDatabaseProvider().GetDropTable("Tb_Any"));
        }
    }
}
