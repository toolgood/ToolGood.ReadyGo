using System.Linq;
using System.Text.RegularExpressions;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.Gadget.TableManager.Providers;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_Provider_Test")]
    [PrimaryKey("Id")]
    public class Tb_Provider_AutoInc
    {
        [DefaultValue("0")]
        public int Id { get; set; }
        public string Name { get; set; }
        public char? Code { get; set; }
    }

    [Table("Tb_Provider_Test_NoAuto")]
    [PrimaryKey("Id", AutoIncrement = false)]
    public class Tb_Provider_NoAutoInc
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// Gadget TableManager Provider 生成 SQL 的正确性验证
    /// </summary>
    public class TableProviderTests
    {
        [Fact]
        public void DuckDb_自增主键_DEFAULT在前且唯一()
        {
            var sql = new DuckDbDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);
            var pkLine = sql.Split('\n').First(l => l.Contains("\"Id\""));
            // DEFAULT 子句位于 PRIMARY KEY 之前，且自增优先于 DefaultValue（不会出现两个 DEFAULT）
            Assert.Contains("DEFAULT NEXTVAL('seq_Tb_Provider_Test') NOT NULL PRIMARY KEY", pkLine);
            Assert.Single(Regex.Matches(pkLine, "DEFAULT"));
        }

        [Fact]
        public void SqlServer_char映射为nchar()
        {
            var sql = new SqlServerDatabaseProvider().GetTryCreateTable(typeof(Tb_Provider_AutoInc), false);
            Assert.Contains("[Code] nchar(1)", sql);
        }

        [Fact]
        public void Sqlite_Truncate_自增表_含序列重置()
        {
            var sql = new SQLiteDatabaseProvider().GetTruncateTable(typeof(Tb_Provider_AutoInc));
            Assert.Contains("DELETE FROM [Tb_Provider_Test];", sql);
            Assert.Contains("DELETE FROM sqlite_sequence WHERE name='Tb_Provider_Test';", sql);
        }

        [Fact]
        public void Sqlite_Truncate_非自增表_不含序列重置()
        {
            var sql = new SQLiteDatabaseProvider().GetTruncateTable(typeof(Tb_Provider_NoAutoInc));
            Assert.Contains("DELETE FROM [Tb_Provider_Test_NoAuto];", sql);
            Assert.DoesNotContain("sqlite_sequence", sql);
        }

        [Fact]
        public void Sqlite_Truncate_String版本_表名转义且不含序列重置()
        {
            var sql = new SQLiteDatabaseProvider().GetTruncateTable("Tb_Any");
            Assert.Equal("DELETE FROM [Tb_Any];", sql);
        }

        [Fact]
        public void Sqlite_Truncate_非自增表_实际执行成功()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_Provider_NoAutoInc));
            helper.Execute("INSERT INTO [Tb_Provider_Test_NoAuto]([Id]) VALUES(1);");

            helper._TableHelper.TruncateTable(typeof(Tb_Provider_NoAutoInc)); // 不应抛错

            Assert.Equal(0, helper.ExecuteScalar<int>("SELECT COUNT(*) FROM [Tb_Provider_Test_NoAuto]"));
        }

        [Fact]
        public void Sqlite_Truncate_自增表_重置自增计数()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_Provider_AutoInc));

            var a = new Tb_Provider_AutoInc { Name = "a" };
            helper.Insert(a);
            var b = new Tb_Provider_AutoInc { Name = "b" };
            helper.Insert(b);
            Assert.Equal(2, b.Id);

            helper._TableHelper.TruncateTable(typeof(Tb_Provider_AutoInc));

            var c = new Tb_Provider_AutoInc { Name = "c" };
            helper.Insert(c);
            Assert.Equal(1, c.Id); // 自增计数已重置
        }
    }
}
