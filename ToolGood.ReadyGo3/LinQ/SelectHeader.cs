using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ToolGood.ReadyGo3.LinQ.Expressions;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.LinQ
{
    /// <summary>
    /// Select内的表头
    /// </summary>
    public class SelectHeader
    {
        /// <summary>
        /// AS 信息
        /// </summary>
        public string AsName;
 
        /// <summary>
        /// 查询SQL
        /// </summary>
        public string QuerySql;
        /// <summary>
        /// 表
        /// </summary>
        public string Table;

        public bool UseAsName;
    }

 
 
}
