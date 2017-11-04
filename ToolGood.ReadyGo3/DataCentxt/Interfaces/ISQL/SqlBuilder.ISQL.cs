using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    public partial class SqlBuilder : ISQL
    {
        public void WhereExists(string sql, params object[] args)
        {
            if (_jump) { _jump = false; return; }

            if (string.IsNullOrWhiteSpace(sql)) {
                throw new NoWhereException();
            }
            if (sql.StartsWith("(") == false) {
                sql = "(" + sql + ")";
            }

            whereCondition = whereCondition.And(new QCodeCondition("EXISTS " + _provider.FormatSql(sql, args)));

        }

        public void WhereNotExists(string sql, params object[] args)
        {
            if (_jump) { _jump = false; return; }
            if (string.IsNullOrWhiteSpace(sql)) {
                throw new NoWhereException();
            }
            if (sql.StartsWith("(") == false) {
                sql = "(" + sql + ")";
            }
            whereCondition = whereCondition.And(new QCodeCondition("NOT EXISTS " + _provider.FormatSql(sql, args)));
        }

        public void Where(string @where, params object[] args)
        {
            if (_jump) { _jump = false; return; }
            whereCondition = whereCondition.And(new QCodeCondition(_provider.FormatSql(@where, args)));
        }

        public void JoinWithOn(string joinWithOn)
        {
            if (_jump) { _jump = false; return; }
            if (_joinOnText == null) {
                _joinOnText = _provider.FormatSql(joinWithOn, null);
            } else {
                _joinOnText += " " + _provider.FormatSql(joinWithOn, null);
            }
        }

        public void GroupBy(string groupBy)
        {
            if (_jump) { _jump = false; return; }
            var column = new QColumn() {
                _columnType = Enums.ColumnType.Code,
                _code = groupBy
            };
            _groupBy.Add(column);

        }

        public void Having(string having)
        {
            if (_jump) { _jump = false; return; }
            var c = new QCodeCondition(having);
            _having.Add(c);

        }

        public void OrderBy(string orderBy)
        {
            if (_jump) { _jump = false; return; }
            _orderBys.Add(new OrderItem(orderBy));

        }

        public void Where(QCondition @where)
        {
            if (_jump) { _jump = false; return; }
            whereCondition = whereCondition.And(@where);
        }

        public void Join(QTable table, JoinType joinType = JoinType.Inner)
        {
            if (_jump) { _jump = false; return; }
            this.AddTable(table);
            table._joinType = joinType;

        }

        public void Join(QTable table, JoinType joinType, QJoinCondition @on)
        {
            if (_jump) { _jump = false; return; }
            this.AddTable(table);
            table._joinType = joinType;
            table._joinCondition = @on;

        }

        public void LeftJoin(QTable table)
        {
            if (_jump) { _jump = false; return; }
            this.AddTable(table);
            table._joinType = JoinType.Left;
        }

        public void LeftJoin(QTable table, QJoinCondition @on)
        {
            if (_jump) { _jump = false; return; }
            this.AddTable(table);
            table._joinType = JoinType.Left;
            table._joinCondition = @on;

        }

        public void RightJoin(QTable table)
        {
            if (_jump) { _jump = false; return; }
            this.AddTable(table);
            table._joinType = JoinType.Right;

        }

        public void RightJoin(QTable table, QJoinCondition @on)
        {
            if (_jump) { _jump = false; return; }
            this.AddTable(table);
            table._joinType = JoinType.Right;
            table._joinCondition = @on;

        }

        public void InnerJoin(QTable table)
        {
            if (_jump) { _jump = false; return; }

            this.AddTable(table);
            table._joinType = JoinType.Inner;

        }

        public void InnerJoin(QTable table, QJoinCondition @on)
        {
            if (_jump) { _jump = false; return; }
            this.AddTable(table);
            table._joinType = JoinType.Inner;
            table._joinCondition = @on;

        }

        public void FullJoin(QTable table)
        {
            if (_jump) { _jump = false; return; }
            this.AddTable(table);
            table._joinType = JoinType.Full;

        }

        public void FullJoin(QTable table, QJoinCondition @on)
        {
            if (_jump) { _jump = false; return; }
            this.AddTable(table);
            table._joinType = JoinType.Full;
            table._joinCondition = @on;

        }

        public void On(QJoinCondition @on)
        {
            if (_jump) { _jump = false; return; }
            var jc = (QJoinCondition)@on;
            var index1 = _tables.IndexOf(jc.leftColumn._table);
            if (index1 < 0) {
                throw new ArgumentException("on 参数无效");
            }
            var index2 = _tables.IndexOf(jc.rightColumn._table);
            if (index2 < 0) {
                throw new ArgumentException("on 参数无效");
            }
            if (index1 > index2) {
                jc.leftColumn._table._joinCondition = @on;
            } else {
                jc.rightColumn._table._joinCondition = @on;
            }

        }

        public NewT Join<NewT>(JoinType joinType = JoinType.Inner) where NewT : QTable
        {
            var table = Activator.CreateInstance<NewT>();
            if (_jump) { _jump = false; return table; }

            this.AddTable(table);
            table._joinType = joinType;
            return table;
        }

        public NewT LeftJoin<NewT>() where NewT : QTable
        {
            var table = Activator.CreateInstance<NewT>();
            if (_jump) { _jump = false; return table; }

            this.AddTable(table);
            table._joinType = JoinType.Left;
            return table;
        }

        public NewT RightJoin<NewT>() where NewT : QTable
        {
            var table = Activator.CreateInstance<NewT>();
            if (_jump) { _jump = false; return table; }

            this.AddTable(table);
            table._joinType = JoinType.Right;
            return table;
        }

        public NewT InnerJoin<NewT>() where NewT : QTable
        {
            var table = Activator.CreateInstance<NewT>();
            if (_jump) { _jump = false; return table; }

            this.AddTable(table);
            table._joinType = JoinType.Inner;
            return table;
        }

        public NewT FullJoin<NewT>() where NewT : QTable
        {
            var table = Activator.CreateInstance<NewT>();
            if (_jump) { _jump = false; return table; }

            this.AddTable(table);
            table._joinType = JoinType.Full;
            return table;
        }


        public void GroupBy(QColumn column, params QColumn[] columns)
        {
            if (_jump) { _jump = false; return; }

            _groupBy.Add(column);
            foreach (var item in columns) {
                _groupBy.Add(item);
            }

        }

        public void Having(QCondition having)
        {
            if (_jump) { _jump = false; return; }

            _having.Add(having);

        }

        public void OrderBy(QColumn column, OrderType orderType = OrderType.Asc)
        {
            if (_jump) { _jump = false; return; }

            _orderBys.Add(new OrderItem(column, orderType));

        }



    }
}
