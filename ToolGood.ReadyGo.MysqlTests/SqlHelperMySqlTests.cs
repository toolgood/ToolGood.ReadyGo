using Xunit;

namespace ToolGood.ReadyGo.MysqlTests
{
    /// <summary>
    /// SqlHelper 同步方法单元测试（基于 MySQL）
    /// </summary>
    [Collection("MySql")]
    public class SqlHelperMySqlTests
    {
        #region Insert

        [Fact]
        public void Insert_ReturnsAutoIncrementId()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;

            var u = new UserInfo { Name = "张三", Age = 20, Remark = "测试1", Money = 100.5m, IsDelete = false };
            var id = Convert.ToInt64(helper.Insert(u));

            Assert.True(id > 0);
            Assert.True(u.Id > 0);
            Assert.Equal(id, u.Id);
        }

        [Fact]
        public void Insert_DateTimeDefaultNow_Works()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;

            var u = new UserInfo { Name = "张三", Age = 20, Money = 1m, IsDelete = false };
            helper.Insert(u);

            Assert.NotEqual(default(DateTime), u.CreateTime);
            var loaded = helper.FirstOrDefault<UserInfo>(u.Id);
            Assert.NotNull(loaded);
            Assert.Equal(u.CreateTime.Date, loaded.CreateTime.Date);
        }

        [Fact]
        public void InsertList_InsertsAll()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;

            var list = new List<UserInfo>();
            for (int i = 0; i < 10; i++) {
                list.Add(new UserInfo { Name = "批量" + i, Age = 50 + i, Money = i, IsDelete = false });
            }
            helper.InsertList(list);

            Assert.Equal(10, helper.Count<UserInfo>());
        }

        #endregion

        #region Count / Exists

        [Fact]
        public void Count_And_Exists()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;
            db.NewUser("张三", 20);
            db.NewUser("李四", 30);
            db.NewUser("王五", 40);

            Assert.Equal(3, helper.Count<UserInfo>());
            Assert.Equal(2, helper.Count<UserInfo>("WHERE Age > 20"));

            Assert.True(helper.Exists<UserInfo>("WHERE Id = @0", 1));
            Assert.False(helper.Exists<UserInfo>("WHERE Id = 999"));
        }

        #endregion

        #region Select / Page

        [Fact]
        public void Select_Page_And_FirstOrDefault()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;
            for (int i = 0; i < 13; i++) {
                db.NewUser("用户" + i, 20 + i);
            }

            Assert.Equal(13, helper.Select<UserInfo>().Count);
            Assert.Equal(5, helper.Select<UserInfo>(5).Count);

            var sp = helper.SelectPage<UserInfo>(1, 5, "ORDER BY Id");
            Assert.Equal(5, sp.Count);

            var page = helper.Page<UserInfo>(2, 5, "ORDER BY Id");
            Assert.Equal(2, page.CurrentPage);
            Assert.Equal(13, page.TotalItems);
            Assert.Equal(5, page.Items.Count);
            Assert.Equal("用户5", page.Items[0].Name);

            var f = helper.FirstOrDefault<UserInfo>("WHERE Name = @0", "用户3");
            Assert.NotNull(f);
            Assert.Equal(23, f.Age);
            Assert.Null(helper.FirstOrDefault<UserInfo>("WHERE Id = 999"));
        }

        #endregion

        #region Update / Save / Delete

        [Fact]
        public void Update_Save_Delete()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("李四", 30);

            u.Age = 31;
            Assert.Equal(1, helper.Update(u));
            Assert.Equal(31, helper.FirstOrDefault<UserInfo>(u.Id).Age);

            Assert.Equal(1, helper.Update<UserInfo>("SET Remark = 'sql更新' WHERE Id = @0", u.Id));
            Assert.Equal("sql更新", helper.FirstOrDefault<UserInfo>(u.Id).Remark);

            var saved = new UserInfo { Name = "保存新", Age = 66, Money = 1m, IsDelete = false };
            helper.Save(saved);
            Assert.True(saved.Id > 0);

            saved.Age = 67;
            helper.Save(saved);
            Assert.Equal(67, helper.FirstOrDefault<UserInfo>(saved.Id).Age);

            Assert.Equal(1, helper.Delete(saved));
            Assert.Equal(1, helper.DeleteById<UserInfo>(u.Id));
            Assert.Equal(0, helper.Count<UserInfo>());
        }

        #endregion

        #region Execute

        [Fact]
        public void ExecuteScalar_And_DataTable()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);
            db.NewUser("乙", 30);

            Assert.Equal(2, helper.ExecuteScalar<int>("SELECT COUNT(*) FROM UserInfo"));
            Assert.Equal("乙", helper.ExecuteScalar<string>("SELECT Name FROM UserInfo WHERE Age = @0", 30));

            var dt = helper.ExecuteDataTable("SELECT Id, Name FROM UserInfo");
            Assert.Equal(2, dt.Rows.Count);
            Assert.Equal("甲", dt.Rows[0]["Name"]);
        }

        #endregion

        #region 事务

        [Fact]
        public void Transaction_Commit()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;

            using (var trans = helper.UseTransaction()) {
                helper.Execute("INSERT INTO UserInfo (Name, Age, Remark, CreateTime, Money, IsDelete) VALUES ('事务1', 1, NULL, '2026-01-01 00:00:00', 1, 0)");
                trans.Complete();
            }
            Assert.True(helper.Exists<UserInfo>("WHERE Name = '事务1'"));
        }

        [Fact]
        public void Transaction_Rollback()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;

            using (var trans = helper.UseTransaction()) {
                helper.Execute("INSERT INTO UserInfo (Name, Age, Remark, CreateTime, Money, IsDelete) VALUES ('事务2', 1, NULL, '2026-01-01 00:00:00', 1, 0)");
                // 不调用 Complete，释放时应回滚
            }
            Assert.False(helper.Exists<UserInfo>("WHERE Name = '事务2'"));
        }

        #endregion
    }

    /// <summary>
    /// SqlHelper 异步方法单元测试（基于 MySQL）
    /// </summary>
    [Collection("MySql")]
    public class SqlHelperMySqlAsyncTests
    {
        [Fact]
        public async Task Insert_Update_Async()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;

            var insert = new UserInfo { Name = "异步插入", Age = 88, Money = 9m, IsDelete = false };
            await helper.Insert_Async(insert);
            Assert.True(insert.Id > 0);

            insert.Age = 89;
            Assert.Equal(1, await helper.Update_Async(insert));
            Assert.Equal(89, (await helper.FirstOrDefault_Async<UserInfo>(insert.Id)).Age);
        }

        [Fact]
        public async Task InsertList_Delete_Async()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;

            var list = new List<UserInfo> {
                new UserInfo { Name = "异步批量1", Age = 1, Money = 1, IsDelete = false },
                new UserInfo { Name = "异步批量2", Age = 2, Money = 2, IsDelete = false }
            };
            await helper.InsertList_Async(list);
            Assert.Equal(2, await helper.Count_Async<UserInfo>());

            // InsertList 批量插入不返回主键，需查询获取实际 Id
            var ids = (await helper.Select_Async<UserInfo>("ORDER BY Id")).Select(x => x.Id).ToList();
            Assert.Equal(2, ids.Count);

            Assert.Equal(1, await helper.DeleteById_Async<UserInfo>(ids[0]));
            Assert.Equal(1, await helper.Delete_Async<UserInfo>("WHERE Id = @0", ids[1]));
            Assert.Equal(0, await helper.Count_Async<UserInfo>());
        }

        [Fact]
        public async Task Select_Page_FirstOrDefault_Async()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;
            for (int i = 0; i < 13; i++) {
                db.NewUser("用户" + i, 20 + i);
            }

            var list = await helper.Select_Async<UserInfo>("WHERE Age > 25");
            Assert.Equal(7, list.Count);

            var page = await helper.Page_Async<UserInfo>(2, 5, "ORDER BY Id");
            Assert.Equal(13, page.TotalItems);
            Assert.Equal(5, page.Items.Count);
            Assert.Equal("用户5", page.Items[0].Name);

            var f = await helper.FirstOrDefault_Async<UserInfo>("WHERE Name = @0", "用户2");
            Assert.NotNull(f);
            Assert.Equal(22, f.Age);
        }

        [Fact]
        public async Task Count_Exists_Async()
        {
            using var db = MySqlTestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);
            db.NewUser("乙", 30);

            Assert.Equal(2, await helper.Count_Async<UserInfo>());
            Assert.True(await helper.Exists_Async<UserInfo>("WHERE Id = @0", 1));
            Assert.False(await helper.Exists_Async<UserInfo>("WHERE Id = 999"));
        }
    }
}
