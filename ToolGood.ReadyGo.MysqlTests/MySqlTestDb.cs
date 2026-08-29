namespace ToolGood.ReadyGo.MysqlTests
{
    /// <summary>
    /// MySQL 测试数据库工厂：每个测试实例连接到同一 MySQL 数据库，通过建表前 DROP 表保证隔离。
    /// </summary>
    internal sealed class MySqlTestDb : IDisposable
    {
        public SqlHelper Helper { get; }

        private MySqlTestDb(SqlHelper helper)
        {
            Helper = helper;
        }

        public static MySqlTestDb Create()
        {
            // 通过 OpenMysql 生成连接字符串：主库会按 MySql.Data/MySqlConnector 版本自动选择 SslMode 等选项
            var helper = SqlHelperFactory.OpenMysql("localhost", 3306, "test", "test", "test123");

            // DateTime 字段插入时自动填充当前时间
            helper._Config.Insert_DateTime_Default_Now = true;

            helper.Execute("DROP TABLE IF EXISTS UserInfo;");
            helper.Execute("DROP TABLE IF EXISTS SimpleUser;");
            helper.Execute(CreateUserInfoSql);
            helper.Execute(CreateSimpleUserSql);
            return new MySqlTestDb(helper);
        }

        public void Dispose()
        {
            Helper.Dispose();
        }

        public const string CreateUserInfoSql = @"
CREATE TABLE UserInfo (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Age INT NOT NULL,
    Remark TEXT NULL,
    CreateTime DATETIME NOT NULL,
    Money DECIMAL(18,2) NOT NULL,
    IsDelete TINYINT(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

        public const string CreateSimpleUserSql = @"
CREATE TABLE SimpleUser (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NULL,
    Age INT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

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
