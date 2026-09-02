using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    /// <summary>
    /// SqlHelper.Where.cs 入口方法单元测试（基于 SQLite）：
    /// 覆盖 Where<T>() 四种重载、UpdateMany、DeleteMany 的入口行为、参数校验，
    /// 以及链式返回的 Provider 上重命名后的 *_Async 异步方法。
    /// </summary>
    public class SqlHelperWhereEntryTests
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

        #region Where 入口重载

        [Fact]
        public void Where_无参_返回全部()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>().ToList();
            Assert.Equal(4, list.Count);
        }

        [Fact]
        public void Where_字符串_无参数()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>("Age > 40").ToList();
            Assert.Single(list);
            Assert.Equal("张伟", list[0].Name);
        }

        [Fact]
        public void Where_字符串_多参数()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>("Age > @0 AND Name = @1", 20, "王五").ToList();
            Assert.Single(list);
            Assert.Equal(40, list[0].Age);
        }

        [Fact]
        public void Where_表达式_入口()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>(q => q.Age >= 30 && q.Vip).ToList();
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void Where_表达式_之后可继续链式()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = helper.Where<Tb_WhereTest>(q => q.Age >= 30)
                .Where(q => q.Vip)
                .OrderBy(q => q.Age)
                .ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("李四", list[0].Name);
        }

        #endregion

        #region 参数校验

        [Fact]
        public void Where_字符串_null_抛异常()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            Assert.Throws<ArgumentNullException>(() => helper.Where<Tb_WhereTest>(null as string));
        }

        [Fact]
        public void Where_字符串_空_抛异常()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            Assert.Throws<ArgumentNullException>(() => helper.Where<Tb_WhereTest>(""));
        }

        [Fact]
        public void Where_表达式_null_抛异常()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            Assert.Throws<ArgumentNullException>(() => helper.Where<Tb_WhereTest>(null as System.Linq.Expressions.Expression<Func<Tb_WhereTest, bool>>));
        }

        #endregion

        #region UpdateMany

        [Fact]
        public void UpdateMany_OnlyFields_只更新指定字段()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var r = helper.UpdateMany<Tb_WhereTest>()
                .Where(q => q.Name == "张三")
                .OnlyFields(q => new { q.Age })
                .Execute(new Tb_WhereTest { Name = "被忽略的名字", Age = 99 });
            Assert.Equal(1, r);

            var u = helper.Where<Tb_WhereTest>(q => q.Name == "张三").FirstOrDefault();
            Assert.NotNull(u);
            Assert.Equal(99, u.Age);
            Assert.Equal("张三", u.Name); // 未指定字段保持原值
        }

        [Fact]
        public void UpdateMany_ExcludeDefaults_跳过默认值字段()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var r = helper.UpdateMany<Tb_WhereTest>()
                .Where(q => q.Name == "李四")
                .ExcludeDefaults()
                .Execute(new Tb_WhereTest { Name = "李四", Vip = false });
            Assert.Equal(1, r);

            var u = helper.Where<Tb_WhereTest>(q => q.Name == "李四").FirstOrDefault();
            Assert.NotNull(u);
            Assert.True(u.Vip); // Vip=false 为默认值被跳过，保持原值 true
        }

        [Fact]
        public void UpdateMany_无条件_更新全部()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var r = helper.UpdateMany<Tb_WhereTest>()
                .OnlyFields(q => new { q.Vip })
                .Execute(new Tb_WhereTest { Vip = true });
            Assert.Equal(4, r);
            Assert.Equal(4, helper.Where<Tb_WhereTest>().Count(q => q.Vip));
        }

        #endregion

        #region DeleteMany

        [Fact]
        public void DeleteMany_条件_删除匹配行()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var r = helper.DeleteMany<Tb_WhereTest>().Where(q => q.Vip).Execute();
            Assert.Equal(2, r);
            Assert.Equal(2, helper.Where<Tb_WhereTest>().Count());
        }

        [Fact]
        public void DeleteMany_无条件_删除全部()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var r = helper.DeleteMany<Tb_WhereTest>().Execute();
            Assert.Equal(4, r);
            Assert.Equal(0, helper.Where<Tb_WhereTest>().Count());
        }

        #endregion

        #region 重命名后的异步方法（*_Async）

        [Fact]
        public async Task Where_ToList_Async()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var list = await helper.Where<Tb_WhereTest>(q => q.Age >= 30).ToList_Async();
            Assert.Equal(3, list.Count);
        }

        [Fact]
        public async Task Where_FirstOrDefault_Async()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var u = await helper.Where<Tb_WhereTest>(q => q.Name == "王五").FirstOrDefault_Async();
            Assert.NotNull(u);
            Assert.Equal(40, u.Age);

            Assert.Null(await helper.Where<Tb_WhereTest>(q => q.Name == "不存在").FirstOrDefault_Async());
        }

        [Fact]
        public async Task Where_Count_Async()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            Assert.Equal(4, await helper.Where<Tb_WhereTest>().Count_Async());
            Assert.Equal(2, await helper.Where<Tb_WhereTest>(q => q.Vip).Count_Async());
        }

        [Fact]
        public async Task Where_Any_Async()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            Assert.True(await helper.Where<Tb_WhereTest>(q => q.Age == 50).Any_Async());
            Assert.False(await helper.Where<Tb_WhereTest>(q => q.Age == 100).Any_Async());
        }

        [Fact]
        public async Task Where_ToPage_Async()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var page = await helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).ToPage_Async(1, 2);
            Assert.Equal(4, page.TotalItems);
            Assert.Equal(2, page.Items.Count);
            Assert.Equal("张三", page.Items[0].Name);
        }

        [Fact]
        public async Task UpdateMany_Execute_Async()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var r = await helper.UpdateMany<Tb_WhereTest>()
                .Where(q => q.Name == "张三")
                .OnlyFields(q => new { q.Age })
                .Execute_Async(new Tb_WhereTest { Age = 21 });
            Assert.Equal(1, r);
            Assert.Equal(21, helper.Where<Tb_WhereTest>(q => q.Name == "张三").FirstOrDefault().Age);
        }

        [Fact]
        public async Task DeleteMany_Execute_Async()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var r = await helper.DeleteMany<Tb_WhereTest>().Where(q => q.Age == 20).Execute_Async();
            Assert.Equal(1, r);
            Assert.Equal(3, helper.Where<Tb_WhereTest>().Count());
        }

        #endregion

        #region SelectPage（ToPage 别名）

        [Fact]
        public void SelectPage_与ToPage_结果一致()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var page = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).Page(2, 2);
            Assert.Equal(4, page.TotalItems);
            Assert.Equal(2, page.Items.Count);
            Assert.Equal("王五", page.Items[0].Name);
            Assert.Equal("张伟", page.Items[1].Name);

            // 与 ToPage 结果一致
            var toPage = helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).ToPage(2, 2);
            Assert.Equal(toPage.TotalItems, page.TotalItems);
            Assert.Equal(toPage.Items.Count, page.Items.Count);
            Assert.Equal(toPage.Items[0].Name, page.Items[0].Name);
        }

        [Fact]
        public async Task SelectPage_Async()
        {
            using var db = CreateWithUsers();
            var helper = db.Helper;

            var page = await helper.Where<Tb_WhereTest>().OrderBy(q => q.Age).Page_Async(1, 3);
            Assert.Equal(4, page.TotalItems);
            Assert.Equal(3, page.Items.Count);
            Assert.Equal("张三", page.Items[0].Name);
        }

        #endregion
    }
}
