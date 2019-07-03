using System;
using ToolGood.ReadyGo3.Attributes;



namespace ToolGood.ReadyGo3.Test.Datas
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
        public virtual int ID { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public virtual int ParentID { get; set; }
        /// <summary>
        /// CODE
        /// </summary>
        public virtual string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public virtual string Icon { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public virtual string Url { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public virtual string Actions { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public virtual int Sort { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public virtual bool IsDelete { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public virtual DateTime AddingTime { get; set; }

    }
}