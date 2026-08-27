using System;
using System.Collections.Generic;
using System.Linq;
using ToolGood.ReadyGo.Attributes;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget.TableManager
{
    /// <summary>
    ///  解析类型，数据库生成器版
    /// </summary>
    public class TableInfo
    {
        internal TableInfo()
        { }

        /// <summary>
        /// 模式名（Schema）
        /// </summary>
        public string SchemaName { get; internal set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; internal set; }

        /// <summary>
        /// 主键列名
        /// </summary>
        public string PrimaryKey { get; internal set; }
        /// <summary>
        /// 主键是否自增
        /// </summary>
        public bool AutoIncrement { get; internal set; }
        /// <summary>
        /// 序列名称
        /// </summary>
        public string SequenceName { get; internal set; }

        /// <summary>
        /// 索引列集合
        /// </summary>
        public List<List<string>> Indexs { get; internal set; } = new List<List<string>>();
        /// <summary>
        /// 唯一约束列集合
        /// </summary>
        public List<List<string>> Uniques { get; internal set; } = new List<List<string>>();
        /// <summary>
        /// 列信息集合
        /// </summary>
        public List<ColumnInfo> Columns { get; internal set; } = new List<ColumnInfo>();

        private static readonly Cache<Type, TableInfo> _tableInfoCache = Cache<Type, TableInfo>.CreateStaticCache();

        /// <summary>
        /// 从类型解析出表结构信息（结果会被缓存，返回实例仅应被只读使用）
        /// </summary>
        /// <param name="t">要解析的类型</param>
        /// <returns>解析出的表结构信息</returns>
        public static TableInfo FromType(Type t)
        {
            return _tableInfoCache.Get(t, () => BuildTableInfo(t));
        }

        private static TableInfo BuildTableInfo(Type t)
        {
            TableInfo ti = new TableInfo();
            var a = t.GetCustomAttributes(typeof(TableAttribute), true);
            if (a.Length > 0) {
                var ta = (a[0] as TableAttribute);
                ti.SchemaName = ta.SchemaName;
                ti.TableName = ta.TableName;
            } else {
                ti.TableName = t.Name;
            }

            foreach (var item in t.GetProperties()) {
                var col = ColumnInfo.FromProperty(item);
                if (col != null) {
                    ti.Columns.Add(col);
                }
            }

            a = t.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
            ti.PrimaryKey = a.Length == 0 ? null : (a[0] as PrimaryKeyAttribute).Value;
            ti.AutoIncrement = a.Length == 0 ? false : (a[0] as PrimaryKeyAttribute).AutoIncrement;
            ti.SequenceName = a.Length == 0 ? null : (a[0] as PrimaryKeyAttribute).SequenceName;

            if (string.IsNullOrEmpty(ti.PrimaryKey)) {
                var prop = t.GetProperties().FirstOrDefault(p => {
                    if (p.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (p.Name.Equals(t.Name + "id", StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (p.Name.Equals(t.Name + "_id", StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (p.Name.Equals(ti.TableName + "id", StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (p.Name.Equals(ti.TableName + "_id", StringComparison.OrdinalIgnoreCase))
                        return true;
                    return false;
                });

                if (prop != null) {
                    ti.PrimaryKey = prop.Name;
                    // 仅整数类型才能推断为自增主键，Guid/DateTime/decimal 等值类型不能自增
                    ti.AutoIncrement = IsIntegerType(prop.PropertyType);
                }
            }

            a = t.GetCustomAttributes(typeof(IndexAttribute), true);
            foreach (IndexAttribute item in a) {
                ti.Indexs.Add(item.ColumnNames);
            }

            a = t.GetCustomAttributes(typeof(UniqueAttribute), true);
            foreach (UniqueAttribute item in a) {
                ti.Uniques.Add(item.ColumnNames);
            }

            return ti;
        }

        /// <summary>
        /// 判断类型是否为可自增的整数类型（支持 Nullable 包装）
        /// </summary>
        private static bool IsIntegerType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            return type == typeof(short) || type == typeof(ushort)
                || type == typeof(int) || type == typeof(uint)
                || type == typeof(long) || type == typeof(ulong);
        }
    }
}
