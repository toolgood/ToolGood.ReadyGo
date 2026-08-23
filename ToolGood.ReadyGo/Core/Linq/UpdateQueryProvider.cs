using System;
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
        public Task<int> ExecuteAsync(T obj, CancellationToken cancellationToken = default)
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
