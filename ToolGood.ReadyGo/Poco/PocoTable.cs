namespace ToolGood.ReadyGo.Poco
{
    /// <summary>
    /// 表信息
    /// </summary>
    public class PocoTable
    {
        /// <summary>
        ///     模式名;
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        ///     表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///     主键
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        ///    Oracle sequence
        /// </summary>
        public string SequenceName { get; set; }

        /// <summary>
        ///     自增长
        /// </summary>
        public bool AutoIncrement { get; set; }

        /// <summary>
        ///     表名修改标签
        ///     可通过此标签添加前缀后缀
        /// </summary>
        public string FixTag { get; set; }
    }
}