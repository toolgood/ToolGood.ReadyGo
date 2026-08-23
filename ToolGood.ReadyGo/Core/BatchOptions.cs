namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 定义批量操作（批量插入、批量更新）的选项。
    /// </summary>
    public class BatchOptions
    {
        /// <summary>
        /// 初始化 <see cref="BatchOptions"/> 实例，默认每批 20 条，语句分隔符为分号。
        /// </summary>
        public BatchOptions()
        {
            BatchSize = 20;
            StatementSeperator = ";";
        }

        /// <summary>
        /// 获取或设置每批处理的记录条数。
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// 获取或设置批量 SQL 语句之间的分隔符。
        /// </summary>
        public string StatementSeperator { get; set; }
    }
}