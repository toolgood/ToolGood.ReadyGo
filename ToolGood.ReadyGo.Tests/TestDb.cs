namespace ToolGood.ReadyGo.Tests
{
    /// <summary>
    /// SQLite 测试数据库工厂：每个测试实例使用独立的临时数据库文件，保证测试间完全隔离、可并行。
    /// </summary>
    internal sealed class TestDb : IDisposable
    {
        public SqlHelper Helper { get; }

        private readonly string _dbFile;

        private TestDb(SqlHelper helper, string dbFile)
        {
            Helper = helper;
            _dbFile = dbFile;
        }

        public static TestDb Create()
        {
            var dbFile = Path.Combine(Path.GetTempPath(), $"readygo_{Guid.NewGuid():N}.db");
            var helper = SqlHelperFactory.OpenMsSqliteFile(dbFile);

            // DateTime 字段插入时自动填充当前时间
            helper._Config.Insert_DateTime_Default_Now = true;

            helper.Execute(CreateUserInfoSql);
            helper.Execute(CreateSimpleUserSql);
            return new TestDb(helper, dbFile);
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

        public const string CreateUserInfoSql = @"
CREATE TABLE UserInfo (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Age INTEGER NOT NULL,
    Remark TEXT,
    CreateTime TEXT NOT NULL,
    Money TEXT NOT NULL,
    IsDelete INTEGER NOT NULL
);";

        public const string CreateSimpleUserSql = @"
CREATE TABLE SimpleUser (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT,
    Age INTEGER
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
