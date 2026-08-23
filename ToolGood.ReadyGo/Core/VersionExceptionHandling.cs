namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 版本列冲突时的处理方式。
    /// </summary>
    public enum VersionExceptionHandling
    {
        /// <summary>
        /// 忽略版本冲突。
        /// </summary>
        Ignore,
        /// <summary>
        /// 版本冲突时抛出异常。
        /// </summary>
        Exception
    }
}
