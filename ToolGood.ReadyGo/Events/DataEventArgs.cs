namespace ToolGood.ReadyGo.Events
{
    public class DataEventArgs : System.EventArgs
    {
        public DataEventArgs(object obj)
        {
            Obj = obj;
            Cancel = false;
        }
        /// <summary>
        /// 是否取消操作
        /// </summary>
        public bool Cancel { get; set; }
        /// <summary>
        /// 操作对象
        /// </summary>
        public object Obj { get; set; }
    }

    public delegate void BeforeInsertEventHandler(object sender, DataEventArgs args);

    public delegate void BeforeUpdateEventHandler(object sender, DataEventArgs args);

    public delegate void BeforeDeleteEventHandler(object sender, DataEventArgs args);

    public class Data2EventArgs : System.EventArgs
    {
        public Data2EventArgs(object obj)
        {
            Obj = obj;
        }
        /// <summary>
        /// 操作对象
        /// </summary>
        public object Obj { get; set; }
    }

    public delegate void AfterInsertEventHandler(object sender, Data2EventArgs args);

    public delegate void AfterUpdateEventHandler(object sender, Data2EventArgs args);

    public delegate void AfterDeleteEventHandler(object sender, Data2EventArgs args);
}