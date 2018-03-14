using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo3;
using ToolGood.ReadyGo3.Test;
using ToolGood.ReadyGo3.CoreTest.Datas;

namespace ToolGood.ReadyGo3.CoreTest
{
    public partial class Setup
    {
        public static void Start()
        {
            if (File.Exists(Config.DbFile)) {
                File.Delete(Config.DbFile);
            }
            File.Create(Config.DbFile).Close();
            var helper2 = Config.DbHelper;
            InitTable(helper2);
            CreateData(helper2);
            helper2.Dispose();
        }

        private static void InitTable(SqlHelper helper)
        {
            var table = helper._TableHelper;
            table.CreateTable(typeof(DbAdmin));
            table.CreateTable(typeof(DbAdminGroup));
            table.CreateTable(typeof(DbAdminLoginLog));
            table.CreateTable(typeof(DbAdminMenu));
            table.CreateTable(typeof(DbAdminMenuPass));
            table.CreateTable(typeof(DbArea));
        }


        private static void CreateData(SqlHelper helper)
        {
            helper._Config.Insert_DateTime_Default_Now = true;
            #region Admin
            helper.Insert(new DbAdmin() {
                Name = "admin",
                Password = "111",
                TrueName = "超级管理员",
                Phone = "",
                Email = "",
                AdminGroupID=1,
            });
            #endregion

            #region AdminGroup
            helper.Insert(new DbAdminGroup() {

                Name = "超级管理员",
                Sort = 1,
            });
            #endregion

            #region AdminMenu
            {
                var menuIndex = 1000;

                {
                    var adminDesktop = new DbAdminMenu() { Name = "管理面板", Icon = "fa-gears", Code = "AdminDesktop", Url = "", Actions = "navbar|show", ParentID = 0, Sort = menuIndex++, };
                    helper.Insert(adminDesktop);
                    var index = 1;
                    helper.Insert(new DbAdminMenu() { Name = "服务器信息", Icon = "", Code = "ServerInfo", Url = "/Admin/Manage/ServerInfo", Actions = "navbar|show", ParentID = adminDesktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "菜单管理", Icon = "", Code = "AdminMenu", Url = "/Admin/Manage/AdminMenu", Actions = "navbar|show|add|edit|delete", ParentID = adminDesktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "角色管理", Icon = "", Code = "AdminGroup", Url = "/Admin/Manage/AdminGroup", Actions = "navbar|show|add|edit|delete", ParentID = adminDesktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "管理员列表", Icon = "", Code = "Admin", Url = "/Admin/Manage/Admin", Actions = "navbar|show|add|edit|delete", ParentID = adminDesktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "登录日志", Icon = "", Code = "AdminLoginLog", Url = "/Admin/Manage/AdminLoginLog", Actions = "navbar|show", ParentID = adminDesktop.ID, Sort = index++, });

                    helper.Insert(new DbAdminMenu() { Name = "文件管理", Icon = "", Code = "FileManager", Url = "/Admin/Manage/FileManager", Actions = "navbar|show", ParentID = adminDesktop.ID, Sort = index++, });

                }
                menuIndex = 1;
                {
                    var desktop = new DbAdminMenu() { Name = "首页", Icon = "fa-pie-chart", Code = "Desktop", Url = "/Admin/Desktop/Index", Actions = "navbar|show", ParentID = 0, Sort = menuIndex++, };
                    helper.Insert(desktop);
                    //var index = 1;

                    //helper.Insert(new DbAdminMenu() { Name = "首页", Icon = "", Code = "Index", Url = "/Admin/Desktop/Index", Actions = "navbar|show", ParentID = desktop.ID, Sort = index++,  });

                }
                menuIndex--;
                {
                    var desktop = new DbAdminMenu() { Name = "网站管理", Icon = "fa-internet-explorer", Code = "WebsiteDesktop", Url = "", Actions = "navbar|show", ParentID = 0, Sort = menuIndex++, };
                    helper.Insert(desktop);
                    var index = 1;
                    helper.Insert(new DbAdminMenu() { Name = "站点信息管理", Icon = "", Code = "Website", Url = "/Admin/Website", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "栏目管理", Icon = "", Code = "Menu", Url = "/Admin/Menu", Actions = "navbar|show|add|edit|delete|code", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "文章管理", Icon = "", Code = "Article", Url = "/Admin/Article", Actions = "navbar|show|add|edit|delete|code", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "留言管理", Icon = "", Code = "ArticleComment", Url = "/Admin/ArticleComment", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });

                    helper.Insert(new DbAdminMenu() { Name = "专题管理", Icon = "", Code = "Special", Url = "/Admin/Special", Actions = "navbar|show|add|edit|delete|code", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "标签管理", Icon = "", Code = "Tag", Url = "/Admin/Tag", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });

                    //helper.Insert(new DbAdminMenu() { Name = "招聘管理", Icon = "", Code = "Job", Url = "/Admin/Job", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++,  });
                    helper.Insert(new DbAdminMenu() { Name = "应聘管理", Icon = "", Code = "JobResume", Url = "/Admin/Job/JobResume", Actions = "navbar|show|delete|message", ParentID = desktop.ID, Sort = index++, });

                    helper.Insert(new DbAdminMenu() { Name = "幻灯片分组管理", Icon = "", Code = "BannerGroup", Url = "/Admin/Banner/Group", Actions = "navbar|show|add|edit|delete|code", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "幻灯片管理", Icon = "", Code = "Banner", Url = "/Admin/Banner", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "友情链接分组管理", Icon = "", Code = "LinkGroup", Url = "/Admin/Link/Group", Actions = "navbar|show|add|edit|delete|code", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "友情链接管理", Icon = "", Code = "Link", Url = "/Admin/Link", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });

                    helper.Insert(new DbAdminMenu() { Name = "客服栏目管理", Icon = "", Code = "CustomService", Url = "/Admin/CustomService", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });



                }
                menuIndex--;
                {
                    var desktop = new DbAdminMenu() { Name = "商城管理", Icon = "fa-internet-explorer", Code = "ShopDesktop", Url = "", Actions = "navbar|show", ParentID = 0, Sort = menuIndex++, };
                    helper.Insert(desktop);
                    var index = 1;
                    helper.Insert(new DbAdminMenu() { Name = "产品管理", Icon = "", Code = "Product", Url = "/Admin/Shop/Product", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "产品属性管理", Icon = "", Code = "Product1", Url = "/Admin/Shop", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "产品标签管理", Icon = "", Code = "Product2", Url = "/Admin/Shop", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "优惠卷管理", Icon = "", Code = "Product3", Url = "/Admin/Shop", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "订单管理", Icon = "", Code = "Product4", Url = "/Admin/Shop", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "产品评论管理", Icon = "", Code = "Product5", Url = "/Admin/Shop", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });




                }
                menuIndex--;
                {
                    var desktop = new DbAdminMenu() { Name = "会员管理", Icon = "fa-internet-explorer", Code = "MemberDesktop", Url = "", Actions = "navbar|show", ParentID = 0, Sort = menuIndex++, };
                    helper.Insert(desktop);
                    var index = 1;
                    helper.Insert(new DbAdminMenu() { Name = "会员分组管理", Icon = "", Code = "MemberGroup", Url = "/Admin/Member/Group", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "会员管理", Icon = "", Code = "Member", Url = "/Admin/Member", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "会员优惠卷", Icon = "", Code = "MemberCoupon", Url = "/Admin/MemberCoupon", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });




                }
                menuIndex--;
                {
                    var desktop = new DbAdminMenu() { Name = "软件运营", Icon = "fa-cubes", Code = "AppDesktop", Url = "", Actions = "navbar|show", ParentID = 0, Sort = menuIndex++, };
                    helper.Insert(desktop);
                    var index = 1;
                    helper.Insert(new DbAdminMenu() { Name = "软件管理", Icon = "", Code = "App", Url = "/Admin/App/", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "版本管理", Icon = "", Code = "AppVersion", Url = "/Admin/App/Version", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "渠道管理", Icon = "", Code = "AppChannel", Url = "/Admin/App/Channel", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "销售管理", Icon = "", Code = "AppSale", Url = "/Admin/App/Sale", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });

                    helper.Insert(new DbAdminMenu() { Name = "建议反馈", Icon = "", Code = "AppFeedback", Url = "/Admin/App/Feedback", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "技术支持", Icon = "", Code = "AppSupport", Url = "/Admin/App/Support", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                }

                menuIndex--;
                {
                    var desktop = new DbAdminMenu() { Name = "广告营销管理", Icon = "fa-internet-explorer", Code = "AdDesktop", Url = "", Actions = "navbar|show", ParentID = 0, Sort = menuIndex++, };
                    helper.Insert(desktop);
                    var index = 1;
                    helper.Insert(new DbAdminMenu() { Name = "广告分组管理", Icon = "", Code = "AdGroup", Url = "/Admin/Ad/AdGroup", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });
                    helper.Insert(new DbAdminMenu() { Name = "广告管理", Icon = "", Code = "Ad", Url = "/Admin/Ad", Actions = "navbar|show|add|edit|delete", ParentID = desktop.ID, Sort = index++, });


                }

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
 

            #region Area

            var areas = GetArea();
            helper.InsertList(areas);

            #endregion

      
        }


    }
}
