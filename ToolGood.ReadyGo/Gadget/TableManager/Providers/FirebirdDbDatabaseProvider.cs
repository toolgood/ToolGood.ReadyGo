using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget.TableManager.Providers
{
    /// <summary>
    /// Firebird 数据库提供程序
    /// </summary>
    public class FirebirdDbDatabaseProvider : DatabaseProvider
    {
        /// <summary>
        /// 获取“表不存在时创建”的建表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="withIndex">是否同时生成索引</param>
        /// <returns>建表 SQL</returns>
        public override string GetTryCreateTable(Type type, bool withIndex = true)
        {
            // Firebird 不支持 CREATE TABLE/INDEX IF NOT EXISTS，且一次只能执行一条语句，
            // 因此将建表/建索引全部合并到单条 EXECUTE BLOCK 中，逐条判断 rdb$relations 实现幂等。
            var ti = TableInfo.FromType(type);
            EnsureColumns(ti);
            var table = GetTableName(ti);

            var definitions = new List<string>();
            foreach (var item in ti.Columns) {
                definitions.Add(CreateColumn(ti, item));
            }
            var ddlList = new List<string> {
                "CREATE TABLE " + table + "(" + string.Join(", ", definitions) + ")"
            };
            if (withIndex) {
                foreach (var item in ti.Indexs) {
                    var txt = "i_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_");
                    var columns = BuildColumns(item);
                    ddlList.Add("CREATE INDEX " + txt + " ON " + table + "(" + columns + ")");
                }
                foreach (var item in ti.Uniques) {
                    var txt = "u_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_");
                    var columns = BuildColumns(item);
                    ddlList.Add("CREATE UNIQUE INDEX " + txt + " ON " + table + "(" + columns + ")");
                }
            }
            var sb = new StringBuilder();
            sb.Append("EXECUTE BLOCK AS BEGIN");
            var tableName = ti.TableName.Replace("'", "''");
            foreach (var ddl in ddlList) {
                sb.Append(" IF (NOT EXISTS(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = '")
                  .Append(tableName)
                  .Append("')) THEN EXECUTE STATEMENT '")
                  .Append(ddl.Replace("'", "''"))
                  .Append("';");
            }
            sb.Append(" END");
            return sb.ToString();
        }

        private static string WrapDropIdempotent(string ddl, string tableName)
        {
            return "EXECUTE BLOCK AS BEGIN IF (EXISTS(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = '" + tableName.Replace("'", "''") + "')) THEN EXECUTE STATEMENT '" + ddl.Replace("'", "''") + "'; END";
        }

        /// <summary>
        /// 获取创建索引 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>创建索引 SQL</returns>
        public override string GetCreateIndex(Type type)
        {
            var ti = TableInfo.FromType(type);
            var table = GetTableName(ti);
            var statements = new List<string>();
            foreach (var item in ti.Indexs) {
                var txt = "i_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_");
                var columns = BuildColumns(item);
                statements.Add($"CREATE INDEX {txt} ON {table}({columns});");
            }
            foreach (var item in ti.Uniques) {
                var txt = "u_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_");
                var columns = BuildColumns(item);
                statements.Add($"CREATE UNIQUE INDEX {txt} ON {table}({columns});");
            }
            return string.Join("\r\n", statements);
        }

        private string BuildColumns(List<string> columnList)
        {
            var sb = new StringBuilder();
            foreach (var col in columnList) {
                sb.Append($"\"{col.Replace("\"", "\"\"")}\",");
            }
            return sb.ToString().Trim(',');
        }

        /// <summary>
        /// 获取删除表 SQL（表不存在时不报错）
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>删除表 SQL</returns>
        public override string GetDropTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return WrapDropIdempotent("DROP TABLE " + GetTableName(ti) + ";", ti.TableName);
        }

        /// <summary>
        /// 获取删除表 SQL（表不存在时不报错）
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>删除表 SQL</returns>
        public override string GetDropTable(string tableName)
        {
            return WrapDropIdempotent("DROP TABLE " + GetTableName(null, tableName) + ";", tableName);
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>清空表 SQL</returns>
        public override string GetTruncateTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return "DELETE FROM " + GetTableName(ti) + ";";
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>清空表 SQL</returns>
        public override string GetTruncateTable(string tableName)
        {
            return "DELETE FROM " + GetTableName(null, tableName) + ";";
        }

        private string CreateColumn(TableInfo ti, ColumnInfo ci)
        {
            var type = ci.PropertyType;
            var isRequired = ci.Required;
            if (ci.SerializedAs == ColumnInfo.SerializedKind.Int) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (ci.SerializedAs == ColumnInfo.SerializedKind.Long) return CreateField(ti, ci, "BIGINT", ci.FieldLength, isRequired);
            if (ci.SerializedAs == ColumnInfo.SerializedKind.String) return CreateField(ti, ci, ci.IsText ? "BLOB SUB_TYPE TEXT" : "VARCHAR", ci.IsText ? "" : (string.IsNullOrEmpty(ci.FieldLength) ? "4000" : ci.FieldLength), false);
            if (type.IsEnum) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(string)) return CreateField(ti, ci, ci.IsText ? "BLOB SUB_TYPE TEXT" : "VARCHAR", ci.IsText ? "" : (string.IsNullOrEmpty(ci.FieldLength) ? "4000" : ci.FieldLength), isRequired);
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
            if (type == typeof(List<double>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<Decimal>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);
            if (type == typeof(List<bool>)) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);

            if (type == typeof(AnsiString)) return CreateField(ti, ci, "VARCHAR", ci.FieldLength, isRequired);

            if (type == typeof(bool)) return CreateField(ti, ci, "BOOLEAN", ci.FieldLength, isRequired);
            if (type == typeof(byte)) return CreateField(ti, ci, "SMALLINT", null, isRequired);
            if (type == typeof(char)) return CreateField(ti, ci, "CHAR", "1", isRequired);

            if (type == typeof(UInt16)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(UInt32)) return CreateField(ti, ci, "BIGINT", ci.FieldLength, isRequired);
            if (type == typeof(UInt64)) return CreateField(ti, ci, "BIGINT", ci.FieldLength, isRequired);
            if (type == typeof(Int16)) return CreateField(ti, ci, "SMALLINT", ci.FieldLength, isRequired);
            if (type == typeof(Int32)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(Int64)) return CreateField(ti, ci, "BIGINT", ci.FieldLength, isRequired);
            if (type == typeof(Single)) return CreateField(ti, ci, "FLOAT", ci.FieldLength, isRequired);
            if (type == typeof(double)) return CreateField(ti, ci, "DOUBLE PRECISION", ci.FieldLength, isRequired);
            if (type == typeof(decimal)) return CreateField(ti, ci, "DECIMAL", ci.FieldLength, isRequired);
            if (type == typeof(DateTime)) return CreateField(ti, ci, "TIMESTAMP", ci.FieldLength, isRequired);
            if (type == typeof(DateOnly)) return CreateField(ti, ci, "DATE", ci.FieldLength, isRequired);
            if (type == typeof(TimeSpan)) return CreateField(ti, ci, "TIME", ci.FieldLength, isRequired);
            if (type == typeof(DateTimeOffset)) return CreateField(ti, ci, "TIMESTAMP WITH TIME ZONE", ci.FieldLength, isRequired);
            if (type == typeof(Guid)) return CreateField(ti, ci, "CHAR", "36", isRequired);

            if (ci.IsSerialized) return CreateField(ti, ci, "BLOB", ci.FieldLength, false);

            throw new NotSupportedException($"Firebird does not support column type: {ci.PropertyType.Name}");
        }

        private string CreateField(TableInfo ti, ColumnInfo ci, string fieldType, string length, bool isRequired)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"\"{ci.ColumnName.Replace("\"", "\"\"")}\"");
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
                // Firebird 不允许显式 NULL 关键字，可空列默认即为可空，仅需声明 NOT NULL
                if (isRequired) {
                    sb.Append(" NOT NULL");
                }
            }
            if (ti.PrimaryKey == ci.ColumnName) {
                sb.Append(" PRIMARY KEY");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="schemaName">模式名</param>
        /// <param name="tableName">表名</param>
        /// <returns>转义后的表名</returns>
        public override string GetTableName(string schemaName, string tableName)
        {
            return $"\"{tableName.Replace("\"", "\"\"")}\"";
        }
    }
}
