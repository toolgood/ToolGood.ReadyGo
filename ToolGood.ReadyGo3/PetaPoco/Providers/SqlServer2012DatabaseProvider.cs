using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.PetaPoco;
using ToolGood.ReadyGo3.PetaPoco.Providers;
using ToolGood.ReadyGo3.PetaPoco.Utilities;

namespace ToolGood.ReadyGo3.PetaPoco.Providers
{
    public class SqlServer2012DatabaseProvider : SqlServerDatabaseProvider
    {


        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (!parts.Sql.ToLower().Contains("order by"))
                throw new Exception("SQL Server 2012 Paging via OFFSET requires an ORDER BY statement.");

            var sqlPage = $"{parts.Sql}\nOFFSET @{args.Length} ROWS FETCH NEXT @{args.Length + 1} ROWS ONLY";
            args = args.Concat(new object[] { skip, take }).ToArray();
            return sqlPage;
        }
        public override string GetAutoIncrementExpression(TableInfo tableInfo)
        {
            if (!string.IsNullOrEmpty(tableInfo.SequenceName))
                return $"NEXT VALUE FOR {tableInfo.SequenceName}";
            return null;
        }

    }
}
