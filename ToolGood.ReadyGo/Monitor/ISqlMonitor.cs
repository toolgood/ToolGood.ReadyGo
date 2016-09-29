namespace ToolGood.ReadyGo.Monitor
{
    public interface ISqlMonitor
    {
        /// <summary>
        /// 输出 为HTML格式
        /// </summary>
        /// <returns></returns>
        string ToHtml();
        /// <summary>
        /// 输出 为文本格式
        /// </summary>
        /// <returns></returns>
        string ToText();

        void ConnectionClosing(ConnectionType type);

        void ConnectionOpened(ConnectionType type);

        void Exception(ConnectionType type, string message);

        void ExecutedCommand(ConnectionType type, string sql, object[] args);

        void ExecutingCommand(ConnectionType type, string sql, object[] args);

        void Transactioned(ConnectionType type);

        void Transactioning(ConnectionType type);
    }
}