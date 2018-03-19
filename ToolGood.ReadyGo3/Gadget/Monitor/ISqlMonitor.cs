namespace ToolGood.ReadyGo3.Gadget.Monitor
{
    /// <summary>
    /// SQL 监控
    /// </summary>
    public interface ISqlMonitor
    {
        ///// <summary>
        ///// 输出 为HTML格式
        ///// </summary>
        ///// <returns></returns>
        //string ToHtml();
        /// <summary>
        /// 输出 为文本格式
        /// </summary>
        /// <returns></returns>
        string ToText();
        /// <summary>
        /// 链接关闭
        /// </summary>
        void ConnectionClosing();
        /// <summary>
        /// 链接开启
        /// </summary>
        void ConnectionOpened();
        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="message"></param>
        void Exception(string message);
        /// <summary>
        /// 执行完Sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        void ExecutedCommand(string sql, object[] args);
        /// <summary>
        /// 开始执行Sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        void ExecutingCommand(string sql, object[] args);
        /// <summary>
        /// 事务结束
        /// </summary>
        void Transactioned();
        /// <summary>
        /// 开始事务
        /// </summary>
        void Transactioning();
    }




}