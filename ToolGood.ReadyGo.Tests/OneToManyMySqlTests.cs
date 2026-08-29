using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    /// <summary>
    /// SelectOneToMany 一对多查询单元测试（基于 MySQL）
    /// </summary>
    [Collection("MySql")]
    public class OneToManyMySqlTests
    {
        private static MySqlTestDb CreateWithOrders()
        {
            var db = MySqlTestDb.Create();
            var helper = db.Helper;
            helper.Execute("DROP TABLE IF EXISTS OrderItem;");
            helper.Execute("DROP TABLE IF EXISTS Customer;");
            helper.Execute("CREATE TABLE Customer (Id INT PRIMARY KEY AUTO_INCREMENT, Name VARCHAR(255));");
            helper.Execute("CREATE TABLE OrderItem (OrderItemId INT PRIMARY KEY AUTO_INCREMENT, CustomerId INT, Product VARCHAR(255));");
            helper.Execute("INSERT INTO Customer (Id, Name) VALUES (1, '张三'), (2, '李四');");
            helper.Execute("INSERT INTO OrderItem (OrderItemId, CustomerId, Product) VALUES (1, 1, '苹果'), (2, 1, '香蕉'), (3, 2, '橙子');");
            return db;
        }

        private const string JoinSql =
            "SELECT c.Id, c.Name, o.OrderItemId, o.CustomerId, o.Product " +
            "FROM Customer c INNER JOIN OrderItem o ON o.CustomerId = c.Id ORDER BY c.Id, o.OrderItemId";

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
