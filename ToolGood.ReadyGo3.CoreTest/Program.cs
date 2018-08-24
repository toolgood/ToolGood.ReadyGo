using System;
using ToolGood.ReadyGo3.Test;

namespace ToolGood.ReadyGo3.CoreTest
{
    class Program
    {
        
        static void Main(string[] args)
        {
            // SQLite is error in Microsoft.NETCore.App
            // SQLite is run in Microsoft.AspNetCore.All


            Setup.Start();
            //SQLitePCL.Batteries.Init();
            PetaTest.Runner.RunMain(args);
            Config.DbHelper.Dispose();
        }
    }
}
