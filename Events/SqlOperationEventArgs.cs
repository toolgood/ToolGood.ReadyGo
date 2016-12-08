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
        /// <summary>
        /// Sql语句
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 参数 
        /// </summary>
        public object[] Args { get; set; }
        /// <summary>
        /// Sql语句 带 参数
        /// </summary>
        public string SqlWithArgs { get; set; }
    }

    public delegate void SqlOperationEventHandler(object sender, SqlOperationEventArgs args);
}