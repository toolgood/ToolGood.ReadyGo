using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    public enum BigUserState : long
    {
        None = 0,
        Normal = 1,
        Vip = 2,
        Large = 3000000000,
    }

    [Table("Tb_Enum2LongTest")]
    [PrimaryKey("Id")]
    public class Tb_Enum2LongTest
    {
        public int Id { get; set; }

        [Enum2Long]
        public BigUserState State { get; set; }

        [Enum2Long]
        public BigUserState? Extra { get; set; }
    }

    /// <summary>
    /// [Enum2Long] 属性：enum 以底层长整数值（long）保存
    /// </summary>
    public class Enum2LongAttributeTests
    {
        [Fact]
        public void 枚举_以底层长整数存储()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Enum2LongTest));

            var item = new Tb_Enum2LongTest { State = BigUserState.Large, Extra = BigUserState.Normal };
            helper.Insert(item);

            // 存储格式：Large → 3000000000（超出 int 范围，验证 long 存储），Normal → 1
            Assert.Equal(3000000000L, System.Convert.ToInt64(helper.ExecuteScalar<object>("SELECT State FROM Tb_Enum2LongTest WHERE Id = @0", item.Id)));
            Assert.Equal(1L, System.Convert.ToInt64(helper.ExecuteScalar<object>("SELECT Extra FROM Tb_Enum2LongTest WHERE Id = @0", item.Id)));

            var loaded = helper.FirstOrDefault<Tb_Enum2LongTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(BigUserState.Large, loaded.State);
            Assert.Equal(BigUserState.Normal, loaded.Extra);
        }

        [Fact]
        public void 可空枚举_Null值序列化与反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Enum2LongTest));

            var item = new Tb_Enum2LongTest { State = BigUserState.None, Extra = null };
            helper.Insert(item);

            // 数据库中实际存 NULL
            var raw = helper.ExecuteScalar<object>("SELECT Extra FROM Tb_Enum2LongTest WHERE Id = @0", item.Id);
            Assert.True(raw == null || raw == DBNull.Value);

            var loaded = helper.FirstOrDefault<Tb_Enum2LongTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Extra);
        }
    }
}
