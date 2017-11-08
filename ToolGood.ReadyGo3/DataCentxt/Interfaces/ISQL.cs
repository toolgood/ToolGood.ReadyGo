using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Internals;

namespace ToolGood.ReadyGo3.DataCentxt.Interfaces
{
    public interface ISQL<T>
        where T : class
    {
        T WhereExists(string sql, params object[] args);
        T WhereNotExists(string sql, params object[] args);

        T Where(string where, params object[] args);
        T JoinWithOn(string joinWithOn);
        T GroupBy(string groupBy);
        T Having(string having);
        T OrderBy(string orderBy);

        T Where(QCondition where);
        T Join(QTable table, JoinType joinType = JoinType.Inner);
        T Join(QTable table, JoinType joinType, QJoinCondition on);
        T LeftJoin(QTable table);
        T LeftJoin(QTable table, QJoinCondition on);
        T RightJoin(QTable table);
        T RightJoin(QTable table, QJoinCondition on);
        T InnerJoin(QTable table);
        T InnerJoin(QTable table, QJoinCondition on);
        T FullJoin(QTable table);
        T FullJoin(QTable table, QJoinCondition on);
        T On(QJoinCondition on);

        NewT Join<NewT>(JoinType joinType = JoinType.Inner) where NewT: QTable;
        NewT LeftJoin<NewT>() where NewT : QTable;
        NewT RightJoin<NewT>() where NewT : QTable;
        NewT InnerJoin<NewT>() where NewT : QTable;
        NewT FullJoin<NewT>() where NewT : QTable;


        T GroupBy(QColumnBase column, params QColumnBase[] columns);
        T Having(QCondition having);
        T OrderBy(QColumnBase column, OrderType orderType = OrderType.Asc);
    }

    public interface ISQL
    {
        void WhereExists(string sql, params object[] args);
        void WhereNotExists(string sql, params object[] args);

        void Where(string where, params object[] args);
        void JoinWithOn(string joinWithOn);
        void GroupBy(string groupBy);
        void Having(string having);
        void OrderBy(string orderBy);

        void Where(QCondition where);
        void Join(QTable table, JoinType joinType = JoinType.Inner);
        void Join(QTable table, JoinType joinType, QJoinCondition on);
        void LeftJoin(QTable table);
        void LeftJoin(QTable table, QJoinCondition on);
        void RightJoin(QTable table);
        void RightJoin(QTable table, QJoinCondition on);
        void InnerJoin(QTable table);
        void InnerJoin(QTable table, QJoinCondition on);
        void FullJoin(QTable table);
        void FullJoin(QTable table, QJoinCondition on);
        void On(QJoinCondition on);

        NewT Join<NewT>(JoinType joinType = JoinType.Inner) where NewT : QTable;
        NewT LeftJoin<NewT>() where NewT : QTable;
        NewT RightJoin<NewT>() where NewT : QTable;
        NewT InnerJoin<NewT>() where NewT : QTable;
        NewT FullJoin<NewT>() where NewT : QTable;


        void GroupBy(QColumnBase column, params QColumnBase[] columns);
        void Having(QCondition having);
        void OrderBy(QColumnBase column, OrderType orderType = OrderType.Asc);
    }
}
