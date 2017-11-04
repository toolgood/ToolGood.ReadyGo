using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.Attributes;
using PetaTest;
using ToolGood.ReadyGo3.Test.Datas;

namespace ToolGood.ReadyGo3.Test.Tests
{
    [TestFixture]
    public class AttributeTest
    {
        public static DbAdmin admin;
        [TestFixtureSetUp]
        public void Start()
        {
            admin = new DbAdmin() {
                AddingTime = DateTime.Now,
                AdminGroupID = 1,
                Email = "3322@qq.com",
                IsDelete = false,
                Name = "red",
                Password = "red",
                Phone = "1395860000",
                TrueName = "小红",
            };
            var helper = Config.TempHelper;
            helper.Insert(admin);
        }
        [Test]
        public void PrimaryKeyAttribute()
        {
            var helper = Config.TempHelper;
            var a1 = helper.SingleById<DbAdmin>(admin.ID);
            var a2 = helper.Single<DbAdmin>("where ID=@0", admin.ID);
        }

        [Test]
        public void ResultColumnAttribute()
        {
            var helper = Config.TempHelper;
            var a1 = helper.SingleById<DbAdmin>(admin.ID);
            var g1 = helper.SingleById<DbAdminGroup>(admin.AdminGroupID);
            Assert.AreEqual(g1.Name, a1.AdminGroupName);
        }



    }
}
