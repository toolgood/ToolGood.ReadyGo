using System.Collections.Generic;
using ToolGood.ReadyGo.NPoco.Expressions;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    /// <summary>
    /// 查询上下文，用于 WhereSql 自定义查询构建。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public class QueryContext<T>
    {
        private readonly IDatabase _database;
        private readonly PocoData _pocoData;
        private readonly Dictionary<string, JoinData> _joinExpressions;

        /// <summary>
        /// 使用数据库、Poco 元数据与关联表达式初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pocoData">实体的 Poco 元数据。</param>
        /// <param name="joinExpressions">关联查询表达式集合。</param>
        public QueryContext(IDatabase database, PocoData pocoData, Dictionary<string, JoinData> joinExpressions)
        {
            _database = database;
            _pocoData = pocoData;
            _joinExpressions = joinExpressions;
        }

        /// <summary>
        /// 数据库类型。
        /// </summary>
        public IDatabaseType DatabaseType
        {
            get { return _database.DatabaseType; }
        }

        /// <summary>
        /// 实体的 Poco 元数据。
        /// </summary>
        public PocoData PocoData => _pocoData;
    }
}