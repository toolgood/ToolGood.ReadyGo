using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.Monitor
{
    public class NullSqlMonitor : ISqlMonitor
    {
        public void ConnectionClosing()
        {
        }

        public void ConnectionOpened()
        {
        }

        public void Exception( string message)
        {
        }

        public void ExecutedCommand( string sql, object[] args)
        {
        }

        public void ExecutingCommand( string sql, object[] args)
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

        public void Transactioned( )
        {
        }

        public void Transactioning( )
        {
        }
    }
}