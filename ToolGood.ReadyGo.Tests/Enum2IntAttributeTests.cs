using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    public enum UserState : byte
    {
        None = 0,
        Normal = 1,
        Vip = 2,
    }

    [Table("Tb_Enum2IntTest")]
    [PrimaryKey("Id")]
    public class Tb_Enum2IntTest
    {
        public int Id { get; set; }

        [Enum2Int]
        public UserState State { get; set; }

        [Enum2Int]
        public UserState? Extra { get; set; }
    }

    /// <summary>
    /// [Enum2Int] 属性：enum 以底层整数值保存
    /// </summary>
    public class Enum2IntAttributeTests
    {
        [Fact]
        public void 可空枚举_Null值序列化与反序列化()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Enum2IntTest));

            var item = new Tb_Enum2IntTest { State = UserState.None, Extra = null };
            helper.Insert(item);

            // 数据库中实际存 NULL
            var raw = helper.ExecuteScalar<object>("SELECT Extra FROM Tb_Enum2IntTest WHERE Id = @0", item.Id);
            Assert.True(raw == null || raw == DBNull.Value);

            var loaded = helper.FirstOrDefault<Tb_Enum2IntTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Null(loaded.Extra);
        }

        [Fact]
        public void 可空枚举_数据库NULL值读取()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Enum2IntTest));

            // 直接 SQL 插入 NULL，模拟历史数据
            helper.Execute("INSERT INTO Tb_Enum2IntTest (Id, State, Extra) VALUES (@0, 2, NULL)", 1);

            var loaded = helper.FirstOrDefault<Tb_Enum2IntTest>(1);
            Assert.NotNull(loaded);
            Assert.Equal(UserState.Vip, loaded.State);
            Assert.Null(loaded.Extra);
        }

        [Fact]
        public void 枚举_以底层整数存储()
        {
            using var db = TestDb.Create();
            var helper = db.Helper;
            helper.TableHelper.TryCreateTable(typeof(Tb_Enum2IntTest));

            var item = new Tb_Enum2IntTest { State = UserState.Vip, Extra = UserState.Normal };
            helper.Insert(item);

            // 存储格式：Vip → 2，Normal → 1
            Assert.Equal(2, System.Convert.ToInt32(helper.ExecuteScalar<object>("SELECT State FROM Tb_Enum2IntTest WHERE Id = @0", item.Id)));
            Assert.Equal(1, System.Convert.ToInt32(helper.ExecuteScalar<object>("SELECT Extra FROM Tb_Enum2IntTest WHERE Id = @0", item.Id)));

            var loaded = helper.FirstOrDefault<Tb_Enum2IntTest>(item.Id);
            Assert.NotNull(loaded);
            Assert.Equal(UserState.Vip, loaded.State);
            Assert.Equal(UserState.Normal, loaded.Extra);
        }
    }
}
