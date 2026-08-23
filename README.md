ToolGood.ReadyGo
===

欢迎使用`ToolGood.ReadyGo`！它是一款轻量级 ORM，基于 NPOCO 核心修改，
汇聚作者多年经验，简单、快捷，能增加代码的可读性。
支持 SqlServer、MySql、SQLite、Oracle、Access、DuckDB。

### 快速上手

```csharp
using ToolGood.ReadyGo;

var helper = SqlHelperFactory.OpenSqliteFile("test.db");
helper._TableHelper.CreateTable(typeof(User));

helper.Insert(new User { Name = "Ted", Age = 21 });
var user = helper.FirstOrDefault<User>("Where Name=@0", "Ted");
var users = helper.Where<User>().Where(x => x.Age > 18).OrderBy(x => x.Name).ToList();
```

### 功能简介

* 表操作：支持表创建、删除、截断，支持索引与唯一索引，定制 Attribute。
* Object 快速增删改：Insert / Update / Delete / Save。
* 快照局部更新：StartSnapshot 记录对象变更，只更新变更的列。
* 批量更新与删除：UpdateMany / DeleteMany 链式操作。
* 原生 SQL：支持 SQL 简化、分页查询、多结果集 FetchMultiple、一对多 FetchOneToMany。
* 动态查询：Where&lt;T&gt;() 链式，支持表达式、IfTrue* 条件开关、WhereIn / WhereLike / WhereExists。
* object 条件查询：以对象为条件执行 Select / Update / Delete / Count / Exists。
* 异步 API：全部核心操作均提供 _Async 异步版本。
* SQL 执行监控。

#### 1、数据表生成与删除

##### 1.1、简单的数据表操作

目前支持【表操作】的数据库有 SqlServer、MySql、SQLite。

```csharp
using ToolGood.ReadyGo.Attributes;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}

using ToolGood.ReadyGo;

var helper = SqlHelperFactory.OpenSqliteFile(dbFile);
var table = helper._TableHelper;
table.TryCreateTable(typeof(User));    // 表不存在则创建
table.CreateTable(typeof(User));       // 创建表
table.CreateTableIndex(typeof(User));  // 创建索引
table.DropTable(typeof(User));         // 删除表
table.TruncateTable(typeof(User));     // 清空表
```

##### 1.2、ToolGood.ReadyGo.Attributes 介绍

在`ToolGood.ReadyGo.Attributes`命名空间内提供以下几种 Attribute：

* ExplicitColumns 用于 Class，所有列（Property）必须显式映射。
* Table   用于 Class，定义表名、schema 名、表名修饰 TAG 名。
* PrimaryKey 用于 Class，定义主键名、自动增加、Sequence 名。
* Column 用于 Property，定义列。
* ResultColumn 用于 Property，定义返回列。
* Ignore 用于 Property，忽略该属性。
* Index 用于 Class，定义索引。
* Unique 用于 Class，定义唯一索引。
* Required 用于 Property，定义非空列。
* FieldLength 用于 Property，定义列长度。
* Text 用于 Property，定义 TEXT 类型列。
* DefaultValue 用于 Property，定义默认值。

#### 2、数据表操作

##### 2.1、增删改操作

```csharp
var helper = SqlHelperFactory.OpenSqliteFile(dbFile);
User u = new User() {
    ...
};
helper.Insert(u);
helper.Update(u);
helper.Save(u);
helper.Delete(u);
helper.Update<User>("Set [Name]=@0 WHERE [Id]=@1", "Test", 1);
helper.Delete<User>("WHERE [Id]=@0", 1);
helper.DeleteById<User>(1);
```

##### 2.2、使用事务

```csharp
using (var tran = helper.UseTransaction()) {
    ...
    tran.Complete();  // 提交事务；若不调用 Complete()，Dispose 时将回滚
}
```

##### 2.3、快照局部更新

```csharp
var user = helper.FirstOrDefault<User>("Where Id=@0", 1);  // Name = "Ted", Age = 21

var snapshot = helper.StartSnapshot(user);  // 开始记录对象变更

user.Name = "Bobby";
user.Age = 21;  // 与快照一致，不算变更

helper.Update(user, snapshot);   // 只更新 Name 列
helper.Update(user, new[] { "Name" });  // 或指定列更新
```

##### 2.4、批量更新与删除

```csharp
helper.UpdateMany<User>().Where(x => x.Age > 30).ExcludeDefaults().Execute(new User { Vip = true });
helper.DeleteMany<User>().Where(x => x.Age < 18).Execute();
```

#### 3、SQL查询

##### 3.1、单个查询

```csharp
var helper = SqlHelperFactory.OpenSqliteFile(dbFile);
var user1 = helper.FirstOrDefault<User>("SELECT * FROM Users where [Id]=@0", 1);
var user2 = helper.FirstOrDefault<User>("Where [Id]=@0", 1);  // 简化 SQL

var dataset = helper.ExecuteDataSet("SELECT * FROM Users where [Id]=@0", 1);
var datatable = helper.ExecuteDataTable("SELECT * FROM Users where [Id]=@0", 1);

var userCount = helper.Count<User>("SELECT COUNT(*) FROM Users Where [UserType]=@0", 1);
var userCount2 = helper.ExecuteScalar<int>("SELECT COUNT(*) FROM Users Where [UserType]=@0", 1);
```

##### 3.2、列表查询

