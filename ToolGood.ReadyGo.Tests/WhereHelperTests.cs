using System.Collections.Generic;
using System.Linq;
using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    [Table("Tb_WhereTest")]
    [PrimaryKey("Id")]
    public class Tb_WhereTest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Vip { get; set; }
    }

    /// <summary>
    /// SqlHelper.Where（Core/Linq QueryProvider 链式查询）、UpdateMany、DeleteMany 测试
    /// </summary>
    public class WhereHelperTests
    {
        private static TestDb CreateWithUsers()
        {
            var db = TestDb.Create();
            var helper = db.Helper;
            helper._TableHelper.TryCreateTable(typeof(Tb_WhereTest));
            helper.Insert(new Tb_WhereTest { Name = "张三", Age = 20, Vip = false });
            helper.Insert(new Tb_WhereTest { Name = "李四", Age = 30, Vip = true });
            helper.Insert(new Tb_WhereTest { Name = "王五", Age = 40, Vip = true });
            helper.Insert(new Tb_WhereTest { Name = "张伟", Age = 50, Vip = false });
            return db;
        }

        [Fact]
        public void Where_字符串参数_查询()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>("Age > @0", 30).OrderBy(q => q.Age).ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("王五", list[0].Name);
            Assert.Equal("张伟", list[1].Name);
        }

        [Fact]
        public void Where_表达式_比较_AND()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>(q => q.Age >= 30 && q.Vip).ToList();
            Assert.Equal(2, list.Count);

            var one = helper.Where<Tb_WhereTest>(q => q.Name == "张三").FirstOrDefault();
            Assert.NotNull(one);
            Assert.Equal(20, one.Age);
        }

        [Fact]
        public void Where_Contains_Like()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>(q => q.Name.Contains("张")).ToList();
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void WhereIn_WhereNotIn()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>().WhereIn(q => q.Age, new int[] { 20, 50 }).ToList();
            Assert.Equal(2, list.Count);

            var list2 = helper.Where<Tb_WhereTest>().WhereNotIn(q => q.Age, new int[] { 20, 50 }).ToList();
            Assert.Equal(2, list2.Count);

            var list3 = helper.Where<Tb_WhereTest>("Age").WhereIn("Age", new object[] { 30, 40 }).ToList();
            Assert.Equal(2, list3.Count);
        }

        [Fact]
        public void WhereLike_系列()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            // WhereLike = '%args%'，WhereLikeStart = 'args%'（前缀匹配），WhereLikeEnd = '%args'（后缀匹配）
            Assert.Equal(2, helper.Where<Tb_WhereTest>().WhereLike(q => q.Name, "张").ToList().Count);
            Assert.Single(helper.Where<Tb_WhereTest>().WhereLikeStart(q => q.Name, "张三").ToList());
            Assert.Single(helper.Where<Tb_WhereTest>().WhereLikeEnd(q => q.Name, "王五").ToList());
        }

        [Fact]
        public void WhereNotLike_系列()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            // WhereNotLike = NOT LIKE '%args%'，WhereNotLikeStart = NOT LIKE 'args%'（非前缀），WhereNotLikeEnd = NOT LIKE '%args'（非后缀）
            Assert.Equal(2, helper.Where<Tb_WhereTest>().WhereNotLike(q => q.Name, "张").ToList().Count);
            Assert.Equal(2, helper.Where<Tb_WhereTest>().WhereNotLikeStart(q => q.Name, "张").ToList().Count);
            Assert.Equal(3, helper.Where<Tb_WhereTest>().WhereNotLikeEnd(q => q.Name, "伟").ToList().Count);

            // 字符串列名版本
            Assert.Equal(3, helper.Where<Tb_WhereTest>().WhereNotLike("Name", "张三").ToList().Count);
            Assert.Equal(2, helper.Where<Tb_WhereTest>().WhereNotLikeStart("Name", "张").ToList().Count);
            Assert.Equal(3, helper.Where<Tb_WhereTest>().WhereNotLikeEnd("Name", "五").ToList().Count);

            // IfTrue 生效 / 跳过
            Assert.Equal(2, helper.Where<Tb_WhereTest>().IfTrueWhereNotLike(true, q => q.Name, "张").ToList().Count);
            Assert.Equal(4, helper.Where<Tb_WhereTest>().IfTrueWhereNotLike(false, q => q.Name, "张").ToList().Count);
            Assert.Equal(2, helper.Where<Tb_WhereTest>().IfTrueWhereNotLikeStart(true, "Name", "张").ToList().Count);
            Assert.Equal(4, helper.Where<Tb_WhereTest>().IfTrueWhereNotLikeEnd(false, "Name", "伟").ToList().Count);
        }

        [Fact]
        public void If_判断_跳过()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var age = 0;
            // age <= 0 时条件为假，Where/OrderBy 全部跳过
            var list = helper.Where<Tb_WhereTest>()
                .IfTrueWhere(age > 0, q => q.Age > 0)
                .IfTrueOrderBy(age > 0, q => q.Age)
                .ToList();
            Assert.Equal(4, list.Count);

            var list2 = helper.Where<Tb_WhereTest>()
                .IfTrueWhere(30 > 0, q => q.Age > 40)
                .ToList();
            Assert.Single(list2);
        }

        [Fact]
        public void OrderBy_Distinct()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>().OrderByDescending(q => q.Age).ToList();
            Assert.Equal("张伟", list[0].Name);
            Assert.Equal("张三", list[3].Name);

            var distinct = helper.Where<Tb_WhereTest>().Distinct(q => new { q.Age }).ToList();
            Assert.Equal(4, distinct.Count);
        }

        [Fact]
        public void SelectCount()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            Assert.Equal(4, helper.Where<Tb_WhereTest>().Count());
            Assert.Equal(2, helper.Where<Tb_WhereTest>(q => q.Age > 30).Count());
        }

        [Fact]
        public void Page_SelectPage()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var page = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).ToPage(1, 2);
            Assert.Equal(4, page.TotalItems);
            Assert.Equal(2, page.Items.Count);
            Assert.Equal("张三", page.Items[0].Name);

            var list = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).ToPage(2, 2).Items;
            Assert.Equal(2, list.Count);
            Assert.Equal("王五", list[0].Name);
        }

        [Fact]
        public void Select_匿名列()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>(q => q.Age > 30).OrderBy(q => q.Age)
                .ProjectTo(q => new { q.Name, q.Age });
            Assert.Equal(2, list.Count);
            Assert.Equal("王五", list[0].Name);
            Assert.Equal(40, list[0].Age);
        }

        [Fact]
        public void UpdateMany_对象()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var r1 = helper.UpdateMany<Tb_WhereTest>()
                .Where(q => q.Name == "张三")
                .ExcludeDefaults()
                .Execute(new Tb_WhereTest { Age = 21 });
            Assert.Equal(1, r1);
            Assert.Equal(21, helper.Where<Tb_WhereTest>(q => q.Name == "张三").FirstOrDefault().Age);
        }

        [Fact]
        public void DeleteMany()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var r = helper.DeleteMany<Tb_WhereTest>().Where(q => q.Age == 20).Execute();
            Assert.Equal(1, r);
            Assert.Equal(3, helper.Where<Tb_WhereTest>().Count());
        }

        [Fact]
        public void Contains_IsIn()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;
            var ids = new[] { 20, 50 };

            var list = helper.Where<Tb_WhereTest>(q => ids.Contains(q.Age)).ToList();
            Assert.Equal(2, list.Count);

            var list2 = helper.Where<Tb_WhereTest>(q => !ids.Contains(q.Age)).ToList();
            Assert.Equal(2, list2.Count);
        }

        [Fact]
        public void WhereExists_WhereNotExists()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            // 关联子查询（NPOCO 主表别名 TT = 类型名首字母缩写）
            var list = helper.Where<Tb_WhereTest>()
                .WhereExists("SELECT 1 FROM Tb_WhereTest t2 WHERE t2.Age > 40 AND t2.Id = TT.Id")
                .ToList();
            Assert.Single(list);

            var list2 = helper.Where<Tb_WhereTest>()
                .WhereNotExists("SELECT 1 FROM Tb_WhereTest t2 WHERE t2.Age > 40 AND t2.Id = TT.Id")
                .ToList();
            Assert.Equal(3, list2.Count);
        }

        [Fact]
        public void Exists_无参数与条件()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            Assert.True(helper.Where<Tb_WhereTest>().Exists());
            Assert.True(helper.Where<Tb_WhereTest>(q => q.Age > 30).Exists());
            Assert.False(helper.Where<Tb_WhereTest>(q => q.Age > 100).Exists());

            // Exists 是 Any 的别名，结果应一致
            Assert.Equal(helper.Where<Tb_WhereTest>().Any(), helper.Where<Tb_WhereTest>().Exists());
        }

        [Fact]
        public async Task Exists_Async()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            Assert.True(await helper.Where<Tb_WhereTest>().Exists_Async());
            Assert.True(await helper.Where<Tb_WhereTest>(q => q.Age > 30).Exists_Async());
            Assert.False(await helper.Where<Tb_WhereTest>(q => q.Age > 100).Exists_Async());
        }

        [Fact]
        public void SelectPage_分页返回当前页列表()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var page1 = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).SelectPage(1, 2);
            Assert.Equal(2, page1.Count);
            Assert.Equal("张三", page1[0].Name);

            var page2 = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).SelectPage(2, 2);
            Assert.Equal(2, page2.Count);
            Assert.Equal("王五", page2[0].Name);
        }

        [Fact]
        public async Task SelectPage_Async()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var page1 = await helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).SelectPage_Async(1, 2);
            Assert.Equal(2, page1.Count);
            Assert.Equal("张三", page1[0].Name);

            var page2 = await helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).SelectPage_Async(2, 2);
            Assert.Equal(2, page2.Count);
            Assert.Equal("王五", page2[0].Name);
        }

        [Fact]
        public void ToPage_参数校验_页码与每页大小()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            // page <= 0 修正为 1，pageSize <= 0 修正为 10
            var p1 = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).ToPage(0, 2);
            Assert.Equal(1, p1.CurrentPage);
            Assert.Equal(2, p1.Items.Count);
            Assert.Equal("张三", p1.Items[0].Name);

            var p2 = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).ToPage(-1, 0);
            Assert.Equal(1, p2.CurrentPage);
            Assert.Equal(10, p2.PageSize);
            Assert.Equal(4, p2.Items.Count);
        }

        [Fact]
        public async Task ToPage_Async_参数校验()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var p1 = await helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).ToPage_Async(0, 2);
            Assert.Equal(1, p1.CurrentPage);
            Assert.Equal(2, p1.Items.Count);
            Assert.Equal("张三", p1.Items[0].Name);

            var p2 = await helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).ToPage_Async(1, 0);
            Assert.Equal(10, p2.PageSize);
            Assert.Equal(4, p2.Items.Count);
        }

        [Fact]
        public void SelectPage_参数校验()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            // 修正为第一页且 pageSize=10，返回全部数据
            var list = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).SelectPage(0, 0);
            Assert.Equal(4, list.Count);
            Assert.Equal("张三", list[0].Name);
        }

        [Fact]
        public async Task Async_后缀方法_回归测试()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            // AsyncQueryProvider 的 _Async 后缀方法经同步链式（IQueryProvider）调用验证
            var all = await helper.Where<Tb_WhereTest>().ToList_Async();
            Assert.Equal(4, all.Count);

            Assert.Equal(4, await helper.Where<Tb_WhereTest>().Count_Async());
            Assert.Equal(2, await helper.Where<Tb_WhereTest>(q => q.Age > 30).Count_Async());
            Assert.Equal(4, await helper.Where<Tb_WhereTest>().SelectCount_Async());

            Assert.True(await helper.Where<Tb_WhereTest>().Any_Async());
            Assert.False(await helper.Where<Tb_WhereTest>(q => q.Age > 100).Any_Async());

            var one = await helper.Where<Tb_WhereTest>(q => q.Name == "张三").FirstOrDefault_Async();
            Assert.Equal(20, one.Age);

            var first = await helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).First_Async();
            Assert.Equal("张三", first.Name);

            var page = await helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).ToPage_Async(1, 2);
            Assert.Equal(4, page.TotalItems);
            Assert.Equal(2, page.Items.Count);
            Assert.Equal("张三", page.Items[0].Name);

            var page2 = await helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).Page_Async(2, 2);
            Assert.Equal("王五", page2.Items[0].Name);

            var projected = await helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).ProjectTo_Async(q => new { q.Name, q.Age });
            Assert.Equal(4, projected.Count);
        }
    }
}
