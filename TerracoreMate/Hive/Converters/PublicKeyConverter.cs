using Newtonsoft.Json;
using TerracoreMate.Hive.Models;

namespace TerracoreMate.Hive.Converters;

public class PublicKeyJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return new PublicKey((string)reader.Value);
    }
    public override bool CanConvert(Type objectType)
    {
        throw new NotImplementedException();
    }
}