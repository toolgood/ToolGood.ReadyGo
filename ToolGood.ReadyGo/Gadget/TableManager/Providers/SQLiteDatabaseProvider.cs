using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget.TableManager.Providers
{
    /// <summary>
    /// SQLite 数据库提供程序
    /// </summary>
    public class SQLiteDatabaseProvider : DatabaseProvider
    {
        /// <summary>
        /// 获取“表不存在时创建”的建表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="withIndex">是否同时生成索引</param>
        /// <returns>建表 SQL</returns>
        public override string GetTryCreateTable(Type type, bool withIndex = true)
        {
            var ti = TableInfo.FromType(type);
            EnsureColumns(ti);
            var table = GetTableName(ti);

            var definitions = new List<string>();
            foreach (var item in ti.Columns) {
                definitions.Add("    " + CreateColumn(ti, item));
            }
            var statements = new List<string> {
                "CREATE TABLE IF NOT EXISTS " + table + "(\r\n" + string.Join(",\r\n", definitions) + "\r\n);"
            };
            if (withIndex) {
                foreach (var item in ti.Indexs) {
                    var txt = "i_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                    var columns = BuildColumns(item);
                    statements.Add("CREATE INDEX IF NOT EXISTS " + txt + " ON " + table + "(" + columns + ");");
                }

                foreach (var item in ti.Uniques) {
                    var txt = "u_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                    var columns = BuildColumns(item);
                    statements.Add("CREATE UNIQUE INDEX IF NOT EXISTS " + txt + " ON " + table + "( " + columns + ");");
                }
            }
            return string.Join("\r\n", statements);
        }

        /// <summary>
        /// 获取创建索引 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>创建索引 SQL</returns>
        public override string GetCreateIndex(Type type)
        {
            //CREATE [UNIQUE|FULLTEXT|SPATIAL] INDEX 索引名 ON 表名（字段名[(长度)][ASC | DESC]）;
            var ti = TableInfo.FromType(type);
            var table = GetTableName(ti);
            var statements = new List<string>();
            foreach (var item in ti.Indexs) {
                var txt = "i_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                var columns = BuildColumns(item);
                statements.Add($"CREATE INDEX {txt} ON {table}({columns});");
            }
            foreach (var item in ti.Uniques) {
                var txt = "u_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                var columns = BuildColumns(item);
                statements.Add($"CREATE UNIQUE INDEX {txt} ON {table}({columns});");
            }
            return string.Join("\r\n", statements);
        }

        private string BuildColumns(List<string> columnList)
        {
            var sb = new StringBuilder();
            foreach (var col in columnList) {
                sb.Append($"[{EscapeBrackets(col)}],");
            }
            return sb.ToString().Trim(',');
        }

        /// <summary>
        /// 获取删除表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>删除表 SQL</returns>
        public override string GetDropTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return "DROP TABLE IF EXISTS " + GetTableName(ti) + ";";
        }

        /// <summary>
        /// 获取删除表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>删除表 SQL</returns>
        public override string GetDropTable(string tableName)
        {
            return "DROP TABLE IF EXISTS " + GetTableName(null, tableName) + ";";
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>清空表 SQL</returns>
        public override string GetTruncateTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return GetTruncateTable(ti.TableName, ti.AutoIncrement);
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>清空表 SQL</returns>
        public override string GetTruncateTable(string tableName)
        {
            return GetTruncateTable(tableName, false);
        }

        private string GetTruncateTable(string tableName, bool withSequenceReset)
        {
            var sql = $"DELETE FROM {GetTableName(null, tableName)};";
            // 仅当目标表为自增表时重置自增计数（此时 sqlite_sequence 表必然已存在），
            // 避免在从未使用 AUTOINCREMENT 的数据库中因 sqlite_sequence 不存在而报错
            if (withSequenceReset) {
                sql += $"\r\nDELETE FROM sqlite_sequence WHERE name='{tableName.Replace("'", "''")}';";
            }
            return sql;
        }

        private string CreateColumn(TableInfo ti, ColumnInfo ci)
        {
            var type = ci.PropertyType;
            var isRequired = ci.Required;
            if (ci.SerializedAs == ColumnInfo.SerializedKind.Int) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (ci.SerializedAs == ColumnInfo.SerializedKind.Long) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (ci.SerializedAs == ColumnInfo.SerializedKind.String) return CreateField(ti, ci, "Text", "", false);
            if (type.IsEnum) return CreateField(ti, ci, "int", ci.FieldLength, isRequired);
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
            if (type == typeof(Double[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(Decimal[])) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
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
            if (type == typeof(List<Double>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<Decimal>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
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
            if (type == typeof(Double)) return CreateField(ti, ci, "REAL", ci.FieldLength, isRequired);
            if (type == typeof(Decimal)) return CreateField(ti, ci, "REAL", ci.FieldLength, isRequired);
            if (type == typeof(DateOnly)) return CreateField(ti, ci, "date", ci.FieldLength, isRequired);
            if (type == typeof(DateTime)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);
            if (type == typeof(TimeSpan)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);
            if (type == typeof(DateTimeOffset)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);

            if (type == typeof(Guid)) return CreateField(ti, ci, "Text", "40", isRequired);

            if (ci.IsSerialized) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);

            throw new NotSupportedException($"Unsupported column type: {ci.PropertyType.Name}");
        }

        private string CreateField(TableInfo ti, ColumnInfo ci, string fieldType, string length, bool isRequired)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[" + EscapeBrackets(ci.ColumnName) + "]");
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
