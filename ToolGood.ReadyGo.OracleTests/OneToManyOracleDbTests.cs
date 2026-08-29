using Xunit;

namespace ToolGood.ReadyGo.OracleTests
{
    /// <summary>
    /// SelectOneToMany 一对多查询单元测试（基于 Oracle）
    /// </summary>
    [Collection("OracleDb")]
    public class OneToManyOracleDbTests
    {
        private static OracleTestDb CreateWithOrders()
        {
            var db = OracleTestDb.Create();
            var helper = db.Helper;
            helper.Execute("CREATE TABLE \"CUSTOMER\" (\"ID\" NUMBER(10) NOT NULL PRIMARY KEY, \"NAME\" NVARCHAR2(255))");
            helper.Execute("CREATE TABLE \"ORDERITEM\" (\"ORDERITEMID\" NUMBER(10) NOT NULL PRIMARY KEY, \"CUSTOMERID\" NUMBER(10), \"PRODUCT\" NVARCHAR2(255))");
            helper.Execute("INSERT INTO \"CUSTOMER\" (\"ID\", \"NAME\") VALUES (1, '张三')");
            helper.Execute("INSERT INTO \"CUSTOMER\" (\"ID\", \"NAME\") VALUES (2, '李四')");
            helper.Execute("INSERT INTO \"ORDERITEM\" (\"ORDERITEMID\", \"CUSTOMERID\", \"PRODUCT\") VALUES (1, 1, '苹果')");
            helper.Execute("INSERT INTO \"ORDERITEM\" (\"ORDERITEMID\", \"CUSTOMERID\", \"PRODUCT\") VALUES (2, 1, '香蕉')");
            helper.Execute("INSERT INTO \"ORDERITEM\" (\"ORDERITEMID\", \"CUSTOMERID\", \"PRODUCT\") VALUES (3, 2, '橙子')");
            return db;
        }

        private const string JoinSql =
            "SELECT c.\"ID\", c.\"NAME\", o.\"ORDERITEMID\", o.\"CUSTOMERID\", o.\"PRODUCT\" " +
            "FROM \"CUSTOMER\" c INNER JOIN \"ORDERITEM\" o ON o.\"CUSTOMERID\" = c.\"ID\" ORDER BY c.\"ID\", o.\"ORDERITEMID\"";

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
