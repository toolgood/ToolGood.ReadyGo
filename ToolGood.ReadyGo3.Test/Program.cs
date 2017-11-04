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
            Setup.Start();
            PetaTest.Runner.RunMain(args);
        }



    }
}
