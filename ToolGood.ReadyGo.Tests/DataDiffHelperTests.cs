using System;
using System.Collections.Generic;
using ToolGood.ReadyGo.Attributes;
using Xunit;

namespace ToolGood.ReadyGo.Tests
{
    #region 测试模型

    /// <summary>普通模型：仅标记 DataName 的属性会被收集输出，验证无枚举字典时的格式化</summary>
    public class DiffUser
    {
        public int Id { get; set; }

        [DataName("姓名")]
        public string Name { get; set; }

        [DataName("年龄")]
        public int Age { get; set; }

        [DataName("启用")]
        public bool Active { get; set; }

        [DataName("创建时间")]
        public DateTime CreateTime { get; set; }

        [DataName("金额")]
        public decimal Money { get; set; }
    }

    /// <summary>带类级 DataName 与属性级 DataName 的模型</summary>
    [DataName("用户")]
    public class DiffNamedUser
    {
        public int Id { get; set; }

        [DataName("姓名")]
        public string Name { get; set; }

        [DataName("年龄")]
        public int Age { get; set; }
    }

    /// <summary>DataEnum 用于 int 属性：索引 0..n 映射名称</summary>
    public class DiffEnumIntUser
    {
        public int Id { get; set; }

        [DataEnum("性别", "男", "女", "未知")]
        public int Gender { get; set; }
    }

    /// <summary>枚举属性 + DataName：成员名映射显示名</summary>
    public enum DiffOrderStatus
    {
        [DataName("待处理")]
        Pending,
        [DataName("已完成")]
        Done,
        [DataName("已取消")]
        Cancelled
    }

    public class DiffEnumUser
    {
        public int Id { get; set; }

        [DataName("状态")]
        public DiffOrderStatus Status { get; set; }
    }

    /// <summary>byte 底层类型枚举（回归：非 int 底层枚举不应崩溃）</summary>
    public enum DiffByteLevel : byte
    {
        [DataName("低")]
        Low = 1,
        [DataName("高")]
        High = 2
    }

    public class DiffByteEnumUser
    {
        public int Id { get; set; }

        [DataName("级别")]
        public DiffByteLevel Level { get; set; }
    }

    /// <summary>DataEnum 用于 string 属性：逗号分隔多值</summary>
    public class DiffStringEnumUser
    {
        public int Id { get; set; }

        [DataEnum("权限", "只读", "写入", "管理")]
        public string Permissions { get; set; }
    }

    /// <summary>DataEnum 用于 bool 属性</summary>
    public class DiffBoolUser
    {
        public int Id { get; set; }

        [DataEnum("启用", "否", "是")]
        public bool Active { get; set; }
    }

    /// <summary>可空属性，验证 null 处理</summary>
    public class DiffNullUser
    {
        public int Id { get; set; }

        [DataName("备注")]
        public string Remark { get; set; }
    }

    /// <summary>无 Id 属性的模型</summary>
    public class DiffNoIdUser
    {
        [DataName("名称")]
        public string Name { get; set; }
    }

    /// <summary>带 DataEnumSql 的模型：SQL 查询失败时应容错不崩溃</summary>
    public class DiffSqlUser
    {
        public int Id { get; set; }

        [DataEnumSql("状态", "SELECT id, name FROM not_exist_table")]
        public int Status { get; set; }
    }

    #endregion

    public class DataDiffHelperTests
    {
        #region 新增（仅新数据）

        [Fact]
        public void 新增_输出全部非空属性值()
        {
            var right = new DiffUser {
                Id = 1,
                Name = "张三",
                Age = 30,
                Active = true,
                CreateTime = new DateTime(2024, 1, 1, 12, 0, 0),
                Money = 10.5m
            };

            var result = DataDiffHelper.Diff(right);

            Assert.Equal("新增[id]1，姓名：张三，年龄：30，启用：True，创建时间：2024-01-01 12:00:00，金额：10.5", result);
        }

        [Fact]
        public void 新增_类名与属性名_使用显示名()
        {
            var right = new DiffNamedUser { Id = 5, Name = "李四", Age = 31 };

            var result = DataDiffHelper.Diff(right);

            Assert.Equal("新增[用户]5，姓名：李四，年龄：31", result);
        }

        [Fact]
        public void 新增_无Id属性_输出新增()
        {
            var right = new DiffNoIdUser { Name = "王五" };

            var result = DataDiffHelper.Diff(right);

            Assert.Equal("新增，名称：王五", result);
        }

        [Fact]
        public void 新增_null和空字符串属性_跳过()
        {
            var right = new DiffNullUser { Id = 1, Remark = null };

            var result = DataDiffHelper.Diff(right);

            Assert.Equal("新增[id]1", result);
        }

