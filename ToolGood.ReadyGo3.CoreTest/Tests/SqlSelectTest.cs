using System;
using System.Collections.Generic;
using System.Text;
using PetaTest;
using ToolGood.ReadyGo3.Test;
using ToolGood.ReadyGo3.Test.Datas;

namespace ToolGood.ReadyGo3.CoreTest.Tests
{
    [TestFixture]
    public class SqlSelectTest
    {
        [Test]
        public void SelectTest()
        {
            var helper = Config.DbHelper;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("id", 1));
            var t22 = helper.Single<DbArea>("where id=@id", parameters);

            SqlParameterCollection parameters2 = new SqlParameterCollection();
            parameters2.Add("id", 1);
            var t222 = helper.Single<DbArea>("where id=@id", parameters2);


            var ts1 = helper.SelectUnion<DbArea>("Select * from Area where id=@@", 1, 2, 3, 2);
            var ts2 = helper.SelectUnionAll<DbArea>("Select * from Area where id=@@", 1, 2, 3, 2);


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

            ISqlHelperSync h = helper;
            ISqlHelperAsync t = helper;


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
