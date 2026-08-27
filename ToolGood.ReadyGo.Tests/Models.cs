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

    /// <summary>
    /// 列名映射测试用：数据库列名与 C# 属性名不同（下划线命名），用于验证对象条件遵循 [Column] 映射。
    /// </summary>
    [Table("MappedUser")]
    [PrimaryKey("Id")]
    public class MappedUser
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("user_age")]
        public int UserAge { get; set; }
    }

    /// <summary>
    /// 字符串主键测试用：验证 object 条件重载对字符串主键的识别。
    /// </summary>
    [Table("StringKeyUser")]
    [PrimaryKey("Code", AutoIncrement = false)]
    public class StringKeyUser
    {
        [Column("Code")]
        public string Code { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Age")]
        public int Age { get; set; }
    }

    /// <summary>
    /// float 列条件测试用：验证 where 中 float[] 作为条件集合（生成 IN 子句）。
    /// </summary>
    [Table("Tb_FloatCondTest")]
    [PrimaryKey("Id")]
    public class Tb_FloatCondTest
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Score")]
        public float Score { get; set; }
    }
}
