# ToolGood.ReadyGo

ToolGood.ReadyGo 是一款轻量级高性能 Micro-ORM，基于 NPoco 核心修改，汇聚作者多年经验，简单、快捷，能增加代码的可读性。支持 SqlServer、MySql、SQLite、PostgreSQL、Oracle、Firebird 等数据库。

## 特性

- 链式动态查询：`Where<T>()` 支持表达式、`IfTrue*` 条件开关、`WhereIn` / `WhereLike` / `WhereExists` 等扩展
- 批量更新与删除：`UpdateMany<T>()` / `DeleteMany<T>()` 链式操作
- 快照局部更新：`StartSnapshot` 记录对象变更，只更新变更的列
- 简化 SQL：省去 `SELECT * FROM` 前缀，直接写条件
- 完整异步 API：所有核心操作均提供 `_Async` 版本
- 支持对象条件查询、分页、多结果集、一对多映射
- SQL 执行监控

## 支持框架与数据库

- 框架：.NET 8.0 / 9.0 / 10.0
- 数据库：SqlServer、MySql、SQLite、PostgreSQL、Oracle、Firebird、DuckDb、Access

## 安装

```bash
dotnet add package ToolGood.ReadyGo
```

## 快速开始

```csharp
using ToolGood.ReadyGo;

// 1. 创建连接
var helper = SqlHelperFactory.OpenMsSqliteFile("test.db");

// 2. 建表（表不存在时创建）
helper._TableHelper.TryCreateTable(typeof(User));

// 3. 插入
helper.Insert(new User { Name = "Ted", Age = 21 });

// 4. 链式查询
var users = helper.Where<User>()
    .Where(x => x.Age > 18)
    .OrderBy(x => x.Name)
    .ToList();

// 5. 分页（SelectPage 为 ToPage 别名）
var page = helper.Where<User>().OrderBy(x => x.Age).SelectPage(1, 10);

// 6. 异步查询
var asyncList = await helper.Where<User>(x => x.Age > 18).ToList_Async();
```

## 数据库连接工厂

```csharp
var helper = SqlHelperFactory.OpenDatabase(connectionString, "MySql.Data.MySqlClient", SqlType.MySql);
var helper = SqlHelperFactory.OpenSqlServer(server, database, user, pwd);
var helper = SqlHelperFactory.OpenMysql(server, database, user, pwd);
var helper = SqlHelperFactory.OpenOracle(server, port, serviceName, user, pwd);
var helper = SqlHelperFactory.OpenSqliteFile(filePath);        // System.Data.SQLite 驱动
var helper = SqlHelperFactory.OpenMsSqliteFile(filePath, pwd); // Microsoft.Data.Sqlite 驱动（支持密码）
var helper = SqlHelperFactory.OpenDuckDbFile(filePath);
var helper = SqlHelperFactory.OpenAccessFile(filePath);        // 32 位
```

## 数据表操作

```csharp
var table = helper._TableHelper;
table.TryCreateTable(typeof(User));    // 表不存在则创建
table.CreateTable(typeof(User));       // 创建表
table.CreateTableIndex(typeof(User));  // 创建索引
table.DropTable(typeof(User));         // 删除表
table.TruncateTable(typeof(User));     // 清空表
```

## 数据增删改

```csharp
helper.Insert(u);
helper.Update(u);
helper.Save(u);                        // 存在则更新，否则插入
helper.Delete(u);
helper.DeleteById<User>(1);

helper.UpdateList(list);               // 批量更新（按主键）
helper.SaveList(list);                 // 批量保存：新对象插入，已存在更新
helper.InsertList(list);               // 批量插入

helper.Update<User>("Set [Name]=@0 WHERE [Id]=@1", "Test", 1);
helper.Delete<User>("WHERE [Id]=@0", 1);
```

### 使用事务

```csharp
using (var tran = helper.UseTransaction()) {
    ...
    tran.Complete();  // 提交事务；若不调用 Complete()，Dispose 时回滚
}
```

### 快照局部更新

```csharp
var user = helper.FirstOrDefault<User>("Where Id=@0", 1);   // Name = "Ted", Age = 21
var snapshot = helper.StartSnapshot(user);                   // 开始记录变更

user.Name = "Bobby";

helper.Update(user, snapshot);              // 只更新变更的列
helper.Update(user, new[] { "Name" });      // 或指定列更新
```

## 动态查询（链式查询）

