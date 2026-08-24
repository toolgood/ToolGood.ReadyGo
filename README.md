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
* Object 快速增删改：Insert / Update / Delete / Save，批量 SaveList / UpdateList / InsertList。
* 快照局部更新：StartSnapshot 记录对象变更，只更新变更的列。
* 批量更新与删除：UpdateMany / DeleteMany 链式操作，集合批量 UpdateList（含快照）/ SaveList。
* 原生 SQL：支持 SQL 简化、分页查询、多结果集 SelectMultiple、一对多 SelectOneToMany。
* 动态查询：Where&lt;T&gt;() 链式，支持表达式、IfTrue* 条件开关、WhereIn / WhereLike / WhereExists。
* object 条件查询：以对象为条件执行 Select / Update / Delete / Count / Exists。
* 异步 API：全部核心操作均提供 _Async 异步版本。
* SQL 执行监控。

#### 1、数据表生成与删除

##### 1.1、简单的数据表操作

目前支持【表操作】的数据库有 SqlServer、SqlServer2012、MySql、MariaDb、SQLite、DuckDb、Oracle、PostgreSQL、FirebirdDb。

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

在`ToolGood.ReadyGo.Attributes`命名空间内提供以下几种 Attribute（标注「类级」的可用于 Class，标注「属性级」的可用于 Property/Field）。

**类级：**

* `Table`：定义表名、schema 名、数据库名。
  `TableAttribute(string tableName)` / `TableAttribute(string tableName, string schemaName)` / `TableAttribute(string tableName, string schemaName, string databaseName)`
* `PrimaryKey`：定义主键名（默认自增）、Sequence 名。
  `PrimaryKeyAttribute(string primaryKey)` / `PrimaryKeyAttribute(string[] primaryKey)`（复合主键）
* `Index`：定义索引（可多次使用）。`IndexAttribute(string column, params string[] columns)`
* `Unique`：定义唯一索引（可多次使用）。`UniqueAttribute(string column, params string[] columns)`
* `ExplicitColumns`：所有列必须显式映射（无参）。

**属性级：**

* `Column`：定义列名与备注。`ColumnAttribute(string name)` / `ColumnAttribute(string name, string comment)` / `ColumnAttribute()`
* `ResultColumn`：定义返回列（只读，不参与 INSERT/UPDATE）。`ResultColumnAttribute()` / `ResultColumnAttribute(string name)`
* `Ignore`：忽略该属性（无参）。
* `Required`：定义非空列。`RequiredAttribute(bool required = true)`
* `FieldLength`：定义列长度。`FieldLengthAttribute(int length)` / `FieldLengthAttribute(int length, int pointLength)`（decimal 长度与小数位）
* `Text` / `MediumText` / `LongText`：定义 TEXT 类型列（无参）。
* `DefaultValue`：定义默认值（默认 SQL）。`DefaultValueAttribute(string defaultstring)`

> 更多特性（`Alias`、`ColumnType`、`ComputedColumn`、`VersionColumn`、`Reference`、`SerializedColumn`、`Date2String`、`Date2Int`、`DateTime2Long`、`DateTime2Timestamp`、`Numeric2Int`、`Numeric2Long`、`NumericArray2Bytes`、`Enum2String`、`ComplexMapping`、`PersistedType`、`Construct`、`StatementPreparationHook` 以及一系列预定义长度的便捷特性如 `PhoneLength`、`UserNameLength`、`EmailLength`、`UrlLength` 等）请参见 `skills/toolgood-readygo/SKILL.md` 中的完整清单。

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
helper.UpdateList(list);       // 批量更新（按主键更新全部列，或传快照仅更新变更列）
helper.SaveList(list);         // 批量保存：新对象插入，已存在对象更新
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

`Execute`、`ExecuteScalar`、`ExecuteDataTable`、`ExecuteDataSet`、`Exists`、`Count`、`Select_Count`、`Select`、`SelectPage`、
`Page`、`SQL_FirstOrDefault`、`SQL_Select`、`SQL_Page`、`SelectOneToMany`、`SelectMultiple`、`FirstOrDefault`、
`Insert`、`InsertList`、`Update`（含快照/指定列/条件）、`UpdateList`（含快照）、`Delete`、`DeleteById`、`Save`、`SaveList`、`UseTransaction`。

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

> **驱动选择说明**
>
> * `OpenDatabase(connectionString, providerName, type)` 的 `providerName` 用于精确定位驱动：优先加载与 `providerName` 匹配的 `DbProviderFactory`，匹配失败再按默认候选顺序回退。同一 `SqlType` 存在多个驱动时（如 SQLite 同时支持 System.Data.SQLite 与 Microsoft.Data.Sqlite），应传入对应的 `providerName`（如 `"Microsoft.Data.Sqlite"`），避免选错驱动。
> * `OpenSqliteFile` 使用 System.Data.SQLite 驱动；`OpenMsSqliteFile` 使用 Microsoft.Data.Sqlite 驱动（支持密码，密码会转义后写入连接字符串）。
> * `OpenMysql` 自动识别已加载的驱动（MySql.Data / MySqlConnector），据此选择连接串关键字（MySql.Data 用 `charset=utf8mb4;AllowUserVariables`，MySqlConnector 用 `CharSet=utf8mb4`），并自动附加合适的 `SslMode`/`AllowPublicKeyRetrieval` 选项。
> * `OpenSqlServerFile` 默认 LocalDB 实例为 `(LocalDB)\MSSQLLocalDB`。
