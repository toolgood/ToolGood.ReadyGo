using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Enums;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Interfaces
{
    interface IConditionConvert
    {
        string ToSql(DatabaseProvider provider, int tableCount);
        void ToSql(StringBuilder stringBuilder, DatabaseProvider provider, int tableCount);


    }
}
namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class QWhereCondition : IConditionConvert
    {
        string IConditionConvert.ToSql(DatabaseProvider provider, int tableCount)
        {
            if (whereType == WhereType.None) {
                return "";
            }
            if (whereType == WhereType.Single) {
                return ((IConditionConvert)leftCondition).ToSql(provider, tableCount);
            }
            StringBuilder stringBuilder = new StringBuilder();
            ((IConditionConvert)this).ToSql(stringBuilder, provider, tableCount);
            return stringBuilder.ToString();
        }

        void IConditionConvert.ToSql(StringBuilder stringBuilder, DatabaseProvider provider, int tableCount)
        {
            if (whereType == WhereType.None) return;
            if (whereType == WhereType.Single) {
                ((IConditionConvert)leftCondition).ToSql(stringBuilder, provider, tableCount);
                return;
            }
            // and 连接
            // 左边代码有 or    (A or B) and C
            // 右边代码有 or    A and (B or C)
            // 两边有or  (A or B) and (C or D)
            // or 连接
            // 左边代码有 or    (A or B) or C
            // 左边代码有 and    (A and B) or C
            // 右边代码有 or    A or (B or C)
            // 两边有or  (A or B) or (C or D)
            if (whereType == WhereType.Or) {
                bool b = leftCondition is QWhereCondition;
                if (b) stringBuilder.Append("(");
                ((IConditionConvert)leftCondition).ToSql(stringBuilder, provider, tableCount);
                if (b) stringBuilder.Append(")");
                stringBuilder.Append(" OR ");

                b = rightCondition is QWhereCondition;
                if (b) stringBuilder.Append("(");
                ((IConditionConvert)rightCondition).ToSql(stringBuilder, provider, tableCount);
                if (b) stringBuilder.Append(")");
                return;
            }

            if (leftHasOr) stringBuilder.Append("(");
            ((IConditionConvert)leftCondition).ToSql(stringBuilder, provider, tableCount);
            if (leftHasOr) stringBuilder.Append(")");
            stringBuilder.Append(" AND ");
            if (rightHasOr) stringBuilder.Append("(");
            ((IConditionConvert)rightCondition).ToSql(stringBuilder, provider, tableCount);
            if (rightHasOr) stringBuilder.Append(")");
        }
    }

    partial class QCodeCondition : IConditionConvert
    {
        string IConditionConvert.ToSql(DatabaseProvider provider, int tableCount)
        {
            return code;
        }

        void IConditionConvert.ToSql(StringBuilder stringBuilder, DatabaseProvider provider, int tableCount)
        {
            stringBuilder.Append(code);
        }
    }

    partial class QColumnValueCondition : IConditionConvert
    {
        string IConditionConvert.ToSql(DatabaseProvider provider, int tableCount)
        {
            if (isSetValaue == false) {
                return ((IColumnConvert)leftColumn).ToSql(provider, tableCount) + " " + op;
            }
            return ((IColumnConvert)leftColumn).ToSql(provider, tableCount) + " " + op + " "
                + provider.ConvertTo(value);
        }

        void IConditionConvert.ToSql(StringBuilder stringBuilder, DatabaseProvider provider, int tableCount)
        {
            stringBuilder.Append(((IColumnConvert)leftColumn).ToSql(provider, tableCount));
            stringBuilder.Append(" ");
            stringBuilder.Append(op);
            if (isSetValaue) {
                stringBuilder.Append(" ");
                stringBuilder.Append(provider.ConvertTo( value));
            }
        }
    }

    partial class QColumnColumnCondition : IConditionConvert
    {
        string IConditionConvert.ToSql(DatabaseProvider provider, int tableCount)
        {
            return ((IColumnConvert)leftColumn).ToSql(provider, tableCount) + " " + Op + " "
                + ((IColumnConvert)rightColumn).ToSql(provider, tableCount);
        }

        void IConditionConvert.ToSql(StringBuilder stringBuilder, DatabaseProvider provider, int tableCount)
        {
            stringBuilder.Append(((IColumnConvert)leftColumn).ToSql(provider, tableCount));
            stringBuilder.Append(" ");

            stringBuilder.Append(Op);
            stringBuilder.Append(" ");
            stringBuilder.Append(((IColumnConvert)rightColumn).ToSql(provider, tableCount));
        }
    }

}

