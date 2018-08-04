using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.Test.Datas;

namespace ToolGood.ReadyGo3.Test
{
    class Program
    {
        static void Main(string[] args)
        {
        var helper=    SqlHelperFactory.OpenSqlServer("", "", "", "");

         var f=   helper.First<DbAdmin>("where IsDelete=0");

            Setup.Start();
            PetaTest.Runner.RunMain(args);


        }



    }
}