```csharp
var helper = SqlHelperFactory.OpenSqliteFile(dbFile);
var users = helper.Select<User>("SELECT * FROM Users Where [UserType]=@0", 1);
var users2 = helper.Select<User>(20, "SELECT * FROM Users Where [UserType]=@0", 1);   // 取前 20 条
var users3 = helper.Select<User>(20, 0, "SELECT * FROM Users Where [UserType]=@0", 1); // 取 20 条、跳过 0 条（参数：limit, offset）
var usersPage = helper.Page<User>(1, 20, "SELECT * FROM Users Where [UserType]=@0", 1); // 分页（含总页数）
```

##### 3.3、多结果集与一对多

```csharp
var (users, addresses) = helper.SelectMultiple<User, Address>(
    "select * from users;select * from addresses;");
var data = helper.SelectMultiple<User, Address, Tuple<List<User>, List<Address>>>(
    (u, a) => Tuple.Create(u, a), sql);  // 回调方式组合结果

var userInfos = helper.SelectOneToMany<UserInfo>(x => x.Addresses, manySql);
```

##### 3.4、简化 SQL

```csharp
var helper = SqlHelperFactory.OpenSqliteFile(dbFile);
var users1 = helper.Select<User>("SELECT * FROM Users Where [UserType]=@0", 1);
var users2 = helper.Select<User>("FROM Users Where [UserType]=@0", 1);
var users3 = helper.Select<User>("Where [UserType]=@0", 1);

helper.Update<User>("UPDATE Users Set [Name]=@0 WHERE [Id]=@1", "Test", 1);
helper.Update<User>("Set [Name]=@0 WHERE [Id]=@1", "Test", 1);
```

#### 4、动态查询

##### 4.1、查询

```csharp
public User FindUser(int userId, string userName, string nickName)
{
    var helper = SqlHelperFactory.OpenMysql("127.0.0.1", "web", "root", "123456");
    return helper.Where<User>()
        .IfTrueWhere(userId > 0, u => u.Id == userId)         // 条件成立才追加 Where
        .IfTrueWhere(userName != null, u => u.UserName == userName)
        .IfTrueWhere(nickName != null, u => u.NickName == nickName)
        .FirstOrDefault();
}
```

方法有：

* 构建：`Where`、`WhereSql`、`OrderBy`、`OrderByDescending`、`Limit`、`SkipTake`、`Distinct`
* 执行：`ToList`、`Select`（等效 ToList）、`First`、`Single`、`Count`、`ToPage`
* 动态条件（IfTrue* 条件成立才生效）：`IfTrueWhere`、`IfTrueOrderBy`、`IfTrueOrderByDescending`、`IfTrueLimit`、`IfTrueWhereIn`、`IfTrueWhereNotIn`、`IfTrueWhereLike`、`IfTrueWhereLikeStart`、`IfTrueWhereLikeEnd`、`IfTrueWhereExists`、`IfTrueWhereNotExists`
* 常用扩展：`WhereIn`、`WhereNotIn`、`WhereLike`（%关键字%）、`WhereLikeStart`、`WhereLikeEnd`、`WhereExists`、`WhereNotExists`

#### 5、object 条件查询

以对象为条件，属性为默认值时忽略该条件：

```csharp
var user = helper.FirstOrDefault<User>(new { Id = 1 });
var users = helper.Select<User>(new { UserType = 1, State = true });
helper.Update<User>(new { NickName = "新昵称" }, new { Id = 1 });   // set 对象, 条件对象
helper.Delete<User>(new { Id = 1 });
var count = helper.Count<User>(new { UserType = 1 });
var exists = helper.Exists<User>(new { UserName = "Ted" });
```

#### 6、SQL执行监控

##### 6.1、上一次 SQL 执行语句

```csharp
var sql = helper._Sql.LastSQL;          // 上次 SQL 语句
var args = helper._Sql.LastArgs;        // 上次 SQL 参数
var cmd = helper._Sql.LastCommand;      // 上次 SQL（带参数格式化）
var err = helper._Sql.LastErrorMessage; // 上次错误信息
```

#### 7、异步 API

所有核心操作均提供 `_Async` 后缀的异步版本：

`Execute`、`ExecuteScalar`、`ExecuteDataTable`、`Exists`、`Count`、`Select_Count`、`Select`、`SelectPage`、
`Page`、`SQL_FirstOrDefault`、`SQL_Select`、`SQL_Page`、`SelectOneToMany`、`SelectMultiple`、`FirstOrDefault`、
`Insert`、`InsertList`、`Update`（含快照/指定列/条件）、`Delete`、`DeleteById`、`Save`。

```csharp
var users = await helper.Select_Async<User>("Where [UserType]=@0", 1);
await helper.Update_Async(user, snapshot);   // 异步快照局部更新
```

#### 8、数据库连接工厂 SqlHelperFactory

```csharp
var helper = SqlHelperFactory.OpenDatabase(connectionString, "MySql.Data.MySqlClient", SqlType.MySql);
var helper = SqlHelperFactory.OpenSqlServer(server, database, user, pwd);
var helper = SqlHelperFactory.OpenSqlServer2012(server, port, database, user, pwd);
var helper = SqlHelperFactory.OpenMysql(server, database, user, pwd);
var helper = SqlHelperFactory.OpenOracle(server, port, serviceName, user, pwd);
var helper = SqlHelperFactory.OpenSqliteFile(filePath);
var helper = SqlHelperFactory.OpenMsSqliteFile(filePath, pwd);
var helper = SqlHelperFactory.OpenDuckDbFile(filePath);
var helper = SqlHelperFactory.OpenAccessFile(filePath);      // 32 位
var helper = SqlHelperFactory.OpenAccessFile64x(filePath);   // 64 位
```
