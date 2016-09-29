using System;
using System.Text;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.SqlBuilding.TableCreate;

namespace ToolGood.ReadyGo.Providers.CreateTables
{
    internal class SqlServerCreateTableHelper : CreateTableHelper
    {
        public SqlServerCreateTableHelper()
        {
            provider = Singleton<SqlServerDatabaseProvider>.Instance;
        }

        public override string CreateColumn(Table ti, Column ci)
        {
            var type = ci.PropertyType;
            if (type.IsEnum) return CreateField(ti, ci, "int", ci.FieldLength, true);
            if (type == typeof(string)) return CreateField(ti, ci, ci.IsText ? "Text" : "nvarchar", ci.FieldLength == null ? "" : (ci.FieldLength == "" ? "4000" : ci.FieldLength), false);
            if (type == typeof(Byte[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(SByte[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(ToolGood.ReadyGo.AnsiString)) return CreateField(ti, ci, "varchar", ci.FieldLength, false);


            var isRequired = ColumnType.IsNullType(type) == false;
            if (isRequired == false) type = ColumnType.GetBaseType(type);

            if (type == typeof(bool)) return CreateField(ti, ci, "tinyint", "1", isRequired);
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

        private string CreateField(Table ti, Column ci, string fieldType, string length, bool isRequired)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(provider.EscapeSqlIdentifier(ci.ColumnName));
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
                sb.AppendFormat(" COMMENT '{0}'", ci.Comment.Replace("\'", "''"));
            }
            return sb.ToString();
        }
    }
}