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

            //List<SqlParameter> parameters = new List<SqlParameter>();
            //parameters.Add(new SqlParameter("id", 1));
            //var t22 = helper.Single<DbArea>("where id=@id", parameters);

            SqlParameterCollection parameters2 = new SqlParameterCollection();
            parameters2.Add("id", 1);
            var t222 = helper.FirstOrDefault<DbArea>("where id=@id", parameters2);


            //var ts1 = helper.SelectUnion<DbArea>("Select * from Area where id=@@", 1, 2, 3, 2);
            //var ts2 = helper.SelectUnionAll<DbArea>("Select * from Area where id=@@", 1, 2, 3, 2);

            var selectColumns = "Name,NameEn";
            var table = "Area";
            var where = "id>120";
            var tt1 = helper.SQL_Select<DbArea>(1, 20, selectColumns, table, null, where);
            var tt2 = helper.SQL_Select<DbArea>(2, 20, selectColumns, table, null, where);

            var tt11 = helper.SQL_Page<DbArea>(1, 20, selectColumns, table, null, where);
            var tt22 = helper.SQL_Page<DbArea>(2, 20, selectColumns, table, null, where);



            helper.FirstOrDefault<DbArea>("where id=@0", 1);
            helper.FirstOrDefault<DbArea>(1);
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



            var selectColumns = "Name,NameEn";
            var table = "Area";
            var where = "id>120";

            var tt13 = await helper.SQL_Select_Async<DbArea>(1, 20, selectColumns, table, null, where);
            var tt23 = await helper.SQL_Select_Async<DbArea>(2, 20, selectColumns, table, null, where);
            var tt33 = await helper.SQL_Page_Async<DbArea>(1, 20, selectColumns, table, null, where);
            var tt43 = await helper.SQL_Page_Async<DbArea>(5, 20, selectColumns, table, null, where);

            await helper.FirstOrDefault_Async<DbArea>("where id=@0", 1);
            await helper.FirstOrDefault_Async<DbArea>(1);
            await helper.FirstOrDefault_Async<DbArea>();

            await helper.Select_Async<DbArea>("where Level=@0", 2);
            await helper.Select_Async<DbArea>(10, "where Level=@0", 2);
            await helper.Select_Async<DbArea>(1, 10, "where Level=@0", 2);
            await helper.Page_Async<DbArea>(1, 10, "where Level=@0", 2);

            await helper.Execute_Async("select count(*) from Area");
            await helper.ExecuteDataTable_Async("select count(*) from Area where Level=@0", 2);
        }



    }
}
