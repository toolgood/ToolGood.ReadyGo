using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget.TableManager.Providers
{
    /// <summary>
    /// Microsoft Access 数据库提供程序（Jet/ACE 方言，基于 System.Data.OleDb）
    /// </summary>
    public class MsAccessDbDatabaseProvider : DatabaseProvider
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
            var sql = "CREATE TABLE " + GetTableName(ti) + "(\r\n";
            foreach (var item in ti.Columns) {
                sql += "    " + CreateColumn(ti, item) + ",\r\n";
            }
            sql = sql.Substring(0, sql.Length - 3);
            sql += "\r\n);\r\n";
            if (withIndex) {
                foreach (var item in ti.Indexs) {
                    var txt = "i_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                    var columns = BuildColumns(item);
                    sql += "CREATE INDEX " + txt + " ON " + GetTableName(ti) + "(" + columns + ");\r\n";
                }

                foreach (var item in ti.Uniques) {
                    var txt = "u_" + ti.TableName + "_" + string.Join("_", item).Replace(" ", "_").Replace("[", "").Replace("]", "");
                    var columns = BuildColumns(item);
                    sql += "CREATE UNIQUE INDEX " + txt + " ON " + GetTableName(ti) + "(" + columns + ");\r\n";
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
            return "DROP TABLE " + GetTableName(ti) + ";";
        }

        /// <summary>
        /// 获取删除表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>删除表 SQL</returns>
        public override string GetDropTable(string tableName)
        {
            return "DROP TABLE [" + tableName + "];";
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>清空表 SQL</returns>
        public override string GetTruncateTable(Type type)
        {
            var ti = TableInfo.FromType(type);
            return GetTruncateTable(ti.TableName, ti.PrimaryKey, ti.AutoIncrement);
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>清空表 SQL</returns>
        public override string GetTruncateTable(string tableName)
        {
            return GetTruncateTable(tableName, null, false);
        }

        private string GetTruncateTable(string tableName, string primaryKey, bool withAutoIncrementReset)
        {
            var sql = $"DELETE FROM [{tableName}];";
            // Access 不支持 TRUNCATE，只能 DELETE；自增表再通过 ALTER COLUMN COUNTER 重置自增计数
            if (withAutoIncrementReset && string.IsNullOrEmpty(primaryKey) == false) {
                sql += $"\r\nALTER TABLE [{tableName}] ALTER COLUMN [{primaryKey}] COUNTER(1,1);";
            }
            return sql;
        }

        private string GetText(ColumnInfo ci)
        {
            if (ci.IsLongText || ci.IsMediumText || ci.IsText) return "MEMO";
            return "TEXT";
        }

        private string GetTextLength(ColumnInfo ci)
        {
            if (ci.IsLongText || ci.IsMediumText || ci.IsText) return "";
            // Access 的 TEXT 最大 255，超过则只能用 MEMO
            if (string.IsNullOrEmpty(ci.FieldLength) == false) {
                return int.TryParse(ci.FieldLength, out int n) && n > 255 ? "" : ci.FieldLength;
            }
            return "255";
        }

        private string CreateColumn(TableInfo ti, ColumnInfo ci)
        {
            var type = ci.PropertyType;
            var isRequired = ci.Required;
            if (ci.IsSerializedAsInt) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (ci.IsSerializedAsLong) return CreateField(ti, ci, "BIGINT", ci.FieldLength, isRequired);
            if (ci.IsSerializedAsString) return CreateField(ti, ci, GetText(ci), GetTextLength(ci), false);
            if (type.IsEnum) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(string)) return CreateField(ti, ci, GetText(ci), GetTextLength(ci), isRequired);
            if (type == typeof(Byte[]) || type == typeof(SByte[])
               || type == typeof(UInt16[]) || type == typeof(UInt32[]) || type == typeof(UInt64[])
               || type == typeof(Int16[]) || type == typeof(Int32[]) || type == typeof(Int64[])
               || type == typeof(Single[]) || type == typeof(double[]) || type == typeof(Decimal[])
               || type == typeof(bool[])
               || type == typeof(List<Byte>) || type == typeof(List<SByte>)
               || type == typeof(List<UInt16>) || type == typeof(List<UInt32>) || type == typeof(List<UInt64>)
               || type == typeof(List<Int16>) || type == typeof(List<Int32>) || type == typeof(List<Int64>)
               || type == typeof(List<Single>) || type == typeof(List<double>) || type == typeof(List<Decimal>)
               || type == typeof(List<bool>)) {
                return CreateField(ti, ci, "LONGBINARY", "", false);
            }

            if (type == typeof(AnsiString)) return CreateField(ti, ci, "TEXT", ci.FieldLength, isRequired);

            if (type == typeof(bool)) return CreateField(ti, ci, "BIT", ci.FieldLength, isRequired);
            if (type == typeof(byte)) return CreateField(ti, ci, "BYTE", "1", isRequired);
            if (type == typeof(char)) return CreateField(ti, ci, "TEXT", "1", isRequired);

            if (type == typeof(UInt16)) return CreateField(ti, ci, "SMALLINT", ci.FieldLength, isRequired);
            if (type == typeof(UInt32)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(UInt64)) return CreateField(ti, ci, "BIGINT", ci.FieldLength, isRequired);
            if (type == typeof(Int16)) return CreateField(ti, ci, "SMALLINT", ci.FieldLength, isRequired);
            if (type == typeof(Int32)) return CreateField(ti, ci, "INTEGER", ci.FieldLength, isRequired);
            if (type == typeof(Int64)) return CreateField(ti, ci, "BIGINT", ci.FieldLength, isRequired);
            if (type == typeof(Single)) return CreateField(ti, ci, "SINGLE", ci.FieldLength, isRequired);
            if (type == typeof(double)) return CreateField(ti, ci, "DOUBLE", ci.FieldLength, isRequired);
            if (type == typeof(decimal)) return CreateField(ti, ci, "DECIMAL", ci.FieldLength, isRequired);
            if (type == typeof(DateTime)) return CreateField(ti, ci, "DATETIME", ci.FieldLength, isRequired);
            if (type == typeof(DateOnly)) return CreateField(ti, ci, "DATETIME", ci.FieldLength, isRequired);
            if (type == typeof(TimeSpan)) return CreateField(ti, ci, "DATETIME", ci.FieldLength, isRequired);
            if (type == typeof(DateTimeOffset)) return CreateField(ti, ci, "DATETIME", ci.FieldLength, isRequired);

            if (type == typeof(Guid)) return CreateField(ti, ci, "GUID", "", isRequired);

            if (ci.IsSerialized) return CreateField(ti, ci, "LONGBINARY", ci.FieldLength, false);

            throw new Exception($"Access does not support column type: {ci.PropertyType.Name}");
        }

        private string CreateField(TableInfo ti, ColumnInfo ci, string fieldType, string length, bool isRequired)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[" + ci.ColumnName + "]");
            sb.AppendFormat(" {0}", fieldType);
            if (string.IsNullOrEmpty(length) == false) {
                sb.AppendFormat("({0})", length);
            }
            // Access 的 AUTOINCREMENT 列隐含 NOT NULL 且不允许 DEFAULT，故忽略 DefaultValue
            if (ti.PrimaryKey == ci.ColumnName && ti.AutoIncrement) {
                sb.Append(" AUTOINCREMENT");
            } else {
                if (isRequired) {
                    sb.Append(" NOT");
                }
                sb.Append(" NULL");
                if (string.IsNullOrEmpty(ci.DefaultValue) == false) {
                    sb.AppendFormat(" DEFAULT {0}", ci.DefaultValue);
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
            // Access 为文件型数据库，无数据库/模式前缀
            return "[" + tableName + "]";
        }
    }
}
