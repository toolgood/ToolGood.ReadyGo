using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using ToolGood.ReadyGo.NPoco;
using Xunit;
using NPocoDatabase = ToolGood.ReadyGo.NPoco.Database;

namespace ToolGood.ReadyGo.Tests
{
    /// <summary>
    /// Core/Linq 删除与更新查询器（IDeleteQueryProvider/IUpdateQueryProvider 及异步版本）
    /// 动态条件方法测试（SQLite）
    /// </summary>
    public class CoreLinqDeleteUpdateTests : IDisposable
    {
        private readonly SqliteConnection _conn;
        private readonly NPocoDatabase _db;

        public CoreLinqDeleteUpdateTests()
        {
            var dbFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"readygo_delupd_{Guid.NewGuid():N}.db");
            _conn = new SqliteConnection($"Data Source={dbFile};Pooling=False;");
            _conn.Open();
            _db = new NPocoDatabase(_conn);
            _db.Execute("CREATE TABLE SimpleUser (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Age INTEGER);");
            Insert("甲", 10);
            Insert("乙", 20);
            Insert("丙", 30);
        }

        public void Dispose()
        {
            _db.Dispose();
            _conn.Dispose();
            if (System.IO.File.Exists(_conn.DataSource)) {
                try {
                    System.IO.File.Delete(_conn.DataSource);
                } catch {
                    // 忽略清理失败
                }
            }
        }

        private void Insert(string name, int age)
        {
            _db.Insert(new SimpleUser { Name = name, Age = age });
        }

        private int Count()
        {
            return _db.Query<SimpleUser>().Select().Count;
        }

        #region 删除查询器 DeleteMany

        [Fact]
        public void Delete_IfTrueWhere_AppliesWhenTrue_SkipWhenFalse()
        {
            var whenTrue = _db.DeleteMany<SimpleUser>().IfTrueWhere(true, x => x.Age > 20).Execute();
            Assert.Equal(1, whenTrue);
            Assert.Equal(2, Count());

            var whenFalse = _db.DeleteMany<SimpleUser>().IfTrueWhere(false, x => x.Age > 20).Execute();
            Assert.Equal(2, whenFalse);
            Assert.Equal(0, Count());
        }

        [Fact]
        public void Delete_WhereIn_ExpressionAndString()
        {
            var byExpr = _db.DeleteMany<SimpleUser>().WhereIn(x => x.Age, new[] { 10, 30 }).Execute();
            Assert.Equal(2, byExpr);
            Assert.Equal(1, Count());

            var byString = _db.DeleteMany<SimpleUser>().WhereIn("Age", new[] { 20 }).Execute();
            Assert.Equal(1, byString);
            Assert.Equal(0, Count());
        }

        [Fact]
        public void Delete_WhereIn_EmptyGeneratesNoRows()
        {
            var empty = _db.DeleteMany<SimpleUser>().WhereIn(x => x.Age, new int[0]).Execute();
            Assert.Equal(0, empty);
            Assert.Equal(3, Count());
        }

        [Fact]
        public void Delete_IfTrueWhereIn_Works()
        {
            var applied = _db.DeleteMany<SimpleUser>().IfTrueWhereIn(true, x => x.Name, new[] { "甲", "丙" }).Execute();
            Assert.Equal(2, applied);

            var skipped = _db.DeleteMany<SimpleUser>().IfTrueWhereIn(false, x => x.Name, new[] { "乙" }).Execute();
            Assert.Equal(1, skipped);
            Assert.Equal(0, Count());
        }

        [Fact]
        public void Delete_WhereLike_And_IfTrueWhereLike()
        {
            var like = _db.DeleteMany<SimpleUser>().WhereLike(x => x.Name, "甲").Execute();
            Assert.Equal(1, like);

            var apply = _db.DeleteMany<SimpleUser>().IfTrueWhereLike(true, "Name", "乙").Execute();
            Assert.Equal(1, apply);

            var skip = _db.DeleteMany<SimpleUser>().IfTrueWhereLike(false, x => x.Name, "丙").Execute();
            Assert.Equal(1, skip);
            Assert.Equal(0, Count());
        }

        [Fact]
        public void Delete_WhereNotIn_And_IfTrueWhereNotIn()
        {
            var byExpr = _db.DeleteMany<SimpleUser>().WhereNotIn(x => x.Age, new[] { 20 }).Execute();
            Assert.Equal(2, byExpr);
            Assert.Equal(1, Count());

            // 剩余"乙"Age=20，<> 20 不匹配 → 删除 0 行
            var byString = _db.DeleteMany<SimpleUser>().WhereNotIn("Age", new[] { 20 }).Execute();
            Assert.Equal(0, byString);
            Assert.Equal(1, Count());

            var applied = _db.DeleteMany<SimpleUser>().IfTrueWhereNotIn(true, x => x.Age, new[] { 20 }).Execute();
            Assert.Equal(0, applied);
            Assert.Equal(1, Count());

            // 空集合生成 1=1，删除剩余全部
            var empty = _db.DeleteMany<SimpleUser>().WhereNotIn(x => x.Age, new int[0]).Execute();
            Assert.Equal(1, empty);
            Assert.Equal(0, Count());

            // false 不添加条件，表已空 → 0 行
            var skip = _db.DeleteMany<SimpleUser>().IfTrueWhereNotIn(false, x => x.Age, new[] { 20 }).Execute();
            Assert.Equal(0, skip);
        }

