using System.Collections.Generic;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示一个已准备好的更新语句及其参数、版本与主键信息。
    /// </summary>
    public class PreparedUpdateStatement
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
        /// 版本列的值。
        /// </summary>
        public object VersionValue { get; set; }
        /// <summary>
        /// 版本列类型。
        /// </summary>
        public VersionColumnType VersionColumnType { get; set; }
        /// <summary>
        /// 生成的更新 SQL 语句。
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 原始参数值列表。
        /// </summary>
        public List<object> Rawvalues { get; set; }
        /// <summary>
        /// 主键名与值的字典。
        /// </summary>
        public Dictionary<string, object> PrimaryKeyValuePairs { get; set; }
    }
}
