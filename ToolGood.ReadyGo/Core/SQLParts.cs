namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示拆分后的 SQL 语句各个组成部分。
    /// </summary>
    public struct SQLParts
    {
        /// <summary>
        /// 完整 SQL 语句。
        /// </summary>
        public string sql;
        /// <summary>
        /// 用于计数的 SQL 语句。
        /// </summary>
        public string sqlCount;
        /// <summary>
        /// 去掉 SELECT 部分后的 SQL 语句。
        /// </summary>
        public string sqlSelectRemoved;
        /// <summary>
        /// ORDER BY 子句。
        /// </summary>
        public string sqlOrderBy;
        /// <summary>
        /// 去掉 ORDER BY 后的 SQL 语句。
        /// </summary>
        public string sqlUnordered;
        /// <summary>
        /// 查询的列部分。
        /// </summary>
        public string sqlColumns;
    }
}
