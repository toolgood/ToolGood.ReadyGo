using System.Data.Common;
using System.Linq;
using ToolGood.ReadyGo3.PetaPoco.Core;
using ToolGood.ReadyGo3.PetaPoco.Utilities;

namespace ToolGood.ReadyGo3.PetaPoco.Providers
{
    public class SqlServerCEDatabaseProviders : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
        {
            return GetFactory(
                //"System.Data.SqlServerCe.SqlCeProviderFactory, System.Data.SqlServerCe, Culture=neutral, PublicKeyToken=89845dcd8080cc91",
                "System.Data.SqlServerCe.SqlCeProviderFactory, System.Data.SqlServerCe"
                );
        }

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (string.IsNullOrEmpty(parts.SqlOrderBy))
                parts.Sql += " ORDER BY ABS(1)";
            var sqlPage = $"{parts.Sql}\nOFFSET @{args.Length} ROWS FETCH NEXT @{args.Length + 1} ROWS ONLY";
            args = args.Concat(new object[] {skip, take}).ToArray();
            return sqlPage;
        }

        public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string primaryKeyName)
        {
            db.ExecuteNonQueryHelper(cmd);
            return db.ExecuteScalar<object>("SELECT @@IDENTITY AS NewID;",new object[0]);
        }
    }
}