using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3;
using ToolGood.ReadyGo3.Attributes;
using ToolGood.ReadyGo3.DataCentxt;

namespace ToolGood.ReadyGo3.Test.Datas
{
    public class TbAdmin : QTable<ToolGood.ReadyGo3.Test.Datas.DbAdmin>
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

        public TbAdmin() : base() { }
		public TbAdmin(SqlHelper sqlHelper) : base(sqlHelper) { }
		public TbAdmin(string connStringName) : base(connStringName) { }

        protected override void Init()
        {
            _SchemaName = "";
            _TableName = "Admin";

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

    public class TbAdminGroup : QTable<ToolGood.ReadyGo3.Test.Datas.DbAdminGroup>
    {
        private QTableColumn<int> _ID;
        private QTableColumn<string> _Name;
        private QTableColumn<string> _Describe;
        private QTableColumn<int> _Sort;
        private QTableColumn<bool> _IsDelete;
        private QTableColumn<System.DateTime> _AddingTime;

        public TbAdminGroup() : base() { }
		public TbAdminGroup(SqlHelper sqlHelper) : base(sqlHelper) { }
		public TbAdminGroup(string connStringName) : base(connStringName) { }

        protected override void Init()
        {
            _SchemaName = "";
            _TableName = "AdminGroup";

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

    public class TbAdminLoginLog : QTable<ToolGood.ReadyGo3.Test.Datas.DbAdminLoginLog>
    {
        private QTableColumn<int> _ID;
        private QTableColumn<string> _Name;
        private QTableColumn<string> _Message;
        private QTableColumn<string> _Ip;
        private QTableColumn<bool> _State;
        private QTableColumn<System.DateTime> _AddingTime;

        public TbAdminLoginLog() : base() { }
		public TbAdminLoginLog(SqlHelper sqlHelper) : base(sqlHelper) { }
		public TbAdminLoginLog(string connStringName) : base(connStringName) { }

        protected override void Init()
        {
            _SchemaName = "";
            _TableName = "AdminLoginLog";

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

    public class TbAdminMenu : QTable<ToolGood.ReadyGo3.Test.Datas.DbAdminMenu>
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

        public TbAdminMenu() : base() { }
		public TbAdminMenu(SqlHelper sqlHelper) : base(sqlHelper) { }
		public TbAdminMenu(string connStringName) : base(connStringName) { }

        protected override void Init()
        {
            _SchemaName = "";
            _TableName = "AdminMenu";

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

    public class TbAdminMenuPass : QTable<ToolGood.ReadyGo3.Test.Datas.DbAdminMenuPass>
    {
        private QTableColumn<int> _AdminGroupID;
        private QTableColumn<int> _MenuID;
        private QTableColumn<string> _Code;
        private QTableColumn<string> _ActionName;

        public TbAdminMenuPass() : base() { }
		public TbAdminMenuPass(SqlHelper sqlHelper) : base(sqlHelper) { }
		public TbAdminMenuPass(string connStringName) : base(connStringName) { }

        protected override void Init()
        {
            _SchemaName = "";
            _TableName = "AdminMenuPass";

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

}




