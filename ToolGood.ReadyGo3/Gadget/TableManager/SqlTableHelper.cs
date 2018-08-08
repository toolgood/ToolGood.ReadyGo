using System;

namespace ToolGood.ReadyGo3.Gadget.TableManager
{
    public class SqlTableHelper
    {
        private SqlHelper _sqlHelper;

        public SqlTableHelper(SqlHelper sqlhelper)
        {
            _sqlHelper = sqlhelper;
        }

        public string GetTryCreateTable(Type type)
        {
            var dp = DatabaseProvider.Resolve(_sqlHelper._sqlType);
            return dp.GetTryCreateTable(type);
        }
        public string GetCreateTable(Type type)
        {
            var dp = DatabaseProvider.Resolve(_sqlHelper._sqlType);
            return dp.GetCreateTable(type);
        }
        public string GetDropTable(Type type)
        {
            var dp = DatabaseProvider.Resolve(_sqlHelper._sqlType);
            return dp.GetDropTable(type);
        }
        public string GetTruncateTable(Type type)
        {
            var dp = DatabaseProvider.Resolve(_sqlHelper._sqlType);
            return dp.GetTruncateTable(type);
        }



        public void TryCreateTable(Type type)
        {
            var sql = GetTryCreateTable(type);
            _sqlHelper.Execute(sql);
        }

        public void CreateTable(Type type)
        {
            var sql = GetCreateTable(type);
            _sqlHelper.Execute(sql);
        }

        public void DropTable(Type type)
        {
            var sql = GetDropTable(type);
            _sqlHelper.Execute(sql);
        }

        public void TruncateTable(Type type)
        {
            var sql = GetTruncateTable(type);
            _sqlHelper.Execute(sql);
        }


    }
}
