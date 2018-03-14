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

    }
}
