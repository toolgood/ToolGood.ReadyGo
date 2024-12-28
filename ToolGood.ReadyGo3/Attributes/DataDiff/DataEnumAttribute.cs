namespace ToolGood.ReadyGo3.Attributes
{
    /// <summary>
    /// 数据名称
    /// </summary>
    public class DataEnumAttribute : DataNameAttribute
    {
        /// <summary>
        /// 枚举名称，从0开始
        /// </summary>
        public string[] EnumName { get; set; }

        /// <summary>
        /// 枚举名称
        /// </summary>
        /// <param name="displayName">显示名称</param>
        /// <param name="enumName">枚举名称，从0开始</param>
        public DataEnumAttribute(string displayName, params string[] enumName) : base(displayName)
        {
            EnumName = enumName;
        }
    }
}