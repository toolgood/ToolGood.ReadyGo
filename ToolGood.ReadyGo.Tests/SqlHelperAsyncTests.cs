using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    /// <summary>
    /// SqlHelper 异步方法单元测试（基于 SQLite）
    /// </summary>
    public class SqlHelperAsyncTests
    {
        #region Execute / ExecuteScalar / ExecuteDataTable

        [Fact]
        public async Task Execute_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);

            var affected = await helper.Execute_Async("UPDATE UserInfo SET Remark = '异步更新' WHERE Id = @0", 1);
            Assert.Equal(1, affected);
            Assert.Equal("异步更新", helper.FirstOrDefault<UserInfo>(1).Remark);
        }

        [Fact]
        public async Task ExecuteScalar_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);
            db.NewUser("乙", 30);

            Assert.Equal(2, await helper.ExecuteScalar_Async<int>("SELECT COUNT(*) FROM UserInfo"));
            Assert.Equal("乙", await helper.ExecuteScalar_Async<string>("SELECT Name FROM UserInfo WHERE Age = @0", 30));
        }

        [Fact]
        public async Task ExecuteDataTable_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);
            db.NewUser("乙", 30);

            var dt = await helper.ExecuteDataTable_Async("SELECT Id FROM UserInfo");
            Assert.Equal(2, dt.Rows.Count);
        }

        #endregion

        #region 查询

        [Fact]
        public async Task Select_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            for (int i = 0; i < 10; i++) {
                db.NewUser("用户" + i, 20 + i);
            }

            var list = await helper.Select_Async<UserInfo>("WHERE Age > 25");
            Assert.Equal(4, list.Count);
        }

        [Fact]
        public async Task Page_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            for (int i = 0; i < 13; i++) {
                db.NewUser("用户" + i, 20 + i);
            }

            var page = await helper.Page_Async<UserInfo>(1, 3, "ORDER BY Id");
            Assert.Equal(13, page.TotalItems);
            Assert.Equal(3, page.Items.Count);
        }

        [Fact]
        public async Task FirstOrDefault_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("李四", 30);

            var f1 = await helper.FirstOrDefault_Async<UserInfo>(u.Id);
            Assert.NotNull(f1);
            Assert.Equal("李四", f1.Name);

            var f2 = await helper.FirstOrDefault_Async<UserInfo>("WHERE Name = @0", "李四");
            Assert.NotNull(f2);
            Assert.Equal(30, f2.Age);

            Assert.Null(await helper.FirstOrDefault_Async<UserInfo>("WHERE Id = 999"));
        }

        [Fact]
        public async Task Count_And_Exists_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("甲", 20);
            db.NewUser("乙", 30);

            Assert.Equal(2, await helper.Count_Async<UserInfo>());
            Assert.True(await helper.Exists_Async<UserInfo>("WHERE Id = @0", u.Id));
            Assert.False(await helper.Exists_Async<UserInfo>("WHERE Id = 999"));
        }

        [Fact]
        public async Task SqlSeries_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("李四", 30);
            for (int i = 0; i < 5; i++) {
                db.NewUser("批量" + i, 40 + i);
            }

            var sq = await helper.SQL_FirstOrDefault_Async<UserInfo>("Id, Name", "UserInfo", "WHERE Id = @0", 1);
            Assert.NotNull(sq);
            Assert.Equal("李四", sq.Name);

            var page = await helper.SQL_Page_Async<UserInfo>(1, 2, "Id, Name", "UserInfo", "ORDER BY Id", "");
            Assert.Equal(6, page.TotalItems);
            Assert.Equal(2, page.Items.Count);
        }

        [Fact]
        public async Task ObjectCondition_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            var su = await helper.FirstOrDefault_Async<SimpleUser>("WHERE Name = '甲'");
            var cond = new SimpleUser { Id = su.Id, Name = su.Name, Age = su.Age };

            var users = await helper.Select_Async<SimpleUser>(cond);
            Assert.Single(users);
        }

        #endregion

        #region 增删改

        [Fact]
        public async Task Insert_Update_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            var insert = new UserInfo { Name = "异步插入", Age = 88, Money = 9m, IsDelete = false };
            await helper.Insert_Async(insert);
            Assert.True(insert.Id > 0);

            insert.Age = 89;
            Assert.Equal(1, await helper.Update_Async(insert));
            Assert.Equal(89, (await helper.FirstOrDefault_Async<UserInfo>(insert.Id)).Age);
        }

        [Fact]
        public async Task InsertList_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            var list = new List<UserInfo> {
                new UserInfo { Name = "异步批量1", Age = 1, Money = 1, IsDelete = false },
                new UserInfo { Name = "异步批量2", Age = 2, Money = 2, IsDelete = false }
            };
            await helper.InsertList_Async(list);

            Assert.Equal(2, await helper.Count_Async<UserInfo>());
        }

        [Fact]
        public async Task Delete_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var a = db.NewUser("甲", 20);
            var b = db.NewUser("乙", 30);
            var c = db.NewUser("丙", 40);

            Assert.Equal(1, await helper.DeleteById_Async<UserInfo>(a.Id));
            Assert.Equal(1, await helper.Delete_Async<UserInfo>("WHERE Id = @0", b.Id));
            Assert.Equal(1, await helper.Delete_Async(c));
            Assert.Equal(0, await helper.Count_Async<UserInfo>());
        }

        [Fact]
        public async Task Save_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            var u = new UserInfo { Name = "异步保存", Age = 5, Money = 5, IsDelete = false };
            await helper.Save_Async(u);
            Assert.True(u.Id > 0);

            u.Age = 6;
            await helper.Save_Async(u);
            Assert.Equal(6, (await helper.FirstOrDefault_Async<UserInfo>(u.Id)).Age);
        }

        [Fact]
        public async Task Update_SetCondition_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            var s = new SimpleUser { Name = "甲", Age = 10 };
            helper.Insert(s);
            var loaded = await helper.FirstOrDefault_Async<SimpleUser>(s.Id);
            var cond = new SimpleUser { Id = loaded.Id, Name = loaded.Name, Age = loaded.Age };

            Assert.Equal(1, await helper.Update_Async<SimpleUser>(new SimpleUser { Age = 100 }, cond, new[] { "Id", "Name" }));
            Assert.Equal(100, (await helper.FirstOrDefault_Async<SimpleUser>(s.Id)).Age);
        }

        #endregion
    }
}