### 基本用法

```csharp
helper.Where<User>()                      // 入口
    .Where(x => x.Age > 18)               // 表达式条件
    .WhereSql("Name = @0", "Ted")         // 原始 SQL 条件
    .OrderBy(x => x.Name)                 // 排序
    .Limit(10)                            // 取前 10 条
    .ToList();
```

### 条件开关（IfTrue*，条件成立才追加）

```csharp
public User FindUser(int userId, string userName)
{
    return helper.Where<User>()
        .IfTrueWhere(userId > 0, u => u.Id == userId)
        .IfTrueWhere(userName != null, u => u.UserName == userName)
        .FirstOrDefault();
}
```

### 方法清单

- 构建：`Where`、`WhereSql`、`OrderBy`、`OrderByDescending`、`Limit`、`SkipTake`、`Distinct`
- 执行：`ToList`、`First`、`Single`、`Count`、`Any`、`ToPage`（别名 `SelectPage`）
- 常用扩展：`WhereIn`、`WhereNotIn`、`WhereLike`、`WhereLikeStart`、`WhereLikeEnd`、`WhereExists`、`WhereNotExists`
- 异步：上述执行方法均有 `_Async` 版本（如 `ToList_Async`、`FirstOrDefault_Async`）

## 批量更新与删除

```csharp
helper.UpdateMany<User>()
    .Where(x => x.Age > 30)
    .ExcludeDefaults()                                  // 跳过默认值字段
    .OnlyFields(x => new { x.Vip })                     // 只更新指定字段
    .Execute(new User { Vip = true });

helper.DeleteMany<User>().Where(x => x.Age < 18).Execute();
```

## object 条件查询

以对象为条件，属性为默认值时忽略该条件：

```csharp
var user = helper.FirstOrDefault<User>(new { Id = 1 });
var users = helper.Select<User>(new { UserType = 1, State = true });
helper.UpdateBy<User>(new { NickName = "新昵称" }, new { Id = 1 });  // set 对象, 条件对象
helper.Delete<User>(new { Id = 1 });
var count = helper.Count<User>(new { UserType = 1 });
var exists = helper.Exists<User>(new { UserName = "Ted" });
```

## 原生 SQL 与简化 SQL

```csharp
// 完整 SQL
var users = helper.Select<User>("SELECT * FROM Users WHERE [UserType]=@0", 1);

// 简化 SQL：可省略 "SELECT * FROM"
var users2 = helper.Select<User>("FROM Users WHERE [UserType]=@0", 1);
var users3 = helper.Select<User>("WHERE [UserType]=@0", 1);

// 分页
var page = helper.Page<User>(1, 20, "SELECT * FROM Users WHERE [UserType]=@0", 1);

// 多结果集 / 一对多
var (users, addresses) = helper.SelectMultiple<User, Address>(
    "select * from users;select * from addresses;");
var userInfos = helper.SelectOneToMany<UserInfo>(x => x.Addresses, manySql);
```

## 异步 API

所有核心操作均提供 `_Async` 后缀版本，例如：

```csharp
var users = await helper.Select_Async<User>("WHERE [UserType]=@0", 1);
await helper.Update_Async(user, snapshot);          // 异步快照局部更新
var list = await helper.Where<User>().ToList_Async(); // 链式异步查询
```

## SQL 执行监控

```csharp
var sql = helper._Sql.LastSQL;          // 上次 SQL 语句
var args = helper._Sql.LastArgs;        // 上次 SQL 参数
var cmd  = helper._Sql.LastCommand;     // 上次 SQL（带参数格式化）
var err  = helper._Sql.LastErrorMessage; // 上次错误信息
```

## Attribute 映射

在 `ToolGood.ReadyGo.Attributes` 命名空间内：

- `Table`：定义表名、schema 名、数据库名
- `PrimaryKey`：定义主键（默认自增），支持复合主键
- `Column`：定义列名与备注
- `ResultColumn`：定义返回列（只读，不参与 INSERT/UPDATE）
- `Ignore`：忽略该属性
- `Required`：定义非空列
- `FieldLength`：定义列长度（decimal 支持长度与小数位）
- `Index` / `Unique`：定义索引与唯一索引
- `ExplicitColumns`：所有列必须显式映射
- 更多便捷特性（`Bool2String`、`DateTime2String`、`Enum2String` 等）参见完整文档

## 许可证

详见 [License.txt](License.txt)。
