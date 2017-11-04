namespace ToolGood.ReadyGo3.Gadget.Monitor
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

        void ConnectionClosing();

        void ConnectionOpened( );

        void Exception(  string message);

        void ExecutedCommand( string sql, object[] args);

        void ExecutingCommand( string sql, object[] args);

        void Transactioned();

        void Transactioning();
    }




}