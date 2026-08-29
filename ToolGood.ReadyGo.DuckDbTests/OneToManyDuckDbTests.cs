using Xunit;

namespace ToolGood.ReadyGo.DuckDbTests
{
    /// <summary>
    /// SelectOneToMany 一对多查询单元测试（基于 DuckDB）
    /// </summary>
    [Collection("DuckDb")]
    public class OneToManyDuckDbTests
    {
        private static DuckDbTestDb CreateWithOrders()
        {
            var db = DuckDbTestDb.Create();
            var helper = db.Helper;
            helper.Execute("CREATE SEQUENCE IF NOT EXISTS seq_Customer START 1;");
            helper.Execute("CREATE TABLE IF NOT EXISTS \"Customer\" (\"Id\" INTEGER DEFAULT NEXTVAL('seq_Customer') PRIMARY KEY, \"Name\" Text);");
            helper.Execute("CREATE SEQUENCE IF NOT EXISTS seq_OrderItem START 1;");
            helper.Execute("CREATE TABLE IF NOT EXISTS \"OrderItem\" (\"OrderItemId\" INTEGER DEFAULT NEXTVAL('seq_OrderItem') PRIMARY KEY, \"CustomerId\" INTEGER, \"Product\" Text);");
            helper.Execute("INSERT INTO \"Customer\" (\"Id\", \"Name\") VALUES (1, '张三');");
            helper.Execute("INSERT INTO \"Customer\" (\"Id\", \"Name\") VALUES (2, '李四');");
            helper.Execute("INSERT INTO \"OrderItem\" (\"OrderItemId\", \"CustomerId\", \"Product\") VALUES (1, 1, '苹果');");
            helper.Execute("INSERT INTO \"OrderItem\" (\"OrderItemId\", \"CustomerId\", \"Product\") VALUES (2, 1, '香蕉');");
            helper.Execute("INSERT INTO \"OrderItem\" (\"OrderItemId\", \"CustomerId\", \"Product\") VALUES (3, 2, '橙子');");
            return db;
        }

        private const string JoinSql =
            "SELECT c.\"Id\", c.\"Name\", o.\"OrderItemId\", o.\"CustomerId\", o.\"Product\" " +
            "FROM \"Customer\" c INNER JOIN \"OrderItem\" o ON o.\"CustomerId\" = c.\"Id\" ORDER BY c.\"Id\", o.\"OrderItemId\"";

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
