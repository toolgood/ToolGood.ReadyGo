using System;
using ToolGood.ReadyGo3.Attributes;

namespace ToolGood.ReadyGo3.Test.Datas
{
    [Table("AdminLoginLog")]
    [Serializable]
    public class DbAdminLoginLog
    {
        /// <summary>
        /// ID
        /// </summary>
        public virtual int ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public virtual string Message { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public virtual string Ip { get; set; }
        /// <summary>
        /// 状态，是否成功登录
        /// </summary>
        public virtual bool State { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public virtual DateTime AddingTime { get; set; }


    }
}