using System.Runtime;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 定义对象加载完成后的回调接口，用于在对象从数据库加载完成后执行自定义逻辑。
    /// </summary>
    public interface IOnLoaded
    {
        /// <summary>
        /// 对象加载完成后调用。
        /// </summary>
        void OnLoaded();
    }
}
