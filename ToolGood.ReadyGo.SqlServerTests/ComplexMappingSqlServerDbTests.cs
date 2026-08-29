using Xunit;

namespace ToolGood.ReadyGo.SqlServerTests
{
    /// <summary>
    /// 复杂对象（嵌套对象）映射测试（基于 SQL Server）
    /// </summary>
    [Collection("SqlServerDb")]
    public class ComplexMappingSqlServerDbTests
    {
        private static SqlServerTestDb CreateWithUsers()
        {
            var db = SqlServerTestDb.Create();
            var helper = db.Helper;
            helper.Execute("DROP TABLE IF EXISTS [dbo].[USERS]");
            helper.Execute("CREATE TABLE [dbo].[USERS] ([USERID] INT NOT NULL PRIMARY KEY, [NAME] NVARCHAR(255), [STREET] NVARCHAR(255), [CITY] NVARCHAR(255))");
            helper.Execute("INSERT INTO [dbo].[USERS] ([USERID], [NAME], [STREET], [CITY]) VALUES (1, '张三', '中山路1号', '上海')");
            helper.Execute("INSERT INTO [dbo].[USERS] ([USERID], [NAME], [STREET], [CITY]) VALUES (2, '李四', '解放路2号', '北京')");
            return db;
        }

        [Fact]
        public void Fetch_平铺列名_自动填充嵌套对象()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var users = helper.Select<User>("select u.[USERID], u.[NAME], u.[STREET], u.[CITY] from [dbo].[USERS] u order by u.[USERID]");

            Assert.Equal(2, users.Count);
            Assert.NotNull(users[0].Address);
            Assert.Equal("中山路1号", users[0].Address.Street);
            Assert.Equal("上海", users[0].Address.City);
            Assert.NotNull(users[1].Address);
            Assert.Equal("解放路2号", users[1].Address.Street);
            Assert.Equal("北京", users[1].Address.City);
        }

        [Fact]
        public void Fetch_双下划线前缀_填充嵌套对象()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var users = helper.Select<User>(
                "select u.[USERID], u.[NAME], u.[STREET] as [Address__Street], u.[CITY] as [Address__City] from [dbo].[USERS] u order by u.[USERID]");

            Assert.Equal(2, users.Count);
            Assert.NotNull(users[0].Address);
            Assert.Equal("中山路1号", users[0].Address.Street);
            Assert.Equal("上海", users[0].Address.City);
        }

        [Fact]
        public async Task Fetch_Async_平铺列名_填充嵌套对象()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var users = await helper.Select_Async<User>("select u.[USERID], u.[NAME], u.[STREET], u.[CITY] from [dbo].[USERS] u order by u.[USERID]");

            Assert.Equal(2, users.Count);
            Assert.NotNull(users[0].Address);
            Assert.Equal("中山路1号", users[0].Address.Street);
            Assert.Equal("上海", users[0].Address.City);
        }
    }
}
