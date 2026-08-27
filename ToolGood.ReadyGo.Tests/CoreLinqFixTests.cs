using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.NPoco;
using Xunit;
using NPocoDatabase = ToolGood.ReadyGo.NPoco.Database;

namespace ToolGood.ReadyGo.Tests
{
    /// <summary>
    /// 一对多分页防护测试模型：显式声明 Many 引用，确保 IncludeMany 能成功构建关联表达式。
    /// </summary>
    [Table("PagedCustomer")]
    [PrimaryKey("Id")]
    public class PagedCustomer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Reference(ReferenceType.Many, ReferenceMemberName = "CustomerId")]
        public List<PagedOrder> Orders { get; set; }
    }

    [Table("PagedOrder")]
    public class PagedOrder
    {
        public int OrderItemId { get; set; }

        public int CustomerId { get; set; }

        public string Product { get; set; }
    }

    /// <summary>
    /// Core/Linq 缺陷修复验证测试（SQLite）。
    /// 覆盖：一对多分页防护、LIKE 通配符转义、列名标识符转义、
    /// 引用成员错误提示、From 判空、QueryContext.PocoData 暴露。
    /// </summary>
    public class CoreLinqFixTests : IDisposable
    {
        private readonly SqliteConnection _conn;
        private readonly NPocoDatabase _db;

        public CoreLinqFixTests()
        {
            var dbFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"readygo_fix_{Guid.NewGuid():N}.db");
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

        #region 修复 #1：一对多分页防护

        [Fact]
        public void IncludeMany_ThenToPage_ThrowsNotImplementedException()
        {
            var ex = Assert.Throws<NotImplementedException>(() =>
            {
                _db.Query<PagedCustomer>().IncludeMany(x => x.Orders).ToPage(1, 10);
            });
            Assert.Contains("One to many", ex.Message);
        }

        [Fact]
        public void IncludeMany_ThenLimit_ThrowsNotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() =>
            {
                _db.Query<PagedCustomer>().IncludeMany(x => x.Orders).Limit(10);
            });
        }

        #endregion

        #region 修复 #3：LIKE 通配符转义

        [Fact]
        public void WhereLike_PercentLiteral_IsEscaped()
        {
            Insert("100%纯", 40);
            Insert("100纯", 50);

            // 修复前 % 会被当作通配符，导致 "100纯" 也被匹配；修复后仅匹配字面量 100%
            var result = _db.Query<SimpleUser>().WhereLike(x => x.Name, "100%").ToList();
            Assert.Single(result);
            Assert.Equal("100%纯", result[0].Name);
        }

        [Fact]
        public void WhereLike_UnderscoreLiteral_IsEscaped()
        {
            Insert("a_b", 60);
            Insert("acb", 70);

            // 修复前 _ 会被当作单字符通配符，导致 "acb" 也被匹配；修复后仅匹配字面量 a_b
            var result = _db.Query<SimpleUser>().WhereLike(x => x.Name, "a_b").ToList();
            Assert.Single(result);
            Assert.Equal("a_b", result[0].Name);
        }

        #endregion

        #region 修复 #2：列名标识符转义

        [Fact]
        public void WhereIn_AlreadyEscapedColumnName_IsNotDoubleEscaped()
        {
            // 已用方括号包裹的标识符应原样返回，不应被二次包裹为 [[Name]]
            var result = _db.Query<SimpleUser>().WhereIn("[Name]", new[] { "甲" }).ToList();
            Assert.Single(result);
            Assert.Equal("甲", result[0].Name);
        }

        #endregion

        #region 修复 #6：引用成员错误提示

        [Fact]
        public void Include_NonReferenceMember_ThrowsFriendlyArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                _db.Query<SimpleUser>().Include<string>(x => x.Name);
            });
            Assert.Contains("SimpleUser", ex.Message);
            Assert.Contains("Name", ex.Message);
        }

        #endregion

        #region 修复 #9：From 判空

        [Fact]
        public void From_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _db.Query<SimpleUser>().From(null);
            });
        }

        #endregion

        #region 修复 #8：QueryContext.PocoData 暴露

        [Fact]
        public void WhereSql_QueryContext_ExposesPocoData()
        {
            var result = _db.Query<SimpleUser>()
                .WhereSql(ctx =>
                {
                    Assert.NotNull(ctx.PocoData);
                    Assert.Equal(typeof(SimpleUser), ctx.PocoData.Type);
                    return new Sql("1 = 1");
                })
                .ToList();

            Assert.Equal(3, result.Count);
        }

        #endregion
    }
}
