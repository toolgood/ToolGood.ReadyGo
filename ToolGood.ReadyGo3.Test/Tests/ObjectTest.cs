using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using ToolGood.ReadyGo3.Test.Datas;

namespace ToolGood.ReadyGo3.Test.Tests
{
    [TestFixture]
    public class ObjectTest
    {
        [Test]
        public void insert()
        {
            var admin = new DbAdmin() {
                AddingTime = DateTime.Now,
                AdminGroupID = 1,
                Email = "1122@qq.com",
                IsDelete = false,
                Name = "xiaoMin",
                Password = "xm123456",
                Phone = "1395860000",
                TrueName = "小明",
            };
            var helper = Config.TempHelper;
            helper.Insert(admin);
            Assert.Greater(admin.ID, 0);
            var ad = helper.SingleById<DbAdmin>(admin.ID);
            Assert.IsNotNull(ad.AdminGroupName);
        }

        [Test]
        public void update()
        {
            var helper = Config.TempHelper;
            helper.Update<DbAdmin>("set Password='123'  where TrueName=@0", "小明");
            var admin = helper.Single<DbAdmin>("where TrueName=@0", "小明");
            Assert.AreEqual("123", admin.Password);

            //
            admin.TrueName = "XXX";
            helper.Update(admin);
            var admin2 = helper.SingleOrDefault <DbAdmin>("where TrueName=@0", "小明");
            Assert.IsNull(admin2);

        }

        [Test]
        public void select()
        {
            var helper = Config.TempHelper;
            var list = helper.Select<DbAdmin>();
            var list2 = helper.Select<DbAdmin>(1, 2);
            var page = helper.Page<DbAdmin>(1, 10);


            Assert.Greater(list.Count, 1);
        }

        [Test]
        public void delete()
        {
            var helper = Config.TempHelper;
            var admin2 = helper.Single<DbAdmin>("where TrueName=@0", "XXX");

            helper.Delete(admin2);
            helper.Delete<DbAdmin>("where id=@0", admin2.ID);
            helper.DeleteById<DbAdmin>(admin2.ID);
        }



    }
}
