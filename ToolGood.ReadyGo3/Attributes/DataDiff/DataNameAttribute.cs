using System;

namespace ToolGood.ReadyGo3.Attributes
{
    /// <summary>
    /// 数据名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DataNameAttribute : Attribute
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// 数据名称
        /// </summary>
        /// <param name="displayName">显示名称</param>
        public DataNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}