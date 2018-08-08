using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Enums;



namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class QCondition
    {
        /// <summary>
        /// 转SQL语句
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="tableCount"></param>
        /// <returns></returns>
        protected internal virtual string ToSql(DatabaseProvider provider, int tableCount) { return null; }

        /// <summary>
        /// 转SQL语句
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="provider"></param>
        /// <param name="tableCount"></param>
        protected internal virtual void ToSql(StringBuilder stringBuilder, DatabaseProvider provider, int tableCount) { }
    }

    partial class QWhereCondition
    {
        /// <summary>
        /// 转SQL语句
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="tableCount"></param>
        /// <returns></returns>
        protected internal override string ToSql(DatabaseProvider provider, int tableCount)
        {
            if (whereType == WhereType.None) {
                return "";
            }
            if (whereType == WhereType.Single) {
                return (leftCondition).ToSql(provider, tableCount);
            }
            StringBuilder stringBuilder = new StringBuilder();
            (this).ToSql(stringBuilder, provider, tableCount);
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 转SQL语句
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="provider"></param>
        /// <param name="tableCount"></param>
        protected internal override void ToSql(StringBuilder stringBuilder, DatabaseProvider provider, int tableCount)
        {
            if (whereType == WhereType.None) return;
            if (whereType == WhereType.Single) {
                (leftCondition).ToSql(stringBuilder, provider, tableCount);
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
                (leftCondition).ToSql(stringBuilder, provider, tableCount);
                if (b) stringBuilder.Append(")");
                stringBuilder.Append(" OR ");

                b = rightCondition is QWhereCondition;
                if (b) stringBuilder.Append("(");
                (rightCondition).ToSql(stringBuilder, provider, tableCount);
                if (b) stringBuilder.Append(")");
                return;
            }

            if (leftHasOr) stringBuilder.Append("(");
            (leftCondition).ToSql(stringBuilder, provider, tableCount);
            if (leftHasOr) stringBuilder.Append(")");
            stringBuilder.Append(" AND ");
            if (rightHasOr) stringBuilder.Append("(");
            (rightCondition).ToSql(stringBuilder, provider, tableCount);
            if (rightHasOr) stringBuilder.Append(")");
        }
    }

    partial class QCodeCondition
    {
        /// <summary>
        /// 转SQL语句
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="tableCount"></param>
        /// <returns></returns>
        protected internal override string ToSql(DatabaseProvider provider, int tableCount)
        {
            return code;
        }
        /// <summary>
        /// 转SQL语句
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="provider"></param>
        /// <param name="tableCount"></param>
        protected internal override void ToSql(StringBuilder stringBuilder, DatabaseProvider provider, int tableCount)
        {
            stringBuilder.Append(code);
        }
    }


    partial class QColumnValueCondition
    {
        /// <summary>
        /// 转SQL语句
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="tableCount"></param>
        /// <returns></returns>
        protected internal override string ToSql(DatabaseProvider provider, int tableCount)
        {
            if (isSetValaue == false) {
                return (leftColumn).ToSql(provider, tableCount) + " " + op;
            }
            return (leftColumn).ToSql(provider, tableCount) + " " + op + " "
                + provider.EscapeParam(value);
        }
        /// <summary>
        /// 转SQL语句
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="provider"></param>
        /// <param name="tableCount"></param>
        protected internal override void ToSql(StringBuilder stringBuilder, DatabaseProvider provider, int tableCount)
        {
            stringBuilder.Append((leftColumn).ToSql(provider, tableCount));
            stringBuilder.Append(" ");
            stringBuilder.Append(op);
            if (isSetValaue) {
                stringBuilder.Append(" ");
                stringBuilder.Append(provider.EscapeParam(value));
            }
        }
    }

    partial class QColumnColumnCondition
    {
        /// <summary>
        /// 转SQL语句
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="tableCount"></param>
        /// <returns></returns>
        protected internal override string ToSql(DatabaseProvider provider, int tableCount)
        {
            return (leftColumn).ToSql(provider, tableCount) + " " + Op + " "
                + (rightColumn).ToSql(provider, tableCount);
        }
        /// <summary>
        /// 转SQL语句
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="provider"></param>
        /// <param name="tableCount"></param>
        protected internal override void ToSql(StringBuilder stringBuilder, DatabaseProvider provider, int tableCount)
        {
            stringBuilder.Append((leftColumn).ToSql(provider, tableCount));
            stringBuilder.Append(" ");

            stringBuilder.Append(Op);
            stringBuilder.Append(" ");
            stringBuilder.Append((rightColumn).ToSql(provider, tableCount));
        }
    }

}

