---
name: "toolgood-readygo"
description: "基于PetaPoco的.NET轻量级ORM框架。在处理ToolGood.ReadyGo3数据库操作、CRUD、LINQ查询或多数据库支持时调用。"
---

# ToolGood.ReadyGo3 - 轻量级ORM框架

## 项目概述

ToolGood.ReadyGo3 是一个轻量级、高性能的 .NET ORM（对象关系映射）框架，由 PetaPoco 演进而来。它提供了简单而强大的数据库操作 API，支持多种数据库系统，包括 SQL Server、MySQL、SQLite、Oracle、PostgreSQL 等。

### 核心特性

- **轻量快速**：直接执行 SQL，开销最小
- **多数据库支持**：SQL Server、MySQL、SQLite、Oracle、PostgreSQL、MS Access、Firebird
- **LINQ 表达式支持**：使用 Lambda 表达式构建类型安全的查询
- **异步操作**：所有数据库操作都支持 async/await
- **动态 SQL 构建**：流式 API 构建复杂查询
- **事务支持**：内置事务管理
- **事件系统**：钩入数据库操作进行日志记录和审计
- **表管理**：编程方式创建、删除、截断表

## 核心架构

### 主要组件

```
ToolGood.ReadyGo3/
├── SqlHelper.cs              # 核心数据库操作类
├── SqlHelperFactory.cs       # 创建 SqlHelper 实例的工厂
├── SqlUtil.cs                # SQL 工具函数
├── Asyncs/                   # 异步操作实现
├── Attributes/               # 实体映射特性
├── LinQ/                     # LINQ 表达式支持
├── PetaPoco/                 # 核心 ORM 引擎
├── Gadget/                   # 工具和辅助类
└── Enums/                    # 枚举类型
```

### 核心类

#### SqlHelper
所有数据库操作的主入口点。提供以下方法：
- CRUD 操作（Insert、Update、Delete、Select）
- 查询执行（Execute、ExecuteScalar、ExecuteDataTable、ExecuteDataSet）
- 分页（Page、SelectPage）
- 动态查询（Where helper）
- 事务管理

#### SqlHelperFactory
创建 SqlHelper 实例的工厂类，支持多种数据库连接：
- `OpenDatabase()` - 通用数据库连接
- `OpenSqlServer()` - SQL Server 专用
- `OpenMysql()` - MySQL 专用
- `OpenSqliteFile()` - SQLite 文件数据库
- `OpenOracle()` - Oracle 专用

#### WhereHelper<T>
构建动态 SQL 查询的流式 API：
- 条件 WHERE 子句
- LIKE、IN、NOT IN 操作
- ORDER BY、GROUP BY、HAVING
- JOIN 支持
- 列选择/排除

## API 参考

### 实体特性

#### TableAttribute
将类映射到数据库表。

```csharp
[Table("Users", "dbo", "MyDatabase")]
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

#### PrimaryKeyAttribute
指定主键列。

```csharp
[PrimaryKey("Id", autoIncrement: true)]
public class User
{
    public int Id { get; set; }
}
```

#### ColumnAttribute
自定义列映射。

```csharp
[Column("user_name", "用户名字段")]
public string UserName { get; set; }
```

#### IgnoreAttribute
从映射中排除属性。

```csharp
[Ignore]
public string CalculatedField { get; set; }
```

#### ResultColumnAttribute
映射从查询结果填充但不用于 INSERT/UPDATE 的列。

```csharp
[ResultColumn]
public int OrderCount { get; set; }
```

### 数据库连接

#### 基本连接

```csharp
// SQL Server
var helper = SqlHelperFactory.OpenSqlServer("server", "database", "user", "password");

// MySQL
var helper = SqlHelperFactory.OpenMysql("server", "database", "user", "password");

// SQLite
var helper = SqlHelperFactory.OpenSqliteFile("path/to/database.db");

// 通用连接
var helper = SqlHelperFactory.OpenDatabase(connectionString, SqlType.SqlServer);
```

#### 使用配置对象连接

```csharp
// SQL Server 使用连接字符串构建器
var connStr = new SqlServerConnectionString();
connStr.Server = "localhost";
connStr.Database = "MyDB";
connStr.UserId = "sa";
connStr.Password = "password";
var helper = SqlHelperFactory.OpenSqlServer(connStr);

// MySQL 使用连接字符串构建器
var mysqlConnStr = new MysqlConnectionString();
mysqlConnStr.Server = "localhost";
mysqlConnStr.Database = "MyDB";
mysqlConnStr.UserId = "root";
mysqlConnStr.Password = "password";
var helper = SqlHelperFactory.OpenMysql(mysqlConnStr);
```

### CRUD 操作

#### 插入

```csharp
// 插入单个实体
var user = new User { Name = "张三", Email = "zhangsan@example.com" };
var id = helper.Insert(user);

