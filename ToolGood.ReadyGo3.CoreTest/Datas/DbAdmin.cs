﻿using System;
using ToolGood.ReadyGo3.Attributes;


namespace ToolGood.ReadyGo3.Test.Datas
{
    [Table("Admin")]
    [Index("AdminGroupID")]
    [Serializable]
    public class DbAdmin
    {
        /// <summary>
        /// ID
        /// </summary>
        public virtual int ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [Column("nick")]
        public virtual string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public virtual string Password { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public virtual string TrueName { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public virtual string Phone { get; set; }
        /// <summary>
        /// 邮箱 
        /// </summary>
        public virtual string Email { get; set; }
        /// <summary>
        /// 管理员ID
        /// </summary>
        public virtual int AdminGroupID { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public virtual bool IsDelete { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        public virtual DateTime AddingTime { get; set; }
        /// <summary>
        /// 管理员名
        /// </summary>
        [ResultColumn("AdminGroupName", "select Name from AdminGroup where Id={0}.AdminGroupID")]
        public virtual string AdminGroupName { get; set; }
    }
}