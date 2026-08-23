namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 提供默认的列序列化器等工厂级静态配置。
    /// </summary>
    public class DatabaseFactory
    {
        /// <summary>
        /// 全局默认的列序列化器，用于序列化/反序列化 POCO 中被标记为序列化的列。
        /// </summary>
        public static IColumnSerializer ColumnSerializer = new FastJsonColumnSerializer();
    }
}
