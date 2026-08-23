using System;

namespace ToolGood.ReadyGo.NPoco
{
    public interface IColumnSerializer
    {
        object Serialize(object value);
        object Deserialize(object value, Type targetType);
    }
}