namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示成员之间的引用关系类型。
    /// </summary>
    public enum ReferenceType
    {
        /// <summary>
        /// 无引用关系。
        /// </summary>
        None,
        /// <summary>
        /// 一对一引用。
        /// </summary>
        OneToOne,
        /// <summary>
        /// 外键引用。
        /// </summary>
        Foreign,
        /// <summary>
        /// 一对多引用。
        /// </summary>
        Many
    }
}
