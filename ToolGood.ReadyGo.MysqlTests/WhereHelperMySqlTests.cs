using Xunit;

namespace ToolGood.ReadyGo.MysqlTests
{
    /// <summary>
    /// SqlHelper.Where 链式查询、UpdateMany、DeleteMany 测试（基于 MySQL）
    /// </summary>
    [Collection("MySql")]
    public class WhereHelperMySqlTests
    {
        private static MySqlTestDb CreateWithUsers()
        {
            var db = MySqlTestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.DropTable(typeof(Tb_WhereTest));
            helper.TableHelper.TryCreateTable(typeof(Tb_WhereTest));
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

            Assert.Equal(2, helper.Where<Tb_WhereTest>().WhereLike(q => q.Name, "张").ToList().Count);
            Assert.Single(helper.Where<Tb_WhereTest>().WhereLikeStart(q => q.Name, "张三").ToList());
            Assert.Single(helper.Where<Tb_WhereTest>().WhereLikeEnd(q => q.Name, "王五").ToList());
        }

        [Fact]
        public void If_判断_跳过()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var age = 0;
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

            var list = helper.Where<Tb_WhereTest>()
                .WhereExists("SELECT 1 FROM Tb_WhereTest t2 WHERE t2.Age > 40 AND t2.Id = TT.Id")
                .ToList();
            Assert.Single(list);

            var list2 = helper.Where<Tb_WhereTest>()
                .WhereNotExists("SELECT 1 FROM Tb_WhereTest t2 WHERE t2.Age > 40 AND t2.Id = TT.Id")
                .ToList();
            Assert.Equal(3, list2.Count);
        }
    }
}
