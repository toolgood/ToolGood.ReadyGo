---
name: "toolgood-readygo"
description: "基于NPoco的.NET轻量级ORM框架。在处理ToolGood.ReadyGo数据库操作、CRUD、LINQ查询或多数据库支持时调用。"
---

# ToolGood.ReadyGo - 轻量级ORM框架

## 项目概述

ToolGood.ReadyGo 是一个轻量级、高性能的 .NET ORM（对象关系映射）框架，由 NPOCO 核心修改而来。它提供了简单而强大的数据库操作 API，支持多种数据库系统，包括 SqlServer、MySql、SQLite、Oracle、Access、DuckDB 等。

### 核心特性

- **轻量快速**：直接执行 SQL，开销最小
- **多数据库支持**：SqlServer、MySql、MariaDb、SQLite、Oracle、PostgreSQL、FirebirdDb、Access、DuckDB
- **LINQ 表达式支持**：使用 Lambda 表达式构建类型安全的动态查询
- **异步操作**：所有核心操作均提供 `_Async` 后缀的异步版本
- **动态 SQL 构建**：`Where<T>()` 流式 API 构建复杂查询
- **object 条件查询**：以对象为条件执行 Select / Update / Delete / Count / Exists
- **快照局部更新**：`StartSnapshot` 记录对象变更，只更新变更的列
- **事务支持**：内置事务管理
- **表管理**：编程方式创建、删除、截断表

## 核心架构

### 目录结构

```
ToolGood.ReadyGo/
├── SqlHelper.cs              # 核心数据库操作类（Execute/Select/CRUD/事务）
├── SqlHelper.Async.cs        # 异步操作实现（_Async 后缀）
├── SqlHelper.Object.cs       # object 条件查询（含主键重载）
├── SqlHelper.Where.cs        # 动态查询入口（Where/UpdateMany/DeleteMany）
├── SqlHelperFactory.cs       # 创建 SqlHelper 实例的工厂
├── SqlUtil.cs                # SQL 工具函数
├── SqlType.cs                # 数据库类型枚举
├── DatabaseProvider.cs       # Provider/数据库类型解析
├── Attributes/               # 实体映射特性
├── Core/                     # NPoco 核心引擎（含 Linq/ 动态查询）
├── Gadget/                   # 表管理、配置（SqlConfig）、SQL 记录（SqlRecord）
├── Internals/                # 内部辅助类
├── Exceptions/               # 异常类型（SqlExecuteException 等）
└── ConnectionStrings/        # JournalMode 等
```

### 核心类

#### SqlHelper
所有数据库操作的主入口点（`partial class`，分布在 SqlHelper.cs / SqlHelper.Async.cs / SqlHelper.Object.cs / SqlHelper.Where.cs 中）：
- CRUD 操作（Insert、Update、Delete、Save、InsertList、UpdateList、SaveList）
- 查询执行（Execute、ExecuteScalar、ExecuteDataTable、ExecuteDataSet）
- 查询与分页（Select、SelectPage、Page、SelectOneToMany、SelectMultiple）
- 计数与存在（Count、Select_Count、Exists）
- 动态查询（Where、UpdateMany、DeleteMany）
- object 条件查询
- 快照局部更新（StartSnapshot）
- 事务管理（UseTransaction）
- 动态表名（GetTableName）

#### SqlHelperFactory
创建 SqlHelper 实例的工厂类，支持多种数据库连接。

#### IQueryProvider<T>（由 Where<T>() 返回）
构建动态 SQL 查询的流式 API，提供条件、排序、分页、投影等能力。

## 快速上手

```csharp
using ToolGood.ReadyGo;

var helper = SqlHelperFactory.OpenSqliteFile("test.db");
helper._TableHelper.CreateTable(typeof(User));

helper.Insert(new User { Name = "Ted", Age = 21 });
var user = helper.FirstOrDefault<User>("Where Name=@0", "Ted");
var users = helper.Where<User>().Where(x => x.Age > 18).OrderBy(x => x.Name).ToList();
```

