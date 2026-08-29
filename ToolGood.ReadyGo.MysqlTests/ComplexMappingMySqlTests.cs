using Xunit;

namespace ToolGood.ReadyGo.MysqlTests
{
    /// <summary>
    /// 复杂对象（嵌套对象）映射测试（基于 MySQL）
    /// </summary>
    [Collection("MySql")]
    public class ComplexMappingMySqlTests
    {
        private static MySqlTestDb CreateWithUsers()
        {
            var db = MySqlTestDb.Create();
            var helper = db.Helper;
            helper.Execute("DROP TABLE IF EXISTS Users;");
            helper.Execute("CREATE TABLE Users (UserId INT PRIMARY KEY AUTO_INCREMENT, Name VARCHAR(255), Street VARCHAR(255), City VARCHAR(255));");
            helper.Execute("INSERT INTO Users (UserId, Name, Street, City) VALUES (1, '张三', '中山路1号', '上海'), (2, '李四', '解放路2号', '北京');");
            return db;
        }

        [Fact]
        public void Fetch_平铺列名_自动填充嵌套对象()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var users = helper.Select<User>("select u.UserId, u.Name, u.Street, u.City from Users u order by u.UserId");

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
                "select u.UserId, u.Name, u.Street as Address__Street, u.City as Address__City from Users u order by u.UserId");

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

            var users = await helper.Select_Async<User>("select u.UserId, u.Name, u.Street, u.City from Users u order by u.UserId");

            Assert.Equal(2, users.Count);
            Assert.NotNull(users[0].Address);
            Assert.Equal("中山路1号", users[0].Address.Street);
            Assert.Equal("上海", users[0].Address.City);
        }
    }
}
