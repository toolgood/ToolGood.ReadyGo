using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 描述 POCO 成员映射到数据库列时的元数据信息。
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// 获取或设置列名。
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 获取或设置列的别名。
        /// </summary>
        public string ColumnAlias { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示该列是否为结果列（只读、不参与插入/更新）。
        /// </summary>
        public bool ResultColumn { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示该列是否为计算列。
        /// </summary>
        public bool ComputedColumn { get; set; }

        /// <summary>
        /// 获取或设置计算列的判定时机。
        /// </summary>
        public ComputedColumnType ComputedColumnType { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示该列是否被忽略映射。
        /// </summary>
        public bool IgnoreColumn { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示该列是否为版本列。
        /// </summary>
        public bool VersionColumn { get; set; }

        /// <summary>
        /// 获取或设置版本列的类型。
        /// </summary>
        public VersionColumnType VersionColumnType { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示是否将 DateTime 值强制转换为 UTC。
        /// </summary>
        public bool ForceToUtc { get; set; } = true;

        /// <summary>
        /// 获取或设置该列的序列化器。
        /// </summary>
        public IColumnSerializer ColumnSerializer { get; set; }

        /// <summary>
        /// 获取或设置该列在数据库中的类型。
        /// </summary>
        public Type ColumnType { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示该列是否为复杂映射（将对象属性展开为多个列）。
        /// </summary>
        public bool ComplexMapping { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示该列是否为值对象列。
        /// </summary>
        public bool ValueObjectColumn { get; set; }

        /// <summary>
        /// 获取或设置复杂映射的列名前缀。
        /// </summary>
        public string ComplexPrefix { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示该列是否为序列化列。
        /// </summary>
        public bool SerializedColumn { get; set; }

        /// <summary>
        /// 获取或设置该列的引用类型。
        /// </summary>
        public ReferenceType ReferenceType { get; set; }

        /// <summary>
        /// 获取或设置引用的成员名称。
        /// </summary>
        public string ReferenceMemberName { get; set; }

        /// <summary>
        /// 获取该列对应的成员元数据信息。
        /// </summary>
        public MemberInfo MemberInfo { get; internal set; }

        /// <summary>
        /// 获取或设置一个值，指示是否进行精确列名匹配。
        /// </summary>
        public bool ExactColumnNameMatch { get; set; }
    }
}
