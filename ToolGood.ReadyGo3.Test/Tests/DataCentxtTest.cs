using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using ToolGood.ReadyGo3.Test.Datas;

namespace ToolGood.ReadyGo3.Test.Tests
{
    [TestFixture]
    public class DataCentxtTest
    {

        [Test]
        public void Select()
        {
            //TbAdminMenu tb = new TbAdminMenu();
            //var list = tb.GetList();
            
            //var list2 = tb.Where(tb.ParentID == 0).GetList();

            //var menu = tb.Where(tb.Name == "管理面板").GetFirst();

        }

        [Test]
        public void Insert()
        {
            TbAdmin admin = new TbAdmin();
            admin.AddingTime = DateTime.Now;
            admin.AdminGroupID = 1;
            admin.Email = "3322@qq.com";
            admin.IsDelete = false;
            admin.Name = "red";
            admin.Password = "red";
            admin.Phone = "1395860000";
            admin.TrueName = "小红";

           var d= admin.Insert(true);
           var d2= admin.Insert(false);
            admin.Clear();



        }




        [Test]
        public void Update()
        {

        }





    }
}
