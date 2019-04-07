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
            var tt1 = helper.SelectSql<DbArea>(1, 20, selectColumns, table, null, where);
            var tt2 = helper.SelectSql<DbArea>(2, 20, selectColumns, table, null, where);

            var tt11 = helper.PageSql<DbArea>(1, 20, selectColumns, table, null, where);
            var tt22 = helper.PageSql<DbArea>(2, 20, selectColumns, table, null, where);

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

            var tt13 = await helper.SelectSqlAsync<DbArea>(1, 20, selectColumns, table, null, where);
            var tt23 = await helper.SelectSqlAsync<DbArea>(2, 20, selectColumns, table, null, where);
            var tt33 = await helper.PageSqlAsync<DbArea>(1, 20, selectColumns, table, null, where);
            var tt43 = await helper.PageSqlAsync<DbArea>(5, 20, selectColumns, table, null, where);

        }
    }
}
