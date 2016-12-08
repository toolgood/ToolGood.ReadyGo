using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.Monitor
{
    public class NullSqlMonitor : ISqlMonitor
    {
        public void ConnectionClosing(ConnectionType type)
        {
        }

        public void ConnectionOpened(ConnectionType type)
        {
        }

        public void Exception(ConnectionType type, string message)
        {
        }

        public void ExecutedCommand(ConnectionType type, string sql, object[] args)
        {
        }

        public void ExecutingCommand(ConnectionType type, string sql, object[] args)
        {
        }

        public string ToHtml()
        {
            return null;
        }

        public string ToText()
        {
            return null;

        }

        public void Transactioned(ConnectionType type)
        {
        }

        public void Transactioning(ConnectionType type)
        {
        }
    }
}