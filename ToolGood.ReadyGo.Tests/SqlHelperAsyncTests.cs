using ToolGood.ReadyGo;
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

        #region UseTransaction

        [Fact]
        public async Task UseTransaction_Async_Commit()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            await using (var tran = await helper.UseTransaction_Async()) {
                await helper.Execute_Async("INSERT INTO UserInfo (Name, Age, Remark, CreateTime, Money, IsDelete) VALUES ('事务1', 1, NULL, '2026-01-01 00:00:00', 1, 0)");
                await tran.CompleteAsync();
            }
            Assert.True(await helper.Exists_Async<UserInfo>("WHERE Name = '事务1'"));
        }

        [Fact]
        public async Task UseTransaction_Async_Rollback()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            await using (var tran = await helper.UseTransaction_Async()) {
                await helper.Execute_Async("INSERT INTO UserInfo (Name, Age, Remark, CreateTime, Money, IsDelete) VALUES ('事务2', 1, NULL, '2026-01-01 00:00:00', 1, 0)");
                // 不调用 CompleteAsync，释放时应回滚
            }
            Assert.False(await helper.Exists_Async<UserInfo>("WHERE Name = '事务2'"));
        }

        #endregion

        #region 对象条件修复专项

        [Fact]
        public async Task ObjectCondition_ColumnMapping_Query_Update_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Execute("CREATE TABLE MappedUser (Id INTEGER PRIMARY KEY AUTOINCREMENT, user_name TEXT, user_age INTEGER);");

            helper.Insert(new MappedUser { UserName = "甲", UserAge = 10 });
            helper.Insert(new MappedUser { UserName = "乙", UserAge = 20 });

            // 条件对象属性 UserName 应映射为数据库列 user_name
            var list = await helper.Select_Async<MappedUser>(new { UserName = "甲" });
            Assert.Single(list);
            Assert.Equal("甲", list[0].UserName);

            // Update 的 set/where 均使用映射列名
            Assert.Equal(1, await helper.Update_Async<MappedUser>(new { UserAge = 30 }, new { UserName = "乙" }));
            Assert.Equal(30, (await helper.FirstOrDefault_Async<MappedUser>(new { UserName = "乙" })).UserAge);
        }

        [Fact]
        public async Task Exists_Null_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            // 空表：Exists(null) 为无条件查询
            Assert.False(await helper.Exists_Async<SimpleUser>(null));

            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            Assert.True(await helper.Exists_Async<SimpleUser>(null));
        }

        [Fact]
        public async Task StringCondition_WithoutWhere_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });

            // object 条件路径中的 string（无 WHERE 前缀）应自动补全 WHERE
            object cond = "Name = '甲'";
            Assert.Single(await helper.Select_Async<SimpleUser>(cond));
            Assert.Equal(1, await helper.Count_Async<SimpleUser>((object)"Age = 10"));
            Assert.Equal("乙", (await helper.FirstOrDefault_Async<SimpleUser>((object)"Name = '乙'")).Name);
            Assert.True(await helper.Exists_Async<SimpleUser>((object)"Age = 20"));
            Assert.Equal(1, await helper.Delete_Async<SimpleUser>(cond));

            // Update 的 condition 为 string 时同样自动补 WHERE
            Assert.Equal(1, await helper.Update_Async<SimpleUser>(new { Age = 100 }, "Name = '乙'"));
            Assert.Equal(1, await helper.Count_Async<SimpleUser>("WHERE Age = 100"));
        }

        [Fact]
        public async Task InList_WithNullElement_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = null, Age = 20 });

            // [null, "甲"] → (Name IS NULL OR Name='甲')
            Assert.Equal(2, (await helper.Select_Async<SimpleUser>(new { Name = new string[] { null, "甲" } })).Count);

            // 空集合 → 1=2 恒假
            Assert.Empty(await helper.Select_Async<SimpleUser>(new { Name = new string[0] }));
        }

        [Fact]
        public async Task Update_SetCollection_Throws_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            // set 属性为集合值无法生成 UPDATE SQL，应抛出明确异常
            await Assert.ThrowsAsync<ArgumentException>(() => helper.Update_Async<SimpleUser>(
                new { Ages = new List<int> { 1, 2 } }, new { Id = 1 }));
        }

        [Fact]
        public async Task Update_EmptySet_Throws_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            // set 为空对象没有可更新字段，应抛出明确异常
            await Assert.ThrowsAsync<ArgumentException>(() => helper.Update_Async<SimpleUser>(new { }, new { Id = 1 }));
        }

        [Fact]
        public async Task Update_EmptyCondition_UpdatesAll_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });

            // 空对象条件 = 无条件更新（与 null 一致）
            Assert.Equal(2, await helper.Update_Async<SimpleUser>(new { Age = 99 }, new { }));
            Assert.Equal(2, await helper.Count_Async<SimpleUser>("WHERE Age = 99"));
        }

        [Fact]
        public async Task EmptyCondition_QueryAll_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });

            Assert.Equal(2, (await helper.Select_Async<SimpleUser>(new { })).Count);
            Assert.NotNull(await helper.FirstOrDefault_Async<SimpleUser>(new { }));
        }

        [Fact]
        public async Task FirstOrDefault_NullAndNullable_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });

            // null 字面量：无条件取第一条（重载歧义已消除）
            Assert.Equal("甲", (await helper.FirstOrDefault_Async<SimpleUser>(null)).Name);

            // (int?)null：无条件取第一条（不再是查主键 0）
            Assert.Equal("甲", (await helper.FirstOrDefault_Async<SimpleUser>((int?)null)).Name);

            // (int?)有值 / 数值重载：按主键查询
            Assert.Null(await helper.FirstOrDefault_Async<SimpleUser>((int?)9999));
            Assert.Null(await helper.FirstOrDefault_Async<SimpleUser>(9999));
        }

        [Fact]
        public async Task FirstOrDefault_LongPk_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("甲", 20);

            Assert.Equal("甲", (await helper.FirstOrDefault_Async<UserInfo>((long)u.Id)).Name);
            Assert.Null(await helper.FirstOrDefault_Async<UserInfo>((long)9999));
        }

        [Fact]
        public async Task Delete_ObjectCondition_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var s = new SimpleUser { Name = "乙", Age = 20 };
            helper.Insert(s);
            var loaded = await helper.FirstOrDefault_Async<SimpleUser>(s.Id);
            var cond = new SimpleUser { Id = loaded.Id, Name = loaded.Name, Age = loaded.Age };

            Assert.Equal(1, await helper.Delete_Async<SimpleUser>(cond));
            Assert.Equal(0, await helper.Count_Async<SimpleUser>());
        }

        [Fact]
        public async Task ObjectCondition_InList_MultipleValues_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });
            helper.Insert(new SimpleUser { Name = "丙", Age = 30 });

            // 多元素 in 列表（不含 null）
            var list = await helper.Select_Async<SimpleUser>(new { Age = new int[] { 10, 30 } });
            Assert.Equal(2, list.Count);
            Assert.Equal("甲", list[0].Name);
            Assert.Equal("丙", list[1].Name);
        }

        [Fact]
        public async Task ObjectCondition_ScalarTypes_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20, 10.5m, false);
            db.NewUser("乙", 30, 99.9m, true);

            // bool 条件 → 0/1
            var boolList = await helper.Select_Async<UserInfo>(new { IsDelete = false });
            Assert.Single(boolList);
            Assert.Equal("甲", boolList[0].Name);

            // decimal 条件
            Assert.Single(await helper.Select_Async<UserInfo>(new { Money = 99.9m }));

            // DateTime 条件：参数化插入后，再以等值条件查询
            var dt = new DateTime(2020, 1, 2, 3, 4, 5, 600);
            await helper.Execute_Async("INSERT INTO UserInfo (Name, Age, Remark, CreateTime, Money, IsDelete) VALUES ('丙', 40, NULL, @0, 0, 0)", dt);
            Assert.Single(await helper.Select_Async<UserInfo>(new { CreateTime = dt }));
        }

        [Fact]
        public async Task ObjectCondition_EnumValue_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_Enum2IntTest));

            helper.Insert(new Tb_Enum2IntTest { State = UserState.None });
            helper.Insert(new Tb_Enum2IntTest { State = UserState.Vip });

            // 枚举作为条件对象值 → 转成底层整数
            var list = await helper.Select_Async<Tb_Enum2IntTest>(new { State = UserState.Vip });
            Assert.Single(list);
            Assert.Equal(UserState.Vip, list[0].State);
        }

        [Fact]
        public async Task ObjectCondition_Update_NullSet_Throws_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            // set 为 null 应抛出明确异常（显式转为 object，避免命中 string 重载）
            await Assert.ThrowsAsync<ArgumentException>(() => helper.Update_Async<SimpleUser>((object)null, new { Id = 1 }));
        }

        [Fact]
        public async Task ObjectCondition_IEnumerableCondition_Throws_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            // 条件对象为集合类型无法生成 WHERE，应抛出明确异常
            await Assert.ThrowsAsync<ArgumentException>(() => helper.Select_Async<SimpleUser>(new List<SimpleUser>()));
        }

        [Fact]
        public async Task StringPrimaryKey_ObjectCondition_FirstOrDefault_Exists_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            // SQLite 仅允许 INTEGER PRIMARY KEY 使用 AUTOINCREMENT，字符串主键需手动建表
            helper.Execute("CREATE TABLE IF NOT EXISTS StringKeyUser (Code TEXT PRIMARY KEY NOT NULL, Name TEXT NULL, Age INTEGER NOT NULL)");

            helper.Insert(new StringKeyUser { Code = "USR-001", Name = "甲", Age = 20 });
            helper.Insert(new StringKeyUser { Code = "USR-002", Name = "乙", Age = 30 });

            // 无 SQL 特征的字符串 → 按字符串主键查询
            var byPk = await helper.FirstOrDefault_Async<StringKeyUser>((object)"USR-001");
            Assert.NotNull(byPk);
            Assert.Equal("甲", byPk.Name);

            Assert.True(await helper.Exists_Async<StringKeyUser>((object)"USR-002"));
            Assert.False(await helper.Exists_Async<StringKeyUser>((object)"USR-999"));

            // 带 SQL 特征的字符串 → 仍按 SQL 片段处理
            object cond = "Name = '乙'";
            Assert.Equal("乙", (await helper.FirstOrDefault_Async<StringKeyUser>(cond)).Name);
        }

        [Fact]
        public async Task ObjectCondition_UnsupportedValueType_Throws_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            // 非整数值类型不应被静默当作主键
            await Assert.ThrowsAsync<ArgumentException>(() => helper.FirstOrDefault_Async<SimpleUser>(20.5));
            await Assert.ThrowsAsync<ArgumentException>(() => helper.FirstOrDefault_Async<SimpleUser>(true));
            await Assert.ThrowsAsync<ArgumentException>(() => helper.Exists_Async<SimpleUser>(20.5m));
        }

        [Fact]
        public async Task ObjectCondition_ByteArray_NotExpandedAsList_Async()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_BlobTest));

            var data = new byte[] { 0x01, 0x02, 0x03 };
            helper.Insert(new Tb_BlobTest { Name = "b1", Data = data });
            helper.Insert(new Tb_BlobTest { Name = "b2", Data = new byte[] { 0x0A, 0x0B } });

            // byte[] 作为条件值应整体匹配（BLOB），而不是被展开成 in 列表
            var list = await helper.Select_Async<Tb_BlobTest>(new { Data = data });
            Assert.Single(list);
            Assert.Equal("b1", list[0].Name);
        }

        #endregion
    }
}
