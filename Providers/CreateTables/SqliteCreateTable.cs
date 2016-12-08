using System;
using System.Text;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.SqlBuilding.TableCreate;

namespace ToolGood.ReadyGo.Providers.CreateTables
{
    internal class SqliteCreateTableHelper : CreateTableHelper
    {
        public SqliteCreateTableHelper()
        {
            provider = Singleton<SQLiteDatabaseProvider>.Instance;
        }

        public override string CreateColumn(Table ti, Column ci)
        {
            var type = ci.PropertyType;
            var isRequired = ci.Required;
            if (type.IsEnum) return CreateField(ti, ci, "int", ci.FieldLength, true);
            if (type == typeof(string)) return CreateField(ti, ci, "Text", "", isRequired);
            if (type == typeof(Byte[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, isRequired);
            if (type == typeof(SByte[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, isRequired);
            if (type == typeof(ToolGood.ReadyGo.AnsiString)) return CreateField(ti, ci, "Text", ci.FieldLength, isRequired);

            //var isRequired = ColumnType.IsNullType(type) == false;
            //if (isRequired == false) type = ColumnType.GetBaseType(type);

            if (type == typeof(bool)) return CreateField(ti, ci, "int", "1", isRequired);
            if (type == typeof(byte)) return CreateField(ti, ci, "int", "1", isRequired);
            if (type == typeof(char)) return CreateField(ti, ci, "char", "1", isRequired);

            if (type == typeof(UInt16)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(UInt32)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(UInt64)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(Int16)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(Int32)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(Int64)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(Single)) return CreateField(ti, ci, "double", ci.FieldLength, isRequired);
            if (type == typeof(double)) return CreateField(ti, ci, "double", ci.FieldLength, isRequired);
            if (type == typeof(decimal)) return CreateField(ti, ci, "double", ci.FieldLength, isRequired);
            if (type == typeof(DateTime)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);
            if (type == typeof(TimeSpan)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);
            if (type == typeof(DateTimeOffset)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);

            if (type == typeof(Guid)) return CreateField(ti, ci, "Text", "40", isRequired);

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
                    sb.Append(" AutoIncrement");
                }
            }
            //if (string.IsNullOrEmpty(ci.Comment) == false) {
            //    sb.AppendFormat(" COMMENT '{0}'", ci.Comment);
            //}
            return sb.ToString();
        }

        public override string GetTableName(Table ti, TableNameManger _tableNameManger)
        {
            if (ti.TableName.Contains(".")) {
                return ti.TableName;
            }
            var tag = ti.FixTag;
            var tableName = ti.TableName;

            var name = _tableNameManger.Get(tag);
            if (name != null) {
                tableName = name.TablePrefix + tableName + name.TableSuffix;
            }
            var schemaName = ti.SchemaName;

            if (string.IsNullOrEmpty(schemaName)) {
                return provider.EscapeSqlIdentifier(tableName);
            }
            return string.Format("[{0}_{1}]", schemaName, tableName);
        }
    }
}