using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.Gadget.TableManager.Providers;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
using ToolGood.ReadyGo3.PetaPoco.Internal;

namespace ToolGood.ReadyGo3.Gadget.TableManager
{
    public abstract class DatabaseProvider
    {
        public virtual string GetTryCreateTable(Type type)
        {
            return null;
        }

        public virtual string GetCreateTable(Type type)
        {
            return null;

        }

        public virtual string GetDropTable(Type type)
        {
            return null;

        }

        public virtual string GetTruncateTable(Type type)
        {
            return null;

        }



        protected virtual string EscapeSqlIdentifier(string name)
        {
            return "[" + name + "]";
        }

        internal static DatabaseProvider Resolve(SqlType type)
        {
            switch (type) {
                case SqlType.SqlServer: return Singleton<SqlServerDatabaseProvider>.Instance;
                case SqlType.MySql: return Singleton<MySqlDatabaseProvider>.Instance;
                case SqlType.SQLite: return Singleton<SQLiteDatabaseProvider>.Instance;
                //case SqlType.MsAccessDb: return Singleton<MsAccessDbDatabaseProvider>.Instance;
                //case SqlType.Oracle: return Singleton<OracleDatabaseProvider>.Instance;
                //case SqlType.PostgreSQL: return Singleton<PostgreSQLDatabaseProvider>.Instance;
                //case SqlType.FirebirdDb: return Singleton<FirebirdDbDatabaseProvider>.Instance;
                case SqlType.MariaDb: return Singleton<MariaDbDatabaseProvider>.Instance;
                //case SqlType.SqlServerCE: return Singleton<SqlServerCEDatabaseProviders>.Instance;
                case SqlType.SqlServer2012: return Singleton<SqlServer2012DatabaseProvider>.Instance;
                default: break;
            }
            throw new NotSupportedException();
            //return Singleton<SqlServerDatabaseProvider>.Instance;
        }

    }
}
