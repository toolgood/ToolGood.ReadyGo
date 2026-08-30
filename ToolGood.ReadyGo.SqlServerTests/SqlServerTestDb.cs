namespace ToolGood.ReadyGo.SqlServerTests
{
    /// <summary>
    /// SQL Server 测试数据库工厂（本地 LocalDB 文件模式，无需数据库服务器）。
    /// 使用 SQL Server Express LocalDB（默认实例 (LocalDB)\MSSQLLocalDB，随 Visual Studio 安装），
    /// 数据库文件存放在 %TEMP%\ToolGood.ReadyGo\ReadyGoTest.mdf，首次运行时自动创建。
    /// SQL Server 默认不区分标识符大小写，建表列名与实体属性名保持一致即可。
    /// </summary>
    internal sealed class SqlServerTestDb : IDisposable
    {
        private const string DatabaseName = "ReadyGoTest";
        private const string MasterConnStr = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=SSPI;TrustServerCertificate=True";
        private static readonly object _initLock = new object();

        public SqlHelper Helper { get; }

        private SqlServerTestDb(SqlHelper helper)
        {
            Helper = helper;
        }

        public static SqlServerTestDb Create()
        {
            // 使用本地 .mdf 文件，不依赖网络数据库服务器
            var dbDir = Path.Combine(Path.GetTempPath(), "ToolGood.ReadyGo");
            Directory.CreateDirectory(dbDir);
            var dbFile = Path.Combine(dbDir, DatabaseName + ".mdf");

            // LocalDB 的 AttachDBFilename 不会自动建库，文件不存在时先连 master 显式创建
            if (File.Exists(dbFile) == false) {
                EnsureDatabaseCreated(dbFile);
            }

            var helper = SqlHelperFactory.OpenSqlServerFile(dbFile, DatabaseName);

            // DateTime 字段插入时自动填充当前时间
            helper._Config.Insert_DateTime_Default_Now = true;

            helper.Execute(CreateUserInfoSql);
            helper.Execute(CreateSimpleUserSql);

            // 每个测试从干净数据开始，避免测试间互相影响
            helper.Execute("DELETE FROM [USERINFO]");
            helper.Execute("DELETE FROM [SIMPLEUSER]");
            // 重置自增计数，保证测试中 ID 从 1 开始
            helper.Execute("DBCC CHECKIDENT('[USERINFO]', RESEED, 0)");
            helper.Execute("DBCC CHECKIDENT('[SIMPLEUSER]', RESEED, 0)");
            return new SqlServerTestDb(helper);
        }

        /// <summary>
        /// 确保 LocalDB 中 ReadyGoTest 数据库与 .mdf 文件状态一致：
        /// 文件与数据库都存在 → 直接使用；数据库仍注册但文件被删 → 先删库再建库；
        /// 文件存在但未注册 → 附加；都不存在 → 新建。
        /// 加锁防止 xUnit 并行测试同时初始化数据库。
        /// </summary>
        private static void EnsureDatabaseCreated(string dbFile)
        {
            if (File.Exists(dbFile) && IsDatabaseRegistered()) return;

            lock (_initLock) {
                if (File.Exists(dbFile) && IsDatabaseRegistered()) return;

                var mdf = dbFile.Replace("'", "''");
                var ldf = Path.ChangeExtension(dbFile, ".ldf").Replace("'", "''");

                using (var conn = new Microsoft.Data.SqlClient.SqlConnection(MasterConnStr)) {
                    conn.Open();

                    if (IsDatabaseRegistered(conn)) {
                        // 数据库仍注册但文件已被删除，状态不一致 → 先强制删除再重建
                        using (var cmd = conn.CreateCommand()) {
                            cmd.CommandText = $"IF DB_ID(N'{DatabaseName}') IS NOT NULL " +
                                              $"BEGIN ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{DatabaseName}]; END";
                            cmd.ExecuteNonQuery();
                        }
                    }

                    if (File.Exists(dbFile)) {
                        // 文件存在但数据库未注册 → 附加
                        using (var cmd = conn.CreateCommand()) {
                            cmd.CommandText = $"CREATE DATABASE [{DatabaseName}] ON (FILENAME = N'{mdf}') FOR ATTACH";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else {
                        using (var cmd = conn.CreateCommand()) {
                            cmd.CommandText = $"CREATE DATABASE [{DatabaseName}] ON PRIMARY (NAME = N'{DatabaseName}', FILENAME = N'{mdf}') " +
                                              $"LOG ON (NAME = N'{DatabaseName}_log', FILENAME = N'{ldf}') " +
                                              "COLLATE Chinese_PRC_CI_AS";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private static bool IsDatabaseRegistered()
        {
            using (var conn = new Microsoft.Data.SqlClient.SqlConnection(MasterConnStr)) {
                conn.Open();
                return IsDatabaseRegistered(conn);
            }
        }

        private static bool IsDatabaseRegistered(Microsoft.Data.SqlClient.SqlConnection conn)
        {
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = @name";
                cmd.Parameters.AddWithValue("@name", DatabaseName);
                return (int)cmd.ExecuteScalar() > 0;
            }
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
