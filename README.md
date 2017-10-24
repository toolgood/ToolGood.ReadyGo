ToolGood.ReadyGo
===


欢迎使用`ToolGood.ReadyGo`！`ToolGood.ReadyGo` 是一款轻量级的ORM，
它简单，因为汇聚作者多年经验，它快捷，基于PetaPoco核心，
它能增加代码的可读性。
它提供了VS插件，使Coding更方便。

### 快速上手

```` csharp
public class User
{
	public int Id {get;set;}
	public string UserName {get;set;}
	public string Password {get;set;}
	public string NickName {get;set;}
}

using ToolGood.ReadyGo;

public User FindUser(int userId,string userName,string nickName)
{
    var helper = SqlHelperFactory.OpenMysql("127.0.0.1", "web", "root", "123456");
    return helper.CreateWhere<User>()
        .IfTrue(userId > 0).Where((u) => u.Id == userId)
        .IfSet(userName).Where((u) => u.UserName == userName)
        .IfSet(nickName).Where((u) => u.NickName == nickName)
        .SingleOrDefault();
}
`````


### 功能简介：

* 表操作：
  * 支持表创建及删除
  * 支持索引创建
  * 支持唯一索引创建。
  * 定制Attribute
* 支持Object快速插入、修改、删除
  * 支持修改前事件。
* 支持原生SQL语言
  * 支持SQL简化
  * 支持分页查询。
  * 查询后执行OnLoaded 方法
* 支持动态SELECT
  * 支持返回dynamic。
  * 语法类似LINQ
  * 支持动态多表SELECT。
  * 支持表名修饰。
* 支持动态UPDATE
  * 语法类似LINQ。
  * 采用AOP思路，不破坏原有类。
* 支持存储过程。
* 支持缓存。
* 支持SQL执行监控。
* 带Sql Parser。
* 带VS Coding插件。


#### 1、数据表生成与删除

##### 1.1、简单的数据表操作
目前支持【表操作】的数据库有Sql Server、MySql、SQLite。

```` csharp
using ToolGood.ReadyGo.Attributes;

public class User
{
    public int Id {get;set;}
    public int UserType {get;set;}
    public string UserName {get;set;}
    public string Password {get;set;}
    public string NickName {get;set;}
}


using ToolGood.ReadyGo;

var helper = SqlHelperFactory.OpenSqliteFile(dbFile);
var table = helper.TableHelper;
table.CreateTable<User>();
table.CreateIndex<User>();
table.CreateUnique<User>();
table.DeleteTable<User>();
````

##### 1.2、ToolGood.ReadyGo.Attributes 介绍
在`ToolGood.ReadyGo.Attributes`命名空间内提供以下几种Attribute
* TableAttribute   用于Class，定义表名、schema名、表名修饰TAG名。
* PrimaryKeyAttribute 用于Class，定义主键名、自动增加、Sequence名。
* IndexAttribute 用于Class，定义索引。
* UniqueAttribute 用于Class，定义唯一索引。
* ColumnAttribute 用于Property，定义列。
* TextAttribute 用于Property，定义TEXT类型列。
* ResultColumnAttribute 用于Property，定义返回列。
* IgnoreAttribute 用于Property，忽略该属性。
* FieldLengthAttribute 用于Property，定义列长度。
* DefaultValueAttribute 用于Property，定义默认值。

#### 2、数据表操作
##### 2.1、增删改操作


```` csharp
var helper = SqlHelperFactory.OpenSqliteFile(dbFile);
User u = new User() {
    ....
};
helper.Insert(u);
helper.Update(u);
helper.Save(u);
helper.Delete(u);
helper.Update<User>("Set [Name]=@0 WHERE [Id]=@1", "Test", 1);
helper.Delete<User>("WHERE [Id]=@0", 1);
helper.DeleteById<User>(1);
````

##### 2.2、增删改操作事件

```` csharp
helper.Events.BeforeInsert += Events_BeforeInsert;
helper.Events.AfterInsert += Events_AfterInsert;
helper.Events.BeforeUpdate += Events_BeforeUpdate;
helper.Events.AfterUpdate += Events_AfterUpdate;
helper.Events.BeforeDelete += Events_BeforeDelete;
helper.Events.AfterDelete += Events_AfterDelete;
helper.Events.BeforeExecuteCommand += Events_BeforeExecuteCommand;
helper.Events.AfterExecuteCommand += Events_AfterExecuteCommand;
helper.Events.ExecuteException += Events_ExecuteException;
````

##### 2.3、使用事务
```` csharp
    using (var tran=helper.UseTransaction()) {
        ...
        tran.Complete();//提交 默认
        tran.Abort();//取消
    }
````

#### 3、SQL查询

##### 3.1、单个查询
```` csharp
var helper = SqlHelperFactory.OpenSqliteFile(dbFile);
var user1 = helper.Single<User>("SELECT * FROM Users where [Id]=@0", 1);
var user2 = helper.SingleById<User>(1);
var user3 = helper.SingleOrDefault<User>("SELECT * FROM Users where [Id]=@0", 1);
var user4 = helper.SingleOrDefaultById<User>(1);

var user5 = helper.First<User>("SELECT * FROM Users where [Id]=@0", 1);
var user6 = helper.FirstOrDefault<User>("SELECT * FROM Users where [Id]=@0", 1);

var dataset = helper.ExecuteDataSet("SELECT * FROM Users where [Id]=@0", 1);
var datatable = helper.ExecuteDataTable("SELECT * FROM Users where [Id]=@0", 1);

var userCount = helper.Count<User>("SELECT COUNT(*) FROM Users Where [UserType]=@0", 1);
var userCount2 = helper.ExecuteScalar<int>("SELECT COUNT(*) FROM Users Where [UserType]=@0", 1);
````


##### 3.2、列表查询
```` csharp
var helper = SqlHelperFactory.OpenSqliteFile(dbFile);
var users = helper.Select<User>("SELECT * FROM Users Where [UserType]=@0", 1);
var usersPage = helper.Page <User>(1,20,"SELECT * FROM Users Where [UserType]=@0", 1);
var users2=helper.SkipTake<User>(0,20,"SELECT * FROM Users Where [UserType]=@0", 1);
````

##### 3.3、简化SQL

```` csharp
var helper = SqlHelperFactory.OpenSqliteFile(dbFile);
var users1 = helper.Select<User>("SELECT * FROM Users Where [UserType]=@0", 1);
var users2 = helper.Select<User>("FROM Users Where [UserType]=@0", 1);
var users3 = helper.Select<User>("Where [UserType]=@0", 1);

helper.Update<User>("UPDATE Users Set [Name]=@0 WHERE [Id]=@1", "Test", 1);
helper.Update<User>("Set [Name]=@0 WHERE [Id]=@1", "Test", 1);

````



##### 3.4、类中的OnLoaded方法

```` csharp
public class User
{
    public int Id {get;set;}
    public int UserType {get;set;}
    public string UserName {get;set;}
    public string Password {get;set;}
    public string NickName {get;set;}
    public bool IsLoad {get;set;}

    public void OnLoaded()
    {
        IsLoad=true;
    }
}
var user1 = helper.Single<User>("where [Id]=@0", 1);
Console.WriteLine(user1.IsLoad.ToString());
````


#### 4、动态查询
##### 4.1、单表查询

```` csharp
public User FindUser(int userId,string userName,string nickName)
{
    var helper = SqlHelperFactory.OpenMysql("127.0.0.1", "web", "root", "123456");
    return helper.CreateWhere<User>()
        .IfTrue(userId > 0).Where((u) => u.Id == userId)
        .IfSet(userName).Where((u) => u.UserName == userName)
        .IfSet(nickName).Where((u) => u.NickName == nickName)
        .SingleOrDefault();
}
````
方法有：
* `IfTrue`、`IfFalse`、`IfSet`、`IfNotSet`、`IfNull`、`IfNotNull`
* `WhereNotIn`、`WhereIn`、`Where`、`OrderBy`、`GroupBy`、`Having`、`SelectColumn`
* `AddNotExistsSql`、`AddExistsSql`、`AddWhereSql`、`AddOrderBySql`、`AddGroupBySql`、`AddHavingSql`、`AddJoinSql`
* `Select`、`Page`、`SkipTake`、`Single`、`SingleOrDefault`、`First`、`FirstOrDefault`、`Count`、`ExecuteDataTable`、`ExecuteDataSet`
* `Select<T>`、`Page<T>`、`SkipTake<T>`、`Single<T>`、`SingleOrDefault<T>`、`First<T>`、`FirstOrDefault<T>`

##### 4.2、多表查询

```` csharp
    var helper = SqlHelperFactory.OpenMysql("127.0.0.1", "web", "root", "123456");
    var list = helper.CreateWhere<User1, UserPay>()
            .On2((u, up) => u.AutoID == up.UserID)
            .Where((u, up) => u.AutoID == 8)
            .Where((u, up) => up.State == 1)
            .Select();
````
最多支持5个数据表，常用方法比单表查询多一点
* `On2`、`On3`、`On4`、`On5`


##### 4.4、表名修饰
```` csharp
[Table("Users",fixTag:"Admin")]
public class User
{
    public int Id {get;set;}
    public int UserType {get;set;}
    public string UserName {get;set;}
    public string Password {get;set;}
    public string NickName {get;set;}
    public bool IsLoad {get;set;}

    public void OnLoaded()
    {
        IsLoad=true;
    }
}
helper.TableNameManger.Set("Admin", "Db", "");
var user1 = helper.Single<User>("where [Id]=@0", 1);
// SELECT * FROM DbUsers where [Id]=@0
````

##### 4.4、辅助类SQL
```` csharp
    var helper = SqlHelperFactory.OpenMysql("127.0.0.1", "web", "root", "123456");
    var list1 = helper.CreateWhere<User1, UserPay>()
            .On2((u, up) => u.AutoID == up.UserID)
            .Where((u, up) => u.AutoID == 8)
            .SelectColumn((u, up) => SQL.Sum(up.Money), "TotlePay")
            .SelectColumn((u, up) =>up.UserID)
            .Select<dynamic>();
````

#### 5、动态UPDATE
##### 5.1、一个简单的动态UPDATE
```` csharp
helper.CreateWhere<User>()
    .Where((u) => u.Id == userId)
    .SetValue((u) => u.NickName = "Test")
    .Update();

var users = helper.CreateWhere<User>()
                .Where((u) => u.Id == userId)
                .SetValue((u) => u.NickName = "Test")
                .UpdateAndSelect();
````

##### 5.2、Update与UpdateAndSelect区别
    `Update`与`UpdateAndSelect`本质的区别是：
    1. `Update`使用SQL语言更新，只支持直接赋值。
    2. `UpdateAndSelect`先SELECT，然后在代码端执行Action，再UPDATE。

```` csharp
helper.CreateWhere<User>()
    .Where((u) => u.Id == userId)
    .SetValue((u) => u.UserName = "Test")
    .SetValue((u)=>u.NickName=u.NickName+"u")
    .Update();
````
运行上面的代码，你会发现`UserName`修改了，而`NickName`未修改。

```` csharp
helper.CreateWhere<User>()
    .Where((u) => u.Id == userId)
    .SetValue((u) => u.UserName = "Test")
    .SetValue((u)=>u.NickName=u.NickName+"u")
    .UpdateAndSelect();
````
运行上面的代码，你会发现`UserName`和`NickName`都修改了。


#### 6、存储过程
##### 6.1、定义类
```` csharp
    public class Chart_GetDeviceCount : SqlProcess
    {
        public Chart_GetDeviceCount(ToolGood.ReadyGo.SqlHelper helper) : base(helper)
        {
        }
        protected override void OnInit()
        {
            _ProcessName = "Chart_GetDeviceCount";

            Add<int>("_AgentId", false);
            Add<int>("_IsAll", false);
            Add<int>("_Type", false);
            Add<DateTime>("_StartDate", false);
            Add<DateTime>("_EndDate", false);

        }
        public int AgentId { get { return _G<int>("_AgentId"); } set { _S("_AgentId", value); } }
        public int IsAll { get { return _G<int>("_IsAll"); } set { _S("_IsAll", value); } }
        public int Type { get { return _G<int>("_Type"); } set { _S("_Type", value); } }
        public DateTime StartDate { get { return _G<DateTime>("_StartDate"); } set { _S("_StartDate", value); } }
        public DateTime EndDate { get { return _G<DateTime>("_EndDate"); } set { _S("_EndDate", value); } }
    }
````

##### 6.2、存储执行
```` csharp
    var helper = SqlHelperFactory.OpenMysql("127.0.0.1", "wifi86", "root", "123456");
    Chart_GetDeviceCount c = new Chart_GetDeviceCount(helper);
    c.AgentId = 0;
    c.IsAll = 0;
    c.Type = 0;
    c.StartDate = DateTime.Parse("2016-06-01");
    c.EndDate = DateTime.Parse("2016-07-01");
            
    var dt= c.ExecuteScalar<int> ();
````

#### 7、缓存
##### 7.1、使用缓存
```` csharp
    var helper = SqlHelperFactory.OpenMysql("127.0.0.1", "wifi86", "root", "123456");
    var user = helper.UseCache(20,"tag").SingleById<User>(1);
````

##### 7.2、替换缓存类
```` csharp
using ToolGood.ReadyGo.Caches

    public class NullCacheService : ICacheService
    {
        ...
    }
    var helper = SqlHelperFactory.OpenMysql("127.0.0.1", "wifi86", "root", "123456");
    helper.Config.CacheService = new NullCacheService();
    helper.Config.CacheTime = 20;
````

#### 8、SQL执行监控

#### 8.1、上一次SQL执行语句

```` csharp
    var sql = helper.Sql.LastSQL;
    var args = helper.Sql.LastArgs;
    var cmd = helper.Sql.LastCommand;
    var err = helper.Sql.LastErrorMessage;
    var commandTimeout = helper.Sql.LastCommandTimeout;

    var isUsed = helper.Sql.LastUsedCacheService;
    var cacheService = helper.Sql.LastCacheService;
    var cacheTime = helper.Sql.LastCacheTime;
    var cacheTag = helper.Sql.LastCacheTag;
````



#### 8.2、查看监控
```` csharp
using ToolGood.ReadyGo.Monitor

var sqlMonitor = helper.Sql.SqlMonitor;
var html = sqlMonitor.ToHtml();
var text = sqlMonitor.ToText();
````



#### 8.3、替换监控类

```` csharp
using ToolGood.ReadyGo.Monitor

public class NullSqlMonitor : ISqlMonitor
{
    ...
}
helper.Config.SqlMonitor = new NullSqlMonitor();
````




#### 9、速度对比

<table>
	<tr>
		<th>类型</th>
		<th>持续时间</th>		
		<th>备注</th>
	</tr>
	<tr>
		<td>Linq 2 SQL </td>
		<td>81ms</td>
		<td>Not super typical involves complex code</td>
	</tr>
	
</table>

手写

PetaPoco

ToolGood.ReadyGo

ToolGood.ReadyGo 动态语言

###下个版本优化内容


优化SQL语句，Single，First之类的，如page方法


----------------------------