        [Fact]
        public void 新增_DataEnum整数属性_输出索引映射()
        {
            var right = new DiffEnumIntUser { Id = 1, Gender = 1 };

            var result = DataDiffHelper.Diff(right);

            Assert.Equal("新增[id]1，性别：1=女", result);
        }

        [Fact]
        public void 新增_枚举属性_输出成员映射()
        {
            var right = new DiffEnumUser { Id = 1, Status = DiffOrderStatus.Pending };

            var result = DataDiffHelper.Diff(right);

            Assert.Equal("新增[id]1，状态：Pending=待处理", result);
        }

        [Fact]
        public void 新增_byte底层枚举_不崩溃()
        {
            var right = new DiffByteEnumUser { Id = 1, Level = DiffByteLevel.High };

            var result = DataDiffHelper.Diff(right);

            Assert.Equal("新增[id]1，级别：High=高", result);
        }

        [Fact]
        public void 新增_DataEnum字符串属性_拆分逗号并映射()
        {
            var right = new DiffStringEnumUser { Id = 1, Permissions = "1,2" };

            var result = DataDiffHelper.Diff(right);

            Assert.Equal("新增[id]1，权限：1=写入|2=管理", result);
        }

        [Fact]
        public void 新增_DataEnum布尔属性_映射()
        {
            var right = new DiffBoolUser { Id = 1, Active = true };

            var result = DataDiffHelper.Diff(right);

            Assert.Equal("新增[id]1，启用：是", result);
        }

        [Fact]
        public void 新增_SqlHelper为空_SQL查询失败不影响输出()
        {
            var right = new DiffSqlUser { Id = 1, Status = 2 };

            var result = DataDiffHelper.Diff(right, (SqlHelper)null);

            Assert.Equal("新增[id]1，状态：2", result);
        }

        #endregion

        #region 修改（左右数据）

        [Fact]
        public void 修改_输出左右值()
        {
            var left = new DiffUser {
                Id = 1,
                Name = "张三",
                Age = 30,
                Active = false,
                CreateTime = new DateTime(2024, 1, 1),
                Money = 10.5m
            };
            var right = new DiffUser {
                Id = 1,
                Name = "李四",
                Age = 31,
                Active = true,
                CreateTime = new DateTime(2024, 2, 2, 8, 30, 0),
                Money = 20.5m
            };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("修改[id]1，姓名：张三->李四，年龄：30->31，启用：False->True，创建时间：2024-01-01 00:00:00->2024-02-02 08:30:00，金额：10.5->20.5", result);
        }

        [Fact]
        public void 修改_类名与属性名_使用显示名()
        {
            var left = new DiffNamedUser { Id = 5, Name = "张三", Age = 30 };
            var right = new DiffNamedUser { Id = 5, Name = "李四", Age = 31 };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("修改[用户]5，姓名：张三->李四，年龄：30->31", result);
        }

        [Fact]
        public void 修改_无Id属性_输出修改()
        {
            var left = new DiffNoIdUser { Name = "张三" };
            var right = new DiffNoIdUser { Name = "李四" };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("修改，名称：张三->李四", result);
        }

        [Fact]
        public void 修改_Id变化_视为新增()
        {
            var left = new DiffNamedUser { Id = 1, Name = "张三", Age = 30 };
            var right = new DiffNamedUser { Id = 2, Name = "李四", Age = 31 };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("新增[用户]2，姓名：李四，年龄：31", result);
        }

        [Fact]
        public void 修改_DataEnum整数属性_输出索引映射()
        {
            var left = new DiffEnumIntUser { Id = 1, Gender = 0 };
            var right = new DiffEnumIntUser { Id = 1, Gender = 1 };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("修改[id]1，性别：0=男->1=女", result);
        }

        [Fact]
        public void 修改_枚举属性_输出成员映射()
        {
            var left = new DiffEnumUser { Id = 1, Status = DiffOrderStatus.Pending };
            var right = new DiffEnumUser { Id = 1, Status = DiffOrderStatus.Done };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("修改[id]1，状态：Pending=待处理->Done已完成", result);
        }

        [Fact]
        public void 修改_byte底层枚举_不崩溃()
        {
            var left = new DiffByteEnumUser { Id = 1, Level = DiffByteLevel.Low };
            var right = new DiffByteEnumUser { Id = 1, Level = DiffByteLevel.High };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("修改[id]1，级别：Low=低->High高", result);
        }

        [Fact]
        public void 修改_DataEnum字符串属性_拆分逗号并映射()
        {
            var left = new DiffStringEnumUser { Id = 1, Permissions = "0,1" };
            var right = new DiffStringEnumUser { Id = 1, Permissions = "1,2" };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("修改[id]1，权限：0=只读|1=写入->1=写入|2=管理", result);
        }

        [Fact]
        public void 修改_DataEnum布尔属性_映射()
        {
            var left = new DiffBoolUser { Id = 1, Active = false };
            var right = new DiffBoolUser { Id = 1, Active = true };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("修改[id]1，启用：否->是", result);
        }