        [Fact]
        public void Delete_WhereLikeStart_And_WhereLikeEnd()
        {
            // WhereLikeStart: LIKE '%乙'，以"乙"结尾 → 乙
            var start = _db.DeleteMany<SimpleUser>().WhereLikeStart(x => x.Name, "乙").Execute();
            Assert.Equal(1, start);
            // WhereLikeEnd: LIKE '丙%'，以"丙"开头 → 丙
            var end = _db.DeleteMany<SimpleUser>().WhereLikeEnd(x => x.Name, "丙").Execute();
            Assert.Equal(1, end);
            Assert.Equal(1, Count());

            var skipStart = _db.DeleteMany<SimpleUser>().IfTrueWhereLikeStart(false, "Name", "甲").Execute();
            Assert.Equal(1, skipStart);
            // 表已清空，再删除为 0 行
            var skipEnd = _db.DeleteMany<SimpleUser>().IfTrueWhereLikeEnd(false, x => x.Name, "甲").Execute();
            Assert.Equal(0, skipEnd);
            Assert.Equal(0, Count());
        }

        [Fact]
        public void Delete_WhereExists_And_WhereNotExists()
        {
            var exists = _db.DeleteMany<SimpleUser>().WhereExists("SELECT 1 FROM SimpleUser WHERE Age > 25").Execute();
            Assert.Equal(3, exists);

            var notExists = _db.DeleteMany<SimpleUser>().WhereNotExists("SELECT 1 FROM SimpleUser WHERE Age > 100").Execute();
            Assert.Equal(0, notExists);
            Assert.Equal(0, Count());
        }

        [Fact]
        public void Delete_IfTrueWhereExists_Works()
        {
            var applied = _db.DeleteMany<SimpleUser>().IfTrueWhereExists(true, "SELECT 1 FROM SimpleUser WHERE Age > 25").Execute();
            Assert.Equal(3, applied);

            var skipped = _db.DeleteMany<SimpleUser>().IfTrueWhereNotExists(false, "SELECT 1 FROM SimpleUser WHERE Age > 25").Execute();
            Assert.Equal(0, skipped);
        }

        [Fact]
        public void Delete_Chain_AllDynamicConditions()
        {
            var n = _db.DeleteMany<SimpleUser>()
                .IfTrueWhere(true, x => x.Age >= 10)
                .WhereIn(x => x.Age, new[] { 10, 20, 30 })
                .IfTrueWhereLike(true, x => x.Name, "甲")
                .WhereNotIn(x => x.Age, new[] { 99 })
                .WhereLikeStart(x => x.Name, "甲")
                .WhereLikeEnd(x => x.Name, "甲")
                .IfTrueWhereExists(false, "SELECT 1 FROM SimpleUser WHERE Age > 100")
                .IfTrueWhereNotExists(true, "SELECT 1 FROM SimpleUser WHERE Age > 100")
                .Execute();
            Assert.Equal(1, n);
            Assert.Equal(2, Count());
        }

        [Fact]
        public async Task DeleteManyAsync_Works()
        {
            var deleted = await _db.DeleteManyAsync<SimpleUser>()
                .IfTrueWhere(true, x => x.Age > 20)
                .WhereIn(x => x.Name, new[] { "丙" })
                .Execute();
            Assert.Equal(1, deleted);
            Assert.Equal(2, Count());
        }

        #endregion 删除查询器 DeleteMany

        #region 更新查询器 UpdateMany

        [Fact]
        public void Update_IfTrueWhere_AppliesWhenTrue()
        {
            var target = _db.Query<SimpleUser>().Select().First(u => u.Name == "乙");
            target.Age = 99;
            var n = _db.UpdateMany<SimpleUser>().IfTrueWhere(true, x => x.Age == 20).Execute(target);
            Assert.Equal(1, n);
            Assert.Equal(99, _db.Query<SimpleUser>().Select().First(u => u.Name == "乙").Age);
        }

