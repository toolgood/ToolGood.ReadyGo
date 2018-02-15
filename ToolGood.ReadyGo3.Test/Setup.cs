using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.Test.Datas;

namespace ToolGood.ReadyGo3.Test
{
    public class Setup
    {
        public static void Start()
        {
            if (File.Exists(Config.DbFile)) {
                File.Delete(Config.DbFile);
            }
            File.Create(Config.DbFile).Close();
            var helper = SqlHelperFactory.OpenSqliteFile(Config.DbFile);
            InitTable(helper);
            InitDatas(helper);

        }

        private static void InitTable(SqlHelper helper)
        {
            var table = helper._TableHelper;
            table.CreateTable(typeof(DbAdmin));
            table.CreateTable(typeof(DbAdminGroup));
            table.CreateTable(typeof(DbAdminLoginLog));
            table.CreateTable(typeof(DbAdminMenu));
            table.CreateTable(typeof(DbAdminMenuPass));
        }

        private static void InitDatas(SqlHelper helper)
        {
            #region Admin
            helper.Insert(new DbAdmin() {
                Name = "admin",
                Password = "admin",
                TrueName = "超级管理员",
                Phone = "",
                Email = "",
                AdminGroupID = 1,
                AddingTime = DateTime.Now,
            });
            #endregion

            #region AdminGroup
            helper.Insert(new DbAdminGroup() {
                AddingTime = DateTime.Now,
                Name = "超级管理员",
                Sort = 1,
            });
            #endregion

            #region AdminMenu

            {
                var adminDesktop = new DbAdminMenu() { Name = "管理面板", Icon = "fa-gears", Code = "AdminDesktop", Url = "", Actions = "navbar|show", ParentID = 0, Sort = 999, AddingTime = DateTime.Now };
                helper.Insert(adminDesktop);
                var index = 1;

                helper.Insert(new DbAdminMenu() { Name = "服务器信息", Icon = "", Code = "ServerInfo", Url = "/Admin/Manage/ServerInfo", Actions = "navbar|show", ParentID = adminDesktop.ID, Sort = index++, AddingTime = DateTime.Now });
                helper.Insert(new DbAdminMenu() { Name = "菜单管理", Icon = "", Code = "AdminMenu", Url = "/Admin/Manage/AdminMenu", Actions = "navbar|show|add|edit|delete", ParentID = adminDesktop.ID, Sort = index++, AddingTime = DateTime.Now });
                helper.Insert(new DbAdminMenu() { Name = "角色管理", Icon = "", Code = "AdminGroup", Url = "/Admin/Manage/AdminGroup", Actions = "navbar|show|add|edit|delete", ParentID = adminDesktop.ID, Sort = index++, AddingTime = DateTime.Now });
                helper.Insert(new DbAdminMenu() { Name = "管理员列表", Icon = "", Code = "Admin", Url = "/Admin/Manage/Admin", Actions = "navbar|show|add|edit|delete", ParentID = adminDesktop.ID, Sort = index++, AddingTime = DateTime.Now });
                helper.Insert(new DbAdminMenu() { Name = "登录日志", Icon = "", Code = "AdminLoginLog", Url = "/Admin/Manage/AdminLoginLog", Actions = "navbar|show", ParentID = adminDesktop.ID, Sort = index++, AddingTime = DateTime.Now });

                helper.Insert(new DbAdminMenu() { Name = "文件管理", Icon = "", Code = "FileManager", Url = "/Admin/Manage/FileManager", Actions = "navbar|show", ParentID = adminDesktop.ID, Sort = index++, AddingTime = DateTime.Now });

            }
            var menuIndex = 1;
            {
                var desktop = new DbAdminMenu() { Name = "统计面板", Icon = "fa-pie-chart", Code = "StatisticsDesktop", Url = "", Actions = "navbar|show", ParentID = 0, Sort = menuIndex++, AddingTime = DateTime.Now };
                helper.Insert(desktop);
                var index = 1;

                helper.Insert(new DbAdminMenu() { Name = "首页", Icon = "", Code = "Index", Url = "/Admin/Statistics/Index", Actions = "navbar|show", ParentID = desktop.ID, Sort = index++, AddingTime = DateTime.Now });

            }
            menuIndex++;
            {
                var desktop = new DbAdminMenu() { Name = "网站管理", Icon = "fa-internet-explorer", Code = "WebsiteDesktop", Url = "", Actions = "navbar|show", ParentID = 0, Sort = menuIndex++, AddingTime = DateTime.Now };
                helper.Insert(desktop);
                var index = 1;
                helper.Insert(new DbAdminMenu() { Name = "网页管理", Icon = "", Code = "Web", Url = "/Admin/Website/Web", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, AddingTime = DateTime.Now });
                helper.Insert(new DbAdminMenu() { Name = "栏目管理", Icon = "", Code = "Menu", Url = "/Admin/Website/Menu", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, AddingTime = DateTime.Now });
                helper.Insert(new DbAdminMenu() { Name = "文章管理", Icon = "", Code = "Article", Url = "/Admin/Website/Article", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, AddingTime = DateTime.Now });
                helper.Insert(new DbAdminMenu() { Name = "留言管理", Icon = "", Code = "Message", Url = "/Admin/Website/Message", Actions = "navbar|show|add|edit|pass|delete", ParentID = desktop.ID, Sort = index++, AddingTime = DateTime.Now });
                helper.Insert(new DbAdminMenu() { Name = "广告管理", Icon = "", Code = "Ad", Url = "/Admin/Website/Ad", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, AddingTime = DateTime.Now });
                helper.Insert(new DbAdminMenu() { Name = "友情链接", Icon = "", Code = "Link", Url = "/Admin/Website/Link", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, AddingTime = DateTime.Now });

            }



            #endregion

            #region AdminMenuPass
            {
                List<DbAdminMenuPass> passList = new List<DbAdminMenuPass>();
                var menus = helper.Select<DbAdminMenu>();

                foreach (var item in menus) {
                    var acs = item.Actions.Split('|');
                    foreach (var ac in acs) {
                        passList.Add(new DbAdminMenuPass {
                            AdminGroupID = 1,
                            MenuID = item.ID,
                            ActionName = ac,
                            Code = item.Code
                        });
                    }
                }
                helper.InsertList(passList);
            }
            #endregion

        }

    }
}
