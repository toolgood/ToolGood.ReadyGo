using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using ToolGood.ReadyGo.NPoco;
using Xunit;
using NPocoDatabase = ToolGood.ReadyGo.NPoco.Database;

namespace ToolGood.ReadyGo.Tests
{
    /// <summary>
    /// Core/Linq IQueryProvider<T> 动态条件方法测试（SQLite）
    /// </summary>
    public class CoreLinqExtensionsTests : IDisposable
    {
        private readonly SqliteConnection _conn;
        private readonly NPocoDatabase _db;

        public CoreLinqExtensionsTests()
        {
            var dbFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"readygo_linq_{Guid.NewGuid():N}.db");
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

        [Fact]
        public void IfTrueWhere_AppliesWhenTrue_SkipWhenFalse()
        {
            var whenTrue = _db.Query<SimpleUser>().IfTrueWhere(true, x => x.Age > 20).ToList();
            Assert.Single(whenTrue);

            var whenFalse = _db.Query<SimpleUser>().IfTrueWhere(false, x => x.Age > 20).ToList();
            Assert.Equal(3, whenFalse.Count);
        }

        [Fact]
        public void WhereIn_ExpressionAndString()
        {
            var byExpr = _db.Query<SimpleUser>().WhereIn(x => x.Age, new[] { 10, 30 }).ToList();
            Assert.Equal(2, byExpr.Count);

            var byString = _db.Query<SimpleUser>().WhereIn("Age", new[] { 20 }).ToList();
            Assert.Single(byString);
            Assert.Equal("乙", byString[0].Name);

            var empty = _db.Query<SimpleUser>().WhereIn(x => x.Age, new int[0]).ToList();
            Assert.Empty(empty);
        }

        [Fact]
        public void IfTrueWhereIn_Works()
        {
            var applied = _db.Query<SimpleUser>().IfTrueWhereIn(true, x => x.Name, new[] { "甲", "丙" }).ToList();
            Assert.Equal(2, applied.Count);

            var skipped = _db.Query<SimpleUser>().IfTrueWhereIn(false, x => x.Name, new[] { "甲" }).ToList();
            Assert.Equal(3, skipped.Count);
        }

        [Fact]
        public void WhereLike_And_IfTrueWhereLike()
        {
            var like = _db.Query<SimpleUser>().WhereLike(x => x.Name, "甲").ToList();
            Assert.Single(like);

            var skip = _db.Query<SimpleUser>().IfTrueWhereLike(false, x => x.Name, "乙").ToList();
            Assert.Equal(3, skip.Count);

            var apply = _db.Query<SimpleUser>().IfTrueWhereLike(true, "Name", "乙").ToList();
            Assert.Single(apply);
        }

        [Fact]
        public void WhereExists_And_WhereNotExists()
        {
            // Exists：存在 Age>25 的记录，返回全部
            var exists = _db.Query<SimpleUser>()
                .WhereExists("SELECT 1 FROM SimpleUser WHERE Age > 25")
                .ToList();
            Assert.Equal(3, exists.Count);

            // NotExists：不存在 Age>100 的记录，返回全部
            var notExists = _db.Query<SimpleUser>()
                .WhereNotExists("SELECT 1 FROM SimpleUser WHERE Age > 100")
                .ToList();
            Assert.Equal(3, notExists.Count);

            // Exists 带参数
            var withArgs = _db.Query<SimpleUser>()
                .WhereExists("SELECT 1 FROM SimpleUser WHERE Age = @0", 30)
                .ToList();
            Assert.Equal(3, withArgs.Count);
        }

        [Fact]
        public void IfTrueWhereExists_Works()
        {
            var applied = _db.Query<SimpleUser>()
                .IfTrueWhereExists(true, "SELECT 1 FROM SimpleUser WHERE Age > 25")
                .ToList();
            Assert.Equal(3, applied.Count);

            var skipped = _db.Query<SimpleUser>()
                .IfTrueWhereNotExists(false, "SELECT 1 FROM SimpleUser WHERE Age > 25")
                .ToList();
            Assert.Equal(3, skipped.Count);
        }

        [Fact]
        public void IfTrueOrderBy_And_IfTrueOrderByDescending()
        {
            var asc = _db.Query<SimpleUser>()
                .IfTrueOrderBy(true, x => x.Age)
                .ToList();
            Assert.Equal(new[] { 10, 20, 30 }, asc.Select(x => x.Age).ToArray());

            var desc = _db.Query<SimpleUser>()
                .IfTrueOrderByDescending(true, x => x.Age)
                .ToList();
            Assert.Equal(new[] { 30, 20, 10 }, desc.Select(x => x.Age).ToArray());

            var skipped = _db.Query<SimpleUser>()
                .IfTrueOrderBy(false, x => x.Age)
                .ToList();
            Assert.Equal(3, skipped.Count);
        }

        [Fact]
        public void IfTrueLimit_Works()
        {
            var limited = _db.Query<SimpleUser>()
                .IfTrueOrderBy(true, x => x.Age)
                .IfTrueLimit(true, 2)
                .ToList();
            Assert.Equal(new[] { 10, 20 }, limited.Select(x => x.Age).ToArray());

            var skipped = _db.Query<SimpleUser>().IfTrueLimit(false, 2).ToList();
            Assert.Equal(3, skipped.Count);

            var paged = _db.Query<SimpleUser>()
                .IfTrueOrderBy(true, x => x.Age)
                .IfTrueLimit(true, 2, 1)
                .ToList();
            Assert.Equal(new[] { 20, 30 }, paged.Select(x => x.Age).ToArray());
        }

        [Fact]
        public void Select_EqualsToList()
        {
            var list = _db.Query<SimpleUser>().Select();
            Assert.Equal(3, list.Count);

            var withFilter = _db.Query<SimpleUser>().IfTrueWhere(true, x => x.Age > 20).Select();
            Assert.Single(withFilter);
        }

        [Fact]
        public void Chain_AllExtensions()
        {
            var list = _db.Query<SimpleUser>()
                .IfTrueWhere(true, x => x.Age >= 10)
                .WhereIn(x => x.Age, new[] { 10, 20, 30 })
                .IfTrueWhereLike(true, x => x.Name, "乙")
                .IfTrueOrderBy(true, x => x.Age)
                .IfTrueLimit(true, 5)
                .ToList();
            Assert.Single(list);
            Assert.Equal("乙", list[0].Name);
        }
    }
}