// 插入并获取自增主键
var newId = (int)helper.Insert(user);

// 批量插入
var users = new List<User> { user1, user2, user3 };
helper.InsertList(users);

// 插入到指定表
helper.Table_Insert("Users", user);
```

#### 更新

```csharp
// 根据主键更新实体
user.Name = "李四";
var affected = helper.Update(user);

// 根据条件更新
helper.Update<User>("SET Name = @0 WHERE Id = @1", "新名称", 1);

// 使用对象条件更新
helper.Update<User>(
    new { Name = "已更新" },
    new { Id = 1 }
);

// 使用 WhereHelper 更新
helper.Where<User>()
    .Where(u => u.Id == 1)
    .Update(new { Name = "已更新" });
```

#### 删除

```csharp
// 删除实体
helper.Delete(user);

// 根据主键删除
helper.DeleteById<User>(1);

// 根据条件删除
helper.Delete<User>("WHERE Age < @0", 18);

// 使用对象条件删除
helper.Delete<User>(new { Status = "Inactive" });

// 使用 WhereHelper 删除
helper.Where<User>()
    .Where(u => u.Status == "Inactive")
    .Delete();
```

#### 查询

```csharp
// 查询所有
var users = helper.Select<User>();

// 根据条件查询
var users = helper.Select<User>("WHERE Age > @0", 18);

// 使用对象条件查询
var users = helper.Select<User>(new { Status = "Active" });

// 限制数量查询
var users = helper.Select<User>(10, "WHERE Status = @0", "Active");

// 分页查询（跳过和获取）
var users = helper.Select<User>(10, 20, "WHERE Status = @0", "Active");

// 查询第一条
var user = helper.FirstOrDefault<User>(1); // 根据主键
var user = helper.FirstOrDefault<User>("WHERE Email = @0", "test@example.com");
var user = helper.FirstOrDefault<User>(new { Email = "test@example.com" });
```

### 分页查询

```csharp
// 分页查询
var page = helper.Page<User>(1, 20, "WHERE Status = @0 ORDER BY Name", "Active");
// page.Items - 用户列表
// page.TotalItems - 总记录数
// page.TotalPages - 总页数
// page.CurrentPage - 当前页码
// page.PageSize - 每页记录数

// SelectPage（仅返回列表）
var users = helper.SelectPage<User>(1, 20, "WHERE Status = @0", "Active");
```

### 查询执行

```csharp
// 执行非查询语句
var affected = helper.Execute("UPDATE Users SET Status = @0", "Active");

// 执行标量查询
var count = helper.ExecuteScalar<int>("SELECT COUNT(*) FROM Users");

// 执行返回 DataTable
var dt = helper.ExecuteDataTable("SELECT * FROM Users WHERE Age > @0", 18);

// 执行返回 DataSet
var ds = helper.ExecuteDataSet("SELECT * FROM Users; SELECT * FROM Orders");

// 计数
var count = helper.Count<User>("WHERE Status = @0", "Active");

// 判断是否存在
var exists = helper.Exists<User>(1);
var exists = helper.Exists<User>("WHERE Email = @0", "test@example.com");
```

### 动态查询构建

```csharp
// 基本条件
var users = helper.Where<User>()
    .Where(u => u.Age > 18)
    .Where(u => u.Status == "Active")
    .Select();

// 带排序
var users = helper.Where<User>()
    .Where(u => u.Age > 18)
    .OrderBy(u => u.Name, OrderType.Asc)
    .Select();

// 带分页
var users = helper.Where<User>()
    .Where(u => u.Status == "Active")
    .SelectPage(1, 20);

// LIKE 查询
var users = helper.Where<User>()
    .WhereLike(u => u.Name, "张")
    .Select();

// IN 查询
var ids = new List<int> { 1, 2, 3, 4, 5 };
var users = helper.Where<User>()
    .WhereIn(u => u.Id, ids)
    .Select();

// 条件构建
var users = helper.Where<User>()
    .IfTrue(!string.IsNullOrEmpty(name))
        .WhereLike(u => u.Name, name)
    .IfTrue(age > 0)
        .Where(u => u.Age >= age)
    .Select();

// 使用 WhereHelper 更新
helper.Where<User>()
    .Where(u => u.Status == "Inactive")
    .Update(new { Status = "Archived" });