## 实体特性（Attributes）参考

位于 `ToolGood.ReadyGo.Attributes` 命名空间。以下按作用目标分组。

### 类级特性（用于 Class）

#### Table
定义表名、schema 名。

```csharp
[Table("Users")]                          // 仅表名
[Table("Users", "dbo")]                   // 表名 + schema
```

- `TableAttribute(string tableName)`
- `TableAttribute(string tableName, string schemaName)`

#### PrimaryKey
定义主键名（默认自增）、Sequence 名。

```csharp
[PrimaryKey("Id")]                          // 单主键，默认自增
[PrimaryKey("Id", AutoIncrement = false)]   // 非自增
[PrimaryKey(new[] { "UserId", "RoleId" })]  // 复合主键
```

- `PrimaryKeyAttribute(string primaryKey)` —— 默认 `AutoIncrement = true`
- `PrimaryKeyAttribute(string[] primaryKey)` —— 复合主键
- 属性：`SequenceName`、`AutoIncrement`、`UseOutputClause`

#### Index
定义索引，可多次使用。

```csharp
[Index("Name")]
[Index("UserId", "Status")]   // 复合索引
```

- `IndexAttribute(string column, params string[] columns)`

#### Unique
定义唯一索引，可多次使用。

```csharp
[Unique("Email")]
[Unique("UserId", "RoleId")]
```

- `UniqueAttribute(string column, params string[] columns)`

#### ExplicitColumns
所有列（Property/Field）必须显式映射。

```csharp
[ExplicitColumns]
public class User { ... }
```

- 无参

#### PersistedType
指定持久化类型。

- `PersistedTypeAttribute(Type persistedType)`

#### StatementPreparationHook
抽象特性，用于声明式 SQL 改写钩子。

- 抽象属性：`IAlterStatementHook AlterStatementHook`

### 属性级特性（用于 Property/Field）

#### Column
定义列名与备注。

```csharp
[Column("user_name")]
[Column("user_name", "用户名字段")]
```

- `ColumnAttribute(string name)`
- `ColumnAttribute(string name, string comment)`
- `ColumnAttribute()`
- 属性：`Name`、`Comment`、`ForceToUtc`、`ExactNameMatch`

#### ResultColumn
定义返回列（只读，不参与 INSERT/UPDATE）。

```csharp
[ResultColumn]
public int OrderCount { get; set; }
```

- `ResultColumnAttribute()` / `ResultColumnAttribute(string name)`

#### Ignore
忽略该属性。

- 无参

#### Alias
定义别名。

- `AliasAttribute(string alias)`

#### ColumnType
指定列数据类型。

- `ColumnTypeAttribute(Type type)`

#### ComputedColumn
标记计算列。

- `ComputedColumnAttribute()`
- `ComputedColumnAttribute(string name)`
- `ComputedColumnAttribute(ComputedColumnType computedColumnType)`
- `ComputedColumnAttribute(string name, ComputedColumnType computedColumnType)`

#### VersionColumn
标记并发版本列。

- `VersionColumnAttribute()` —— 默认 `VersionColumnType.Number`
- `VersionColumnAttribute(VersionColumnType versionColumnType)`
- `VersionColumnAttribute(string name, VersionColumnType versionColumnType)`

#### Reference
标记关系引用（外键等）。

- `ReferenceAttribute()` —— 默认 `ReferenceType.Foreign`
- `ReferenceAttribute(ReferenceType referenceType)`
- 属性：`ReferenceMemberName`、`ColumnName`

#### SerializedColumn
标记序列化列。

- `SerializedColumnAttribute()` / `SerializedColumnAttribute(string name)`

#### Bool2Int
bool 以 0/1 整数保存（需整数列）。如 `true` → 1，`false` → 0。

```csharp
[Bool2Int]
public bool IsVip { get; set; }
```

- `Bool2IntAttribute()` / `Bool2IntAttribute(string name)`

