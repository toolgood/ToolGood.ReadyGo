using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ToolGood.ReadyGo3.DataCentxt.Internals;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QColumn
    {
        #region operator +
        /// <summary>
        /// 重载 +
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator +(QColumn col, QColumn value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} + {1})", col, value);
        }
        /// <summary>
        /// 重载 +
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator +(QColumn col, Int16 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} + {1})", col, value);
        }
        /// <summary>
        /// 重载 +
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator +(QColumn col, Int32 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} + {1})", col, value);
        }
        /// <summary>
        /// 重载 +
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator +(QColumn col, Int64 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} + {1})", col, value);
        }
        /// <summary>
        /// 重载 +
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator +(QColumn col, UInt16 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} + {1})", col, value);
        }
        /// <summary>
        /// 重载 +
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator +(QColumn col, UInt32 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} + {1})", col, value);
        }
        /// <summary>
        /// 重载 +
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator +(QColumn col, UInt64 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} + {1})", col, value);
        }
        /// <summary>
        /// 重载 +
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator +(QColumn col, Single value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} + {1})", col, value);
        }
        /// <summary>
        /// 重载 +
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator +(QColumn col, Double value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} + {1})", col, value);
        }
        /// <summary>
        /// 重载 +
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator +(QColumn col, Decimal value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} + {1})", col, value);
        }
        /// <summary>
        /// 重载 +
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator +(QColumn col, string value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} + {1})", col, value);
        }
        #endregion

        #region operator -
        /// <summary>
        /// 重载 -
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator -(QColumn col, QColumn value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} - {1})", col, value);
        }
        /// <summary>
        /// 重载 -
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator -(QColumn col, Int16 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} - {1})", col, value);
        }
        /// <summary>
        /// 重载 -
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator -(QColumn col, Int32 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} - {1})", col, value);
        }
        /// <summary>
        /// 重载 -
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator -(QColumn col, Int64 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} - {1})", col, value);
        }
        /// <summary>
        /// 重载 -
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator -(QColumn col, UInt16 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} - {1})", col, value);
        }
        /// <summary>
        /// 重载 -
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator -(QColumn col, UInt32 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} - {1})", col, value);
        }
        /// <summary>
        /// 重载 -
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator -(QColumn col, UInt64 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} - {1})", col, value);
        }
        /// <summary>
        /// 重载 -
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator -(QColumn col, Single value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} - {1})", col, value);
        }
        /// <summary>
        /// 重载 -
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator -(QColumn col, Double value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} - {1})", col, value);
        }
        /// <summary>
        /// 重载 -
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator -(QColumn col, Decimal value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "({0} - {1})", col, value);
        }

        #endregion

        #region operator *
        /// <summary>
        /// 重载 *
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator *(QColumn col, QColumn value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} * {1}", col, value);
        }
        /// <summary>
        /// 重载 *
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator *(QColumn col, Int16 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} * {1}", col, value);
        }
        /// <summary>
        /// 重载 *
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator *(QColumn col, Int32 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} * {1}", col, value);
        }
        /// <summary>
        /// 重载 *
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator *(QColumn col, Int64 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} * {1}", col, value);
        }
        /// <summary>
        /// 重载 *
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator *(QColumn col, UInt16 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} * {1}", col, value);
        }
        /// <summary>
        /// 重载 *
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator *(QColumn col, UInt32 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} * {1}", col, value);
        }
        /// <summary>
        /// 重载 *
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator *(QColumn col, UInt64 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} * {1}", col, value);
        }
        /// <summary>
        /// 重载 *
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator *(QColumn col, Single value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} * {1}", col, value);
        }
        /// <summary>
        /// 重载 *
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator *(QColumn col, Double value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} * {1}", col, value);
        }
        /// <summary>
        /// 重载 *
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator *(QColumn col, Decimal value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} * {1}", col, value);
        }

        #endregion

        #region operator /
        /// <summary>
        /// 重载 /
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator /(QColumn col, QColumn value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} / {1}", col, value);
        }
        /// <summary>
        /// 重载 /
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator /(QColumn col, Int16 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} / {1}", col, value);
        }
        /// <summary>
        /// 重载 /
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator /(QColumn col, Int32 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} / {1}", col, value);
        }
        /// <summary>
        /// 重载 /
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator /(QColumn col, Int64 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} / {1}", col, value);
        }
        /// <summary>
        /// 重载 /
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator /(QColumn col, UInt16 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} / {1}", col, value);
        }
        /// <summary>
        /// 重载 /
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator /(QColumn col, UInt32 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} / {1}", col, value);
        }
        /// <summary>
        /// 重载 /
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator /(QColumn col, UInt64 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} / {1}", col, value);
        }
        /// <summary>
        /// 重载 /
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator /(QColumn col, Single value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} / {1}", col, value);
        }
        /// <summary>
        /// 重载 /
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator /(QColumn col, Double value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} / {1}", col, value);
        }
        /// <summary>
        /// 重载 /
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator /(QColumn col, Decimal value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "{0} / {1}", col, value);
        }

        #endregion

        #region operator %
        /// <summary>
        /// 重载 %
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator %(QColumn col, QColumn value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "MOD({0}, {1})", col, value);
        }
        /// <summary>
        /// 重载 %
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator %(QColumn col, Int16 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "MOD({0}, {1})", col, value);
        }
        /// <summary>
        /// 重载 %
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator %(QColumn col, Int32 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "MOD({0}, {1})", col, value);
        }
        /// <summary>
        /// 重载 %
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator %(QColumn col, Int64 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "MOD({0}, {1})", col, value);
        }
        /// <summary>
        /// 重载 %
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator %(QColumn col, UInt16 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "MOD({0}, {1})", col, value);
        }
        /// <summary>
        /// 重载 %
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator %(QColumn col, UInt32 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "MOD({0}, {1})", col, value);
        }
        /// <summary>
        /// 重载 %
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator %(QColumn col, UInt64 value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "MOD({0}, {1})", col, value);
        }
        /// <summary>
        /// 重载 %
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator %(QColumn col, Single value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "MOD({0}, {1})", col, value);
        }
        /// <summary>
        /// 重载 %
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator %(QColumn col, Double value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "MOD({0}, {1})", col, value);
        }
        /// <summary>
        /// 重载 %
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QSqlColumn operator %(QColumn col, Decimal value)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, "MOD({0}, {1})", col, value);
        }

        #endregion

        #region operator Null
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="Null"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, DBNull Null)
        {
            return new QColumnValueCondition(col, " IS NULL");
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="Null"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, DBNull Null)
        {
            return new QColumnValueCondition(col, " IS NOT NULL");
        }
        #endregion

        #region QJoinCondition operator == operator !=
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        public static QJoinCondition operator ==(QColumn col1, QColumn col2)
        {
            return new QJoinCondition(col1, "=", col2);
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        public static QJoinCondition operator !=(QColumn col1, QColumn col2)
        {
            return new QJoinCondition(col1, "<>", col2);
        }
        #endregion

        #region operator ==
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, bool value)
        {
            return new QColumnValueCondition(col, "=", value);
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, Int16 value)
        {
            return new QColumnValueCondition(col, "=", value);
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, Int32 value)
        {
            return new QColumnValueCondition(col, "=", value);
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, Int64 value)
        {
            return new QColumnValueCondition(col, "=", value);
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, UInt16 value)
        {
            return new QColumnValueCondition(col, "=", value);
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, UInt32 value)
        {
            return new QColumnValueCondition(col, "=", value);
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, UInt64 value)
        {
            return new QColumnValueCondition(col, "=", value);
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, Single value)
        {
            return new QColumnValueCondition(col, "=", value);
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, Double value)
        {
            return new QColumnValueCondition(col, "=", value);
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, Decimal value)
        {
            return new QColumnValueCondition(col, "=", value);
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, string value)
        {
            if (value == null) {
                return new QColumnValueCondition(col, "IS NULL");
            }
            return new QColumnValueCondition(col, "=", EscapeParam(value));
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, char value)
        {
            return new QColumnValueCondition(col, "=", EscapeParam(value.ToString()));
        }
        /// <summary>
        /// 重载 ==
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator ==(QColumn col, DateTime value)
        {
            return new QColumnValueCondition(col, "=", value);
        }
        #endregion

        #region operator !=
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, bool value)
        {
            return new QColumnValueCondition(col, "<>", value);
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, Int16 value)
        {
            return new QColumnValueCondition(col, "<>", value);
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, Int32 value)
        {
            return new QColumnValueCondition(col, "<>", value);
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, Int64 value)
        {
            return new QColumnValueCondition(col, "<>", value);
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, UInt16 value)
        {
            return new QColumnValueCondition(col, "<>", value);
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, UInt32 value)
        {
            return new QColumnValueCondition(col, "<>", value);
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, UInt64 value)
        {
            return new QColumnValueCondition(col, "<>", value);
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, Single value)
        {
            return new QColumnValueCondition(col, "<>", value);
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, Double value)
        {
            return new QColumnValueCondition(col, "<>", value);
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, Decimal value)
        {
            return new QColumnValueCondition(col, "<>", value);
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, string value)
        {
            if (value == null) {
                return new QColumnValueCondition(col, "IS NOT NULL");
            }
            return new QColumnValueCondition(col, "<>", EscapeParam(value));
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, char value)
        {
            return new QColumnValueCondition(col, "<>", EscapeParam(value.ToString()));
        }
        /// <summary>
        /// 重载 !=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator !=(QColumn col, DateTime value)
        {
            return new QColumnValueCondition(col, "<>", value);
        }
        #endregion

        #region operator >
        /// <summary>
        /// 重载 >
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >(QColumn col, QColumn value)
        {
            return new QColumnValueCondition(col, ">", value);
        }
        /// <summary>
        /// 重载 >
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >(QColumn col, Int16 value)
        {
            return new QColumnValueCondition(col, ">", value);
        }
        /// <summary>
        /// 重载 >
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >(QColumn col, Int32 value)
        {
            return new QColumnValueCondition(col, ">", value);
        }
        /// <summary>
        /// 重载 >
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >(QColumn col, Int64 value)
        {
            return new QColumnValueCondition(col, ">", value);
        }
        /// <summary>
        /// 重载 >
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >(QColumn col, UInt16 value)
        {
            return new QColumnValueCondition(col, ">", value);
        }
        /// <summary>
        /// 重载 >
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >(QColumn col, UInt32 value)
        {
            return new QColumnValueCondition(col, ">", value);
        }
        /// <summary>
        /// 重载 >
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >(QColumn col, UInt64 value)
        {
            return new QColumnValueCondition(col, ">", value);
        }
        /// <summary>
        /// 重载 >
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >(QColumn col, Single value)
        {
            return new QColumnValueCondition(col, ">", value);
        }
        /// <summary>
        /// 重载 >
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >(QColumn col, Double value)
        {
            return new QColumnValueCondition(col, ">", value);
        }
        /// <summary>
        /// 重载 >
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >(QColumn col, Decimal value)
        {
            return new QColumnValueCondition(col, ">", value);
        }
        /// <summary>
        /// 重载 >
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >(QColumn col, DateTime value)
        {
            return new QColumnValueCondition(col, ">", value);
        }
 
        #endregion

        #region operator >=
        /// <summary>
        /// 重载 >=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >=(QColumn col, QColumn value)
        {
            return new QColumnValueCondition(col, ">=", value);
        }
        /// <summary>
        /// 重载 >=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >=(QColumn col, Int16 value)
        {
            return new QColumnValueCondition(col, ">=", value);
        }
        /// <summary>
        /// 重载 >=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >=(QColumn col, Int32 value)
        {
            return new QColumnValueCondition(col, ">=", value);
        }
        /// <summary>
        /// 重载 >=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >=(QColumn col, Int64 value)
        {
            return new QColumnValueCondition(col, ">=", value);
        }
        /// <summary>
        /// 重载 >=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >=(QColumn col, UInt16 value)
        {
            return new QColumnValueCondition(col, ">=", value);
        }
        /// <summary>
        /// 重载 >=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >=(QColumn col, UInt32 value)
        {
            return new QColumnValueCondition(col, ">=", value);
        }
        /// <summary>
        /// 重载 >=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >=(QColumn col, UInt64 value)
        {
            return new QColumnValueCondition(col, ">=", value);
        }
        /// <summary>
        /// 重载 >=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >=(QColumn col, Single value)
        {
            return new QColumnValueCondition(col, ">=", value);
        }
        /// <summary>
        /// 重载 >=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >=(QColumn col, Double value)
        {
            return new QColumnValueCondition(col, ">=", value);
        }
        /// <summary>
        /// 重载 >=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >=(QColumn col, Decimal value)
        {
            return new QColumnValueCondition(col, ">=", value);
        }
        /// <summary>
        /// 重载 >=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator >=(QColumn col, DateTime value)
        {
            return new QColumnValueCondition(col, ">=", value);
        }

        #endregion

        #region operator <
        /// <summary>
        /// 重载 &lt;
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <(QColumn col, QColumn value)
        {
            return new QColumnValueCondition(col, "<", value);
        }
        /// <summary>
        /// 重载 &lt;
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <(QColumn col, Int16 value)
        {
            return new QColumnValueCondition(col, "<", value);
        }
        /// <summary>
        /// 重载 &lt;
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <(QColumn col, Int32 value)
        {
            return new QColumnValueCondition(col, "<", value);
        }
        /// <summary>
        /// 重载 &lt;
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <(QColumn col, Int64 value)
        {
            return new QColumnValueCondition(col, "<", value);
        }
        /// <summary>
        /// 重载 &lt;
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <(QColumn col, UInt16 value)
        {
            return new QColumnValueCondition(col, "<", value);
        }
        /// <summary>
        /// 重载 &lt;
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <(QColumn col, UInt32 value)
        {
            return new QColumnValueCondition(col, "<", value);
        }
        /// <summary>
        /// 重载 &lt;
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <(QColumn col, UInt64 value)
        {
            return new QColumnValueCondition(col, "<", value);
        }
        /// <summary>
        /// 重载 &lt;
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <(QColumn col, Single value)
        {
            return new QColumnValueCondition(col, "<", value);
        }
        /// <summary>
        /// 重载 &lt;
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <(QColumn col, Double value)
        {
            return new QColumnValueCondition(col, "<", value);
        }
        /// <summary>
        /// 重载 &lt;
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <(QColumn col, Decimal value)
        {
            return new QColumnValueCondition(col, "<", value);
        }
        /// <summary>
        /// 重载 &lt;
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <(QColumn col, DateTime value)
        {
            return new QColumnValueCondition(col, "<", value);
        }

        #endregion

        #region operator <=
        /// <summary>
        /// 重载 &lt;=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <=(QColumn col, QColumn value)
        {
            return new QColumnValueCondition(col, "<=", value);
        }
        /// <summary>
        /// 重载 &lt;=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <=(QColumn col, Int16 value)
        {
            return new QColumnValueCondition(col, "<=", value);
        }
        /// <summary>
        /// 重载 &lt;=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <=(QColumn col, Int32 value)
        {
            return new QColumnValueCondition(col, "<=", value);
        }
        /// <summary>
        /// 重载 &lt;=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <=(QColumn col, Int64 value)
        {
            return new QColumnValueCondition(col, "<=", value);
        }
        /// <summary>
        /// 重载 &lt;=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <=(QColumn col, UInt16 value)
        {
            return new QColumnValueCondition(col, "<=", value);
        }
        /// <summary>
        /// 重载 &lt;=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <=(QColumn col, UInt32 value)
        {
            return new QColumnValueCondition(col, "<=", value);
        }
        /// <summary>
        /// 重载 &lt;=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <=(QColumn col, UInt64 value)
        {
            return new QColumnValueCondition(col, "<=", value);
        }
        /// <summary>
        /// 重载 &lt;=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <=(QColumn col, Single value)
        {
            return new QColumnValueCondition(col, "<=", value);
        }
        /// <summary>
        /// 重载 &lt;=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <=(QColumn col, Double value)
        {
            return new QColumnValueCondition(col, "<=", value);
        }
        /// <summary>
        /// 重载 &lt;=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <=(QColumn col, Decimal value)
        {
            return new QColumnValueCondition(col, "<=", value);
        }
        /// <summary>
        /// 重载 &lt;=
        /// </summary>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QCondition operator <=(QColumn col, DateTime value)
        {
            return new QColumnValueCondition(col, "<=", value);
        }
 
        #endregion


    }
}