// 使用 WhereHelper 删除
helper.Where<User>()
    .Where(u => u.LastLogin < DateTime.Now.AddYears(-1))
    .Delete();

// 选择特定列
var results = helper.Where<User>()
    .Where(u => u.Status == "Active")
    .Select(u => new { u.Id, u.Name, u.Email });

// Group By 和 Having
var stats = helper.Where<User>()
    .GroupBy(u => u.Department)
    .Having("COUNT(*) > @0", 5)
    .Select(u => new { u.Department, Count = SqlFunction.Count });
```

### LINQ 表达式支持

```csharp
// Where 中使用 Lambda 表达式
var users = helper.Where<User>()
    .Where(u => u.Age > 18 && u.Status == "Active")
    .Select();

// 使用 Lambda 排序
var users = helper.Where<User>()
    .OrderBy(u => u.Name, OrderType.Asc)
    .OrderByDescending(u => u.CreatedDate)
    .Select();

// 使用 Lambda 分组
var groups = helper.Where<User>()
    .GroupBy(u => u.Department)
    .Select();
```

### 异步操作

```csharp
// 异步插入
var id = await helper.Insert_Async(user);

// 异步更新
var affected = await helper.Update_Async(user);

// 异步删除
var affected = await helper.Delete_Async(user);
var affected = await helper.DeleteById_Async<User>(1);

// 异步查询
var users = await helper.Select_Async<User>();
var users = await helper.Select_Async<User>("WHERE Status = @0", "Active");

// 异步获取第一条
var user = await helper.FirstOrDefault_Async<User>(1);

// 异步分页
var page = await helper.Page_Async<User>(1, 20, "WHERE Status = @0", "Active");

// 异步计数
var count = await helper.Count_Async<User>();

// 异步判断存在
var exists = await helper.Exists_Async<User>(1);

// 异步执行
var affected = await helper.Execute_Async("UPDATE Users SET Status = @0", "Active");
var count = await helper.ExecuteScalar_Async<int>("SELECT COUNT(*) FROM Users");
```

### 事务管理

```csharp
// 使用事务
using (var tran = helper.UseTransaction())
{
    helper.Insert(user1);
    helper.Insert(user2);
    helper.Update(user3);
    
    tran.Complete(); // 提交事务
    // 如果不调用 Complete()，事务将回滚
}

// 手动事务控制
helper._Config.SetIsolationLevel(System.Data.IsolationLevel.ReadCommitted);
using (var tran = helper.UseTransaction())
{
    try
    {
        helper.Insert(user);
        helper.Update(order);
        tran.Complete();
    }
    catch
    {
        // 事务将自动回滚
        throw;
    }
}
```

### 表管理

```csharp
// 获取表管理助手
var tableHelper = helper._Sql.GetTableHelper();

// 根据实体创建表
tableHelper.CreateTable(typeof(User));
tableHelper.CreateTable(typeof(User), withIndex: true);

// 尝试创建表（IF NOT EXISTS）
tableHelper.TryCreateTable(typeof(User));

// 创建索引
tableHelper.CreateTableIndex(typeof(User));

// 删除表
tableHelper.DropTable(typeof(User));
tableHelper.DropTable("Users");

// 截断表
tableHelper.TruncateTable(typeof(User));
tableHelper.TruncateTable("Users");

// 获取 SQL 脚本
var createSql = tableHelper.GetCreateTable(typeof(User));
var dropSql = tableHelper.GetDropTable(typeof(User));
var truncateSql = tableHelper.GetTruncateTable(typeof(User));
```

### 事件系统

```csharp
// 设置事件处理器
helper._Events.OnException = (ex, sql, args) => 
{
    Console.WriteLine($"SQL 错误: {ex.Message}");
    Console.WriteLine($"SQL: {sql}");
    return true; // 返回 true 以重新抛出异常
};

helper._Events.OnExecutingCommand = (sql, args) => 
{
    Console.WriteLine($"正在执行: {sql}");
};

helper._Events.OnExecutedCommand = (sql, args) => 
{
    Console.WriteLine($"已执行: {sql}");
};

helper._Events.OnBeforeInsert = (poco) => 
{
    // 返回 true 取消插入
    return false;
};

helper._Events.OnAfterInsert = (poco) => 
{
    // 日志或审计
};

helper._Events.OnBeforeUpdate = (poco) => false;
helper._Events.OnAfterUpdate = (poco) => { };
helper._Events.OnBeforeDelete = (poco) => false;
helper._Events.OnAfterDelete = (poco) => { };
```

### 配置选项

```csharp
// 设置命令超时
helper._Config.SetCommandTimeout(30); // 30 秒