#### Bool2String
bool 以 "true"/"false" 文本保存（需文本列）。如 `true` → `"true"`，`false` → `"false"`。

- `Bool2StringAttribute()` / `Bool2StringAttribute(string name)`

#### Date2String
只保存日期（数据库中仅存 `yyyy-MM-dd`）。

- `Date2StringAttribute()` / `Date2StringAttribute(string name)`

#### DateTime2String
时间以 "yyyy-MM-dd HH:mm:ss" 文本保存（需文本列）。支持 `DateTime` / `DateTimeOffset`。

```csharp
[DateTime2String]
public DateTime CreateTime { get; set; }
```

- `DateTime2StringAttribute()` / `DateTime2StringAttribute(string name)`

#### Date2Int
只保存日期为 yyyyMMdd 整数（不保存时间），存为 int。
如 `2026-08-23` → `20260823`。

```csharp
[Date2Int]
public DateTime TradeDate { get; set; }
```

- `Date2IntAttribute()` / `Date2IntAttribute(string name)`

#### DateTime2Long
时间以 yyyyMMddHHmmss 整数保存（秒级精度，需 long 存储）。
如 `2026-08-23 15:30:45` → `20260823153045`。

```csharp
[DateTime2Long]
public DateTime TradeTime { get; set; }
```

- `DateTime2LongAttribute()` / `DateTime2LongAttribute(string name)`

#### DateTime2Timestamp
以 Unix 时间戳（UTC 基准）保存，精度支持秒和毫秒，需 long 存储。

```csharp
[DateTime2Timestamp]                                   // 秒级（默认）
[DateTime2Timestamp(TimestampPrecision.Milliseconds)]  // 毫秒级
[DateTime2Timestamp("create_time", TimestampPrecision.Milliseconds)]
```

- `DateTime2TimestampAttribute(TimestampPrecision precision = TimestampPrecision.Seconds)`
- `DateTime2TimestampAttribute(string name, TimestampPrecision precision = TimestampPrecision.Seconds)`

#### Numeric2Int
小数转 int 保存（保存时 ×10^scale 四舍五入，读取时 ÷10^scale），值超出 int 范围会抛异常。

```csharp
[Numeric2Int(2)]            // 1.23 存为 123
[Numeric2Int("price", 2)]
```

- `Numeric2IntAttribute(int scale = 2)`
- `Numeric2IntAttribute(string name, int scale = 2)`

#### Numeric2Long
小数转 long 保存（保存时 ×10^scale 四舍五入，读取时 ÷10^scale），适合大数。

```csharp
[Numeric2Long(2)]           // 1.23 存为 123
[Numeric2Long("price", 2)]
```

- `Numeric2LongAttribute(int scale = 2)`
- `Numeric2LongAttribute(string name, int scale = 2)`

#### NumericArray2Bytes
将 `float[]` / `double[]` / `int[]` 及其 `List<T>` 以 byte[]（BLOB 列）保存。

- `NumericArray2BytesAttribute()` / `NumericArray2BytesAttribute(string name)`

#### NumericArray2String
将 `int[]` / `long[]` / `double[]` / `decimal[]` 等数值数组及其 `List<T>` 以分隔符文本保存（需文本列）。默认逗号分隔，支持自定义分隔符。

- `NumericArray2StringAttribute(string separator = ",")`
- `NumericArray2StringAttribute(string name, string separator)`

#### Enum2Int
enum 以底层整数值（int）保存（需 int 列）。如 `UserState.Vip` → 2。

- `Enum2IntAttribute()` / `Enum2IntAttribute(string name)`

#### Enum2Long
enum 以底层长整数值（long）保存（需 bigint 列）。如 `UserState.Vip` → 2。

- `Enum2LongAttribute()` / `Enum2LongAttribute(string name)`

#### Enum2String
枚举显示名称（用于枚举类型）。

- 无参

