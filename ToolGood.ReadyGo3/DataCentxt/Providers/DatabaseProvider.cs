using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Enums;

using ToolGood.ReadyGo3.PetaPoco.Internal;
using ToolGood.ReadyGo3.DataCentxt.Providers;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;

namespace ToolGood.ReadyGo3.DataCentxt
{
    public class DatabaseProvider
    {
        protected bool usedEscapeSql = false;
        protected char escapeSql = '`';

        public virtual string EscapeSqlIdentifier(string sqlIdentifier)
        {
            return $"[{sqlIdentifier}]";
        }
 
        public virtual string CreateFunction(SqlFunction function, params object[] args)
        {
            switch (function) {
                case SqlFunction.Len: return CreateFunction("LEN({0})", args);
                case SqlFunction.Max: return CreateFunction("MAX({0})", args);
                case SqlFunction.Min: return CreateFunction("MIN({0})", args);
                case SqlFunction.Avg: return CreateFunction("AVG({0})", args);
                case SqlFunction.Sum: return CreateFunction("SUM({0})", args);
                case SqlFunction.Count: return CreateFunction("COUNT({0})", args);
                case SqlFunction.CountDistinct: return CreateFunction("COUNT(DISTINCT {0})", args);
                case SqlFunction.DatePart: return CreateFunction("DATEPART({0},{1})", args);
                case SqlFunction.DateDiff: return CreateFunction("DATEDIFF({0},{1})", args);
                case SqlFunction.Year: return CreateFunction("YEAR({0})", args);
                case SqlFunction.Month: return CreateFunction("MONTH({0})", args);
                case SqlFunction.Day: return CreateFunction("DAY({0})", args);
                case SqlFunction.Hour: return CreateFunction("HOUR({0})", args);
                case SqlFunction.Minute: return CreateFunction("MINUTE({0})", args);
                case SqlFunction.Second: return CreateFunction("SECOND({0})", args);
                case SqlFunction.DayOfYear: return CreateFunction("DAYOFYEAR({0})", args);
                case SqlFunction.Week: return CreateFunction("WEEK({0})", args);
                case SqlFunction.WeekDay: return CreateFunction("WEEKDAY({0})", args);
                case SqlFunction.SubString3: return CreateFunction("SUBSTRING({0},{1},{2})", args);
                case SqlFunction.SubString2: return CreateFunction("SUBSTRING({0},{1})", args);
                case SqlFunction.Left: return CreateFunction("LEFT({0},{1})", args);
                case SqlFunction.Right: return CreateFunction("RIGHT({0},{1})", args);
                case SqlFunction.Lower: return CreateFunction("LOWER({0})", args);
                case SqlFunction.Upper: return CreateFunction("UPPER({0})", args);
                case SqlFunction.Ascii: return CreateFunction("ASCII({0})", args);
                case SqlFunction.Concat:
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("CONCAT(");
                    foreach (var item in args) {
                        stringBuilder.Append(EscapeParam(item));
                        stringBuilder.Append(',');
                    }
                    stringBuilder[stringBuilder.Length - 1] = ')';
                    return stringBuilder.ToString();
                default: break;
            }
            return CreateFunction(args[0].ToString(), args, 1);
        }

        protected string CreateFunction(string sql, object[] args, int index = 0)
        {
            List<string> list = new List<string>();
            for (int i = index; i < args.Length; i++) {
                list.Add(EscapeParam(args[i]));
            }
            return string.Format(sql, list);
        }


        public virtual string Delete(List<QTable> tables, QColumn pk, string tableName, string fromtable, string jointables, string where)
        {
            if (object.Equals(pk, null)) throw new NoPrimaryKeyException();
            var pk1 = pk.ToSql(this, 1);
            var pk2 = pk.ToSql(this, tables.Count);
            return $"DELETE {tableName} WHERE {pk1} IN (SELECT {pk2} FROM {fromtable} {jointables} WHERE {where});";
        }

        public virtual string Update(List<QTable> tables, string setValues, string fromtable, string jointables, string where)
        {
            throw new DatabaseUnsupportException();
        }

        public virtual string Select(List<QTable> tables, bool useDistinct, int limit, int offset, List<string> selectColumns, string fromtable,
            string jointables, string where, string order, string group, string having)
        {
            throw new DatabaseUnsupportException();
        }



