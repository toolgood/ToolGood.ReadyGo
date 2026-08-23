using System;

namespace ToolGood.ReadyGo.NPoco
{
    public class FastJsonColumnSerializer : IColumnSerializer
    {
        public fastJSON.JSONParameters JSONParameters { get; set; } = new fastJSON.JSONParameters()
        {
            UseUTCDateTime = false,
            UseExtensions = false,
            UseFastGuid = false
        };

        public object Serialize(object value)
        {
            var serializer = new fastJSON.JSONSerializer(JSONParameters);
            return serializer.ConvertToJSON(value);
        }

        public object Deserialize(object value, Type targetType)
        {
            var deserializer = new fastJSON.Deserializer(JSONParameters);
            return deserializer.ToObject(value as string ?? value?.ToString(), targetType);
        }
    }
}