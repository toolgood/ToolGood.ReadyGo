using System;

namespace ToolGood.ReadyGo3.Attributes
{
    /// <summary>
    /// 返回
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ResultColumnAttribute : ColumnAttribute
    {
        /// <summary>
        ///
        /// </summary>
        public ResultColumnAttribute()
        {
            Required = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="required"></param>
        public ResultColumnAttribute(bool required)
        {
            Required = required;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        public ResultColumnAttribute(string name) : base(name)
        {
            Required = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="definition"></param>
        public ResultColumnAttribute(string name, string definition) : base(name)
        {
            Required = true;
            if (string.IsNullOrEmpty(definition) == false) {
                definition = definition.Replace("{0}.", "{0}").Trim();
                if (definition.StartsWith("(") == false) {
                    definition = "(" + definition + ")";
                }
                Definition = definition;
            }
        }
        public bool Required;
        /// <summary>
        ///
        /// </summary>
        public string Definition;
    }
}