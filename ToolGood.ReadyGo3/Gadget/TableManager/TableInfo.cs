using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.Attributes;

namespace ToolGood.ReadyGo3.Gadget.TableManager
{
    public class TableInfo
    {
        public TableInfo()
        {
            Indexs = new List<List<string>>();
            Uniques = new List<List<string>>();
            Columns = new List<ColumnInfo>();
        }

        public string SchemaName;
        public string TableName;

        public string PrimaryKey;
        public bool AutoIncrement;
        public string SequenceName;

        public List<List<string>> Indexs;
        public List<List<string>> Uniques;
        public List<ColumnInfo> Columns;

 

        internal static TableInfo FromType(Type t)
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
