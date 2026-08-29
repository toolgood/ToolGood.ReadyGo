using Xunit;

namespace ToolGood.ReadyGo.SqlServerTests
{
    /// <summary>
    /// SelectOneToMany 一对多查询单元测试（基于 SQL Server）
    /// </summary>
    [Collection("SqlServerDb")]
    public class OneToManySqlServerDbTests
    {
        private static SqlServerTestDb CreateWithOrders()
        {
            var db = SqlServerTestDb.Create();
            var helper = db.Helper;
            helper.Execute("DROP TABLE IF EXISTS [dbo].[ORDERITEM]");
            helper.Execute("DROP TABLE IF EXISTS [dbo].[CUSTOMER]");
            helper.Execute("CREATE TABLE [dbo].[CUSTOMER] ([ID] INT NOT NULL PRIMARY KEY, [NAME] NVARCHAR(255))");
            helper.Execute("CREATE TABLE [dbo].[ORDERITEM] ([ORDERITEMID] INT NOT NULL PRIMARY KEY, [CUSTOMERID] INT, [PRODUCT] NVARCHAR(255))");
            helper.Execute("INSERT INTO [dbo].[CUSTOMER] ([ID], [NAME]) VALUES (1, '张三')");
            helper.Execute("INSERT INTO [dbo].[CUSTOMER] ([ID], [NAME]) VALUES (2, '李四')");
            helper.Execute("INSERT INTO [dbo].[ORDERITEM] ([ORDERITEMID], [CUSTOMERID], [PRODUCT]) VALUES (1, 1, '苹果')");
            helper.Execute("INSERT INTO [dbo].[ORDERITEM] ([ORDERITEMID], [CUSTOMERID], [PRODUCT]) VALUES (2, 1, '香蕉')");
            helper.Execute("INSERT INTO [dbo].[ORDERITEM] ([ORDERITEMID], [CUSTOMERID], [PRODUCT]) VALUES (3, 2, '橙子')");
            return db;
        }

        private const string JoinSql =
            "SELECT c.[ID], c.[NAME], o.[ORDERITEMID], o.[CUSTOMERID], o.[PRODUCT] " +
            "FROM [dbo].[CUSTOMER] c INNER JOIN [dbo].[ORDERITEM] o ON o.[CUSTOMERID] = c.[ID] ORDER BY c.[ID], o.[ORDERITEMID]";

        [Fact]
        public void SelectOneToMany_默认主键合并子表()
        {
            using var db = CreateWithOrders();
            var helper = db.Helper;

            var customers = helper.SelectOneToMany<Customer>(x => x.Orders, JoinSql);

            Assert.Equal(2, customers.Count);
            Assert.Equal("张三", customers[0].Name);
            Assert.Equal(2, customers[0].Orders.Count);
            Assert.Equal("苹果", customers[0].Orders[0].Product);
            Assert.Equal("香蕉", customers[0].Orders[1].Product);
            Assert.Single(customers[1].Orders);
            Assert.Equal("橙子", customers[1].Orders[0].Product);
        }

        [Fact]
        public void SelectOneToMany_idFunc指定主键合并()
        {
            using var db = CreateWithOrders();
            var helper = db.Helper;

            var customers = helper.SelectOneToMany<Customer>(x => x.Orders, c => c.Id, JoinSql);

            Assert.Equal(2, customers.Count);
            Assert.Equal(2, customers[0].Orders.Count);
            Assert.Single(customers[1].Orders);
            Assert.Equal("橙子", customers[1].Orders[0].Product);
        }

        [Fact]
        public async Task SelectOneToMany_Async_默认主键合并子表()
        {
            using var db = CreateWithOrders();
            var helper = db.Helper;

            var customers = await helper.SelectOneToMany_Async<Customer>(x => x.Orders, JoinSql);

            Assert.Equal(2, customers.Count);
            Assert.Equal(2, customers[0].Orders.Count);
            Assert.Equal("苹果", customers[0].Orders[0].Product);
            Assert.Equal("香蕉", customers[0].Orders[1].Product);
        }

        [Fact]
        public async Task SelectOneToMany_Async_idFunc指定主键合并()
        {
            using var db = CreateWithOrders();
            var helper = db.Helper;

            var customers = await helper.SelectOneToMany_Async<Customer>(x => x.Orders, c => c.Id, JoinSql);

            Assert.Equal(2, customers.Count);
            Assert.Single(customers[1].Orders);
            Assert.Equal("橙子", customers[1].Orders[0].Product);
        }
    }
}
