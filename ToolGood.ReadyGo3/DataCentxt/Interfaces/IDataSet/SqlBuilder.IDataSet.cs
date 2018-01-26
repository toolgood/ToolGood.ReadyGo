using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ToolGood.ReadyGo3.DataCentxt.Exceptions;

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class SqlBuilder //: IDataSet
    {
        public int Delete()
        {
            var sql = (this).GetFullDeleteSql(Provider);
            return GetSqlHelper().Execute(sql);
        }

        public object Insert(bool returnInsertId = false)
        {
            var sql = (this).GetFullInsertSql(Provider);
            if (returnInsertId) {
                var pk = (_tables[0]).GetPrimaryKey();
                if (object.Equals(pk, null)) {
                    throw new NoPrimaryKeyException();
                }
                return GetSqlHelper().Insert(sql, pk._columnName);
            }
            return GetSqlHelper().Execute(sql);
        }

        public int Update()
        {
            var sql = (this).GetFullUpdateSql(Provider);
            return GetSqlHelper().Execute(sql);
        }
    }
}
