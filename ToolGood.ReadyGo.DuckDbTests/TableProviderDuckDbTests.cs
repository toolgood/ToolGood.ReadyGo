using ToolGood.ReadyGo.Gadget.TableManager.Providers;
using Xunit;

namespace ToolGood.ReadyGo.DuckDbTests
{
    /// <summary>
    /// DuckDbDatabaseProvider 生成 SQL 的正确性验证
    /// </summary>
    public class TableProviderDuckDbTests
    {
        [Fact]
        public void DuckDb_自增主键_包含SEQUENCE与DEFAULT_NEXTVAL()
        {
            var sql = new DuckDbDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);

            Assert.Contains("CREATE SEQUENCE IF NOT EXISTS seq_Tb_Provider_Test START 1;", sql);
            Assert.Contains("CREATE TABLE IF NOT EXISTS \"Tb_Provider_Test\"", sql);
            Assert.Contains("\"Id\" INTEGER DEFAULT NEXTVAL('seq_Tb_Provider_Test') NOT NULL PRIMARY KEY", sql);
        }

        [Fact]
        public void DuckDb_char映射为char()
        {
            var sql = new DuckDbDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);
            Assert.Contains("\"Code\" char(1)", sql);
        }

        [Fact]
        public void DuckDb_非自增主键_不含NEXTVAL()
        {
            var sql = new DuckDbDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_NoAutoInc), false);
            Assert.Contains("PRIMARY KEY", sql);
            Assert.DoesNotContain("NEXTVAL", sql);
        }

        [Fact]
        public void DuckDb_Truncate()
        {
            Assert.Equal("DELETE FROM \"Tb_Provider_Test\";",
                new DuckDbDatabaseProvider().GetTruncateTable(typeof(Tb_Provider_AutoInc)));

            Assert.Equal("DELETE FROM \"Tb_Any\";",
                new DuckDbDatabaseProvider().GetTruncateTable("Tb_Any"));
        }

        [Fact]
        public void DuckDb_DropTable()
        {
            Assert.Equal("DROP TABLE IF EXISTS \"Tb_Provider_Test\";\r\nDROP SEQUENCE IF EXISTS seq_Tb_Provider_Test;",
                new DuckDbDatabaseProvider().GetDropTable(typeof(Tb_Provider_AutoInc)));

            Assert.Equal("DROP TABLE IF EXISTS \"Tb_Any\";\r\nDROP SEQUENCE IF EXISTS seq_Tb_Any;",
                new DuckDbDatabaseProvider().GetDropTable("Tb_Any"));
        }
    }
}
