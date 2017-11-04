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

            var sqlPage = string.Format("{0}\nOFFSET @{1} ROWS FETCH NEXT @{2} ROWS ONLY", parts.Sql, args.Length, args.Length + 1);
            args = args.Concat(new object[] { skip, take }).ToArray();
            return sqlPage;
        }
        public override string GetAutoIncrementExpression(TableInfo tableInfo)
        {
            if (!string.IsNullOrEmpty(tableInfo.SequenceName))
                return string.Format("NEXT VALUE FOR {0}", tableInfo.SequenceName);
            return null;
        }

    }
}
