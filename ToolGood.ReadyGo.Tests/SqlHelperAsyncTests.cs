using ToolGood.ReadyGo.NPoco;
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

        [Fact]
        public async Task ExecuteDataSet_Async_MultiResultSet()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);
            db.NewUser("乙", 30);

            var ds = await helper.ExecuteDataSet_Async("SELECT Id FROM UserInfo; SELECT Name FROM UserInfo");
            Assert.Equal(2, ds.Tables.Count);
            Assert.Equal(2, ds.Tables[0].Rows.Count);
            Assert.Equal(2, ds.Tables[1].Rows.Count);
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
        public async Task SQL_Select_Async_AllOverloads()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            for (int i = 0; i < 10; i++) {
                db.NewUser("用户" + i, 20 + i);
            }

            // 无分页重载
            var all = await helper.SQL_Select_Async<UserInfo>("Id, Name", "UserInfo", "ORDER BY Id", "");
            Assert.Equal(10, all.Count);
            Assert.Equal("用户0", all[0].Name);

            // limit 重载：取前 3 条
            var limit = await helper.SQL_Select_Async<UserInfo>(3, "Id, Name", "UserInfo", "ORDER BY Id", "");
            Assert.Equal(3, limit.Count);
            Assert.Equal("用户2", limit[2].Name);

            // page 重载：第 2 页，每页 4 条
            var pageList = await helper.SQL_Select_Async<UserInfo>(2, 4, "Id, Name", "UserInfo", "ORDER BY Id", "");
            Assert.Equal(4, pageList.Count);
            Assert.Equal("用户4", pageList[0].Name);
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

        [Fact]
        public async Task StartSnapshot_Update_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("Ted", 21);
            var user = await helper.FirstOrDefault_Async<UserInfo>(u.Id);

            var snapshot = helper.StartSnapshot(user);
            user.Name = "Bobby";
            user.Age = 21; // 与快照一致，不算变更

            Assert.Equal(1, await helper.Update_Async(user, snapshot.UpdatedColumns()));
            Assert.Equal(1, await helper.Update_Async(user, snapshot)); // 两种方式均可

            var loaded = await helper.FirstOrDefault_Async<UserInfo>(u.Id);
            Assert.Equal("Bobby", loaded.Name);
            Assert.Equal(21, loaded.Age);
        }

        [Fact]
        public async Task FetchMultiple_Async_ReturnsTwoResultSets()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("张三", 20);
            db.NewUser("李四", 30);
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });

            var data = await helper.SelectMultiple_Async<UserInfo, SimpleUser>("SELECT * FROM UserInfo;SELECT * FROM SimpleUser;");

            Assert.Equal(2, data.Item1.Count);
            Assert.Equal(2, data.Item2.Count);
            Assert.Equal("张三", data.Item1[0].Name);
            Assert.Equal("甲", data.Item2[0].Name);
        }

        [Fact]
        public async Task FetchMultiple_Async_WithCallback()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("张三", 20);
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            var tuple = await helper.SelectMultiple_Async<UserInfo, SimpleUser, Tuple<List<UserInfo>, List<SimpleUser>>>(
                (u, s) => Tuple.Create(u, s),
                "SELECT * FROM UserInfo;SELECT * FROM SimpleUser;");

            Assert.Single(tuple.Item1);
            Assert.Single(tuple.Item2);
            Assert.Equal("张三", tuple.Item1[0].Name);
        }

        #endregion

        #region 补充覆盖

        [Fact]
        public async Task Select_Count_And_RawCount_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);
            db.NewUser("乙", 30);
            db.NewUser("丙", 40);

            Assert.Equal(3, await helper.Select_Count_Async<UserInfo>());
            Assert.Equal(2, await helper.Select_Count_Async<UserInfo>("WHERE Age > 20"));
            Assert.Equal(3, await helper.Count_Async<UserInfo>());
            Assert.Equal(2, await helper.Count_Async<UserInfo>("WHERE Age > 20"));
            Assert.Equal(3, await helper.Count_Async("SELECT COUNT(*) FROM UserInfo"));
            Assert.Equal(1, await helper.Count_Async("SELECT COUNT(*) FROM UserInfo WHERE Name = @0", "甲"));
        }

        [Fact]
        public async Task Exists_ObjectCondition_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var s = new SimpleUser { Name = "甲", Age = 10 };
            helper.Insert(s);
            var loaded = await helper.FirstOrDefault_Async<SimpleUser>(s.Id);

            var cond = new SimpleUser { Id = loaded.Id, Name = loaded.Name, Age = loaded.Age };
            Assert.True(await helper.Exists_Async<SimpleUser>(cond));
            Assert.True(await helper.Exists_Async<SimpleUser>(loaded.Id));
            Assert.False(await helper.Exists_Async<SimpleUser>(999));
        }

        [Fact]
        public async Task FirstOrDefault_NumericPkOverloads_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("甲", 20);

            Assert.Equal("甲", (await helper.FirstOrDefault_Async<UserInfo>((uint)u.Id)).Name);
            Assert.Equal("甲", (await helper.FirstOrDefault_Async<UserInfo>((ulong)u.Id)).Name);
            Assert.Null(await helper.FirstOrDefault_Async<UserInfo>((uint)9999));
        }

        [Fact]
        public async Task Select_ObjectCondition_LimitAndPage_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var s = new SimpleUser { Name = "甲", Age = 10 };
            helper.Insert(s);
            var loaded = await helper.FirstOrDefault_Async<SimpleUser>(s.Id);
            var cond = new SimpleUser { Id = loaded.Id, Name = loaded.Name, Age = loaded.Age };

            Assert.Single(await helper.Select_Async<SimpleUser>(5, cond));
            Assert.Single(await helper.Select_Async<SimpleUser>(5, 0, cond));
            Assert.Single(await helper.SelectPage_Async<SimpleUser>(1, 3, cond));
        }

        [Fact]
        public async Task Update_Async_RawSql()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("甲", 20);

            Assert.Equal(1, await helper.Update_Async<UserInfo>("SET Remark = '原生sql异步' WHERE Id = @0", u.Id));
            Assert.Equal("原生sql异步", (await helper.FirstOrDefault_Async<UserInfo>(u.Id)).Remark);
        }

        [Fact]
        public async Task FetchMultiple_Async_FourResultSets()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("张三", 20);
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            var (users, simpleUsers, users2, simpleUsers2) = await helper.SelectMultiple_Async<UserInfo, SimpleUser, UserInfo, SimpleUser>(
                "SELECT * FROM UserInfo;SELECT * FROM SimpleUser;SELECT * FROM UserInfo;SELECT * FROM SimpleUser;");

            Assert.Single(users);
            Assert.Single(simpleUsers);
            Assert.Single(users2);
            Assert.Single(simpleUsers2);
        }

        [Fact]
        public async Task FetchMultiple_Async_ThreeFourTupleCallback()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("张三", 20);
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            var r3 = await helper.SelectMultiple_Async<UserInfo, SimpleUser, UserInfo, int>(
                (u, s, u2) => u.Count + s.Count + u2.Count,
                "SELECT * FROM UserInfo;SELECT * FROM SimpleUser;SELECT * FROM UserInfo;");
            Assert.Equal(3, r3);

            var r4 = await helper.SelectMultiple_Async<UserInfo, SimpleUser, UserInfo, SimpleUser, int>(
                (u, s, u2, s2) => u.Count + s.Count + u2.Count + s2.Count,
                "SELECT * FROM UserInfo;SELECT * FROM SimpleUser;SELECT * FROM UserInfo;SELECT * FROM SimpleUser;");
            Assert.Equal(4, r4);
        }

        #endregion

        #region UpdateList

        [Fact]
        public async Task UpdateList_Async_EmptyList_ReturnsZero()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            Assert.Equal(0, await helper.UpdateList_Async(new List<UserInfo>()));
            Assert.Equal(0, await helper.UpdateList_Async(new List<UserInfo>(), new List<Snapshot<UserInfo>>()));
        }

        [Fact]
        public async Task UpdateList_Async_MismatchedSnapshots_Throws()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("Ted", 21);
            var user = await helper.FirstOrDefault_Async<UserInfo>(u.Id);
            var snapshot = helper.StartSnapshot(user);

            var list = new List<UserInfo> { user, user };
            var snapshots = new List<Snapshot<UserInfo>> { snapshot };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => helper.UpdateList_Async(list, snapshots));
            Assert.Equal("list.Count must equal snapshots.Count.", ex.Message);
        }

        [Fact]
        public async Task UpdateList_Async_WithSnapshots_OnlyUpdatesChangedColumns()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u1 = db.NewUser("Ted", 21);
            var u2 = db.NewUser("Bobby", 30);

            var users = await helper.Select_Async<UserInfo>("ORDER BY Id");
            var snapshots = users.Select(x => helper.StartSnapshot(x)).ToList();

            users[0].Name = "Ted改"; // 仅 Name 变更
            users[1].Age = 31;       // 仅 Age 变更

            Assert.Equal(2, await helper.UpdateList_Async(users, snapshots));

            var loaded1 = await helper.FirstOrDefault_Async<UserInfo>(u1.Id);
            var loaded2 = await helper.FirstOrDefault_Async<UserInfo>(u2.Id);
            Assert.Equal("Ted改", loaded1.Name);
            Assert.Equal(21, loaded1.Age);      // 未变更列保持原值
            Assert.Equal("Bobby", loaded2.Name); // 未变更列保持原值
            Assert.Equal(31, loaded2.Age);
        }

        #endregion

        #region SaveList

        [Fact]
        public async Task SaveList_Async_EmptyList_DoesNothing()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("Ted", 21);

            await helper.SaveList_Async(new List<UserInfo>());

            Assert.Equal(1, await helper.Count_Async<UserInfo>());
            Assert.Equal(21, (await helper.FirstOrDefault_Async<UserInfo>(u.Id)).Age);
        }

        [Fact]
        public async Task SaveList_Async_AllNew_InsertsAll()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var newA = new UserInfo { Name = "NewA", Age = 20 };
            var newB = new UserInfo { Name = "NewB", Age = 30 };

            await helper.SaveList_Async(new List<UserInfo> { newA, newB });

            Assert.Equal(2, await helper.Count_Async<UserInfo>());
            Assert.NotNull(await helper.FirstOrDefault_Async<UserInfo>("Where Name=@0", "NewA"));
            Assert.NotNull(await helper.FirstOrDefault_Async<UserInfo>("Where Name=@0", "NewB"));
        }

        [Fact]
        public async Task SaveList_Async_AllExisting_UpdatesAll()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u1 = db.NewUser("Ted", 21);
            var u2 = db.NewUser("Bobby", 30);

            var users = await helper.Select_Async<UserInfo>("ORDER BY Id");
            users[0].Name = "Ted改";
            users[1].Age = 31;

            await helper.SaveList_Async(users);

            Assert.Equal(2, await helper.Count_Async<UserInfo>());
            Assert.Equal("Ted改", (await helper.FirstOrDefault_Async<UserInfo>(u1.Id)).Name);
            Assert.Equal(31, (await helper.FirstOrDefault_Async<UserInfo>(u2.Id)).Age);
        }

        [Fact]
        public async Task SaveList_Async_Mixed_InsertsAndUpdates()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u1 = db.NewUser("Ted", 21);
            var u2 = db.NewUser("Bobby", 30);

            var existing = await helper.FirstOrDefault_Async<UserInfo>(u1.Id);
            existing.Age = 22;
            var newA = new UserInfo { Name = "NewA", Age = 20 };

            await helper.SaveList_Async(new List<UserInfo> { existing, newA });

            Assert.Equal(3, await helper.Count_Async<UserInfo>());
            Assert.Equal(22, (await helper.FirstOrDefault_Async<UserInfo>(u1.Id)).Age);
            Assert.NotNull(await helper.FirstOrDefault_Async<UserInfo>("Where Name=@0", "NewA"));
        }

        #endregion
    }
}
