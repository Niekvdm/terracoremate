using Newtonsoft.Json;
using TerracoreMate.Hive.Models;

namespace TerracoreMate.Hive.Converters;

public class OpJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        throw new NotImplementedException();
    }
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var op = (Op)value;
        writer.WriteStartArray();
        writer.WriteValue(op.name);
        serializer.Serialize(writer,op.payload);
        writer.WriteEndArray();
    }
}