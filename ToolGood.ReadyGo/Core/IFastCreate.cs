using System.Data.Common;

namespace ToolGood.ReadyGo.NPoco
{
    public interface IFastCreate
    {
        object Create(DbDataReader dataReader);
    }
}