#### DictionaryUintUint2Bytes
将 `Dictionary<uint, uint>` 以 byte[]（BLOB 列）保存，按键升序、差值压缩存储。

- `DictionaryUintUint2BytesAttribute()` / `DictionaryUintUint2BytesAttribute(string name)`

#### StringArray2String
将 `List<string>` / `string[]` 以分隔符文本保存（需文本列）。默认逗号分隔，支持自定义分隔符与转义（`\` 与分隔符前加 `\`）。

- `StringArray2StringAttribute(string separator = ",")`
- `StringArray2StringAttribute(string name, string separator)`

#### ComplexMapping
复杂映射。

- `ComplexMappingAttribute()` / `ComplexMappingAttribute(string customPrefix)`

#### Construct
标记构造方法（用于构造函数）。

- 无参

### 建表特性（用于 Property，影响表结构）

#### Required
非空列。

- `RequiredAttribute(bool required = true)`

#### FieldLength
列长度。

```csharp
[FieldLength(50)]     // 长度 50
[FieldLength(10, 2)]  // decimal(10, 2)
```

- `FieldLengthAttribute(int length)`
- `FieldLengthAttribute(int length, int pointLength)`

#### Text / MediumText / LongText
TEXT 类型列。

- 均无参

#### DefaultValue
默认值（默认 SQL）。

- `DefaultValueAttribute(string defaultstring)`

## 数据库连接

### 基本连接

```csharp
// SQLite
var helper = SqlHelperFactory.OpenSqliteFile("path/to/database.db");

// Microsoft.Data.Sqlite（支持密码）
var helper = SqlHelperFactory.OpenMsSqliteFile("path/to/database.db", "pwd");

// SqlServer
var helper = SqlHelperFactory.OpenSqlServer("server", "database", "user", "password");
var helper = SqlHelperFactory.OpenSqlServer("server", 1433, "database", "user", "password");

// MySQL
var helper = SqlHelperFactory.OpenMysql("server", "database", "user", "password");
var helper = SqlHelperFactory.OpenMysql("server", 3306, "database", "user", "password");

// Oracle
var helper = SqlHelperFactory.OpenOracle("server", 1521, "serviceName", "user", "password");

// DuckDB（不支持密码，DuckDB 加密需用 ATTACH ... ENCRYPTION_KEY）
var helper = SqlHelperFactory.OpenDuckDbFile("path/to/file.db");

// Access（32 位 / 64 位）
var helper = SqlHelperFactory.OpenAccessFile("path/to/file.mdb");
var helper = SqlHelperFactory.OpenAccessFile64x("path/to/file.accdb");

// 通用连接（按 SqlType）
var helper = SqlHelperFactory.OpenDatabase(connectionString, SqlType.SqlServer);
var helper = SqlHelperFactory.OpenDatabase(connectionString, "System.Data.SqlClient", SqlType.SqlServer);
```

#### 驱动选择说明

* `OpenDatabase(connectionString, providerName, type)` 的 `providerName` 用于精确定位驱动：优先加载与 `providerName` 匹配的 `DbProviderFactory`，匹配失败再按默认候选顺序回退。同一 `SqlType` 存在多个驱动时（如 SQLite 同时支持 System.Data.SQLite 与 Microsoft.Data.Sqlite），应传入对应的 `providerName`（如 `"Microsoft.Data.Sqlite"`），避免选错驱动。
* `OpenSqliteFile` 使用 System.Data.SQLite 驱动；`OpenMsSqliteFile` 使用 Microsoft.Data.Sqlite 驱动（支持密码，密码会转义后写入连接字符串）。
* `OpenMysql` 自动识别已加载的驱动（MySql.Data / MySqlConnector），据此选择连接串关键字（MySql.Data 用 `charset=utf8mb4;AllowUserVariables`，MySqlConnector 用 `CharSet=utf8mb4`），并自动附加合适的 `SslMode`/`AllowPublicKeyRetrieval` 选项。
* `OpenSqlServerFile` 默认 LocalDB 实例为 `(LocalDB)\MSSQLLocalDB`。

## CRUD 操作

### 插入

```csharp
// 插入单个实体，返回主键
var newId = helper.Insert(user);

