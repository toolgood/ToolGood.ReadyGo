using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget.TableManager.Providers
{
    /// <summary>
    /// SQL Server 数据库提供程序
    /// </summary>
    public class SqlServerDatabaseProvider : DatabaseProvider
    {
        // http://www.cnblogs.com/gouchaonan/p/6127799.html

        /// <summary>
        /// 获取“表不存在时创建”的建表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="withIndex">是否同时生成索引</param>
        /// <returns>建表 SQL</returns>
        public override string GetTryCreateTable(Type type, bool withIndex = true)
        {
            // SQL Server 不支持 CREATE TABLE/INDEX IF NOT EXISTS，
            // 通过 IF NOT EXISTS(sys.tables) 包裹整段 DDL 保证幂等，重复调用不会因“对象已存在”报错。
            var ti = TableInfo.FromType(type);
            EnsureColumns(ti);
            var table = GetTableName(ti);
            var schema = string.IsNullOrEmpty(ti.SchemaName) ? "dbo" : ti.SchemaName.Replace("'", "''");
            var tableName = ti.TableName.Replace("'", "''");

            var sb = new StringBuilder();
            sb.AppendLine($"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'{tableName}' AND schema_id = SCHEMA_ID(N'{schema}'))");
            sb.AppendLine("BEGIN");
            sb.Append("    CREATE TABLE " + table + "(\r\n");
            foreach (var item in ti.Columns) {
                sb.Append("        " + CreateColumn(ti, item) + ",\r\n");
            }
            if (withIndex) {
                foreach (var item in ti.Uniques) {
                    var txt = "u_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                    var columns = BuildColumns(item);
                    sb.Append("        CONSTRAINT " + txt + " UNIQUE (" + columns + "),\r\n");
                }
            }
            sb.Length -= 3; // 去掉末尾的 ",\r\n"
            sb.Append("\r\n    );");
            if (withIndex) {
                foreach (var item in ti.Indexs) {
                    var txt = "i_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                    var columns = BuildColumns(item);
                    sb.Append("\r\n    CREATE INDEX " + txt + " ON " + table + "(" + columns + ");");
                }
            }
            sb.AppendLine();
            sb.Append("END");
            return sb.ToString();
        }

        /// <summary>
        /// 获取创建索引 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>创建索引 SQL</returns>
        public override string GetCreateIndex(Type type)
        {
            //CREATE [UNIQUE|FULLTEXT|SPATIAL] INDEX 索引名 ON 表名（字段名[(长度)][ASC | DESC]）;
            string sql = "";
            var ti = TableInfo.FromType(type);
            foreach (var item in ti.Indexs) {
                var txt = "i_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                var columns = BuildColumns(item);
                sql += $"CREATE INDEX {txt} ON {GetTableName(ti)}({columns});\r\n";
            }
            foreach (var item in ti.Uniques) {
                var txt = "u_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                var columns = BuildColumns(item);
                sql += $"CREATE UNIQUE INDEX {txt} ON {GetTableName(ti)}({columns});\r\n";
            }
            return sql;
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
            return "TRUNCATE TABLE " + GetTableName(ti) + ";";
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>清空表 SQL</returns>
        public override string GetTruncateTable(string tableName)
        {
            return "TRUNCATE TABLE " + GetTableName(null, tableName) + ";";
        }

        /// <summary>
        /// 生成列定义 SQL
        /// </summary>
        /// <param name="ti">表结构信息</param>
        /// <param name="ci">列信息</param>
        /// <returns>列定义 SQL</returns>
        public string CreateColumn(TableInfo ti, ColumnInfo ci)
        {
            var type = ci.PropertyType;
            var isRequired = ci.Required;
            if (ci.SerializedAs == ColumnInfo.SerializedKind.Int) return CreateField(ti, ci, "int", ci.FieldLength, isRequired);
            if (ci.SerializedAs == ColumnInfo.SerializedKind.Long) return CreateField(ti, ci, "bigint", ci.FieldLength, isRequired);
            if (ci.SerializedAs == ColumnInfo.SerializedKind.String) return CreateField(ti, ci, ci.IsText ? "Text" : "nvarchar", ci.IsText ? "" : (string.IsNullOrEmpty(ci.FieldLength) ? "4000" : ci.FieldLength), false);
            if (type.IsEnum) return CreateField(ti, ci, "int", ci.FieldLength, isRequired);
            if (type == typeof(string)) return CreateField(ti, ci, ci.IsText ? "Text" : "nvarchar", ci.IsText ? "" : (string.IsNullOrEmpty(ci.FieldLength) ? "4000" : ci.FieldLength), isRequired);
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

            if (type == typeof(AnsiString)) return CreateField(ti, ci, "varchar", ci.FieldLength, isRequired);

            if (type == typeof(bool)) return CreateField(ti, ci, "bit", null, isRequired);
            if (type == typeof(byte)) return CreateField(ti, ci, "tinyint", null, isRequired);
            if (type == typeof(char)) return CreateField(ti, ci, "nchar", "1", isRequired);

            if (type == typeof(UInt16)) return CreateField(ti, ci, "int", ci.FieldLength, isRequired);
            if (type == typeof(UInt32)) return CreateField(ti, ci, "bigint", ci.FieldLength, isRequired);
            if (type == typeof(UInt64)) return CreateField(ti, ci, "bigint", ci.FieldLength, isRequired);
            if (type == typeof(Int16)) return CreateField(ti, ci, "smallint", ci.FieldLength, isRequired);
            if (type == typeof(Int32)) return CreateField(ti, ci, "int", ci.FieldLength, isRequired);
            if (type == typeof(Int64)) return CreateField(ti, ci, "bigint", ci.FieldLength, isRequired);
            if (type == typeof(Single)) return CreateField(ti, ci, "real", ci.FieldLength, isRequired);
            if (type == typeof(double)) return CreateField(ti, ci, "float", ci.FieldLength, isRequired);
            if (type == typeof(decimal)) return CreateField(ti, ci, "decimal", ci.FieldLength, isRequired);
            if (type == typeof(DateOnly)) return CreateField(ti, ci, "date", ci.FieldLength, isRequired);
            if (type == typeof(DateTime)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);
            if (type == typeof(DateTimeOffset)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);
            if (type == typeof(TimeSpan)) return CreateField(ti, ci, "time", ci.FieldLength, isRequired);
            if (type == typeof(Guid)) return CreateField(ti, ci, "uniqueidentifier", ci.FieldLength, isRequired);

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
            // SQL Server 的 IDENTITY 属性必须位于 NULL/NOT NULL 与列约束（PRIMARY KEY）之前
            if (ti.PrimaryKey == ci.ColumnName && ti.AutoIncrement) {
                sb.Append(" identity(1,1)");
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
            }
            return sb.ToString();
        }
    }
}
