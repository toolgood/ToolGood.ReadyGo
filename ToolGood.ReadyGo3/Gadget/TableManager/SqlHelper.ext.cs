using ToolGood.ReadyGo3.Gadget.TableManager;

namespace ToolGood.ReadyGo3
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