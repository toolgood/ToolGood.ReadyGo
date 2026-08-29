using System;
using System.Collections.Generic;
using ToolGood.ReadyGo.Attributes;

namespace ToolGood.ReadyGo.SqlServerTests
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

    [Table("Tb_BlobTest")]
    [PrimaryKey("Id")]
    public class Tb_BlobTest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] Data { get; set; }
    }

    [Table("Tb_MoneyTest")]
    [PrimaryKey("Id")]
    public class Tb_MoneyTest
    {
        public int Id { get; set; }

        [Numeric2Int(2)]
        public decimal Money { get; set; }

        [Numeric2Long(3)]
        public double Weight { get; set; }
    }

    [Table("Tb_NumericArrayTest")]
    [PrimaryKey("Id")]
    public class Tb_NumericArrayTest
    {
        public int Id { get; set; }

        [NumericArray2Bytes]
        public float[] Floats { get; set; }

        [NumericArray2Bytes]
        public List<float> ValueList { get; set; }

        [NumericArray2Bytes]
        public double[] Doubles { get; set; }

        [NumericArray2Bytes]
        public List<int> Ints { get; set; }
    }

    [Table("Users")]
    [PrimaryKey("UserId")]
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }
    }

    [Table("Customer")]
    [PrimaryKey("Id")]
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<OrderItem> Orders { get; set; }
    }

    [Table("OrderItem")]
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public int CustomerId { get; set; }

        public string Product { get; set; }
    }

    [Table("Tb_Order")]
    [PrimaryKey("Id")]
    [Index("UserId")]
    [Unique("OrderNo")]
    public class Tb_Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [FieldLength(50)]
        public string OrderNo { get; set; }

        [Text]
        public string Remark { get; set; }

        [DefaultValue("0")]
        public decimal Money { get; set; }

        public DateTime CreateTime { get; set; }
    }

    [Table("Tb_WhereTest")]
    [PrimaryKey("Id")]
    public class Tb_WhereTest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public bool Vip { get; set; }
    }

    [Table("Tb_Provider_Test")]
    [PrimaryKey("Id")]
    public class Tb_Provider_AutoInc
    {
        [DefaultValue("0")]
        public int Id { get; set; }

        public string Name { get; set; }

        public char? Code { get; set; }
    }

    [Table("Tb_Provider_Test_NoAuto")]
    [PrimaryKey("Id", AutoIncrement = false)]
    public class Tb_Provider_NoAutoInc
    {
        public int Id { get; set; }
    }
}
