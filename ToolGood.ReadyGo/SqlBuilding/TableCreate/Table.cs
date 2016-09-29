using System;
using System.Collections.Generic;
using System.Linq;
using ToolGood.ReadyGo.Attributes;

namespace ToolGood.ReadyGo.SqlBuilding.TableCreate
{
    public class Table
    {
        public Table()
        {
            Indexs = new List<List<string>>();
            Uniques = new List<List<string>>();
            Columns = new List<Column>();
        }

        public string SchemaName { get; set; }
        public string TableName { get; set; }

        public string PrimaryKey { get; set; }
        public bool AutoIncrement { get; set; }
        public string SequenceName { get; set; }

        public List<List<string>> Indexs { get; set; }
        public List<List<string>> Uniques { get; set; }
        public List<Column> Columns { get; set; }

        public bool UsedPrefix { get; private set; }
        public bool UsedSuffix { get; private set; }
        public string FixTag { get; private set; }

        internal static Table FromType(Type t)
        {
            Table ti = new Table();
            var a = t.GetCustomAttributes(typeof(TableAttribute), true);
            if (a.Length > 0) {
                var ta = (a[0] as TableAttribute);
                ti.SchemaName = ta.SchemaName;
                ti.TableName = ta.TableName;
                ti.FixTag = ta.FixTag;
            } else {
                ti.TableName = t.Name;
            }

            foreach (var item in t.GetProperties()) {
                var col = Column.FromProperty(item);
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