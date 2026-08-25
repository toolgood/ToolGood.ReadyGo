# ToolGood.ReadyGo

ToolGood.ReadyGo 是一个高性能 Micro-ORM，支持 SQL Server、MySQL、Sqlite、PostgreSQL、Oracle、Firebird。基于 NPoco 核心，提供 ReadyGo 风格 API。

## 特性

- 链式查询：`Where<T>()` 后接 Where/WhereSql/OrderBy/Limit 及 IfTrue*、WhereIn、WhereLike 等扩展
- 同步与异步接口：`ToList`/`ToList_Async`、`FirstOrDefault`/`FirstOrDefault_Async`、`ToPage`/`ToPage_Async`（别名 `SelectPage`/`SelectPage_Async`）等
- 批量更新 `UpdateMany<T>()`、批量删除 `DeleteMany<T>()`
- 表达式树转 SQL，支持 `[Table]`、`[Column]`、`[PrimaryKey]` 等特性映射

## 支持框架

- .NET 8.0 / 9.0 / 10.0

## 快速开始

```csharp
using ToolGood.ReadyGo;

var helper = SqlHelperFactory.OpenMsSqliteFile("test.db");
helper._TableHelper.TryCreateTable(typeof(User));

// 链式查询
var list = helper.Where<User>(q => q.Age >= 30)
                 .OrderBy(q => q.Age)
                 .ToList();

// 分页（SelectPage 为 ToPage 别名）
var page = helper.Where<User>().OrderBy(q => q.Age).SelectPage(1, 10);

// 异步查询
var asyncList = await helper.Where<User>(q => q.Age >= 30).ToList_Async();

// 批量更新
helper.UpdateMany<User>().Where(q => q.Age > 30).ExcludeDefaults()
      .Execute(new User { Vip = true });

// 批量删除
helper.DeleteMany<User>().Where(q => q.Age < 18).Execute();
```

## 构建与测试

```bash
dotnet build ToolGood.ReadyGo.sln
dotnet test ToolGood.ReadyGo.Tests/ToolGood.ReadyGo.Tests.csproj
```

## 许可证

详见 [License.txt](License.txt)。
