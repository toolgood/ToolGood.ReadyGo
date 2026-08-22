using System;

namespace ToolGood.ReadyGo.NPoco
{
    public interface IPocoDataFactory
    {
        PocoData ForType(Type type);
        TableInfo TableInfoForType(Type type);
        PocoData ForObject(object o, string primaryKeyName, bool autoIncrement);
    }
}