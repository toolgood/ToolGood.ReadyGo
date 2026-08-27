using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ToolGood.ReadyGo;
using ToolGood.ReadyGo.NPoco.Expressions;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 异步查询器，提供异步链式查询与结果获取能力。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public class AsyncQueryProvider<T> : IAsyncQueryProviderWithIncludes<T>, INeedDatabase, INeedSql
    {
        /// <summary>
        /// 数据库实例。
        /// </summary>
        protected readonly Database _database;
        /// <summary>
        /// SQL 表达式构建器。
        /// </summary>
        protected ISqlExpression<T> _sqlExpression;
        /// <summary>
        /// 关联查询集合，键为关联标识。
        /// </summary>
        protected Dictionary<string, JoinData> _joinSqlExpressions = new Dictionary<string, JoinData>();
        /// <summary>
        /// 复杂 SQL 构建器。
        /// </summary>
        protected readonly ComplexSqlBuilder<T> _buildComplexSql;
        /// <summary>
        /// 一对多关联的集合属性表达式。
        /// </summary>
        protected Expression<Func<T, IList>> _listExpression = null;
        /// <summary>
        /// 实体元数据。
        /// </summary>
        protected PocoData _pocoData;

        /// <summary>
        /// 使用数据库与筛选条件初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="whereExpression">初始筛选条件。</param>
        public AsyncQueryProvider(Database database, Expression<Func<T, bool>> whereExpression)
        {
            _database = database;
            _pocoData = database.PocoDataFactory.ForType(typeof(T));
            _pocoData.IsQueryGenerated = true;
            _sqlExpression = database.DatabaseType.ExpressionVisitor<T>(database, _pocoData, true);
            _buildComplexSql = new ComplexSqlBuilder<T>(database, _pocoData, _sqlExpression, _joinSqlExpressions);
            _sqlExpression = _sqlExpression.Where(whereExpression);
        }

        /// <summary>
        /// 使用数据库初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        public AsyncQueryProvider(Database database) : this(database, null)
        {
        }

        /// <summary>
        /// 添加筛选条件。
        /// </summary>
        /// <param name="whereExpression">筛选条件表达式。</param>
        protected void AddWhere(Expression<Func<T, bool>> whereExpression)
        {
            if (whereExpression != null)
                _sqlExpression = _sqlExpression.Where(whereExpression);
        }

        /// <summary>
        /// 构建当前查询的 SQL。
        /// </summary>
        /// <returns>查询 SQL。</returns>
        protected Sql BuildSql()
        {
            Sql sql;
            if (_joinSqlExpressions.Any())
                sql = _buildComplexSql.BuildJoin(_database, _sqlExpression, _joinSqlExpressions.Values.ToList(), null, false, false);
            else
                sql = new Sql(true, _sqlExpression.Context.ToSelectStatement(), _sqlExpression.Context.Params);
            return sql;
        }

        /// <summary>
        /// 添加一对多关联加载。
        /// </summary>
        /// <param name="expression">集合属性表达式。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> IncludeMany(Expression<Func<T, IList>> expression, JoinType joinType = JoinType.Left, string joinTableHint = "")
        {
            _listExpression = expression;
            return QueryProviderWithIncludes(expression, null, joinType, joinTableHint);
        }
        
        /// <summary>
        /// 按类型自动加载一对一或外键关联。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProviderWithIncludes<T> Include<T2>(JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class
        {
            var oneToOneMembers = _database.PocoDataFactory.ForType(typeof(T))
                .Members.Where(x => (x.ReferenceType == ReferenceType.OneToOne || x.ReferenceType == ReferenceType.Foreign)
                                    && x.MemberInfoData.MemberType == typeof(T2));

            foreach (var o2oMember in oneToOneMembers)
            {
                var entityParam = Expression.Parameter(typeof(T), "entity");
                var joinProperty = Expression.Lambda<Func<T, T2>>(Expression.PropertyOrField(entityParam, o2oMember.Name), entityParam);
                Include(joinProperty, joinType, joinTableHint);
            }

            return this;
        }

        /// <summary>
        /// 按表达式加载关联。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="expression">关联属性表达式。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProviderWithIncludes<T> Include<T2>(Expression<Func<T, T2>> expression, JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class
        {
            return QueryProviderWithIncludes(expression, null, joinType, joinTableHint);
        }

        /// <summary>
        /// 按表达式加载关联并指定表别名。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="expression">关联属性表达式。</param>
        /// <param name="tableAlias">表别名。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProviderWithIncludes<T> Include<T2>(Expression<Func<T, T2>> expression, string tableAlias, JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class
        {
            return QueryProviderWithIncludes(expression, tableAlias, joinType, joinTableHint);
        }

        /// <summary>
        /// 指定主表别名。
        /// </summary>
        /// <param name="tableAlias">表别名。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProviderWithIncludes<T> UsingAlias(string tableAlias)
        {
            if (!string.IsNullOrEmpty(tableAlias))
                _pocoData.TableInfo.AutoAlias = tableAlias;
            return this;
        }

        /// <summary>
        /// 添加表提示。
        /// </summary>
        /// <param name="tableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProviderWithIncludes<T> Hint(string tableHint)
        {
            _sqlExpression.Hint(tableHint);
            return this;
        }

        private IAsyncQueryProviderWithIncludes<T> QueryProviderWithIncludes(Expression expression, string tableAlias, JoinType joinType, string joinTableHint)
        {
            var joinExpressions = _buildComplexSql.GetJoinExpressions(expression, tableAlias, joinType, joinTableHint);
            foreach (var joinExpression in joinExpressions)
            {
                _joinSqlExpressions[joinExpression.Key] = joinExpression.Value;
            }

            return this;
        }

        /// <summary>
        /// 异步返回结果列表。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>结果列表。</returns>
        public Task<List<T>> ToList_Async(CancellationToken cancellationToken = default)
        {
            return ToEnumerable_Async(cancellationToken).ToListAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步返回结果列表（ToList_Async 的别名）。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>结果列表。</returns>
        public Task<List<T>> Select_Async(CancellationToken cancellationToken = default)
        {
            return ToList_Async(cancellationToken);
        }

        /// <summary>
        /// 异步返回结果数组。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>结果数组。</returns>
        public Task<T[]> ToArray_Async(CancellationToken cancellationToken = default)
        {
            return ToEnumerable_Async(cancellationToken).ToArrayAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 返回异步枚举序列。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>异步枚举序列。</returns>
        public IAsyncEnumerable<T> ToEnumerable_Async(CancellationToken cancellationToken = default)
        {
            return ExecuteQuery_Async(BuildSql(), cancellationToken);
        }

        private IAsyncEnumerable<T> ExecuteQuery_Async(Sql sql, CancellationToken cancellationToken)
        {
            return _database.QueryAsync<T>(default, _listExpression, null, sql, _pocoData, cancellationToken);
        }

        /// <summary>
        /// 异步返回第一个元素或默认值。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素或默认值。</returns>
        public Task<T> FirstOrDefault_Async(CancellationToken cancellationToken = default)
        {
            return FirstOrDefault_Async(null, cancellationToken);
        }

        /// <summary>
        /// 异步返回满足条件的第一个元素或默认值。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素或默认值。</returns>
        public Task<T> FirstOrDefault_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            AddWhere(whereExpression);
            return ToEnumerable_Async(cancellationToken).FirstOrDefaultAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步返回第一个元素。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素。</returns>
        public Task<T> First_Async(CancellationToken cancellationToken = default)
        {
            return First_Async(null, cancellationToken);
        }

        /// <summary>
        /// 异步返回满足条件的第一个元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>第一个元素。</returns>
        public Task<T> First_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            AddWhere(whereExpression);
            return ToEnumerable_Async(cancellationToken).FirstAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步返回唯一元素或默认值。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素或默认值。</returns>
        public Task<T> SingleOrDefault_Async(CancellationToken cancellationToken = default)
        {
            return SingleOrDefault_Async(null, cancellationToken);
        }

        /// <summary>
        /// 异步返回满足条件的唯一元素或默认值。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素或默认值。</returns>
        public Task<T> SingleOrDefault_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            AddWhere(whereExpression);
            return ToEnumerable_Async(cancellationToken).SingleOrDefaultAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步返回唯一元素。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素。</returns>
        public Task<T> Single_Async(CancellationToken cancellationToken = default)
        {
            return Single_Async(null, cancellationToken);
        }

        /// <summary>
        /// 异步返回满足条件的唯一元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>唯一元素。</returns>
        public Task<T> Single_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            AddWhere(whereExpression);
            return ToEnumerable_Async(cancellationToken).SingleAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步返回元素数量。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        public Task<int> Count_Async(CancellationToken cancellationToken = default)
        {
            return Count_Async(null, cancellationToken);
        }

        /// <summary>
        /// 异步统计元素数量（Count_Async 的别名）。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        public Task<int> SelectCount_Async(CancellationToken cancellationToken = default)
        {
            return Count_Async(null, cancellationToken);
        }

        /// <summary>
        /// 异步返回满足条件的元素数量。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        public Task<int> Count_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            AddWhere(whereExpression);
            var sql = _buildComplexSql.BuildJoin(_database, _sqlExpression, _joinSqlExpressions.Values.ToList(), null, true, false);
            return _database.ExecuteScalarAsync<int>(sql, cancellationToken);
        }

        /// <summary>
        /// 异步统计满足条件的元素数量（Count_Async 的别名）。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>元素数量。</returns>
        public Task<int> SelectCount_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return Count_Async(whereExpression, cancellationToken);
        }

        /// <summary>
        /// 异步判断是否存在元素。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public Task<bool> Any_Async(CancellationToken cancellationToken = default)
        {
            return Any_Async(null, cancellationToken);
        }

        /// <summary>
        /// 异步判断是否存在满足条件的元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public async Task<bool> Any_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return (await Count_Async(whereExpression, cancellationToken).ConfigureAwait(false)) > 0;
        }

        /// <summary>
        /// 异步判断是否存在元素（Any_Async 的别名）。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public Task<bool> Exists_Async(CancellationToken cancellationToken = default)
        {
            return Any_Async(null, cancellationToken);
        }

        /// <summary>
        /// 异步判断是否存在满足条件的元素（Any_Async 的别名）。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public Task<bool> Exists_Async(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return Any_Async(whereExpression, cancellationToken);
        }

        /// <summary>
        /// 异步分页返回结果。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>分页结果。</returns>
        public async Task<Page<T>> ToPage_Async(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            int offset = (page - 1) * pageSize;

            // Setup the paged result
            var result = new Page<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.TotalItems = await Count_Async(cancellationToken).ConfigureAwait(false);
            result.TotalPages = result.TotalItems / pageSize;
            if ((result.TotalItems % pageSize) != 0)
                result.TotalPages++;

            _sqlExpression = _sqlExpression.Limit(offset, pageSize);

            result.Items = await ToList_Async(cancellationToken).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// 异步分页返回结果（ToPage_Async 的别名）。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>分页结果。</returns>
        public Task<Page<T>> Page_Async(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return ToPage_Async(page, pageSize, cancellationToken);
        }

        /// <summary>
        /// 异步分页返回结果列表（仅返回当前页数据）。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>当前页数据列表。</returns>
        public async Task<List<T>> SelectPage_Async(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var result = await ToPage_Async(page, pageSize, cancellationToken).ConfigureAwait(false);
            return result.Items;
        }

        /// <summary>
        /// 异步投影返回结果列表。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>投影结果列表。</returns>
        public Task<List<T2>> ProjectTo_Async<T2>(Expression<Func<T, T2>> projectionExpression, CancellationToken cancellationToken = default)
        {
            var sql = _buildComplexSql.GetSqlForProjection(projectionExpression, false);
            return ExecuteQuery_Async(sql, cancellationToken).Select(projectionExpression.Compile()).ToListAsync(cancellationToken).AsTask();
        }
        
        /// <summary>
        /// 异步投影分页返回结果。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>投影分页结果。</returns>
        public async Task<Page<T2>> ToProjectedPage_Async<T2>(Expression<Func<T, T2>> projectionExpression, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            int offset = (page - 1) * pageSize;

            // Setup the paged result
            var result = new Page<T2>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.TotalItems = await Count_Async().ConfigureAwait(false);
            result.TotalPages = result.TotalItems / pageSize;
            if ((result.TotalItems % pageSize) != 0)
                result.TotalPages++;

            var sql = _buildComplexSql.GetSqlForProjection(projectionExpression, false, offset, pageSize);
            result.Items = await ExecuteQuery_Async(sql, cancellationToken).Select(projectionExpression.Compile()).ToListAsync(cancellationToken).AsTask();

            return result;
        }

        /// <summary>
        /// 异步去重返回结果列表。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>去重后的结果列表。</returns>
        public Task<List<T>> Distinct_Async(CancellationToken cancellationToken = default)
        {
            return ExecuteQuery_Async(new Sql(_sqlExpression.Context.ToSelectStatement(true, true), _sqlExpression.Context.Params), cancellationToken).ToListAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步按投影去重返回结果列表。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>去重后的投影结果列表。</returns>
        public Task<List<T2>> Distinct_Async<T2>(Expression<Func<T, T2>> projectionExpression, CancellationToken cancellationToken = default)
        {
            var sql = _buildComplexSql.GetSqlForProjection(projectionExpression, true);
            return ExecuteQuery_Async(sql, cancellationToken).Select(projectionExpression.Compile()).ToListAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 添加 WHERE 条件。
        /// </summary>
        /// <param name="whereExpression">条件表达式。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression)
        {
            _sqlExpression = _sqlExpression.Where(whereExpression);
            return this;
        }

        /// <summary>
        /// 添加 WHERE 条件 SQL。
        /// </summary>
        /// <param name="sql">条件 SQL。</param>
        /// <param name="args">条件参数。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> WhereSql(string sql, params object[] args)
        {
            _sqlExpression = _sqlExpression.Where(sql, args);
            return this;
        }

        /// <summary>
        /// 添加 WHERE 条件 SQL。
        /// </summary>
        /// <param name="sql">条件 SQL。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> WhereSql(Sql sql)
        {
            _sqlExpression = _sqlExpression.Where(sql.SQL, sql.Arguments);
            return this;
        }

        /// <summary>
        /// 添加 WHERE 条件 SQL（通过查询上下文构建）。
        /// </summary>
        /// <param name="queryBuilder">查询上下文构建函数。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> WhereSql(Func<QueryContext<T>, Sql> queryBuilder)
        {
            var sql = queryBuilder(new QueryContext<T>(_database, _pocoData, _joinSqlExpressions));
            return WhereSql(sql);
        }

        /// <summary>
        /// 限制返回行数。
        /// </summary>
        /// <param name="rows">返回行数。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> Limit(int rows)
        {
            ThrowIfOneToMany();
            _sqlExpression = _sqlExpression.Limit(rows);
            return this;
        }

        /// <summary>
        /// 限制返回行数并跳过指定行数。
        /// </summary>
        /// <param name="skip">跳过的行数。</param>
        /// <param name="rows">返回行数。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> Limit(int skip, int rows)
        {
            ThrowIfOneToMany();
            _sqlExpression = _sqlExpression.Limit(skip, rows);
            return this;
        }

        private void ThrowIfOneToMany()
        {
            if (_listExpression != null)
            {
                throw new NotImplementedException("One to many queries with paging is not implemented");
            }
        }

        /// <summary>
        /// 添加升序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> OrderBy(Expression<Func<T, object>> column)
        {
            _sqlExpression = _sqlExpression.OrderBy(column);
            return this;
        }

        /// <summary>
        /// 添加降序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> OrderByDescending(Expression<Func<T, object>> column)
        {
            _sqlExpression = _sqlExpression.OrderByDescending(column);
            return this;
        }

        /// <summary>
        /// 追加升序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> ThenBy(Expression<Func<T, object>> column)
        {
            _sqlExpression = _sqlExpression.ThenBy(column);
            return this;
        }

        /// <summary>
        /// 追加降序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> ThenByDescending(Expression<Func<T, object>> column)
        {
            _sqlExpression = _sqlExpression.ThenByDescending(column);
            return this;
        }

        /// <summary>
        /// 应用查询构建器中的条件、排序与分页。
        /// </summary>
        /// <param name="builder">查询构建器。</param>
        /// <returns>当前查询器。</returns>
        public IAsyncQueryProvider<T> From(QueryBuilder<T> builder)
        {
            if (!builder.Data.Skip.HasValue && builder.Data.Rows.HasValue)
            {
                Limit(builder.Data.Rows.Value);
            }

            if (builder.Data.Skip.HasValue && builder.Data.Rows.HasValue)
            {
                Limit(builder.Data.Skip.Value, builder.Data.Rows.Value);
            }

            if (builder.Data.WhereExpression != null)
            {
                Where(builder.Data.WhereExpression);
            }

            if (builder.Data.OrderByExpression != null)
            {
                OrderBy(builder.Data.OrderByExpression);
            }

            if (builder.Data.OrderByDescendingExpression != null)
            {
                OrderByDescending(builder.Data.OrderByDescendingExpression);
            }

            if (builder.Data.ThenByExpression.Any())
            {
                foreach (var expression in builder.Data.ThenByExpression)
                {
                    ThenBy(expression);
                }
            }

            if (builder.Data.ThenByDescendingExpression.Any())
            {
                foreach (var expression in builder.Data.ThenByDescendingExpression)
                {
                    ThenByDescending(expression);
                }
            }

            return this;
        }

        /// <summary>
        /// 返回动态对象列表。
        /// </summary>
        /// <returns>动态对象列表。</returns>
        public List<dynamic> ToDynamicList()
        {
            return ToDynamicEnumerable().ToList();
        }

        /// <summary>
        /// 返回动态对象枚举序列。
        /// </summary>
        /// <returns>动态对象枚举序列。</returns>
        public IEnumerable<dynamic> ToDynamicEnumerable()
        {
            var sql = BuildSql();
            return _database.QueryImp<dynamic>(null, null, null, sql);
        }

        IDatabase INeedDatabase.GetDatabase()
        {
            return _database;
        }

        Sql INeedSql.GetSql()
        {
            return BuildSql();
        }
    }

    /// <summary>
    /// 提供当前查询 SQL 的接口。
    /// </summary>
    public interface INeedSql
    {
        /// <summary>
        /// 获取当前查询 SQL。
        /// </summary>
        /// <returns>查询 SQL。</returns>
        Sql GetSql();
    }

    /// <summary>
    /// 提供当前数据库实例的接口。
    /// </summary>
    public interface INeedDatabase
    {
        /// <summary>
        /// 获取数据库实例。
        /// </summary>
        /// <returns>数据库实例。</returns>
        IDatabase GetDatabase();
    }

    /// <summary>
    /// 同步查询器，提供同步链式查询与结果获取能力。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public class QueryProvider<T> : AsyncQueryProvider<T>, IQueryProviderWithIncludes<T>
    {

        /// <summary>
        /// 使用数据库与筛选条件初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="whereExpression">初始筛选条件。</param>
        public QueryProvider(Database database, Expression<Func<T, bool>> whereExpression) : base(database, whereExpression)
        {
        }

        /// <summary>
        /// 使用数据库初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        public QueryProvider(Database database) : base(database, null)
        {
        }

        /// <summary>
        /// 返回第一个元素或默认值。
        /// </summary>
        /// <returns>第一个元素或默认值。</returns>
        public T FirstOrDefault()
        {
            return FirstOrDefault(null);
        }

        /// <summary>
        /// 返回满足条件的第一个元素或默认值。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>第一个元素或默认值。</returns>
        public T FirstOrDefault(Expression<Func<T, bool>> whereExpression)
        {
            AddWhere(whereExpression);
            return ToEnumerable().FirstOrDefault();
        }

        /// <summary>
        /// 返回第一个元素。
        /// </summary>
        /// <returns>第一个元素。</returns>
        public T First()
        {
            return First(null);
        }

        /// <summary>
        /// 返回满足条件的第一个元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>第一个元素。</returns>
        public T First(Expression<Func<T, bool>> whereExpression)
        {
            AddWhere(whereExpression);
            return ToEnumerable().First();
        }

        /// <summary>
        /// 返回唯一元素或默认值。
        /// </summary>
        /// <returns>唯一元素或默认值。</returns>
        public T SingleOrDefault()
        {
            return SingleOrDefault(null);
        }

        /// <summary>
        /// 返回满足条件的唯一元素或默认值。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>唯一元素或默认值。</returns>
        public T SingleOrDefault(Expression<Func<T, bool>> whereExpression)
        {
            AddWhere(whereExpression);
            return ToEnumerable().SingleOrDefault();
        }

        /// <summary>
        /// 返回唯一元素。
        /// </summary>
        /// <returns>唯一元素。</returns>
        public T Single()
        {
            return Single(null);
        }

        /// <summary>
        /// 返回满足条件的唯一元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>唯一元素。</returns>
        public T Single(Expression<Func<T, bool>> whereExpression)
        {
            AddWhere(whereExpression);
            return ToEnumerable().Single();
        }

        /// <summary>
        /// 返回元素数量。
        /// </summary>
        /// <returns>元素数量。</returns>
        public int Count()
        {
            return Count(null);
        }

        /// <summary>
        /// 统计元素数量（Count 的别名）。
        /// </summary>
        /// <returns>元素数量。</returns>
        public int SelectCount()
        {
            return Count(null);
        }

        /// <summary>
        /// 返回满足条件的元素数量。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>元素数量。</returns>
        public int Count(Expression<Func<T, bool>> whereExpression)
        {
            AddWhere(whereExpression);
            var sql = _buildComplexSql.BuildJoin(_database, _sqlExpression, _joinSqlExpressions.Values.ToList(), null, true, false);
            return _database.ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// 统计满足条件的元素数量（Count 的别名）。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>元素数量。</returns>
        public int SelectCount(Expression<Func<T, bool>> whereExpression)
        {
            return Count(whereExpression);
        }

        /// <summary>
        /// 判断是否存在元素。
        /// </summary>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public bool Any()
        {
            return Count() > 0;
        }

        /// <summary>
        /// 判断是否存在满足条件的元素。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public bool Any(Expression<Func<T, bool>> whereExpression)
        {
            return Count(whereExpression) > 0;
        }

        /// <summary>
        /// 判断是否存在元素（Any 的别名）。
        /// </summary>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public bool Exists()
        {
            return Any();
        }

        /// <summary>
        /// 判断是否存在满足条件的元素（Any 的别名）。
        /// </summary>
        /// <param name="whereExpression">筛选条件。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public bool Exists(Expression<Func<T, bool>> whereExpression)
        {
            return Any(whereExpression);
        }

        /// <summary>
        /// 分页返回结果。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <returns>分页结果。</returns>
        public Page<T> ToPage(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            int offset = (page - 1) * pageSize;

            // Setup the paged result
            var result = new Page<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.TotalItems = Count();
            result.TotalPages = result.TotalItems / pageSize;
            if ((result.TotalItems % pageSize) != 0)
                result.TotalPages++;

            _sqlExpression = _sqlExpression.Limit(offset, pageSize);

            result.Items = ToList();

            return result;
        }

        /// <summary>
        /// 分页返回结果（ToPage 的别名）。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <returns>分页结果。</returns>
        public Page<T> Page(int page, int pageSize)
        {
            return ToPage(page, pageSize);
        }

        /// <summary>
        /// 分页返回结果列表（仅返回当前页数据）。
        /// </summary>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <returns>当前页数据列表。</returns>
        public List<T> SelectPage(int page, int pageSize)
        {
            return ToPage(page, pageSize).Items;
        }

        /// <summary>
        /// 投影返回结果列表。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <returns>投影结果列表。</returns>
        public List<T2> ProjectTo<T2>(Expression<Func<T, T2>> projectionExpression)
        {
            var sql = _buildComplexSql.GetSqlForProjection(projectionExpression, false);
            return ExecuteQuery(sql).Select(projectionExpression.Compile()).ToList();
        }

        /// <summary>
        /// 投影分页返回结果。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <returns>投影分页结果。</returns>
        public Page<T2> ToProjectedPage<T2>(Expression<Func<T, T2>> projectionExpression, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            int offset = (page - 1) * pageSize;

            // Setup the paged result
            var result = new Page<T2>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.TotalItems = Count();
            result.TotalPages = result.TotalItems / pageSize;
            if ((result.TotalItems % pageSize) != 0)
                result.TotalPages++;

            var sql = _buildComplexSql.GetSqlForProjection(projectionExpression, false, offset, pageSize);
            result.Items = ExecuteQuery(sql).Select(projectionExpression.Compile()).ToList();

            return result;
        }

        /// <summary>
        /// 按投影去重返回结果列表。
        /// </summary>
        /// <typeparam name="T2">投影结果类型。</typeparam>
        /// <param name="projectionExpression">投影表达式。</param>
        /// <returns>去重后的投影结果列表。</returns>
        public List<T2> Distinct<T2>(Expression<Func<T, T2>> projectionExpression)
        {
            var sql = _buildComplexSql.GetSqlForProjection(projectionExpression, true);
            return ExecuteQuery(sql).Select(projectionExpression.Compile()).ToList();
        }

        /// <summary>
        /// 去重返回结果列表。
        /// </summary>
        /// <returns>去重后的结果列表。</returns>
        public List<T> Distinct()
        {
            return ExecuteQuery(new Sql(_sqlExpression.Context.ToSelectStatement(true, true), _sqlExpression.Context.Params)).ToList();
        }
        /// <summary>
        /// 返回结果数组。
        /// </summary>
        /// <returns>结果数组。</returns>
        public T[] ToArray()
        {
            return ToEnumerable().ToArray();
        }

        /// <summary>
        /// 返回结果列表。
        /// </summary>
        /// <returns>结果列表。</returns>
        public List<T> ToList()
        {
            return ToEnumerable().ToList();
        }

        /// <summary>
        /// 返回结果列表（ToList 的别名）。
        /// </summary>
        /// <returns>结果列表。</returns>
        public List<T> Select()
        {
            return ToList();
        }

        /// <summary>
        /// 返回枚举序列。
        /// </summary>
        /// <returns>枚举序列。</returns>
        public IEnumerable<T> ToEnumerable()
        {
            return ExecuteQuery(BuildSql());
        }
        
        private IEnumerable<T> ExecuteQuery(Sql sql)
        {
            return _database.QueryImp(default(T), _listExpression, null, sql, _pocoData);
        }

        /// <summary>
        /// 添加一对多关联加载。
        /// </summary>
        /// <param name="expression">集合属性表达式。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> IncludeMany(Expression<Func<T, IList>> expression, JoinType joinType = JoinType.Left, string joinTableHint = "")
        {
            return (IQueryProvider<T>)base.IncludeMany(expression, joinType, joinTableHint);
        }

        /// <summary>
        /// 按类型自动加载一对一或外键关联。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProviderWithIncludes<T> Include<T2>(JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class
        {
            return (IQueryProviderWithIncludes<T>)base.Include<T2>(joinType, joinTableHint);
        }

        /// <summary>
        /// 按表达式加载关联。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="expression">关联属性表达式。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProviderWithIncludes<T> Include<T2>(Expression<Func<T, T2>> expression, JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class
        {
            return (IQueryProviderWithIncludes<T>)base.Include(expression, joinType, joinTableHint);
        }

        /// <summary>
        /// 按表达式加载关联并指定表别名。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="expression">关联属性表达式。</param>
        /// <param name="tableAlias">表别名。</param>
        /// <param name="joinType">关联类型。</param>
        /// <param name="joinTableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProviderWithIncludes<T> Include<T2>(Expression<Func<T, T2>> expression, string tableAlias, JoinType joinType = JoinType.Left, string joinTableHint = "") where T2 : class
        {
            return (IQueryProviderWithIncludes<T>)base.Include(expression, tableAlias, joinType, joinTableHint);
        }

        /// <summary>
        /// 指定主表别名。
        /// </summary>
        /// <param name="tableAlias">表别名。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProviderWithIncludes<T> UsingAlias(string tableAlias)
        {
            return (IQueryProviderWithIncludes<T>)base.UsingAlias(tableAlias);
        }

        /// <summary>
        /// 添加表提示。
        /// </summary>
        /// <param name="tableHint">表提示。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProviderWithIncludes<T> Hint(string tableHint)
        {
            return (IQueryProviderWithIncludes<T>)base.Hint(tableHint);
        }

        /// <summary>
        /// 添加 WHERE 条件。
        /// </summary>
        /// <param name="whereExpression">条件表达式。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression)
        {
            return (IQueryProvider<T>)base.Where(whereExpression);
        }

        /// <summary>
        /// 添加 WHERE 条件 SQL。
        /// </summary>
        /// <param name="sql">条件 SQL。</param>
        /// <param name="args">条件参数。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> WhereSql(string sql, params object[] args)
        {
            return (IQueryProvider<T>)base.WhereSql(sql, args);
        }

        /// <summary>
        /// 添加 WHERE 条件 SQL。
        /// </summary>
        /// <param name="sql">条件 SQL。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> WhereSql(Sql sql)
        {
            return (IQueryProvider<T>)base.WhereSql(sql);
        }

        /// <summary>
        /// 添加 WHERE 条件 SQL（通过查询上下文构建）。
        /// </summary>
        /// <param name="queryBuilder">查询上下文构建函数。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> WhereSql(Func<QueryContext<T>, Sql> queryBuilder)
        {
            return (IQueryProvider<T>)base.WhereSql(queryBuilder);
        }

        /// <summary>
        /// 添加升序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> OrderBy(Expression<Func<T, object>> column)
        {
            return (IQueryProvider<T>)base.OrderBy(column);
        }

        /// <summary>
        /// 添加降序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> OrderByDescending(Expression<Func<T, object>> column)
        {
            return (IQueryProvider<T>)base.OrderByDescending(column);
        }

        /// <summary>
        /// 追加升序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> ThenBy(Expression<Func<T, object>> column)
        {
            return (IQueryProvider<T>)base.ThenBy(column);
        }

        /// <summary>
        /// 追加降序排序字段。
        /// </summary>
        /// <param name="column">排序字段表达式。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> ThenByDescending(Expression<Func<T, object>> column)
        {
            return (IQueryProvider<T>)base.ThenByDescending(column);
        }

        /// <summary>
        /// 限制返回行数。
        /// </summary>
        /// <param name="rows">返回行数。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> Limit(int rows)
        {
            return (IQueryProvider<T>)base.Limit(rows);
        }

        /// <summary>
        /// 限制返回行数并跳过指定行数。
        /// </summary>
        /// <param name="skip">跳过的行数。</param>
        /// <param name="rows">返回行数。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> Limit(int skip, int rows)
        {
            return (IQueryProvider<T>)base.Limit(skip, rows);
        }

        /// <summary>
        /// 应用查询构建器中的条件、排序与分页。
        /// </summary>
        /// <param name="builder">查询构建器。</param>
        /// <returns>当前查询器。</returns>
        public new IQueryProvider<T> From(QueryBuilder<T> builder)
        {
            return (IQueryProvider<T>)base.From(builder);
        }
    }
}
