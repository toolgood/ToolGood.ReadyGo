#nullable enable
namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 打开数据库连接时的配置选项。
    /// </summary>
    public class OpenConnectionOptions
    {
        /// <summary>
        /// 获取或设置是否延迟打开连接。
        /// </summary>
        public bool Lazy { get; set; }
    }
}
