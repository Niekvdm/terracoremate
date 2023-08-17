using Newtonsoft.Json;
using TerracoreMate.Hive.Converters;

namespace TerracoreMate.Hive.Models;

[JsonConverter(typeof(OpJsonConverter))] public class Op
{
    public string name;
    public object payload;
}