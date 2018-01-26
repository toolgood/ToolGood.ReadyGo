using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.Gadget.Monitor
{
    /// <summary>
    /// 监控类型
    /// </summary>
    [Flags]
    public enum SqlMonitorType
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// 开始连接
        /// </summary>
        ConnectionOpened = 1,
        /// <summary>
        /// 关闭转接时
        /// </summary>
        ConnectionClosing = 2,
        /// <summary>
        /// 开始事务
        /// </summary>
        Transactioning = 4,
        /// <summary>
        /// 结束事务
        /// </summary>
        Transactioned = 8,
        ///// <summary>
        ///// 执行命令
        ///// </summary>
        //ExecutingCommand = 16,
        /// <summary>
        /// 执行命令结束
        /// </summary>
        ExecutedCommand = 16,
        /// <summary>
        /// 异常
        /// </summary>
        Exception = 32,
        /// <summary>
        /// 全部
        /// </summary>
        All = ConnectionOpened | ConnectionClosing | Transactioning | Transactioned |  ExecutedCommand | Exception,
    }
}
