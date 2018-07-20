using System.Data.Common;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.PetaPoco.Providers
{
    public class MySqlDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
        {
            return GetFactory(
                //"MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Culture=neutral, PublicKeyToken=c5687fc88969c44d",
                "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data"
                );
        }

        public override string GetParameterPrefix(string connectionString)
        {
            if (connectionString != null && connectionString.IndexOf("Allow User Variables=true") >= 0)
                return "?";
            else
                return "@";
        }

        public override string EscapeSqlIdentifier(string sqlIdentifier)
        {
            return $"`{sqlIdentifier}`";
        }

        public override string GetExistsSql()
        {
            return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
        }
    }
}