// 批量插入（不返回主键）
helper.InsertList(new List<User> { user1, user2, user3 });
```

### 更新

```csharp
// 根据主键更新实体
user.Name = "李四";
var affected = helper.Update(user);

// 使用 SQL 更新
helper.Update<User>("Set [Name]=@0 WHERE [Id]=@1", "新名称", 1);

// 指定列更新
helper.Update(user, new[] { "Name" });

// 快照局部更新（只更新变更的列）
var snapshot = helper.StartSnapshot(user);
user.Name = "Bobby";
helper.Update(user, snapshot);

// 批量更新（按主键更新全部列）
helper.UpdateList(new List<User> { user1, user2 });

// 批量更新，仅更新快照中变更的列
var snapshots = users.Select(x => helper.StartSnapshot(x)).ToList();
helper.UpdateList(users, snapshots);

// 使用对象条件更新
helper.Update<User>(new { NickName = "新昵称" }, new { Id = 1 });
```

### 删除

```csharp
// 删除实体
helper.Delete(user);

// 根据主键删除
helper.DeleteById<User>(1);

// 根据条件删除
helper.Delete<User>("WHERE Age < @0", 18);

// 使用对象条件删除
helper.Delete<User>(new { Id = 1 });
```

### 保存

```csharp
helper.Save(user); // 根据主键判断插入或更新

// 批量保存：新对象（主键为默认值）插入，已存在对象更新
helper.SaveList(new List<User> { newUser, existingUser });
```

## SQL 查询

### 单个查询

```csharp
var user1 = helper.FirstOrDefault<User>("SELECT * FROM Users where [Id]=@0", 1);
var user2 = helper.FirstOrDefault<User>("Where [Id]=@0", 1);   // 简化 SQL（自动补 SELECT/FROM）

var dataset = helper.ExecuteDataSet("SELECT * FROM Users where [Id]=@0", 1);
var datatable = helper.ExecuteDataTable("SELECT * FROM Users where [Id]=@0", 1);

var userCount = helper.Count<User>("SELECT COUNT(*) FROM Users Where [UserType]=@0", 1);
var userCount2 = helper.ExecuteScalar<int>("SELECT COUNT(*) FROM Users Where [UserType]=@0", 1);
```

### 列表查询

```csharp
var users = helper.Select<User>("SELECT * FROM Users Where [UserType]=@0", 1);
var users2 = helper.Select<User>(20, "SELECT * FROM Users Where [UserType]=@0", 1);     // 取前 20 条
var users3 = helper.Select<User>(20, 0, "SELECT * FROM Users Where [UserType]=@0", 1);  // 取 20 条、跳过 0 条（limit, offset）
var usersPage = helper.Page<User>(1, 20, "SELECT * FROM Users Where [UserType]=@0", 1); // 分页（含总页数）
```

### 多结果集与一对多

```csharp
var (users, addresses) = helper.SelectMultiple<User, Address>(
    "select * from users;select * from addresses;");
var data = helper.SelectMultiple<User, Address, Tuple<List<User>, List<Address>>>(
    (u, a) => Tuple.Create(u, a), sql);  // 回调方式组合结果

var userInfos = helper.SelectOneToMany<UserInfo>(x => x.Addresses, manySql);
```

### 简化 SQL

```csharp
var users1 = helper.Select<User>("SELECT * FROM Users Where [UserType]=@0", 1);
var users2 = helper.Select<User>("FROM Users Where [UserType]=@0", 1);
var users3 = helper.Select<User>("Where [UserType]=@0", 1);

