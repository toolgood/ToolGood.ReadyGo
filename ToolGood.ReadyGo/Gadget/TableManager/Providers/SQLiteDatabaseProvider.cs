using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget.TableManager.Providers
{
    public class SQLiteDatabaseProvider : DatabaseProvider
    {
        public override string GetTryCreateTable(Type type, bool withIndex = true)
        {
            var ti = TableInfo.FromType(type);
            var sql = "CREATE TABLE IF NOT EXISTS [" + ti.TableName + "](\r\n";
            foreach (var item in ti.Columns) {
                sql += "    " + CreateColumn(ti, item) + ",\r\n";
            }
            sql = sql.Substring(0, sql.Length - 3);
            sql += "\r\n);\r\n";
            if (withIndex) {
                foreach (var item in ti.Indexs) {
                    var txt = "i_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                    var columns = BuildColumns(item);
                    sql += "CREATE INDEX IF NOT EXISTS " + txt + " ON [" + ti.TableName + "](" + columns + ");\r\n";
                }

                foreach (var item in ti.Uniques) {
                    var txt = "u_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                    var columns = BuildColumns(item);
                    sql += "CREATE UNIQUE INDEX IF NOT EXISTS " + txt + " ON [" + ti.TableName + "]( " + columns + ");\r\n";
                }
            }
            sql = sql.Substring(0, sql.Length - 2);
            return sql;
        }

        public override string GetCreateIndex(Type type)
        {
            //CREATE [UNIQUE|FULLTEXT|SPATIAL] INDEX 索引名 ON 表名（字段名[(长度)][ASC | DESC]）;
            string sql = "";
            var ti = TableInfo.FromType(type);
            foreach (var item in ti.Indexs) {
                var txt = "i_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                var columns = BuildColumns(item);
                sql += $"CREATE INDEX {txt} ON {GetTableName(ti)}({columns});\r\n";
            }
            foreach (var item in ti.Uniques) {
                var txt = "u_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                var columns = BuildColumns(item);
                sql += $"CREATE UNIQUE INDEX {txt} ON {GetTableName(ti)}({columns});\r\n";
            }
            return sql;
        }

        private string BuildColumns(List<string> columnList)
        {
            var sb = new StringBuilder();
            foreach (var col in columnList) {
                sb.Append($"[{col}],");
            }
            return sb.ToString().Replace("[[", "[").Replace("]]", "]").Trim(',');
        }

        public override string GetDropTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return "DROP TABLE IF EXISTS [" + ti.TableName + "];";
        }

        public override string GetDropTable(string tableName)
        {
            return "DROP TABLE IF EXISTS [" + tableName + "];";
        }

        public override string GetTruncateTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return GetTruncateTable(ti.TableName);
        }

        public override string GetTruncateTable(string tableName)
        {
            return $@"DELETE FROM {tableName};
DELETE FROM sqlite_sequence WHERE name='{tableName}';";
        }

        private string CreateColumn(TableInfo ti, ColumnInfo ci)
        {
            var type = ci.PropertyType;
            var isRequired = ci.Required;
            if (type.IsEnum) return CreateField(ti, ci, "int", ci.FieldLength, true);
            if (type == typeof(string)) return CreateField(ti, ci, "Text", "", false);
            if (type == typeof(Byte[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(SByte[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(UInt16[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(UInt32[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(UInt64[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(Int16[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(Int32[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(Int64[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(Single[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(double[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(bool[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);

            if (type == typeof(List<Byte>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<SByte>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<UInt16>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<UInt32>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<UInt64>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<Int16>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<Int32>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<Int64>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<Single>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<double>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<bool>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);

            if (type == typeof(AnsiString)) return CreateField(ti, ci, "Text", ci.FieldLength, isRequired);

            if (type == typeof(bool)) return CreateField(ti, ci, "BOOLEAN", ci.FieldLength, isRequired);
            if (type == typeof(byte)) return CreateField(ti, ci, "int", "1", isRequired);
            if (type == typeof(char)) return CreateField(ti, ci, "char", "1", isRequired);

            if (type == typeof(UInt16)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(UInt32)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(UInt64)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(Int16)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(Int32)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(Int64)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(Single)) return CreateField(ti, ci, "FLOAT", ci.FieldLength, isRequired);
            if (type == typeof(double)) return CreateField(ti, ci, "REAL", ci.FieldLength, isRequired);
            if (type == typeof(decimal)) return CreateField(ti, ci, "REAL", ci.FieldLength, isRequired);
            if (type == typeof(DateTime)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);
            if (type == typeof(TimeSpan)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);
            if (type == typeof(DateTimeOffset)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);

            if (type == typeof(Guid)) return CreateField(ti, ci, "Text", "40", isRequired);

            throw new Exception($"Unsupported column type: {ci.PropertyType.Name}");
        }

        private string CreateField(TableInfo ti, ColumnInfo ci, string fieldType, string length, bool isRequired)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[" + ci.ColumnName + "]");
            sb.AppendFormat(" {0}", fieldType);
            if (string.IsNullOrEmpty(length) == false) {
                sb.AppendFormat("({0})", length);
            }
            if (isRequired) {
                sb.Append(" NOT");
            }
            sb.Append(" NULL");

            if (string.IsNullOrEmpty(ci.DefaultValue) == false) {
                sb.AppendFormat(" DEFAULT({0})", ci.DefaultValue);
            }
            if (ti.PrimaryKey == ci.ColumnName) {
                sb.Append(" PRIMARY KEY");
                if (ti.AutoIncrement) {
                    sb.Append(" AutoIncrement");
                }
            }
            return sb.ToString();
        }
    }
}
