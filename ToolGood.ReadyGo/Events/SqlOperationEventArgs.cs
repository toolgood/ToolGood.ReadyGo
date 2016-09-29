namespace ToolGood.ReadyGo.Events
{
    public class SqlOperationEventArgs : System.EventArgs
    {
        public SqlOperationEventArgs(string sql, object[] args, string sqlWithArgs)
        {
            Sql = sql;
            Args = args;
            SqlWithArgs = sqlWithArgs;
        }

        public string Sql { get; set; }
        public object[] Args { get; set; }
        public string SqlWithArgs { get; set; }
    }

    public delegate void SqlOperationEventHandler(object sender, SqlOperationEventArgs args);
}