using System;
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
        public int ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [Column("nick")]
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string TrueName { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 邮箱 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 管理员ID
        /// </summary>
        public int AdminGroupID { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        public DateTime AddingTime { get; set; }
        /// <summary>
        /// 管理员名
        /// </summary>
        [ResultColumn("AdminGroupName", "select Name from AdminGroup where Id={0}.AdminGroupID")]
        public string AdminGroupName { get; set; }
    }
}