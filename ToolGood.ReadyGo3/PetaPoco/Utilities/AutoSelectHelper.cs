using System.Linq;
using System;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.PetaPoco.Internal
{
    internal static class AutoSelectHelper
    {
        private static Regex rxSelect = new Regex(@"\A\s*(SELECT|EXECUTE|CALL|WITH|SET|DECLARE)\s",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static Regex rxFrom = new Regex(@"\A\s*FROM\s",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static string AddSelectClause<T>(DatabaseProvider provider, string sql, StandardMapper defaultMapper)
        {
            sql = sql.Trim();
            if (sql.StartsWith(";")) return sql.Substring(1);


            if (!rxSelect.IsMatch(sql))
            {
                var pd = PocoData.ForType(typeof(T), defaultMapper);
                var tableName = provider.EscapeTableName(pd.TableInfo.TableName);
                
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in pd.Columns) {
                    var col = item.Value;
                    stringBuilder.Append(",");

                    if (col.ResultColumn) {
                        if (string.IsNullOrEmpty(col.ResultSql)==false) {
                            stringBuilder.AppendFormat(col.ResultSql, tableName + ".");
                            stringBuilder.Append(" AS ");
                            stringBuilder.Append(col.ColumnName);
                        }
                    } else {
                        stringBuilder.AppendFormat("{0}.{1}", tableName, provider.EscapeSqlIdentifier(col.ColumnName));
                    }
                }
                if (stringBuilder.Length==0) throw new NoColumnException();
                stringBuilder.Remove(0, 1);

   
                if (!rxFrom.IsMatch(sql))
                    sql = $"SELECT {stringBuilder.ToString()} FROM {tableName} {sql}";
                else
                    sql = $"SELECT {stringBuilder.ToString()} {sql}";
            }
            return sql;
        }
    }
}