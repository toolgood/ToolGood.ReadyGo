namespace ToolGood.ReadyGo3.Gadget.Monitor
{
    /// <summary>
    /// 空监视
    /// </summary>
    public class NullSqlMonitor : ISqlMonitor
    {
        /// <summary>
        /// 
        /// </summary>
        public void ConnectionClosing()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public void ConnectionOpened()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Exception(string message)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        public void ExecutedCommand(string sql, object[] args)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        public void ExecutingCommand(string sql, object[] args)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToText()
        {
            return null;

        }
        /// <summary>
        /// 
        /// </summary>
        public void Transactioned()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public void Transactioning()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToText();
        }
    }
}