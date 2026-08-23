namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示一个 ANSI 字符串值的包装类型，用于在映射到数据库时标记字符串为 ANSI（非 Unicode）编码。
    /// </summary>
    public class AnsiString
    {
        /// <summary>
        /// 使用指定的字符串初始化 <see cref="AnsiString"/> 实例。
        /// </summary>
        /// <param name="str">要包装的字符串值。</param>
        public AnsiString(string str)
        {
            Value = str;
        }

        /// <summary>
        /// 获取包装的字符串值。
        /// </summary>
        public string Value { get; private set; }
    }
}