        [Fact]
        public void Update_WhereIn_ExpressionAndString()
        {
            var byExpr = _db.Query<SimpleUser>().Select().First(u => u.Name == "甲");
            byExpr.Age = 11;
            var n1 = _db.UpdateMany<SimpleUser>().WhereIn(x => x.Id, new[] { byExpr.Id }).Execute(byExpr);
            Assert.Equal(1, n1);
            Assert.Equal(11, _db.Query<SimpleUser>().Select().First(u => u.Name == "甲").Age);

            var byString = _db.Query<SimpleUser>().Select().First(u => u.Name == "丙");
            byString.Age = 33;
            var n2 = _db.UpdateMany<SimpleUser>().WhereIn("Id", new[] { byString.Id }).Execute(byString);
            Assert.Equal(1, n2);
            Assert.Equal(33, _db.Query<SimpleUser>().Select().First(u => u.Name == "丙").Age);
        }

        [Fact]
        public void Update_WhereLike_And_IfTrueWhereLike()
        {
            var target = _db.Query<SimpleUser>().Select().First(u => u.Name == "甲");
            target.Age = 12;
            var n1 = _db.UpdateMany<SimpleUser>().WhereLike(x => x.Name, "甲").Execute(target);
            Assert.Equal(1, n1);

            var target2 = _db.Query<SimpleUser>().Select().First(u => u.Name == "乙");
            target2.Age = 22;
            var n2 = _db.UpdateMany<SimpleUser>().IfTrueWhereLike(true, "Name", "乙").Execute(target2);
            Assert.Equal(1, n2);

            Assert.Equal(12, _db.Query<SimpleUser>().Select().First(u => u.Name == "甲").Age);
            Assert.Equal(22, _db.Query<SimpleUser>().Select().First(u => u.Name == "乙").Age);
        }

        [Fact]
        public void Update_WhereNotIn_And_WhereExists()
        {
            var target = new SimpleUser { Name = "乙", Age = 21 };
            var n1 = _db.UpdateMany<SimpleUser>().WhereNotIn(x => x.Id, new[] { 1, 3 }).Execute(target);
            Assert.Equal(1, n1);
            Assert.Equal(21, _db.Query<SimpleUser>().Select().First(u => u.Id == 2).Age);

            // WhereExists 为真时更新所有匹配行（此时表中 3 行均满足 EXISTS）
            var target2 = new SimpleUser { Name = "甲", Age = 13 };
            var n2 = _db.UpdateMany<SimpleUser>()
                .WhereExists("SELECT 1 FROM SimpleUser WHERE Age = 21")
                .Execute(target2);
            Assert.Equal(3, n2);
            Assert.Equal(13, _db.Query<SimpleUser>().Select().First().Age);
        }

        [Fact]
        public void Update_WhereLikeStart_And_WhereLikeEnd()
        {
            var target = _db.Query<SimpleUser>().Select().First(u => u.Name == "乙");
            target.Age = 24;
            var n1 = _db.UpdateMany<SimpleUser>().WhereLikeStart(x => x.Name, "乙").Execute(target);
            Assert.Equal(1, n1);

            var target2 = _db.Query<SimpleUser>().Select().First(u => u.Name == "丙");
            target2.Age = 34;
            var n2 = _db.UpdateMany<SimpleUser>().WhereLikeEnd(x => x.Name, "丙").Execute(target2);
            Assert.Equal(1, n2);

            Assert.Equal(24, _db.Query<SimpleUser>().Select().First(u => u.Name == "乙").Age);
            Assert.Equal(34, _db.Query<SimpleUser>().Select().First(u => u.Name == "丙").Age);
        }

        [Fact]
        public void Update_Chain_AllDynamicConditions()
        {
            var target = new SimpleUser { Id = 1, Name = "甲", Age = 14 };
            var n = _db.UpdateMany<SimpleUser>()
                .IfTrueWhere(true, x => x.Age >= 10)
                .WhereIn(x => x.Id, new[] { 1, 2, 3 })
                .IfTrueWhereLike(true, x => x.Name, "甲")
                .WhereNotIn(x => x.Age, new[] { 99 })
                .WhereLikeStart(x => x.Name, "甲")
                .IfTrueWhereNotExists(true, "SELECT 1 FROM SimpleUser WHERE Age > 100")
                .Execute(target);
            Assert.Equal(1, n);
            Assert.Equal(14, _db.Query<SimpleUser>().Select().First(u => u.Name == "甲").Age);
        }

        [Fact]
        public async Task UpdateManyAsync_Works()
        {
            var target = _db.Query<SimpleUser>().Select().First(u => u.Name == "乙");
            target.Age = 88;
            var n = await _db.UpdateManyAsync<SimpleUser>()
                .IfTrueWhere(true, x => x.Name == "乙")
                .WhereIn(x => x.Age, new[] { 20 })
                .Execute(target);
            Assert.Equal(1, n);
            Assert.Equal(88, _db.Query<SimpleUser>().Select().First(u => u.Name == "乙").Age);
        }

        #endregion 更新查询器 UpdateMany
    }
}
