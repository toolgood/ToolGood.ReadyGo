using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class PrimaryKeyAttribute : Attribute
    {
        /// <summary>
        ///     The column name.
        /// </summary>
        public string PrimaryKey { get; private set; }

        /// <summary>
        ///     A flag which specifies if the primary key is auto incrementing.
        /// </summary>
        public bool AutoIncrement { get; set; }

        /// <summary>
        ///     The sequence name.
        /// </summary>
        public string SequenceName { get; set; }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="PrimaryKeyAttribute" />.
        /// </summary>
        /// <param name="primaryKey">The name of the primary key column.</param>
        public PrimaryKeyAttribute(string primaryKey)
        {
            PrimaryKey = primaryKey;
            AutoIncrement = true;
        }
        /// <summary>
        /// 主键
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <param name="autoIncrement"></param>
        public PrimaryKeyAttribute(string primaryKey, bool autoIncrement)
        {
            PrimaryKey = primaryKey;
            AutoIncrement = autoIncrement;
        }
        /// <summary>
        /// 主键
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <param name="autoIncrement"></param>
        /// <param name="sequenceName"></param>
        public PrimaryKeyAttribute(string primaryKey, bool autoIncrement, string sequenceName)
        {
            PrimaryKey = primaryKey;
            AutoIncrement = autoIncrement;
            SequenceName = sequenceName;
        }
    }
}