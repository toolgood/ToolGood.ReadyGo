
using ToolGood.ReadyGo3.Gadget.TableManager;

namespace ToolGood.ReadyGo3
{
    partial class SqlHelper
    {
        private SqlTableHelper _tableHelper;

        /// <summary>
        /// 
        /// </summary>
        public SqlTableHelper TableHelper
        {
            get
            {
                if (_tableHelper == null) {
                    _tableHelper = new SqlTableHelper(this);
                }
                return _tableHelper;
            }
        }
    }
}