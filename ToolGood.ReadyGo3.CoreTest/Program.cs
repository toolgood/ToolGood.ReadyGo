using System;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.Test;
using ToolGood.ReadyGo3.Test.Datas;

namespace ToolGood.ReadyGo3.CoreTest
{
    class Program
    {

        static void Main(string[] args)
        {
            // SQLite is error in Microsoft.NETCore.App
            // SQLite is run in Microsoft.AspNetCore.All


            //Setup.Start();
            var helper = Config.SqlServerHelper;
            var selectColumns = "Name,NameEn";
            var table = "(Select id,Name,NameEn from Area) t";
            var where = "id>120";
            var tt1 = helper.SQL_Select<DbArea>(1, 20, selectColumns, table, null, where);
            var tt2 = helper.SQL_Select<DbArea>(2, 20, selectColumns, table, null, where);

            var tt11 = helper.SQL_Page<DbArea>(1, 20, selectColumns, table, null, where);
            var tt22 = helper.SQL_Page<DbArea>(2, 20, selectColumns, table, null, where);

            var t = tt(helper);
            t.Wait();

            //SQLitePCL.Batteries.Init();
            PetaTest.Runner.RunMain(args);
            Config.DbHelper.Dispose();
        }

        public async static Task tt(SqlHelper helper)
        {
            var selectColumns = "Name,NameEn";
            var table = "Area";
            var where = "id>120";

            var tt13 = await helper.SQL_SelectAsync<DbArea>(1, 20, selectColumns, table, null, where);
            var tt23 = await helper.SQL_SelectAsync<DbArea>(2, 20, selectColumns, table, null, where);
            var tt33 = await helper.SQL_PageAsync<DbArea>(1, 20, selectColumns, table, null, where);
            var tt43 = await helper.SQL_PageAsync<DbArea>(5, 20, selectColumns, table, null, where);

        }
    }
}
