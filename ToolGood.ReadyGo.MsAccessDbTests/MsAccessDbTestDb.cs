using System.Data.OleDb;

namespace ToolGood.ReadyGo.MsAccessDbTests
{
    /// <summary>
    /// Access 测试数据库工厂：每个测试实例使用独立的 .mdb 文件，保证测试间完全隔离。
    /// Jet 4.0 为 32 位驱动，64 位进程需改用 OpenAccessFile64x（ACE.OLEDB.12.0，需本机安装 Access 数据库引擎）。
    /// 本机未安装 Jet/ACE 驱动时仅能编译。
    /// </summary>
    internal sealed class MsAccessDbTestDb : IDisposable
    {
        public SqlHelper Helper { get; }

        private readonly string _dbFile;

        private static readonly object _createLock = new object();

        private MsAccessDbTestDb(SqlHelper helper, string dbFile)
        {
            Helper = helper;
            _dbFile = dbFile;
        }

        public static MsAccessDbTestDb Create()
        {
            var dbFile = Path.Combine(Path.GetTempPath(), $"readygo_{Guid.NewGuid():N}.mdb");
            lock (_createLock) {
                if (File.Exists(dbFile) == false) {
                    CreateDatabaseFile(dbFile);
                }
            }

            var helper = SqlHelperFactory.OpenAccessFile(dbFile);

            // DateTime 字段插入时自动填充当前时间
            helper._Config.Insert_DateTime_Default_Now = true;

            helper.Execute(CreateUserInfoSql);
            helper.Execute(CreateSimpleUserSql);
            return new MsAccessDbTestDb(helper, dbFile);
        }

        /// <summary>
        /// 通过 Jet OLE DB 的空 Provider 连接执行 CREATE DATABASE 创建空库。
        /// </summary>
        private static void CreateDatabaseFile(string dbFile)
        {
            using (var conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"CREATE DATABASE '{dbFile.Replace("'", "''")}';";
                    cmd.ExecuteNonQuery();
                }
            }
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
CREATE TABLE [UserInfo] (
    [Id] AUTOINCREMENT PRIMARY KEY,
    [Name] TEXT(255) NOT NULL,
    [Age] INTEGER NOT NULL,
    [Remark] TEXT(255),
    [CreateTime] DATETIME NOT NULL,
    [Money] DECIMAL(18,2) NOT NULL,
    [IsDelete] BIT NOT NULL
);";

        public const string CreateSimpleUserSql = @"
CREATE TABLE [SimpleUser] (
    [Id] AUTOINCREMENT PRIMARY KEY,
    [Name] TEXT(255),
    [Age] INTEGER
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
