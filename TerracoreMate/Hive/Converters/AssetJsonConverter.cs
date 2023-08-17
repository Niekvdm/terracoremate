using Newtonsoft.Json;

namespace TerracoreMate.Hive.Converters;

public class AssetJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
    public override bool CanConvert(Type objectType)
    {
        throw new NotImplementedException();
    }
}