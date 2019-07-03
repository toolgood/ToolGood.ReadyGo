﻿using System;
using ToolGood.ReadyGo3.Attributes;

namespace ToolGood.ReadyGo3.Test.Datas
{
    [Table("AdminMenuPass")]
    [Index("AdminGroupID")]
    [Index("Code")]
    [Index("ActionName")]
    [Serializable]
    public class DbAdminMenuPass
    {
        /// <summary>
        /// 管理组ID
        /// </summary>
        public virtual int AdminGroupID { get; set; }
        /// <summary>
        /// 菜单ID
        /// </summary>
        public virtual int MenuID { get; set; }
        /// <summary>
        /// CODE
        /// </summary>
        public virtual string Code { get; set; }
        /// <summary>
        /// 操作名
        /// </summary>
        public virtual string ActionName { get; set; }
    }
}