        [Fact]
        public void 修改_值变为null_输出NULL标记()
        {
            var left = new DiffNullUser { Id = 1, Remark = "abc" };
            var right = new DiffNullUser { Id = 1, Remark = null };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("修改[id]1，备注：abc->(NULL)", result);
        }

        [Fact]
        public void 修改_null变为值_输出NULL标记()
        {
            var left = new DiffNullUser { Id = 1, Remark = null };
            var right = new DiffNullUser { Id = 1, Remark = "abc" };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("修改[id]1，备注：(NULL)->abc", result);
        }

        [Fact]
        public void 修改_左右皆null_不输出该属性()
        {
            var left = new DiffNullUser { Id = 1, Remark = null };
            var right = new DiffNullUser { Id = 1, Remark = null };

            var result = DataDiffHelper.Diff(left, right);

            Assert.Equal("修改[id]1", result);
        }

        #endregion

        #region 列表 Diff

        [Fact]
        public void 列表_string版本_输出左右列表()
        {
            var lefts = new List<string> { "a", "b", "" };
            var rights = new List<string> { "b", "c", "" };

            var result = DataDiffHelper.Diff("角色", lefts, rights);

            Assert.Equal("角色：a|b->b|c", result);
        }

        [Fact]
        public void 列表_string版本_无变化_返回空()
        {
            var result = DataDiffHelper.Diff("角色", new List<string> { "a", "b" }, new List<string> { "a", "b" });

            Assert.Equal("", result);
        }

        [Fact]
        public void 列表_struct版本_输出左右列表()
        {
            var result = DataDiffHelper.Diff("编号", new List<int> { 1, 2, 3 }, new List<int> { 2, 3, 4 });

            Assert.Equal("编号：1|2|3->2|3|4", result);
        }

        [Fact]
        public void 列表_struct版本_无变化_返回空()
        {
            var result = DataDiffHelper.Diff("编号", new List<int> { 1, 2 }, new List<int> { 1, 2 });

            Assert.Equal("", result);
        }

        [Fact]
        public void 列表_struct带字典_右侧输出新增项()
        {
            var dict = new Dictionary<int, string> { { 1, "一" }, { 2, "二" }, { 3, "三" }, { 4, "四" } };

            var result = DataDiffHelper.Diff("编号", new List<int> { 1, 2, 3 }, new List<int> { 2, 3, 4 }, dict);

            // 左侧原列表 + 右侧仅新增项（adds = [4]）
            Assert.Equal("编号：1=一|2=二|3=三->4=四", result);
        }

        [Fact]
        public void 列表_struct带字典_只有新增时右侧不空白()
        {
            var dict = new Dictionary<int, string> { { 1, "一" }, { 2, "二" } };

            var result = DataDiffHelper.Diff("编号", new List<int> { 1 }, new List<int> { 1, 2 }, dict);

            Assert.Equal("编号：1=一->2=二", result);
        }

        [Fact]
        public void 列表_func转换版本_输出新增项()
        {
            var dict = new Dictionary<int, string> { { 1, "一" }, { 2, "二" }, { 3, "三" } };

            var result = DataDiffHelper.Diff(
                "名称长度",
                new List<string> { "a", "bb" },
                new List<string> { "bb", "ccc" },
                s => s.Length,
                dict);

            Assert.Equal("名称长度：1=一|2=二->3=三", result);
        }

        #endregion

        #region JsonDiff

        [Fact]
        public void JsonDiff_相同_返回未修改()
        {
            var result = DataDiffHelper.JsonDiff("{\"a\":1}", "{\"a\":1}");

            Assert.Equal("未修改", result);
        }

        [Fact]
        public void JsonDiff_左侧为空_返回新增()
        {
            var result = DataDiffHelper.JsonDiff("", "{\"a\":1}");

            Assert.Equal("新增{\"a\":1}", result);
        }

        [Fact]
        public void JsonDiff_右侧为空_返回删除()
        {
            var result = DataDiffHelper.JsonDiff("{\"a\":1}", "");

            Assert.Equal("删除{\"a\":1}", result);
        }

        [Fact]
        public void JsonDiff_有差异_返回差异文档()
        {
            var result = DataDiffHelper.JsonDiff("{\"a\":1,\"b\":2}", "{\"a\":1,\"b\":3,\"c\":4}");

            // JsonNode.ToString() 输出缩进 JSON，且换行符随平台（Windows 为 \r\n），先规范化再比较
            var normalized = result.Replace("\r\n", "\n");

            Assert.Equal(
                "{\n  \"b\": [\n    2,\n    3\n  ],\n  \"c\": [\n    4\n  ]\n}",
                normalized);
        }

        #endregion
    }
}
