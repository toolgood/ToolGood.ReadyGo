using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
using ToolGood.ReadyGo3.DataCentxt.Internals;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QColumnBase
    {
        public QCondition InSelect(string sql, params object[] args)
        {
            if (_columnType != Enums.ColumnType.Column) throw new ColumnTypeException();

            var code = _table.GetSqlBuilder().Provider.FormatSql(sql, args);
            if (code.StartsWith("(")) {
                return new QColumnValueCondition(this, "IN " + code);
            }
            return new QColumnValueCondition(this, "IN (" + code + ")");
        }

        public QCondition IsNull()
        {
            if (_columnType != Enums.ColumnType.Column) throw new ColumnTypeException();
            return new QColumnValueCondition(this, "IS NULL");
        }

        public QCondition IsNotNull()
        {
            if (_columnType != Enums.ColumnType.Column) throw new ColumnTypeException();
            return new QColumnValueCondition(this, "IS NOT NULL");
        }


        public QCondition In(params string[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN ('" + string.Join("','", array) + "')");
        }

        public QCondition In(IList<string> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN ('" + string.Join("','", array) + "')");
        }

        public QCondition UnIn(params string[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition UnIn(IList<string> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition In(params short[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition In(IList<short> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition UnIn(params short[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition UnIn(IList<short> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition In(params int[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition In(IList<int> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition UnIn(params int[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition UnIn(IList<int> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition In(params long[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition In(IList<long> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition UnIn(params long[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition UnIn(IList<long> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition In(params ushort[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition In(IList<ushort> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition UnIn(params ushort[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition UnIn(IList<ushort> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition In(params uint[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition In(IList<uint> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition UnIn(params uint[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition UnIn(IList<uint> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition In(params ulong[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition In(IList<ulong> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition UnIn(params ulong[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition UnIn(IList<ulong> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition In(params float[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition In(IList<float> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition UnIn(params float[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition UnIn(IList<float> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition In(params double[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition In(IList<double> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition UnIn(params double[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition UnIn(IList<double> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition In(params decimal[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition In(IList<decimal> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "IN (" + string.Join(",", array) + ")");
        }

        public QCondition UnIn(params decimal[] array)
        {
            if (array.Length == 0) return QCondition.False;
            if (array.Length == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition UnIn(IList<decimal> array)
        {
            if (array.Count == 0) return QCondition.False;
            if (array.Count == 1) return new QColumnValueCondition(this, "=", array[0]);
            return new QColumnValueCondition(this, "NOT IN ('" + string.Join("','", array) + "')");
        }

        public QCondition Like(string txt)
        {
            return new QColumnValueCondition(this, "LIKE '" + txt + "'");
        }

        public QCondition UnLike(string txt)
        {
            return new QColumnValueCondition(this, "NOT LIKE '" + txt + "'");
        }

        public QCondition Contains(string txt)
        {
            return new QColumnValueCondition(this, "LIKE '%" + EscapeParam(txt) + "%'");
        }

        public QCondition StartsWith(string txt)
        {
            return new QColumnValueCondition(this, "LIKE '" + EscapeParam(txt) + "%'");
        }

        public QCondition EndsWith(string txt)
        {
            return new QColumnValueCondition(this, "LIKE '%" + EscapeParam(txt) + "'");
        }

        internal string EscapeParam(string param)
        {
            return param.Replace(@"\", @"\\\\")
                .Replace("_", @"\_")
                .Replace("%", @"\%")
                .Replace("'", @"\'")
                .Replace("[", @"\[")
                .Replace("]", @"\]");
        }
    }
}
