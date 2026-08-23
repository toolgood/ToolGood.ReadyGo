using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget.TableManager.Providers
{
    /// <summary>
    /// PostgreSQL 数据库提供程序
    /// </summary>
    public class PostgreSQLDatabaseProvider : DatabaseProvider
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
            var sql = "CREATE SEQUENCE IF NOT EXISTS seq_" + ti.TableName + " START 1;";
            sql += "CREATE TABLE IF NOT EXISTS \"" + ti.TableName + "\"(\r\n";
            foreach (var item in ti.Columns) {
                sql += "    " + CreateColumn(ti, item) + ",\r\n";
            }
            sql = sql.Substring(0, sql.Length - 3);
            sql += "\r\n);\r\n";
            if (withIndex) {
                foreach (var item in ti.Indexs) {
                    var txt = "i_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_");
                    var columns = BuildColumns(item);
                    sql += "CREATE INDEX IF NOT EXISTS " + txt + " ON \"" + ti.TableName + "\"(" + columns + ");\r\n";
                }

                foreach (var item in ti.Uniques) {
                    var txt = "u_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_");
                    var columns = BuildColumns(item);
                    sql += "CREATE UNIQUE INDEX IF NOT EXISTS " + txt + " ON \"" + ti.TableName + "\"( " + columns + ");\r\n";
                }
            }
            sql = sql.Substring(0, sql.Length - 2);
            return sql;
        }

        /// <summary>
        /// 获取创建索引 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>创建索引 SQL</returns>
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

        /// <summary>
        /// 获取删除表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>删除表 SQL</returns>
        public override string GetDropTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return "DROP TABLE IF EXISTS \"" + ti.TableName + "\";\r\nDROP SEQUENCE IF EXISTS seq_" + ti.TableName + ";";
        }

        /// <summary>
        /// 获取删除表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>删除表 SQL</returns>
        public override string GetDropTable(string tableName)
        {
            return "DROP TABLE IF EXISTS \"" + tableName + "\";\r\nDROP SEQUENCE IF EXISTS seq_" + tableName + ";";
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>清空表 SQL</returns>
        public override string GetTruncateTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return GetTruncateTable(ti.TableName);
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>清空表 SQL</returns>
        public override string GetTruncateTable(string tableName)
        {
            return $"TRUNCATE TABLE \"{tableName}\";";
        }

        private string CreateColumn(TableInfo ti, ColumnInfo ci)
        {
            var type = ci.PropertyType;
            var isRequired = ci.Required;
            if (ci.IsSerializedAsInt) return CreateField(ti, ci, "bigint", ci.FieldLength, isRequired);
            if (type.IsEnum) return CreateField(ti, ci, "integer", ci.FieldLength, isRequired);
            if (type == typeof(string)) return CreateField(ti, ci, ci.IsText ? "text" : "varchar", ci.IsText ? "" : (string.IsNullOrEmpty(ci.FieldLength) ? "4000" : ci.FieldLength), isRequired);
            if (type == typeof(Byte[])) return CreateField(ti, ci, "bytea", ci.FieldLength, false);
            if (type == typeof(SByte[])) return CreateField(ti, ci, "bytea", ci.FieldLength, false);
            if (type == typeof(UInt16[])) return CreateField(ti, ci, "smallint[]", ci.FieldLength, false);
            if (type == typeof(UInt32[])) return CreateField(ti, ci, "integer[]", ci.FieldLength, false);
            if (type == typeof(UInt64[])) return CreateField(ti, ci, "bigint[]", ci.FieldLength, false);
            if (type == typeof(Int16[])) return CreateField(ti, ci, "smallint[]", ci.FieldLength, false);
            if (type == typeof(Int32[])) return CreateField(ti, ci, "integer[]", ci.FieldLength, false);
            if (type == typeof(Int64[])) return CreateField(ti, ci, "bigint[]", ci.FieldLength, false);
            if (type == typeof(Single[])) return CreateField(ti, ci, "real[]", ci.FieldLength, false);
            if (type == typeof(double[])) return CreateField(ti, ci, "double precision[]", ci.FieldLength, false);
            if (type == typeof(bool[])) return CreateField(ti, ci, "boolean[]", ci.FieldLength, false);

            if (type == typeof(List<Byte>)) return CreateField(ti, ci, "bytea", ci.FieldLength, false);
            if (type == typeof(List<SByte>)) return CreateField(ti, ci, "bytea", ci.FieldLength, false);
            if (type == typeof(List<UInt16>)) return CreateField(ti, ci, "smallint[]", ci.FieldLength, false);
            if (type == typeof(List<UInt32>)) return CreateField(ti, ci, "integer[]", ci.FieldLength, false);
            if (type == typeof(List<UInt64>)) return CreateField(ti, ci, "bigint[]", ci.FieldLength, false);
            if (type == typeof(List<Int16>)) return CreateField(ti, ci, "smallint[]", ci.FieldLength, false);
            if (type == typeof(List<Int32>)) return CreateField(ti, ci, "integer[]", ci.FieldLength, false);
            if (type == typeof(List<Int64>)) return CreateField(ti, ci, "bigint[]", ci.FieldLength, false);
            if (type == typeof(List<Single>)) return CreateField(ti, ci, "real[]", ci.FieldLength, false);
            if (type == typeof(List<double>)) return CreateField(ti, ci, "double precision[]", ci.FieldLength, false);
            if (type == typeof(List<bool>)) return CreateField(ti, ci, "boolean[]", ci.FieldLength, false);

            if (type == typeof(AnsiString)) return CreateField(ti, ci, "varchar", ci.FieldLength, isRequired);

            if (type == typeof(bool)) return CreateField(ti, ci, "boolean", ci.FieldLength, isRequired);
            if (type == typeof(byte)) return CreateField(ti, ci, "smallint", "1", isRequired);
            if (type == typeof(char)) return CreateField(ti, ci, "char", "1", isRequired);

            if (type == typeof(UInt16)) return CreateField(ti, ci, "integer", ci.FieldLength, isRequired);
            if (type == typeof(UInt32)) return CreateField(ti, ci, "bigint", ci.FieldLength, isRequired);
            if (type == typeof(UInt64)) return CreateField(ti, ci, "numeric", "20", isRequired);
            if (type == typeof(Int16)) return CreateField(ti, ci, "smallint", ci.FieldLength, isRequired);
            if (type == typeof(Int32)) return CreateField(ti, ci, "integer", ci.FieldLength, isRequired);
            if (type == typeof(Int64)) return CreateField(ti, ci, "bigint", ci.FieldLength, isRequired);
            if (type == typeof(Single)) return CreateField(ti, ci, "real", ci.FieldLength, isRequired);
            if (type == typeof(double)) return CreateField(ti, ci, "double precision", ci.FieldLength, isRequired);
            if (type == typeof(decimal)) return CreateField(ti, ci, "numeric", ci.FieldLength, isRequired);
            if (type == typeof(DateTime)) return CreateField(ti, ci, "timestamp", ci.FieldLength, isRequired);
            if (type == typeof(DateOnly)) return CreateField(ti, ci, "date", ci.FieldLength, isRequired);
            if (type == typeof(TimeSpan)) return CreateField(ti, ci, "time", ci.FieldLength, isRequired);
            if (type == typeof(DateTimeOffset)) return CreateField(ti, ci, "timestamptz", ci.FieldLength, isRequired);
            if (type == typeof(Guid)) return CreateField(ti, ci, "uuid", ci.FieldLength, isRequired);

            if (ci.IsSerialized) return CreateField(ti, ci, "bytea", ci.FieldLength, false);

            throw new Exception($"PostgreSQL does not support column type: {ci.PropertyType.Name}");
        }

        private string CreateField(TableInfo ti, ColumnInfo ci, string fieldType, string length, bool isRequired)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"\"{ci.ColumnName}\"");
            sb.AppendFormat(" {0}", fieldType);
            if (string.IsNullOrEmpty(length) == false) {
                sb.AppendFormat("({0})", length);
            }
            // DEFAULT 子句必须位于列约束之前；自增优先于 DefaultValue，避免出现两个 DEFAULT
            if (ti.PrimaryKey == ci.ColumnName && ti.AutoIncrement) {
                sb.AppendFormat(" DEFAULT NEXTVAL('seq_{0}')", ti.TableName);
            } else if (string.IsNullOrEmpty(ci.DefaultValue) == false) {
                sb.AppendFormat(" DEFAULT({0})", ci.DefaultValue);
            }
            if (isRequired) {
                sb.Append(" NOT");
            }
            sb.Append(" NULL");
            if (ti.PrimaryKey == ci.ColumnName) {
                sb.Append(" PRIMARY KEY");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="databaseName">数据库名</param>
        /// <param name="schemaName">模式名</param>
        /// <param name="tableName">表名</param>
        /// <returns>转义后的表名</returns>
        public override string GetTableName(string databaseName, string schemaName, string tableName)
        {
            if (string.IsNullOrEmpty(schemaName) == false) {
                return $"\"{schemaName}\".\"{tableName}\"";
            }
            return $"\"{tableName}\"";
        }
    }
}
