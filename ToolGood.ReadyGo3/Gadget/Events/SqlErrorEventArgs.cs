namespace ToolGood.ReadyGo3.Gadget.Events
{
    /// <summary>
    /// sql错误事件事件参数 
    /// </summary>
    public class SqlErrorEventArgs : System.EventArgs
    {
        /// <summary>
        /// sql错误事件事件参数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="sqlWithArgs"></param>
        /// <param name="errorMsg"></param>
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