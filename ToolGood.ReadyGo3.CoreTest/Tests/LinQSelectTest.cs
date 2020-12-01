using System;
using System.Collections.Generic;
using System.Text;
using PetaTest;
using ToolGood.ReadyGo3.Test.Datas;
using ToolGood.ReadyGo3.Test;

namespace ToolGood.ReadyGo3.CoreTest.Tests
{
    [TestFixture]
    public class LinQSelectTest
    {
        [Test]
        public void LinQTest()
        {
            var helper = Config.DbHelper;
            helper.Where<DbArea>(q => q.Level == 2).FirstOrDefault();
            helper.Where<DbArea>(q => q.Level == 2).Select();
            helper.Where<DbArea>(q => q.Level == 2).Select(10);
            helper.Where<DbArea>(q => q.Level == 2).Select(2, 10);
            helper.Where<DbArea>(q => q.Level == 2).Page(2, 10);

            helper.Where<DbArea>(q => q.Level == 2).ExecuteDataTable();


            var db = helper.Where<DbArea>()
                      .Where("Id=@0", 8)
                      .Where("ParentId=@0", 1)
                      .FirstOrDefault();
            #region 字符串操作
            db = helper.Where<DbArea>()
                       .Where(q => q.NamePinyin.ToLower() == "yazhou")
                      .FirstOrDefault();
            db = helper.Where<DbArea>()
                  .Where(q => q.NamePinyin.ToUpper() == "yazhou".ToUpper())
                 .FirstOrDefault();

            db = helper.Where<DbArea>()
               .Where(q => q.NamePinyin.Substring(1, 2) == "ya")
              .FirstOrDefault();

            db = helper.Where<DbArea>()
                .Where(q => q.NamePinyin.Contains("ya"))
               .FirstOrDefault();

            db = helper.Where<DbArea>()
                   .Where(q => q.NamePinyin.StartsWith("ya"))
                  .FirstOrDefault();
            db = helper.Where<DbArea>()
                         .Where(q => q.NamePinyin.EndsWith("ou"))
                        .FirstOrDefault();
            db = helper.Where<DbArea>()
                         .Where(q => q.NamePinyin.Trim() == ("yazhou"))
                        .FirstOrDefault();
            db = helper.Where<DbArea>()
                 .Where(q => q.NamePinyin.TrimStart() == ("yazhou"))
                .FirstOrDefault();
            db = helper.Where<DbArea>()
                 .Where(q => q.NamePinyin.TrimEnd() == ("yazhou"))
                .FirstOrDefault();
            db = helper.Where<DbArea>()
                 .Where(q => q.NamePinyin.ToString() == ("yazhou"))
                .FirstOrDefault();
            #endregion

            #region 日期操作
            db = helper.Where<DbArea>()
                .Where(q => q.AddingTime.Year < 2019)
               .FirstOrDefault();
            db = helper.Where<DbArea>()
               .Where(q => q.AddingTime.Year <= DateTime.Now.Year)
              .FirstOrDefault();

            var db2 = helper.Where<DbArea2>()
                      .Where(q => q.AddingTime.Value.Year < 2019)
                     .FirstOrDefault();
            db2 = helper.Where<DbArea2>()
                   .Where(q => q.AddingTime.Value.Year <= DateTime.Now.Year)
                  .FirstOrDefault();
            db2 = helper.Where<DbArea2>()
                  .Where(q => q.AddingTime.Value < DateTime.Now)
                 .FirstOrDefault();
            db2 = helper.Where<DbArea2>()
                .Where(q => q.AddingTime < DateTime.Now)
               .FirstOrDefault();
            db2 = helper.Where<DbArea2>()
                 .Where(q => q.AddingTime == null)
                .FirstOrDefault();

            db2 = helper.Where<DbArea2>()
                 .Where(q => q.AddingTime != null)
                .FirstOrDefault();
            #endregion

        }


        [Test]
        public void LinQAsyncTest()
        {
            var helper = Config.DbHelper;
            helper.Where<DbArea>(q => q.Level == 2).FirstOrDefault_Async();

            helper.Where<DbArea>(q => q.Level == 2).Select_Async();
            helper.Where<DbArea>(q => q.Level == 2).Select_Async(10);

            helper.Where<DbArea>(q => q.Level == 2).Select_Async(2, 10);

            helper.Where<DbArea>(q => q.Level == 2).Page_Async(2, 10);
            helper.Where<DbArea>(q => q.Level == 2).ExecuteDataTable_Async();

        }

        [Test]
        public void LinQWhere()
        {
            var helper = Config.DbHelper;

            var db = helper.Where<DbArea>()
                .Where("Id=@0", 8)
                .Where("ParentId=@0", 1)
                .FirstOrDefault();

            var dbs = helper.Where<DbArea>()
                 .Where("ParentId=@0", 1)
                 .WhereIn("Name", new List<string>() { "中国", "英国" })
                 .Select();

            dbs = helper.Where<DbArea>()
                  .Where("ParentId=@0", 1)
                  .WhereNotIn("Name", new List<string>() { "中国", "英国" })
                  .Select();

            dbs = helper.Where<DbArea>()
                .IfSet("1").Where("ParentId=@0", 1)
                 .WhereIn("Name", new List<string>() { "中国", "英国" })
                 .Select();
        }

        [Test]
        public void Update()
        {
            var helper = Config.DbHelper;
            helper.Where<DbArea>()
             .Where("Id=@0", 1125)//圣菲利普
             .Update("Name=@0,NameEn=@1", "测试", "123");

            var db = helper.FirstOrDefault<DbArea>(1125);
            Assert.AreEqual("测试", db.Name);
            Assert.AreEqual("123", db.NameEn);

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["Name"] = "测试";
            dict["NameEn"] = "456";

            helper.Where<DbArea>()
                 .Where("Id=@0", 1175)
                 .Update(dict);

            db = helper.FirstOrDefault<DbArea>(1175);
            Assert.AreEqual("测试", db.Name);
            Assert.AreEqual("456", db.NameEn);

            helper.Where<DbArea>()
                 .Where("Id=@0", 1185)
                 .Update(new { Name = "测试", NameEn = "789" });

            db = helper.FirstOrDefault<DbArea>(1185);
            Assert.AreEqual("测试", db.Name);
            Assert.AreEqual("789", db.NameEn);



        }

        [Test]
        public void Delete()
        {
            var helper = Config.DbHelper;

            helper.Where<DbArea>()
                .Where("Id=@0", 1990)
                .Delete();

            var db = helper.FirstOrDefault<DbArea>(1990);
            Assert.AreEqual(null, db);
        }

        [Test]
        public async void SelectInsertTest()
        {
            var helper = Config.DbHelper;

            helper.Where<DbArea>(q => q.Id < 500).SelectInsert<DbArea3>("", "1 as Level");
            await helper.Where<DbArea>(q => q.Id < 500).FirstOrDefault_Async<DbArea3>();

        }

    }
}
