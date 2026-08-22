using ToolGood.ReadyGo.Attributes;

namespace ToolGood.ReadyGo.Tests
{
    [Table("UserInfo")]
    [PrimaryKey("Id")]
    public class UserInfo
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Age")]
        public int Age { get; set; }

        [Column("Remark")]
        public string Remark { get; set; }

        [Column("CreateTime")]
        public DateTime CreateTime { get; set; }

        [Column("Money")]
        public decimal Money { get; set; }

        [Column("IsDelete")]
        public bool IsDelete { get; set; }
    }

    /// <summary>
    /// 对象条件测试用（无 DateTime，避免 Microsoft.Data.Sqlite 存储精度差异）
    /// </summary>
    [Table("SimpleUser")]
    [PrimaryKey("Id")]
    public class SimpleUser
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Age")]
        public int Age { get; set; }
    }
}
