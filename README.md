ToolGood.ReadyGo
===


欢迎使用`ToolGood.ReadyGo`！`ToolGood.ReadyGo` 是一款轻量级的ORM，
它简单，因为汇聚作者多年经验，它快捷，基于PetaPoco核心，
它能增加代码的可读性。

### 快速上手

```` csharp
测试版本补充中
`````


### 功能简介：

* 表操作：
  * 支持表创建及删除
  * 支持索引创建。
  * 支持唯一索引创建。
  * 定制Attribute。
* 支持Object快速插入、修改、删除
  * 增删改操作事件。
* 支持原生SQL语言
  * 支持SQL简化。
  * 支持分页查询。
  * 查询后执行OnLoaded 方法。
* 支持动态SELECT
  * 支持返回dynamic。
  * 语法类似LINQ。
* 支持存储过程。
* 支持SQL执行监控。


#### 1、数据表生成与删除

##### 1.1、简单的数据表操作
目前支持【表操作】的数据库有Sql Server、MySql、SQLite。

```` csharp
using ToolGood.ReadyGo3.Attributes;

public class User
{
    public int Id {get;set;}
    public int UserType {get;set;}
    public string UserName {get;set;}
    public string Password {get;set;}
    public string NickName {get;set;}
}


using ToolGood.ReadyGo3;

var helper = SqlHelperFactory.OpenSqliteFile(dbFile);
var table = helper._TableHelper;
table.CreateTable(typeof(User));
table.DropTable(typeof(User))
````

##### 1.2、ToolGood.ReadyGo3.Attributes 介绍
在`ToolGood.ReadyGo3.Attributes`命名空间内提供以下几种Attribute
* ExplicitColumns 用于Class，所有列（Property）必须显式映射。
* Table   用于Class，定义表名、schema名、表名修饰TAG名。
* PrimaryKey 用于Class，定义主键名、自动增加、Sequence名。
* Column 用于Property，定义列。
* ResultColumn 用于Property，定义返回列。
* Ignore 用于Property，忽略该属性。
* Index 用于Class，定义索引。
* Unique 用于Class，定义唯一索引。
* Required 用于Property，定义非空列。
* FieldLength 用于Property，定义列长度。
* Text 用于Property，定义TEXT类型列。
* DefaultValue 用于Property，定义默认值。

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
helper._Events.BeforeInsert += Events_BeforeInsert;
helper._Events.AfterInsert += Events_AfterInsert;
helper._Events.BeforeUpdate += Events_BeforeUpdate;
helper._Events.AfterUpdate += Events_AfterUpdate;
helper._Events.BeforeDelete += Events_BeforeDelete;
helper._Events.AfterDelete += Events_AfterDelete;
helper._Events.BeforeExecuteCommand += Events_BeforeExecuteCommand;
helper._Events.AfterExecuteCommand += Events_AfterExecuteCommand;
helper._Events.ExecuteException += Events_ExecuteException;
````

##### 2.3、使用事务
```` csharp
    using (var tran=helper.UseTransaction()) {
        ...

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
var users2 = helper.Select<User>(20,"SELECT * FROM Users Where [UserType]=@0", 1);
var users2 = helper.Select<User>(0,20,"SELECT * FROM Users Where [UserType]=@0", 1);
var usersPage = helper.Page<User>(1,20,"SELECT * FROM Users Where [UserType]=@0", 1);
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




#### 4、动态查询
##### 4.1、查询

```` csharp
public User FindUser(int userId,string userName,string nickName)
{
    var helper = SqlHelperFactory.OpenMysql("127.0.0.1", "web", "root", "123456");
    return helper.Where<User>()
        .IfTrue(userId > 0).Where((u) => u.Id == userId)
        .IfSet(userName).Where((u) => u.UserName == userName)
        .IfSet(nickName).Where((u) => u.NickName == nickName)
        .SingleOrDefault();
}
````
方法有：
* `IfTrue`、`IfFalse`、`IfSet`、`IfNotSet`、`IfNull`、`IfNotNull`
* `WhereNotIn`、`WhereIn`、`Where`、`OrderBy`、`GroupBy`、`Having`、`SelectColumn`
* `Select`、`Page`、`Single`、`SingleOrDefault`、`First`、`FirstOrDefault`、`Count`、`ExecuteDataTable`、`ExecuteDataSet`
* `Select<T>`、`Page<T>`、`SkipTake<T>`、`Single<T>`、`SingleOrDefault<T>`、`First<T>`、`FirstOrDefault<T>`
 

#### 5、存储过程
##### 5.1、定义类
```` csharp
    public class Chart_GetDeviceCount : SqlProcess
    {
        public Chart_GetDeviceCount(ToolGood.ReadyGo.SqlHelper helper) : base(helper)
        {        }
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

##### 5.2、存储执行
```` csharp
    var helper = SqlHelperFactory.OpenMysql("127.0.0.1", "wifi", "root", "123456");
    Chart_GetDeviceCount c = new Chart_GetDeviceCount(helper);
    c.AgentId = 0;
    c.IsAll = 0;
    c.Type = 0;
    c.StartDate = DateTime.Parse("2016-06-01");
    c.EndDate = DateTime.Parse("2016-07-01");
            
    var dt= c.ExecuteScalar<int> ();
````

 
#### 6、SQL执行监控

#### 6.1、上一次SQL执行语句

```` csharp
    var sql = helper._Sql.LastSQL;
    var args = helper._SqlSql.LastArgs;
    var cmd = helper._Sql.LastCommand;
    var err = helper._Sql.LastErrorMessage;

````


 
 
