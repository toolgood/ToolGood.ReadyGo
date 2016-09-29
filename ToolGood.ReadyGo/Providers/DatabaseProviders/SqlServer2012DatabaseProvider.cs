using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.SqlBuilding;

namespace ToolGood.ReadyGo.Providers
{
    class SqlServer2012DatabaseProvider: SqlServerDatabaseProvider
    {
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (!parts.Sql.ToLower().Contains("order by"))
                throw new Exception("SQL Server 2012 Paging via OFFSET requires an ORDER BY statement.");

            var sqlPage = string.Format("{0}\nOFFSET @{1} ROWS FETCH NEXT @{2} ROWS ONLY", parts.Sql, args.Length, args.Length + 1);
            args = args.Concat(new object[] { skip, take }).ToArray();
            return sqlPage;
        }

        public override string GetAutoIncrementExpression(PocoTable ti)
        {
            if (!string.IsNullOrEmpty(ti.SequenceName))
                return string.Format("NEXT VALUE FOR {0}", ti.SequenceName);

            return null;
        }
    }
}
