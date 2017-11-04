namespace ToolGood.ReadyGo3.Gadget.Events
{
    public class SqlOperationEventArgs : System.EventArgs
    {
        public SqlOperationEventArgs(string sql, object[] args, string sqlWithArgs)
        {
            Sql = sql;
            Args = args;
            SqlWithArgs = sqlWithArgs;
        }

        /// <summary>
        /// Sql语句
        /// </summary>
        public string Sql;

        /// <summary>
        /// 参数 
        /// </summary>
        public object[] Args;

        /// <summary>
        /// Sql语句 带 参数
        /// </summary>
        public string SqlWithArgs;
    }

    public delegate void SqlOperationEventHandler(object sender, SqlOperationEventArgs args);
}