using System;
using System.Data.Common;

namespace ToolGood.ReadyGo.NPoco
{
    public class NullFastCreate : IFastCreate
    {
        public object Create(DbDataReader dataReader)
        {
            throw new NotImplementedException();
        }
    }
}