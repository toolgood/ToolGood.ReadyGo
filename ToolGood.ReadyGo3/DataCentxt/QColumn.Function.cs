using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;

using ToolGood.ReadyGo3.DataCentxt.Internals;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QColumn
    {
        /// <summary>
        /// In （Select * ）语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public QCondition InSelect(string sql, params object[] args)
        {
            if (_columnType != Enums.ColumnType.Column) throw new ColumnTypeException();

            var code = _table.GetSqlBuilder().Provider.FormatSql(sql, args);
            if (code.StartsWith("(")) {
                return new QColumnValueCondition(this, "IN " + code);
            }
            return new QColumnValueCondition(this, "IN (" + code + ")");
        }
        /// <summary>
        /// NOT In （Select * ）语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public QCondition NotInSelect(string sql, params object[] args)
        {
            if (_columnType != Enums.ColumnType.Column) throw new ColumnTypeException();

            var code = _table.GetSqlBuilder().Provider.FormatSql(sql, args);
            if (code.StartsWith("(")) {
                return new QColumnValueCondition(this, "NOT IN " + code);
            }
            return new QColumnValueCondition(this, "NOT IN (" + code + ")");
        }
        /// <summary>
        /// IS NULL 语句
        /// </summary>
        /// <returns></returns>
        public QCondition IsNull()
        {
            if (_columnType != Enums.ColumnType.Column) throw new ColumnTypeException();
            return new QColumnValueCondition(this, "IS NULL");
        }
        /// <summary>
        /// IS NOT NULL 语句
        /// </summary>
        /// <returns></returns>
        public QCondition IsNotNull()
        {
            if (_columnType != Enums.ColumnType.Column) throw new ColumnTypeException();
            return new QColumnValueCondition(this, "IS NOT NULL");
        }
        /// <summary>
        /// In语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(params string[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", EscapeParam(array[0]));
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("IN (");
            for (int i = 0; i < array.Length; i++) {
                stringBuilder.Append("'");
                stringBuilder.Append(EscapeParam(array[i]));
                stringBuilder.Append("',");
            }
            stringBuilder[stringBuilder.Length - 1] = ')';
            return new QColumnValueCondition(this, stringBuilder.ToString());
        }
        /// <summary>
        /// In语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(IList<string> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", EscapeParam(array[0]));
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("IN (");
            for (int i = 0; i < array.Count; i++) {
                stringBuilder.Append("'");
                stringBuilder.Append(EscapeParam(array[i]));
                stringBuilder.Append("',");
            }
            stringBuilder[stringBuilder.Length - 1] = ')';
            return new QColumnValueCondition(this, stringBuilder.ToString());
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(params string[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "<>", EscapeParam(array[0]));
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("NOT IN (");
            for (int i = 0; i < array.Length; i++) {
                stringBuilder.Append("'");
                stringBuilder.Append(EscapeParam(array[i]));
                stringBuilder.Append("',");
            }
            stringBuilder[stringBuilder.Length - 1] = ')';
            return new QColumnValueCondition(this, stringBuilder.ToString());
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(IList<string> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "<>", EscapeParam(array[0]));
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("NOT IN (");
            for (int i = 0; i < array.Count; i++) {
                stringBuilder.Append("'");
                stringBuilder.Append(EscapeParam(array[i]));
                stringBuilder.Append("',");
            }
            stringBuilder[stringBuilder.Length - 1] = ')';
            return new QColumnValueCondition(this, stringBuilder.ToString());
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(params short[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(IList<short> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(params short[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(IList<short> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(params int[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(IList<int> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(params int[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(IList<int> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(params long[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(IList<long> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(params long[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(IList<long> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(params ushort[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(IList<ushort> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(params ushort[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(IList<ushort> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(params uint[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(IList<uint> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(params uint[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(IList<uint> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(params ulong[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(IList<ulong> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(params ulong[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(IList<ulong> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(params float[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(IList<float> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(params float[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(IList<float> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(params double[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(IList<double> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(params double[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(IList<double> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(params decimal[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition In(IList<decimal> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(params decimal[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// NOT IN语句
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public QCondition NotIn(IList<decimal> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "<>", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }
        /// <summary>
        /// LIKE语句
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public QCondition Like(string txt, LikeType type = LikeType.Default)
        {
            if (txt == null) return QCondition.False;
            switch (type) {
                case LikeType.Default:
                    return new QColumnValueCondition(this, "LIKE '" + EscapeLikeParam2(txt) + "'");
                case LikeType.StartWith:
                    return new QColumnValueCondition(this, "LIKE '%" + EscapeLikeParam(txt) + "'");
                case LikeType.EndWith:
                    return new QColumnValueCondition(this, "LIKE '" + EscapeLikeParam(txt) + "%'");
                case LikeType.Contains:
                default:
                    return new QColumnValueCondition(this, "LIKE '%" + EscapeLikeParam(txt) + "%'");
            }
        }
        /// <summary>
        /// NOT LIKE语句
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public QCondition NotLike(string txt, LikeType type = LikeType.Default)
        {
            if (txt == null) return QCondition.False;
            switch (type) {
                case LikeType.Default:
                    return new QColumnValueCondition(this, "NOT LIKE '" + EscapeLikeParam2(txt) + "'");
                case LikeType.StartWith:
                    return new QColumnValueCondition(this, "NOT LIKE '%" + EscapeLikeParam(txt) + "'");
                case LikeType.EndWith:
                    return new QColumnValueCondition(this, "NOT LIKE '" + EscapeLikeParam(txt) + "%'");
                case LikeType.Contains:
                default:
                    return new QColumnValueCondition(this, "NOT LIKE '%" + EscapeLikeParam(txt) + "%'");
            }
        }
        /// <summary>
        /// LIKE语句 Contains
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public QCondition Contains(string txt)
        {
            if (txt == null) return QCondition.False;
            return new QColumnValueCondition(this, "LIKE '%" + EscapeLikeParam(txt) + "%'");
        }
        /// <summary>
        /// LIKE语句 StartsWith
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public QCondition StartsWith(string txt)
        {
            if (txt == null) return QCondition.False;
            return new QColumnValueCondition(this, "LIKE '" + EscapeLikeParam(txt) + "%'");
        }
        /// <summary>
        /// LIKE语句 EndsWith
        /// </summary> 
        /// <param name="txt"></param>
        /// <returns></returns>
        public QCondition EndsWith(string txt)
        {
            if (txt == null) return QCondition.False;
            return new QColumnValueCondition(this, "LIKE '%" + EscapeLikeParam(txt) + "'");
        }

        internal static string EscapeParam(string param)
        {
            return param.Replace(@"\", @"\\").Replace("'", @"\'");
        }

        internal static string EscapeLikeParam(string param)
        {
            return param.Replace(@"\", @"\\")
                .Replace("_", @"\_")
                .Replace("%", @"\%")
                .Replace("'", @"\'")
                .Replace("[", @"\[")
                .Replace("]", @"\]");
        }

        internal string EscapeLikeParam2(string param)
        {
            param = param.Replace(@"\\", @"\").Replace("''", "'").Replace(@"\'", "'");
            return param.Replace(@"\", @"\\").Replace("'", @"\'");
        }
    }
}
