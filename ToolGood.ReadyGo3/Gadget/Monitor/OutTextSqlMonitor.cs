using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.Gadget.Monitor
{
    /// <summary>
    /// SQL监控输出文本
    /// </summary>
    public class OutTextSqlMonitor : ISqlMonitor
    {
        private readonly Dictionary<SqlMonitorType, Action<string>> _dict = new Dictionary<SqlMonitorType, Action<string>>();

        private void SetOutAction(SqlMonitorType type, Action<string> action)
        {
            if (type.HasFlag(SqlMonitorType.ConnectionOpened)) {
                _dict[SqlMonitorType.ConnectionOpened] = action;
            } else if (type.HasFlag(SqlMonitorType.ConnectionClosing)) {
                _dict[SqlMonitorType.ConnectionClosing] = action;
            } else if (type.HasFlag(SqlMonitorType.Transactioning)) {
                _dict[SqlMonitorType.Transactioning] = action;
            } else if (type.HasFlag(SqlMonitorType.Transactioned)) {
                _dict[SqlMonitorType.Transactioned] = action;
                //} else if (type.HasFlag(SqlMonitorType.ExecutingCommand)) {
                //    dict[SqlMonitorType.ExecutingCommand] = action;
            } else if (type.HasFlag(SqlMonitorType.ExecutedCommand)) {
                _dict[SqlMonitorType.ExecutedCommand] = action;
            } else if (type.HasFlag(SqlMonitorType.Exception)) {
                _dict[SqlMonitorType.Exception] = action;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ConnectionClosing()
        {
            if (_dict.TryGetValue(SqlMonitorType.ConnectionClosing, out Action<string> action)) {
                var str = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 关闭连接\r\n";
                action(str);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void ConnectionOpened()
        {
            if (_dict.TryGetValue(SqlMonitorType.ConnectionOpened, out Action<string> action)) {
                var str = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 开始连接\r\n";
                action(str);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Exception(string message)
        {
            if (_dict.TryGetValue(SqlMonitorType.Exception, out Action<string> action)) {
                var str = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 错误：{message}\r\n";
                action(str);
            }
        }
        private DateTime _startTime;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        public void ExecutedCommand(string sql, object[] args)
        {
            if (_dict.TryGetValue(SqlMonitorType.ExecutedCommand, out Action<string> action)) {
                var str = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}[{(DateTime.Now - _startTime).TotalMilliseconds}ms] {(DateTime.Now - _startTime).TotalMilliseconds}\r\n";
                action(str);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        public void ExecutingCommand(string sql, object[] args)
        {
            _startTime = DateTime.Now;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToHtml()
        {
            return null;
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
            if (_dict.TryGetValue(SqlMonitorType.Transactioned, out Action<string> action)) {
                var str = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 开启事务\r\n";
                action(str);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Transactioning()
        {
            if (_dict.TryGetValue(SqlMonitorType.Transactioning, out Action<string> action)) {
                var str = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 开启事务\r\n";
                action(str);
            }
        }
    }
}
