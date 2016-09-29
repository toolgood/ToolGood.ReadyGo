namespace ToolGood.ReadyGo.Events
{
    public class SqlErrorEventArgs : System.EventArgs
    {
        public SqlErrorEventArgs(string sql, object[] args, string sqlWithArgs, string errorMsg)
        {
            SqlWithArgs = sqlWithArgs;
            ErrorMsg = errorMsg;
            Sql = sql;
            Args = args;
            Handle = false;
        }

        public string Sql { get; set; }
        public object[] Args { get; set; }

        public string SqlWithArgs { get; set; }
        public string ErrorMsg { get; set; }
        public bool Handle { get; set; }
    }

    public delegate void SqlErrorEventHandler(object sender, SqlErrorEventArgs args);
}