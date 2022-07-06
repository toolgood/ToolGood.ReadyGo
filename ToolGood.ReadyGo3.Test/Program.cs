namespace ToolGood.ReadyGo3.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {

            using (var helper=SqlHelperFactory.OpenSqliteFile("1.db")) {
                helper._TableHelper.CreateTable(typeof(testArray));


                helper.Insert(new testArray() { });


                helper.Update<testArray>("set doubleArray=@0 where id=@1", new double[2] { 1, 2 }, 1);


            }


        }
    }
    public class testArray
    {
        public int id { get; set; }

        public double[] doubleArray { get; set; }
    }
}