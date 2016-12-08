using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 返回
    /// </summary>
    [AttributeUsage(AttributeTargets.Property , AllowMultiple = false, Inherited = true)]
    public class ResultColumnAttribute : ColumnAttribute
    {
        /// <summary>
        ///
        /// </summary>
        public ResultColumnAttribute() { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        public ResultColumnAttribute(string name) : base(name) { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="definition"></param>
        public ResultColumnAttribute(string name, string definition) : base(name)
        {
            Definition = definition;
        }

        /// <summary>
        ///
        /// </summary>
        public string Definition { get; set; }
    }
}