using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Enums;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    public class QCondition
    {
        public static QCodeCondition True = new QCodeCondition("1=1");
        public static QCodeCondition False = new QCodeCondition("1=2");

        public static QWhereCondition operator &(QCondition c1, QCondition c2)
        {
            if (c1 is QWhereCondition) {
                return ((QWhereCondition)c1).And(c2);
            }
            if (c2 is QWhereCondition) {
                return ((QWhereCondition)c2).And(c1);
            }
            return new QWhereCondition(c1, WhereType.And, c2);
        }
        public static QWhereCondition operator |(QCondition c1, QCondition c2)
        {
            if (c1 is QWhereCondition) {
                return ((QWhereCondition)c1).Or(c2);
            }
            if (c2 is QWhereCondition) {
                return ((QWhereCondition)c2).Or(c1);
            }
            return new QWhereCondition(c1, WhereType.Or, c2);
        }
    }

    public partial class QWhereCondition : QCondition
    {
        private WhereType whereType;
        private QCondition leftCondition;
        private QCondition rightCondition;
        private bool leftHasOr;
        private bool rightHasOr;

        public WhereType WhereType { get { return whereType; } }

        public QWhereCondition()
        {
            whereType = WhereType.None;
        }
        public QWhereCondition(QCondition c1, WhereType whereType, QCondition c2)
        {
            leftCondition = c1;
            this.whereType = whereType;
            rightCondition = c2;
        }


        public QWhereCondition And(QCondition condition)
        {
            return AddCondition(condition, WhereType.And);
        }

        public QWhereCondition Or(QCondition condition)
        {
            return AddCondition(condition, WhereType.Or);
        }

        private QWhereCondition AddCondition(QCondition condition, WhereType type)
        {
            if (whereType == WhereType.None) {
                if (condition is QWhereCondition) {
                    return (QWhereCondition)condition;
                }
                whereType = WhereType.Single;
                leftCondition = condition;
                return this;
            }
            if (whereType == WhereType.Single) {
                whereType = type;
                rightCondition = condition;
                if (rightCondition is QWhereCondition) {
                    rightHasOr = ((QWhereCondition)rightCondition).whereType == WhereType.Or;
                }
                return this;
            }
            QWhereCondition where = new QWhereCondition();
            where.whereType = type;
            where.leftCondition = this;
            where.rightCondition = condition;
            where.leftHasOr = this.whereType == WhereType.Or;
            if (rightCondition is QWhereCondition) {
                where.rightHasOr = ((QWhereCondition)rightCondition).whereType == WhereType.Or;
            }
            return where;
        }

    }

    public partial class QCodeCondition : QCondition
    {
        private string code;

        public QCodeCondition() { }

        public QCodeCondition(string code)
        {
            this.code = code;
        }
    }

    public partial class QColumnValueCondition : QCondition
    {
        private QColumnBase leftColumn;
        private string op;
        private object value;
        private bool isSetValaue;

        public QColumnValueCondition(QColumnBase column1, string op, object value)
        {
            leftColumn = column1;
            this.op = op;
            this.value = value;
            isSetValaue = true;
        }

        public QColumnValueCondition(QColumnBase column1, string op)
        {
            leftColumn = column1;
            this.op = op;
        }

    }
    public partial class QColumnColumnCondition : QCondition
    {
        internal QColumnBase leftColumn;
        private string Op;
        internal QColumnBase rightColumn;

        public QColumnColumnCondition()
        {
        }

        public QColumnColumnCondition(QColumnBase column1, string op, QColumnBase column2)
        {
            leftColumn = column1;
            Op = op;
            rightColumn = column2;
        }

    }

    public partial class QJoinCondition : QColumnColumnCondition
    {
        //internal new QColumnBase leftColumn;
        //internal new QColumnBase rightColumn;
        public QJoinCondition(QColumnBase column1, string op, QColumnBase column2)
            : base(column1, op, column2)
        {
            //leftColumn = column1;
            //rightColumn = column2;
        }
    }
}
