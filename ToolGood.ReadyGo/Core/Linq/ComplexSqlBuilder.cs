using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ToolGood.ReadyGo.NPoco.Expressions;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 复杂查询（含投影、关联）SQL 构建器。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public class ComplexSqlBuilder<T>
    {
        private readonly IDatabase _database;
        private readonly PocoData _pocoData;
        private readonly ISqlExpression<T> _sqlExpression;
        private readonly Dictionary<string, JoinData> _joinSqlExpressions;

        /// <summary>
        /// 使用数据库、Poco 元数据、SQL 表达式与关联表达式初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pocoData">实体的 Poco 元数据。</param>
        /// <param name="sqlExpression">SQL 表达式。</param>
        /// <param name="joinSqlExpressions">关联查询表达式集合。</param>
        public ComplexSqlBuilder(IDatabase database, PocoData pocoData, ISqlExpression<T> sqlExpression, Dictionary<string, JoinData> joinSqlExpressions)
        {
            _database = database;
            _pocoData = pocoData;
            _sqlExpression = sqlExpression;
            _joinSqlExpressions = joinSqlExpressions;
        }

        /// <summary>
        /// 为投影查询生成 SQL。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="distinct">是否去重。</param>
        /// <returns>投影查询 SQL。</returns>
        public Sql GetSqlForProjection<T2>(Expression<Func<T, T2>> projectionExpression, bool distinct)
        {
            var selectMembers = _database.DatabaseType.ExpressionVisitor<T>(_database, _pocoData).SelectProjection(projectionExpression);

            ((ISqlExpression)_sqlExpression).SelectMembers.Clear();
            ((ISqlExpression)_sqlExpression).SelectMembers.AddRange(selectMembers);

            if (!_joinSqlExpressions.Any())
            {
                var finalsql = ((ISqlExpression)_sqlExpression).ApplyPaging(_sqlExpression.Context.ToSelectStatement(false, distinct), selectMembers.Select(x => x.PocoColumns), _joinSqlExpressions);
                return new Sql(finalsql, _sqlExpression.Context.Params);
            }

            var sql = BuildJoin(_database, _sqlExpression, _joinSqlExpressions.Values.ToList(), selectMembers, false, distinct);
            return sql;
        }
                
        /// <summary>
        /// 为投影查询生成带分页的 SQL。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="distinct">是否去重。</param>
        /// <param name="skip">跳过的行数。</param>
        /// <param name="rows">返回行数。</param>
        /// <returns>投影查询 SQL。</returns>
        public Sql GetSqlForProjection<T2>(Expression<Func<T, T2>> projectionExpression, bool distinct, int skip, int rows)
        {
            var selectMembers = _database.DatabaseType.ExpressionVisitor<T>(_database, _pocoData).SelectProjection(projectionExpression);

            ((ISqlExpression)_sqlExpression).SelectMembers.Clear();
            ((ISqlExpression)_sqlExpression).SelectMembers.AddRange(selectMembers);

            _sqlExpression.Limit(skip, rows);

            if (!_joinSqlExpressions.Any())
            {
                var finalsql = ((ISqlExpression)_sqlExpression).ApplyPaging(_sqlExpression.Context.ToSelectStatement(false, distinct), selectMembers.Select(x => x.PocoColumns), _joinSqlExpressions);
                return new Sql(finalsql, _sqlExpression.Context.Params);
            }

            var sql = BuildJoin(_database, _sqlExpression, _joinSqlExpressions.Values.ToList(), selectMembers, false, distinct);
            return sql;
        }

        /// <summary>
        /// 构建关联查询 SQL。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="sqlExpression">SQL 表达式。</param>
        /// <param name="joinSqlExpressions">关联查询表达式集合。</param>
        /// <param name="newMembers">用于覆盖查询列的成员集合。</param>
        /// <param name="count">是否生成 COUNT 语句。</param>
        /// <param name="distinct">是否去重。</param>
        /// <returns>关联查询 SQL。</returns>
        public Sql BuildJoin(IDatabase database, ISqlExpression<T> sqlExpression, List<JoinData> joinSqlExpressions, List<SelectMember> newMembers, bool count, bool distinct)
        {
            var modelDef = _pocoData;
            var sqlTemplate = count
                ? "SELECT COUNT(*) FROM {1} {2} {3} {4}"
                : "SELECT {0} FROM {1} {2} {3} {4}";

            // build cols
            var cols = modelDef.QueryColumns
                .Select(x => x.Value)
                .Select((x, j) =>
                {
                    var col = new StringPocoCol();
                    col.StringCol = database.DatabaseType.EscapeTableName(x.TableInfo.AutoAlias) + "." +
                                    database.DatabaseType.EscapeSqlIdentifier(x.ColumnName) + " as " + database.DatabaseType.EscapeSqlIdentifier(x.MemberInfoKey);
                    col.PocoColumn = new[] { x };
                    return col;
                }).ToList();

            // build wheres
            var where = sqlExpression.Context.ToWhereStatement();
            where = (string.IsNullOrEmpty(where) ? string.Empty : "\n" + where);

            // build joins and add cols
            var joins = BuildJoinSql(database, joinSqlExpressions, ref cols);

            // build orderbys
            ISqlExpression exp = sqlExpression;
            var orderbys = string.Empty;
            if (!count && exp.OrderByMembers.Any())
            {
                var orderMembers = exp.OrderByMembers.Select(x =>
                {
                    return new
                    {
                        Column = x.PocoColumns.Last().MemberInfoKey,
                        x.AscDesc
                    };
                }).ToList();

                orderbys = "\nORDER BY " + string.Join(", ", orderMembers.Select(x => database.DatabaseType.EscapeSqlIdentifier(x.Column) + " " + x.AscDesc).ToArray());
            }

            // Override select columns with projected ones
            if (newMembers != null)
            {
                var selectMembers = exp.OrderByMembers
                    .Select(x => new SelectMember() {PocoColumn = x.PocoColumn, EntityType = x.EntityType, PocoColumns = x.PocoColumns})
                    .Where(x => !newMembers.Any(y => y.EntityType == x.EntityType && y.PocoColumns.SequenceEqual(x.PocoColumns)));

                cols = newMembers.Concat(selectMembers).Select(x =>
                {
                    return new StringPocoCol
                    {
                        StringCol = database.DatabaseType.EscapeTableName(x.PocoColumn.TableInfo.AutoAlias) + "." +
                                    database.DatabaseType.EscapeSqlIdentifier(x.PocoColumn.ColumnName) + " as " + database.DatabaseType.EscapeSqlIdentifier(x.PocoColumns.Last().MemberInfoKey),
                        PocoColumn = x.PocoColumns
                    };
                }).ToList();
            }

            // replace templates
            var resultantSql = string.Format(sqlTemplate,
                (distinct ? "DISTINCT " : "") + string.Join(", ", cols.Select(x=>x.StringCol).ToArray()),
                database.DatabaseType.EscapeTableName(modelDef.TableInfo.TableName) + " " + database.DatabaseType.EscapeTableName(modelDef.TableInfo.AutoAlias) + exp.TableHint,
                joins,
                where,
                orderbys);

            var newsql = count ? resultantSql : exp.ApplyPaging(resultantSql, cols.Select(x=>x.PocoColumn), _joinSqlExpressions);

            return new Sql(newsql, _sqlExpression.Context.Params);
        }

        private static IEnumerable<PocoColumn> GetJoiPocoColumns(IEnumerable<PocoMember> members)
        {
            foreach (var member in members)
            {
                switch (member.ReferenceType)
                {
                    case ReferenceType.Foreign:
                        break;
                    case ReferenceType.None:
                    {
                        yield return member.PocoColumn;
                        foreach (var pocoMemberChild in GetJoiPocoColumns(member.PocoMemberChildren))
                        {
                            yield return pocoMemberChild;
                        }
                        break;
                    }
                }
            }
        }

        private static string BuildJoinSql(IDatabase database, List<JoinData> joinSqlExpressions, ref List<StringPocoCol> cols)
        {
            var joins = new List<string>();

            foreach (var joinSqlExpression in joinSqlExpressions)
            {
                var member = joinSqlExpression.PocoMemberJoin;

                cols = cols.Concat(GetJoiPocoColumns(joinSqlExpression.PocoMembers)
                    .Where(x => x != null && !x.ResultColumn)
                    .Select(x => new StringPocoCol
                {
                    StringCol = database.DatabaseType.EscapeTableName(x.TableInfo.AutoAlias)
                                + "." + database.DatabaseType.EscapeSqlIdentifier(x.ColumnName) + " as " + database.DatabaseType.EscapeSqlIdentifier(x.MemberInfoKey),
                    PocoColumn = new[] { x }
                })).ToList();

                joins.Add(string.Format("  {0} JOIN " + database.DatabaseType.EscapeTableName(member.PocoColumn.TableInfo.TableName) + " " + database.DatabaseType.EscapeTableName(member.PocoColumn.TableInfo.AutoAlias) + joinSqlExpression.Hint + " ON " + joinSqlExpression.OnSql, joinSqlExpression.JoinType == JoinType.Inner ? "INNER" : "LEFT"));
            }

            return joins.Any() ? " \n" + string.Join(" \n", joins.ToArray()) : string.Empty;
        }

        /// <summary>
        /// 根据表达式解析关联查询信息。
        /// </summary>
        /// <param name="expression">关联表达式。</param>
        /// <param name="tableAlias">表别名。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="hint">表提示。</param>
        /// <returns>关联查询表达式集合。</returns>
        public Dictionary<string, JoinData> GetJoinExpressions(Expression expression, string tableAlias, JoinType joinType, string hint)
        {
            var memberInfos = MemberChainHelper.GetMembers(expression);
            var members = _pocoData.Members;
            var joinExpressions = new Dictionary<string, JoinData>();

            foreach (var memberInfo in memberInfos)
            {
                var pocoMember = members
                    .Where(x => x.ReferenceType != ReferenceType.None)
                    .Single(x => x.MemberInfoData.MemberInfo.Name == memberInfo.Name);

                var pocoColumn1 = pocoMember.PocoColumn;
                var pocoMember2 = pocoMember.PocoMemberChildren.Single(x => x.Name == pocoMember.ReferenceMemberName);
                var pocoColumn2 = pocoMember2.PocoColumn;

                pocoColumn2.TableInfo.AutoAlias = tableAlias ?? pocoColumn2.TableInfo.AutoAlias;

                var onSql = _database.DatabaseType.EscapeTableName(pocoColumn1.TableInfo.AutoAlias)
                            + "." + _database.DatabaseType.EscapeSqlIdentifier(pocoColumn1.ColumnName)
                            + " = " + _database.DatabaseType.EscapeTableName(pocoColumn2.TableInfo.AutoAlias)
                            + "." + _database.DatabaseType.EscapeSqlIdentifier(pocoColumn2.ColumnName);

                if (!joinExpressions.ContainsKey(onSql))
                {
                    joinExpressions.Add(onSql, new JoinData()
                    {
                        OnSql = onSql,
                        PocoMember = pocoMember,
                        PocoMemberJoin = pocoMember2,
                        PocoMembers = pocoMember.PocoMemberChildren,
                        JoinType = joinType,
                        Hint = hint == string.Empty ? hint : " " + hint
                    });
                }

                members = pocoMember.PocoMemberChildren;
            }

            return joinExpressions;
        }
    }

    /// <summary>
    /// 表示字符串形式的查询列及其列信息。
    /// </summary>
    public class StringPocoCol
    {
        /// <summary>
        /// 列的字符串 SQL。
        /// </summary>
        public string StringCol { get; set; }
        /// <summary>
        /// 关联的列信息数组。
        /// </summary>
        public PocoColumn[] PocoColumn { get; set; }
    }
}