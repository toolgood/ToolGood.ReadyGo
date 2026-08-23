using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget.TableManager.Providers
{
    /// <summary>
    /// MariaDB 数据库提供程序（继承 MySql 方言）
    /// </summary>
    public class MariaDbDatabaseProvider : MySqlDatabaseProvider
    {
    }

    /// <summary>
    /// MySql 数据库提供程序
    /// </summary>
    public class MySqlDatabaseProvider : DatabaseProvider
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
            var sql = "CREATE TABLE IF NOT EXISTS " + GetTableName(ti) + "(\r\n";
            foreach (var item in ti.Columns) {
                sql += "    " + CreateColumn(ti, item) + ",\r\n";
            }
            if (withIndex) {
                foreach (var item in ti.Indexs) {
                    var txt = "i_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                    var columns = BuildColumns(item);
                    sql += "    INDEX " + txt + "(" + columns + "),\r\n";
                }
                foreach (var item in ti.Uniques) {
                    var txt = "u_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                    var columns = BuildColumns(item);
                    sql += "    UNIQUE INDEX " + txt + " ( " + columns + "),\r\n";
                }
            }
            sql = sql.Substring(0, sql.Length - 3);
            sql += "\r\n);";
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
                var txt = "i_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                var columns = BuildColumns(item);
                sql += $"ALTER TABLE {GetTableName(ti)} ADD INDEX {txt} ({columns});\r\n";
            }
            foreach (var item in ti.Uniques) {
                var txt = "u_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                var columns = BuildColumns(item);
                sql += $"ALTER TABLE {GetTableName(ti)} ADD UNIQUE INDEX {txt} ({columns});\r\n";
            }
            return sql;
        }

        private string BuildColumns(List<string> columnList)
        {
            var sb = new StringBuilder();
            foreach (var col in columnList) {
                sb.Append($"`{col}`,");
            }
            return sb.ToString().Replace("[", "`").Replace("]", "`").Replace("``", "`").Trim(',');
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
            return "DROP TABLE IF EXISTS " + tableName + ";";
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
            return "TRUNCATE TABLE " + tableName + ";";
        }

        private string GetText(ColumnInfo ci)
        {
            if (ci.IsLongText) {
                return "longtext";
            }
            if (ci.IsMediumText) {
                return "mediumtext";
            }
            if (ci.IsText) {
                return "Text";
            }
            return "varchar";
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
            if (type.IsEnum) return CreateField(ti, ci, "int", ci.FieldLength, true);
            if (type == typeof(string)) return CreateField(ti, ci, GetText(ci), ci.IsText ? "" : (string.IsNullOrEmpty(ci.FieldLength) ? "4000" : ci.FieldLength), isRequired);
            if (type == typeof(Byte[]) || type == typeof(SByte[])
               || type == typeof(UInt16[]) || type == typeof(UInt32[]) || type == typeof(UInt64[])
               || type == typeof(Int16[]) || type == typeof(Int32[]) || type == typeof(Int64[])
               || type == typeof(Single[]) || type == typeof(Double[])
               || type == typeof(List<Byte>) || type == typeof(List<SByte>)
               || type == typeof(List<UInt16>) || type == typeof(List<UInt32>) || type == typeof(List<UInt64>)
               || type == typeof(List<Int16>) || type == typeof(List<Int32>) || type == typeof(List<Int64>)
               || type == typeof(List<Single>) || type == typeof(List<Double>)
               || type == typeof(List<bool>) || type == typeof(bool[])

                ) {
                if (int.TryParse(ci.FieldLength, out int fieldLength)) {
                    if (fieldLength <= 255) {
                        return CreateField(ti, ci, "tinyblob", "", false);
                    } else if (fieldLength <= 65535) {
                        return CreateField(ti, ci, "blob", "", false);
                    } else if (fieldLength <= 16777215) {
                        return CreateField(ti, ci, "mediumblob", "", false);
                    } else {
                        return CreateField(ti, ci, "longBlob", "", false);
                    }
                } else {
                    return CreateField(ti, ci, "tinyblob", "", false);
                }
            }
            if (type == typeof(AnsiString)) return CreateField(ti, ci, "varchar", ci.FieldLength, isRequired);

            if (type == typeof(bool)) return CreateField(ti, ci, "tinyint", "1", isRequired);
            if (type == typeof(byte)) return CreateField(ti, ci, "tinyint", "1", isRequired);
            if (type == typeof(char)) return CreateField(ti, ci, "char", "1", isRequired);

            if (type == typeof(UInt16)) return CreateField(ti, ci, "UNSIGNED smallint", ci.FieldLength, isRequired);
            if (type == typeof(UInt32)) return CreateField(ti, ci, "UNSIGNED int", ci.FieldLength, isRequired);
            if (type == typeof(UInt64)) return CreateField(ti, ci, "UNSIGNED bigint", ci.FieldLength, isRequired);
            if (type == typeof(Int16)) return CreateField(ti, ci, "smallint", ci.FieldLength, isRequired);
            if (type == typeof(Int32)) return CreateField(ti, ci, "int", ci.FieldLength, isRequired);
            if (type == typeof(Int64)) return CreateField(ti, ci, "bigint", ci.FieldLength, isRequired);
            if (type == typeof(Single)) return CreateField(ti, ci, "FLOAT", ci.FieldLength, isRequired);
            if (type == typeof(double)) return CreateField(ti, ci, "DOUBLE", ci.FieldLength, isRequired);
            if (type == typeof(decimal)) return CreateField(ti, ci, "decimal", ci.FieldLength, isRequired);
            if (type == typeof(DateTime)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);
            if (type == typeof(TimeSpan)) return CreateField(ti, ci, "time", ci.FieldLength, isRequired);
            if (type == typeof(DateTimeOffset)) return CreateField(ti, ci, "dateTime", ci.FieldLength, isRequired);

            if (type == typeof(Guid)) return CreateField(ti, ci, "char", "40", isRequired);

            if (ci.IsSerialized) return CreateField(ti, ci, "blob", "", false);

            throw new Exception($"Unsupported column type: {ci.PropertyType.Name}");
        }

        private string CreateField(TableInfo ti, ColumnInfo ci, string fieldType, string length, bool isRequired)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("`" + ci.ColumnName + "`");
            sb.AppendFormat(" {0}", fieldType);
            if (string.IsNullOrEmpty(length) == false) {
                sb.AppendFormat("({0})", length);
            }
            if (isRequired) {
                sb.Append(" NOT");
            }
            sb.Append(" NULL");
            if (string.IsNullOrEmpty(ci.DefaultValue) == false) {
                sb.AppendFormat(" DEFAULT {0} ", ci.DefaultValue);
            }
            if (ti.PrimaryKey == ci.ColumnName) {
                sb.Append(" PRIMARY KEY");
                if (ti.AutoIncrement) {
                    sb.Append(" AUTO_INCREMENT");
                }
            }
            if (string.IsNullOrEmpty(ci.Comment) == false) {
                sb.AppendFormat(" COMMENT '{0}'", ci.Comment.Replace("'", @"\'"));
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
            if (string.IsNullOrEmpty(databaseName) == false) {
                return $"`{databaseName}`.`{tableName}`";
            }
            if (string.IsNullOrEmpty(schemaName) == false) {
                return $"`{schemaName}`.`{tableName}`";
            }
            return $"`{tableName}`";
        }
    }
}
