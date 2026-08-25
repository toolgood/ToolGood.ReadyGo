using System;
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

        /// <summary>
        /// 异步执行删除。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> Execute_Async(CancellationToken cancellationToken = default)
        {
            return base.Execute(cancellationToken);
        }

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

        /// <summary>
        /// 异步执行删除。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> Execute(CancellationToken cancellationToken = default)
        {
            return _database.ExecuteAsync(_sqlExpression.Context.ToDeleteStatement(), _sqlExpression.Context.Params, cancellationToken);
        }
    }
}