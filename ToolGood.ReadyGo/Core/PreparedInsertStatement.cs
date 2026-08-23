using System.Collections.Generic;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示一个已准备好的插入语句及其参数与版本信息。
    /// </summary>
    public class PreparedInsertStatement
    {
        /// <summary>
        /// 关联的 POCO 数据。
        /// </summary>
        public PocoData PocoData { get; set; }
        /// <summary>
        /// 版本列名称（若有）。
        /// </summary>
        public string VersionName { get; set; }
        /// <summary>
        /// 生成的插入 SQL 语句。
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 原始参数值列表。
        /// </summary>
        public List<object> Rawvalues { get; set; }
    }
}
