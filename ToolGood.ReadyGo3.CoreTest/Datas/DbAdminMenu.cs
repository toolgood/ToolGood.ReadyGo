using System;
using ToolGood.ReadyGo3.Attributes;



namespace ToolGood.ReadyGo3.CoreTest.Datas
{
    [Table("AdminMenu ")]
    [Index("ParentID")]
    [Unique("Code")]
    [Serializable]
    public class DbAdminMenu 
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// CODE
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string Actions { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddingTime { get; set; }

    }
}