using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.NPoco.DatabaseTypes
{
    /// <summary>
    /// Oracle 托管驱动（Oracle.ManagedDataAccess）数据库类型实现。
    /// </summary>
    public class OracleManagedDatabaseType : OracleDatabaseType
    {
        /// <summary>
        /// 获取 Oracle 托管驱动提供程序名称。
        /// </summary>
        /// <returns>提供程序名称。</returns>
        public override string GetProviderName()
        {
            return "Oracle.ManagedDataAccess.Client";
        }
    }
}
