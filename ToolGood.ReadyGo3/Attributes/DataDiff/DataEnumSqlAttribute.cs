namespace ToolGood.ReadyGo3.Attributes
{
    /// <summary>
    /// 数据名称
    /// </summary>
    public class DataEnumSqlAttribute : DataNameAttribute
    {
        /// <summary>
        /// SQL语句，返回两列：id,名称
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 枚举名称
        /// </summary>
        /// <param name="displayName">显示名称</param>
        /// <param name="sql">SQL语句，返回两列：id,名称</param>
        public DataEnumSqlAttribute(string displayName, string sql) : base(displayName)
        {
            Sql = sql;
        }
    }
}