helper.Update<User>("UPDATE Users Set [Name]=@0 WHERE [Id]=@1", "Test", 1);
helper.Update<User>("Set [Name]=@0 WHERE [Id]=@1", "Test", 1);
```

## object 条件查询

以对象为条件，属性为默认值时忽略该条件：

```csharp
var user = helper.FirstOrDefault<User>(new { Id = 1 });
var users = helper.Select<User>(new { UserType = 1, State = true });
helper.Update<User>(new { NickName = "新昵称" }, new { Id = 1 });   // set 对象, 条件对象
helper.Delete<User>(new { Id = 1 });
var count = helper.Count<User>(new { UserType = 1 });
var exists = helper.Exists<User>(new { UserName = "Ted" });
```

同时提供按主键查询的重载（`FirstOrDefault<T>(int / long / uint / ulong)` 等）：

```csharp
var user = helper.FirstOrDefault<User>(1);
```

## 动态查询

### 链式查询（Where<T>()）

`Where<T>()` 返回 `IQueryProvider<T>`，可链式调用后再执行。

```csharp
public User FindUser(int userId, string userName, string nickName)
{
    var helper = SqlHelperFactory.OpenMysql("127.0.0.1", "web", "root", "123456");
    return helper.Where<User>()
        .IfTrueWhere(userId > 0, u => u.Id == userId)           // 条件成立才追加 Where
        .IfTrueWhere(userName != null, u => u.UserName == userName)
        .IfTrueWhere(nickName != null, u => u.NickName == nickName)
        .FirstOrDefault();
}
```

构建方法：

- `Where(expression)`、`WhereSql(sql, args)`、`OrderBy(column)`、`OrderByDescending(column)`、`ThenBy`、`ThenByDescending`、`Limit(rows)`、`Limit(skip, rows)`、`From(builder)`

执行方法：

- `ToList()`、`ToArray()`、`ToEnumerable()`、`First()`、`FirstOrDefault()`、`Single()`、`SingleOrDefault()`、`Count()`、`Any()`、`ToPage(page, pageSize)`、`ProjectTo<T2>(expression)`、`ToProjectedPage<T2>(expression, page, pageSize)`、`Distinct()`

动态条件扩展（IfTrue* 条件成立才生效）：

- `IfTrueWhere`、`IfTrueOrderBy`、`IfTrueOrderByDescending`、`IfTrueLimit`
- `IfTrueWhereIn`、`IfTrueWhereNotIn`、`IfTrueWhereLike`、`IfTrueWhereLikeStart`、`IfTrueWhereLikeEnd`、`IfTrueWhereExists`、`IfTrueWhereNotExists`

常用扩展：

- `WhereIn(column|field, values)`、`WhereNotIn(...)`、`WhereLike(column|field, pattern)`（%关键字%）、`WhereLikeStart`、`WhereLikeEnd`、`WhereExists(sql, args)`、`WhereNotExists(sql, args)`

```csharp
// 排序 + 分页
var users = helper.Where<User>()
    .Where(u => u.Age > 18)
    .OrderBy(u => u.Name)
    .Limit(10, 20)                       // 取 10 条，跳过 20 条
    .ToList();

var page = helper.Where<User>()
    .Where(u => u.Status == "Active")
    .ToPage(1, 20);

// LIKE 查询
var users = helper.Where<User>()
    .WhereLike(u => u.Name, "张")
    .ToList();

// IN 查询
var ids = new List<int> { 1, 2, 3, 4, 5 };
var users = helper.Where<User>()
    .WhereIn(u => u.Id, ids)
    .ToList();

// 投影
var results = helper.Where<User>()
    .Where(u => u.Status == "Active")
    .ProjectTo(u => new { u.Id, u.Name });
```

### 批量更新与删除

```csharp
helper.UpdateMany<User>()
    .Where(x => x.Age > 30)
    .ExcludeDefaults()                  // 跳过默认值字段
    .Execute(new User { Vip = true });

helper.DeleteMany<User>()
    .Where(x => x.Age < 18)
    .Execute();

// 批量更新集合（按主键更新全部列）
helper.UpdateList(users);

