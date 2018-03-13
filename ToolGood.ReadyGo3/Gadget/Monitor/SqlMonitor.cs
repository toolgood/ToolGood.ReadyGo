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
                Layer = _cur.Layer + 1,
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
                Layer = _cur.Layer + 1,
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
                Layer = _cur.Layer + 1,
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
                Layer = _cur.Layer + 1,
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
                Layer = _cur.Layer + 1,
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
        public string ToHtml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<pre>");
#if NETSTANDARD2_0
            System.Net.WebUtility.HtmlEncode(ToText());
#else 
            sb.Append(System.Web.HttpUtility.HtmlEncode(ToText()));
#endif
            sb.Append("</pre>");
            return sb.ToString();
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

        private class SqlMonitorItem
        {
            public SqlMonitorItem Parent;
            public int Layer;
            public string Sql;
            public string Exception;
            public DateTime StartTime;
            public DateTime EndTime;

            public void ToString(StringBuilder sb)
            {
                sb.Append(new string(' ', Layer * 2));

                sb.AppendFormat("{0:HH:mm:ss}[{1}ms]：", StartTime, (int)(EndTime - StartTime).TotalMilliseconds);
                sb.Append(Sql);
                if (string.IsNullOrEmpty(Exception) == false) {
                    sb.Append("\r\n");
                    sb.Append(new string(' ', Layer * 2 + 2));
                    sb.Append(Exception);
                }
            }
        }
    }
}