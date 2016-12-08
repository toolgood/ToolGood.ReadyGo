using ToolGood.ReadyGo.SqlBuilding.TableCreate;

namespace ToolGood.ReadyGo
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