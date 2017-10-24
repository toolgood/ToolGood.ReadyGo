using System;
using System.Collections.Generic;
using System.Text;

namespace ToolGood.ReadyGo.Monitor
{
    public class SqlMonitor : ISqlMonitor
    {
        private List<SqlMonitorItem> Items = new List<SqlMonitorItem>();
        private SqlMonitorItem root = new SqlMonitorItem();
        private SqlMonitorItem cur;

        public SqlMonitor()
        {
            cur = root;
            cur.Parent = root;
        }

        public void ConnectionOpened()
        {
            SqlMonitorItem item = new SqlMonitorItem();
            item.Layer = cur.Layer + 1;
            item.Parent = cur;
            item.StartTime = DateTime.Now;
            item.Sql += "开始连接...";
            Items.Add(item);
            cur = item;
        }

        public void ConnectionClosing()
        {
            SqlMonitorItem item = new SqlMonitorItem();
            item.Layer = cur.Layer + 1;
            item.Parent = cur;
            item.StartTime = DateTime.Now;
            item.EndTime = DateTime.Now;
            item.Sql += "结束连接...";
            Items.Add(item);

            cur.EndTime = DateTime.Now;
            cur = cur.Parent;
        }

        public void Transactioning( )
        {
            SqlMonitorItem item = new SqlMonitorItem();
            item.Layer = cur.Layer + 1;
            item.Parent = cur;
            item.StartTime = DateTime.Now;
            item.Sql += "开始事务...";
            Items.Add(item);
            cur = item;
        }

        public void Transactioned( )
        {
            SqlMonitorItem item = new SqlMonitorItem();
            item.Layer = cur.Layer + 1;
            item.Parent = cur;
            item.StartTime = DateTime.Now;
            item.EndTime = DateTime.Now;
            item.Sql += "结束事务...";
            Items.Add(item);

            cur.EndTime = DateTime.Now;
            cur = cur.Parent;
        }

        public void ExecutingCommand(  string sql, object[] args)
        {
            SqlMonitorItem item = new SqlMonitorItem();
            item.Layer = cur.Layer + 1;
            item.Parent = cur;
            item.StartTime = DateTime.Now;
            item.Sql = sql;

            Items.Add(item);
            cur = item;
        }

        public void ExecutedCommand(  string sql, object[] args)
        {
            cur.EndTime = DateTime.Now;
            cur = cur.Parent;
        }

        public void Exception( string message)
        {
            cur.EndTime = DateTime.Now;
            cur.Exception = message;
            cur = cur.Parent;
        }

        public string ToHtml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<pre>");
            sb.Append(System.Web.HttpUtility.HtmlEncode(ToText()));
            sb.Append("</pre>");
            return sb.ToString();
        }

        public string ToText()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Items) {
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

                sb.AppendFormat("{0}[{1}ms]：", StartTime.ToString("HH:mm:ss"), (int)(EndTime - StartTime).TotalMilliseconds);
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