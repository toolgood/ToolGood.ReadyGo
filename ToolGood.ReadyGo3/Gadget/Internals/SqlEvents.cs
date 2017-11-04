using System.Text;

namespace ToolGood.ReadyGo3.Gadget.Events
{
    public class SqlEvents
    {
        /// <summary>
        /// 事件：插入对象前
        /// </summary>
        public event BeforeInsertEventHandler BeforeInsert;

        /// <summary>
        /// 事件：更新对象前
        /// </summary>
        public event BeforeUpdateEventHandler BeforeUpdate;

        /// <summary>
        /// 事件：删除对象前
        /// </summary>
        public event BeforeDeleteEventHandler BeforeDelete;

        /// <summary>
        /// 事件：插入对象后
        /// </summary>
        public event AfterInsertEventHandler AfterInsert;

        /// <summary>
        /// 事件：更新对象后
        /// </summary>
        public event AfterUpdateEventHandler AfterUpdate;

        /// <summary>
        /// 事件：删除对象后
        /// </summary>
        public event AfterDeleteEventHandler AfterDelete;

        /// <summary>
        /// 事件：执行Command前
        /// </summary>
        public event SqlOperationEventHandler BeforeExecuteCommand;

        /// <summary>
        /// 事件：执行Command后
        /// </summary>
        public event SqlOperationEventHandler AfterExecuteCommand;

        /// <summary>
        /// 事件：出错
        /// </summary>
        public event SqlErrorEventHandler ExecuteException;

        internal bool OnBeforeInsert(object obj)
        {
            if (BeforeInsert != null) {
                DataEventArgs e = new DataEventArgs(obj);
                BeforeInsert(this, e);
                return e.Cancel;
            }
            return false;
        }

        internal bool OnBeforeUpdate(object obj)
        {
            if (BeforeUpdate != null) {
                DataEventArgs e = new DataEventArgs(obj);
                BeforeUpdate(this, e);
                return e.Cancel;
            }
            return false;
        }

        internal bool OnBeforeDelete(object obj)
        {
            if (BeforeDelete != null) {
                DataEventArgs e = new DataEventArgs(obj);
                BeforeDelete(this, e);
                return e.Cancel;
            }
            return false;
        }

        internal void OnAfterInsert(object obj)
        {
            if (AfterInsert != null) {
                Data2EventArgs e = new Data2EventArgs(obj);
                AfterInsert(this, e);
            }
        }

        internal void OnAfterUpdate(object obj)
        {
            if (AfterUpdate != null) {
                Data2EventArgs e = new Data2EventArgs(obj);
                AfterUpdate(this, e);
            }
        }

        internal void OnAfterDelete(object obj)
        {
            if (AfterDelete != null) {
                Data2EventArgs e = new Data2EventArgs(obj);
                AfterDelete(this, e);
            }
        }

        internal void OnExecutingCommand(string sql, object[] args)
        {
            if (BeforeExecuteCommand != null) {
                SqlOperationEventArgs e = new SqlOperationEventArgs(sql, args, FormatCommand(sql, args));
                BeforeExecuteCommand(this, e);
            }
        }

        internal void OnExecutedCommand(string sql, object[] args)
        {
            if (AfterExecuteCommand != null) {
                SqlOperationEventArgs e = new SqlOperationEventArgs(sql, args, FormatCommand(sql, args));
                AfterExecuteCommand(this, e);
            }
        }

        internal bool OnException(string message, string sql, params object[] args)
        {
            if (ExecuteException != null) {
                SqlErrorEventArgs e = new SqlErrorEventArgs(sql, args, FormatCommand(sql, args), message);
                ExecuteException(this, e);
                if (e.Handle) {
                    return false;
                }
            }
            return true;
        }
        internal static string FormatCommand(string sql, object[] args)
        {
            if (string.IsNullOrEmpty(sql)) return "";
            var sb = new StringBuilder();
            sb.Append(sql);
            if (args != null && args.Length > 0) {
                sb.Append("\n");
                for (int i = 0; i < args.Length; i++) {
                    sb.AppendFormat("\t -> ({0})[{1}] = \"{2}\"\n", i, args[i].GetType().Name, args[i]);
                }
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
    }
}