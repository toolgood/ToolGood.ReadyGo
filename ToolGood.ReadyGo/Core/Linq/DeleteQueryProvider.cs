using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ToolGood.ReadyGo.NPoco.Expressions;

namespace ToolGood.ReadyGo.NPoco.Linq
{

    /// <summary>
    /// 同步删除查询器。
    /// </summary>
    /// <typeparam name="T">删除的实体类型。</typeparam>
    public class DeleteQueryProvider<T> : AsyncDeleteQueryProvider<T>, IDeleteQueryProvider<T>
    {
        /// <summary>
        /// 使用数据库初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        public DeleteQueryProvider(IDatabase database) : base(database)
        {
        }

        /// <summary>
        /// 添加删除条件。
        /// </summary>
        /// <param name="whereExpression">删除条件表达式。</param>
        /// <returns>当前删除查询器。</returns>
        public new IDeleteQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression)
        {
            return (IDeleteQueryProvider<T>)base.Where(whereExpression);
        }
#pragma warning disable CS0109
        /// <summary>
        /// 执行删除。
        /// </summary>
        /// <returns>受影响的行数。</returns>
        public new int Execute()
        {
            return _database.Execute(_sqlExpression.Context.ToDeleteStatement(), _sqlExpression.Context.Params);
        }
#pragma warning restore CS0109

        #region 动态条件便捷方法

