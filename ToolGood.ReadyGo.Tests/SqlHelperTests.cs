using System.Data;
using ToolGood.ReadyGo;
using ToolGood.ReadyGo.NPoco;
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

            var data = helper.SelectMultiple<UserInfo, SimpleUser>("SELECT * FROM UserInfo;SELECT * FROM SimpleUser;");

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

            var (users, simpleUsers, users2) = helper.SelectMultiple<UserInfo, SimpleUser, UserInfo>(
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

            var tuple = helper.SelectMultiple<UserInfo, SimpleUser, Tuple<List<UserInfo>, List<SimpleUser>>>(
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

        #region 对象条件修复专项

        [Fact]
        public void ObjectCondition_ColumnMapping_Query_Update()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Execute("CREATE TABLE MappedUser (Id INTEGER PRIMARY KEY AUTOINCREMENT, user_name TEXT, user_age INTEGER);");

            helper.Insert(new MappedUser { UserName = "甲", UserAge = 10 });
            helper.Insert(new MappedUser { UserName = "乙", UserAge = 20 });

            // 条件对象属性 UserName 应映射为数据库列 user_name
            var list = helper.Select<MappedUser>(new { UserName = "甲" });
            Assert.Single(list);
            Assert.Equal("甲", list[0].UserName);

            // Update 的 set/where 均使用映射列名
            Assert.Equal(1, helper.Update<MappedUser>(new { UserAge = 30 }, new { UserName = "乙" }));
            Assert.Equal(30, helper.FirstOrDefault<MappedUser>(new { UserName = "乙" }).UserAge);

            // 值类型主键查询不受影响
            Assert.Equal("甲", helper.FirstOrDefault<MappedUser>(list[0].Id).UserName);
        }

        [Fact]
        public void ObjectCondition_Exists_Null_AndEmpty()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            // 空表：Exists(null) 为无条件查询
            Assert.False(helper.Exists<SimpleUser>(null));

            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            Assert.True(helper.Exists<SimpleUser>(null));
        }

        [Fact]
        public void ObjectCondition_StringCondition_WithoutWhere_AllApis()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });

            // object 条件路径中的 string（无 WHERE 前缀）应自动补全 WHERE
            object cond = "Name = '甲'";
            Assert.Single(helper.Select<SimpleUser>(cond));
            Assert.Equal(1, helper.Count<SimpleUser>((object)"Age = 10"));
            Assert.Equal("乙", helper.FirstOrDefault<SimpleUser>((object)"Name = '乙'").Name);
            Assert.True(helper.Exists<SimpleUser>((object)"Age = 20"));
            Assert.Equal(1, helper.Delete<SimpleUser>(cond));
            Assert.Equal(1, helper.Count<SimpleUser>());

            // Update 的 condition 为 string 时同样自动补 WHERE
            Assert.Equal(1, helper.Update<SimpleUser>(new { Age = 100 }, "Name = '乙'"));
            Assert.Equal(1, helper.Count<SimpleUser>("WHERE Age = 100"));
        }

        [Fact]
        public void ObjectCondition_InList_WithNullElement()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = null, Age = 20 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 30 });

            // [null, "甲"] → (Name IS NULL OR Name='甲')
            Assert.Equal(2, helper.Select<SimpleUser>(new { Name = new string[] { null, "甲" } }).Count);

            // 纯 null → Name IS NULL
            var onlyNull = helper.Select<SimpleUser>(new { Name = new string[] { null } });
            Assert.Single(onlyNull);
            Assert.Equal(20, onlyNull[0].Age);

            // 空集合 → 1=2 恒假
            Assert.Empty(helper.Select<SimpleUser>(new { Name = new string[0] }));
        }

        [Fact]
        public void ObjectCondition_Update_SetCollection_Throws()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            // set 属性为集合值无法生成 UPDATE SQL，应抛出明确异常
            Assert.Throws<ArgumentException>(() => helper.Update<SimpleUser>(
                new { Ages = new List<int> { 1, 2 } }, new { Id = 1 }));
        }

        [Fact]
        public void ObjectCondition_Update_EmptySet_Throws()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            // set 为空对象没有可更新字段，应抛出明确异常
            Assert.Throws<ArgumentException>(() => helper.Update<SimpleUser>(new { }, new { Id = 1 }));
        }

        [Fact]
        public void ObjectCondition_Update_EmptyCondition_UpdatesAll()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });

            // 空对象条件 = 无条件更新（与 null 一致）
            Assert.Equal(2, helper.Update<SimpleUser>(new { Age = 99 }, new { }));
            Assert.Equal(2, helper.Count<SimpleUser>("WHERE Age = 99"));
        }

        [Fact]
        public void ObjectCondition_EmptyCondition_QueryAll()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });

            Assert.Equal(2, helper.Select<SimpleUser>(new { }).Count);
            Assert.NotNull(helper.FirstOrDefault<SimpleUser>(new { }));
            Assert.Equal(2, helper.Count<SimpleUser>(new { }));
        }

        [Fact]
        public void FirstOrDefault_NullAndNullableOverloads()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });
            helper.Insert(new SimpleUser { Name = "乙", Age = 20 });

            // null 字面量：无条件取第一条（重载歧义已消除）
            Assert.Equal("甲", helper.FirstOrDefault<SimpleUser>(null).Name);

            // (int?)null：无条件取第一条（不再是查主键 0）
            Assert.Equal("甲", helper.FirstOrDefault<SimpleUser>((int?)null).Name);

            // (int?)有值 / 数值重载：按主键查询
            Assert.Null(helper.FirstOrDefault<SimpleUser>((int?)9999));
            Assert.Null(helper.FirstOrDefault<SimpleUser>(9999));
        }

        [Fact]
        public void EscapeParam_FallbackType_UsesSameEscapingAsString()
        {
            var exposer = new EscapeParamExposer();

            // 未识别类型走兜底分支：转义结果与 string 分支完全一致（同为 ToEscapeParam）
            var raw = "o'brien\\x";
            Assert.Equal(exposer.PublicEscapeParam(raw), exposer.PublicEscapeParam(new QuoteStr(raw)));

            // 兜底分支具体输出（修复前为未转义拼接："'o'brien\\x'"）
            Assert.Equal("'o\\'brien\\\\x'", exposer.PublicEscapeParam(new QuoteStr(raw)));
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

        [Fact]
        public void SQL_Select_LimitOverload()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            for (int i = 0; i < 10; i++) {
                db.NewUser("用户" + i, 20 + i);
            }

            var list = helper.SQL_Select<UserInfo>(5, "Id, Name", "UserInfo", "ORDER BY Id", "");

            Assert.Equal(5, list.Count);
            Assert.Equal("用户0", list[0].Name);
            Assert.Equal("用户4", list[4].Name);
        }

        [Fact]
        public void SQL_Select_PageOverload()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            for (int i = 0; i < 10; i++) {
                db.NewUser("用户" + i, 20 + i);
            }

            var list = helper.SQL_Select<UserInfo>(3, 4, "Id, Name", "UserInfo", "ORDER BY Id", "");

            Assert.Equal(2, list.Count);
            Assert.Equal("用户8", list[0].Name);
            Assert.Equal("用户9", list[1].Name);
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

        #region 补充覆盖

        [Fact]
        public void Select_Count_And_RawCount()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);
            db.NewUser("乙", 30);
            db.NewUser("丙", 40);

            // Select_Count<T> 别名
            Assert.Equal(3, helper.Select_Count<UserInfo>());
            Assert.Equal(2, helper.Select_Count<UserInfo>("WHERE Age > 20"));
            // Count<T>(sql) 包装为 SELECT COUNT(*)
            Assert.Equal(3, helper.Count<UserInfo>());
            Assert.Equal(2, helper.Count<UserInfo>("WHERE Age > 20"));
            // 原生 SQL Count
            Assert.Equal(3, helper.Count("SELECT COUNT(*) FROM UserInfo"));
            Assert.Equal(1, helper.Count("SELECT COUNT(*) FROM UserInfo WHERE Name = @0", "甲"));
        }

        [Fact]
        public void GetTableName_Generic()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("甲", 20);

            // dynamic 访问
            dynamic tb = helper.GetTableName<UserInfo>("u");
            var sql = $"SELECT {tb.Id}, {tb.Name} FROM {tb} WHERE {tb.Age} > 0";
            Assert.NotEmpty(helper.Select<UserInfo>(sql));

            // 强类型 F() 访问
            var tb2 = helper.GetTableName<UserInfo>("u");
            var sql2 = $"SELECT {tb2.F(x => x.Id)}, {tb2.F(x => x.Name)} FROM {tb2} WHERE {tb2.F(x => x.Age)} > 0";
            Assert.NotEmpty(helper.Select<UserInfo>(sql2));
        }

        [Fact]
        public void Exists_ObjectCondition_ClassAndScalar()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var s = new SimpleUser { Name = "甲", Age = 10 };
            helper.Insert(s);
            var loaded = helper.FirstOrDefault<SimpleUser>(s.Id);

            // 对象条件（class）
            var cond = new SimpleUser { Id = loaded.Id, Name = loaded.Name, Age = loaded.Age };
            Assert.True(helper.Exists<SimpleUser>(cond));
            Assert.False(helper.Exists<SimpleUser>(new SimpleUser { Id = 999, Name = "不存在", Age = 0 }));

            // 主键值（标量）
            Assert.True(helper.Exists<SimpleUser>(loaded.Id));
            Assert.False(helper.Exists<SimpleUser>(999));
        }

        [Fact]
        public void FirstOrDefault_NumericPkOverloads()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("甲", 20);

            Assert.Equal("甲", helper.FirstOrDefault<UserInfo>((long)u.Id).Name);
            Assert.Equal("甲", helper.FirstOrDefault<UserInfo>((uint)u.Id).Name);
            Assert.Equal("甲", helper.FirstOrDefault<UserInfo>((ulong)u.Id).Name);
            Assert.Null(helper.FirstOrDefault<UserInfo>((uint)9999));
        }

        [Fact]
        public void Select_ObjectCondition_LimitAndPage()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var s = new SimpleUser { Name = "甲", Age = 10 };
            helper.Insert(s);
            var loaded = helper.FirstOrDefault<SimpleUser>(s.Id);
            var cond = new SimpleUser { Id = loaded.Id, Name = loaded.Name, Age = loaded.Age };

            Assert.Single(helper.Select<SimpleUser>(5, cond));
            Assert.Single(helper.Select<SimpleUser>(5, 0, cond));
            Assert.Single(helper.SelectPage<SimpleUser>(1, 3, cond));
        }

        [Fact]
        public void FetchMultiple_FourResultSets()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("张三", 20);
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            var (users, simpleUsers, users2, simpleUsers2) = helper.SelectMultiple<UserInfo, SimpleUser, UserInfo, SimpleUser>(
                "SELECT * FROM UserInfo;SELECT * FROM SimpleUser;SELECT * FROM UserInfo;SELECT * FROM SimpleUser;");

            Assert.Single(users);
            Assert.Single(simpleUsers);
            Assert.Single(users2);
            Assert.Single(simpleUsers2);
        }

        [Fact]
        public void FetchMultiple_ThreeFourTupleCallback()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            db.NewUser("张三", 20);
            helper.Insert(new SimpleUser { Name = "甲", Age = 10 });

            var r3 = helper.SelectMultiple<UserInfo, SimpleUser, UserInfo, int>(
                (u, s, u2) => u.Count + s.Count + u2.Count,
                "SELECT * FROM UserInfo;SELECT * FROM SimpleUser;SELECT * FROM UserInfo;");
            Assert.Equal(3, r3);

            var r4 = helper.SelectMultiple<UserInfo, SimpleUser, UserInfo, SimpleUser, int>(
                (u, s, u2, s2) => u.Count + s.Count + u2.Count + s2.Count,
                "SELECT * FROM UserInfo;SELECT * FROM SimpleUser;SELECT * FROM UserInfo;SELECT * FROM SimpleUser;");
            Assert.Equal(4, r4);
        }

        #endregion

        #region UpdateList

        [Fact]
        public void UpdateList_EmptyList_ReturnsZero()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;

            Assert.Equal(0, helper.UpdateList(new List<UserInfo>()));
            Assert.Equal(0, helper.UpdateList(new List<UserInfo>(), new List<Snapshot<UserInfo>>()));
        }

        [Fact]
        public void UpdateList_MismatchedSnapshots_Throws()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("Ted", 21);
            var user = helper.FirstOrDefault<UserInfo>(u.Id);
            var snapshot = helper.StartSnapshot(user);

            var list = new List<UserInfo> { user, user };
            var snapshots = new List<Snapshot<UserInfo>> { snapshot };

            var ex = Assert.Throws<ArgumentException>(() => helper.UpdateList(list, snapshots));
            Assert.Equal("list.Count must equal snapshots.Count.", ex.Message);
        }

        [Fact]
        public void UpdateList_WithSnapshots_OnlyUpdatesChangedColumns()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u1 = db.NewUser("Ted", 21);
            var u2 = db.NewUser("Bobby", 30);

            var users = helper.Select<UserInfo>("ORDER BY Id");
            var snapshots = users.Select(x => helper.StartSnapshot(x)).ToList();

            users[0].Name = "Ted改"; // 仅 Name 变更
            users[1].Age = 31;       // 仅 Age 变更

            Assert.Equal(2, helper.UpdateList(users, snapshots));

            var loaded1 = helper.FirstOrDefault<UserInfo>(u1.Id);
            var loaded2 = helper.FirstOrDefault<UserInfo>(u2.Id);
            Assert.Equal("Ted改", loaded1.Name);
            Assert.Equal(21, loaded1.Age);     // 未变更列保持原值
            Assert.Equal("Bobby", loaded2.Name); // 未变更列保持原值
            Assert.Equal(31, loaded2.Age);
        }

        #endregion

        #region SaveList

        [Fact]
        public void SaveList_EmptyList_DoesNothing()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u = db.NewUser("Ted", 21);

            helper.SaveList(new List<UserInfo>());

            Assert.Equal(1, helper.Count<UserInfo>());
            Assert.Equal(21, helper.FirstOrDefault<UserInfo>(u.Id).Age);
        }

        [Fact]
        public void SaveList_AllNew_InsertsAll()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var newA = new UserInfo { Name = "NewA", Age = 20 };
            var newB = new UserInfo { Name = "NewB", Age = 30 };

            helper.SaveList(new List<UserInfo> { newA, newB });

            Assert.Equal(2, helper.Count<UserInfo>());
            Assert.NotNull(helper.FirstOrDefault<UserInfo>("Where Name=@0", "NewA"));
            Assert.NotNull(helper.FirstOrDefault<UserInfo>("Where Name=@0", "NewB"));
        }

        [Fact]
        public void SaveList_AllExisting_UpdatesAll()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u1 = db.NewUser("Ted", 21);
            var u2 = db.NewUser("Bobby", 30);

            var users = helper.Select<UserInfo>("ORDER BY Id");
            users[0].Name = "Ted改";
            users[1].Age = 31;

            helper.SaveList(users);

            Assert.Equal(2, helper.Count<UserInfo>());
            Assert.Equal("Ted改", helper.FirstOrDefault<UserInfo>(u1.Id).Name);
            Assert.Equal(31, helper.FirstOrDefault<UserInfo>(u2.Id).Age);
        }

        [Fact]
        public void SaveList_Mixed_InsertsAndUpdates()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            var u1 = db.NewUser("Ted", 21);
            var u2 = db.NewUser("Bobby", 30);

            var existing = helper.FirstOrDefault<UserInfo>(u1.Id);
            existing.Age = 22;
            var newA = new UserInfo { Name = "NewA", Age = 20 };

            helper.SaveList(new List<UserInfo> { existing, newA });

            Assert.Equal(3, helper.Count<UserInfo>());
            Assert.Equal(22, helper.FirstOrDefault<UserInfo>(u1.Id).Age);
            Assert.NotNull(helper.FirstOrDefault<UserInfo>("Where Name=@0", "NewA"));
        }

        #endregion
    }

    /// <summary>
    /// 未识别类型：ToString 返回指定值，用于验证 EscapeParam 兜底分支的转义行为。
    /// </summary>
    internal sealed class QuoteStr
    {
        private readonly string _value;

        public QuoteStr(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;
    }

    /// <summary>
    /// 暴露 SqlHelper 的 protected EscapeParam 方法，用于直接断言转义输出。
    /// </summary>
    internal sealed class EscapeParamExposer : SqlHelper
    {
        public EscapeParamExposer() : base(":memory:", Microsoft.Data.Sqlite.SqliteFactory.Instance, SqlType.SQLite)
        {
        }

        public string PublicEscapeParam(object value) => EscapeParam(value);
    }
}
