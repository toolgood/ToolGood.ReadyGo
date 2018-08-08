namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// Like语句类型
    /// </summary>
    public enum LikeType
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,
        /// <summary>
        /// 文本最后添加 %
        /// </summary>
        StartWith,
        /// <summary>
        /// 文本最前添加 %
        /// </summary>
        EndWith,
        /// <summary>
        /// 文本前后添加 %
        /// </summary>
        Contains
    }
}
