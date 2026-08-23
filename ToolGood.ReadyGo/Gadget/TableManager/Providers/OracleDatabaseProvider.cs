using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget.TableManager.Providers
{
    public class OracleDatabaseProvider : DatabaseProvider
    {
        public override string GetTryCreateTable(Type type, bool withIndex = true)
        {
            var ti = TableInfo.FromType(type);
            var sql = "CREATE TABLE " + GetTableName(ti) + "(\r\n";
            foreach (var item in ti.Columns) {
                sql += "    " + CreateColumn(ti, item) + ",\r\n";
            }
            sql = sql.Substring(0, sql.Length - 3);
            sql += "\r\n);\r\n";
            if (withIndex) {
                foreach (var item in ti.Indexs) {
                    var txt = "i_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_");
                    var columns = BuildColumns(item);
                    sql += "CREATE INDEX " + txt + " ON " + GetTableName(ti) + "(" + columns + ");\r\n";
                }
                foreach (var item in ti.Uniques) {
                    var txt = "u_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_");
                    var columns = BuildColumns(item);
                    sql += "CREATE UNIQUE INDEX " + txt + " ON " + GetTableName(ti) + "(" + columns + ");\r\n";
                }
            }
            sql = sql.Substring(0, sql.Length - 2);
            return sql;
        }

        public override string GetCreateIndex(Type type)
        {
            string sql = "";
            var ti = TableInfo.FromType(type);
            foreach (var item in ti.Indexs) {
                var txt = "i_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_");
                var columns = BuildColumns(item);
                sql += $"CREATE INDEX {txt} ON {GetTableName(ti)}({columns});\r\n";
            }
            foreach (var item in ti.Uniques) {
                var txt = "u_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_");
                var columns = BuildColumns(item);
                sql += $"CREATE UNIQUE INDEX {txt} ON {GetTableName(ti)}({columns});\r\n";
            }
            return sql;
        }

        private string BuildColumns(List<string> columnList)
        {
            var sb = new StringBuilder();
            foreach (var col in columnList) {
                sb.Append($"\"{col}\",");
            }
            return sb.ToString().Trim(',');
        }

        public override string GetDropTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return "DROP TABLE " + GetTableName(ti) + ";";
        }

        public override string GetDropTable(string tableName)
        {
            return "DROP TABLE \"" + tableName + "\";";
        }

        public override string GetTruncateTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return "TRUNCATE TABLE " + GetTableName(ti) + ";";
        }

        public override string GetTruncateTable(string tableName)
        {
            return "TRUNCATE TABLE \"" + tableName + "\";";
        }

        private string CreateColumn(TableInfo ti, ColumnInfo ci)
        {
            var type = ci.PropertyType;
            var isRequired = ci.Required;
            if (type.IsEnum) return CreateField(ti, ci, "NUMBER", "10", isRequired);
            if (type == typeof(string)) return CreateField(ti, ci, ci.IsText ? "CLOB" : "NVARCHAR2", ci.IsText ? "" : (string.IsNullOrEmpty(ci.FieldLength) ? "4000" : ci.FieldLength), isRequired);
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

            if (type == typeof(AnsiString)) return CreateField(ti, ci, "VARCHAR2", ci.FieldLength, isRequired);

            if (type == typeof(bool)) return CreateField(ti, ci, "NUMBER", "1", isRequired);
            if (type == typeof(byte)) return CreateField(ti, ci, "NUMBER", "3", isRequired);
            if (type == typeof(char)) return CreateField(ti, ci, "CHAR", "1", isRequired);

            if (type == typeof(UInt16)) return CreateField(ti, ci, "NUMBER", "5", isRequired);
            if (type == typeof(UInt32)) return CreateField(ti, ci, "NUMBER", "10", isRequired);
            if (type == typeof(UInt64)) return CreateField(ti, ci, "NUMBER", "20", isRequired);
            if (type == typeof(Int16)) return CreateField(ti, ci, "NUMBER", "5", isRequired);
            if (type == typeof(Int32)) return CreateField(ti, ci, "NUMBER", "10", isRequired);
            if (type == typeof(Int64)) return CreateField(ti, ci, "NUMBER", "19", isRequired);
            if (type == typeof(Single)) return CreateField(ti, ci, "BINARY_FLOAT", ci.FieldLength, isRequired);
            if (type == typeof(double)) return CreateField(ti, ci, "BINARY_DOUBLE", ci.FieldLength, isRequired);
            if (type == typeof(decimal)) return CreateField(ti, ci, "NUMBER", ci.FieldLength, isRequired);
            if (type == typeof(DateTime)) return CreateField(ti, ci, "DATE", ci.FieldLength, isRequired);
            if (type == typeof(DateOnly)) return CreateField(ti, ci, "DATE", ci.FieldLength, isRequired);
            if (type == typeof(TimeSpan)) return CreateField(ti, ci, "INTERVAL DAY(2) TO SECOND(6)", ci.FieldLength, isRequired);
            if (type == typeof(DateTimeOffset)) return CreateField(ti, ci, "TIMESTAMP WITH TIME ZONE", ci.FieldLength, isRequired);
            if (type == typeof(Guid)) return CreateField(ti, ci, "CHAR", "36", isRequired);

            throw new Exception($"Oracle does not support column type: {ci.PropertyType.Name}");
        }

        private string CreateField(TableInfo ti, ColumnInfo ci, string fieldType, string length, bool isRequired)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"\"{ci.ColumnName}\"");
            sb.AppendFormat(" {0}", fieldType);
            if (string.IsNullOrEmpty(length) == false) {
                sb.AppendFormat("({0})", length);
            }
            // 自增主键使用 identity 列，隐含 NOT NULL，且不允许再显式 DEFAULT / NULL
            if (ti.PrimaryKey == ci.ColumnName && ti.AutoIncrement) {
                sb.Append(" GENERATED BY DEFAULT AS IDENTITY");
            } else {
                if (string.IsNullOrEmpty(ci.DefaultValue) == false) {
                    sb.AppendFormat(" DEFAULT {0}", ci.DefaultValue);
                }
                if (isRequired) {
                    sb.Append(" NOT");
                }
                sb.Append(" NULL");
            }
            if (ti.PrimaryKey == ci.ColumnName) {
                sb.Append(" PRIMARY KEY");
            }
            return sb.ToString();
        }

        public override string GetTableName(string databaseName, string schemaName, string tableName)
        {
            if (string.IsNullOrEmpty(schemaName) == false) {
                return $"\"{schemaName}\".\"{tableName}\"";
            }
            return $"\"{tableName}\"";
        }
    }
}
