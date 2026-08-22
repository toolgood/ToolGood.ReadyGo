using ToolGood.ReadyGo.Gadget.TableManager;

namespace ToolGood.ReadyGo
{
    /// <summary>
    ///
    /// </summary>
    public partial class SqlHelper
    {
        private SqlTableHelper _tableHelper;

        /// <summary>
        ///
        /// </summary>
        public SqlTableHelper _TableHelper {
            get { return _tableHelper ?? (_tableHelper = new SqlTableHelper(this)); }
        }
    }
}
