using ToolGood.ReadyGo.Gadget.TableManager;

namespace ToolGood.ReadyGo
{
    /// <summary>
    /// SqlHelper 表管理扩展
    /// </summary>
    public partial class SqlHelper
    {
        private SqlTableHelper _tableHelper;

        /// <summary>
        /// 表管理助手
        /// </summary>
        public SqlTableHelper _TableHelper {
            get { return _tableHelper ?? (_tableHelper = new SqlTableHelper(this)); }
        }
    }
}
