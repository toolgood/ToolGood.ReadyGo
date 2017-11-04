using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class SqlBuilder : IDataSet
    {
        public int Delete()
        {
            var sql = ((ISqlBuilderConvert)this).GetFullDeleteSql(_provider);
            return _sqlHelper.Delete(sql);
        }

        public object Insert(bool returnInsertId = true)
        {
            var sql = ((ISqlBuilderConvert)this).GetFullInsertSql(_provider);
            if (returnInsertId) {
                var pk = ((ITableConvert) _tables[0]).GetPrimaryKey();
                if (object.Equals(pk,null)) {
                    throw new NoPrimaryKeyException();
                }
                return _sqlHelper.Insert(sql, pk._columnName);
            }
            return _sqlHelper.Execute(sql);
        }

        public int Update()
        {
            var sql = ((ISqlBuilderConvert)this).GetFullUpdateSql(_provider);
            return _sqlHelper.Update(sql);
        }
    }
}