// 设置隔离级别
helper._Config.SetIsolationLevel(System.Data.IsolationLevel.ReadCommitted);

// 为新实体设置默认值
helper._Config.SetStringDefaultNotNull(true); // 字符串属性默认为空字符串
helper._Config.SetDateTimeDefaultNow(true); // DateTime 属性默认为当前时间
helper._Config.SetGuidDefaultNew(true); // Guid 属性默认为新 GUID

// 启用 SQL 日志
helper._Sql.LastSQL; // 最后执行的 SQL
helper._Sql.LastArgs; // 最后的参数
helper._Sql.LastCommand; // 最后的命令（带参数）
helper._Sql.LastErrorMessage; // 最后的错误消息
```

### SQL 工具类

```csharp
// 转义参数
var escaped = SqlUtil.ToEscapeParam("O'Brien"); // O\'Brien

// 转义 LIKE 参数
var escaped = SqlUtil.ToEscapeLikeParam("test%value"); // test\%value

// 构建 LIKE 子句
var where = SqlUtil.WhereLike("Name", "张"); // Name LIKE '%张%'
var where = SqlUtil.WhereLikeStart("Name", "张"); // Name LIKE '%张'
var where = SqlUtil.WhereLikeEnd("Name", "张"); // Name LIKE '张%'

// 构建 IN 子句
var where = SqlUtil.WhereIn("Id", new List<int> { 1, 2, 3 }); // Id IN (1,2,3)
var where = SqlUtil.WhereIn("Name", new List<string> { "A", "B" }); // Name IN ('A','B')

// 构建 NOT IN 子句
var where = SqlUtil.WhereNotIn("Id", new List<int> { 1, 2, 3 });
```

### 动态表名

```csharp
// 获取动态表名用于列绑定
var u = helper.GetTableName<User>("u");
var sql = $"SELECT {u.Id}, {u.Name} FROM {u} WHERE {u.Status} = 'Active'";

// 使用类型
var table = helper.GetTableName(typeof(User), "t");
var sql = $"SELECT {table.Id} FROM {table}";
```

## 支持的数据库

| 数据库 | SqlType 枚举 | 提供程序 |
|--------|--------------|----------|
| SQL Server | SqlServer, SqlServer2012 | System.Data.SqlClient |
| MySQL | MySql, MariaDb | MySql.Data.MySqlClient |
| SQLite | SQLite | System.Data.SQLite |
| Oracle | Oracle | Oracle.ManagedDataAccess |
| PostgreSQL | PostgreSQL | Npgsql |
| MS Access | MsAccessDb | System.Data.OleDb |
| Firebird | FirebirdDb | FirebirdSql.Data.Firebird |
| SQL Server CE | SqlServerCE | System.Data.SqlServerCe |

## 最佳实践

### 1. 连接管理

```csharp
// 推荐：使用 using 语句
using (var helper = SqlHelperFactory.OpenSqlServer(connectionString))
{
    var users = helper.Select<User>();
}

// 推荐：复用连接执行多个操作
var helper = SqlHelperFactory.OpenSqlServer(connectionString);
try
{
    helper.Insert(user1);
    helper.Insert(user2);
}
finally
{
    helper.Dispose();
}
```

### 2. 参数化查询

```csharp
// 推荐：使用参数
var users = helper.Select<User>("WHERE Name = @0 AND Age > @1", name, age);

// 不推荐：字符串拼接（SQL 注入风险）
var users = helper.Select<User>($"WHERE Name = '{name}'");
```

### 3. 批量操作

```csharp
// 推荐：使用 InsertList 批量插入
var users = new List<User>();
for (int i = 0; i < 1000; i++)
{
    users.Add(new User { Name = $"用户{i}" });
}
helper.InsertList(users); // 高效批量插入

// 不推荐：逐条插入
foreach (var user in users)
{
    helper.Insert(user); // 大量数据时非常慢
}
```

### 4. 事务使用

```csharp
// 推荐：相关操作使用事务
using (var tran = helper.UseTransaction())
{
    helper.Delete<Order>(new { UserId = userId });
    helper.Delete<User>(userId);
    tran.Complete();
}
```

### 5. 异步操作

```csharp
// 推荐：I/O 密集型操作使用异步
public async Task<List<User>> GetActiveUsersAsync()
{
    return await helper.Select_Async<User>("WHERE Status = @0", "Active");
}

