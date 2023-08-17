using Newtonsoft.Json;
using TerracoreMate.Hive.Converters;

namespace TerracoreMate.Hive.Models;

public class Authority
{
    public uint weight_threshold;
    [JsonConverter(typeof(AccountAuthsJsonConverter))] 
    public Dictionary<string, ushort> account_auths = new Dictionary<string, ushort>();

    [JsonConverter(typeof(KeyAuthsJsonConverter))] 
    public Dictionary<PublicKey, ushort> key_auths = new Dictionary<PublicKey, ushort>();
}