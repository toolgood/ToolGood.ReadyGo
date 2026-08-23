using System;
using System.Collections.Generic;
using System.Linq;
using ToolGood.ReadyGo.Attributes;

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
        /// 数据库名
        /// </summary>
        public string DatabaseName;
        /// <summary>
        /// 模式名（Schema）
        /// </summary>
        public string SchemaName;
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName;
        /// <summary>
        /// 配置名称
        /// </summary>
        public string SettingName;

        /// <summary>
        /// 主键列名
        /// </summary>
        public string PrimaryKey;
        /// <summary>
        /// 主键是否自增
        /// </summary>
        public bool AutoIncrement;
        /// <summary>
        /// 序列名称
        /// </summary>
        public string SequenceName;

        /// <summary>
        /// 索引列集合
        /// </summary>
        public List<List<string>> Indexs = new List<List<string>>();
        /// <summary>
        /// 唯一约束列集合
        /// </summary>
        public List<List<string>> Uniques = new List<List<string>>();
        /// <summary>
        /// 列信息集合
        /// </summary>
        public List<ColumnInfo> Columns = new List<ColumnInfo>();

        /// <summary>
        /// 从类型解析出表结构信息
        /// </summary>
        /// <param name="t">要解析的类型</param>
        /// <returns>解析出的表结构信息</returns>
        public static TableInfo FromType(Type t)
        {
            TableInfo ti = new TableInfo();
            var a = t.GetCustomAttributes(typeof(TableAttribute), true);
            if (a.Length > 0) {
                var ta = (a[0] as TableAttribute);
                ti.DatabaseName = ta.DatabaseName;
                ti.SchemaName = ta.SchemaName;
                ti.TableName = ta.TableName;
                ti.SettingName = ta.SettingName;
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
            ti.PrimaryKey = a.Length == 0 ? null : (a[0] as PrimaryKeyAttribute).PrimaryKey;
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
                    ti.AutoIncrement = prop.PropertyType.IsValueType;
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
    }
}
