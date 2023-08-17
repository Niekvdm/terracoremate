using Newtonsoft.Json;
using TerracoreMate.Hive.Operations;

namespace TerracoreMate.Hive.Models;

public class Transaction
{
    public TransactionBody tx;
    public string txid;
}

public class TransactionBody
{
    public ushort ref_block_num;
    public uint ref_block_prefix;
    public DateTime expiration;
    public object[] operations;
    public object[] extensions = Array.Empty<object>();
    public string[] signatures = Array.Empty<string>();

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this).Replace("operations\":[{", "operations\":[[\"custom_json\",{")
            .Replace(",\"opid\":18}", "}]");
    }

    public static CustomJson CreateCustomJson(string id, string username, bool isConfidential, object data)
    {
        CustomJson customJsonOperation = new()
        {
            required_auths = isConfidential ? new[] { username } : Array.Empty<string>(),
            required_posting_auths = !isConfidential ? new[] { username } : Array.Empty<string>(),
            id = id,
            json = JsonConvert.SerializeObject(data)
        };

        return customJsonOperation;
    }
}