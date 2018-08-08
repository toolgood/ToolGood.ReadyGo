using ToolGood.ReadyGo3.DataCentxt.Enums;


namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    /// <summary>
    /// SQL条件
    /// </summary>
    public partial class QCondition
    {
        /// <summary>
        /// True
        /// </summary>
        public static QCodeCondition True = new QCodeCondition("1=1");

        /// <summary>
        /// False
        /// </summary>
        public static QCodeCondition False = new QCodeCondition("1=2");

        /// <summary>
        /// 并且 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 或
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
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

    /// <summary>
    /// SQL条件 Where
    /// </summary>
    public partial class QWhereCondition : QCondition
    {
        private WhereType whereType;
        private QCondition leftCondition;
        private QCondition rightCondition;
        private bool leftHasOr;
        private bool rightHasOr;
        /// <summary>
        /// Where类型
        /// </summary>
        internal WhereType WhereType { get { return whereType; } }
        /// <summary>
        /// 
        /// </summary>
        internal QWhereCondition()
        {
            whereType = WhereType.None;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="whereType"></param>
        /// <param name="c2"></param>
        internal QWhereCondition(QCondition c1, WhereType whereType, QCondition c2)
        {
            leftCondition = c1;
            this.whereType = whereType;
            rightCondition = c2;
        }


        internal QWhereCondition And(QCondition condition)
        {
            return AddCondition(condition, WhereType.And);
        }

        internal QWhereCondition Or(QCondition condition)
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
            var where = new QWhereCondition();
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
    /// <summary>
    /// 
    /// </summary>
    public partial class QCodeCondition : QCondition
    {
        private string code;

        internal QCodeCondition() { }

        internal QCodeCondition(string code)
        {
            this.code = code;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class QColumnValueCondition : QCondition
    {
        private QColumn leftColumn;
        private string op;
        private object value;
        private bool isSetValaue;

        internal QColumnValueCondition(QColumn column1, string op, object value)
        {
            leftColumn = column1;
            this.op = op;
            this.value = value;
            //if ("=|==|<>|!=|>|<|>=|<=".Contains(op)) {
            //    if (value is Int16 || value is Int32 || value is Int64
            //        || value is UInt16 || value is UInt32 || value is UInt64
            //        || value is Single || value is Double || value is Decimal
            //        ) {
            //        this.value = value.ToString();
            //    }
            //    if (value is Int16? || value is Int32? || value is Int64?
            //        || value is UInt16? || value is UInt32? || value is UInt64?
            //        || value is Single? || value is Double? || value is Decimal?
            //        ) {
            //        if (object.Equals(null, value) == false) {
            //            this.value = value.ToString();
            //        }
            //    }
            //    if (value is bool) {
            //        this.value = (bool)value ? "1" : "0";
            //    }
            //    if (value is bool?) {
            //        if (object.Equals(null, value) == false) {
            //            this.value = ((bool?)value).Value ? "1" : "0";
            //        }
            //    }
            //}
            isSetValaue = true;
        }

        internal QColumnValueCondition(QColumn column1, string op)
        {
            leftColumn = column1;
            this.op = op;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class QColumnColumnCondition : QCondition
    {
        internal QColumn leftColumn;
        private string Op;
        internal QColumn rightColumn;

        internal QColumnColumnCondition()
        {
        }

        internal QColumnColumnCondition(QColumn column1, string op, QColumn column2)
        {
            leftColumn = column1;
            Op = op;
            rightColumn = column2;
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public partial class QJoinCondition : QColumnColumnCondition
    {
        //internal new QColumnBase leftColumn;
        //internal new QColumnBase rightColumn;
        internal QJoinCondition(QColumn column1, string op, QColumn column2)
            : base(column1, op, column2)
        {
            //leftColumn = column1;
            //rightColumn = column2;
        }



    }
}
