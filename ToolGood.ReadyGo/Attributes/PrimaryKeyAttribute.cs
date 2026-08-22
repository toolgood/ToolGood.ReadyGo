using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class PrimaryKeyAttribute : ToolGood.ReadyGo.NPoco.PrimaryKeyAttribute
    {
        /// <summary>
        ///     The column name.
        /// </summary>
        public string PrimaryKey => Value;

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="PrimaryKeyAttribute" />.
        /// </summary>
        /// <param name="primaryKey">The name of the primary key column.</param>
        public PrimaryKeyAttribute(string primaryKey) : base(primaryKey.Trim())
        {
        }

        /// <summary>
        /// 主键
        /// </summary>
        /// <param name="primaryKey"></param>
        public PrimaryKeyAttribute(string[] primaryKey) : base(primaryKey)
        {
        }

        /// <summary>
        /// 主键
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <param name="autoIncrement"></param>
        public PrimaryKeyAttribute(string primaryKey, bool autoIncrement) : base(primaryKey.Trim())
        {
            AutoIncrement = autoIncrement;
        }

        /// <summary>
        /// 主键
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <param name="autoIncrement"></param>
        /// <param name="sequenceName"></param>
        public PrimaryKeyAttribute(string primaryKey, bool autoIncrement, string sequenceName) : base(primaryKey.Trim())
        {
            AutoIncrement = autoIncrement;
            SequenceName = sequenceName.Trim();
        }
    }
}
