namespace ToolGood.ReadyGo.DuckDbTests
{
    /// <summary>
    /// DuckDB 测试数据库工厂：每个测试实例使用独立的临时数据库文件，保证测试间完全隔离、可并行。
    /// 依赖 DuckDB.NET.Data.Full 包（自带各平台原生 duckdb 库）。
    /// </summary>
    internal sealed class DuckDbTestDb : IDisposable
    {
        public SqlHelper Helper { get; }

        private readonly string _dbFile;

        private DuckDbTestDb(SqlHelper helper, string dbFile)
        {
            Helper = helper;
            _dbFile = dbFile;
        }

        public static DuckDbTestDb Create()
        {
            var dbFile = Path.Combine(Path.GetTempPath(), $"readygo_{Guid.NewGuid():N}.duckdb");
            var helper = SqlHelperFactory.OpenDuckDbFile(dbFile);

            // DateTime 字段插入时自动填充当前时间
            helper._Config.Insert_DateTime_Default_Now = true;

            // DuckDB 引号标识符区分大小写，建表 SQL 必须与 ORM 生成的 "UserInfo" 一致
            helper.Execute(CreateSequenceUserInfoSql);
            helper.Execute(CreateUserInfoSql);
            helper.Execute(CreateSequenceSimpleUserSql);
            helper.Execute(CreateSimpleUserSql);
            return new DuckDbTestDb(helper, dbFile);
        }

        public void Dispose()
        {
            Helper.Dispose();
            if (File.Exists(_dbFile)) {
                try {
                    File.Delete(_dbFile);
                } catch {
                    // 忽略清理失败
                }
            }
        }

        public const string CreateSequenceUserInfoSql = "CREATE SEQUENCE IF NOT EXISTS seq_UserInfo START 1;";

        public const string CreateUserInfoSql = @"
CREATE TABLE IF NOT EXISTS ""UserInfo"" (
    ""Id"" INTEGER DEFAULT NEXTVAL('seq_UserInfo') PRIMARY KEY,
    ""Name"" Text NOT NULL,
    ""Age"" INTEGER NOT NULL,
    ""Remark"" Text,
    ""CreateTime"" DATETIME NOT NULL,
    ""Money"" NUMERIC NOT NULL,
    ""IsDelete"" BOOLEAN NOT NULL
);";

        public const string CreateSequenceSimpleUserSql = "CREATE SEQUENCE IF NOT EXISTS seq_SimpleUser START 1;";

        public const string CreateSimpleUserSql = @"
CREATE TABLE IF NOT EXISTS ""SimpleUser"" (
    ""Id"" INTEGER DEFAULT NEXTVAL('seq_SimpleUser') PRIMARY KEY,
    ""Name"" Text,
    ""Age"" INTEGER
);";

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
