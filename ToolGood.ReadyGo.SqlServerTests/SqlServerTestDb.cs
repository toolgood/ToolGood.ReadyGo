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
            return new SqlServerTestDb(helper);
        }

        private static void EnsureDatabaseCreated(string dbFile)
        {
            var mdf = dbFile.Replace("'", "''");
            var ldf = Path.ChangeExtension(dbFile, ".ldf").Replace("'", "''");
            var sql = $"CREATE DATABASE [{DatabaseName}] ON PRIMARY (NAME = N'{DatabaseName}', FILENAME = N'{mdf}') " +
                      $"LOG ON (NAME = N'{DatabaseName}_log', FILENAME = N'{ldf}')";

            using (var conn = new Microsoft.Data.SqlClient.SqlConnection(
                       "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=SSPI;TrustServerCertificate=True")) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
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
