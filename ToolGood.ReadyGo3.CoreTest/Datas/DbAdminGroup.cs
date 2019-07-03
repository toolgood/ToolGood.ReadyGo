using System;
using ToolGood.ReadyGo3.Attributes;

namespace ToolGood.ReadyGo3.Test.Datas
{
    [Table("AdminGroup")]
    [Serializable]
    public class DbAdminGroup
    {
        /// <summary>
        /// ID
        /// </summary>
        public virtual int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Describe { get; set; }
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