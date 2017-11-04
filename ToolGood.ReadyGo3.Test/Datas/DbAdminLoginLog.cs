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
        public int ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 状态，是否成功登录
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddingTime { get; set; }


    }
}