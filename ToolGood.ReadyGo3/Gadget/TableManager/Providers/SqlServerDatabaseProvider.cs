using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.Gadget.TableManager.Providers
{
    public class SqlServer2012DatabaseProvider : SqlServerDatabaseProvider{}
    public class SqlServerDatabaseProvider : DatabaseProvider
    {
        // http://www.cnblogs.com/gouchaonan/p/6127799.html

        public override string GetTryCreateTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            var sql = "CREATE TABLE IF NOT EXISTS " + getTableName(ti) + "(\r\n";
            foreach (var item in ti.Columns) {
                sql += "    " + CreateColumn(ti, item) + ",\r\n";
            }
            foreach (var item in ti.Indexs) {
                var txt = "i_" + string.Join("_", item);
                var columns = string.Join(",", item);
                sql += "    INDEX " + txt + "(" + columns + "),\r\n";
            }
            foreach (var item in ti.Uniques) {
                var txt = "u_" + string.Join("_", item);
                var columns = string.Join(",", item);
                sql += "    CONSTRAINT " + txt + " UNIQUE (" + columns + "),\r\n  ";
            }
            sql = sql.Substring(0, sql.Length - 3);
            sql += "\r\n);";
            return sql;
        }

        public override string GetCreateTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            var sql = "CREATE TABLE " + getTableName(ti) + "(\r\n";
            foreach (var item in ti.Columns) {
                sql += "    " + CreateColumn(ti, item) + ",\r\n";
            }
            foreach (var item in ti.Indexs) {
                var txt = "i_" + string.Join("_", item);
                var columns = string.Join(",", item);
                sql += "    INDEX " + txt + "(" + columns + "),\r\n";
            }
            foreach (var item in ti.Uniques) {
                var txt = "u_" + string.Join("_", item);
                var columns = string.Join(",", item);
                sql += "    CONSTRAINT " + txt + " UNIQUE (" + columns + "),\r\n  ";
            }
            sql = sql.Substring(0, sql.Length - 3);
            sql += "\r\n);";
            return sql;
        }

        public override string GetDropTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return "DROP TABLE IF EXISTS [" + ti.TableName + "];";
        }

        public override string GetTruncateTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return "TRUNCATE TABLE " + getTableName(ti) + ";";
        }

        private string getTableName(TableInfo ti)
        {
            if (string.IsNullOrEmpty(ti.SchemaName)) {
                return "[" + ti.TableName + "]";
            }
            return "[" + ti.SchemaName + "].[" + ti.TableName + "]";
        }


        public string CreateColumn(TableInfo ti, ColumnInfo ci)
        {
            var type = ci.PropertyType;
            var isRequired = ci.Required;
            if (type.IsEnum) return CreateField(ti, ci, "int", ci.FieldLength, true);
            if (type == typeof(string)) return CreateField(ti, ci, ci.IsText ? "Text" : "nvarchar", ci.IsText ? "" : (ci.FieldLength == "" ? "4000" : ci.FieldLength), isRequired);
            if (type == typeof(Byte[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(SByte[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(AnsiString)) return CreateField(ti, ci, "varchar", ci.FieldLength, isRequired);


            //var isRequired = ColumnType.IsNullType(type) == false;
            //if (isRequired == false) type = ColumnType.GetBaseType(type);

            if (type == typeof(bool)) return CreateField(ti, ci, "bit", null, isRequired);
            if (type == typeof(byte)) return CreateField(ti, ci, "tinyint", "1", isRequired);
            if (type == typeof(char)) return CreateField(ti, ci, "tinyint", "1", isRequired);

            if (type == typeof(UInt16)) return CreateField(ti, ci, "UNSIGNED smallint", ci.FieldLength, isRequired);
            if (type == typeof(UInt32)) return CreateField(ti, ci, "UNSIGNED int", ci.FieldLength, isRequired);
            if (type == typeof(UInt64)) return CreateField(ti, ci, "UNSIGNED bigint", ci.FieldLength, isRequired);
            if (type == typeof(Int16)) return CreateField(ti, ci, "smallint", ci.FieldLength, isRequired);
            if (type == typeof(Int32)) return CreateField(ti, ci, "int", ci.FieldLength, isRequired);
            if (type == typeof(Int64)) return CreateField(ti, ci, "bigint", ci.FieldLength, isRequired);
            if (type == typeof(Single)) return CreateField(ti, ci, "real", ci.FieldLength, isRequired);
            if (type == typeof(double)) return CreateField(ti, ci, "float", ci.FieldLength, isRequired);
            if (type == typeof(decimal)) return CreateField(ti, ci, "decimal", ci.FieldLength, isRequired);
            if (type == typeof(DateTime)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);
            if (type == typeof(DateTimeOffset)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);
            if (type == typeof(TimeSpan)) return CreateField(ti, ci, "time", ci.FieldLength, isRequired);
            if (type == typeof(Guid)) return CreateField(ti, ci, "uniqueidentifier", ci.FieldLength, isRequired);

            throw new Exception("");
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
                    sb.Append(" identity(1,1) ");
                }
            }
            if (string.IsNullOrEmpty(ci.Comment) == false) {
                sb.AppendFormat(" COMMENT '{0}'", ci.Comment.Replace("'", @"\'"));
            }
            return sb.ToString();
        }
    }
}
