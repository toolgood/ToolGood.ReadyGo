using System;
using System.Collections.Generic;
using System.Text;

namespace ToolGood.ReadyGo3.Gadget.Monitor
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlMonitor : ISqlMonitor
    {
        private readonly List<SqlMonitorItem> _items = new List<SqlMonitorItem>();
        private readonly SqlMonitorItem _root = new SqlMonitorItem();
        private SqlMonitorItem _cur;
        /// <summary>
        /// 
        /// </summary>
        public SqlMonitor()
        {
            _cur = _root;
            _cur.Parent = _root;
        }
        /// <summary>
        /// 
        /// </summary>
        public void ConnectionOpened()
        {
            SqlMonitorItem item = new SqlMonitorItem {
                Parent = _cur,
                StartTime = DateTime.Now
            };
            item.Sql += "开始连接...";
            _items.Add(item);
            _cur = item;
        }
        /// <summary>
        /// 
        /// </summary>
        public void ConnectionClosing()
        {
            SqlMonitorItem item = new SqlMonitorItem {
                Parent = _cur,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            };
            item.Sql += "结束连接...";
            _items.Add(item);

            _cur.EndTime = DateTime.Now;
            _cur = _cur.Parent;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Transactioning()
        {
            SqlMonitorItem item = new SqlMonitorItem {
                Parent = _cur,
                StartTime = DateTime.Now
            };
            item.Sql += "开始事务...";
            _items.Add(item);
            _cur = item;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Transactioned()
        {
            SqlMonitorItem item = new SqlMonitorItem {
                Parent = _cur,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            };
            item.Sql += "结束事务...";
            _items.Add(item);

            _cur.EndTime = DateTime.Now;
            _cur = _cur.Parent;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        public void ExecutingCommand(string sql, object[] args)
        {
            SqlMonitorItem item = new SqlMonitorItem {
                Parent = _cur,
                StartTime = DateTime.Now,
                Sql = sql
            };

            _items.Add(item);
            _cur = item;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        public void ExecutedCommand(string sql, object[] args)
        {
            _cur.EndTime = DateTime.Now;
            _cur = _cur.Parent;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Exception(string message)
        {
            _cur.EndTime = DateTime.Now;
            _cur.Exception = message;
            _cur = _cur.Parent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToText()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in _items) {
                if (sb.Length > 0) {
                    sb.Append("\r\n");
                }
                item.ToString(sb);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToText();
        }

        private class SqlMonitorItem
        {
            public SqlMonitorItem Parent;
            public string Sql;
            public string Exception;
            public DateTime StartTime;
            public DateTime EndTime;

            public void ToString(StringBuilder sb)
            {
                var ts = (int)(EndTime - StartTime).TotalMilliseconds;
                if (ts < 0) { ts = 0; }

                sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss}[{1}ms]：", StartTime, ts);
                sb.Append(Sql);
                if (string.IsNullOrEmpty(Exception) == false) {
                    sb.Append("\r\n");
                    sb.Append("\t" + Exception.Replace("\r\n", "\n").Replace("\n", "\r\n\t"));
                    sb.Append("\r\n");
                    sb.Append("\r\n");
                }
            }
        }
    }
}