using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3;
using ToolGood.ReadyGo3.Attributes;
using ToolGood.ReadyGo3.DataCentxt;
using ToolGood.ReadyGo3.Test;

namespace ToolGood.ReadyGo3.CoreTest.Datas
{
    public class TbAdmin : QTable<ToolGood.ReadyGo3.CoreTest.Datas.DbAdmin>
    {
        private QTableColumn<int> _ID;
        private QTableColumn<string> _Name;
        private QTableColumn<string> _Password;
        private QTableColumn<string> _TrueName;
        private QTableColumn<string> _Phone;
        private QTableColumn<string> _Email;
        private QTableColumn<int> _AdminGroupID;
        private QTableColumn<bool> _IsDelete;
        private QTableColumn<System.DateTime> _AddingTime;
        private QTableColumn<string> _AdminGroupName;

        public TbAdmin() : base(Config.DbHelper) { }
        public TbAdmin(SqlHelper sqlHelper) : base(sqlHelper) { }

        protected override void Init()
        {
            __SchemaName__ = "";
            __TableName__ = "Admin";

            _ID = AddColumn<int>("ID", "ID", true);
            _Name = AddColumn<string>("Name", "Name", false);
            _Password = AddColumn<string>("Password", "Password", false);
            _TrueName = AddColumn<string>("TrueName", "TrueName", false);
            _Phone = AddColumn<string>("Phone", "Phone", false);
            _Email = AddColumn<string>("Email", "Email", false);
            _AdminGroupID = AddColumn<int>("AdminGroupID", "AdminGroupID", false);
            _IsDelete = AddColumn<bool>("IsDelete", "IsDelete", false);
            _AddingTime = AddColumn<System.DateTime>("AddingTime", "AddingTime", false);
            _AdminGroupName = AddColumn<string>("AdminGroupName", "AdminGroupName", "select Name from AdminGroup where Id={0}.AdminGroupID");
        }

        ///<summary>
        ///ID
        ///</summary>
        public QTableColumn<int> ID { get { return _ID; } set { _ID.NewValue = value; } }

        ///<summary>
        ///用户名
        ///</summary>
        public QTableColumn<string> Name { get { return _Name; } set { _Name.NewValue = value; } }

        ///<summary>
        ///密码
        ///</summary>
        public QTableColumn<string> Password { get { return _Password; } set { _Password.NewValue = value; } }

        ///<summary>
        ///真实姓名
        ///</summary>
        public QTableColumn<string> TrueName { get { return _TrueName; } set { _TrueName.NewValue = value; } }

        ///<summary>
        ///手机
        ///</summary>
        public QTableColumn<string> Phone { get { return _Phone; } set { _Phone.NewValue = value; } }

        ///<summary>
        ///邮箱 
        ///</summary>
        public QTableColumn<string> Email { get { return _Email; } set { _Email.NewValue = value; } }

        ///<summary>
        ///管理员ID
        ///</summary>
        public QTableColumn<int> AdminGroupID { get { return _AdminGroupID; } set { _AdminGroupID.NewValue = value; } }

        ///<summary>
        ///是否删除
        ///</summary>
        public QTableColumn<bool> IsDelete { get { return _IsDelete; } set { _IsDelete.NewValue = value; } }

        ///<summary>
        ///添加日期
        ///</summary>
        public QTableColumn<System.DateTime> AddingTime { get { return _AddingTime; } set { _AddingTime.NewValue = value; } }

        ///<summary>
        ///管理员名
        ///</summary>
        public QTableColumn<string> AdminGroupName { get { return _AdminGroupName; } }

    }

    public class TbAdminGroup : QTable<ToolGood.ReadyGo3.CoreTest.Datas.DbAdminGroup>
    {
        private QTableColumn<int> _ID;
        private QTableColumn<string> _Name;
        private QTableColumn<string> _Describe;
        private QTableColumn<int> _Sort;
        private QTableColumn<bool> _IsDelete;
        private QTableColumn<System.DateTime> _AddingTime;

        public TbAdminGroup() : base(Config.DbHelper) { }
        public TbAdminGroup(SqlHelper sqlHelper) : base(sqlHelper) { }

        protected override void Init()
        {
            __SchemaName__ = "";
            __TableName__ = "AdminGroup";

            _ID = AddColumn<int>("ID", "ID", true);
            _Name = AddColumn<string>("Name", "Name", false);
            _Describe = AddColumn<string>("Describe", "Describe", false);
            _Sort = AddColumn<int>("Sort", "Sort", false);
            _IsDelete = AddColumn<bool>("IsDelete", "IsDelete", false);
            _AddingTime = AddColumn<System.DateTime>("AddingTime", "AddingTime", false);
        }

        ///<summary>
        ///ID
        ///</summary>
        public QTableColumn<int> ID { get { return _ID; } set { _ID.NewValue = value; } }

        ///<summary>
        ///名称
        ///</summary>
        public QTableColumn<string> Name { get { return _Name; } set { _Name.NewValue = value; } }

