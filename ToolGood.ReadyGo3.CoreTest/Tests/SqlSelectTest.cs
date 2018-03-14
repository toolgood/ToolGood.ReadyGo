using System;
using System.Collections.Generic;
using System.Text;
using PetaTest;
using ToolGood.ReadyGo3.Test;
using ToolGood.ReadyGo3.CoreTest.Datas;

namespace ToolGood.ReadyGo3.CoreTest.Tests
{
    [TestFixture]
    public class SqlSelectTest
    {
        [Test]
        public void SelectTest()
        {
            var helper = Config.DbHelper;

            helper.Single<DbArea>("where id=@0", 1);
            helper.SingleOrDefault<DbArea>("where id=@0", 1);
            helper.SingleById<DbArea>(1);
            helper.SingleOrDefaultById<DbArea>(1);
            helper.First<DbArea>();
            helper.FirstOrDefault<DbArea>();

            helper.Select<DbArea>("where Level=@0", 2);
            helper.Select<DbArea>(10, "where Level=@0", 2);
            helper.Select<DbArea>(1, 10, "where Level=@0", 2);
            helper.Page<DbArea>(1, 10, "where Level=@0", 2);

            helper.Execute("select count(*) from Area");
            helper.ExecuteDataTable("select count(*) from Area where Level=@0", 2);

        }




        [Test]
        public async void SelectAsyncTest()
        {
            var helper = Config.DbHelper;

            await helper.SingleAsync<DbArea>("where id=@0", 1);
            await helper.SingleOrDefaultAsync<DbArea>("where id=@0", 1);
            await helper.SingleByIdAsync<DbArea>(1);
            await helper.SingleOrDefaultByIdAsync<DbArea>(1);
            await helper.FirstAsync<DbArea>();
            await helper.FirstOrDefaultAsync<DbArea>();

            await helper.SelectAsync<DbArea>("where Level=@0", 2);
            await helper.SelectAsync<DbArea>(10, "where Level=@0", 2);
            await helper.SelectAsync<DbArea>(1, 10, "where Level=@0", 2);
            await helper.PageAsync<DbArea>(1, 10, "where Level=@0", 2);

            await helper.ExecuteAsync("select count(*) from Area");
            await helper.ExecuteDataTableAsync("select count(*) from Area where Level=@0", 2);
        }



    }
}
