using System.Collections.Generic;
using System.Linq;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.LinQ;
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
    /// WhereHelper 动态SQL拼接
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

            var list = helper.Where<Tb_WhereTest>("Age > @0", 30).OrderBy(q => q.Age).Select();
            Assert.Equal(2, list.Count);
            Assert.Equal("王五", list[0].Name);
            Assert.Equal("张伟", list[1].Name);
        }

        [Fact]
        public void Where_表达式_比较_AND()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>(q => q.Age >= 30 && q.Vip).Select();
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

            var list = helper.Where<Tb_WhereTest>(q => q.Name.Contains("张")).Select();
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void WhereIn_WhereNotIn()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>().WhereIn(q => q.Age, new int[] { 20, 50 }).Select();
            Assert.Equal(2, list.Count);

            var list2 = helper.Where<Tb_WhereTest>().WhereNotIn(q => q.Age, new int[] { 20, 50 }).Select();
            Assert.Equal(2, list2.Count);

            var list3 = helper.Where<Tb_WhereTest>("Age").WhereIn("Age", new object[] { 30, 40 }).Select();
            Assert.Equal(2, list3.Count);
        }

        [Fact]
        public void WhereLike_系列()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            // 旧库语义：WhereLike = '%args%'，WhereLikeStart = '%args'（右匹配），WhereLikeEnd = 'args%'（左匹配）
            Assert.Equal(2, helper.Where<Tb_WhereTest>().WhereLike(q => q.Name, "张").Select().Count);
            Assert.Single(helper.Where<Tb_WhereTest>().WhereLikeStart(q => q.Name, "张三").Select());
            Assert.Single(helper.Where<Tb_WhereTest>().WhereLikeEnd(q => q.Name, "王五").Select());
        }

        [Fact]
        public void If_判断_跳过()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var age = 0;
            // age <= 0 时 IfPositiveInteger 为假，Where/OrderBy 全部跳过
            var list = helper.Where<Tb_WhereTest>()
                .IfPositiveInteger(age)
                .Where(q => q.Age > 0)
                .OrderBy(q => q.Age)
                .Select();
            Assert.Equal(4, list.Count);

            var list2 = helper.Where<Tb_WhereTest>()
                .IfPositiveInteger(30)
                .Where(q => q.Age > 40)
                .Select();
            Assert.Single(list2);
        }

        [Fact]
        public void OrderBy_Distinct()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age, OrderType.Desc).Select();
            Assert.Equal("张伟", list[0].Name);
            Assert.Equal("张三", list[3].Name);
        }

        [Fact]
        public void AddColumn_RemoveColumn()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var sql = helper.Where<Tb_WhereTest>()
                .RemoveColumn(q => q.Vip)
                .AddColumn(q => q.Age, "Age2")
                .GetFullSelectSql();
            Assert.Contains("Age2", sql);
            Assert.DoesNotContain("Vip", sql);
        }

        [Fact]
        public void SelectCount()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            Assert.Equal(4, helper.Where<Tb_WhereTest>().SelectCount());
            Assert.Equal(2, helper.Where<Tb_WhereTest>(q => q.Age > 30).SelectCount());
        }

        [Fact]
        public void Page_SelectPage()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var page = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).Page(1, 2);
            Assert.Equal(4, page.TotalItems);
            Assert.Equal(2, page.Items.Count);
            Assert.Equal("张三", page.Items[0].Name);

            var list = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).SelectPage(2, 2);
            Assert.Equal(2, list.Count);
            Assert.Equal("王五", list[0].Name);
        }

        [Fact]
        public void Select_匿名列()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>(q => q.Age > 30).OrderBy(q => q.Age)
                .Select(q => new { q.Name, q.Age });
            Assert.Equal(2, list.Count);
            Assert.Equal("王五", list[0].Name);
            Assert.Equal(40, list[0].Age);
        }

        [Fact]
        public void Select_T_指定列()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>(q => q.Age > 30)
                .Select<Tb_WhereTest>("Name, Age");
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void GroupBy_Having()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var dt = helper.Where<Tb_WhereTest>()
                .GroupBy(q => q.Age)
                .Having("COUNT(1) >= 1")
                .ExecuteDataTable("Age, COUNT(1) AS C");
            Assert.Equal(4, dt.Rows.Count);
        }

        [Fact]
        public void Update_对象_字典_Sql()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var r1 = helper.Where<Tb_WhereTest>(q => q.Name == "张三").Update(new { Age = 21 });
            Assert.Equal(1, r1);
            Assert.Equal(21, helper.Where<Tb_WhereTest>(q => q.Name == "张三").FirstOrDefault().Age);

            var r2 = helper.Where<Tb_WhereTest>(q => q.Name == "李四").Update(new Dictionary<string, object> { ["Vip"] = false });
            Assert.Equal(1, r2);
            Assert.False(helper.Where<Tb_WhereTest>(q => q.Name == "李四").FirstOrDefault().Vip);

            var r3 = helper.Where<Tb_WhereTest>(q => q.Name == "王五").Update("Age = Age + 1");
            Assert.Equal(1, r3);
            Assert.Equal(41, helper.Where<Tb_WhereTest>(q => q.Name == "王五").FirstOrDefault().Age);
        }

        [Fact]
        public void Delete()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var r = helper.Where<Tb_WhereTest>(q => q.Age == 20).Delete();
            Assert.Equal(1, r);
            Assert.Equal(3, helper.Where<Tb_WhereTest>().SelectCount());
        }

        [Fact]
        public void GetFullSelectSql_GetArgs()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var wh = helper.Where<Tb_WhereTest>("Age > @0 AND Age < @1", 25, 45);
            var sql = wh.GetFullSelectSql();
            Assert.Contains("Age > @0", sql);
            Assert.Equal(new object[] { 25, 45 }, wh.GetArgs());
        }

        [Fact]
        public void ObjectExtend_IsIn()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>(q => q.Age.IsIn(new[] { 20, 50 })).Select();
            Assert.Equal(2, list.Count);

            var list2 = helper.Where<Tb_WhereTest>(q => q.Age.IsNotIn(new[] { 20, 50 })).Select();
            Assert.Equal(2, list2.Count);
        }

        [Fact]
        public void WhereExists_WhereNotExists()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>()
                .WhereExists("SELECT 1 FROM Tb_WhereTest t2 WHERE t2.Age > 40 AND t2.Id = t1.Id")
                .Select();
            Assert.Single(list);

            var list2 = helper.Where<Tb_WhereTest>()
                .WhereNotExists("SELECT 1 FROM Tb_WhereTest t2 WHERE t2.Age > 40 AND t2.Id = t1.Id")
                .Select();
            Assert.Equal(3, list2.Count);
        }
    }
}
