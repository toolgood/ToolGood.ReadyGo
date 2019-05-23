using System.Text;
using System.Text.RegularExpressions;
using ToolGood.ReadyGo3.Exceptions;
using ToolGood.ReadyGo3.Gadget;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.PetaPoco.Internal
{
    internal static class AutoSelectHelper
    {
        private static Regex rxSelect = new Regex(@"\A\s*(SELECT|SQLEXEC|EXEC|EXECUTE|CALL|WITH|SET|DECLARE|USE|GO|PRINT)\s",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static Regex rxFrom = new Regex(@"\A\s*FROM\s",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static string AddSelectClause<T>(DatabaseProvider provider, string sql)
        {
            sql = sql.Trim();
            if (sql.StartsWith(";")) return sql.Substring(1);


            if (!rxSelect.IsMatch(sql))
            {
                var pd = PocoData.ForType(typeof(T));
                var columns = CrudCache.GetSelectColumnsSql(provider, pd);
                if (!rxFrom.IsMatch(sql)) {
                    var tableName = provider.EscapeTableName(pd.TableInfo.TableName);
                    sql = $"SELECT {columns} FROM {tableName} {sql}";
                }
                else
                    sql = $"SELECT {columns} {sql}";
            }
            return sql;
        }
    }
}