using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ToolGood.ReadyGo.NPoco.Expressions;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 同步更新查询器。
    /// </summary>
    /// <typeparam name="T">更新的实体类型。</typeparam>
    public class UpdateQueryProvider<T> : AsyncUpdateQueryProvider<T>, IUpdateQueryProvider<T>
    {
        /// <summary>
        /// 使用数据库初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        public UpdateQueryProvider(IDatabase database) : base(database)
        {
        }

        /// <summary>
        /// 添加更新条件。
        /// </summary>
        /// <param name="whereExpression">更新条件表达式。</param>
        /// <returns>当前更新查询器。</returns>
        public new IUpdateQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression)
        {
            return (IUpdateQueryProvider<T>) base.Where(whereExpression);
        }

        /// <summary>
        /// 排除默认值字段。
        /// </summary>
        /// <returns>当前更新查询器。</returns>
        public new IUpdateQueryProvider<T> ExcludeDefaults()
        {
            return (IUpdateQueryProvider<T>)base.ExcludeDefaults();
        }

        /// <summary>
        /// 仅更新指定字段。
        /// </summary>
        /// <param name="onlyFields">字段选择器。</param>
        /// <returns>当前更新查询器。</returns>
        public new IUpdateQueryProvider<T> OnlyFields(Expression<Func<T, object>> onlyFields)
        {
            return (IUpdateQueryProvider<T>)base.OnlyFields(onlyFields);
        }

        #region 动态条件便捷方法

        /// <summary>
        /// 条件成立时添加更新条件。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhere(bool condition, Expression<Func<T, bool>> predicate)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhere(condition, predicate);
        }

        /// <summary>
        /// Where Exists（自动添加 "EXISTS(" 与 "SELECT * " 前缀）。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereExists(string sql, params object[] args)
        {
            return (IUpdateQueryProvider<T>)base.WhereExists(sql, args);
        }

        /// <summary>
        /// Where Not Exists（自动添加 "NOT EXISTS(" 前缀）。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereNotExists(string sql, params object[] args)
        {
            return (IUpdateQueryProvider<T>)base.WhereNotExists(sql, args);
        }

        /// <summary>
        /// 条件成立时添加 Where Exists。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereExists(bool condition, string sql, params object[] args)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereExists(condition, sql, args);
        }

        /// <summary>
        /// 条件成立时添加 Where Not Exists。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereNotExists(bool condition, string sql, params object[] args)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereNotExists(condition, sql, args);
        }

        /// <summary>
        /// Where {column} In (values)。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereIn<TValue>(string column, IEnumerable<TValue> values)
        {
            return (IUpdateQueryProvider<T>)base.WhereIn(column, values);
        }

        /// <summary>
        /// Where {field} In (values)。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereIn<TValue>(Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return (IUpdateQueryProvider<T>)base.WhereIn(field, values);
        }

        /// <summary>
        /// 条件成立时添加 Where In（字符串列名版本）。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereIn<TValue>(bool condition, string column, IEnumerable<TValue> values)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereIn(condition, column, values);
        }

        /// <summary>
        /// 条件成立时添加 Where In（表达式版本）。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereIn<TValue>(bool condition, Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereIn(condition, field, values);
        }

        /// <summary>
        /// Where {column} Like '%pattern%'。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereLike(string column, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.WhereLike(column, pattern);
        }

        /// <summary>
        /// Where {field} Like '%pattern%'。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereLike<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.WhereLike(field, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like（字符串列名版本）。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereLike(bool condition, string column, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereLike(condition, column, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like（表达式版本）。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereLike<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereLike(condition, field, pattern);
        }

        /// <summary>
        /// Where {column} Not In (values)。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereNotIn<TValue>(string column, IEnumerable<TValue> values)
        {
            return (IUpdateQueryProvider<T>)base.WhereNotIn(column, values);
        }

        /// <summary>
        /// Where {field} Not In (values)。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereNotIn<TValue>(Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return (IUpdateQueryProvider<T>)base.WhereNotIn(field, values);
        }

        /// <summary>
        /// 条件成立时添加 Where Not In（字符串列名版本）。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereNotIn<TValue>(bool condition, string column, IEnumerable<TValue> values)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereNotIn(condition, column, values);
        }

        /// <summary>
        /// 条件成立时添加 Where Not In（表达式版本）。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereNotIn<TValue>(bool condition, Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereNotIn(condition, field, values);
        }

        /// <summary>
        /// Where {column} Like '%pattern'（右匹配）。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereLikeStart(string column, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.WhereLikeStart(column, pattern);
        }

        /// <summary>
        /// Where {field} Like '%pattern'（右匹配）。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereLikeStart<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.WhereLikeStart(field, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like Start（字符串列名版本）。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereLikeStart(bool condition, string column, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereLikeStart(condition, column, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like Start（表达式版本）。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereLikeStart<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereLikeStart(condition, field, pattern);
        }

        /// <summary>
        /// Where {column} Like 'pattern%'（左匹配）。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereLikeEnd(string column, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.WhereLikeEnd(column, pattern);
        }

        /// <summary>
        /// Where {field} Like 'pattern%'（左匹配）。
        /// </summary>
        public new IUpdateQueryProvider<T> WhereLikeEnd<TValue>(Expression<Func<T, TValue>> field, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.WhereLikeEnd(field, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like End（字符串列名版本）。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereLikeEnd(bool condition, string column, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereLikeEnd(condition, column, pattern);
        }

        /// <summary>
        /// 条件成立时添加 Where Like End（表达式版本）。
        /// </summary>
        public new IUpdateQueryProvider<T> IfTrueWhereLikeEnd<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return (IUpdateQueryProvider<T>)base.IfTrueWhereLikeEnd(condition, field, pattern);
        }

        #endregion 动态条件便捷方法

#pragma warning disable CS0109
        /// <summary>
        /// 执行更新。
        /// </summary>
        /// <param name="obj">待更新的实体。</param>
        /// <returns>受影响的行数。</returns>
        public new int Execute(T obj)
        {
            var updateStatement = _sqlExpression.Context.ToUpdateStatement(obj, _excludeDefaults, _onlyFields);
            return _database.Execute(updateStatement, _sqlExpression.Context.Params);
        }
#pragma warning restore CS0109

        /// <summary>
        /// 异步执行更新。
        /// </summary>
        /// <param name="obj">待更新的实体。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> Execute_Async(T obj, CancellationToken cancellationToken = default)
        {
            return base.Execute(obj, cancellationToken);
        }
    }

    /// <summary>
    /// 异步更新查询器。
    /// </summary>
    /// <typeparam name="T">更新的实体类型。</typeparam>
    public class AsyncUpdateQueryProvider<T> : IAsyncUpdateQueryProvider<T>
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
        /// 是否排除默认值字段。
        /// </summary>
        protected bool _excludeDefaults;
        /// <summary>
        /// 是否仅更新指定字段。
        /// </summary>
        protected bool _onlyFields;

        /// <summary>
        /// 使用数据库初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        public AsyncUpdateQueryProvider(IDatabase database)
        {
            _database = database;
            _sqlExpression = database.DatabaseType.ExpressionVisitor<T>(database, database.PocoDataFactory.ForType(typeof(T)), false);
        }

        /// <summary>
        /// 添加更新条件。
        /// </summary>
        /// <param name="whereExpression">更新条件表达式。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> Where(Expression<Func<T, bool>> whereExpression)
        {
            _sqlExpression = _sqlExpression.Where(whereExpression);
            return this;
        }

        #region 动态条件便捷方法

        /// <summary>
        /// 条件成立时添加更新条件。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="predicate">筛选条件表达式。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhere(bool condition, Expression<Func<T, bool>> predicate)
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
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereExists(string sql, params object[] args)
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
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereNotExists(string sql, params object[] args)
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
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereExists(bool condition, string sql, params object[] args)
        {
            return condition ? WhereExists(sql, args) : this;
        }

        /// <summary>
        /// 条件成立时添加 Where Not Exists。
        /// </summary>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="sql">子查询 SQL 或表名/过滤条件。</param>
        /// <param name="args">SQL 参数。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereNotExists(bool condition, string sql, params object[] args)
        {
            return condition ? WhereNotExists(sql, args) : this;
        }

        /// <summary>
        /// Where {column} In (values)。空集合生成 1=2，单值生成等值判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="column">列名（可带别名，如 "t0.Age"）。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereIn<TValue>(string column, IEnumerable<TValue> values)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            SqlConditionHelper.ApplyWhereIn(WhereSql, column, values);
            return this;
        }

        /// <summary>
        /// Where {field} In (values)。空集合生成 1=2，单值生成等值判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Age。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereIn<TValue>(Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            SqlConditionHelper.ApplyWhereIn(WhereSql, SqlConditionHelper.GetFieldName(field), values);
            return this;
        }

        /// <summary>
        /// 条件成立时添加 Where In（字符串列名版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereIn<TValue>(bool condition, string column, IEnumerable<TValue> values)
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
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereIn<TValue>(bool condition, Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return condition ? WhereIn(field, values) : this;
        }

        /// <summary>
        /// Where {column} Like '%pattern%'。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加前后 %）。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereLike(string column, string pattern)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            if (string.IsNullOrEmpty(pattern)) return this;
            WhereSql($"{column} LIKE @0", $"%{pattern}%");
            return this;
        }

        /// <summary>
        /// Where {field} Like '%pattern%'。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Name。</param>
        /// <param name="pattern">匹配内容（自动加前后 %）。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereLike<TValue>(Expression<Func<T, TValue>> field, string pattern)
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
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereLike(bool condition, string column, string pattern)
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
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereLike<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return condition ? WhereLike(field, pattern) : this;
        }

        /// <summary>
        /// Where {column} Not In (values)。空集合生成 1=1，单值生成不等于判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="column">列名（可带别名，如 "t0.Age"）。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereNotIn<TValue>(string column, IEnumerable<TValue> values)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            SqlConditionHelper.ApplyWhereNotIn(WhereSql, column, values);
            return this;
        }

        /// <summary>
        /// Where {field} Not In (values)。空集合生成 1=1，单值生成不等于判断。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式，如 x =&gt; x.Age。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereNotIn<TValue>(Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            SqlConditionHelper.ApplyWhereNotIn(WhereSql, SqlConditionHelper.GetFieldName(field), values);
            return this;
        }

        /// <summary>
        /// 条件成立时添加 Where Not In（字符串列名版本）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="condition">条件开关，为 true 时生效。</param>
        /// <param name="column">列名。</param>
        /// <param name="values">值集合。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereNotIn<TValue>(bool condition, string column, IEnumerable<TValue> values)
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
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereNotIn<TValue>(bool condition, Expression<Func<T, TValue>> field, IEnumerable<TValue> values)
        {
            return condition ? WhereNotIn(field, values) : this;
        }

        /// <summary>
        /// Where {column} Like '%pattern'（右匹配）。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加前缀 %）。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereLikeStart(string column, string pattern)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            if (string.IsNullOrEmpty(pattern)) return this;
            WhereSql($"{column} LIKE @0", $"%{pattern}");
            return this;
        }

        /// <summary>
        /// Where {field} Like '%pattern'（右匹配）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereLikeStart<TValue>(Expression<Func<T, TValue>> field, string pattern)
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
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereLikeStart(bool condition, string column, string pattern)
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
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereLikeStart<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return condition ? WhereLikeStart(field, pattern) : this;
        }

        /// <summary>
        /// Where {column} Like 'pattern%'（左匹配）。
        /// </summary>
        /// <param name="column">列名（可带别名，如 "t0.Name"）。</param>
        /// <param name="pattern">匹配内容（自动加后缀 %）。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereLikeEnd(string column, string pattern)
        {
            if (string.IsNullOrEmpty(column)) throw new ArgumentNullException(nameof(column));
            if (string.IsNullOrEmpty(pattern)) return this;
            WhereSql($"{column} LIKE @0", $"{pattern}%");
            return this;
        }

        /// <summary>
        /// Where {field} Like 'pattern%'（左匹配）。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="field">列表达式。</param>
        /// <param name="pattern">匹配内容。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> WhereLikeEnd<TValue>(Expression<Func<T, TValue>> field, string pattern)
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
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereLikeEnd(bool condition, string column, string pattern)
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
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> IfTrueWhereLikeEnd<TValue>(bool condition, Expression<Func<T, TValue>> field, string pattern)
        {
            return condition ? WhereLikeEnd(field, pattern) : this;
        }

        private void WhereSql(string sql, params object[] args)
        {
            _sqlExpression = _sqlExpression.Where(sql, args);
        }

        #endregion 动态条件便捷方法

        /// <summary>
        /// 排除默认值字段。
        /// </summary>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> ExcludeDefaults()
        {
            _excludeDefaults = true;
            return this;
        }

        /// <summary>
        /// 仅更新指定字段。
        /// </summary>
        /// <param name="onlyFields">字段选择器。</param>
        /// <returns>当前更新查询器。</returns>
        public IAsyncUpdateQueryProvider<T> OnlyFields(Expression<Func<T, object>> onlyFields)
        {
            _sqlExpression = _sqlExpression.Update(onlyFields);
            _onlyFields = true;
            return this;
        }

        /// <summary>
        /// 异步执行更新。
        /// </summary>
        /// <param name="obj">待更新的实体。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>受影响的行数。</returns>
        public async Task<int> Execute(T obj, CancellationToken cancellationToken = default)
        {
            var updateStatement = _sqlExpression.Context.ToUpdateStatement(obj, _excludeDefaults, _onlyFields);
            return await _database.ExecuteAsync(updateStatement, _sqlExpression.Context.Params, cancellationToken).ConfigureAwait(false);
        }
    }
}
