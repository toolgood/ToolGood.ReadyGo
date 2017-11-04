using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.Gadget.Monitor
{
    public class OutTextSqlMonitor : ISqlMonitor
    {
        Dictionary<SqlMonitorType, Action<string>> dict = new Dictionary<SqlMonitorType, Action<string>>();

        public void SetOutAction(SqlMonitorType type, Action<string> action)
        {
            if (type.HasFlag(SqlMonitorType.ConnectionOpened)) {
                dict[SqlMonitorType.ConnectionOpened] = action;
            } else if (type.HasFlag(SqlMonitorType.ConnectionClosing)) {
                dict[SqlMonitorType.ConnectionClosing] = action;
            } else if (type.HasFlag(SqlMonitorType.Transactioning)) {
                dict[SqlMonitorType.Transactioning] = action;
            } else if (type.HasFlag(SqlMonitorType.Transactioned)) {
                dict[SqlMonitorType.Transactioned] = action;
            //} else if (type.HasFlag(SqlMonitorType.ExecutingCommand)) {
            //    dict[SqlMonitorType.ExecutingCommand] = action;
            } else if (type.HasFlag(SqlMonitorType.ExecutedCommand)) {
                dict[SqlMonitorType.ExecutedCommand] = action;
            } else if (type.HasFlag(SqlMonitorType.Exception)) {
                dict[SqlMonitorType.Exception] = action;
            }
        }


        public void ConnectionClosing( )
        {
            Action<string> action;
            if (dict.TryGetValue(SqlMonitorType.ConnectionClosing, out action)) {
                var str = string.Format("{0} 关闭连接\r\n",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                action(str);
            }
        }

        public void ConnectionOpened( )
        {
            Action<string> action;
            if (dict.TryGetValue(SqlMonitorType.ConnectionOpened, out action)) {
                var str = string.Format("{0} 开始连接\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                action(str);
            }
        }

        public void Exception( string message)
        {
            Action<string> action;
            if (dict.TryGetValue(SqlMonitorType.Exception, out action)) {
                var str = string.Format("{0} 错误：{1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),message);
                action(str);
            }
        }
        private DateTime StartTime;

        public void ExecutedCommand( string sql, object[] args)
        {
            Action<string> action;
            if (dict.TryGetValue(SqlMonitorType.ExecutedCommand, out action)) {
                var str = string.Format("{0}[{1}ms] {1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    ,(DateTime.Now- StartTime).TotalMilliseconds, sql
                    );
                action(str);
            }
        }

        public void ExecutingCommand(  string sql, object[] args)
        {
            StartTime = DateTime.Now;
        }

        public string ToHtml()
        {
            return null;
        }

        public string ToText()
        {
            return null;
        }

        public void Transactioned()
        {
            Action<string> action;
            if (dict.TryGetValue(SqlMonitorType.Transactioned, out action)) {
                var str = string.Format("{0} 开启事务\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                action(str);
            }
        }

        public void Transactioning( )
        {
            Action<string> action;
            if (dict.TryGetValue(SqlMonitorType.Transactioning, out action)) {
                var str = string.Format("{0} 开启事务\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                action(str);
            }
        }
    }
}
