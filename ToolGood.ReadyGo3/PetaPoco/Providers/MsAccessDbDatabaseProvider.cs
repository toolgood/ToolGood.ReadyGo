using System;
using System.Data;
using System.Data.Common;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
using ToolGood.ReadyGo3.PetaPoco.Core;
using ToolGood.ReadyGo3.PetaPoco.Utilities;

namespace ToolGood.ReadyGo3.PetaPoco.Providers
{
    public class MsAccessDbDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
        {
#if NETSTANDARD2_0
            throw new DatabaseUnsupportException();
#else
            return DbProviderFactories.GetFactory("System.Data.OleDb");
#endif
        }

        public override object ExecuteInsert(Database database, IDbCommand cmd, string primaryKeyName)
        {
            database.ExecuteNonQueryHelper(cmd);
            cmd.CommandText = "SELECT @@IDENTITY AS NewID;";
            return database.ExecuteScalarHelper(cmd);
        }

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            throw new NotSupportedException("The Access provider does not support paging.");
        }
    }
}