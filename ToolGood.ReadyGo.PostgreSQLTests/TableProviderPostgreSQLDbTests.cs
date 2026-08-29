using ToolGood.ReadyGo.Gadget.TableManager.Providers;
using Xunit;

namespace ToolGood.ReadyGo.PostgreSQLTests
{
    /// <summary>
    /// PostgreSQLDatabaseProvider 生成 SQL 的正确性验证
    /// </summary>
    public class TableProviderPostgreSQLDbTests
    {
        [Fact]
        public void PostgreSQL_自增主键_包含SEQUENCE与DEFAULT_NEXTVAL()
        {
            var sql = new PostgreSQLDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);

            Assert.Contains("CREATE SEQUENCE IF NOT EXISTS seq_Tb_Provider_Test START 1;", sql);
            Assert.Contains("CREATE TABLE IF NOT EXISTS \"Tb_Provider_Test\"(", sql);
            Assert.Contains("\"Id\" integer DEFAULT NEXTVAL('seq_Tb_Provider_Test') NOT NULL PRIMARY KEY", sql);
        }

        [Fact]
        public void PostgreSQL_char映射为char()
        {
            var sql = new PostgreSQLDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);
            Assert.Contains("\"Code\" char(1)", sql);
        }

        [Fact]
        public void PostgreSQL_非自增主键_不含NEXTVAL()
        {
            var sql = new PostgreSQLDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_NoAutoInc), false);
            Assert.Contains("\"Id\" integer NOT NULL PRIMARY KEY", sql);
            Assert.DoesNotContain("NEXTVAL", sql);
        }

        [Fact]
        public void PostgreSQL_Truncate()
        {
            Assert.Equal("TRUNCATE TABLE \"Tb_Provider_Test\";",
                new PostgreSQLDatabaseProvider().GetTruncateTable(typeof(Tb_Provider_AutoInc)));

            Assert.Equal("TRUNCATE TABLE \"Tb_Any\";",
                new PostgreSQLDatabaseProvider().GetTruncateTable("Tb_Any"));
        }

        [Fact]
        public void PostgreSQL_DropTable()
        {
            Assert.Equal("DROP TABLE IF EXISTS \"Tb_Provider_Test\";\r\nDROP SEQUENCE IF EXISTS seq_Tb_Provider_Test;",
                new PostgreSQLDatabaseProvider().GetDropTable(typeof(Tb_Provider_AutoInc)));

            Assert.Equal("DROP TABLE IF EXISTS \"Tb_Any\";\r\nDROP SEQUENCE IF EXISTS seq_Tb_Any;",
                new PostgreSQLDatabaseProvider().GetDropTable("Tb_Any"));
        }
    }
}
