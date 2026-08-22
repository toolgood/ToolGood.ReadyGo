using System.Threading.Tasks;
using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Users")]
    [PrimaryKey("UserId")]
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }
    }

    /// <summary>
    /// 复杂对象（嵌套对象）映射测试：SQL 中平铺返回子对象列，自动填充 Address 属性
    /// </summary>
    public class ComplexMappingTests
    {
        private static TestDb CreateWithUsers()
        {
            var db = TestDb.Create();
            var helper = db.Helper;
            helper.Execute("CREATE TABLE Users (UserId INTEGER PRIMARY KEY, Name TEXT, Street TEXT, City TEXT)");
            helper.Execute("INSERT INTO Users VALUES (1, '张三', '中山路1号', '上海')");
            helper.Execute("INSERT INTO Users VALUES (2, '李四', '解放路2号', '北京')");
            return db;
        }

        [Fact]
        public void Fetch_平铺列名_自动填充嵌套对象()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            // 用户示例：select u.UserId, u.Name, u.Street, u.City from Users
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

            // NPoco 约定的嵌套列命名：属性名 + "__" + 子属性名
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