        /// <summary>
        /// 格式SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string FormatSql(string sql, object[] args)
        {
            if (args == null || args.Length == 0) {
                return sql;
            } else if (sql.Contains("@") == false) {
                return sql;
            }

            StringBuilder _where = new StringBuilder();

            bool isInText = false, isStart = false, isInTableColumn = false;
            var c = 'a';
            var text = "";

            for (int i = 0; i < sql.Length; i++) {
                var t = sql[i];
                if (isInText) {
                    if (t == c) isInText = false;
                } else if (isInTableColumn) {
                    if (t == ']') {
                        isInTableColumn = false;
                        if (usedEscapeSql) {
                            _where.Append(escapeSql);
                            continue;
                        }
                    }
                } else if (t == '[') {
                    isInTableColumn = true;
                    if (usedEscapeSql) {
                        _where.Append(escapeSql);
                        continue;
                    }
                } else if ("\"'`".Contains(t)) {
                    isInText = true;
                    c = t;
                    isStart = false;
                } else if (isStart == false) {
                    if (t == '@') {
                        isStart = true;
                        text = "@";
                        continue;
                    }
                } else if ("@1234567890".Contains(t)) {
                    text += t;
                    continue;
                } else {
                    whereTranslate(_where, text, args);
                    isStart = false;
                }
                _where.Append(t);
            }
            if (isStart) whereTranslate(_where, text, args);

            return _where.ToString();
        }
        private void whereTranslate(StringBuilder where, string text, object[] args)
        {
            int p = int.Parse(text.Replace("@", ""));
            var value = args[p];
            if (value is QColumn) {
                var sb = ((QColumn)value).GetSqlBuilder();
                if (sb == null) {
                    throw new ArgumentNullException();
                }
                where.Append(((QColumn)value).ToSql(this, sb._tables.Count));
            } else if (value is ICollection) {
                var v = (ICollection)value;
                if (v.Count == 0) {
                    where.Append("(Null)");
                    where.Append(" AND 1=2 ");
                } else {
                    where.Append("(");
                    foreach (var item in (ICollection)value) {
                        where.Append(EscapeParam(item));
                        where.Append(",");
                    }
                    where[where.Length - 1] = ')';
                }
            } else {
                where.Append(EscapeParam(value));
            }
        }



        /// <summary>
        /// 转化成SQL语言的片段，value不能为Null.
        /// 
        /// </summary>
        /// <param name="sqlType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string EscapeParam(object value)
        {
            if (object.Equals(value, null)) return "NULL";
            if (value is QColumn) {
                var sb = ((QColumn)value).GetSqlBuilder();
                if (sb == null) {
                    throw new ArgumentNullException();
                }
                return (((QColumn)value).ToSql(this, sb._tables.Count));
            }
            var fieldType = value.GetType();
            if (fieldType.IsEnum) {
                var isEnumFlags = fieldType.IsEnum;
                long enumValue;
                if (!isEnumFlags && Int64.TryParse(value.ToString(), out enumValue)) {
                    value = Enum.ToObject(fieldType, enumValue).ToString();
                }
                var enumString = value.ToString();
                return !isEnumFlags ? "'" + enumString.Trim('"') + "'" : enumString;
            }

            var typeCode = Type.GetTypeCode(fieldType);
            switch (typeCode) {
                case TypeCode.Boolean: return (bool)value ? "1" : "0";
                case TypeCode.Single: return ((float)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Double: return ((double)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Decimal: return ((decimal)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64: return value.ToString();
                default: break;
            }
            if (value is string || value is char) {
                var txt = (value.ToString()).Replace(@"\", @"\\").Replace("'", @"\'");
                return "'" + txt + "'";
            }
            if (fieldType == typeof(DateTime)) return "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            if (fieldType == typeof(TimeSpan)) return ((TimeSpan)value).Ticks.ToString(CultureInfo.InvariantCulture);
            if (fieldType == typeof(byte[])) {
                var txt = Encoding.BigEndianUnicode.GetString((byte[])value);
                return "'" + txt + "'";
            }
            return "'" + value.ToString() + "'";
        }


        /// <summary>
        /// 格式化 LIKE 内容，适用于Contains，StartWith，EndWith
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string EscapeLikeParam(string param)
        {
            return param.Replace(@"\", @"\\")
                .Replace("_", @"\_")
                .Replace("%", @"\%")
                .Replace("'", @"\'")
                .Replace("[", @"\[")
                .Replace("]", @"\]");
        }
        /// <summary>
        /// 格式化 LIKE 内容2，适用于Default
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string EscapeLikeParam2(string param)
        {
            param = param.Replace(@"\\", @"\").Replace("''", "'").Replace(@"\'", "'");
            return param.Replace(@"\", @"\\").Replace("'", @"\'");
        }

        internal static DatabaseProvider Resolve(SqlType type)
        {
            switch (type) {
                case SqlType.SqlServer: return Singleton<SqlServerDatabaseProvider>.Instance;
                case SqlType.MySql: return Singleton<MySqlDatabaseProvider>.Instance;
                case SqlType.SQLite: return Singleton<SQLiteDatabaseProvider>.Instance;
                case SqlType.MsAccessDb: return Singleton<MsAccessDbDatabaseProvider>.Instance;
                case SqlType.Oracle: return Singleton<OracleDatabaseProvider>.Instance;
                case SqlType.PostgreSQL: return Singleton<PostgreSQLDatabaseProvider>.Instance;
                case SqlType.FirebirdDb: return Singleton<FirebirdDbDatabaseProvider>.Instance;
                case SqlType.MariaDb: return Singleton<MySqlDatabaseProvider>.Instance;
                case SqlType.SqlServerCE: return Singleton<SqlServerCEDatabaseProviders>.Instance;
                case SqlType.SqlServer2012: return Singleton<SqlServer2012DatabaseProvider>.Instance;
                default: break;
            }
            return Singleton<SqlServerDatabaseProvider>.Instance;
        }
    }
}