        ///<summary>
        ///描述
        ///</summary>
        public QTableColumn<string> Describe { get { return _Describe; } set { _Describe.NewValue = value; } }

        ///<summary>
        ///排序 
        ///</summary>
        public QTableColumn<int> Sort { get { return _Sort; } set { _Sort.NewValue = value; } }

        ///<summary>
        ///是否删除
        ///</summary>
        public QTableColumn<bool> IsDelete { get { return _IsDelete; } set { _IsDelete.NewValue = value; } }

        ///<summary>
        ///添加时间
        ///</summary>
        public QTableColumn<System.DateTime> AddingTime { get { return _AddingTime; } set { _AddingTime.NewValue = value; } }

    }

    public class TbAdminLoginLog : QTable<ToolGood.ReadyGo3.CoreTest.Datas.DbAdminLoginLog>
    {
        private QTableColumn<int> _ID;
        private QTableColumn<string> _Name;
        private QTableColumn<string> _Message;
        private QTableColumn<string> _Ip;
        private QTableColumn<bool> _State;
        private QTableColumn<System.DateTime> _AddingTime;

        public TbAdminLoginLog() : base(Config.DbHelper) { }
        public TbAdminLoginLog(SqlHelper sqlHelper) : base(sqlHelper) { }

        protected override void Init()
        {
            __SchemaName__ = "";
            __TableName__ = "AdminLoginLog";

            _ID = AddColumn<int>("ID", "ID", true);
            _Name = AddColumn<string>("Name", "Name", false);
            _Message = AddColumn<string>("Message", "Message", false);
            _Ip = AddColumn<string>("Ip", "Ip", false);
            _State = AddColumn<bool>("State", "State", false);
            _AddingTime = AddColumn<System.DateTime>("AddingTime", "AddingTime", false);
        }

        ///<summary>
        ///ID
        ///</summary>
        public QTableColumn<int> ID { get { return _ID; } set { _ID.NewValue = value; } }

        ///<summary>
        ///用户名
        ///</summary>
        public QTableColumn<string> Name { get { return _Name; } set { _Name.NewValue = value; } }

        ///<summary>
        ///消息
        ///</summary>
        public QTableColumn<string> Message { get { return _Message; } set { _Message.NewValue = value; } }

        ///<summary>
        ///IP地址
        ///</summary>
        public QTableColumn<string> Ip { get { return _Ip; } set { _Ip.NewValue = value; } }

        ///<summary>
        ///状态，是否成功登录
        ///</summary>
        public QTableColumn<bool> State { get { return _State; } set { _State.NewValue = value; } }

        ///<summary>
        ///添加时间
        ///</summary>
        public QTableColumn<System.DateTime> AddingTime { get { return _AddingTime; } set { _AddingTime.NewValue = value; } }

    }

    public class TbAdminMenu : QTable<ToolGood.ReadyGo3.CoreTest.Datas.DbAdminMenu>
    {
        private QTableColumn<int> _ID;
        private QTableColumn<int> _ParentID;
        private QTableColumn<string> _Code;
        private QTableColumn<string> _Name;
        private QTableColumn<string> _Icon;
        private QTableColumn<string> _Url;
        private QTableColumn<string> _Actions;
        private QTableColumn<int> _Sort;
        private QTableColumn<bool> _IsDelete;
        private QTableColumn<System.DateTime> _AddingTime;

        public TbAdminMenu() : base(Config.DbHelper) { }
        public TbAdminMenu(SqlHelper sqlHelper) : base(sqlHelper) { }

        protected override void Init()
        {
            __SchemaName__ = "";
            __TableName__ = "AdminMenu";

            _ID = AddColumn<int>("ID", "ID", true);
            _ParentID = AddColumn<int>("ParentID", "ParentID", false);
            _Code = AddColumn<string>("Code", "Code", false);
            _Name = AddColumn<string>("Name", "Name", false);
            _Icon = AddColumn<string>("Icon", "Icon", false);
            _Url = AddColumn<string>("Url", "Url", false);
            _Actions = AddColumn<string>("Actions", "Actions", false);
            _Sort = AddColumn<int>("Sort", "Sort", false);
            _IsDelete = AddColumn<bool>("IsDelete", "IsDelete", false);
            _AddingTime = AddColumn<System.DateTime>("AddingTime", "AddingTime", false);
        }

        ///<summary>
        ///ID
        ///</summary>
        public QTableColumn<int> ID { get { return _ID; } set { _ID.NewValue = value; } }

        ///<summary>
        ///父ID
        ///</summary>
        public QTableColumn<int> ParentID { get { return _ParentID; } set { _ParentID.NewValue = value; } }

        ///<summary>
        ///CODE
        ///</summary>
        public QTableColumn<string> Code { get { return _Code; } set { _Code.NewValue = value; } }

        ///<summary>
        ///名称
        ///</summary>
        public QTableColumn<string> Name { get { return _Name; } set { _Name.NewValue = value; } }

