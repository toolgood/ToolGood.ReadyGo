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
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
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