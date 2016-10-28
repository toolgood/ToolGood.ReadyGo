using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.Providers;

namespace ToolGood.ReadyGo.SqlBuilding
{
    internal static partial class SelectHelper
    {
        public static string GetSelectCount<T>(DatabaseProvider DatabaseType, string sql, TableNameManger manger)
        {
            if (sql.StartsWith(";")) return sql.Substring(1);
            var ss = SqlSearch.Search(sql);
            if (ss.HasSelect) return sql;

            if (ss.HasFrom) {
                return "SELECT Count(1) " + sql;
            }

            var pd = PocoData.ForType(typeof(T));
            var fullTableName = DatabaseType.GetTableName(pd, manger);

            if (ss.HasT1) {
                return string.Format("SELECT Count(1) FROM {0} AS t1 {1}", fullTableName, sql);
            }
            return string.Format("SELECT Count(1) FROM {0} {1}", fullTableName, sql);

        }
        public static string AddSelectClause<T>(DatabaseProvider DatabaseType, string sql, TableNameManger manger)
        {
            if (sql.StartsWith(";")) return sql.Substring(1);
            var ss = SqlSearch.Search(sql);
            if (ss.HasSelect) return sql;

            StringBuilder s = CreateSelectHeader(typeof(T), DatabaseType, ss.HasT1, manger);

            if (ss.HasFrom==false) {
                s.Append(" FROM ");
                var pd = PocoData.ForType(typeof(T));
                var fullTableName = DatabaseType.GetTableName(pd, manger);
                s.Append(fullTableName);

                if (ss.HasT1) s.Append(" AS t1");
            }
            s.Append(" ");
            s.Append(sql);
            return s.ToString();
        }
        private static StringBuilder CreateSelectHeader(Type type, DatabaseProvider DatabaseType, bool hasT1, TableNameManger manger)
        {
            var pd = PocoData.ForType(type);
            string tableName = hasT1 ? "t1" : DatabaseType.GetTableName(pd, manger);

            StringBuilder sb = new StringBuilder();
            foreach (var col in pd.Columns) {
                if (sb.Length > 0) sb.Append(",");

                if (col.ResultColumn && string.IsNullOrEmpty(col.ResultSql) == false) {
                    sb.AppendFormat(col.ResultSql, tableName + ".");
                    sb.Append(" AS '");
                    sb.Append(col.ColumnName);
                    sb.Append("'");
                } else {
                    sb.Append(tableName);
                    sb.Append(".");
                    sb.Append(DatabaseType.EscapeSqlIdentifier(col.ColumnName));
                }
            }
            if (sb.Length == 0) {
                sb.Append("NULL");
            }
            sb.Insert(0, "SELECT ");
            return sb;
        }


        public static string CreateSelectHeader(List<SelectHeader> defineHeader, List<Type> types)
        {
            var headers = GetSelectHeader(types);
            if (defineHeader != null && defineHeader.Count > 0) {
                foreach (var header in defineHeader) {
                    SelectHeader h;
                    if (string.IsNullOrEmpty(header.Table)) {
                        h = defineHeader.FirstOrDefault(q => q.AsName == header.AsName);
                    } else {
                        h = defineHeader.FirstOrDefault(q => q.AsName == header.AsName && q.Table == header.Table);
                    }
                    if (h != null) {
                        h.QuerySql = header.QuerySql;
                    } else {
                        headers.Add(h);
                    }
                }
            }
            StringBuilder sb = new StringBuilder();

            foreach (var h in headers) {
                if (sb.Length > 0) sb.Append(",");
                sb.Append(h.QuerySql);
                sb.Append(" As ");
                if (h.Table != "t1") {
                    sb.Append(h.Table);
                    sb.Append("_");
                }
                sb.Append(h.AsName);
            }
            sb.Insert(0, "SELECT ");
            return sb.ToString();
        }

        public static string CreateSelectHeader(Type outType, List<SelectHeader> defineHeader, List<Type> types)
        {
            if (defineHeader == null) defineHeader = new List<SelectHeader>();
            defineHeader.AddRange(GetSelectHeader(types));

            if (outType == typeof(object)) {
                List<string> asNames = new List<string>();
                StringBuilder sb = new StringBuilder();
                foreach (var h in defineHeader) {
                    if (sb.Length > 0) { sb.Append(","); }
                    sb.Append(h.QuerySql);
                    sb.Append(" As '");
                    sb.Append(h.AsName.Replace("'", "''"));
                    if (asNames.Contains(h.AsName)) {
                        sb.Append("_");
                        sb.Append(asNames.Count(q => q == h.AsName).ToString());
                    }
                    sb.Append("'");
                    asNames.Add(h.AsName);
                }
                sb.Insert(0, "SELECT ");
                return sb.ToString();
            } else {
                var pd = PocoData.ForType(outType);
                StringBuilder sb = new StringBuilder();
                foreach (var item in pd.Columns) {
                    var h = defineHeader.FirstOrDefault(q => q.AsName == item.ColumnName);
                    if (h != null) {
                        if (sb.Length > 0) { sb.Append(","); }
                        sb.Append(h.QuerySql);
                        sb.Append(" As '");
                        sb.Append(h.AsName.Replace("'", "''"));
                        sb.Append("'");
                    }
                }
                sb.Insert(0, "SELECT ");
                return sb.ToString();
            }
        }

        private static List<SelectHeader> GetSelectHeader(List<Type> types)
        {
            //return SelectDict.Get(types, () => {
            List<SelectHeader> list = new List<SelectHeader>();
            for (int i = 0; i < types.Count; i++) {
                var pd = PocoData.ForType(types[i]);
                foreach (var col in pd.Columns) {
                    SelectHeader header = new SelectHeader();
                    header.Table = "t" + (i + 1).ToString();
                    header.AsName = col.ColumnName;

                    if (col.ResultColumn) {
                        if (string.IsNullOrEmpty(col.ResultSql)) {
                            header.QuerySql = header.Table + "." + col.ColumnName;
                        } else {
                            header.QuerySql = string.Format(col.ResultSql, header.Table + ".");
                        }
                    } else {
                        header.QuerySql = header.Table + "." + col.ColumnName;
                    }
                    list.Add(header);
                }
            }
            return list;
            //});

        }
    }
}