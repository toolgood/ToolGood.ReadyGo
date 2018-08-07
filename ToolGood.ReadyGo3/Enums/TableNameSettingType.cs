

namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// 表名配置 类型
    /// </summary>
    public enum TableNameSettingType
    {
        /// <summary>
        /// 数据库 默认字符串
        /// </summary>
        DatabaseNameNullText,

        /// <summary>
        /// 数据库 前缀 不包括默认字符串
        /// </summary>
        DatabaseNamePrefixText,

        /// <summary>
        /// 数据库 后缀 不包括默认字符串
        /// </summary>
        DatabaseNameSuffixText,

        /// <summary>
        /// Schema 默认字符串
        /// </summary>
        SchemaNameNullText,

        /// <summary>
        /// Schema 前缀 不包括默认字符串
        /// </summary>
        SchemaNamePrefixText,

        /// <summary>
        /// Schema 后缀 不包括默认字符串
        /// </summary>
        SchemaNameSuffixText,

        /// <summary>
        /// 表前缀
        /// </summary>
        TableNamePrefixText,

        /// <summary>
        /// 表后缀
        /// </summary>
        TableNameSuffixText,
    }
}
