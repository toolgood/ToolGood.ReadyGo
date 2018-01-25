using System.Data;
using System.Data.Common;
using System.Linq;
using ToolGood.ReadyGo3.PetaPoco.Core;
using ToolGood.ReadyGo3.PetaPoco.Utilities;

namespace ToolGood.ReadyGo3.PetaPoco.Providers
{
    public class FirebirdDbDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
        {
            return GetFactory("FirebirdSql.Data.FirebirdClient.FirebirdClientFactory, FirebirdSql.Data.FirebirdClient");
        }

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var sql = $"{parts.Sql}\nROWS @{args.Length} TO @{args.Length + 1}";
            args = args.Concat(new object[] { skip + 1, skip + take }).ToArray();
            return sql;
        }

        public override object ExecuteInsert(Database database, IDbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText = cmd.CommandText.TrimEnd();

            if (cmd.CommandText.EndsWith(";"))
                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1);

            cmd.CommandText += " RETURNING " + EscapeSqlIdentifier(primaryKeyName) + ";";
            return database.ExecuteScalarHelper(cmd);
        }

        public override string EscapeSqlIdentifier(string sqlIdentifier)
        {
            return $"\"{sqlIdentifier}\"";
        }
    }
}