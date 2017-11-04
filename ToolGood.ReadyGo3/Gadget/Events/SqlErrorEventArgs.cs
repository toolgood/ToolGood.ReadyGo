namespace ToolGood.ReadyGo3.Gadget.Events
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

        public string Sql;
        public object[] Args;

        public string SqlWithArgs;
        public string ErrorMsg;
        public bool Handle;
    }

    public delegate void SqlErrorEventHandler(object sender, SqlErrorEventArgs args);
}