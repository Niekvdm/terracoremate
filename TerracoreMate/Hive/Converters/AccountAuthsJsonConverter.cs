using Newtonsoft.Json;
using TerracoreMate.Hive.Models;

namespace TerracoreMate.Hive.Converters;

public class AccountAuthsJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var auths = (Dictionary<string, ushort>)value;

        writer.WriteStartArray();
        foreach (var auth in auths)
        {
            writer.WriteStartArray();
            writer.WriteValue(auth.Key);
            writer.WriteValue(auth.Value);
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
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