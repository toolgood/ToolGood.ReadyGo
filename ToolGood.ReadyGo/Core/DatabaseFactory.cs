namespace ToolGood.ReadyGo.NPoco
{
    public class DatabaseFactory
    {
        public static IColumnSerializer ColumnSerializer = new FastJsonColumnSerializer();
    }
}
