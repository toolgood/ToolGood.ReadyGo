using System.Collections.Generic;
using ToolGood.ReadyGo.NPoco.Expressions;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    public class QueryContext<T>
    {
        private readonly IDatabase _database;
        private readonly PocoData _pocoData;
        private readonly Dictionary<string, JoinData> _joinExpressions;

        public QueryContext(IDatabase database, PocoData pocoData, Dictionary<string, JoinData> joinExpressions)
        {
            _database = database;
            _pocoData = pocoData;
            _joinExpressions = joinExpressions;
        }

        public IDatabaseType DatabaseType
        {
            get { return _database.DatabaseType; }
        }

        public PocoData PocoData =>_database.PocoDataFactory.ForType(typeof(T));
    }
}