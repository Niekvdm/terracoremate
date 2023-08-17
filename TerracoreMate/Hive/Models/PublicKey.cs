using Cryptography.ECDSA;
using Newtonsoft.Json;
using TerracoreMate.Hive.Converters;

namespace TerracoreMate.Hive.Models;

[JsonConverter(typeof(PublicKeyJsonConverter))] public class PublicKey
{
    public string key;

    public PublicKey(string strKey )
    {
        key = strKey;
    }
    public override string ToString()
    {
        return key;
    }
    public byte[] Decode()
    {
        return Base58.RemoveCheckSum(Base58.Decode(key.Substring(3)));
    }
}