// 批量保存：新对象插入、已存在对象更新
helper.SaveList(new List<User> { newUser, existingUser });
```

## 异步 API

所有核心操作均提供 `_Async` 后缀的异步版本：`Execute_Async`、`ExecuteScalar_Async`、`ExecuteDataTable_Async`、`ExecuteDataSet_Async`、`Exists_Async`、`Count_Async`、`Select_Async`、`SelectPage_Async`、`Page_Async`、`SelectOneToMany_Async`、`SelectMultiple_Async`、`FirstOrDefault_Async`、`Insert_Async`、`InsertList_Async`、`Update_Async`（含快照/指定列/条件）、`UpdateList_Async`（含快照）、`Delete_Async`、`DeleteById_Async`、`Save_Async`、`SaveList_Async`、`UseTransaction_Async`。

对象条件版本使用同名 `object condition` 重载：`FirstOrDefault`、`Select`、`SelectPage`、`Page`、`Count`、`Exists`、`Update`、`Delete`（均含 `_Async` 版本）。

```csharp
var users = await helper.Select_Async<User>("Where [UserType]=@0", 1);
var user = await helper.FirstOrDefault_Async<User>(1);
var page = await helper.Page_Async<User>(1, 20, "Where Status=@0", "Active");
await helper.Insert_Async(user);
await helper.Update_Async(user, snapshot);   // 异步快照局部更新
```

## 事务管理

```csharp
using (var tran = helper.UseTransaction()) {
    helper.Insert(user1);
    helper.Insert(user2);
    tran.Complete(); // 提交事务；若不调用 Complete()，Dispose 时将回滚
}
```

## 表管理

通过 `helper._TableHelper`（SqlTableHelper）编程方式创建、删除、截断表：

```csharp
var table = helper._TableHelper;
table.TryCreateTable(typeof(User));    // 表不存在则创建
table.CreateTable(typeof(User));       // 创建表（可传 withIndex: true 同时创建索引）
table.CreateTableIndex(typeof(User));  // 创建索引
table.DropTable(typeof(User));         // 删除表
table.TruncateTable(typeof(User));     // 清空表

// 获取 SQL 脚本
var createSql = table.GetCreateTable(typeof(User));
var dropSql = table.GetDropTable(typeof(User));
var truncateSql = table.GetTruncateTable(typeof(User));
```

## 配置选项

通过 `helper._Config`（SqlConfig）设置配置，均为属性：

```csharp
// 命令超时（秒）
helper._Config.CommandTimeout = 30;

// 隔离级别
helper._Config.IsolationLevel = System.Data.IsolationLevel.ReadCommitted;

// 插入时为实体设置默认值
helper._Config.Insert_String_Default_NotNull = true;  // 字符串默认空字符串
helper._Config.Insert_DateTime_Default_Now = true;    // DateTime 默认当前时间
helper._Config.Insert_Guid_Default_New = true;        // Guid 默认新 GUID

// First 查询使用 LIMIT 1
helper._Config.Select_First_With_Limit_1 = true;
```

## SQL 执行监控

通过 `helper._Sql`（SqlRecord）获取最近一次执行信息：

```csharp
var sql = helper._Sql.LastSQL;          // 上次 SQL 语句
var args = helper._Sql.LastArgs;        // 上次 SQL 参数
var cmd = helper._Sql.LastCommand;      // 上次 SQL（带参数格式化）
var err = helper._Sql.LastErrorMessage; // 上次错误信息
```

## SQL 工具类（SqlUtil）

```csharp
var escaped = SqlUtil.ToEscapeParam("O'Brien");         // 转义参数
var escaped = SqlUtil.ToEscapeLikeParam("test%value");  // 转义 LIKE 参数

var where = SqlUtil.WhereLike("Name", "张");        // Name LIKE '张'
var where = SqlUtil.WhereLikeStart("Name", "张");   // Name LIKE '%张'
var where = SqlUtil.WhereLikeEnd("Name", "张");     // Name LIKE '张%'

