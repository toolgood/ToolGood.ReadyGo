using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
using ToolGood.ReadyGo3.DataCentxt.Internals;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QTable<T> : ISQL<QTable<T>>
    {
        /// <summary>
        /// 【Exists】语句,累加到【Where】语句，连接使用【AND】
        /// </summary>
        /// <param name="sql">不用写【Exists】，sql语句，列标识使用 [ ] ，参数使用 @数字</param>
        /// <param name="args">参数组</param>
        /// <returns></returns>
        public  QTable<T> WhereExists(string sql, params object[] args)
        {
            getSqlBuilder().WhereExists(sql, args);
            return this;
        }
        /// <summary>
        /// 【Not Exists】语句,累加到【Where】语句，连接使用【AND】
        /// </summary>
        /// <param name="sql">不用写【Not Exists】 ，sql语句，列标识使用 [ ] ，参数使用 @数字</param>
        /// <param name="args">参数组</param>
        /// <returns></returns>
        public  QTable<T> WhereNotExists(string sql, params object[] args)
        {
            getSqlBuilder().WhereNotExists(sql, args);
            return this;
        }
        /// <summary>
        /// 累加到【Where】语句，连接使用【AND】
        /// </summary>
        /// <param name="where">不用写【where】，sql语句，列标识使用 [ ] ，参数使用 @数字</param>
        /// <param name="args">参数组</param>
        /// <returns></returns>
        public  QTable<T> Where(string @where, params object[] args)
        {
            getSqlBuilder().Where(@where, args);
            return this;
        }
        /// <summary>
        /// 累加到【Join XXX On XXX】语句
        /// </summary>
        /// <param name="joinWithOn">以【(left|right|inner|full) join】开头,sql语句，列标识使用 [ ] </param>
        /// <returns></returns>
        public  QTable<T> JoinWithOn(string joinWithOn)
        {
            getSqlBuilder().JoinWithOn(joinWithOn);
            return this;
        }
        /// <summary>
        /// 累加到【Group By】语句
        /// </summary>
        /// <param name="groupBy">不写【Group By】，列标识使用 [ ]</param>
        /// <returns></returns>
        public  QTable<T> GroupBy(string groupBy)
        {
            getSqlBuilder().GroupBy(groupBy);
            return this;
        }
        /// <summary>
        /// 累加到【Having】语句
        /// </summary>
        /// <param name="having">不写【Having】，列标识使用 [ ]</param>
        /// <returns></returns>
        public  QTable<T> Having(string having)
        {
            getSqlBuilder().Having(having);
            return this;
        }
        /// <summary>
        /// 累加到【Order By】语句
        /// </summary>
        /// <param name="orderBy">不写【Order By】，列标识使用 [ ]</param>
        /// <returns></returns>
        public  QTable<T> OrderBy(string orderBy)
        {
            getSqlBuilder().OrderBy(orderBy);
            return this;
        }

        /// <summary>
        /// 累加到【Where】语句，连接使用【AND】
        /// </summary>
        /// <param name="where">【Where】语句</param>
        /// <returns></returns>
        public  QTable<T> Where(QCondition @where)
        {
            getSqlBuilder().Where(@where);
            return this;
        }
        /// <summary>
        /// 累加到【Join】语句
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="joinType">Join类型</param>
        /// <returns></returns>
        public  QTable<T> Join(QTable table, JoinType joinType = JoinType.Inner)
        {
            getSqlBuilder().Join(table, joinType);
            return this;
        }
        /// <summary>
        /// 累加到【Join】语句
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="joinType">Join类型</param>
        /// <param name="on">On条件</param>
        /// <returns></returns>
        public  QTable<T> Join(QTable table, JoinType joinType, QJoinCondition @on)
        {
            getSqlBuilder().Join(table, joinType, @on);
            return this;
        }
        /// <summary>
        /// 累加到【Join】语句 Left Join 类型
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public  QTable<T> LeftJoin(QTable table)
        {
            getSqlBuilder().LeftJoin(table);
            return this;
        }
        /// <summary>
        /// 累加到【Join】语句 Left Join 类型
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="on">On条件</param>
        /// <returns></returns>
        public  QTable<T> LeftJoin(QTable table, QJoinCondition @on)
        {
            getSqlBuilder().LeftJoin(table, @on);
            return this;
        }
        /// <summary>
        /// 累加到【Join】语句 Right Join 类型
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public  QTable<T> RightJoin(QTable table)
        {
            getSqlBuilder().RightJoin(table);
            return this;
        }
        /// <summary>
        /// 累加到【Join】语句 Right Join 类型
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="on">On条件</param>
        /// <returns></returns>
        public  QTable<T> RightJoin(QTable table, QJoinCondition @on)
        {
            getSqlBuilder().RightJoin(table, @on);
            return this;
        }

        /// <summary>
        /// 累加到【Join】语句 Inner Join 类型
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public  QTable<T> InnerJoin(QTable table)
        {
            getSqlBuilder().InnerJoin(table);
            return this;
        }

        /// <summary>
        /// 累加到【Join】语句 Inner Join 类型
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="on">On条件</param>
        /// <returns></returns>
        public  QTable<T> InnerJoin(QTable table, QJoinCondition @on)
        {
            getSqlBuilder().InnerJoin(table, @on);
            return this;
        }

        /// <summary>
        /// 累加到【Join】语句 Full Join 类型
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public  QTable<T> FullJoin(QTable table)
        {
            getSqlBuilder().FullJoin(table);
            return this;
        }
        /// <summary>
        /// 累加到【Join】语句 Full Join 类型
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="on">On条件</param>
        /// <returns></returns>
        public  QTable<T> FullJoin(QTable table, QJoinCondition @on)
        {
            getSqlBuilder().FullJoin(table, @on);
            return this;
        }

        /// <summary>
        /// 累加到【On】语句
        /// </summary>
        /// <param name="on">On条件</param>
        /// <returns></returns>
        public  QTable<T> On(QJoinCondition @on)
        {
            getSqlBuilder().On(@on);
            return this;
        }

        /// <summary>
        /// 累加到【Group By】语句
        /// </summary>
        /// <param name="column">列</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public  QTable<T> GroupBy(QColumn column, params QColumn[] columns)
        {
            getSqlBuilder().GroupBy(column, columns);
            return this;
        }
        /// <summary>
        /// 累加到【Having】语句
        /// </summary>
        /// <param name="having">having条件</param>
        /// <returns></returns>
        public  QTable<T> Having(QCondition having)
        {
            getSqlBuilder().Having(having);
            return this;
        }
        /// <summary>
        /// 累加到【Order By】语句
        /// </summary>
        /// <param name="column">列</param>
        /// <param name="orderType">order类型</param>
        /// <returns></returns>
        public  QTable<T> OrderBy(QColumn column, OrderType orderType = OrderType.Asc)
        {
            getSqlBuilder().OrderBy(column, orderType);
            return this;
        }


        /// <summary>
        /// 累加到【Join】语句
        /// </summary>
        /// <typeparam name="NewT">表名</typeparam>
        /// <param name="joinType">Join 类型</param>
        /// <returns></returns>
        public NewT Join<NewT>(JoinType joinType = JoinType.Inner) where NewT : QTable
        {
            return getSqlBuilder().Join<NewT>(joinType);
        }

        /// <summary>
        /// 累加到【Join】语句 Left Join 类型
        /// </summary>
        /// <typeparam name="NewT">表名</typeparam>
        /// <returns></returns>
        public NewT LeftJoin<NewT>() where NewT : QTable
        {
            return getSqlBuilder().LeftJoin<NewT>();
        }
        /// <summary>
        /// 累加到【Join】语句 Right Join 类型
        /// </summary>
        /// <typeparam name="NewT">表名</typeparam>
        /// <returns></returns>
        public NewT RightJoin<NewT>() where NewT : QTable
        {
            return getSqlBuilder().RightJoin<NewT>();
        }
        /// <summary>
        /// 累加到【Join】语句 Inner Join 类型
        /// </summary>
        /// <typeparam name="NewT">表名</typeparam>
        /// <returns></returns>
        public NewT InnerJoin<NewT>() where NewT : QTable
        {
            return getSqlBuilder().InnerJoin<NewT>();
        }
        /// <summary>
        /// 累加到【Join】语句 Full Join 类型
        /// </summary>
        /// <typeparam name="NewT">表名</typeparam>
        /// <returns></returns>
        public NewT FullJoin<NewT>() where NewT : QTable
        {
            return getSqlBuilder().FullJoin<NewT>();
        }

    }
}