// 推荐：并行异步操作
var usersTask = helper.Select_Async<User>();
var ordersTask = helper.Select_Async<Order>();
await Task.WhenAll(usersTask, ordersTask);
```

### 6. 实体设计

```csharp
// 推荐：清晰的实体特性标注
[Table("Users")]
[PrimaryKey("Id", autoIncrement: true)]
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
    
    public UserRepository(SqlHelper db)
    {
        _db = db;
    }
    
    public User GetById(int id)
    {
        return _db.FirstOrDefault<User>(id);
    }
    
    public List<User> GetActiveUsers()
    {
        return _db.Select<User>(new { Status = "Active" });
    }
    
    public void Save(User user)
    {
        _db.Save(user);
    }
    
    public void Delete(int id)
    {
        _db.DeleteById<User>(id);
    }
}
```

### 查询构建器模式

```csharp
public class UserQuery
{
    private readonly WhereHelper<User> _query;
    
    public UserQuery(SqlHelper db)
    {
        _query = db.Where<User>();
    }
    
    public UserQuery WithName(string name)
    {
        if (!string.IsNullOrEmpty(name))
            _query.WhereLike(u => u.Name, name);
        return this;
    }
    
    public UserQuery WithAge(int? minAge, int? maxAge)
    {
        if (minAge.HasValue)
            _query.Where(u => u.Age >= minAge.Value);
        if (maxAge.HasValue)
            _query.Where(u => u.Age <= maxAge.Value);
        return this;
    }
    
    public UserQuery ActiveOnly()
    {
        _query.Where(u => u.Status == "Active");
        return this;
    }
    
    public List<User> Execute()
    {
        return _query.OrderBy(u => u.Name).Select();
    }
    
    public Page<User> ExecutePaged(int page, int pageSize)
    {
        return _query.OrderBy(u => u.Name).Page(page, pageSize);
    }
}

// 使用示例
var users = new UserQuery(db)
    .WithName("张")
    .WithAge(18, 65)
    .ActiveOnly()
    .Execute();
```

## 错误处理

```csharp
// 配置错误处理
helper._Events.OnException = (ex, sql, args) =>
{
    // 记录错误
    Logger.Error(ex, $"SQL 错误: {sql}");
    
    // 返回 true 重新抛出，false 抑制异常
    return true;
};

// Try-catch 模式
try
{
    var user = helper.FirstOrDefault<User>(id);
    if (user == null)
    {
        throw new NotFoundException($"未找到用户 {id}");
    }
    return user;
}
catch (SqlExecuteException ex)
{
    // 处理 SQL 执行错误
    throw new DataAccessException("数据库错误", ex);
}
```

## 性能优化建议

1. **批量插入使用 InsertList** - 批量插入性能更好
2. **复用 SqlHelper 实例** - 避免每次操作都创建新连接
3. **使用异步操作** - Web 应用更好的可扩展性
4. **只选择需要的列** - 使用 Select<T>(expression) 投影特定列
5. **使用分页** - 不要一次加载所有记录
6. **合理创建索引** - 在经常查询的列上创建索引
7. **使用连接池** - 配置连接字符串启用连接池

## 从 PetaPoco 迁移

ToolGood.ReadyGo3 基于 PetaPoco，并进行了多项增强：

1. **异步支持** - 所有操作都有异步版本
2. **流式 API** - WhereHelper 提供流式查询构建
3. **更多数据库提供程序** - 扩展的数据库支持
4. **更好的类型处理** - 改进的类型转换和映射
5. **事件系统** - 内置日志和审计钩子
6. **表管理** - 编程方式创建/删除/截断表

大多数 PetaPoco 代码只需少量修改即可使用。主要区别：
- 使用 `SqlHelper` 代替 `Database`
- 使用 `SqlHelperFactory` 创建连接
- 异步方法有 `_Async` 后缀
- 额外的 `Table_` 前缀方法用于动态表名

## 故障排除

### 常见问题

1. **连接无法打开**
   - 检查连接字符串格式
   - 验证数据库服务器是否运行
   - 检查防火墙设置

2. **映射错误**
   - 确保属性有公共的 getter/setter
   - 检查列名是否匹配（某些数据库区分大小写）
   - 使用 ColumnAttribute 指定不同的列名

3. **性能问题**
   - 使用 InsertList 代替逐条插入
   - 添加适当的索引
   - 对大结果集使用分页
   - 检查 N+1 查询问题

4. **事务问题**
   - 始终调用 Complete() 提交
   - 使用 using 语句确保清理
   - 检查隔离级别设置

## 许可证

本项目是开源的。有关许可证详情，请参阅项目仓库。

## 支持

如有问题、疑问或贡献，请访问项目仓库：
https://github.com/ToolGood/ToolGood.ReadyGo
