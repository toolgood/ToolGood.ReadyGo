using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    /// <summary>
    /// 一对多查询模型：主表 Customer，子表 OrderItem
    /// </summary>
    [Table("Customer")]
    [PrimaryKey("Id")]
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<OrderItem> Orders { get; set; }
    }

    [Table("OrderItem")]
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public int CustomerId { get; set; }

        public string Product { get; set; }
    }

    /// <summary>
    /// SelectOneToMany 一对多查询单元测试（基于 SQLite）
    /// </summary>
    public class OneToManyTests
    {
        private static TestDb CreateWithOrders()
        {
            var db = TestDb.Create();
            var helper = db.Helper;
            helper.Execute("CREATE TABLE Customer (Id INTEGER PRIMARY KEY, Name TEXT)");
            helper.Execute("CREATE TABLE OrderItem (OrderItemId INTEGER PRIMARY KEY, CustomerId INTEGER, Product TEXT)");
            helper.Execute("INSERT INTO Customer VALUES (1, '张三')");
            helper.Execute("INSERT INTO Customer VALUES (2, '李四')");
            helper.Execute("INSERT INTO OrderItem VALUES (1, 1, '苹果')");
            helper.Execute("INSERT INTO OrderItem VALUES (2, 1, '香蕉')");
            helper.Execute("INSERT INTO OrderItem VALUES (3, 2, '橙子')");
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
