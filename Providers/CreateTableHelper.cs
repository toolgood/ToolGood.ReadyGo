using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.Providers.CreateTables;
using ToolGood.ReadyGo.SqlBuilding.TableCreate;

namespace ToolGood.ReadyGo.Providers
{
    internal abstract class CreateTableHelper
    {
        protected DatabaseProvider provider;

        public virtual string GetTableName(Table ti, TableNameManger _tableNameManger)
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
            return string.Format("{0}.{1}", provider.EscapeSqlIdentifier(schemaName), provider.EscapeSqlIdentifier(tableName));
        }

        public virtual string CreateTable(string tableName, Table ti, List<Column> cis, string nextLine = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE {0} (", tableName);

            for (int i = 0; i < cis.Count; i++) {
                var ci = cis[i];
                if (i > 0) sb.Append(",");
                sb.Append(nextLine);
                sb.Append(CreateColumn(ti, ci));
            }
            sb.Append(");");
            return sb.ToString();
        }

        public abstract string CreateColumn(Table ti, Column ci);

        public virtual string CreateIndex(string tableName, List<string> list)
        {
            var sql = "CREATE INDEX {1} ON {0} ({2});";
            var name = "index_" + tableName.Replace("[", "").Replace("]", "").Replace("`", "").Replace("'", "").Replace(".", "_")
                + "_" + string.Join("_", list);
            var columns = string.Join(",", list.Select(q => provider.EscapeSqlIdentifier(q)));
            return string.Format(sql, tableName, name, columns);
        }

        public virtual string CreateUnique(string tableName, List<string> list)
        {
            var sql = "CREATE UNIQUE INDEX {1} ON {0} ({2});";
            var name = "index_" + tableName.Replace("[", "").Replace("]", "").Replace("`", "").Replace("'", "").Replace(".", "_")
                + "_" + string.Join("_", list);
            var columns = string.Join(",", list.Select(q => provider.EscapeSqlIdentifier(q)));
            return string.Format(sql, tableName, name, columns);
        }

        public string DeleteTable(string tableName)
        {
            return string.Format("DROP TABLE IF EXISTS {0};", tableName);
        }

        public static CreateTableHelper Resolve(SqlType type)
        {
            switch (type) {
                case SqlType.SqlServer: return new SqlServerCreateTableHelper();
                case SqlType.MySql: return new MysqlCreateTableHelper();
                case SqlType.SQLite: return new SqliteCreateTableHelper();
                case SqlType.MsAccessDb:
                    break;

                case SqlType.Oracle:
                    break;

                case SqlType.PostgreSQL:
                    break;

                case SqlType.FirebirdDb:
                    break;

                case SqlType.MariaDb:
                    break;

                case SqlType.SqlServerCE:
                    break;

                default:
                    break;
            }
            return new SqliteCreateTableHelper();
        }
    }
}