        ///<summary>
        ///图标
        ///</summary>
        public QTableColumn<string> Icon { get { return _Icon; } set { _Icon.NewValue = value; } }

        ///<summary>
        ///链接地址
        ///</summary>
        public QTableColumn<string> Url { get { return _Url; } set { _Url.NewValue = value; } }

        ///<summary>
        ///操作
        ///</summary>
        public QTableColumn<string> Actions { get { return _Actions; } set { _Actions.NewValue = value; } }

        ///<summary>
        ///排序
        ///</summary>
        public QTableColumn<int> Sort { get { return _Sort; } set { _Sort.NewValue = value; } }

        ///<summary>
        ///是否删除
        ///</summary>
        public QTableColumn<bool> IsDelete { get { return _IsDelete; } set { _IsDelete.NewValue = value; } }

        ///<summary>
        ///添加时间
        ///</summary>
        public QTableColumn<System.DateTime> AddingTime { get { return _AddingTime; } set { _AddingTime.NewValue = value; } }

    }

    public class TbAdminMenuPass : QTable<ToolGood.ReadyGo3.CoreTest.Datas.DbAdminMenuPass>
    {
        private QTableColumn<int> _AdminGroupID;
        private QTableColumn<int> _MenuID;
        private QTableColumn<string> _Code;
        private QTableColumn<string> _ActionName;

        public TbAdminMenuPass() : base(Config.DbHelper) { }
        public TbAdminMenuPass(SqlHelper sqlHelper) : base(sqlHelper) { }

        protected override void Init()
        {
            __SchemaName__ = "";
            __TableName__ = "AdminMenuPass";

            _AdminGroupID = AddColumn<int>("AdminGroupID", "AdminGroupID", false);
            _MenuID = AddColumn<int>("MenuID", "MenuID", false);
            _Code = AddColumn<string>("Code", "Code", false);
            _ActionName = AddColumn<string>("ActionName", "ActionName", false);
        }

        ///<summary>
        ///管理组ID
        ///</summary>
        public QTableColumn<int> AdminGroupID { get { return _AdminGroupID; } set { _AdminGroupID.NewValue = value; } }

        ///<summary>
        ///菜单ID
        ///</summary>
        public QTableColumn<int> MenuID { get { return _MenuID; } set { _MenuID.NewValue = value; } }

        ///<summary>
        ///CODE
        ///</summary>
        public QTableColumn<string> Code { get { return _Code; } set { _Code.NewValue = value; } }

        ///<summary>
        ///操作名
        ///</summary>
        public QTableColumn<string> ActionName { get { return _ActionName; } set { _ActionName.NewValue = value; } }

    }

    public class TbArea : QTable<ToolGood.ReadyGo3.CoreTest.Datas.DbArea>
    {
        private QTableColumn<int> _Id;
        private QTableColumn<int> _ParentId;
        private QTableColumn<string> _Path;
        private QTableColumn<int> _Level;
        private QTableColumn<string> _Name;
        private QTableColumn<string> _NameEn;
        private QTableColumn<string> _NamePinyin;
        private QTableColumn<string> _Code;
        private QTableColumn<DateTime> _AddingTime;

        public TbArea() : base(Config.DbHelper) { }
        public TbArea(SqlHelper sqlHelper) : base(sqlHelper) { }

        protected override void Init()
        {
            __SchemaName__ = "";
            __TableName__ = "Area";

            _Id = AddColumn<int>("Id", "Id", true);
            _ParentId = AddColumn<int>("ParentId", "ParentId", false);
            _Path = AddColumn<string>("Path", "Path", false);
            _Level = AddColumn<int>("Level", "Level", false);
            _Name = AddColumn<string>("Name", "Name", false);
            _NameEn = AddColumn<string>("NameEn", "NameEn", false);
            _NamePinyin = AddColumn<string>("NamePinyin", "NamePinyin", false);
            _Code = AddColumn<string>("Code", "Code", false);
            _AddingTime = AddColumn<DateTime>("AddingTime", "AddingTime", false);
        }


        public QTableColumn<int> Id { get { return _Id; } set { _Id.NewValue = value; } }


        public QTableColumn<int> ParentId { get { return _ParentId; } set { _ParentId.NewValue = value; } }


        public QTableColumn<string> Path { get { return _Path; } set { _Path.NewValue = value; } }


        public QTableColumn<int> Level { get { return _Level; } set { _Level.NewValue = value; } }


        public QTableColumn<string> Name { get { return _Name; } set { _Name.NewValue = value; } }


        public QTableColumn<string> NameEn { get { return _NameEn; } set { _NameEn.NewValue = value; } }


        public QTableColumn<string> NamePinyin { get { return _NamePinyin; } set { _NamePinyin.NewValue = value; } }


        public QTableColumn<string> Code { get { return _Code; } set { _Code.NewValue = value; } }

        public QTableColumn<DateTime> AddingTime { get { return _AddingTime; } set { _AddingTime.NewValue = value; } }

    }

}




