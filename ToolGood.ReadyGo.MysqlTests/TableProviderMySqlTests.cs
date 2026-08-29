using ToolGood.ReadyGo.Gadget.TableManager.Providers;
using Xunit;

namespace ToolGood.ReadyGo.MysqlTests
{
    /// <summary>
    /// MySqlDatabaseProvider 生成 SQL 的正确性验证
    /// </summary>
    public class TableProviderMySqlTests
    {
        [Fact]
        public void MySql_自增主键_包含PRIMARY_KEY_AUTO_INCREMENT()
        {
            var sql = new MySqlDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);

            Assert.Contains("CREATE TABLE IF NOT EXISTS `Tb_Provider_Test`", sql);
            Assert.Contains("`Id` int", sql);
            Assert.Contains("PRIMARY KEY", sql);
            Assert.Contains("AUTO_INCREMENT", sql);
        }

        [Fact]
        public void MySql_char映射为char()
        {
            var sql = new MySqlDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);
            Assert.Contains("`Code` char(1)", sql);
        }

        [Fact]
        public void MySql_非自增主键_不含AUTO_INCREMENT()
        {
            var sql = new MySqlDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_NoAutoInc), false);
            Assert.Contains("PRIMARY KEY", sql);
            Assert.DoesNotContain("AUTO_INCREMENT", sql);
        }

        [Fact]
        public void MySql_Truncate()
        {
            Assert.Equal("TRUNCATE TABLE `Tb_Provider_Test`;",
                new MySqlDatabaseProvider().GetTruncateTable(typeof(Tb_Provider_AutoInc)));

            Assert.Equal("TRUNCATE TABLE `Tb_Any`;",
                new MySqlDatabaseProvider().GetTruncateTable("Tb_Any"));
        }

        [Fact]
        public void MySql_DropTable()
        {
            Assert.Equal("DROP TABLE IF EXISTS `Tb_Provider_Test`;",
                new MySqlDatabaseProvider().GetDropTable(typeof(Tb_Provider_AutoInc)));

            Assert.Equal("DROP TABLE IF EXISTS `Tb_Any`;",
                new MySqlDatabaseProvider().GetDropTable("Tb_Any"));
        }
    }
}
