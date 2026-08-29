using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 主键标签：用于指定数据表的主键列。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute : Attribute
    {
        /// <summary>
        /// 主键标签（单列默认自增，多列默认不自增）
        /// </summary>
        /// <param name="primaryKey">主键列名（多列用逗号分隔）</param>
        public PrimaryKeyAttribute(string primaryKey)
        {
            if (primaryKey == null) {
                throw new ArgumentNullException(nameof(primaryKey));
            }
            Value = primaryKey;
            _autoIncrement = !primaryKey.Contains(",");
        }

        /// <summary>
        /// 主键标签（多列默认不自增）
        /// </summary>
        /// <param name="primaryKey">主键列名数组</param>
        public PrimaryKeyAttribute(string[] primaryKey) : this(JoinPrimaryKey(primaryKey))
        {
        }

        private static string JoinPrimaryKey(string[] primaryKey)
        {
            if (primaryKey == null) {
                throw new ArgumentNullException(nameof(primaryKey));
            }
            if (primaryKey.Length == 0) {
                throw new ArgumentException("主键列名数组不能为空", nameof(primaryKey));
            }
            foreach (var item in primaryKey) {
                if (item == null) {
                    throw new ArgumentNullException(nameof(primaryKey));
                }
            }
            return string.Join(",", primaryKey);
        }

        /// <summary>
        /// 主键列名字符串（多列用逗号分隔）
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// 序列名称
        /// </summary>
        public string SequenceName { get; set; }
        private bool _autoIncrement;

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool AutoIncrement
        {
            get { return _autoIncrement; }
            set
            {
                _autoIncrement = value;
                if (value && Value.Contains(","))
                {
                    throw new InvalidOperationException("Cannot set AutoIncrement to true when the primary key is a Composite Key");
                }
            }
        }

        /// <summary>
        /// 是否使用 OUTPUT 子句返回自增值
        /// </summary>
        public bool UseOutputClause { get; set; }
    }
}