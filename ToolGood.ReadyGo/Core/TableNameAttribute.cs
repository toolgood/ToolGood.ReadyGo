using System;

namespace ToolGood.ReadyGo.NPoco
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string tableName)
        {
            Value = tableName;
        }
        public string Value { get; private set; }
    }
}