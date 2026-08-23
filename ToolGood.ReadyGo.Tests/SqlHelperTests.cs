using System.Data;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    /// <summary>
    /// SqlHelper 同步方法单元测试（基于 SQLite）
    /// </summary>
    public class SqlHelperTests
    {
        #region Insert

        [Fact]
        public void Insert_ReturnsAutoIncrementId()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            var u = new UserInfo { Name = "张三", Age = 20, Remark = "测试1", Money = 100.5m, IsDelete = false };
            var id = (long)helper.Insert(u);

            Assert.True(id > 0);
            Assert.True(u.Id > 0);
            Assert.Equal(id, u.Id);
        }

        [Fact]
        public void Insert_DateTimeDefaultNow_Works()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            var u = new UserInfo { Name = "张三", Age = 20, Money = 1m, IsDelete = false };
            helper.Insert(u);

            Assert.NotEqual(default(DateTime), u.CreateTime);
            // 重新读取，DateTime 可正常反序列化
            var loaded = helper.FirstOrDefault<UserInfo>(u.Id);
            Assert.NotNull(loaded);
            Assert.Equal(u.CreateTime.Date, loaded.CreateTime.Date);
        }

        [Fact]
        public void InsertList_InsertsAll()
        {
            using var db = TestDb.Create();
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
        public void Count_NoArg_And_WithCondition()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("张三", 20);
            db.NewUser("李四", 30);
            db.NewUser("王五", 40);

            Assert.Equal(3, helper.Count<UserInfo>());
            Assert.Equal(2, helper.Count<UserInfo>("WHERE Age > 20"));
        }

        [Fact]
        public void Exists_TrueAndFalse()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("张三", 20);

            Assert.True(helper.Exists<UserInfo>("WHERE Id = @0", u.Id));
            Assert.False(helper.Exists<UserInfo>("WHERE Id = 999"));
        }

        #endregion

        #region Select

        [Fact]
        public void Select_Variants()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            for (int i = 0; i < 10; i++) {
                db.NewUser("用户" + i, 20 + i);
            }

            Assert.Equal(10, helper.Select<UserInfo>().Count);
            Assert.Equal(5, helper.Select<UserInfo>(5).Count);
            Assert.Equal(3, helper.Select<UserInfo>(3, 2).Count);

            var bySql = helper.Select<UserInfo>("WHERE Age > 25 ORDER BY Id DESC LIMIT 5");
            Assert.Equal(4, bySql.Count);
            Assert.Equal("用户9", bySql[0].Name);
        }

        [Fact]
        public void SelectPage_And_Page()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            for (int i = 0; i < 13; i++) {
                db.NewUser("用户" + i, 20 + i);
            }

            var sp = helper.SelectPage<UserInfo>(1, 5, "ORDER BY Id");
            Assert.Equal(5, sp.Count);

            var page = helper.Page<UserInfo>(2, 5, "ORDER BY Id");
            Assert.Equal(2, page.CurrentPage);
            Assert.Equal(13, page.TotalItems);
            Assert.Equal(5, page.Items.Count);
            Assert.Equal(5, page.PageSize);
            Assert.Equal("用户5", page.Items[0].Name);
        }

        [Fact]
        public void FirstOrDefault_ByPk_BySql_Null()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u1 = db.NewUser("张三", 20);
            db.NewUser("李四", 30);

            var f1 = helper.FirstOrDefault<UserInfo>(u1.Id);
            Assert.NotNull(f1);
            Assert.Equal("张三", f1.Name);

            var f2 = helper.FirstOrDefault<UserInfo>("WHERE Name = @0", "李四");
            Assert.NotNull(f2);
            Assert.Equal(30, f2.Age);

            Assert.Null(helper.FirstOrDefault<UserInfo>("WHERE Id = 999"));
        }

        #endregion

        #region FetchMultiple

        [Fact]
        public void FetchMultiple_ReturnsTwoResultSets()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("张三", 20);
            db.NewUser("李四", 30);
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });

            var data = helper.FetchMultiple<UserInfo, SimpleUser>("SELECT * FROM UserInfo;SELECT * FROM SimpleUser;");

            Assert.Equal(2, data.Item1.Count);
            Assert.Equal(2, data.Item2.Count);
            Assert.Equal("张三", data.Item1[0].Name);
            Assert.Equal("甲", data.Item2[0].Name);
        }

        [Fact]
        public void FetchMultiple_ReturnsThreeResultSets()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("张三", 20);
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });

            var (users, simpleUsers, users2) = helper.FetchMultiple<UserInfo, SimpleUser, UserInfo>(
                "SELECT * FROM UserInfo;SELECT * FROM SimpleUser;SELECT * FROM UserInfo;");

            Assert.Single(users);
            Assert.Equal(2, simpleUsers.Count);
            Assert.Single(users2);
        }

        [Fact]
        public void FetchMultiple_WithCallback_Works()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("张三", 20);
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            var tuple = helper.FetchMultiple<UserInfo, SimpleUser, Tuple<List<UserInfo>, List<SimpleUser>>>(
                (u, s) => Tuple.Create(u, s),
                "SELECT * FROM UserInfo;SELECT * FROM SimpleUser;");

            Assert.Single(tuple.Item1);
            Assert.Single(tuple.Item2);
            Assert.Equal("张三", tuple.Item1[0].Name);
        }

        #endregion

        #region Update / Delete / Save

        [Fact]
        public void Update_Poco_And_Sql()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("李四", 30);
            var u2 = helper.FirstOrDefault<UserInfo>(u.Id);

            u2.Age = 31;
            Assert.Equal(1, helper.Update(u2));
            Assert.Equal(31, helper.FirstOrDefault<UserInfo>(u.Id).Age);

            Assert.Equal(1, helper.Update<UserInfo>("SET Remark = 'sql更新' WHERE Id = @0", u.Id));
            Assert.Equal("sql更新", helper.FirstOrDefault<UserInfo>(u.Id).Remark);
        }

        [Fact]
        public void StartSnapshot_Update_OnlyChangedColumns()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("Ted", 21);
            var user = helper.FirstOrDefault<UserInfo>(u.Id);

            var snapshot = helper.StartSnapshot(user); // 之后发生的修改都会被记录

            user.Name = "Bobby";
            user.Age = 21; // 与快照一致，不算变更

            helper.Update(user, snapshot.UpdatedColumns()); // 只更新 Name 列

            var loaded = helper.FirstOrDefault<UserInfo>(u.Id);
            Assert.Equal("Bobby", loaded.Name);
            Assert.Equal(21, loaded.Age);
        }

        [Fact]
        public void StartSnapshot_UpdateWithSnapshot_Works()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("Ted", 21);
            var user = helper.FirstOrDefault<UserInfo>(u.Id);

            var snapshot = helper.StartSnapshot(user);
            user.Name = "Bobby";
            user.Age = 22;

            Assert.Equal(1, helper.Update(user, snapshot)); // 只更新 Name、Age 两列
            var loaded = helper.FirstOrDefault<UserInfo>(u.Id);
            Assert.Equal("Bobby", loaded.Name);
            Assert.Equal(22, loaded.Age);
        }

        [Fact]
        public void Update_OnlySpecifiedColumns()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("Ted", 21);
            var user = helper.FirstOrDefault<UserInfo>(u.Id);

            user.Name = "Bobby";
            user.Age = 99;
            helper.Update(user, new[] { "Name" }); // 只更新 Name 列

            var loaded = helper.FirstOrDefault<UserInfo>(u.Id);
            Assert.Equal("Bobby", loaded.Name);
            Assert.Equal(21, loaded.Age); // Age 保持原值
        }

        [Fact]
        public void Delete_Poco_ById_Sql()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var a = db.NewUser("甲", 20);
            var b = db.NewUser("乙", 30);
            var c = db.NewUser("丙", 40);

            Assert.Equal(1, helper.Delete(a));
            Assert.Equal(1, helper.DeleteById<UserInfo>(b.Id));
            Assert.Equal(1, helper.Delete<UserInfo>("WHERE Id = @0", c.Id));
            Assert.Equal(0, helper.Count<UserInfo>());
        }

        [Fact]
        public void Save_Insert_Then_Update()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            var u = new UserInfo { Name = "保存新", Age = 66, Money = 1m, IsDelete = false };
            helper.Save(u);
            Assert.True(u.Id > 0);

            u.Age = 67;
            helper.Save(u);
            Assert.Equal(67, helper.FirstOrDefault<UserInfo>(u.Id).Age);
        }

        #endregion

        #region 对象条件（全字段等值匹配）

        [Fact]
        public void ObjectCondition_Select_FirstOrDefault_Count_Page()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });
            var s1 = helper.FirstOrDefault<SimpleUser>("WHERE Name = '甲'");
            var cond = new SimpleUser { Id = s1.Id, Name = s1.Name, Age = s1.Age };

            var users = helper.Select<SimpleUser>(cond);
            Assert.Single(users);
            Assert.Equal("甲", users[0].Name);

            var f = helper.FirstOrDefault<SimpleUser>(cond);
            Assert.NotNull(f);
            Assert.Equal(10, f.Age);

            Assert.Equal(1, helper.Count<SimpleUser>(cond));

            var page = helper.Page<SimpleUser>(1, 3, cond);
            Assert.Equal(1, page.TotalItems);
            Assert.Single(page.Items);
        }

        [Fact]
        public void ObjectCondition_Update_Set()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            var s = new SimpleUser { Name = "甲", Age = 10 };
            helper.Insert(s);
            var loaded = helper.FirstOrDefault<SimpleUser>(s.Id);
            var cond = new SimpleUser { Id = loaded.Id, Name = loaded.Name, Age = loaded.Age };

            Assert.Equal(1, helper.Update<SimpleUser>(new SimpleUser { Age = 99 }, cond, new[] { "Id", "Name" }));
            Assert.Equal(99, helper.FirstOrDefault<SimpleUser>(s.Id).Age);
        }

        [Fact]
        public void ObjectCondition_Delete()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            var s = new SimpleUser { Name = "乙", Age = 20 };
            helper.Insert(s);
            var loaded = helper.FirstOrDefault<SimpleUser>(s.Id);

            Assert.Equal(1, helper.Delete<SimpleUser>(new SimpleUser { Id = loaded.Id, Name = loaded.Name, Age = loaded.Age }));
            Assert.Equal(0, helper.Count<SimpleUser>());
        }

        #endregion

        #region SQL_* 系列

        [Fact]
        public void SqlSeries_FirstOrDefault_Select_Page()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("李四", 30);
            for (int i = 0; i < 10; i++) {
                db.NewUser("批量" + i, 40 + i);
            }

            var sq = helper.SQL_FirstOrDefault<UserInfo>("Id, Name, Age", "UserInfo", "WHERE Id = @0", u.Id);
            Assert.NotNull(sq);
            Assert.Equal("李四", sq.Name);

            var list = helper.SQL_Select<UserInfo>("Id, Name", "UserInfo", "ORDER BY Id", "WHERE Age > 30");
            Assert.Equal(10, list.Count);

            var page = helper.SQL_Page<UserInfo>(1, 3, "Id, Name", "UserInfo", "ORDER BY Id", "WHERE Age > 30");
            Assert.Equal(10, page.TotalItems);
            Assert.Equal(3, page.Items.Count);
        }

        #endregion

        #region ExecuteDataTable / ExecuteDataSet / ExecuteScalar

        [Fact]
        public void ExecuteDataTable_ReturnsRows()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);
            db.NewUser("乙", 30);

            var dt = helper.ExecuteDataTable("SELECT Id, Name FROM UserInfo");
            Assert.Equal(2, dt.Rows.Count);
            Assert.Equal("甲", dt.Rows[0]["Name"]);
        }

        [Fact]
        public void ExecuteDataSet_MultiResultSet()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);
            db.NewUser("乙", 30);

            var ds = helper.ExecuteDataSet("SELECT Id FROM UserInfo; SELECT Name FROM UserInfo");
            Assert.Equal(2, ds.Tables.Count);
            Assert.Equal(2, ds.Tables[0].Rows.Count);
            Assert.Equal(2, ds.Tables[1].Rows.Count);
        }

        [Fact]
        public void ExecuteScalar_Value()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);
            db.NewUser("乙", 30);

            Assert.Equal(2, helper.ExecuteScalar<int>("SELECT COUNT(*) FROM UserInfo"));
            Assert.Equal("乙", helper.ExecuteScalar<string>("SELECT Name FROM UserInfo WHERE Age = @0", 30));
        }

        #endregion

        #region GetTableName / LastSQL

        [Fact]
        public void GetTableName_DynamicTableName()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);

            dynamic tb = helper.GetTableName(typeof(UserInfo), "u");
            var sql = $"SELECT {tb.Id}, {tb.Name} FROM {tb} WHERE {tb.Age} > 0";
            var users = helper.Select<UserInfo>(sql);
            Assert.NotEmpty(users);
        }

        [Fact]
        public void LastSQL_NotEmpty()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);

            Assert.False(string.IsNullOrEmpty(helper._Sql.LastSQL));
        }

        #endregion

        #region 事务

        [Fact]
        public void Transaction_Commit()
        {
            using var db = TestDb.Create();
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
            using var db = TestDb.Create();
            var helper = db.Helper;

            using (var trans = helper.UseTransaction()) {
                helper.Execute("INSERT INTO UserInfo (Name, Age, Remark, CreateTime, Money, IsDelete) VALUES ('事务2', 1, NULL, '2026-01-01 00:00:00', 1, 0)");
                // 不调用 Complete，释放时应回滚
            }
            Assert.False(helper.Exists<UserInfo>("WHERE Name = '事务2'"));
        }

        #endregion
    }
}
