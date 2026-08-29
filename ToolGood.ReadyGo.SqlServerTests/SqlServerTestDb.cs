namespace ToolGood.ReadyGo.SqlServerTests
{
    /// <summary>
    /// SQL Server 测试数据库工厂。
    /// SQL Server 默认不区分标识符大小写，建表列名与实体属性名保持一致即可。
    /// 本机未配置 SQL Server 服务器时仅能编译。
    /// </summary>
    internal sealed class SqlServerTestDb : IDisposable
    {
        public SqlHelper Helper { get; }

        private SqlServerTestDb(SqlHelper helper)
        {
            Helper = helper;
        }

        public static SqlServerTestDb Create()
        {
            var helper = SqlHelperFactory.OpenSqlServer("127.0.0.1", 1433, "test", "sa", "123456", true);

            // DateTime 字段插入时自动填充当前时间
            helper._Config.Insert_DateTime_Default_Now = true;

            helper.Execute(CreateUserInfoSql);
            helper.Execute(CreateSimpleUserSql);
            return new SqlServerTestDb(helper);
        }

        public void Dispose()
        {
            Helper.Dispose();
        }

        public const string CreateUserInfoSql = @"
IF OBJECT_ID(N'dbo.USERINFO', N'U') IS NULL
CREATE TABLE [dbo].[USERINFO] (
    [ID] INT IDENTITY(1,1) PRIMARY KEY,
    [NAME] NVARCHAR(255) NOT NULL,
    [AGE] INT NOT NULL,
    [REMARK] NVARCHAR(4000),
    [CREATETIME] DATETIME NOT NULL,
    [MONEY] DECIMAL(19,4) NOT NULL,
    [ISDELETE] BIT NOT NULL
)";

        public const string CreateSimpleUserSql = @"
IF OBJECT_ID(N'dbo.SIMPLEUSER', N'U') IS NULL
CREATE TABLE [dbo].[SIMPLEUSER] (
    [ID] INT IDENTITY(1,1) PRIMARY KEY,
    [NAME] NVARCHAR(255),
    [AGE] INT
)";

        /// <summary>
        /// 快速插入一条 UserInfo（CreateTime 由默认值机制填充）
        /// </summary>
        public UserInfo NewUser(string name, int age, decimal money = 0m, bool isDelete = false, string remark = null)
        {
            var u = new UserInfo { Name = name, Age = age, Remark = remark, Money = money, IsDelete = isDelete };
            Helper.Insert(u);
            return u;
        }
    }
}
