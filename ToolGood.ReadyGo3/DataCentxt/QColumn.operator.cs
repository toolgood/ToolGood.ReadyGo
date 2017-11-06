using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
using ToolGood.ReadyGo3.DataCentxt.Internals;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QColumnBase
    {
        protected static QColumn CreateFunctionColumn2(string funName, string funFormat, params object[] args)
        {
            QColumn column = new QColumn();
            column._columnType = Enums.ColumnType.Function;
            column._functionName = funName;
            column._functionFormat = funFormat;
            column._functionArgs = args;
            return column;
        }
        protected static QCondition CreateCondition(QColumnBase col1, string op, QColumnBase col2)
        {
            return new QColumnColumnCondition(col1, op, col2);
        }
        protected static QCondition CreateCondition(QColumnBase col1, string op, object col2)
        {
            return new QColumnValueCondition(col1, op, col2);
        }

        #region operator +
        public static QColumn operator +(QColumnBase col, QColumn value)
        {
            return CreateFunctionColumn2("+", "({0} + {1})", col, value);
        }
        public static QColumn operator +(QColumnBase col, Int16 value)
        {
            return CreateFunctionColumn2("+", "({0} + {1})", col, value);
        }
        public static QColumn operator +(QColumnBase col, Int32 value)
        {
            return CreateFunctionColumn2("+", "({0} + {1})", col, value);
        }
        public static QColumn operator +(QColumnBase col, Int64 value)
        {
            return CreateFunctionColumn2("+", "({0} + {1})", col, value);
        }
        public static QColumn operator +(QColumnBase col, UInt16 value)
        {
            return CreateFunctionColumn2("+", "({0} + {1})", col, value);
        }
        public static QColumn operator +(QColumnBase col, UInt32 value)
        {
            return CreateFunctionColumn2("+", "({0} + {1})", col, value);
        }
        public static QColumn operator +(QColumnBase col, UInt64 value)
        {
            return CreateFunctionColumn2("+", "({0} + {1})", col, value);
        }
        public static QColumn operator +(QColumnBase col, Single value)
        {
            return CreateFunctionColumn2("+", "({0} + {1})", col, value);
        }
        public static QColumn operator +(QColumnBase col, Double value)
        {
            return CreateFunctionColumn2("+", "({0} + {1})", col, value);
        }
        public static QColumn operator +(QColumnBase col, Decimal value)
        {
            return CreateFunctionColumn2("+", "({0} + {1})", col, value);
        }
        public static QColumn operator +(QColumnBase col, string value)
        {
            return CreateFunctionColumn2("+", "({0} + {1})", col, value);
        }
        #endregion

        #region operator -
        public static QColumn operator -(QColumnBase col, QColumn value)
        {
            return CreateFunctionColumn2("-", "({0} - {1})", col, value);
        }
        public static QColumn operator -(QColumnBase col, Int16 value)
        {
            return CreateFunctionColumn2("-", "({0} - {1})", col, value);
        }
        public static QColumn operator -(QColumnBase col, Int32 value)
        {
            return CreateFunctionColumn2("-", "({0} - {1})", col, value);
        }
        public static QColumn operator -(QColumnBase col, Int64 value)
        {
            return CreateFunctionColumn2("-", "({0} - {1})", col, value);
        }
        public static QColumn operator -(QColumnBase col, UInt16 value)
        {
            return CreateFunctionColumn2("-", "({0} - {1})", col, value);
        }
        public static QColumn operator -(QColumnBase col, UInt32 value)
        {
            return CreateFunctionColumn2("-", "({0} - {1})", col, value);
        }
        public static QColumn operator -(QColumnBase col, UInt64 value)
        {
            return CreateFunctionColumn2("-", "({0} - {1})", col, value);
        }
        public static QColumn operator -(QColumnBase col, Single value)
        {
            return CreateFunctionColumn2("-", "({0} - {1})", col, value);
        }
        public static QColumn operator -(QColumnBase col, Double value)
        {
            return CreateFunctionColumn2("-", "({0} - {1})", col, value);
        }
        public static QColumn operator -(QColumnBase col, Decimal value)
        {
            return CreateFunctionColumn2("-", "({0} - {1})", col, value);
        }

        #endregion

        #region operator *
        public static QColumn operator *(QColumnBase col, QColumn value)
        {
            return CreateFunctionColumn2("*", "{0} * {1}", col, value);
        }
        public static QColumn operator *(QColumnBase col, Int16 value)
        {
            return CreateFunctionColumn2("*", "{0} * {1}", col, value);
        }
        public static QColumn operator *(QColumnBase col, Int32 value)
        {
            return CreateFunctionColumn2("*", "{0} * {1}", col, value);
        }
        public static QColumn operator *(QColumnBase col, Int64 value)
        {
            return CreateFunctionColumn2("*", "{0} * {1}", col, value);
        }
        public static QColumn operator *(QColumnBase col, UInt16 value)
        {
            return CreateFunctionColumn2("*", "{0} * {1}", col, value);
        }
        public static QColumn operator *(QColumnBase col, UInt32 value)
        {
            return CreateFunctionColumn2("*", "{0} * {1}", col, value);
        }
        public static QColumn operator *(QColumnBase col, UInt64 value)
        {
            return CreateFunctionColumn2("*", "{0} * {1}", col, value);
        }
        public static QColumn operator *(QColumnBase col, Single value)
        {
            return CreateFunctionColumn2("*", "{0} * {1}", col, value);
        }
        public static QColumn operator *(QColumnBase col, Double value)
        {
            return CreateFunctionColumn2("*", "{0} * {1}", col, value);
        }
        public static QColumn operator *(QColumnBase col, Decimal value)
        {
            return CreateFunctionColumn2("*", "{0} * {1}", col, value);
        }

        #endregion

        #region operator /
        public static QColumn operator /(QColumnBase col, QColumn value)
        {
            return CreateFunctionColumn2("/", "{0} / {1}", col, value);
        }
        public static QColumn operator /(QColumnBase col, Int16 value)
        {
            return CreateFunctionColumn2("/", "{0} / {1}", col, value);
        }
        public static QColumn operator /(QColumnBase col, Int32 value)
        {
            return CreateFunctionColumn2("/", "{0} / {1}", col, value);
        }
        public static QColumn operator /(QColumnBase col, Int64 value)
        {
            return CreateFunctionColumn2("/", "{0} / {1}", col, value);
        }
        public static QColumn operator /(QColumnBase col, UInt16 value)
        {
            return CreateFunctionColumn2("/", "{0} / {1}", col, value);
        }
        public static QColumn operator /(QColumnBase col, UInt32 value)
        {
            return CreateFunctionColumn2("/", "{0} / {1}", col, value);
        }
        public static QColumn operator /(QColumnBase col, UInt64 value)
        {
            return CreateFunctionColumn2("/", "{0} / {1}", col, value);
        }
        public static QColumn operator /(QColumnBase col, Single value)
        {
            return CreateFunctionColumn2("/", "{0} / {1}", col, value);
        }
        public static QColumn operator /(QColumnBase col, Double value)
        {
            return CreateFunctionColumn2("/", "{0} / {1}", col, value);
        }
        public static QColumn operator /(QColumnBase col, Decimal value)
        {
            return CreateFunctionColumn2("/", "{0} / {1}", col, value);
        }

        #endregion

        #region operator %
        public static QColumn operator %(QColumnBase col, QColumn value)
        {
            return CreateFunctionColumn2("%", "{0} % {1}", col, value);
        }
        public static QColumn operator %(QColumnBase col, Int16 value)
        {
            return CreateFunctionColumn2("%", "{0} % {1}", col, value);
        }
        public static QColumn operator %(QColumnBase col, Int32 value)
        {
            return CreateFunctionColumn2("%", "{0} % {1}", col, value);
        }
        public static QColumn operator %(QColumnBase col, Int64 value)
        {
            return CreateFunctionColumn2("%", "{0} % {1}", col, value);
        }
        public static QColumn operator %(QColumnBase col, UInt16 value)
        {
            return CreateFunctionColumn2("%", "{0} % {1}", col, value);
        }
        public static QColumn operator %(QColumnBase col, UInt32 value)
        {
            return CreateFunctionColumn2("%", "{0} % {1}", col, value);
        }
        public static QColumn operator %(QColumnBase col, UInt64 value)
        {
            return CreateFunctionColumn2("%", "{0} % {1}", col, value);
        }
        public static QColumn operator %(QColumnBase col, Single value)
        {
            return CreateFunctionColumn2("%", "{0} % {1}", col, value);
        }
        public static QColumn operator %(QColumnBase col, Double value)
        {
            return CreateFunctionColumn2("%", "{0} % {1}", col, value);
        }
        public static QColumn operator %(QColumnBase col, Decimal value)
        {
            return CreateFunctionColumn2("%", "{0} % {1}", col, value);
        }

        #endregion


        #region operator Null

        public static QCondition operator ==(QColumnBase col, DBNull Null)
        {
            return new QColumnValueCondition(col, " IS NULL");
        }
        public static QCondition operator !=(QColumnBase col, DBNull Null)
        {
            return new QColumnValueCondition(col, " IS NOT NULL");
        }
        #endregion

        #region QJoinCondition operator == operator !=
        public static QJoinCondition operator ==(QColumnBase col1, QColumnBase col2)
        {
            return new QJoinCondition(col1, "=", col2);
        }

        public static QJoinCondition operator !=(QColumnBase col1, QColumnBase col2)
        {
            return new QJoinCondition(col1, "<>", col2);
        }
        #endregion

        #region operator ==
        public static QCondition operator ==(QColumnBase col, bool value)
        {
            return CreateCondition(col, "=", value);
        }
        public static QCondition operator ==(QColumnBase col, Int16 value)
        {
            return CreateCondition(col, "=", value);
        }
        public static QCondition operator ==(QColumnBase col, Int32 value)
        {
            return CreateCondition(col, "=", value);
        }
        public static QCondition operator ==(QColumnBase col, Int64 value)
        {
            return CreateCondition(col, "=", value);
        }
        public static QCondition operator ==(QColumnBase col, UInt16 value)
        {
            return CreateCondition(col, "=", value);
        }
        public static QCondition operator ==(QColumnBase col, UInt32 value)
        {
            return CreateCondition(col, "=", value);
        }
        public static QCondition operator ==(QColumnBase col, UInt64 value)
        {
            return CreateCondition(col, "=", value);
        }
        public static QCondition operator ==(QColumnBase col, Single value)
        {
            return CreateCondition(col, "=", value);
        }
        public static QCondition operator ==(QColumnBase col, Double value)
        {
            return CreateCondition(col, "=", value);
        }
        public static QCondition operator ==(QColumnBase col, Decimal value)
        {
            return CreateCondition(col, "=", value);
        }
        public static QCondition operator ==(QColumnBase col, string value)
        {
            if (value == null) {
                return new QColumnValueCondition(col, "IS NULL");
            }
            return CreateCondition(col, "=", value);
        }
        public static QCondition operator ==(QColumnBase col, DateTime value)
        {
            return CreateCondition(col, "=", value);
        }
        #endregion

        #region operator !=
        public static QCondition operator !=(QColumnBase col, bool value)
        {
            return CreateCondition(col, "<>", value);
        }
        public static QCondition operator !=(QColumnBase col, Int16 value)
        {
            return CreateCondition(col, "<>", value);
        }
        public static QCondition operator !=(QColumnBase col, Int32 value)
        {
            return CreateCondition(col, "<>", value);
        }
        public static QCondition operator !=(QColumnBase col, Int64 value)
        {
            return CreateCondition(col, "<>", value);
        }
        public static QCondition operator !=(QColumnBase col, UInt16 value)
        {
            return CreateCondition(col, "<>", value);
        }
        public static QCondition operator !=(QColumnBase col, UInt32 value)
        {
            return CreateCondition(col, "<>", value);
        }
        public static QCondition operator !=(QColumnBase col, UInt64 value)
        {
            return CreateCondition(col, "<>", value);
        }
        public static QCondition operator !=(QColumnBase col, Single value)
        {
            return CreateCondition(col, "<>", value);
        }
        public static QCondition operator !=(QColumnBase col, Double value)
        {
            return CreateCondition(col, "<>", value);
        }
        public static QCondition operator !=(QColumnBase col, Decimal value)
        {
            return CreateCondition(col, "<>", value);
        }
        public static QCondition operator !=(QColumnBase col, string value)
        {
            if (value == null) {
                return new QColumnValueCondition(col, "IS NOT NULL");
            }
            return CreateCondition(col, "<>", value);
        }
        public static QCondition operator !=(QColumnBase col, DateTime value)
        {
            return CreateCondition(col, "<>", value);
        }
        #endregion

        #region operator >
        public static QCondition operator >(QColumnBase col, QColumn col2)
        {
            return CreateCondition(col, ">", col2);
        }
        public static QCondition operator >(QColumnBase col, Int16 value)
        {
            return CreateCondition(col, ">", value);
        }
        public static QCondition operator >(QColumnBase col, Int32 value)
        {
            return CreateCondition(col, ">", value);
        }
        public static QCondition operator >(QColumnBase col, Int64 value)
        {
            return CreateCondition(col, ">", value);
        }
        public static QCondition operator >(QColumnBase col, UInt16 value)
        {
            return CreateCondition(col, ">", value);
        }
        public static QCondition operator >(QColumnBase col, UInt32 value)
        {
            return CreateCondition(col, ">", value);
        }
        public static QCondition operator >(QColumnBase col, UInt64 value)
        {
            return CreateCondition(col, ">", value);
        }
        public static QCondition operator >(QColumnBase col, Single value)
        {
            return CreateCondition(col, ">", value);
        }
        public static QCondition operator >(QColumnBase col, Double value)
        {
            return CreateCondition(col, ">", value);
        }
        public static QCondition operator >(QColumnBase col, Decimal value)
        {
            return CreateCondition(col, ">", value);
        }
        public static QCondition operator >(QColumnBase col, DateTime value)
        {
            return CreateCondition(col, ">", value);
        }
        #endregion

        #region operator >=
        public static QCondition operator >=(QColumnBase col, QColumn col2)
        {
            return CreateCondition(col, ">=", col2);
        }
        public static QCondition operator >=(QColumnBase col, Int16 value)
        {
            return CreateCondition(col, ">=", value);
        }
        public static QCondition operator >=(QColumnBase col, Int32 value)
        {
            return CreateCondition(col, ">=", value);
        }
        public static QCondition operator >=(QColumnBase col, Int64 value)
        {
            return CreateCondition(col, ">=", value);
        }
        public static QCondition operator >=(QColumnBase col, UInt16 value)
        {
            return CreateCondition(col, ">=", value);
        }
        public static QCondition operator >=(QColumnBase col, UInt32 value)
        {
            return CreateCondition(col, ">=", value);
        }
        public static QCondition operator >=(QColumnBase col, UInt64 value)
        {
            return CreateCondition(col, ">=", value);
        }
        public static QCondition operator >=(QColumnBase col, Single value)
        {
            return CreateCondition(col, ">=", value);
        }
        public static QCondition operator >=(QColumnBase col, Double value)
        {
            return CreateCondition(col, ">=", value);
        }
        public static QCondition operator >=(QColumnBase col, Decimal value)
        {
            return CreateCondition(col, ">=", value);
        }
        public static QCondition operator >=(QColumnBase col, DateTime value)
        {
            return CreateCondition(col, ">=", value);
        }
        #endregion

        #region operator <
        public static QCondition operator <(QColumnBase col, QColumn col2)
        {
            return CreateCondition(col, "<", col2);
        }
        public static QCondition operator <(QColumnBase col, Int16 value)
        {
            return CreateCondition(col, "<", value);
        }
        public static QCondition operator <(QColumnBase col, Int32 value)
        {
            return CreateCondition(col, "<", value);
        }
        public static QCondition operator <(QColumnBase col, Int64 value)
        {
            return CreateCondition(col, "<", value);
        }
        public static QCondition operator <(QColumnBase col, UInt16 value)
        {
            return CreateCondition(col, "<", value);
        }
        public static QCondition operator <(QColumnBase col, UInt32 value)
        {
            return CreateCondition(col, "<", value);
        }
        public static QCondition operator <(QColumnBase col, UInt64 value)
        {
            return CreateCondition(col, "<", value);
        }
        public static QCondition operator <(QColumnBase col, Single value)
        {
            return CreateCondition(col, "<", value);
        }
        public static QCondition operator <(QColumnBase col, Double value)
        {
            return CreateCondition(col, "<", value);
        }
        public static QCondition operator <(QColumnBase col, Decimal value)
        {
            return CreateCondition(col, "<", value);
        }
        public static QCondition operator <(QColumnBase col, DateTime value)
        {
            return CreateCondition(col, "<", value);
        }
        #endregion

        #region operator <=
        public static QCondition operator <=(QColumnBase col, QColumn col2)
        {
            return CreateCondition(col, "<=", col2);
        }
        public static QCondition operator <=(QColumnBase col, Int16 value)
        {
            return CreateCondition(col, "<=", value);
        }
        public static QCondition operator <=(QColumnBase col, Int32 value)
        {
            return CreateCondition(col, "<=", value);
        }
        public static QCondition operator <=(QColumnBase col, Int64 value)
        {
            return CreateCondition(col, "<=", value);
        }
        public static QCondition operator <=(QColumnBase col, UInt16 value)
        {
            return CreateCondition(col, "<=", value);
        }
        public static QCondition operator <=(QColumnBase col, UInt32 value)
        {
            return CreateCondition(col, "<=", value);
        }
        public static QCondition operator <=(QColumnBase col, UInt64 value)
        {
            return CreateCondition(col, "<=", value);
        }
        public static QCondition operator <=(QColumnBase col, Single value)
        {
            return CreateCondition(col, "<=", value);
        }
        public static QCondition operator <=(QColumnBase col, Double value)
        {
            return CreateCondition(col, "<=", value);
        }
        public static QCondition operator <=(QColumnBase col, Decimal value)
        {
            return CreateCondition(col, "<=", value);
        }
        public static QCondition operator <=(QColumnBase col, DateTime value)
        {
            return CreateCondition(col, "<=", value);
        }
        #endregion



    }
}
