using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
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

        public virtual bool IsFunctionUseDefaultFormat(string funName)
        {
            return true;
        }

        public virtual string GetFunctionFormat(string funName)
        {
            return null;
        }

        public virtual string Delete(List<QTable> tables, QColumnBase pk, string tableName, string fromtable, string jointables, string where)
        {
            if (object.Equals(pk, null)) throw new NoPrimaryKeyException();
            var pk1 = ((IColumnConvert) pk).ToSql(this, 1);
            var pk2 = ((IColumnConvert)pk).ToSql(this, tables.Count);
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
            if (value is QColumnBase) {
                var sb = ((IColumnConvert)value).GetSqlBuilder();
                if (sb == null) {
                    throw new ArgumentNullException();
                }
                where.Append(((IColumnConvert)value).ToSql(this, sb._tables.Count));
            } else {
                where.Append(ConvertTo(value));
            }
        }

        /// <summary>
        /// 转化成SQL语言的片段，value不能为Null.
        /// 
        /// </summary>
        /// <param name="sqlType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ConvertTo(object value)
        {
            if (object.Equals(value, null)) return "NULL";
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
            if (fieldType == typeof(DateTime)) return "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (fieldType == typeof(TimeSpan)) return ((TimeSpan)value).Ticks.ToString(CultureInfo.InvariantCulture);
            if (fieldType == typeof(byte[])) {
                var txt = Encoding.BigEndianUnicode.GetString((byte[])value);
                return "'" + txt + "'";
            }
            return "'" + value.ToString() + "'";
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