var where = SqlUtil.WhereIn("Id", new List<int> { 1, 2, 3 });
var where = SqlUtil.WhereNotIn("Id", new List<int> { 1, 2, 3 });
```

## 动态表名

```csharp
// 获取动态表名用于列绑定
var u = helper.GetTableName<User>("u");
var sql = $"SELECT {u.Id}, {u.Name} FROM {u} WHERE {u.Status} = 'Active'";

// 使用类型
var table = helper.GetTableName(typeof(User), "t");
var sql = $"SELECT {table.Id} FROM {table}";
```

## 支持的数据库

SqlHelperFactory 提供以下连接方法：

| 数据库 | 工厂方法 | 提供程序 |
|--------|----------|----------|
| SQL Server | OpenSqlServer | System.Data.SqlClient |
| SQL Server | OpenSqlServerFile（LocalDB 文件库，默认实例 `(LocalDB)\MSSQLLocalDB`） | System.Data.SqlClient |
| MySQL | OpenMysql | MySql.Data 或 MySqlConnector（按已加载驱动自动识别） |
| SQLite | OpenSqliteFile | System.Data.SQLite |
| SQLite | OpenMsSqliteFile | Microsoft.Data.Sqlite |
| Oracle | OpenOracle | Oracle.ManagedDataAccess |
| DuckDB | OpenDuckDbFile（不支持密码，加密需用 ATTACH ... ENCRYPTION_KEY） | DuckDB.NET.Data.Full |
| MS Access | OpenAccessFile（32位）/ OpenAccessFile64x（64位） | System.Data.OleDb |

`SqlType` 枚举还包含 SqlServerCE、MsAccessDb 等类型（表操作暂不支持）。

## 最佳实践

### 1. 参数化查询

```csharp
// 推荐：使用参数
var users = helper.Select<User>("WHERE Name = @0 AND Age > @1", name, age);

// 不推荐：字符串拼接（SQL 注入风险）
var users = helper.Select<User>($"WHERE Name = '{name}'");
```

### 2. 批量操作

```csharp
// 推荐：使用 InsertList 批量插入
helper.InsertList(users);

// 批量更新与批量保存
helper.UpdateList(users);
helper.SaveList(new List<User> { newUser, existingUser });

// 不推荐：逐条插入
foreach (var user in users) helper.Insert(user);
```

### 3. 事务使用

```csharp
using (var tran = helper.UseTransaction()) {
    helper.Delete<Order>(new { UserId = userId });
    helper.Delete<User>(userId);
    tran.Complete();
}
```

### 4. 异步操作

```csharp
public async Task<List<User>> GetActiveUsersAsync()
{
    return await helper.Select_Async<User>("WHERE Status = @0", "Active");
}
```

### 5. 实体设计

```csharp
[Table("Users")]
[PrimaryKey("Id")]
public class User
{
    public int Id { get; set; }

    [Column("user_name")]
    public string UserName { get; set; }

    [Ignore]
    public string ComputedField { get; set; }

    [ResultColumn]
    public int OrderCount { get; set; }
}
```

## 常见模式

### 仓储模式

```csharp
public class UserRepository
{
    private readonly SqlHelper _db;

    public UserRepository(SqlHelper db) { _db = db; }

    public User GetById(int id) => _db.FirstOrDefault<User>(id);
    public List<User> GetActiveUsers() => _db.Select<User>(new { Status = "Active" });
    public void Save(User user) => _db.Save(user);
    public void Delete(int id) => _db.DeleteById<User>(id);
}
```

## 错误处理

```csharp
try
{
    var user = helper.FirstOrDefault<User>(id);
    if (user == null) throw new NotFoundException($"未找到用户 {id}");
    return user;
}
catch (SqlExecuteException ex)
{
    // 处理 SQL 执行错误
    throw new DataAccessException("数据库错误", ex);
}
```

## 许可证

本项目是开源的。有关许可证详情，请参阅项目仓库：https://github.com/ToolGood/ToolGood.ReadyGo