        /// <summary>
        /// 条件成立时添加删除条件。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhere(bool condition, Expression<Func<T, bool>> predicate)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhere(condition, predicate);
        }

        /// <summary>
        /// Where Exists（自动添加 "EXISTS(" 与 "SELECT * " 前缀）。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereExists(string sql, params object[] args)
        {
            return (IDeleteQueryProvider<T>)base.WhereExists(sql, args);
        }

        /// <summary>
        /// Where Not Exists（自动添加 "NOT EXISTS(" 前缀）。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereNotExists(string sql, params object[] args)
        {
            return (IDeleteQueryProvider<T>)base.WhereNotExists(sql, args);
        }

        /// <summary>
        /// 条件成立时添加 Where Exists。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereExists(bool condition, string sql, params object[] args)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereExists(condition, sql, args);
        }

        /// <summary>
        /// 条件成立时添加 Where Not Exists。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereNotExists(bool condition, string sql, params object[] args)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereNotExists(condition, sql, args);
        }

        /// <summary>
        /// Where {column} In (values)。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereIn<TValue>(string column, IEnumerable<TValue> values)
        {
            return (IDeleteQueryProvider<T>)base.WhereIn(column, values);
        }

        /// <summary>
        /// Where {field} In (values)。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereIn<TValue>(Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return (IDeleteQueryProvider<T>)base.WhereIn(field, values);
        }

        /// <summary>
        /// 条件成立时添加 Where In（字符串列名版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereIn<TValue>(bool condition, string column, IEnumerable<TValue> values)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereIn(condition, column, values);
        }

        /// <summary>
        /// 条件成立时添加 Where In（表达式版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereIn<TValue>(bool condition, Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereIn(condition, field, values);
        }

        /// <summary>
        /// Where {column} Like '%pattern%'。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereLike(string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereLike(column, pattern);
        }

        /// <summary>
        /// Where {field} Like '%pattern%'。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereLike<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereLike(field, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like（字符串列名版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereLike(bool condition, string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereLike(condition, column, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like（表达式版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereLike<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereLike(condition, field, pattern);
        }

        /// <summary>
        /// Where {column} Not In (values)。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereNotIn<TValue>(string column, IEnumerable<TValue> values)
        {
            return (IDeleteQueryProvider<T>)base.WhereNotIn(column, values);
        }

        /// <summary>
        /// Where {field} Not In (values)。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereNotIn<TValue>(Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return (IDeleteQueryProvider<T>)base.WhereNotIn(field, values);
        }

        /// <summary>
        /// 条件成立时添加 Where Not In（字符串列名版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereNotIn<TValue>(bool condition, string column, IEnumerable<TValue> values)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereNotIn(condition, column, values);
        }

        /// <summary>
        /// 条件成立时添加 Where Not In（表达式版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereNotIn<TValue>(bool condition, Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereNotIn(condition, field, values);
        }

        /// <summary>
        /// Where {column} Like 'pattern%'（前缀匹配）。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereLikeStart(string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereLikeStart(column, pattern);
        }

        /// <summary>
        /// Where {field} Like 'pattern%'（前缀匹配）。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereLikeStart<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereLikeStart(field, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like Start（字符串列名版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereLikeStart(bool condition, string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereLikeStart(condition, column, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like Start（表达式版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereLikeStart<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereLikeStart(condition, field, pattern);
        }

        /// <summary>
        /// Where {column} Like '%pattern'（后缀匹配）。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereLikeEnd(string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereLikeEnd(column, pattern);
        }

        /// <summary>
        /// Where {field} Like '%pattern'（后缀匹配）。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereLikeEnd<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereLikeEnd(field, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like End（字符串列名版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereLikeEnd(bool condition, string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereLikeEnd(condition, column, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like End（表达式版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereLikeEnd<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereLikeEnd(condition, field, pattern);
        }

        /// <summary>
        /// Where {column} Not Like '%pattern%'。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereNotLike(string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereNotLike(column, pattern);
        }

        /// <summary>
        /// Where {field} Not Like '%pattern%'。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereNotLike<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereNotLike(field, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like（字符串列名版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereNotLike(bool condition, string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereNotLike(condition, column, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like（表达式版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereNotLike<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereNotLike(condition, field, pattern);
        }

        /// <summary>
        /// Where {column} Not Like 'pattern%'（前缀匹配）。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereNotLikeStart(string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereNotLikeStart(column, pattern);
        }

        /// <summary>
        /// Where {field} Not Like 'pattern%'（前缀匹配）。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereNotLikeStart<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereNotLikeStart(field, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like Start（字符串列名版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereNotLikeStart(bool condition, string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereNotLikeStart(condition, column, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like Start（表达式版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereNotLikeStart<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereNotLikeStart(condition, field, pattern);
        }

        /// <summary>
        /// Where {column} Not Like '%pattern'（后缀匹配）。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereNotLikeEnd(string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereNotLikeEnd(column, pattern);
        }

        /// <summary>
        /// Where {field} Not Like '%pattern'（后缀匹配）。
        /// </summary>
        public new IDeleteQueryProvider<T> WhereNotLikeEnd<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.WhereNotLikeEnd(field, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like End（字符串列名版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereNotLikeEnd(bool condition, string column, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereNotLikeEnd(condition, column, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like End（表达式版本）。
        /// </summary>
        public new IDeleteQueryProvider<T> IfTrueWhereNotLikeEnd<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return (IDeleteQueryProvider<T>)base.IfTrueWhereNotLikeEnd(condition, field, pattern);
        }

        #endregion 动态条件便捷方法

    }

    /// <summary>
    /// 异步删除查询器。
    /// </summary>
    /// <typeparam name="T">删除的实体类型。</typeparam>
    public class AsyncDeleteQueryProvider<T> : IAsyncDeleteQueryProvider<T>
    {
        /// <summary>
        /// 数据库实例。
        /// </summary>
        protected readonly IDatabase _database;
        /// <summary>
        /// SQL 表达式。
        /// </summary>
        protected ISqlExpression<T> _sqlExpression;

        /// <summary>
        /// 使用数据库初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        public AsyncDeleteQueryProvider(IDatabase database)
        {
            _database = database;
            _sqlExpression = database.DatabaseType.ExpressionVisitor<T>(database, database.PocoDataFactory.ForType(typeof(T)), false);
        }

        /// <summary>
        /// 添加删除条件。
        /// </summary>
        /// <param name="whereExpression">删除条件表达式。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression)
        {
            _sqlExpression = _sqlExpression.Where(whereExpression);
            return this;
        }

        #region 动态条件便捷方法

        /// <summary>
        /// 条件成立时添加删除条件。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="predicate">筛选条件表达式。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhere(bool condition, Expression<Func<T, bool>> predicate)
        {
            if (condition && predicate != null)
                Where(predicate);
            return this;
        }

        /// <summary>
        /// Where Exists（自动添加 "EXISTS(" 与 "SELECT * " 前缀）。
        /// </summary>
        /// <param name="sql">子查询 SQL 或表名/过滤条件。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereExists(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException(nameof(sql));
            WhereSql(SqlConditionHelper.BuildExistsSql(sql), args);
            return this;
        }

        /// <summary>
        /// Where Not Exists（自动添加 "NOT EXISTS(" 前缀）。
        /// </summary>
        /// <param name="sql">子查询 SQL 或表名/过滤条件。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereNotExists(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException(nameof(sql));
            WhereSql("NOT " + SqlConditionHelper.BuildExistsSql(sql), args);
            return this;
        }

        /// <summary>
        /// 条件成立时添加 Where Exists。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="sql">子查询 SQL 或表名/过滤条件。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereExists(bool condition, string sql, params object[] args)
        {
            return condition ? WhereExists(sql, args) : this;
        }

        /// <summary>
        /// 条件成立时添加 Where Not Exists。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="sql">子查询 SQL 或表名/过滤条件。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereNotExists(bool condition, string sql, params object[] args)
        {
            return condition ? WhereNotExists(sql, args) : this;
        }

        /// <summary>
        /// Where {column} In (values)。空集合生成 1=2，单值生成等值判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="column">列名（可带别名，如 "t0.Age"）。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereIn<TValue>(string column, IEnumerable<TValue> values)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            SqlConditionHelper.ApplyWhereIn(WhereSql, column, values, _database.DatabaseType);
            return this;
        }

        /// <summary>
        /// Where {field} In (values)。空集合生成 1=2，单值生成等值判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Age。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereIn<TValue>(Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            SqlConditionHelper.ApplyWhereIn(WhereSql, SqlConditionHelper.GetFieldName(field), values, _database.DatabaseType);
            return this;
        }

        /// <summary>
        /// 条件成立时添加 Where In（字符串列名版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereIn<TValue>(bool condition, string column, IEnumerable<TValue> values)
        {
            return condition ? WhereIn(column, values) : this;
        }

        /// <summary>
        /// 条件成立时添加 Where In（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereIn<TValue>(bool condition, Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return condition ? WhereIn(field, values) : this;
        }

        /// <summary>
        /// Where {column} Like '%pattern%'。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加前后 %）。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereLike(string column, string pattern)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            if (string.IsNullOrEmpty(pattern)) return this;
            WhereSql($"{SqlConditionHelper.EscapeColumnName(column, _database.DatabaseType)} LIKE @0 ESCAPE {_database.DatabaseType.LikeEscapeLiteral}", $"%{SqlConditionHelper.EscapeLikePattern(pattern)}%");
            return this;
        }

        /// <summary>
        /// Where {field} Like '%pattern%'。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Name。</param>
        /// <param name="pattern">匹配内容（自动加前后 %）。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereLike<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return WhereLike(SqlConditionHelper.GetFieldName(field), pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like（字符串列名版本）。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereLike(bool condition, string column, string pattern)
        {
            return condition ? WhereLike(column, pattern) : this;
        }

        /// <summary>
        /// 条件成立时添加 Where Like（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereLike<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return condition ? WhereLike(field, pattern) : this;
        }

        /// <summary>
        /// Where {column} Not In (values)。空集合生成 1=1，单值生成不等于判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="column">列名（可带别名，如 "t0.Age"）。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereNotIn<TValue>(string column, IEnumerable<TValue> values)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            SqlConditionHelper.ApplyWhereNotIn(WhereSql, column, values, _database.DatabaseType);
            return this;
        }

        /// <summary>
        /// Where {field} Not In (values)。空集合生成 1=1，单值生成不等于判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Age。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereNotIn<TValue>(Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            SqlConditionHelper.ApplyWhereNotIn(WhereSql, SqlConditionHelper.GetFieldName(field), values, _database.DatabaseType);
            return this;
        }

        /// <summary>
        /// 条件成立时添加 Where Not In（字符串列名版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereNotIn<TValue>(bool condition, string column, IEnumerable<TValue> values)
        {
            return condition ? WhereNotIn(column, values) : this;
        }

        /// <summary>
        /// 条件成立时添加 Where Not In（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereNotIn<TValue>(bool condition, Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return condition ? WhereNotIn(field, values) : this;
        }

        /// <summary>
        /// Where {column} Like 'pattern%'（前缀匹配）。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加后缀 %）。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereLikeStart(string column, string pattern)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            if (string.IsNullOrEmpty(pattern)) return this;
            WhereSql($"{SqlConditionHelper.EscapeColumnName(column, _database.DatabaseType)} LIKE @0 ESCAPE {_database.DatabaseType.LikeEscapeLiteral}", $"{SqlConditionHelper.EscapeLikePattern(pattern)}%");
            return this;
        }

        /// <summary>
        /// Where {field} Like 'pattern%'（前缀匹配）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereLikeStart<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return WhereLikeStart(SqlConditionHelper.GetFieldName(field), pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like Start（字符串列名版本）。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereLikeStart(bool condition, string column, string pattern)
        {
            return condition ? WhereLikeStart(column, pattern) : this;
        }

        /// <summary>
        /// 条件成立时添加 Where Like Start（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereLikeStart<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return condition ? WhereLikeStart(field, pattern) : this;
        }

        /// <summary>
        /// Where {column} Like '%pattern'（后缀匹配）。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加前缀 %）。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereLikeEnd(string column, string pattern)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            if (string.IsNullOrEmpty(pattern)) return this;
            WhereSql($"{SqlConditionHelper.EscapeColumnName(column, _database.DatabaseType)} LIKE @0 ESCAPE {_database.DatabaseType.LikeEscapeLiteral}", $"%{SqlConditionHelper.EscapeLikePattern(pattern)}");
            return this;
        }

        /// <summary>
        /// Where {field} Like '%pattern'（后缀匹配）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereLikeEnd<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return WhereLikeEnd(SqlConditionHelper.GetFieldName(field), pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like End（字符串列名版本）。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereLikeEnd(bool condition, string column, string pattern)
        {
            return condition ? WhereLikeEnd(column, pattern) : this;
        }

        /// <summary>
        /// 条件成立时添加 Where Like End（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereLikeEnd<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return condition ? WhereLikeEnd(field, pattern) : this;
        }

        /// <summary>
        /// Where {column} Not Like '%pattern%'。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加前后 %）。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereNotLike(string column, string pattern)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            if (string.IsNullOrEmpty(pattern)) return this;
            WhereSql($"{SqlConditionHelper.EscapeColumnName(column, _database.DatabaseType)} NOT LIKE @0 ESCAPE {_database.DatabaseType.LikeEscapeLiteral}", $"%{SqlConditionHelper.EscapeLikePattern(pattern)}%");
            return this;
        }

        /// <summary>
        /// Where {field} Not Like '%pattern%'。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Name。</param>
        /// <param name="pattern">匹配内容（自动加前后 %）。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereNotLike<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return WhereNotLike(SqlConditionHelper.GetFieldName(field), pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like（字符串列名版本）。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereNotLike(bool condition, string column, string pattern)
        {
            return condition ? WhereNotLike(column, pattern) : this;
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereNotLike<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return condition ? WhereNotLike(field, pattern) : this;
        }

        /// <summary>
        /// Where {column} Not Like 'pattern%'（前缀匹配）。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加后缀 %）。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereNotLikeStart(string column, string pattern)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            if (string.IsNullOrEmpty(pattern)) return this;
            WhereSql($"{SqlConditionHelper.EscapeColumnName(column, _database.DatabaseType)} NOT LIKE @0 ESCAPE {_database.DatabaseType.LikeEscapeLiteral}", $"{SqlConditionHelper.EscapeLikePattern(pattern)}%");
            return this;
        }

        /// <summary>
        /// Where {field} Not Like 'pattern%'（前缀匹配）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Name。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereNotLikeStart<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return WhereNotLikeStart(SqlConditionHelper.GetFieldName(field), pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like Start（字符串列名版本）。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereNotLikeStart(bool condition, string column, string pattern)
        {
            return condition ? WhereNotLikeStart(column, pattern) : this;
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like Start（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereNotLikeStart<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return condition ? WhereNotLikeStart(field, pattern) : this;
        }

        /// <summary>
        /// Where {column} Not Like '%pattern'（后缀匹配）。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加前缀 %）。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereNotLikeEnd(string column, string pattern)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            if (string.IsNullOrEmpty(pattern)) return this;
            WhereSql($"{SqlConditionHelper.EscapeColumnName(column, _database.DatabaseType)} NOT LIKE @0 ESCAPE {_database.DatabaseType.LikeEscapeLiteral}", $"%{SqlConditionHelper.EscapeLikePattern(pattern)}");
            return this;
        }

        /// <summary>
        /// Where {field} Not Like '%pattern'（后缀匹配）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Name。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> WhereNotLikeEnd<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return WhereNotLikeEnd(SqlConditionHelper.GetFieldName(field), pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like End（字符串列名版本）。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereNotLikeEnd(bool condition, string column, string pattern)
        {
            return condition ? WhereNotLikeEnd(column, pattern) : this;
        }

        /// <summary>
        /// 条件成立时添加 Where Not Like End（表达式版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前删除查询器。</returns>
        public IAsyncDeleteQueryProvider<T> IfTrueWhereNotLikeEnd<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return condition ? WhereNotLikeEnd(field, pattern) : this;
        }

        private void WhereSql(string sql, params object[] args)
        {
            _sqlExpression = _sqlExpression.Where(sql, args);
        }

        #endregion 动态条件便捷方法

        /// <summary>
        /// 异步执行删除。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> Execute_Async(CancellationToken cancellationToken = default)
        {
            return _database.ExecuteAsync(_sqlExpression.Context.ToDeleteStatement(), _sqlExpression.Context.Params, cancellationToken);
        }
    }
}