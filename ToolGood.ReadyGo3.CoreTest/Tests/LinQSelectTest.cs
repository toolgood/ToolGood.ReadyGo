using System;
using System.Collections.Generic;
using System.Text;
using PetaTest;
using ToolGood.ReadyGo3.CoreTest.Datas;
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
            helper.Where<DbArea>(q => q.Id == 2).Single();
            helper.Where<DbArea>(q => q.Id == 2).SingleOrDefault();
            helper.Where<DbArea>(q => q.Level == 2).First();
            helper.Where<DbArea>(q => q.Level == 2).FirstOrDefault();
            helper.Where<DbArea>(q => q.Level == 2).Select();
            helper.Where<DbArea>(q => q.Level == 2).Select(10);
            helper.Where<DbArea>(q => q.Level == 2).Select(2, 10);
            helper.Where<DbArea>(q => q.Level == 2).Page(2, 10);

            helper.Where<DbArea>(q => q.Level == 2).ExecuteDataTable();

            var db = helper.Where<DbArea>()
                      .Where("Id=@0", 8)
                      .Where("ParentId=@0", 1)
                      .First();
            db = helper.Where<DbArea>()
                       .Where(q=>q.NamePinyin.ToLower()== "yazhou")
                      .First();
        }


        [Test]
        public void LinQAsyncTest()
        {
            var helper = Config.DbHelper;
            helper.Where<DbArea>(q => q.Id == 2).SingleAsync();
            helper.Where<DbArea>(q => q.Id == 2).SingleOrDefaultAsync();
            helper.Where<DbArea>(q => q.Level == 2).FirstAsync();
            helper.Where<DbArea>(q => q.Level == 2).FirstOrDefaultAsync();

            helper.Where<DbArea>(q => q.Level == 2).SelectAsync();
            helper.Where<DbArea>(q => q.Level == 2).SelectAsync(10);

            helper.Where<DbArea>(q => q.Level == 2).SelectAsync(2, 10);

            helper.Where<DbArea>(q => q.Level == 2).PageAsync(2, 10);
            helper.Where<DbArea>(q => q.Level == 2).ExecuteDataTableAsync();

        }

        [Test]
        public void LinQWhere()
        {
            var helper = Config.DbHelper;

            var db = helper.Where<DbArea>()
                .Where("Id=@0", 8)
                .Where("ParentId=@0", 1)
                .First();

            var dbs = helper.Where<DbArea>()
                 .Where("ParentId=@0", 1)
                 .WhereIn("Name", new List<string>() { "中国", "英国" })
                 .Select();

            dbs = helper.Where<DbArea>()
                  .Where("ParentId=@0", 1)
                  .WhereNotIn("Name", new List<string>() { "中国", "英国" })
                  .Select();

            dbs = helper.Where<DbArea>()
                .IfSet("1") .Where("ParentId=@0", 1)
                 .WhereIn("Name", new List<string>() { "中国", "英国" })
                 .Select();
        }

    }
}
